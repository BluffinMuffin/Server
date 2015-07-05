using System;
using System.Linq;
using System.Threading;
using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Protocol.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes.EventHandling;
using Com.Ericmas001.Util;

namespace BluffinMuffin.Server.Logic.GameModules
{
    class FirstBettingRoundModule : BettingRoundModule
    {
        public FirstBettingRoundModule(PokerGameObserver o, PokerTable table)
            : base(o, table, RoundTypeEnum.Preflop.ToString())
        {
        }

        protected override SeatInfo GetSeatOfTheFirstPlayer()
        {
            if (Table.Params.Blind.OptionType == BlindTypeEnum.Blinds)
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
