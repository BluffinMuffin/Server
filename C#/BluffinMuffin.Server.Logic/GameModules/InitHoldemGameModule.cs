using BluffinMuffin.Server.DataTypes.EventHandling;

namespace BluffinMuffin.Server.Logic.GameModules
{
    public class InitHoldemGameModule : AbstractInitGameModule
    {
        public InitHoldemGameModule(PokerGameObserver o, PokerTable table) : base(o, table)
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
