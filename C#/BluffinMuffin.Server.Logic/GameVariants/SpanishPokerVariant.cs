﻿using System.Diagnostics.CodeAnalysis;
using System.Linq;
using BluffinMuffin.HandEvaluator;
using BluffinMuffin.HandEvaluator.HandRankers;
using BluffinMuffin.HandEvaluator.Selectors;
using BluffinMuffin.Protocol.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes;
using BluffinMuffin.Server.DataTypes.Attributes;

namespace BluffinMuffin.Server.Logic.GameVariants
{
    [GameVariant(GameSubTypeEnum.SpanishPoker)]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class SpanishPokerVariant : AbstractLongFlopHoldemGameVariant
    {
        public override EvaluationParams EvaluationParms => new EvaluationParams
        {
            HandRanker = new FlushBeatsFullHouseHandRanker(),
            Selector = new Use2Player3CommunitySelector(),
            UsedCardValues = Dealer.UsedValues.ToArray()
        };

        protected override AbstractDealer GenerateDealer()
        {
            return new Shuffled28HighCardsDealer();
        }
    }
}
