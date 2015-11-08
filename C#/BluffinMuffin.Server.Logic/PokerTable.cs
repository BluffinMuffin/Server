using System;
using System.Collections.Generic;
using BluffinMuffin.HandEvaluator;
using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Protocol.DataTypes.Enums;
using System.Linq;
using BluffinMuffin.Server.DataTypes;
using BluffinMuffin.Server.Logic.Extensions;
using BluffinMuffin.Server.Logic.GameVariants;

namespace BluffinMuffin.Server.Logic
{
    public class PokerTable
    {
        #region Fields
        private SeatInfo[] m_Seats;
        private readonly List<MoneyPot> m_Pots = new List<MoneyPot>();
        private TableParams m_Params;
        private AbstractGameVariant m_Variant;

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
                m_Variant = RuleFactory.Variants[Params.Variant];
                m_Seats = new SeatInfo[value.MaxPlayers];
                for (var i = 0; i < value.MaxPlayers; ++i)
                    m_Seats[i] = new SeatInfo() { NoSeat = i };
            }
        }
        /// <summary>
        /// Contains all the People that are watching anbd playing the game. Everybody in the room.
        /// </summary>
        public List<PlayerInfo> People { get; } = new List<PlayerInfo>();

        /// <summary>
        /// Cards on the Board
        /// </summary>
        public string[] Cards { get; set; }

        /// <summary>
        /// List of MoneyPots currently on the table. There should always have at least one MoneyPot
        /// </summary>
        public List<MoneyPot> Pots => m_Pots;

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

        /// <summary>
        /// Where is the FirstTalker
        /// </summary>
        public SeatInfo FirstTalkerSeat
        {
            get
            {
                return m_Seats.FirstOrDefault(s => s.SeatAttributes.Contains(SeatAttributeEnum.FirstTalker));
            }
        }

        public SeatInfo CurrentPlayerSeat
        {
            get
            {
                return m_Seats.FirstOrDefault(s => s.SeatAttributes.Contains(SeatAttributeEnum.CurrentPlayer));
            }
        }
        public int NoSeatCurrentPlayer => CurrentPlayerSeat?.NoSeat ?? -1;

        /// <summary>
        /// Who is the current player
        /// </summary>
        public PlayerInfo CurrentPlayer => CurrentPlayerSeat?.Player;

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
        public int NbPlaying => PlayingPlayers.Count;

        /// <summary>
        /// How many players are still in the Game (All-In included)
        /// </summary>
        public int NbPlayingAndAllIn => NbPlaying + NbAllIn;

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
        public List<SeatInfo> Seats => m_Seats.ToList();

        /// <summary>
        /// List of the playing Players in order starting from the first seat
        /// </summary>
        public List<PlayerInfo> PlayingPlayers => PlayingPlayersFrom();

        /// <summary>
        /// List of the playing Players in order starting from the first seat
        /// </summary>
        public IEnumerable<PlayerInfo> PlayingAndAllInPlayers => PlayingAndAllInPlayersFrom();

        public bool HadPlayers { get; private set; }

        public AbstractGameVariant Variant => m_Variant;

        public bool NoMoreRoundsNeeded { get; set; }
        /// <summary>
        /// Total amount of money still needed as Blinds for the game to start
        /// </summary>
        public int TotalBlindNeeded => m_BlindNeeded.Values.Sum();

        public List<PlayerInfo> NewArrivals { get; }

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
            Cards = new string[0];
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
            var noSeat = seat?.NoSeat ?? -1;
            for (var i = 0; i < Params.MaxPlayers; ++i)
            {
                var si = m_Seats[(noSeat + 1 + i) % Params.MaxPlayers];
                if (si.HasPlayerPlaying())
                    return si;
            }
            return seat;
        }
        public SeatInfo GetSeatOfPlayingPlayerJustBefore(SeatInfo seat)
        {
            var noSeat = seat?.NoSeat ?? -1;
            for (var i = 0; i < Params.MaxPlayers; ++i)
            {
                var id = (noSeat - 1 - i) % Params.MaxPlayers;
                if (id < 0)
                    id = Params.MaxPlayers + id;
                var si = m_Seats[id];
                if (si.HasPlayerPlaying())
                    return si;
            }
            return seat;
        }

        public bool JoinTable(PlayerInfo p)
        {
            if (PeopleContainsPlayer(p))
            {
                Logger.LogError("Already someone with the same name!");
                return false;
            }
            People.Add(p);
            p.State = PlayerStateEnum.Joined;
            return true;
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
        public bool ContainsPlayer(string name)
        {
            return Players.Any(p => p.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }
        /// <summary>
        /// Add cards to the board
        /// </summary>
        public void InitCards()
        {
            Cards = new string[0];
        }
        /// <summary>
        /// Add cards to the board
        /// </summary>
        public void AddCards(params string[] c)
        {
            Cards = Cards.Concat(c).ToArray();
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

        public SeatInfo SitIn(PlayerInfo p, int preferedSeat = -1)
        {
            if (!RemainingSeats.Any())
            {
                Logger.LogError("Not enough seats to join!");
                return null;
            }

            if (!Params.IsValidBuyIn(p.MoneyAmnt))
            {
                Logger.LogError("Player Money ({0}) is not between Minimum ({1}) and Maximum ({2})", p.MoneyAmnt, Params.MinimumBuyInAmount, Params.MaximumBuyInAmount);
                return null;
            }

            if (SeatsContainsPlayer(p))
            {
                Logger.LogError("Already someone seated with the same name! Is this you ?");
                return null;
            }

            var seat = preferedSeat;

            if (preferedSeat < 0 || preferedSeat >= Seats.Count || !Seats[preferedSeat].IsEmpty)
                seat = RemainingSeats.First();

            HadPlayers = true;

            p.State = PlayerStateEnum.SitIn;
            p.NoSeat = seat;
            m_Seats[seat].Player = p;
            return m_Seats[seat];
        }


        /// <summary>
        /// Put a number on the current "Hand" of a player. The we will use that number to compare who is winning !
        /// </summary>
        /// <param name="playerCards">Player cards</param>
        /// <returns>A unsigned int that we can use to compare with another hand</returns>
        private HandEvaluationResult EvaluateCards(params string[] playerCards)
        {
            if (Cards == null || playerCards == null || Cards.Union(playerCards).Count(x => !string.IsNullOrEmpty(x)) < 5)
                return null;

            return HandEvaluators.Evaluate(playerCards.Where(x => !string.IsNullOrEmpty(x)), Cards.Where(x => !string.IsNullOrEmpty(x)));
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
                    var handValue = EvaluateCards(p.Cards);
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
            return m_Seats.Where(SeatInfoExtensions.HasPlayerPlaying).Select(s => s.Player).ToList();
        }

        private IEnumerable<PlayerInfo> PlayingAndAllInPlayersFrom()
        {
            return m_Seats.Where(SeatInfoExtensions.HasPlayerPlayingOrAllIn).Select(s => s.Player);
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

            if (bet >= 0 && p.IsPlayingOrAllIn())
                pot.AttachPlayer(p);
        }
        private void InitPokerTable()
        {
            var previousDealer = DealerSeat;
            
            Seats.ForEach(s => s.SeatAttributes = new SeatAttributeEnum[0]);

            if (Params.Options.OptionType != GameTypeEnum.StudPoker)
            {
                var nextDealerSeat = GetSeatOfPlayingPlayerNextTo(previousDealer);
                nextDealerSeat.SeatAttributes = nextDealerSeat.SeatAttributes.Union(new[] {SeatAttributeEnum.Dealer}).ToArray();
            }

            m_BlindNeeded.Clear();

            switch(Params.Blind)
            {
                case BlindTypeEnum.Blinds:
                    var smallSeat = NbPlaying == 2 ? DealerSeat : GetSeatOfPlayingPlayerNextTo(DealerSeat);
                    if (NewArrivals.All(x => x.NoSeat != smallSeat.NoSeat))
                    {
                        smallSeat.SeatAttributes = smallSeat.SeatAttributes.Union(new[] { SeatAttributeEnum.SmallBlind }).ToArray();
                        m_BlindNeeded.Add(smallSeat.Player, Params.GameSize / 2);
                    }

                    var bigSeat = GetSeatOfPlayingPlayerNextTo(smallSeat);
                    bigSeat.SeatAttributes = bigSeat.SeatAttributes.Union(new[] { SeatAttributeEnum.BigBlind }).ToArray();

                    NewArrivals.ForEach(x => Seats[x.NoSeat].SeatAttributes = Seats[x.NoSeat].SeatAttributes.Union(new[] { SeatAttributeEnum.BigBlind }).ToArray());
                    NewArrivals.Clear();

                    Seats.Where(x => x.SeatAttributes.Contains(SeatAttributeEnum.BigBlind)).ToList().ForEach(x => { m_BlindNeeded.Add(x.Player, Params.GameSize); });
                    break;
                case BlindTypeEnum.Antes:
                    PlayingPlayers.ForEach(x => { m_BlindNeeded.Add(x, Math.Max(1, Params.GameSize / 10)); });
                    break;
            }
        }
        #endregion Private Methods
    }
}
