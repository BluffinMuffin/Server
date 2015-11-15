using System.Collections.Generic;
using System.Linq;
using BluffinMuffin.HandEvaluator;
using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Server.DataTypes;

namespace BluffinMuffin.Server.Logic
{
    public class MoneyPot
    {
        public int MoneyAmount { get; private set; }
        private List<PlayerInfo> ContributingPlayers { get; } = new List<PlayerInfo>();

        public void Contribute(PlayerInfo p, int amount)
        {
            if (amount <= 0 || p.MoneyBetAmnt < amount)
                return;

            p.MoneyBetAmnt -= amount;

            MoneyAmount += amount;

            if (!ContributingPlayers.Contains(p))
                ContributingPlayers.Add(p);
        }

        public IEnumerable<KeyValuePair<EvaluatedCardHolder<PlayerCardHolder>, int>> Distribute(IEnumerable<EvaluatedCardHolder<PlayerCardHolder>> rankedPlayers)
        {
            //get all contributing players with their ranks
            var contributingPlayersWithRank = rankedPlayers.Where(x => ContributingPlayers.Contains(x.CardsHolder.Player)).ToArray();

            //get the lowest rank of contributing players
            var minRank = contributingPlayersWithRank.Select(x => x.Rank).DefaultIfEmpty(0).Min();

            //get only the players with the lowest rank
            var winningPlayers = contributingPlayersWithRank.Where(x => x.Rank == minRank).ToArray();

            //what equal portion of the pot everyone is getting ?
            var winningAmount = winningPlayers.Any() ? MoneyAmount /winningPlayers.Length : MoneyAmount;

            //Distribute money to everybody
            var winners = new List<KeyValuePair<EvaluatedCardHolder<PlayerCardHolder>, int>>();
            foreach (var wp in winningPlayers)
            {
                winners.Add(new KeyValuePair<EvaluatedCardHolder<PlayerCardHolder>, int>(wp, winningAmount));
                MoneyAmount -= winningAmount;
                wp.CardsHolder.Player.MoneySafeAmnt += winningAmount;
            }

            //If there is still money in the pot after distribution, give it to the casino ! :)
            if(MoneyAmount > 0)
                winners.Add(new KeyValuePair<EvaluatedCardHolder<PlayerCardHolder>, int>(null, MoneyAmount));
            MoneyAmount = 0;

            return winners;

        }
    }
}
