using System;
using BluffinMuffin.HandEvaluator.Enums;
using BluffinMuffin.Server.DataTypes.Attributes;
using BluffinMuffin.Server.DataTypes.Enums;
using BluffinMuffin.Server.Logic.GameModules;

namespace BluffinMuffin.Server.Logic.GameVariants
{
    [GameVariant(GameVariantEnum.CrazyPineapple)]
    public class CrazyPineappleVariant : AbstractGameVariant
    {
        public override int NbCardsInHand => 3;

        public override CardSelectionEnum CardSelectionType => CardSelectionEnum.AllPlayerAndAllCommunity;

        public override Type InitModuleType => typeof (InitPineappleGameModule);
    }
}
