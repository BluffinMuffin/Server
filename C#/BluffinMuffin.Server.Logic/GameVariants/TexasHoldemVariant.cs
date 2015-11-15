using System.Diagnostics.CodeAnalysis;
using BluffinMuffin.Protocol.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes.Attributes;

namespace BluffinMuffin.Server.Logic.GameVariants
{
    [GameVariant(GameSubTypeEnum.TexasHoldem)]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class TexasHoldemVariant : AbstractHoldemGameVariant
    {
      
    }
}
