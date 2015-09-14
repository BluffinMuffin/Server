﻿using System;
using BluffinMuffin.HandEvaluator;
using BluffinMuffin.Protocol.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes;
using BluffinMuffin.Server.DataTypes.Attributes;
using BluffinMuffin.Server.Logic.GameModules;

namespace BluffinMuffin.Server.Logic.GameVariants
{
    [GameVariant(GameSubTypeEnum.LazyPineapple)]
    public class LazyPineappleVariant : AbstractGameVariant
    {
        public override int NbCardsInHand => 3;

        public override Type InitModuleType => typeof (InitLazyPineappleGameModule);
    }
}