using System.Linq;
using BluffinMuffin.HandEvaluator;
using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Protocol.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes;
using BluffinMuffin.Server.DataTypes.EventHandling;

namespace BluffinMuffin.Server.Logic.GameModules
{
    public class FirstBettingRoundModule : BettingRoundModule
    {
        public FirstBettingRoundModule(PokerGameObserver o, PokerTable table)
            : base(o, table)
        {
        }

        protected override SeatInfo GetSeatOfTheFirstPlayer()
        {
            if (Table.Params.Options.OptionType == GameTypeEnum.StudPoker)
            {
                return Table.Seats[HandEvaluators.Evaluate(Table.PlayingPlayers.Select(p => new CardHolder(p, p.FaceUpCards, new string[0])).Cast<IStringCardsHolder>().ToArray(), new EvaluationParams { UseSuitRanking = true }).Last().Select(x => x.CardsHolder).Cast<CardHolder>().First().Player.NoSeat];
            }

            if (Table.Params.Blind == BlindTypeEnum.Blinds)
            {
                //Ad B : A      A
                //Ad B C: A     A->B->C->A
                //Ad B C D: D   A->B->C->D
                return Table.NbPlayingAndAllIn < 3 ? Table.DealerSeat : Table.GetSeatOfPlayingPlayerNextTo(Table.GetSeatOfPlayingPlayerNextTo(Table.GetSeatOfPlayingPlayerNextTo(Table.DealerSeat)));
            }

            return base.GetSeatOfTheFirstPlayer();
        }
    }
}
