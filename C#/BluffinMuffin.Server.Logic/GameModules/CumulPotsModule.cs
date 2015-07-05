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
    class CumulPotsModule : AbstractGameModule
    {
        public CumulPotsModule(PokerGameObserver o, PokerTable table)
            : base(o, table)
        {
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
            Table.ManagePotsRoundEnd();

            Observer.RaiseGameBettingRoundEnded(Table.Round);

            if (Table.NbPlayingAndAllIn <= 1)
                Table.NoMoreRoundsNeeded = true;

            RaiseCompleted();
        }
    }
}
