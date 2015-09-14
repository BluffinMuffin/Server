using BluffinMuffin.Server.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes.EventHandling;

namespace BluffinMuffin.Server.Logic.GameModules
{
    public class InitPineappleGameModule : AbstractInitGameModule
    {
        public InitPineappleGameModule(PokerGameObserver o, PokerTable table)
            : base(o, table)
        {
        }
        
        public override void InitModulePlaying()
        {
            //Preflop
            AddModule(new DealMissingCardsToPlayersModule(Observer, Table, Table.Variant.NbCardsInHand));

            //Discard 1 to go back to 2 hole cards
            AddModule(new DiscardRoundModule(Observer, Table, 1, 1));

            AddModule(new FirstBettingRoundModule(Observer, Table));
            AddModule(new CumulPotsModule(Observer, Table));

            //Flop
            AddModule(new DealCardsToBoardModule(Observer, Table, 3));
            AddModule(new BettingRoundModule(Observer, Table));
            AddModule(new CumulPotsModule(Observer, Table));

            //Turn
            AddModule(new DealCardsToBoardModule(Observer, Table, 1));
            AddModule(new BettingRoundModule(Observer, Table));
            AddModule(new CumulPotsModule(Observer, Table));

            //River
            AddModule(new DealCardsToBoardModule(Observer, Table, 1));
            AddModule(new BettingRoundModule(Observer, Table));
            AddModule(new CumulPotsModule(Observer, Table));
        }
    }
}
