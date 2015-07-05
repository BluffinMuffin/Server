using System;
using System.Collections.Generic;
using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Protocol.DataTypes.Enums;
using System.Linq;

namespace BluffinMuffin.Server.DataTypes
{
    public class TableInfo
    {
        #region Fields
        protected readonly string[] m_Cards = new string[5];
        protected SeatInfo[] m_Seats;
        private readonly List<PlayerInfo> m_People = new List<PlayerInfo>();
        protected readonly List<MoneyPot> m_Pots = new List<MoneyPot>();
        private TableParams m_Params;
        #endregion Fields

        #region Properties
        /// <summary>
        /// Contains all the rules of the current game
        /// </summary>
        public TableParams Params
        {
            get
            {
                return m_Params;
            }
            set
            {
                m_Params = value;
                m_Seats = new SeatInfo[value.MaxPlayers];
                for (var i = 0; i < value.MaxPlayers; ++i)
                    m_Seats[i] = new SeatInfo() { NoSeat = i };
            }
        }
        /// <summary>
        /// Contains all the People that are watching anbd playing the game. Everybody in the room.
        /// </summary>
        public List<PlayerInfo> People { get { return m_People; } }

        /// <summary>
        /// Cards on the Board
        /// </summary>
        public string[] Cards
        {
            get { return m_Cards.Select(c => c ?? String.Empty).ToArray(); }
            protected set
            {
                if (value != null && value.Length == 5)
                {
                    for (var i = 0; i < 5; ++i)
                        m_Cards[i] = value[i];
                }
            }
        }

        /// <summary>
        /// List of MoneyPots currently on the table. There should always have at least one MoneyPot
        /// </summary>
        public List<MoneyPot> Pots
        {
            get { return m_Pots; }
        }

        public IEnumerable<int> PotAmountsPadded
        {
            get
            {
                return Pots.Select(pot => pot.Amount).Union(Enumerable.Repeat(0, Params.MaxPlayers - Pots.Count));
            }
        }

        /// <summary>
        /// Contains all the money currently on the table (All Pots + Money currently played in front of the players)
        /// </summary>
        public int TotalPotAmnt{ get; set; }

        /// <summary>
        /// Minimum amount to Raise
        /// </summary>
        public int MinimumRaiseAmount { get; set; }

        /// <summary>
        /// Where is the Dealer
        /// </summary>
        public SeatInfo DealerSeat
        {
            get
            {
                return m_Seats.FirstOrDefault(s => s.SeatAttributes.Contains(SeatAttributeEnum.Dealer));
            }
        }
        public int NoSeatDealer
        {
            get
            {
                return DealerSeat == null ? -1 : DealerSeat.NoSeat;
            }
        }

        public SeatInfo CurrentPlayerSeat
        {
            get
            {
                return m_Seats.FirstOrDefault(s => s.SeatAttributes.Contains(SeatAttributeEnum.CurrentPlayer));
            }
        }
        public int NoSeatCurrentPlayer
        {
            get
            {
                return CurrentPlayerSeat == null ? -1 : CurrentPlayerSeat.NoSeat;
            }
        }

        /// <summary>
        /// Who is the current player
        /// </summary>
        public PlayerInfo CurrentPlayer
        {
            get
            {
                return CurrentPlayerSeat == null ? null : CurrentPlayerSeat.Player;
            }
        }

        /// <summary>
        /// How many player have played this round and are ready to play the next one
        /// </summary>
        public int NbPlayed { get; set; }

        /// <summary>
        /// How many players are All In
        /// </summary>
        public int NbAllIn { get; set; }

        /// <summary>
        /// How many players are still in the Game (All-In not included)
        /// </summary>
        public int NbPlaying { get { return PlayingPlayers.Count; } }

        /// <summary>
        /// How many players are still in the Game (All-In included)
        /// </summary>
        public int NbPlayingAndAllIn { get { return NbPlaying + NbAllIn; } }

        /// <summary>
        /// What is the amount to equal to stay in the game ?
        /// </summary>
        public int HigherBet { get; set; }

        /// <summary>
        /// What is the actual Round of the Game
        /// </summary>
        public int BettingRoundId { get; set; }

        /// <summary>
        /// List of the Players currently seated
        /// </summary>
        public List<PlayerInfo> Players { get { return m_Seats.Where(s => !s.IsEmpty).Select(s => s.Player).ToList(); } }

        /// <summary>
        /// List of the Seats
        /// </summary>
        public List<SeatInfo> Seats { get { return m_Seats.ToList(); } }

        /// <summary>
        /// List of the playing Players in order starting from the first seat
        /// </summary>
        public List<PlayerInfo> PlayingPlayers
        {
            get { return PlayingPlayersFrom(); }
        }

        /// <summary>
        /// List of the playing Players in order starting from the first seat
        /// </summary>
        public IEnumerable<PlayerInfo> PlayingAndAllInPlayers
        {
            get { return PlayingAndAllInPlayersFrom(); }
        }
        #endregion Properties

