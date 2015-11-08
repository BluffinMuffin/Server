using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Protocol.DataTypes.Enums;

namespace BluffinMuffin.Server.Logic.Extensions
{
    public static class PlayerInfoExtensions
    {
        public static bool IsPlayingOrAllIn(this PlayerInfo p) => p.State == PlayerStateEnum.Playing || p.State == PlayerStateEnum.AllIn;
        
        /// <summary>
        /// Check if the player has enough money to bet some amount
        /// </summary>
        public static bool CanBet(this PlayerInfo p, int amnt) => amnt <= p.MoneySafeAmnt;

        /// <summary>
        /// Tries to put some money on the table
        /// </summary>
        /// <returns>True if the money has been successfully played</returns>
        public static bool TryBet(this PlayerInfo p, int amnt)
        {
            if (!p.CanBet(amnt))
            {
                return false;
            }

            p.MoneySafeAmnt -= amnt;
            p.MoneyBetAmnt += amnt;
            return true;
        }

        public static bool CanPlay(this PlayerInfo p) => p.NoSeat >= 0 && p.MoneySafeAmnt > 0;
    }
}
