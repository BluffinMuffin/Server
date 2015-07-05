using System;
using System.Linq;
using System.Threading;
using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Protocol.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes.EventHandling;
using Com.Ericmas001.Util;

namespace BluffinMuffin.Server.Logic.GameModules
{
    class DealCardsToPlayersModule : AbstractGameModule
    {
        protected int NbCards { get; private set; }
        public DealCardsToPlayersModule(PokerGameObserver o, PokerTable table, int nbCards)
            : base(o, table)
        {
            NbCards = nbCards;
        }

        public override GameStateEnum GameState
        {
            get { return GameStateEnum.Playing; }
        }

        public override void InitModule()
        {
            foreach (var p in Table.PlayingAndAllInPlayers)
            {
                p.HoleCards = Table.Dealer.DealCards(NbCards).Select(x => x.ToString()).ToArray();
                Observer.RaisePlayerHoleCardsChanged(p);
            }
            RaiseCompleted();
        }
    }
}
