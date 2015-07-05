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
    class DealCardsToBoardModule : AbstractGameModule
    {
        protected int NbCards { get; private set; }
        public DealCardsToBoardModule(PokerGameObserver o, PokerTable table, int nbCards)
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
            if (Table.NoMoreRoundsNeeded)
            {
                RaiseCompleted();
                return;
            }
            
            Table.AddCards(Table.Dealer.DealCards(NbCards).Select(x => x.ToString()).ToArray());
            RaiseCompleted();
        }
    }
}
