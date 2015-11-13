using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Protocol.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes.EventHandling;
using BluffinMuffin.Server.Logic.Extensions;

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
            if (Table.Params.Blind == BlindTypeEnum.Blinds)
            {
                //Ad B : A      A
                //Ad B C: A     A->B->C->A
                //Ad B C D: D   A->B->C->D
                return Table.NbPlayingAndAllIn < 3 ? Table.Seats.SeatOfDealer() : Table.Seats.SeatOfPlayingPlayerNextTo(Table.Seats.SeatOfPlayingPlayerNextTo(Table.Seats.SeatOfPlayingPlayerNextTo(Table.Seats.SeatOfDealer())));
            }

            return base.GetSeatOfTheFirstPlayer();
        }
    }
}
