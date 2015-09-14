using System;
using BluffinMuffin.HandEvaluator;
using BluffinMuffin.HandEvaluator.Selectors;
using BluffinMuffin.Protocol.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes;
using BluffinMuffin.Server.DataTypes.Attributes;
using BluffinMuffin.Server.Logic.GameModules;

namespace BluffinMuffin.Server.Logic.GameVariants
{
    [GameVariant(GameSubTypeEnum.FiveCardsDraw)]
    public class FiveCardsDrawVariant : AbstractGameVariant
    {
        public override int NbCardsInHand => 5;

        public override EvaluationParams EvaluationParms => new EvaluationParams {Selector = new OnlyHoleCardsSelector()};

        public override Type InitModuleType => typeof(InitFiveCardsDrawGameModule);
    }
}
