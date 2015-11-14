using System.Linq;
using BluffinMuffin.Server.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes.EventHandling;
using BluffinMuffin.Server.Logic.Extensions;

namespace BluffinMuffin.Server.Logic.GameModules
{
    public class InitGameModule : AbstractGameModule
    {
        public InitGameModule(PokerGameObserver o, PokerTable table) : base(o, table)
        {
        }

        public override GameStateEnum GameState => GameStateEnum.Init;

        private void InitModuleBegginning()
        {
            Table.NoMoreRoundsNeeded = false;
            Table.BettingRoundId = 0;
            Table.Seats.Players().ToList().ForEach(x => x.FaceDownCards = new string[0]);
            Table.Seats.Players().ToList().ForEach(x => x.FaceUpCards = new string[0]);
            Table.InitCards();

            AddModule(new WaitForPlayerModule(Observer, Table));
            AddModule(new WaitForBlindsModule(Observer, Table));
        }

        private void InitModuleEnding()
        {
            AddModule(new ShowDownModule(Observer, Table));
            AddModule(new DecideWinnersModule(Observer, Table));
            AddModule(new DistributeMoneyModule(Observer, Table));
            RaiseCompleted();
        }

        public override void InitModule()
        {
            InitModuleBegginning();
            foreach (var m in Table.Variant.GetModules(Observer, Table))
                AddModule(m);
            InitModuleEnding();
        }
    }
}
