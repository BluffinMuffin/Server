using System;
using System.Linq;
using BluffinMuffin.HandEvaluator;
using BluffinMuffin.HandEvaluator.HandRankers;
using BluffinMuffin.HandEvaluator.Selectors;
using BluffinMuffin.Protocol.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes;
using BluffinMuffin.Server.DataTypes.Attributes;
using BluffinMuffin.Server.Logic.GameModules;

namespace BluffinMuffin.Server.Logic.GameVariants
{
    [GameVariant(GameSubTypeEnum.SpanishPoker)]
    public class SpanishPokerVariant : AbstractGameVariant
    {
        public override EvaluationParams EvaluationParms => new EvaluationParams {HandRanker = new FlushBeatsFullHouseHandRanker(), Selector = new Use2Player3CommunitySelector(), UsedCardValues = Dealer.UsedValues.ToArray()};

        public override Type InitModuleType => typeof(InitLongFlopHoldemGameModule);

        protected override AbstractDealer GenerateDealer()
        {
            return new Shuffled28HighCardsDealer();
        }
    }
}
