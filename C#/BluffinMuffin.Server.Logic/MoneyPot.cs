using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BluffinMuffin.Protocol.DataTypes;

namespace BluffinMuffin.Server.Logic
{
    class MoneyPot
    {
        public int MoneyAmount { get; private set; } = 0;
        private List<PlayerInfo> ContributingPlayers { get; } = new List<PlayerInfo>();

        public void Contribute(PlayerInfo p, int amount)
        {
            if (amount > 0 && p.MoneyBetAmnt >= amount)
            {
                p.MoneyBetAmnt -= amount;

                MoneyAmount += amount;

                if (!ContributingPlayers.Contains(p))
                    ContributingPlayers.Add(p);
            }
        }
        public IEnumerable<KeyValuePair<PlayerInfo, int>> Distribute(Dictionary<PlayerInfo, int> playersWithRank)
        {
            var contributingPlayersWithRank = playersWithRank.Where(x => ContributingPlayers.Contains(x.Key)).ToDictionary(x => x.Key, x => x.Value);
            var minRank = contributingPlayersWithRank.Values.DefaultIfEmpty(0).Min();
            var winningPlayers = contributingPlayersWithRank.Where(x => x.Value == minRank).Select(x => x.Key).ToArray();

            var winningAmount = winningPlayers.Any() ? MoneyAmount /winningPlayers.Length : MoneyAmount;

            var winners = winningPlayers.Select(x => new KeyValuePair<PlayerInfo,int>(x, winningAmount)).ToList();

            MoneyAmount -= winners.Select(x => x.Value).DefaultIfEmpty(0).Sum();

            winners.ToList().ForEach(x => x.Key.MoneySafeAmnt += x.Value);

            if(MoneyAmount > 0)
                winners.Add(new KeyValuePair<PlayerInfo, int>(null, MoneyAmount));

            return winners;

        }
    }
}
