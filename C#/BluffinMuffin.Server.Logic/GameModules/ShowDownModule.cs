using System.Linq;
using BluffinMuffin.Server.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes.EventHandling;
using BluffinMuffin.Server.Logic.Extensions;

namespace BluffinMuffin.Server.Logic.GameModules
{
    public class ShowDownModule : AbstractGameModule
    {
        public ShowDownModule(PokerGameObserver o, PokerTable table)
            : base(o, table)
        {
        }

        public override GameStateEnum GameState => GameStateEnum.Showdown;

        public override void InitModule()
        {
            foreach (var p in Table.Players.Where(p => p.IsPlayingOrAllIn()))
            {
                p.FaceUpCards = p.FaceUpCards.Concat(p.FaceDownCards).ToArray();
                p.FaceDownCards = new string[0];
                Observer.RaisePlayerHoleCardsChanged(p);
            }
            RaiseCompleted();
        }
    }
}
