using System;
using System.Threading;
using BluffinMuffin.Server.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes;
using BluffinMuffin.Server.DataTypes.EventHandling;
using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Protocol.DataTypes.Enums;
using BluffinMuffin.Server.Logic.GameModules;
using Com.Ericmas001.Util;

namespace BluffinMuffin.Server.Logic
{
    /// <summary>
    /// This represent only one "Game" of Poker. This means from Putting Blinds to preflop, flop, turn, river and then declaring the winner.
    /// Tipically you would have many "Game" while sitting at a table, but players can sit-in and sit-out, so it's like a different game every "Game".
    /// </summary>
    public class PokerGame : IPokerGame
    {
        #region Fields

        // STATES
        private IGameModule m_CurrentModule;
        private GameStateEnum m_State; // L'etat global de la game
        #endregion Fields

        #region Properties
        public PokerGameObserver Observer { get; private set; }

        /// <summary>
        /// The Table Entity
        /// </summary>
        public TableInfo Table { get; private set; }
        /// <summary>
        /// The PokerTable Entity
        /// </summary>
        public PokerTable GameTable { get { return (PokerTable)Table; } }

        /// <summary>
        /// The Rules Entity
        /// </summary>
        public TableParams Params { get; private set; }

        /// <summary>
        /// Current Round of the Playing State
        /// </summary>
        public RoundTypeEnum Round
        {
            get { return Table.Round; }
        }

        /// <summary>
        /// Is the Game currently Running ? (Not Ended)
        /// </summary>
        public bool IsRunning
        {
            get { return m_State != GameStateEnum.End; }
        }

        /// <summary>
        /// Is the Game currently Running ? (Not Ended)
        /// </summary>
        public bool IsPlaying
        {
            get { return IsRunning && m_State >= GameStateEnum.WaitForBlinds; }
        }

        /// <summary>
        /// Current State of the Game
        /// </summary>
        public GameStateEnum State
        {
            get { return m_State; }
        }

        #endregion

        #region Ctors & Init

        public PokerGame(PokerTable table)
            : this(new TexasHoldemDealer(), table)
        {
        }

        private PokerGame(AbstractDealer dealer, PokerTable table)
        {
            Observer = new PokerGameObserver(this);
            table.Dealer = dealer;
            Table = table;
            Params = table.Params;
            m_State = GameStateEnum.Init;
        }
        #endregion Ctors & Init

        #region Public Methods
        /// <summary>
        /// Starts the Game. Only works in Init State
        /// </summary>
        public void Start()
        {
            if (IsInitializing)
                SetModule(new InitGameModule(Observer, GameTable));
        }

        /// <summary>
        /// Add a player to the table
        /// </summary>
        public bool JoinGame(PlayerInfo p)
        {
            if (m_State == GameStateEnum.Init || m_State == GameStateEnum.End)
            {
                LogManager.Log(LogLevel.Error, "PokerGame.JoinGame", "Can't join, bad timing: {0}", m_State);
                return false;
            }

            Observer.RaisePlayerJoined(p);
            return GameTable.JoinTable(p);
        }

        public int AfterPlayerSat(PlayerInfo p, int noSeat = -1, int moneyAmount = 1500)
        {
            var seat = p.NoSeat == -1 ? null : Table.Seats[p.NoSeat];
            if (seat != null && !seat.IsEmpty)
            {
                Observer.RaiseSeatUpdated(seat.Clone());

                if (m_CurrentModule != null)
                    m_CurrentModule.OnSitIn();
                if (m_State > GameStateEnum.WaitForPlayers)
                    GameTable.NewArrivals.Add(p);
                return p.NoSeat;
            }
            return -1;
        }

        private bool IsInitializing
        {
            get { return m_State == GameStateEnum.Init; }
        }

        public bool SitOut(PlayerInfo p)
        {
            var oldSeat = p.NoSeat;
            if (oldSeat == -1)
                return true;

            var blindNeeded = GameTable.GetBlindNeeded(p);

            p.State = PlayerStateEnum.Zombie;
            if (State == GameStateEnum.Playing && Table.CurrentPlayer == p)
                PlayMoney(p, -1);
            else if (blindNeeded > 0)
                PlayMoney(p, blindNeeded);

            if (Table.SeatsContainsPlayer(p) && Table.SitOut(p))
            {
                var seat = new SeatInfo()
                {
                    Player = null,
                    NoSeat = oldSeat,
                };
                Observer.RaiseSeatUpdated(seat);
                if (m_CurrentModule != null)
                    m_CurrentModule.OnSitOut();
                return true;
            }
            return false;
        }

