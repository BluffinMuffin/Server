using System.Linq;
using BluffinMuffin.Server.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes.EventHandling;

namespace BluffinMuffin.Server.Logic.GameModules
{
    public class DealCardsToBoardModule : AbstractGameModule
    {
        protected int NbCards { get; }
        public DealCardsToBoardModule(PokerGameObserver o, PokerTable table, int nbCards)
            : base(o, table)
        {
            NbCards = nbCards;
        }

        public override GameStateEnum GameState => GameStateEnum.Playing;

        public override void InitModule()
        {
            if (Table.NoMoreRoundsNeeded)
            {
                RaiseCompleted();
                return;
            }
            
            Table.AddCards(Table.Variant.Dealer.DealCards(NbCards).Select(x => x.ToString()).ToArray());
            RaiseCompleted();
        }
    }
}
