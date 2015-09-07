using System.Linq;
using BluffinMuffin.Server.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes.EventHandling;

namespace BluffinMuffin.Server.Logic.GameModules
{
    public class DealCardsToPlayersModule : AbstractGameModule
    {
        protected int NbCards { get; }
        public DealCardsToPlayersModule(PokerGameObserver o, PokerTable table, int nbCards)
            : base(o, table)
        {
            NbCards = nbCards;
        }

        public override GameStateEnum GameState => GameStateEnum.Playing;

        public override void InitModule()
        {
            foreach (var p in Table.PlayingAndAllInPlayers)
            {
                string[] cards = p.FaceDownCards?.Where(x => !string.IsNullOrEmpty(x)).ToArray() ?? new string[0];
                p.FaceDownCards = cards.Union(Table.Dealer.DealCards(NbCards - cards.Length).Select(x => x.ToString())).ToArray();
                Observer.RaisePlayerHoleCardsChanged(p);
            }
            RaiseCompleted();
        }
    }
}