        /// <summary>
        /// The player is leaving the game
        /// </summary>
        public void LeaveGame(PlayerInfo p)
        {
            var sitOutOk = SitOut(p);

            if (sitOutOk && Table.LeaveTable(p))
            {
                if (Table.Players.Count == 0)
                    m_State = GameStateEnum.End;
            }
        }

        /// <summary>
        /// The player is putting money in the game
        /// </summary>
        public bool PlayMoney(PlayerInfo p, int amount)
        {
            lock(Table)
            {
                var amnt = Math.Min(amount, p.MoneySafeAmnt);
                LogManager.Log(LogLevel.MessageLow, "PokerGame.PlayMoney", "{0} is playing {1} money on state: {2}", p.Name, amnt, m_State);

                if (m_CurrentModule != null)
                    return m_CurrentModule.OnMoneyPlayed(p, amount);

                LogManager.Log(LogLevel.Warning, "PokerGame.PlayMoney", "{0} played money but the game is not in the right state", p.Name);

                return false;
            }
        }
        #endregion

        #region Private Methods

        private void SetModule(IGameModule module)
        {
            m_CurrentModule = module;
            m_CurrentModule.ModuleCompleted += delegate(object sender, SuccessEventArg arg)
            {
                if (arg.Success)
                {
                    AdvanceToNextGameState();
                }
                else
                {
                    m_State = GameStateEnum.End;
                    Observer.RaiseEverythingEnded();
                }
            };
            m_CurrentModule.InitModule();
        }
        
        private void AdvanceToNextGameState()
        {
            if (m_State == GameStateEnum.End)
                return;

            m_State = (GameStateEnum)(((int)m_State) + 1);
            m_CurrentModule = null;

            switch (m_State)
            {
                case GameStateEnum.WaitForPlayers:
                    SetModule(new WaitForPlayerModule(Observer, GameTable));
                    break;
                case GameStateEnum.WaitForBlinds:
                    SetModule(new WaitForBlindsModule(Observer, GameTable));
                    break;
                case GameStateEnum.Playing:
                    SetModule(new PlayingModule(Observer, GameTable));
                    break;
                case GameStateEnum.Showdown:
                    ShowAllCards();
                    break;
                case GameStateEnum.DecideWinners:
                    DecideWinners();
                    break;
                case GameStateEnum.DistributeMoney:
                    DistributeMoney();
                    StartANewGame();
                    break;
                case GameStateEnum.End:
                    Observer.RaiseEverythingEnded();
                    break;
            }
        }
        private void StartANewGame()
        {
            Observer.RaiseGameEnded();
            m_State = GameStateEnum.Init;
            AdvanceToNextGameState();
        }
        private void ShowAllCards()
        {
            foreach (var p in Table.Players)
                if (p.IsPlaying || p.IsAllIn)
                {
                    p.IsShowingCards = true;
                    Observer.RaisePlayerHoleCardsChanged(p);
                }
            AdvanceToNextGameState(); //Advancing to DecideWinners State
        }
        private void DistributeMoney()
        {
            foreach (var pot in Table.Pots)
            {
                var players = pot.AttachedPlayers;
                if (players.Length > 0)
                {
                    var wonAmount = pot.Amount / players.Length;
                    if (wonAmount > 0)
                    {
                        foreach (var p in players)
                        {
                            p.Player.MoneySafeAmnt += wonAmount;
                            Observer.RaisePlayerWonPot(p, pot, wonAmount);
                            WaitALittle(Params.WaitingTimes.AfterPotWon);
                        }
                    }
                }
            }
        }
        private void DecideWinners()
        {
            GameTable.CleanPotsForWinning();
            AdvanceToNextGameState(); //Advancing to DistributeMoney State
        }
        private void WaitALittle(int waitingTime)
        {
            Thread.Sleep(waitingTime);
        }
        #endregion Private Methods
    }
}
