using System;
using BluffinMuffin.HandEvaluator.Enums;
using BluffinMuffin.Server.DataTypes.Attributes;
using BluffinMuffin.Server.DataTypes.Enums;
using BluffinMuffin.Server.Logic.GameModules;

namespace BluffinMuffin.Server.Logic.GameVariants
{
    [GameVariant(GameVariantEnum.OmahaHoldem)]
    public class OmahaHoldemVariant : AbstractGameVariant
    {
        public override int NbCardsInHand => 4;

        public override CardSelectionEnum CardSelectionType => CardSelectionEnum.TwoPlayersAndThreeCommunity;

        public override Type InitModuleType => typeof (InitHoldemGameModule);
    }
}
