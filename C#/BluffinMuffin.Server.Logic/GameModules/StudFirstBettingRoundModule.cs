using System;
using System.Linq;
using BluffinMuffin.HandEvaluator;
using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Protocol.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes;
using BluffinMuffin.Server.DataTypes.EventHandling;
using BluffinMuffin.Server.Logic.Extensions;
using BluffinMuffin.Server.Logic.GameVariants;

namespace BluffinMuffin.Server.Logic.GameModules
{
    public class StudFirstBettingRoundModule : FirstBettingRoundModule
    {
        private AbstractStudGameVariant Variant { get; }
        public StudFirstBettingRoundModule(PokerGameObserver o, PokerTable table)
            : base(o, table)
        {
            Variant = Table.Variant as AbstractStudGameVariant;
            if (Table.Params.Options.OptionType != GameTypeEnum.StudPoker || Variant == null)
                throw new Exception("This should only used with STUD games.");
        }

        protected override bool CanFold()
        {
            return !Variant.NeedsBringIn;
        }

        protected override SeatInfo GetSeatOfTheFirstPlayer()
        {
            return Table.Seats[HandEvaluators.Evaluate(Table.Seats.PlayingPlayers().Select(p => new CardHolder(p, p.FaceUpCards, new string[0])).Cast<IStringCardsHolder>().ToArray(), new EvaluationParams { UseSuitRanking = true }).Last().Select(x => x.CardsHolder).Cast<CardHolder>().First().Player.NoSeat];
        }

        protected override void InitModuleSpecific()
        {
            Variant.NeedsBringIn = true;
            Table.HigherBet = Table.Params.HalfBetAmount();
            Table.MinimumRaiseAmount = Table.Params.HalfBetAmount();
        }

        public override bool OnMoneyPlayed(PlayerInfo p, int amnt)
        {
            if (Variant.NeedsBringIn)
            {
                Logger.LogDebugInformation("Currently, we need {0}, the bring in !!", Table.NeededCallAmountForPlayer(p));
                if (amnt == -1)
                    return false;
            }

            return base.OnMoneyPlayed(p, amnt);
        }

        protected override void ContinueBettingRound()
        {
            if (Variant.NeedsBringIn)
            {
                Table.MinimumRaiseAmount = Table.Params.BetAmount();
                Variant.NeedsBringIn = false;
            }
            base.ContinueBettingRound();
        }
    }
}
