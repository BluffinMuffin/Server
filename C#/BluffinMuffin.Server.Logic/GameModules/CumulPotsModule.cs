using BluffinMuffin.Server.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes.EventHandling;

namespace BluffinMuffin.Server.Logic.GameModules
{
    public class CumulPotsModule : AbstractGameModule
    {
        public CumulPotsModule(PokerGameObserver o, PokerTable table)
            : base(o, table)
        {
        }

        public override GameStateEnum GameState => GameStateEnum.Playing;

        public override void InitModule()
        {
            if (Table.NoMoreRoundsNeeded)
            {
                RaiseCompleted();
                return;
            }
            Table.ManagePotsRoundEnd();

            Observer.RaiseGameBettingRoundEnded();

            if (Table.NbPlayingAndAllIn <= 1)
                Table.NoMoreRoundsNeeded = true;

            RaiseCompleted();
        }
    }
}
