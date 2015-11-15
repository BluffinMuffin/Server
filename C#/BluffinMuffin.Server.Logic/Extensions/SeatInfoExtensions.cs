using System.Linq;
using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Protocol.DataTypes.Enums;

namespace BluffinMuffin.Server.Logic.Extensions
{
    public static class SeatInfoExtensions
    {

        /// <summary>
        /// Checks if the seats contains a player that is in the playing state
        /// </summary>
        /// <param name="si">Seat</param>
        /// <returns>True if the seats contains a player that is in the playing state</returns>
        public static bool HasPlayerPlaying(this SeatInfo si)
        {
            return !si.IsEmpty && si.Player.State == PlayerStateEnum.Playing;
        }

        /// <summary>
        /// Checks if the seats contains a player that is in the all-in state
        /// </summary>
        /// <param name="si">Seat</param>
        /// <returns>True if the seats contains a player that is in the all=in state</returns>
        public static bool HasPlayerAllIn(this SeatInfo si)
        {
            return !si.IsEmpty && si.Player.State == PlayerStateEnum.AllIn;
        }

        /// <summary>
        /// Checks if the seats contains a player that is in the playing or all-in state
        /// </summary>
        /// <param name="si">Seat</param>
        /// <returns>True if the seats contains a player that is in the playing or all-in state</returns>
        public static bool HasPlayerPlayingOrAllIn(this SeatInfo si)
        {
            return !si.IsEmpty && si.Player.IsPlayingOrAllIn();
        }

        /// <summary>
        /// Checks if the seats has a specific attribute
        /// </summary>
        /// <param name="si">Seat</param>
        /// <param name="att">Specific attribute</param>
        /// <returns>True if the seats has a specific attribute</returns>
        public static bool HasAttribute(this SeatInfo si, SeatAttributeEnum att)
        {
            return si.SeatAttributes.Contains(att);
        }

        /// <summary>
        /// Adds a specific attribute to the seat
        /// </summary>
        /// <param name="si">Seat</param>
        /// <param name="att">Specific attribute</param>
        public static void AddAttribute(this SeatInfo si, SeatAttributeEnum att)
        {
            si.SeatAttributes = si.SeatAttributes.Union(new[] { att }).ToArray();
        }

        /// <summary>
        /// Removes a specific attribute from the seat
        /// </summary>
        /// <param name="si">Seat</param>
        /// <param name="att">Specific attribute</param>
        public static void RemoveAttribute(this SeatInfo si, SeatAttributeEnum att)
        {
            si.SeatAttributes = si.SeatAttributes.Except(new[] { att }).ToArray();
        }
    }
}
