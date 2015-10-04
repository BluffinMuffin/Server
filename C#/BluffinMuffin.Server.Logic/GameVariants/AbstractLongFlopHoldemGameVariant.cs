using System.Collections.Generic;
using BluffinMuffin.Server.DataTypes;
using BluffinMuffin.Server.DataTypes.EventHandling;
using BluffinMuffin.Server.Logic.GameModules;

namespace BluffinMuffin.Server.Logic.GameVariants
{
    public abstract class AbstractLongFlopHoldemGameVariant : AbstractHoldemGameVariant
    {

        public override IEnumerable<IGameModule> GetModules(PokerGameObserver o, PokerTable t)
        {
            //Preflop
            yield return new DealMissingCardsToPlayersModule(o, t, NbCardsInHand);
            yield return new FirstBettingRoundModule(o, t);
            yield return new CumulPotsModule(o, t);

            //Flop 1
            yield return new DealCardsToBoardModule(o, t, 1);
            yield return new BettingRoundModule(o, t);
            yield return new CumulPotsModule(o, t);

            //Flop 2
            yield return new DealCardsToBoardModule(o, t, 1);
            yield return new BettingRoundModule(o, t);
            yield return new CumulPotsModule(o, t);

            //Flop 3
            yield return new DealCardsToBoardModule(o, t, 1);
            yield return new BettingRoundModule(o, t);
            yield return new CumulPotsModule(o, t);

            //Turn
            yield return new DealCardsToBoardModule(o, t, 1);
            yield return new BettingRoundModule(o, t);
            yield return new CumulPotsModule(o, t);

            //River
            yield return new DealCardsToBoardModule(o, t, 1);
            yield return new BettingRoundModule(o, t);
            yield return new CumulPotsModule(o, t);
        }
    }
}
