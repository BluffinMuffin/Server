using BluffinMuffin.Server.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes.EventHandling;

namespace BluffinMuffin.Server.Logic.GameModules
{
    public class InitGameModule : AbstractGameModule
    {
        public InitGameModule(PokerGameObserver o, PokerTable table) : base(o, table)
        {
        }

        public override GameStateEnum GameState => GameStateEnum.Init;

        public virtual void InitModuleBegginning()
        {
            Table.NoMoreRoundsNeeded = false;
            Table.BettingRoundId = 0;
            Table.Players.ForEach(x => x.FaceDownCards = new string[0]);
            Table.Players.ForEach(x => x.FaceUpCards = new string[0]);
            Table.InitCards();

            AddModule(new WaitForPlayerModule(Observer, Table));
            AddModule(new WaitForBlindsModule(Observer, Table));
        }

        public virtual void InitModuleEnding()
        {
            AddModule(new ShowDownModule(Observer, Table));
            AddModule(new DecideWinnersModule(Observer, Table));
            AddModule(new DistributeMoneyModule(Observer, Table));
            RaiseCompleted();
        }

        public override void InitModule()
        {
            InitModuleBegginning();
            Table.Variant.GetModules(Observer, Table);
            InitModuleEnding();
        }
    }
}
