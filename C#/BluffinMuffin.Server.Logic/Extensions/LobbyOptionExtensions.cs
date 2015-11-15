using BluffinMuffin.Protocol.DataTypes.Enums;
using BluffinMuffin.Protocol.DataTypes.Options;

namespace BluffinMuffin.Server.Logic.Extensions
{
    public static class LobbyOptionExtensions
    {

        /// <summary>
        /// The minimum amount needed to enter the game
        /// </summary>
        /// <param name="lo">Current lobby Option</param>
        /// <param name="gameSize">Size of the game (Size of bet, size of big blind)</param>
        /// <returns>The minimum amount needed to enter the game</returns>
        public static int MinimumBuyInAmount(this LobbyOptions lo, int gameSize)
        {
            switch (lo.MinimumBuyInParameter)
            {
                case BuyInParameterEnum.FixedAmount:
                    return lo.MinimumBuyInValue;
                case BuyInParameterEnum.Multiplicator:
                    return lo.MinimumBuyInValue * gameSize;
            }
            return 0;
        }

        /// <summary>
        /// The maximum amount you can put in the game
        /// </summary>
        /// <param name="lo">Current lobby Option</param>
        /// <param name="gameSize">Size of the game (Size of bet, size of big blind)</param>
        /// <returns>The maximum amount you can put in the game</returns>
        public static int MaximumBuyInAmount(this LobbyOptions lo, int gameSize)
        {
            switch (lo.MaximumBuyInParameter)
            {
                case BuyInParameterEnum.FixedAmount:
                    return lo.MaximumBuyInValue;
                case BuyInParameterEnum.Multiplicator:
                    return lo.MaximumBuyInValue * gameSize;
            }
            return int.MaxValue;
        }
    }
}
