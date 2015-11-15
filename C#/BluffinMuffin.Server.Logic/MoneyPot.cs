using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BluffinMuffin.HandEvaluator;
using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Server.DataTypes;

namespace BluffinMuffin.Server.Logic
{
    public class MoneyPot
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
        public IEnumerable<KeyValuePair<EvaluatedCardHolder<PlayerCardHolder>, int>> Distribute(IEnumerable<EvaluatedCardHolder<PlayerCardHolder>> rankedPlayers)
        {
            var playerWithRank = rankedPlayers.ToArray(); 
            var contributingPlayersWithRank = playerWithRank.Where(x => ContributingPlayers.Contains(x.CardsHolder.Player)).ToArray();
            var minRank = contributingPlayersWithRank.Select(x => x.Rank).DefaultIfEmpty(0).Min();
            var winningPlayers = contributingPlayersWithRank.Where(x => x.Rank == minRank).ToArray();

            var winningAmount = winningPlayers.Any() ? MoneyAmount /winningPlayers.Length : MoneyAmount;

            var winners = winningPlayers.Select(x => new KeyValuePair<EvaluatedCardHolder<PlayerCardHolder>, int>(x, winningAmount)).ToList();

            MoneyAmount -= winners.Select(x => x.Value).DefaultIfEmpty(0).Sum();

            winners.ToList().ForEach(x => x.Key.CardsHolder.Player.MoneySafeAmnt += x.Value);

            if(MoneyAmount > 0)
                winners.Add(new KeyValuePair<EvaluatedCardHolder<PlayerCardHolder>, int>(null, MoneyAmount));

            MoneyAmount = 0;

            return winners;

        }
    }
}
