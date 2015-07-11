using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BluffinMuffin.HandEvaluator.Enums;
using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Protocol.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes.Attributes;
using BluffinMuffin.Server.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes.EventHandling;
using BluffinMuffin.Server.Logic.GameModules;
using Com.Ericmas001.Util;

namespace BluffinMuffin.Server.Logic.GameVariants
{
    public abstract class AbstractGameVariant
    {
        public abstract int NbCardsInHand { get; }
        public abstract CardSelectionEnum CardSelectionType { get; }

        public abstract Type InitModuleType { get; }

        public virtual RuleInfo Info
        {
            get
            {
                return new RuleInfo()
                {
                    Name = EnumFactory<GameVariantEnum>.ToString(Variant),
                    GameType = GameTypeEnum.Holdem,
                    MinPlayers = 2,
                    MaxPlayers = 10,
                    AvailableLimits = new List<LimitTypeEnum>() { LimitTypeEnum.NoLimit /*,LimitTypeEnum.FixedLimit,LimitTypeEnum.PotLimit*/},
                    DefaultLimit = LimitTypeEnum.NoLimit,
                    AvailableBlinds = new List<BlindTypeEnum>() { BlindTypeEnum.Blinds, BlindTypeEnum.Antes, BlindTypeEnum.None },
                    DefaultBlind = BlindTypeEnum.Blinds,
                    CanConfigWaitingTime = true,
                    AvailableLobbys = new List<LobbyTypeEnum>() { LobbyTypeEnum.QuickMode, LobbyTypeEnum.RegisteredMode },
                };
            }
        }

        public virtual GameVariantEnum Variant
        {
            get
            {
                var att = GetType().GetCustomAttribute<GameVariantAttribute>();
                if (att != null)
                    return att.Variant;
                return GameVariantEnum.TexasHoldem;
            }
        }

        public virtual bool IsFavorite
        {
            get
            {
                var att = GetType().GetCustomAttribute<FavoriteGameVariantAttribute>();
                if (att != null)
                    return true;
                return false;
            }
        }

        public virtual AbstractGameModule GenerateInitModule(PokerGameObserver observer,PokerTable table)
        {
            if (!InitModuleType.IsSubclassOf(typeof (AbstractGameModule)))
                return null;
            var ctor = InitModuleType.GetConstructor(new[] { typeof(PokerGameObserver), typeof(PokerTable) });
            if (ctor != null)
                return (AbstractGameModule)ctor.Invoke(new object[] { observer, table });
            return null;
        }
    }
}
