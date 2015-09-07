using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Protocol.DataTypes.Enums;
using BluffinMuffin.Protocol.DataTypes.Options;
using BluffinMuffin.Server.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes.EventHandling;
using Com.Ericmas001.Util;

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
            LogManager.Log(LogLevel.MessageVeryLow, "PokerGame.PlayMoney", "Total blinds needed is {0}", Table.TotalBlindNeeded);
            LogManager.Log(LogLevel.MessageVeryLow, "PokerGame.PlayMoney", "{0} is putting blind of {1}", p.Name, amnt);

            //What is the need Blind from the player ?
            var needed = Table.GetBlindNeeded(p);

            //If the player isn't giving what we expected from him
            if (amnt != needed)
            {
                //If the player isn't playing enough but it's all he got, time to go All-In
                if (amnt < needed && !p.CanBet(amnt + 1))
                {
                    LogManager.Log(LogLevel.MessageVeryLow, "PokerGame.PlayMoney", "Player now All-In !");
                    p.State = PlayerStateEnum.AllIn;
                    Table.NbAllIn++;
                    Table.AddAllInCap(p.MoneyBetAmnt + amnt);
                }
                else //well, it's just not fair to play that
                {
                    LogManager.Log(LogLevel.Warning, "PokerGame.PlayMoney", "{0} needed to put a blind of {1} and tried {2}", p.Name, needed, amnt);
                    return false;
                }
            }

            //Let's hope the player has enough money ! Time to put the blinds !
            if (!p.TryBet(amnt))
            {
                LogManager.Log(LogLevel.Warning, "PokerGame.PlayMoney", "{0} just put more money than he actually have ({1} > {2})", p.Name, amnt, p.MoneySafeAmnt);
                return false;
            }

            //Hmmm ... More Money !! 
            Table.TotalPotAmnt += amnt;

            //Take note of the given Blind Amount for the player.
            Table.SetBlindNeeded(p, 0);

            //Take note of the action
            var whatAmIDoing = GameActionEnum.PostAnte;
            if (Table.Params.Blind.OptionType == BlindTypeEnum.Blinds)
            {
                var bob = Table.Params.Blind as BlindOptionsBlinds;
                if (bob != null && needed == bob.SmallBlindAmount)
                    whatAmIDoing = GameActionEnum.PostSmallBlind;
                else
                    whatAmIDoing = GameActionEnum.PostBigBlind;
            }
            LogManager.Log(LogLevel.MessageLow, "PokerGame.PlayMoney", "{0} POSTED BLIND ({1})", p.Name, whatAmIDoing);
            Observer.RaisePlayerActionTaken(p, whatAmIDoing, amnt);

            //Let's set the HigherBet
            if (amnt > Table.HigherBet)
                Table.HigherBet = amnt;

            LogManager.Log(LogLevel.MessageVeryLow, "PokerGame.PlayMoney", "Total blinds still needed is {0}", Table.TotalBlindNeeded);

            DidWeGetAllWeNeeded();
            return true;
        }

        private void DidWeGetAllWeNeeded()
        {
            if (Table.TotalBlindNeeded == 0)
                RaiseCompleted();
        }
    }
}
