using System.Diagnostics.CodeAnalysis;
using BluffinMuffin.Protocol.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes.Attributes;

namespace BluffinMuffin.Server.Logic.GameVariants
{
    [GameVariant(GameSubTypeEnum.ThreeCardsHoldem)]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class ThreeCardsHoldemVariant : AbstractHoldemGameVariant
    {
        protected override int NbCardsInHand => 3;
    }
}
