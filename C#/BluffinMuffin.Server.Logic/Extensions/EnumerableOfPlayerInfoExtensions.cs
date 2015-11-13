using System.Collections.Generic;
using System.Linq;
using BluffinMuffin.Protocol.DataTypes;

namespace BluffinMuffin.Server.Logic.Extensions
{
    public static class EnumerableOfPlayerInfoExtensions
    {
        public static bool ContainsPlayerWithSameName(this IEnumerable<PlayerInfo> players, PlayerInfo p)
        {
            PlayerInfo[] ps = players.ToArray();
            return ps.Contains(p) || ps.ContainsPlayerNamed(p.Name);
        }
        public static bool ContainsPlayerNamed(this IEnumerable<PlayerInfo> players, string name)
        {
            PlayerInfo[] ps = players.ToArray();
            return ps.Any(x => x.Name.ToLower() == name.ToLower());
        }
    }
}
