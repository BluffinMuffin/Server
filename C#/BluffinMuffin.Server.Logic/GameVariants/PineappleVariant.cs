﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using BluffinMuffin.Protocol.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes;
using BluffinMuffin.Server.DataTypes.Attributes;
using BluffinMuffin.Server.DataTypes.EventHandling;
using BluffinMuffin.Server.Logic.GameModules;

namespace BluffinMuffin.Server.Logic.GameVariants
{
    [GameVariant(GameSubTypeEnum.Pineapple)]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class PineappleVariant : AbstractGameVariant
    {
        protected override int NbCardsInHand => 3;

        public override IEnumerable<IGameModule> GetModules(PokerGameObserver o, PokerTable t)
        {
            //Preflop
            yield return new DealMissingCardsToPlayersModule(o, t, NbCardsInHand);

            //Discard 1 to go back to 2 hole cards
            yield return new DiscardRoundModule(o, t, 1, 1);

            yield return new FirstBettingRoundModule(o, t);
            yield return new CumulPotsModule(o, t);

            //Flop
            yield return new DealCardsToBoardModule(o, t, 3);
            yield return new BettingRoundModule(o, t);
            yield return new CumulPotsModule(o, t);

            //Turn
            yield return new DealCardsToBoardModule(o, t, 1);
            yield return new BettingRoundModule(o, t);
            yield return new CumulPotsModule(o, t);

            //River
            yield return new DealCardsToBoardModule(o, t, 1);
            yield return new BettingRoundModule(o, t);
            yield return new CumulPotsModule(o, t);
        }
    }
}
