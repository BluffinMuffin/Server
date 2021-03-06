﻿using System.Diagnostics.CodeAnalysis;
using BluffinMuffin.HandEvaluator;
using BluffinMuffin.HandEvaluator.Selectors;
using BluffinMuffin.Protocol.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes.Attributes;

namespace BluffinMuffin.Server.Logic.GameVariants
{
    [GameVariant(GameSubTypeEnum.OmahaHoldem)]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class OmahaHoldemVariant : AbstractHoldemGameVariant
    {
        protected override int NbCardsInHand => 4;

        public override EvaluationParams EvaluationParms => new EvaluationParams
        {
            Selector = new Use2Player3CommunitySelector()
        };
    }
}
