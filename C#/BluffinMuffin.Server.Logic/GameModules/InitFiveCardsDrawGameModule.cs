using BluffinMuffin.Server.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes.EventHandling;

namespace BluffinMuffin.Server.Logic.GameModules
{
    public class InitFiveCardsDrawGameModule : AbstractGameModule
    {
        public InitFiveCardsDrawGameModule(PokerGameObserver o, PokerTable table)
            : base(o, table)
        {
        }

        public override GameStateEnum GameState => GameStateEnum.Init;

        public override void InitModule()
        {
            Table.NoMoreRoundsNeeded = false;
            Table.BettingRoundId = 0;
            Table.Players.ForEach(x => x.Cards = new string[0]);

            AddModule(new WaitForPlayerModule(Observer, Table));
            AddModule(new WaitForBlindsModule(Observer, Table));

            AddModule(new DealCardsToPlayersModule(Observer, Table, Table.Variant.NbCardsInHand));
            AddModule(new FirstBettingRoundModule(Observer, Table));
            AddModule(new CumulPotsModule(Observer, Table));

            AddModule(new DiscardRoundModule(Observer, Table, 0, 5));

            AddModule(new DealCardsToPlayersModule(Observer, Table, Table.Variant.NbCardsInHand));
            AddModule(new BettingRoundModule(Observer, Table));
            AddModule(new CumulPotsModule(Observer, Table));
            
            AddModule(new ShowDownModule(Observer, Table));
            AddModule(new DecideWinnersModule(Observer, Table));
            AddModule(new DistributeMoneyModule(Observer, Table));
            RaiseCompleted();
        }
    }
}
