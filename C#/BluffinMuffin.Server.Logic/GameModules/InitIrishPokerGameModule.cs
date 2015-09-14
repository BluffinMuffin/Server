using BluffinMuffin.Server.DataTypes.EventHandling;

namespace BluffinMuffin.Server.Logic.GameModules
{
    public class InitIrishPokerGameModule : AbstractInitGameModule
    {
        public InitIrishPokerGameModule(PokerGameObserver o, PokerTable table)
            : base(o, table)
        {
        }
        
        public override void InitModulePlaying()
        {
            //Preflop
            AddModule(new DealMissingCardsToPlayersModule(Observer, Table, Table.Variant.NbCardsInHand));
            AddModule(new FirstBettingRoundModule(Observer, Table));
            AddModule(new CumulPotsModule(Observer, Table));

            //Flop
            AddModule(new DealCardsToBoardModule(Observer, Table, 3));
            AddModule(new BettingRoundModule(Observer, Table));
            AddModule(new CumulPotsModule(Observer, Table));

            //Discard 2 to go back to 2 hole cards
            AddModule(new DiscardRoundModule(Observer, Table, 2, 2));

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
