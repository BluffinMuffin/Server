using System;
using System.Collections.Generic;
using System.Linq;
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

        private IGameModule m_CurrentModule;
        private readonly Queue<IGameModule> m_Modules = new Queue<IGameModule>();

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
        /// Is the Game currently Running ? (Not Ended)
        /// </summary>
        public bool IsRunning
        {
            get { return State != GameStateEnum.End; }
        }

        /// <summary>
        /// Is the Game currently Running ? (Not Ended)
        /// </summary>
        public bool IsPlaying
        {
            get { return IsRunning && State >= GameStateEnum.WaitForBlinds; }
        }

        /// <summary>
        /// Current State of the Game
        /// </summary>
        public GameStateEnum State
        {
            get { return m_CurrentModule == null ? GameStateEnum.Init : m_CurrentModule.GameState; }
        }

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
        }
        #endregion Ctors & Init

        #region Public Methods
        /// <summary>
        /// Starts the Game. Only works in Init State
        /// </summary>
        public void Start()
        {
            if (IsInitializing)
                AdvanceToNextGameState();
        }

        /// <summary>
        /// Add a player to the table
        /// </summary>
        public bool JoinGame(PlayerInfo p)
        {
            if (IsInitializing || !IsRunning)
            {
                LogManager.Log(LogLevel.Error, "PokerGame.JoinGame", "Can't join, bad timing: {0}", State);
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
                if (State > GameStateEnum.WaitForPlayers)
                    GameTable.NewArrivals.Add(p);
                return p.NoSeat;
            }
            return -1;
        }

        private bool IsInitializing
        {
            get { return State == GameStateEnum.Init; }
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
                    SetModule(new EndGameModule(Observer,GameTable));
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
                LogManager.Log(LogLevel.MessageLow, "PokerGame.PlayMoney", "{0} is playing {1} money on state: {2}", p.Name, amnt, State);

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
                    SetModule(new EndGameModule(Observer,GameTable));
                }
            };
            m_CurrentModule.ModuleGenerated += (sender, arg) => m_Modules.Enqueue(arg.Module);
            m_CurrentModule.InitModule();
        }
        
        private void AdvanceToNextGameState()
        {
            if (!IsRunning)
                return;

            if (!m_Modules.Any())
            {
                Observer.RaiseGameEnded();
                m_Modules.Enqueue(new InitGameModule(Observer,GameTable));
            }
            SetModule(m_Modules.Dequeue());
        }
        #endregion Private Methods
    }
}
