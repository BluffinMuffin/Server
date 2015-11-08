using System;
using System.Collections.Generic;
using System.Linq;
using BluffinMuffin.Server.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes;
using BluffinMuffin.Server.DataTypes.EventHandling;
using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Protocol.DataTypes.Enums;
using BluffinMuffin.Server.Logic.GameModules;

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

        public PokerGameObserver Observer { get; }

        /// <summary>
        /// The Table Entity
        /// </summary>
        public PokerTable Table { get; }

        /// <summary>
        /// Is the Game currently Running ? (Not Ended)
        /// </summary>
        public bool IsRunning => State != GameStateEnum.End;

        private bool IsInitializing => State == GameStateEnum.Init;

        /// <summary>
        /// Is the Game currently Running ? (Not Ended)
        /// </summary>
        public bool IsPlaying => IsRunning && State >= GameStateEnum.WaitForBlinds;

        /// <summary>
        /// Current State of the Game
        /// </summary>
        public GameStateEnum State => m_CurrentModule?.GameState ?? GameStateEnum.Init;

        #region Ctors & Init

        public PokerGame(PokerTable table)
        {
            Observer = new PokerGameObserver(this);
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
                Logger.LogError("Can't join, bad timing: {0}", State);
                return false;
            }

            Observer.RaisePlayerJoined(p);
            return Table.JoinTable(p);
        }

        public int AfterPlayerSat(PlayerInfo p)
        {
            var seat = p.NoSeat == -1 ? null : Table.Seats[p.NoSeat];
            if (seat != null && !seat.IsEmpty)
            {
                if (State > GameStateEnum.WaitForPlayers)
                    Table.NewArrivals.Add(p);

                Observer.RaiseSeatUpdated(seat.Clone());

                m_CurrentModule?.OnSitIn();
                return p.NoSeat;
            }
            return -1;
        }

        public bool SitOut(PlayerInfo p)
        {
            var oldSeat = p.NoSeat;
            if (oldSeat == -1)
                return true;

            var blindNeeded = Table.GetBlindNeeded(p);

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
                m_CurrentModule?.OnSitOut();
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
                    SetModule(new EndGameModule(Observer,Table));
            }
        }

        /// <summary>
        /// The player is putting money in the game
        /// </summary>
        public bool PlayMoney(PlayerInfo p, int amount)
        {
            lock (Table)
            {
                var amnt = Math.Min(amount, p.MoneySafeAmnt);
                Logger.LogDebugInformation("{0} is playing {1} money on state: {2}", p.Name, amnt, State);

                if (m_CurrentModule != null)
                    return m_CurrentModule.OnMoneyPlayed(p, amount);

                Logger.LogDebugInformation("{0} played money but the game is not in the right state", p.Name);

                return false;
            }
        }

        /// <summary>
        /// The player is discarding cards
        /// </summary>
        public bool Discard(PlayerInfo p, string[] cards)
        {
            lock (Table)
            {
                Logger.LogDebugInformation("{0} is discarding [{1}] on state: {2}", p.Name, string.Join(", ",cards), State);

                if (m_CurrentModule != null)
                    return m_CurrentModule.OnCardDiscarded(p, cards);

                Logger.LogWarning("{0} tried discarding but the game is not in the right state", p.Name);

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
                    SetModule(new EndGameModule(Observer,Table));
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
                m_Modules.Enqueue(new InitGameModule(Observer,Table));
            }
            SetModule(m_Modules.Dequeue());
        }
        #endregion Private Methods
    }
}
