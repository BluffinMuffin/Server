using BluffinMuffin.Protocol.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes.Attributes;

namespace BluffinMuffin.Server.Logic.GameVariants
{
    [GameVariant(GameSubTypeEnum.ThreeCardsHoldem)]
    public class ThreeCardsHoldemVariant : AbstractHoldemGameVariant
    {
        public override int NbCardsInHand => 3;
    }
}
