using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Protocol.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes.EventHandling;

namespace BluffinMuffin.Server.Logic.GameModules
{
    class InitTexasHoldemGameModule : AbstractGameModule
    {
        public InitTexasHoldemGameModule(PokerGameObserver o, PokerTable table) : base(o, table)
        {
        }

        public override GameStateEnum GameState
        {
            get { return GameStateEnum.Init; }
        }

        public override void InitModule()
        {
            Table.NoMoreRoundsNeeded = false;
            Table.BettingRoundId = 0;

            AddModule(new WaitForPlayerModule(Observer, Table));
            AddModule(new WaitForBlindsModule(Observer, Table));

            //Preflop
            AddModule(new DealCardsToPlayersModule(Observer, Table, 2));
            AddModule(new FirstBettingRoundModule(Observer, Table));
            AddModule(new CumulPotsModule(Observer, Table));
            
            //Flop
            AddModule(new DealCardsToBoardModule(Observer, Table, 3));
            AddModule(new BettingRoundModule(Observer, Table));
            AddModule(new CumulPotsModule(Observer, Table));
            
            //Turn
            AddModule(new DealCardsToBoardModule(Observer, Table, 1));
            AddModule(new BettingRoundModule(Observer, Table));
            AddModule(new CumulPotsModule(Observer, Table));
            
            //River
            AddModule(new DealCardsToBoardModule(Observer, Table, 1));
            AddModule(new BettingRoundModule(Observer, Table));
            AddModule(new CumulPotsModule(Observer, Table));
            
            AddModule(new ShowDownModule(Observer, Table));
            AddModule(new DecideWinnersModule(Observer, Table));
            AddModule(new DistributeMoneyModule(Observer, Table));
            RaiseCompleted();
        }
    }
}
