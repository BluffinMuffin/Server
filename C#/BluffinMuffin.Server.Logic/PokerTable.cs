using System;
using System.Collections.Generic;
using BluffinMuffin.HandEvaluator;
using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Protocol.DataTypes.Enums;
using BluffinMuffin.Protocol.DataTypes.Options;
using System.Linq;
using BluffinMuffin.Server.DataTypes;
using Com.Ericmas001.Util;

namespace BluffinMuffin.Server.Logic
{
    public class PokerTable
    {
        #region Fields
        private readonly string[] m_Cards = new string[5];
        private SeatInfo[] m_Seats;
        private readonly List<PlayerInfo> m_People = new List<PlayerInfo>();
        private readonly List<MoneyPot> m_Pots = new List<MoneyPot>();
        private TableParams m_Params;

        private readonly List<int> m_AllInCaps = new List<int>(); // All the distincts ALL_IN CAPS of the ROUND
        private readonly Dictionary<PlayerInfo, int> m_BlindNeeded = new Dictionary<PlayerInfo, int>();
        private int m_CurrPotId;
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
        public int TotalPotAmnt { get; set; }

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
        public AbstractDealer Dealer { get; set; }
        public bool HadPlayers { get; private set; }

        public bool NoMoreRoundsNeeded { get; set; }
        /// <summary>
        /// Total amount of money still needed as Blinds for the game to start
        /// </summary>
        public int TotalBlindNeeded { get { return m_BlindNeeded.Values.Sum(); } }

        public List<PlayerInfo> NewArrivals { get; private set; }

        #endregion Properties

        #region Ctors & Init

        public PokerTable()
            : this(new TableParams())
        {
            NewArrivals = new List<PlayerInfo>();
            HadPlayers = false;
        }

        public PokerTable(TableParams parms)
        {
            Params = parms;
            NewArrivals = new List<PlayerInfo>();
            HadPlayers = false;
        }