        #region Ctors & Init

        protected TableInfo()
            : this(new TableParams())
        {
        }

        protected TableInfo(TableParams parms)
        {
            Params = parms;
        }

        public virtual void InitTable()
        {
            Cards = new string[5];
            NbPlayed = 0;
            TotalPotAmnt = 0;
            m_Pots.Clear();
            m_Pots.Add(new MoneyPot(0));
            NbAllIn = 0;
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Return the next playing player next to a seat number (All-In not included)
        /// </summary>
        public SeatInfo GetSeatOfPlayingPlayerNextTo(SeatInfo seat)
        {
            var noSeat = seat == null ? -1 : seat.NoSeat;
            for (var i = 0; i < Params.MaxPlayers; ++i)
            {
                var si = m_Seats[(noSeat + 1 + i) % Params.MaxPlayers];
                if (!si.IsEmpty && si.Player.IsPlaying)
                    return si;
            }
            return seat;
        }
        public SeatInfo GetSeatOfPlayingPlayerJustBefore(SeatInfo seat)
        {
            var noSeat = seat == null ? -1 : seat.NoSeat;
            for (var i = 0; i < Params.MaxPlayers; ++i)
            {
                var id = (noSeat - 1 - i) % Params.MaxPlayers;
                if (id < 0)
                    id = Params.MaxPlayers + id;
                var si = m_Seats[id];
                if (!si.IsEmpty && si.Player.IsPlaying)
                    return si;
            }
            return seat;
        }

        public virtual bool JoinTable(PlayerInfo p)
        {
            People.Add(p);
            p.State = PlayerStateEnum.Joined;
            return true;
        }

        /// <summary>
        /// Sit a player without the validations. This is used here after validation, or on the client side when the game is telling the client where a player is seated
        /// </summary>
        public SeatInfo SitInToTable(PlayerInfo p, int seat)
        {
            p.State = PlayerStateEnum.SitIn;
            p.NoSeat = seat;
            m_Seats[seat].Player = p;
            return m_Seats[seat];
        }

        /// <summary>
        /// When a player leaves the table
        /// </summary>
        public virtual bool LeaveTable(PlayerInfo p)
        {
            if (!PeopleContainsPlayer(p))
                return false;
            People.Remove(p);

            return SitOut(p);
        }

        public bool SitOut(PlayerInfo p)
        {
            if (!SeatsContainsPlayer(p))
                return true;

            var seat = p.NoSeat;
            p.State = PlayerStateEnum.Zombie;
            p.NoSeat = -1;
            m_Seats[seat].Player = null;
            return true;
        }

        /// <summary>
        /// Can this player use the CHECK action ?
        /// </summary>
        public bool CanCheck(PlayerInfo p)
        {
            return HigherBet <= p.MoneyBetAmnt;
        }

        /// <summary>
        /// What is the minimum amount that this player can put on the table to RAISE
        /// </summary>
        public int MinRaiseAmnt(PlayerInfo p)
        {
            return Math.Min(CallAmnt(p) + MinimumRaiseAmount, MaxRaiseAmnt(p));
        }

        /// <summary>
        /// What is the maximum amount that this player can put on the table ?
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private int MaxRaiseAmnt(PlayerInfo p)
        {
            return p.MoneySafeAmnt;
        }

        /// <summary>
        /// What is the needed amount for this player to CALL ?
        /// </summary>
        public int CallAmnt(PlayerInfo p)
        {
            return HigherBet - p.MoneyBetAmnt;
        }

        public void ChangeCurrentPlayerTo(SeatInfo seat)
        {
            var oldPlayerSeat = CurrentPlayerSeat;
            if (oldPlayerSeat != null)
                oldPlayerSeat.SeatAttributes = oldPlayerSeat.SeatAttributes.Except(new [] { SeatAttributeEnum.CurrentPlayer }).ToArray();
            if (seat != null)
                seat.SeatAttributes = seat.SeatAttributes.Union(new[] { SeatAttributeEnum.CurrentPlayer }).ToArray();
        }
        #endregion Public Methods

        #region Protected Methods

        private List<PlayerInfo> PlayingPlayersFrom()
        {
            return m_Seats.Where(s => (!s.IsEmpty && s.Player.IsPlaying)).Select(s => s.Player).ToList();
        }

        private IEnumerable<PlayerInfo> PlayingAndAllInPlayersFrom()
        {
            return m_Seats.Where(s => (!s.IsEmpty && (s.Player.IsPlaying || s.Player.IsAllIn))).Select(s => s.Player);
        }

        public bool SeatsContainsPlayer(PlayerInfo p)
        {
            return Players.Contains(p) || Players.Count(x => x.Name.ToLower() == p.Name.ToLower()) > 0;
        }
        protected bool PeopleContainsPlayer(PlayerInfo p)
        {
            return People.Contains(p) || People.Count(x => x.Name.ToLower() == p.Name.ToLower()) > 0;
        }
        #endregion Protected Methods
    }
}
