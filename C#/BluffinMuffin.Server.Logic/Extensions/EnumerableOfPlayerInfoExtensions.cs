using System.Collections.Generic;
using System.Linq;
using BluffinMuffin.Protocol.DataTypes;

namespace BluffinMuffin.Server.Logic.Extensions
{
    public static class EnumerableOfPlayerInfoExtensions
    {
        /// <summary>
        /// Checks if the collection contains the player or someone with the same name case unsensitive
        /// </summary>
        /// <param name="players">Collection of players</param>
        /// <param name="p">Compared player</param>
        /// <returns>True if contains the player or someone with the same name, case unsensitive</returns>
        public static bool ContainsPlayerWithSameName(this IEnumerable<PlayerInfo> players, PlayerInfo p)
        {
            PlayerInfo[] ps = players.ToArray();
            return ps.Contains(p) || ps.ContainsPlayerNamed(p.Name);
        }

        /// <summary>
        /// Checks if the collection contains someone named accordingly, case unsensitive
        /// </summary>
        /// <param name="players">Collection of players</param>
        /// <param name="name">Compared name</param>
        /// <returns>True if somemone is named accordingly, case unsensitive</returns>
        public static bool ContainsPlayerNamed(this IEnumerable<PlayerInfo> players, string name)
        {
            PlayerInfo[] ps = players.ToArray();
            return ps.Any(x => x.Name.ToLower() == name.ToLower());
        }
    }
}
