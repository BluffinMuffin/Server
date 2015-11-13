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

        private readonly List<int> m_AllInCaps = new List<int>(); // All the distincts ALL_IN CAPS of the ROUND
        private readonly Dictionary<PlayerInfo, int> m_BlindNeeded = new Dictionary<PlayerInfo, int>();
        private int m_CurrPotId;
        #endregion Fields

        #region Properties
        
        /// <summary>
        /// Contains all the rules of the current game
        /// </summary>
        public TableParams Params { get; }
        /// <summary>
        /// Contains all the People that are watching anbd playing the game. Everybody in the room.
        /// </summary>
        private List<PlayerInfo> People { get; } = new List<PlayerInfo>();

        /// <summary>
        /// Cards on the Board
        /// </summary>
        public string[] Cards { get; private set; }

        /// <summary>
        /// List of MoneyPots currently on the table. There should always have at least one MoneyPot
        /// </summary>
        public List<MoneyPot> Pots { get; } = new List<MoneyPot>();

        /// <summary>
        /// Contains all the money currently on the table (All Pots + Money currently played in front of the players)
        /// </summary>
        public int TotalPotAmnt { get; set; }

        /// <summary>
        /// Minimum amount to Raise
        /// </summary>
        public int MinimumRaiseAmount { get; set; }

        /// <summary>
        /// How many player have played this round and are ready to play the next one
        /// </summary>
        public int NbPlayed { get; set; }

        /// <summary>
        /// How many players are All In
        /// </summary>
        public int NbAllIn { get; set; }

        /// <summary>
        /// What is the amount to equal to stay in the game ?
        /// </summary>
        public int HigherBet { get; set; }

        /// <summary>
        /// What is the actual Round of the Game
        /// </summary>
        public int BettingRoundId { get; set; }

        /// <summary>
        /// List of the Seats
        /// </summary>
        public SeatInfo[] Seats { get; }

        public bool HadPlayers { get; private set; }

        public AbstractGameVariant Variant { get; }

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
            Variant = RuleFactory.Variants[Params.Variant];
            Seats = Enumerable.Range(0, Params.MaxPlayers).Select(i => new SeatInfo() { NoSeat = i }).ToArray();
        }

        public void InitTable()
        {
            Cards = new string[0];
            NbPlayed = 0;
            TotalPotAmnt = 0;
            Pots.Clear();
            Pots.Add(new MoneyPot(0));
            NbAllIn = 0;
            InitPokerTable();
            m_AllInCaps.Clear();
            m_CurrPotId = 0;
        }
        #endregion

        #region Public Methods

        public bool JoinTable(PlayerInfo p)
        {
            if (People.ContainsPlayerWithSameName(p))
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
            if (!People.ContainsPlayerWithSameName(p))
                return false;

            People.Remove(p);
            if (!SitOut(p))
                return false;

            return true;
        }

        public bool SitOut(PlayerInfo p)
        {
            if (!Seats.Players().ContainsPlayerWithSameName(p))
                return true;

            var seat = p.NoSeat;
            p.State = PlayerStateEnum.Zombie;
            p.NoSeat = -1;
            Seats[seat].Player = null;
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


        public IEnumerable<int> PotAmountsPadded
        {
            get
            {
                return Pots.Select(pot => pot.Amount).Union(Enumerable.Repeat(0, Params.MaxPlayers - Pots.Count));
            }
        }
        public SeatInfo SitIn(PlayerInfo p, int preferedSeat = -1)
        {
            if (!Seats.RemainingSeatIds().Any())
            {
                Logger.LogError("Not enough seats to join!");
                return null;
            }

            if (!Params.IsValidBuyIn(p.MoneyAmnt))
            {
                Logger.LogError("Player Money ({0}) is not between Minimum ({1}) and Maximum ({2})", p.MoneyAmnt, Params.Lobby.MinimumBuyInAmount(Params.GameSize), Params.Lobby.MaximumBuyInAmount(Params.GameSize));
                return null;
            }

            if (Seats.Players().ContainsPlayerWithSameName(p))
            {
                Logger.LogError("Already someone seated with the same name! Is this you ?");
                return null;
            }

            var seat = preferedSeat;

            if (preferedSeat < 0 || preferedSeat >= Seats.Length || !Seats[preferedSeat].IsEmpty)
                seat = Seats.RemainingSeatIds().First();

            HadPlayers = true;

            p.State = PlayerStateEnum.SitIn;
            p.NoSeat = seat;
            Seats[seat].Player = p;
            return Seats[seat];
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
                var pot = Pots[m_CurrPotId];
                pot.DetachAllPlayers();

                var aicf = m_AllInCaps[0];
                m_AllInCaps.RemoveAt(0);

                var cap = aicf - currentTaken;
                foreach (var p in Seats.Players())
                    AddBet(p, pot, Math.Min(p.MoneyBetAmnt, cap));

                currentTaken += cap;
                m_CurrPotId++;
                Pots.Add(new MoneyPot(m_CurrPotId));
            }

            var curPot = Pots[m_CurrPotId];
            curPot.DetachAllPlayers();
            foreach (var p in Seats.Players())
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
                var pot = Pots[i];
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

        #endregion Public Methods

        #region Private Methods
        private void AddBet(PlayerInfo p, MoneyPot pot, int bet)
        {
            p.MoneyBetAmnt -= bet;
            pot.AddAmount(bet);

            if (bet >= 0 && p.IsPlayingOrAllIn())
                pot.AttachPlayer(p);
        }
        private void InitPokerTable()
        {
            var previousDealer = Seats.SeatOfDealer();
            
            Seats.ToList().ForEach(s => s.SeatAttributes = new SeatAttributeEnum[0]);

            if (Params.Options.OptionType != GameTypeEnum.StudPoker)
                Seats.SeatOfPlayingPlayerNextTo(previousDealer).AddAttribute(SeatAttributeEnum.Dealer);

            m_BlindNeeded.Clear();

            switch(Params.Blind)
            {
                case BlindTypeEnum.Blinds:
                    var smallSeat = Seats.PlayingPlayers().Count() == 2 ? Seats.SeatOfDealer() : Seats.SeatOfPlayingPlayerNextTo(Seats.SeatOfDealer());
                    if (NewArrivals.All(x => x.NoSeat != smallSeat.NoSeat))
                    {
                        smallSeat.AddAttribute(SeatAttributeEnum.SmallBlind);
                        m_BlindNeeded.Add(smallSeat.Player, Params.GameSize / 2);
                    }

                    var bigSeat = Seats.SeatOfPlayingPlayerNextTo(smallSeat);
                    bigSeat.AddAttribute(SeatAttributeEnum.BigBlind);

                    NewArrivals.ForEach(x => Seats[x.NoSeat].AddAttribute(SeatAttributeEnum.BigBlind));
                    NewArrivals.Clear();

                    Seats.WithAttribute(SeatAttributeEnum.BigBlind).ToList().ForEach(x => { m_BlindNeeded.Add(x.Player, Params.GameSize); });
                    break;
                case BlindTypeEnum.Antes:
                    Seats.PlayingPlayers().ToList().ForEach(x => { m_BlindNeeded.Add(x, Math.Max(1, Params.GameSize / 10)); });
                    break;
            }
        }
        #endregion Private Methods
    }
}
