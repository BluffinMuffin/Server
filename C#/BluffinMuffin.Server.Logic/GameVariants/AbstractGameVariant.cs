using System;
using System.Reflection;
using BluffinMuffin.HandEvaluator;
using BluffinMuffin.Protocol.DataTypes.Attributes;
using BluffinMuffin.Protocol.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes.Attributes;
using BluffinMuffin.Server.DataTypes.EventHandling;
using BluffinMuffin.Server.Logic.GameModules;
using Com.Ericmas001.Util;

namespace BluffinMuffin.Server.Logic.GameVariants
{
    public abstract class AbstractGameVariant
    {
        public abstract int NbCardsInHand { get; }
        public abstract EvaluationParams EvaluationParms { get; }

        public abstract Type InitModuleType { get; }

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
