using BluffinMuffin.Server.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes.EventHandling;

namespace BluffinMuffin.Server.Logic.GameModules
{
    public class InitFiveCardsDrawGameModule : AbstractInitGameModule
    {
        public InitFiveCardsDrawGameModule(PokerGameObserver o, PokerTable table)
            : base(o, table)
        {
        }

        public override void InitModulePlaying()
        {
            AddModule(new DealMissingCardsToPlayersModule(Observer, Table, Table.Variant.NbCardsInHand));
            AddModule(new FirstBettingRoundModule(Observer, Table));
            AddModule(new CumulPotsModule(Observer, Table));

            AddModule(new DiscardRoundModule(Observer, Table, 0, 5));

            AddModule(new DealMissingCardsToPlayersModule(Observer, Table, Table.Variant.NbCardsInHand));
            AddModule(new BettingRoundModule(Observer, Table));
            AddModule(new CumulPotsModule(Observer, Table));

        }
    }
}
