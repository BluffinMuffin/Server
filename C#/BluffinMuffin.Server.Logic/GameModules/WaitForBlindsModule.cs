using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Protocol.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes;
using BluffinMuffin.Server.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes.EventHandling;
using BluffinMuffin.Server.Logic.Extensions;

namespace BluffinMuffin.Server.Logic.GameModules
{
    public class WaitForBlindsModule : AbstractGameModule
    {
        public WaitForBlindsModule(PokerGameObserver o, PokerTable table)
            : base(o, table)
        {
        }

        public override GameStateEnum GameState => GameStateEnum.WaitForBlinds;

        public override void InitModule()
        {
            Table.HigherBet = 0;
            DidWeGetAllWeNeeded();
        }

        public override bool OnMoneyPlayed(PlayerInfo p, int amnt)
        {
            Logger.LogDebugInformation("Total blinds needed is {0}", Table.Bank.TotalDebtAmount);
            Logger.LogDebugInformation("{0} is putting blind of {1}", p.Name, amnt);

            //What is the need Blind from the player ?
            var needed = Table.Bank.DebtAmount(p);

            //If the player isn't giving what we expected from him
            if (amnt != needed)
            {
                //If the player isn't playing enough but it's all he got, time to go All-In
                if (amnt < needed && !p.CanBet(amnt + 1))
                {
                    Logger.LogDebugInformation("Player now All-In !");
                    p.State = PlayerStateEnum.AllIn;
                }
                else //well, it's just not fair to play that
                {
                    Logger.LogWarning("{0} needed to put a blind of {1} and tried {2}", p.Name, needed, amnt);
                    return false;
                }
            }

            //Let's hope the player has enough money ! Time to put the blinds !
            if (!Table.Bank.CollectMoneyFromPlayer(p, amnt))
            {
                Logger.LogWarning("{0} just put more money than he actually have ({1} > {2})", p.Name, amnt, p.MoneySafeAmnt);
                return false;
            }

            //Take note of the action
            var whatAmIDoing = GameActionEnum.PostAnte;
            if (Table.Params.Blind == BlindTypeEnum.Blinds)
                whatAmIDoing = (needed == Table.Params.BigBlindAmount() ? GameActionEnum.PostBigBlind : GameActionEnum.PostSmallBlind);

            Logger.LogDebugInformation("{0} POSTED BLIND ({1})", p.Name, whatAmIDoing);
            Observer.RaisePlayerActionTaken(p, whatAmIDoing, amnt);

            //Let's set the HigherBet
            if (amnt > Table.HigherBet)
                Table.HigherBet = amnt;

            Logger.LogDebugInformation("Total blinds still needed is {0}", Table.Bank.TotalDebtAmount);

            DidWeGetAllWeNeeded();
            return true;
        }

        private void DidWeGetAllWeNeeded()
        {
            if (Table.Bank.TotalDebtAmount == 0)
                RaiseCompleted();
        }
    }
}
