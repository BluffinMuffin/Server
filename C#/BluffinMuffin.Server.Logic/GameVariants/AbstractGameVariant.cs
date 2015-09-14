﻿using System;
using System.Reflection;
using BluffinMuffin.HandEvaluator;
using BluffinMuffin.Protocol.DataTypes.Attributes;
using BluffinMuffin.Protocol.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes;
using BluffinMuffin.Server.DataTypes.Attributes;
using BluffinMuffin.Server.DataTypes.EventHandling;
using BluffinMuffin.Server.Logic.GameModules;
using Com.Ericmas001.Util;

namespace BluffinMuffin.Server.Logic.GameVariants
{
    public abstract class AbstractGameVariant
    {
        public virtual int NbCardsInHand => 2;
        public virtual EvaluationParams EvaluationParms => new EvaluationParams();
        public virtual Type InitModuleType => typeof(InitHoldemGameModule);

        private AbstractDealer m_Dealer;
        public AbstractDealer Dealer => m_Dealer ?? (m_Dealer = GenerateDealer());
        
        protected virtual AbstractDealer GenerateDealer()
        {
            return new Shuffled52CardsDealer();
        }

        public GameSubTypeEnum Variant
        {
            get
            {
                var att = GetType().GetCustomAttribute<GameVariantAttribute>();
                if (att != null)
                    return att.Variant;
                return GameSubTypeEnum.TexasHoldem;
            }
        }

        public GameTypeEnum GameType
        {
            get
            {
                var att = EnumFactory<GameSubTypeEnum>.GetAttribute<GameTypeAttribute>(Variant);
                if (att != null)
                    return att.GameType;
                return GameTypeEnum.CommunityCardsPoker;
            }
        }

        public virtual AbstractGameModule GenerateInitModule(PokerGameObserver observer,PokerTable table)
        {
            if (!InitModuleType.IsSubclassOf(typeof (AbstractGameModule)))
                return null;
            var ctor = InitModuleType.GetConstructor(new[] { typeof(PokerGameObserver), typeof(PokerTable) });
            return (AbstractGameModule) ctor?.Invoke(new object[] { observer, table });
        }
    }
}
