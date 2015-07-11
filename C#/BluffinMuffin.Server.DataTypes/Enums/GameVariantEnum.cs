using System.ComponentModel;

namespace BluffinMuffin.Server.DataTypes.Enums
{
    public enum GameVariantEnum
    {
        [Description("Texas Hold'em")]
        TexasHoldem,

        [Description("Omaha Holdem")]
        OmahaHoldem
    }
}
