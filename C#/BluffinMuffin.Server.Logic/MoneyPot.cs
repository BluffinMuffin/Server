using System.Collections.Generic;
using System.Linq;
using BluffinMuffin.HandEvaluator;
using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Server.DataTypes;

namespace BluffinMuffin.Server.Logic
{
    /// <summary>
    /// Pot of money
    /// </summary>
    public class MoneyPot
    {
        #region Public Properties

        /// <summary>
        /// Total amount of money currently in the pot
        /// </summary>
        public int MoneyAmount { get; private set; }

        #endregion Public Properties

        #region Private Properties

        private List<PlayerInfo> ContributingPlayers { get; } = new List<PlayerInfo>();

        #endregion Private Properties

        #region Public Methods

        /// <summary>
        /// Contribute to pot with money from player
        /// </summary>
        /// <param name="p">Player</param>
        /// <param name="amount">Amount contributed</param>
        public void Contribute(PlayerInfo p, int amount)
        {
            if (amount <= 0 || p.MoneyBetAmnt < amount)
                return;

            p.MoneyBetAmnt -= amount;

            MoneyAmount += amount;

            if (!ContributingPlayers.Contains(p))
                ContributingPlayers.Add(p);
        }

        /// <summary>
        /// Distribute money to deserving players
        /// </summary>
        /// <param name="rankedPlayers">Ranked Players</param>
        /// <returns>Whom won how many</returns>
        public IEnumerable<KeyValuePair<EvaluatedCardHolder<PlayerCardHolder>, int>> Distribute(IEnumerable<EvaluatedCardHolder<PlayerCardHolder>> rankedPlayers)
        {
            //get all contributing players with their ranks
            var contributingPlayersWithRank = rankedPlayers.Where(x => ContributingPlayers.Contains(x.CardsHolder.Player)).ToArray();

            //get the lowest rank of contributing players
            var minRank = contributingPlayersWithRank.Select(x => x.Rank).DefaultIfEmpty(0).Min();

            //get only the players with the lowest rank
            var winningPlayers = contributingPlayersWithRank.Where(x => x.Rank == minRank).ToArray();

            //what equal portion of the pot everyone is getting ?
            var winningAmount = winningPlayers.Any() ? MoneyAmount / winningPlayers.Length : MoneyAmount;

            //Distribute money to everybody
            var winners = new List<KeyValuePair<EvaluatedCardHolder<PlayerCardHolder>, int>>();
            foreach (var wp in winningPlayers)
            {
                winners.Add(new KeyValuePair<EvaluatedCardHolder<PlayerCardHolder>, int>(wp, winningAmount));
                MoneyAmount -= winningAmount;
                wp.CardsHolder.Player.MoneySafeAmnt += winningAmount;
            }

            //If there is still money in the pot after distribution, give it to the casino ! :)
            if (MoneyAmount > 0)
                winners.Add(new KeyValuePair<EvaluatedCardHolder<PlayerCardHolder>, int>(null, MoneyAmount));
            MoneyAmount = 0;

            return winners;

        }

        #endregion Public Methods

    }
}
