using BluffinMuffin.HandEvaluator;
using BluffinMuffin.HandEvaluator.Selectors;
using BluffinMuffin.Protocol.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes.Attributes;

namespace BluffinMuffin.Server.Logic.GameVariants
{
    [GameVariant(GameSubTypeEnum.OmahaHoldem)]
    public class OmahaHoldemVariant : AbstractGameVariant
    {
        public override int NbCardsInHand => 4;

        public override EvaluationParams EvaluationParms => new EvaluationParams
        {
            Selector = new Use2Player3CommunitySelector()
        };
    }
}
