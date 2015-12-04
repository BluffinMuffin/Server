using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Protocol.DataTypes.Enums;

namespace BluffinMuffin.Server.Logic.Extensions
{
    public static class PlayerInfoExtensions
    {

        /// <summary>
        /// Checks if the player is playing or all-in
        /// </summary>
        /// <param name="p">Player</param>
        /// <returns>True if the player is playing or all-in</returns>
        public static bool IsPlayingOrAllIn(this PlayerInfo p)
        {
            return p.IsPlaying() || p.State == PlayerStateEnum.AllIn;
        }

        /// <summary>
        /// Checks if the player is playing
        /// </summary>
        /// <param name="p">Player</param>
        /// <returns>True if the player is playing</returns>
        public static bool IsPlaying(this PlayerInfo p)
        {
            return p.State == PlayerStateEnum.Playing;
        }

        public static void ChangeState(this PlayerInfo p, PlayerStateEnum state)
        {
            p.State = state;
        }


        /// <summary>
        /// Check if the player has enough money to bet some amount
        /// </summary>
        /// <param name="p">Player</param>
        /// <param name="amnt">Amount of money</param>
        /// <returns>True if the player has enough money to bet some amount</returns>
        public static bool CanBet(this PlayerInfo p, int amnt)
        {
            return amnt <= p.MoneySafeAmnt;
        }


        /// <summary>
        /// Tries to put some money on the table
        /// </summary>
        /// <param name="p">Player</param>
        /// <param name="amnt">Amount of money</param>
        /// <returns>True if the money has been successfully played</returns>
        public static bool TryBet(this PlayerInfo p, int amnt)
        {
            if (!p.CanBet(amnt))
                return false;

            p.MoneySafeAmnt -= amnt;
            p.MoneyBetAmnt += amnt;
            return true;
        }

        /// <summary>
        /// Checks if the player is ready to play
        /// </summary>
        /// <param name="p">Player</param>
        /// <returns>True if the player is ready to play</returns>
        public static bool IsReadyToPlay(this PlayerInfo p)
        {
            return p.NoSeat >= 0 && p.MoneySafeAmnt > 0;
        }
    }
}
