﻿using System.Collections.Generic;
using System.Reflection;
using BluffinMuffin.HandEvaluator;
using BluffinMuffin.Protocol.DataTypes.Attributes;
using BluffinMuffin.Protocol.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes;
using BluffinMuffin.Server.DataTypes.Attributes;
using BluffinMuffin.Server.DataTypes.EventHandling;
using Com.Ericmas001.Common;

namespace BluffinMuffin.Server.Logic.GameVariants
{
    public abstract class AbstractGameVariant
    {
        protected virtual int NbCardsInHand => 2;
        public virtual EvaluationParams EvaluationParms => new EvaluationParams();

        public abstract IEnumerable<IGameModule> GetModules(PokerGameObserver o, PokerTable table); 

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
                var att = EnumUtil<GameSubTypeEnum>.GetAttribute<GameTypeAttribute>(Variant);
                if (att != null)
                    return att.GameType;
                return GameTypeEnum.CommunityCardsPoker;
            }
        }
    }
}