        public void InitTable()
        {
            Cards = new string[5];
            NbPlayed = 0;
            TotalPotAmnt = 0;
            m_Pots.Clear();
            m_Pots.Add(new MoneyPot(0));
            NbAllIn = 0;
            InitPokerTable();
            m_AllInCaps.Clear();
            m_CurrPotId = 0;
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

        public bool JoinTable(PlayerInfo p)
        {
            if (PeopleContainsPlayer(p))
            {
                LogManager.Log(LogLevel.Error, "TableInfo.JoinTable", "Already someone with the same name!");
                return false;
            }
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
        public bool LeaveTable(PlayerInfo p)
        {
            if (!PeopleContainsPlayer(p))
                return false;

            People.Remove(p);
            if (!SitOut(p))
                return false;

            return true;
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
                oldPlayerSeat.SeatAttributes = oldPlayerSeat.SeatAttributes.Except(new[] { SeatAttributeEnum.CurrentPlayer }).ToArray();
            if (seat != null)
                seat.SeatAttributes = seat.SeatAttributes.Union(new[] { SeatAttributeEnum.CurrentPlayer }).ToArray();
        }


        /// <summary>
        /// Is there already a player of that name, seated at the table ?
        /// </summary>
        public bool ContainsPlayer(String name)
        {
            return Players.Any(p => p.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }
        /// <summary>
        /// Add cards to the board
        /// </summary>
        public void AddCards(params string[] c)
        {
            var firstUnused =  Enumerable.Range(0,m_Cards.Length).First(i => String.IsNullOrEmpty(m_Cards[i]));
            for (var j = firstUnused; j < Math.Min(5, c.Length + firstUnused); ++j)
                m_Cards[j] = c[j - firstUnused];
        }

        /// <summary>
        /// Add an AllInCap that will be used when splitting the pot
        /// </summary>
        public void AddAllInCap(int val)
        {
            if (!m_AllInCaps.Contains(val))
                m_AllInCaps.Add(val);
        }

        /// <summary>
        /// Sets how much money is still needed from a specific player as Blind
        /// </summary>
        public void SetBlindNeeded(PlayerInfo p, int amnt)
        {
            if (m_BlindNeeded.ContainsKey(p))
                m_BlindNeeded[p] = amnt;
            else
                m_BlindNeeded.Add(p, amnt);
        }

        /// <summary>
        /// How much money a player needs to put as Blind
        /// </summary>
        public int GetBlindNeeded(PlayerInfo p)
        {
            if (m_BlindNeeded.ContainsKey(p))
                return m_BlindNeeded[p];
            return 0;
        }

        public SeatInfo SitIn(PlayerInfo p, int preferedSeat)
        {
            if (!RemainingSeats.Any())
            {
                LogManager.Log(LogLevel.Error, "TableInfo.JoinTable", "Not enough seats to join!");
                return null;
            }

            if (p.MoneyAmnt < Params.Lobby.MinimumAmountForBuyIn || p.MoneyAmnt > Params.Lobby.MaximumAmountForBuyIn)
            {
                LogManager.Log(LogLevel.Error, "TableInfo.JoinTable", "Player Money ({0}) is not between Minimum ({1}) and Maximum ({2})", p.MoneyAmnt, Params.Lobby.MinimumAmountForBuyIn, Params.Lobby.MaximumAmountForBuyIn);
                return null;
            }

            if (SeatsContainsPlayer(p))
            {
                LogManager.Log(LogLevel.Error, "TableInfo.JoinTable", "Already someone seated with the same name! Is this you ?");
                return null;
            }

            var seat = preferedSeat;

            if (preferedSeat < 0 || preferedSeat >= Seats.Count || !Seats[preferedSeat].IsEmpty)
                seat = RemainingSeats.First();
            HadPlayers = true;
            return SitInToTable(p, seat);
        }


        /// <summary>
        /// Put a number on the current "Hand" of a player. The we will use that number to compare who is winning !
        /// </summary>
        /// <param name="playerCards">Player cards</param>
        /// <returns>A unsigned int that we can use to compare with another hand</returns>
        private HandEvaluationResult EvaluateCards(params string[] playerCards)
        {
            if (Cards == null || Cards.Length != 5 || Cards.Any(String.IsNullOrEmpty) || playerCards == null || playerCards.Length != 2)
                return null;

            return HandEvaluator.HandEvaluator.Evaluate(Cards.Union(playerCards).ToArray());
        }

        /// <summary>
        /// At the end of a Round, it's time to separate all the money into one or more pots of money (Depending on when a player wen All-In)
        /// For every cap, we take money from each player that still have money in front of them
        /// </summary>
        public void ManagePotsRoundEnd()
        {
            var currentTaken = 0;
            m_AllInCaps.Sort();

            while (m_AllInCaps.Count > 0)
            {
                var pot = m_Pots[m_CurrPotId];
                pot.DetachAllPlayers();

                var aicf = m_AllInCaps[0];
                m_AllInCaps.RemoveAt(0);

                var cap = aicf - currentTaken;
                foreach (var p in Players)
                    AddBet(p, pot, Math.Min(p.MoneyBetAmnt, cap));

                currentTaken += cap;
                m_CurrPotId++;
                m_Pots.Add(new MoneyPot(m_CurrPotId));
            }

            var curPot = m_Pots[m_CurrPotId];
            curPot.DetachAllPlayers();
            foreach (var p in Players)
                AddBet(p, curPot, p.MoneyBetAmnt);

            HigherBet = 0;
        }

        /// <summary>
        /// Detach all the players that are not winning this pot
        /// </summary>
        public void CleanPotsForWinning()
        {
            for (var i = 0; i <= m_CurrPotId; ++i)
            {
                var pot = m_Pots[i];
                HandEvaluationResult bestHand = null;
                var infos = pot.AttachedPlayers.Select(x => x.Player).ToArray();

                foreach (var p in infos)
                {
                    var handValue = EvaluateCards(p.HoleCards);
                    if (handValue != null)
                    {
                        switch (handValue.CompareTo(bestHand))
                        {
                            case 1:
                                pot.DetachAllPlayers();
                                pot.AttachPlayer(p, handValue);
                                bestHand = handValue;
                                break;
                            case 0:
                                pot.AttachPlayer(p, handValue);
                                break;
                        }
                    }
                }
            }
        }

        public bool SeatsContainsPlayer(PlayerInfo p)
        {
            return Players.Contains(p) || Players.Count(x => x.Name.ToLower() == p.Name.ToLower()) > 0;
        }

        #endregion Public Methods

        #region Private Methods

        private List<PlayerInfo> PlayingPlayersFrom()
        {
            return m_Seats.Where(s => (!s.IsEmpty && s.Player.IsPlaying)).Select(s => s.Player).ToList();
        }

        private IEnumerable<PlayerInfo> PlayingAndAllInPlayersFrom()
        {
            return m_Seats.Where(s => (!s.IsEmpty && (s.Player.IsPlaying || s.Player.IsAllIn))).Select(s => s.Player);
        }
        private bool PeopleContainsPlayer(PlayerInfo p)
        {
            return People.Contains(p) || People.Count(x => x.Name.ToLower() == p.Name.ToLower()) > 0;
        }

        private IEnumerable<int> RemainingSeats
        {
            get
            {
                for (var i = 0; i < Seats.Count; ++i)
                    if (Seats[i].IsEmpty)
                        yield return i;
            }
        }
        private void AddBet(PlayerInfo p, MoneyPot pot, int bet)
        {
            p.MoneyBetAmnt -= bet;
            pot.AddAmount(bet);

            if (bet >= 0 && (p.IsPlaying || p.IsAllIn))
                pot.AttachPlayer(p);
        }
        private void InitPokerTable()
        {
            var previousDealer = DealerSeat;
            
            Seats.ForEach(s => s.SeatAttributes = new SeatAttributeEnum[0]);

            var nextDealerSeat = GetSeatOfPlayingPlayerNextTo(previousDealer);
            nextDealerSeat.SeatAttributes = nextDealerSeat.SeatAttributes.Union(new[] { SeatAttributeEnum.Dealer }).ToArray();

            m_BlindNeeded.Clear();

            switch(Params.Blind.OptionType)
            {
                case BlindTypeEnum.Blinds:
                    var bob = Params.Blind as BlindOptionsBlinds;

                    var smallSeat = NbPlaying == 2 ? DealerSeat : GetSeatOfPlayingPlayerNextTo(DealerSeat);
                    if (NewArrivals.All(x => x.NoSeat != smallSeat.NoSeat))
                    {
                        smallSeat.SeatAttributes = smallSeat.SeatAttributes.Union(new[] { SeatAttributeEnum.SmallBlind }).ToArray();
                        if (bob != null) m_BlindNeeded.Add(smallSeat.Player, bob.SmallBlindAmount);
                    }

                    var bigSeat = GetSeatOfPlayingPlayerNextTo(smallSeat);
                    bigSeat.SeatAttributes = bigSeat.SeatAttributes.Union(new[] { SeatAttributeEnum.BigBlind }).ToArray();

                    NewArrivals.ForEach(x => Seats[x.NoSeat].SeatAttributes = Seats[x.NoSeat].SeatAttributes.Union(new[] { SeatAttributeEnum.BigBlind }).ToArray());
                    NewArrivals.Clear();

                    Seats.Where(x => x.SeatAttributes.Contains(SeatAttributeEnum.BigBlind)).ToList().ForEach(x => { if (bob != null) m_BlindNeeded.Add(x.Player, bob.BigBlindAmount); });
                    break;
                case BlindTypeEnum.Antes:
                    var boa = Params.Blind as BlindOptionsAnte;
                    PlayingPlayers.ForEach(x => { if (boa != null) m_BlindNeeded.Add(x, boa.AnteAmount); });
                    break;
            }
        }
        #endregion Private Methods
    }
}
