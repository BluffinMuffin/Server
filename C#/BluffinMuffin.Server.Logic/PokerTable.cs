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
    public class PokerTable : TableInfo
    {
        #region Fields

        private readonly List<int> m_AllInCaps = new List<int>(); // All the distincts ALL_IN CAPS of the ROUND
        private readonly Dictionary<PlayerInfo, int> m_BlindNeeded = new Dictionary<PlayerInfo, int>();
        private int m_CurrPotId;
        #endregion Fields

        #region Properties

        /// <summary>
        /// Total amount of money still needed as Blinds for the game to start
        /// </summary>
        public int TotalBlindNeeded { get { return m_BlindNeeded.Values.Sum(); } }

        public List<PlayerInfo> NewArrivals { get; private set; }

        #endregion Properties

        #region Ctors & Init
        public PokerTable()
        {
            NewArrivals = new List<PlayerInfo>();
        }

        public PokerTable(TableParams parms)
            :base(parms)
        {
            NewArrivals = new List<PlayerInfo>();
        }

        public override void InitTable()
        {
            base.InitTable();
            InitPokerTable();
            m_AllInCaps.Clear();
            m_CurrPotId = 0;
        }
        #endregion

        #region Public Methods


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

        /// <summary>
        /// When a player joined the table
        /// </summary>
        public override bool JoinTable(PlayerInfo p)
        {

            if (PeopleContainsPlayer(p))
            {
                LogManager.Log(LogLevel.Error, "TableInfo.JoinTable", "Already someone with the same name!");
                return false;
            }
            var ok = base.JoinTable(p);
            //if(ok)
            //    ok = SitIn(p);
            //if(!ok)
            //    base.LeaveTable(p);
            return ok;
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
            return SitInToTable(p, seat);
        }

        /// <summary>
        /// When a player leaves the table
        /// </summary>
        public override bool LeaveTable(PlayerInfo p)
        {
            if (!PeopleContainsPlayer(p))
                return false;

            if (!base.LeaveTable(p))
                return false;

            return true;
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

        #endregion Public Methods

        #region Private Methods

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
