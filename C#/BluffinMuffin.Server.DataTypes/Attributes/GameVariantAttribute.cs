using System;
using BluffinMuffin.Protocol.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes.Enums;

namespace BluffinMuffin.Server.DataTypes.Attributes
{
    public class GameVariantAttribute : Attribute
    {
        public GameSubTypeEnum Variant { get; private set; }

        public GameVariantAttribute(GameSubTypeEnum variant)
        {
            Variant = variant;
        }
    }
}
