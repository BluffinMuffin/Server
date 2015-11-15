using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Protocol.DataTypes.Enums;

namespace BluffinMuffin.Server.Logic.Extensions
{
    public static class TableParamExtensions
    {
        public static bool IsValidBuyIn(this TableParams t, int money) => t.Lobby.MinimumBuyInAmount(t.GameSize) <= money && money <= t.Lobby.MaximumBuyInAmount(t.GameSize);

        public static int BigBlindAmount(this TableParams t) => Math.Max(2, t.GameSize);
        public static int SmallBlindAmount(this TableParams t) => Math.Max(1, t.GameSize / 2);
        public static int AnteAmount(this TableParams t) => Math.Max(1, t.GameSize / 10);
        public static int BetAmount(this TableParams t) => Math.Max(2, t.GameSize);
        public static int HalfBetAmount(this TableParams t) => Math.Max(1, t.GameSize / 2);

    }
}
