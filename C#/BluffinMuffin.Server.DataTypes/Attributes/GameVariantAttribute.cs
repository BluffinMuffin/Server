using System;
using BluffinMuffin.Server.DataTypes.Enums;

namespace BluffinMuffin.Server.DataTypes.Attributes
{
    public class GameVariantAttribute : Attribute
    {
        public GameVariantEnum Variant { get; private set; }

        public GameVariantAttribute(GameVariantEnum variant)
        {
            Variant = variant;
        }
    }
}
