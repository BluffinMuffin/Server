using System;
using BluffinMuffin.HandEvaluator;
using BluffinMuffin.HandEvaluator.Selectors;
using BluffinMuffin.Protocol.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes.Attributes;
using BluffinMuffin.Server.Logic.GameModules;

namespace BluffinMuffin.Server.Logic.GameVariants
{
    [GameVariant(GameSubTypeEnum.FiveCardsStud)]
    public class FiveCardsStudVariant : AbstractStudGameVariant
    {
        public override int NbCardsInHand => 5;

        public override Type InitModuleType => typeof(InitFiveCardsStudGameModule);
    }
}
