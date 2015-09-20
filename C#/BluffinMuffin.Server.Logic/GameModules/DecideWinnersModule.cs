using BluffinMuffin.Server.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes.EventHandling;

namespace BluffinMuffin.Server.Logic.GameModules
{
    public class DecideWinnersModule : AbstractGameModule
    {
        public DecideWinnersModule(PokerGameObserver o, PokerTable table)
            : base(o, table)
        {
        }

        public override GameStateEnum GameState => GameStateEnum.DecideWinners;

        public override void InitModule()
        {
            Table.CleanPotsForWinning();
            RaiseCompleted();
        }
    }
}
