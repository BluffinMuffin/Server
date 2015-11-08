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
        public static bool IsValidBuyIn(this TableParams t, int money) => t.MinimumBuyInAmount <= money && money <= t.MaximumBuyInAmount;
    }
}
