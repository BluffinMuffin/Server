using System;
using System.Linq;
using BluffinMuffin.HandEvaluator;
using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Protocol.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes;
using BluffinMuffin.Server.DataTypes.EventHandling;
using BluffinMuffin.Server.Logic.GameVariants;
using Com.Ericmas001.Util;

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


        protected override SeatInfo GetSeatOfTheFirstPlayer()
        {
            return Table.Seats[HandEvaluators.Evaluate(Table.PlayingPlayers.Select(p => new CardHolder(p, p.FaceUpCards, new string[0])).Cast<IStringCardsHolder>().ToArray(), new EvaluationParams { UseSuitRanking = true }).Last().Select(x => x.CardsHolder).Cast<CardHolder>().First().Player.NoSeat];
        }

        protected override void InitModuleSpecific()
        {
            Variant.NeedsBringIn = true;
            var halfBet = Table.Params.GameSize / 2;
            Table.HigherBet = halfBet;
            Table.MinimumRaiseAmount = halfBet;
        }

        public override bool OnMoneyPlayed(PlayerInfo p, int amnt)
        {
            if (Variant.NeedsBringIn)
            {
                LogManager.Log(LogLevel.MessageVeryLow, "StudGame.PlayMoney", "Currently, we need {0}, the bring in !!", Table.CallAmnt(p));
                if (amnt == -1)
                    return false;
            }

            var ok = base.OnMoneyPlayed(p, amnt);

            if (!ok || !Variant.NeedsBringIn)
                return ok;

            Table.MinimumRaiseAmount = Table.Params.GameSize;
            Variant.NeedsBringIn = false;
            return true;
        }
    }
}
