using System.Linq;
using BluffinMuffin.Server.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes.EventHandling;
using BluffinMuffin.Server.Logic.Extensions;

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

            Table.Bank.DepositMoneyInPlay();
            Table.HigherBet = 0;

            Observer.RaiseGameBettingRoundEnded();

            if (Table.Seats.PlayingAndAllInPlayers().Count() <= 1)
                Table.NoMoreRoundsNeeded = true;

            RaiseCompleted();
        }
    }
}
