using System;
using BluffinMuffin.HandEvaluator;
using BluffinMuffin.Protocol.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes;
using BluffinMuffin.Server.DataTypes.Attributes;
using BluffinMuffin.Server.Logic.GameModules;

namespace BluffinMuffin.Server.Logic.GameVariants
{
    [FavoriteGameVariant]
    [GameVariant(GameSubTypeEnum.TexasHoldem)]
    public class TexasHoldemVariant : AbstractGameVariant
    {
        public override int NbCardsInHand => 2;

        public override EvaluationParams EvaluationParms => new EvaluationParams();

        public override Type InitModuleType => typeof(InitHoldemGameModule);
        protected override AbstractDealer GenerateDealer()
        {
            return new Shuffled52CardsDealer();
        }
    }
}
