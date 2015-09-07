using System.Linq;
using BluffinMuffin.Server.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes.EventHandling;

namespace BluffinMuffin.Server.Logic.GameModules
{
    public class ShowDownModule : AbstractGameModule
    {
        public ShowDownModule(PokerGameObserver o, PokerTable table)
            : base(o, table)
        {
        }

        public override GameStateEnum GameState
        {
            get { return GameStateEnum.Showdown; }
        }

        public override void InitModule()
        {
            foreach (var p in Table.Players.Where(p => p.IsPlaying || p.IsAllIn))
            {
                p.IsShowingCards = true;
                p.NbHiddenCards = 0;
                Observer.RaisePlayerHoleCardsChanged(p);
            }
            RaiseCompleted();
        }
    }
}
