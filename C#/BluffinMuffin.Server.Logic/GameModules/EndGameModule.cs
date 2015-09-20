using BluffinMuffin.Server.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes.EventHandling;

namespace BluffinMuffin.Server.Logic.GameModules
{
    public class EndGameModule : AbstractGameModule
    {
        public EndGameModule(PokerGameObserver o, PokerTable table)
            : base(o, table)
        {
        }

        public override GameStateEnum GameState => GameStateEnum.End;

        public override void InitModule()
        {
            Observer.RaiseEverythingEnded();
        }
    }
}
