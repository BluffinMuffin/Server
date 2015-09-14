using System;
using BluffinMuffin.HandEvaluator;
using BluffinMuffin.HandEvaluator.Enums;
using BluffinMuffin.Protocol.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes.Attributes;
using BluffinMuffin.Server.DataTypes.Enums;
using BluffinMuffin.Server.Logic.GameModules;

namespace BluffinMuffin.Server.Logic.GameVariants
{
    [GameVariant(GameSubTypeEnum.Pineapple)]
    public class PineappleVariant : AbstractGameVariant
    {
        public override int NbCardsInHand => 3;

        public override EvaluationParams EvaluationParms => new EvaluationParams();

        public override Type InitModuleType => typeof (InitPineappleGameModule);
    }
}
