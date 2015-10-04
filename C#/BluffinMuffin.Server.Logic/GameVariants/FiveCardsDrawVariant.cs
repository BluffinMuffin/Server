using System.Collections.Generic;
using BluffinMuffin.HandEvaluator;
using BluffinMuffin.HandEvaluator.Selectors;
using BluffinMuffin.Protocol.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes;
using BluffinMuffin.Server.DataTypes.Attributes;
using BluffinMuffin.Server.DataTypes.EventHandling;
using BluffinMuffin.Server.Logic.GameModules;

namespace BluffinMuffin.Server.Logic.GameVariants
{
    [GameVariant(GameSubTypeEnum.FiveCardsDraw)]
    public class FiveCardsDrawVariant : AbstractGameVariant
    {
        public override int NbCardsInHand => 5;

        public override EvaluationParams EvaluationParms => new EvaluationParams
        {
            Selector = new OnlyHoleCardsSelector()
        };

        public override IEnumerable<IGameModule> GetModules(PokerGameObserver o, PokerTable t)
        {
            yield return new DealMissingCardsToPlayersModule(o, t, NbCardsInHand);
            yield return new FirstBettingRoundModule(o, t);
            yield return new CumulPotsModule(o, t);

            yield return new DiscardRoundModule(o, t, 0, 5);

            yield return new DealMissingCardsToPlayersModule(o, t, NbCardsInHand);
            yield return new BettingRoundModule(o, t);
            yield return new CumulPotsModule(o, t);
        }
    }
}
