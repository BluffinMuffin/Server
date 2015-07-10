using System.Collections.Generic;
using System.Linq;
using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Protocol.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes.Enums;
using Com.Ericmas001.Util;

namespace BluffinMuffin.Server.Logic
{
    public static class RuleFactory
    {
        public static RuleInfo[] SupportedRules
        {
            get
            {
                return EnumFactory<GameVariantEnum>.AllValues.Select(x => new RuleInfo()
                {
                    Name = EnumFactory<GameVariantEnum>.ToString(x),
                    GameType = GameTypeEnum.Holdem,
                    MinPlayers = 2,
                    MaxPlayers = 10,
                    AvailableLimits = new List<LimitTypeEnum>() {LimitTypeEnum.NoLimit /*,LimitTypeEnum.FixedLimit,LimitTypeEnum.PotLimit*/},
                    DefaultLimit = LimitTypeEnum.NoLimit,
                    AvailableBlinds = new List<BlindTypeEnum>() {BlindTypeEnum.Blinds, BlindTypeEnum.Antes, BlindTypeEnum.None},
                    DefaultBlind = BlindTypeEnum.Blinds,
                    CanConfigWaitingTime = true,
                    AvailableLobbys = new List<LobbyTypeEnum>() {LobbyTypeEnum.QuickMode, LobbyTypeEnum.RegisteredMode},
                }).ToArray();
            }
        }
    }
}
