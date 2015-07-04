using System.Linq;
using System.Threading;
using BluffinMuffin.Server.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes.EventHandling;

namespace BluffinMuffin.Server.Logic.GameModules
{
    class DistributeMoneyModule : AbstractGameModule
    {
        public DistributeMoneyModule(PokerGameObserver o, PokerTable table)
            : base(o, table)
        {
        }

        public override GameStateEnum GameState
        {
            get { return GameStateEnum.DistributeMoney; }
        }

        public override void InitModule()
        {
            foreach (var pot in Table.Pots)
            {
                var players = pot.AttachedPlayers;
                if (players.Length > 0)
                {
                    var wonAmount = pot.Amount / players.Length;
                    if (wonAmount > 0)
                    {
                        foreach (var p in players)
                        {
                            p.Player.MoneySafeAmnt += wonAmount;
                            Observer.RaisePlayerWonPot(p, pot, wonAmount);
                            WaitALittle(Table.Params.WaitingTimes.AfterPotWon);
                        }
                    }
                }
            }
            RaiseCompleted();
        }
        private void WaitALittle(int waitingTime)
        {
            Thread.Sleep(waitingTime);
        }
    }
}
