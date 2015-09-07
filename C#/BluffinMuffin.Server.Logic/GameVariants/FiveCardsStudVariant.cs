using System;
using BluffinMuffin.HandEvaluator.Enums;
using BluffinMuffin.Protocol.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes.Attributes;
using BluffinMuffin.Server.DataTypes.Enums;
using BluffinMuffin.Server.Logic.GameModules;

namespace BluffinMuffin.Server.Logic.GameVariants
{
    [GameVariant(GameVariantEnum.FiveCardsStud)]
    public class FiveCardsStudVariant : AbstractGameVariant
    {
        public override int NbCardsInHand => 5;

        public override GameTypeEnum GameType => GameTypeEnum.StudPoker;

        public override CardSelectionEnum CardSelectionType => CardSelectionEnum.AllPlayerAndAllCommunity;

        public override Type InitModuleType => typeof(InitFiveCardsStudGameModule);
    }
}
