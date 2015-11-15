using System.Linq;
using BluffinMuffin.Server.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes.EventHandling;
using BluffinMuffin.Server.Logic.Extensions;

namespace BluffinMuffin.Server.Logic.GameModules
{
    public class DealMissingCardsToPlayersModule : AbstractGameModule
    {
        private int NbCardsFaceDown { get; }
        private int NbCardsFaceUp { get; }
        public DealMissingCardsToPlayersModule(PokerGameObserver o, PokerTable table, int nbCardsFaceDown, int nbCardsFaceUp = 0)
            : base(o, table)
        {
            NbCardsFaceDown = nbCardsFaceDown;
            NbCardsFaceUp = nbCardsFaceUp;
        }

        public override GameStateEnum GameState => GameStateEnum.Playing;

        public override void InitModule()
        {
            foreach (var p in Table.Seats.PlayingAndAllInPlayers())
            {
                string[] downCards = p.FaceDownCards?.Where(x => !string.IsNullOrEmpty(x)).ToArray() ?? new string[0];
                p.FaceDownCards = downCards.Union(Table.Variant.Dealer.DealCards(NbCardsFaceDown - downCards.Length).Select(x => x.ToString())).ToArray();
                string[] upCards = p.FaceUpCards?.Where(x => !string.IsNullOrEmpty(x)).ToArray() ?? new string[0];
                p.FaceUpCards = upCards.Union(Table.Variant.Dealer.DealCards(NbCardsFaceUp - upCards.Length).Select(x => x.ToString())).ToArray();
                Observer.RaisePlayerHoleCardsChanged(p);
            }
            RaiseCompleted();
        }
    }
}
