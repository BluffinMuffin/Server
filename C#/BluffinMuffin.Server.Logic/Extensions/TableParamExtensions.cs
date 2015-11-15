using System;
using BluffinMuffin.Protocol.DataTypes;

namespace BluffinMuffin.Server.Logic.Extensions
{
    public static class TableParamExtensions
    {
        /// <summary>
        /// Checks if a certain amount of money is valid according to the table parameters
        /// </summary>
        /// <param name="t">Table parameters</param>
        /// <param name="money">Amount of money</param>
        /// <returns>True if the amount of money is valid</returns>
        public static bool IsValidBuyIn(this TableParams t, int money)
        {
            return t.Lobby.MinimumBuyInAmount(t.GameSize) <= money && money <= t.Lobby.MaximumBuyInAmount(t.GameSize);
        }

        /// <summary>
        /// Calculates the big blind amount from the Gamesize
        /// </summary>
        /// <param name="t">Table parameters</param>
        /// <returns>The Gamesize or 2, if lower</returns>
        public static int BigBlindAmount(this TableParams t)
        {
            return Math.Max(2, t.GameSize);
        }

        /// <summary>
        /// Calculates the big blind amount from the Gamesize
        /// </summary>
        /// <param name="t">Table parameters</param>
        /// <returns>The Gamesize / 2 or 1, if lower</returns>
        public static int SmallBlindAmount(this TableParams t)
        {
            return Math.Max(1, t.GameSize/2);
        }

        /// <summary>
        /// Calculates the big blind amount from the Gamesize
        /// </summary>
        /// <param name="t">Table parameters</param>
        /// <returns>The Gamesize / 10 or 1, if lower</returns>
        public static int AnteAmount(this TableParams t)
        {
            return Math.Max(1, t.GameSize/10);
        }

        /// <summary>
        /// Calculates the big blind amount from the Gamesize
        /// </summary>
        /// <param name="t">Table parameters</param>
        /// <returns>The Gamesize or 2, if lower</returns>
        public static int BetAmount(this TableParams t)
        {
            return Math.Max(2, t.GameSize);
        }

        /// <summary>
        /// Calculates the big blind amount from the Gamesize
        /// </summary>
        /// <param name="t">Table parameters</param>
        /// <returns>The Gamesize / 2 or 1, if lower</returns>
        public static int HalfBetAmount(this TableParams t)
        {
            return Math.Max(1, t.GameSize/2);
        }

    }
}
