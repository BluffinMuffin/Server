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
            return ps.Contains(p) || ps.Any(x => x.Name.ToLower() == p.Name.ToLower());
        }
    }
}
