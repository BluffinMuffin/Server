using System;
using BluffinMuffin.HandEvaluator;
using BluffinMuffin.Protocol.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes;
using BluffinMuffin.Server.DataTypes.Attributes;
using BluffinMuffin.Server.Logic.GameModules;

namespace BluffinMuffin.Server.Logic.GameVariants
{
    [GameVariant(GameSubTypeEnum.ThreeCardsHoldem)]
    public class ThreeCardsHoldemVariant : AbstractGameVariant
    {
        public override int NbCardsInHand => 3;

        public override EvaluationParams EvaluationParms => new EvaluationParams();

        public override Type InitModuleType => typeof(InitHoldemGameModule);
        protected override AbstractDealer GenerateDealer()
        {
            return new Shuffled52CardsDealer();
        }
    }
}
