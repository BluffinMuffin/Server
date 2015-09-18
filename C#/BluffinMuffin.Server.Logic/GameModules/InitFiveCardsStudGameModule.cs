using BluffinMuffin.Server.DataTypes.EventHandling;

namespace BluffinMuffin.Server.Logic.GameModules
{
    public class InitFiveCardsStudGameModule : AbstractInitGameModule
    {
        public InitFiveCardsStudGameModule(PokerGameObserver o, PokerTable table)
            : base(o, table)
        {
        }

        public override void InitModulePlaying()
        {
            AddModule(new DealCardsToPlayersModule(Observer, Table, 1, 1));
            AddModule(new StudFirstBettingRoundModule(Observer, Table));
            AddModule(new CumulPotsModule(Observer, Table));

            AddModule(new DealCardsToPlayersModule(Observer, Table, 0, 1));
            AddModule(new BettingRoundModule(Observer, Table));
            AddModule(new CumulPotsModule(Observer, Table));

            AddModule(new DealCardsToPlayersModule(Observer, Table, 0, 1));
            AddModule(new BettingRoundModule(Observer, Table));
            AddModule(new CumulPotsModule(Observer, Table));

            AddModule(new DealCardsToPlayersModule(Observer, Table, 0, 1));
            AddModule(new BettingRoundModule(Observer, Table));
            AddModule(new CumulPotsModule(Observer, Table));
        }
    }
}
