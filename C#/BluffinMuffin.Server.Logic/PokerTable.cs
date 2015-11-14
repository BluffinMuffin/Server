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

        #region Properties
        
        public MoneyBank Bank { get; } = new MoneyBank();

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
        /// Minimum amount to Raise
        /// </summary>
        public int MinimumRaiseAmount { get; set; }

        /// <summary>
        /// How many player have played this round and are ready to play the next one
        /// </summary>
        public int NbPlayed { get; set; }

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

        public List<PlayerInfo> NewArrivals { get; }

        #endregion Properties

        #region Ctors & Init

        public PokerTable()
            : this(new TableParams())
        {
        }

        public PokerTable(TableParams parms)
        {
            Params = parms;
            NewArrivals = new List<PlayerInfo>();
            HadPlayers = false;
            Variant = RuleFactory.Variants[Params.Variant];
            Seats = Enumerable.Range(0, Params.MaxPlayers).Select(i => new SeatInfo() { NoSeat = i }).ToArray();
        }
        #endregion

        #region Public Methods

        public void InitTable()
        {
            Cards = new string[0];
            NbPlayed = 0;

            var previousDealer = Seats.SeatOfDealer();

            Seats.ClearAllAttributes();

            if (Params.Options.OptionType != GameTypeEnum.StudPoker)
                Seats.SeatOfPlayingPlayerNextTo(previousDealer).AddAttribute(SeatAttributeEnum.Dealer);

            foreach (var s in GetPlayersWhoNeedsToPutAntes())
                Bank.AddDebt(s.Player, Math.Max(1, Params.GameSize / 10));

            foreach (var s in GetPlayersWhoNeedsToPutSmallBlind())
            {
                s.AddAttribute(SeatAttributeEnum.SmallBlind);
                Bank.AddDebt(s.Player, Params.GameSize / 2);
            }

            foreach (var s in GetPlayersWhoNeedsToPutBigBlind())
            {
                s.AddAttribute(SeatAttributeEnum.BigBlind);
                Bank.AddDebt(s.Player, Params.GameSize);
            }

            NewArrivals.Clear();
        }

        /// <summary>
        /// When a player joins the table
        /// </summary>
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
            SitOut(p);

            return true;
        }

        public void SitOut(PlayerInfo p)
        {
            if (!Seats.Players().ContainsPlayerWithSameName(p))
                return;

            var seat = p.NoSeat;
            p.State = PlayerStateEnum.Zombie;
            p.NoSeat = -1;
            Seats[seat].Player = null;
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
        public int MinRaiseAmountForPlayer(PlayerInfo p)
        {
            return Math.Min(NeededCallAmountForPlayer(p) + MinimumRaiseAmount, MaxRaiseAmountForPlayer(p));
        }

        /// <summary>
        /// What is the maximum amount that this player can put on the table ?
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private int MaxRaiseAmountForPlayer(PlayerInfo p)
        {
            return p.MoneySafeAmnt;
        }

        /// <summary>
        /// What is the needed amount for this player to CALL ?
        /// </summary>
        public int NeededCallAmountForPlayer(PlayerInfo p)
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

        public IEnumerable<SeatInfo> GetPlayersWhoNeedsToPutAntes()
        {
            return Params.Blind == BlindTypeEnum.Antes ? Seats.PlayingPlayers().Select(x => Seats[x.NoSeat]) : new SeatInfo[0];
        }
        public IEnumerable<SeatInfo> GetPlayersWhoNeedsToPutSmallBlind()
        {
            if (Params.Blind != BlindTypeEnum.Blinds)
                return new SeatInfo[0];

            var smallSeat = Seats.SeatOfShouldBeSmallBlind();
            return NewArrivals.Any(x => x.NoSeat == smallSeat.NoSeat) ? new SeatInfo[0] : new[] {smallSeat};
        }
        public IEnumerable<SeatInfo> GetPlayersWhoNeedsToPutBigBlind()
        {
            return Params.Blind == BlindTypeEnum.Blinds ? NewArrivals.Select(x => Seats[x.NoSeat]).Union(new[] {Seats.SeatOfShouldBeBigBlind()}) : new SeatInfo[0];
        }

        #endregion Public Methods
    }
}
