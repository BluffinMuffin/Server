using System;
using BluffinMuffin.HandEvaluator;
using BluffinMuffin.Protocol.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes.Attributes;
using BluffinMuffin.Server.Logic.GameModules;

namespace BluffinMuffin.Server.Logic.GameVariants
{
    [GameVariant(GameSubTypeEnum.IrishPoker)]
    public class IrishPokerVariant : AbstractGameVariant
    {
        public override int NbCardsInHand => 4;

        public override EvaluationParams EvaluationParms => new EvaluationParams();

        public override Type InitModuleType => typeof (InitIrishPokerGameModule);
    }
}
