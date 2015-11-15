using System;
using System.Collections.Generic;
using System.Linq;
using BluffinMuffin.HandEvaluator;
using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Protocol.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes;
using BluffinMuffin.Server.Logic.Extensions;

namespace BluffinMuffin.Server.Logic
{
    /// <summary>
    /// Contain information about money not owned by any player.
    /// </summary>
    public class MoneyBank
    {
        #region Public Properties

        /// <summary>
        /// Amount of money currently owned by the bank, not the players.
        /// </summary>
        public int MoneyAmount { get; private set; }

        /// <summary>
        /// Total amount of money owed to the bank by the players
        /// </summary>
        public int TotalDebtAmount => Debts.Values.Sum();

        #endregion Public Properties
        
        #region Private Properties

        private List<PlayerInfo> PlayersWithMoneyAmountInPlay { get; } = new List<PlayerInfo>();
        private Stack<MoneyPot> Pots { get; } = new Stack<MoneyPot>();
        private Dictionary<PlayerInfo, int> Debts { get; } = new Dictionary<PlayerInfo, int>();

        #endregion Private Properties

        public MoneyBank()
        {
            Pots.Push(new MoneyPot());
        }

        #region Public Methods

        /// <summary>
        /// If successful, takes money from player SafeMoneyAmount to BetMoneyAmount. It will also repay debts if any.
        /// </summary>
        /// <param name="p">Player</param>
        /// <param name="amount">Amount of money collecting</param>
        /// <returns></returns>
        public bool CollectMoneyFromPlayer(PlayerInfo p, int amount)
        {
            bool hadEnoughMoney = p.TryBet(amount);

            if (hadEnoughMoney)
            {
                MoneyAmount += amount;
                RepayDebt(p, amount);
                if (!PlayersWithMoneyAmountInPlay.Contains(p))
                    PlayersWithMoneyAmountInPlay.Add(p);
            }

            return hadEnoughMoney;
        }

        /// <summary>
        /// Takes all the money in front of every player and deposit it into money pots.
        /// </summary>
        public void DepositMoneyInPlay()
        {
            var allInPlayers = PlayersWithMoneyAmountInPlay.Where(p => p.State == PlayerStateEnum.AllIn).ToArray();
            if (allInPlayers.Any())
            {
                int given = 0;
                foreach (var cap in allInPlayers.Select(p => p.MoneyBetAmnt).OrderBy(a => a))
                {
                    PlayersWithMoneyAmountInPlay.ForEach(p => Pots.Peek().Contribute(p, cap - given));
                    given += (cap - given);
                    Pots.Push(new MoneyPot());
                }
            }
            PlayersWithMoneyAmountInPlay.ForEach(p => Pots.Peek().Contribute(p, p.MoneyBetAmnt));
            PlayersWithMoneyAmountInPlay.Clear();
        }

        /// <summary>
        /// Add debt to a player. This is used for mandatory bets (Ex: Blinds, Ante)
        /// </summary>
        /// <param name="p">Player</param>
        /// <param name="amount">Debt Amount</param>
        public void AddDebt(PlayerInfo p, int amount)
        {
            if (!Debts.ContainsKey(p))
                Debts.Add(p, 0);

            Debts[p] += amount;
        }

        /// <summary>
        /// Get debt amount currently owed by a player
        /// </summary>
        /// <param name="p">Player</param>
        /// <returns>Debt Amount</returns>
        public int DebtAmount(PlayerInfo p)
        {
            return Debts.ContainsKey(p) ? Debts[p] : 0;
        }

        /// <summary>
        /// Takes evey money pots and distribute money to the deserving players
        /// </summary>
        /// <param name="rankedPlayers">Players ranked</param>
        /// <returns>All the won pots</returns>
        public IEnumerable<WonPot> DistributeMoney(IEnumerable<EvaluatedCardHolder<PlayerCardHolder>> rankedPlayers)
        {

            IList<WonPot> pots = new List<WonPot>();
            var playersWithRank = rankedPlayers.ToArray();

            //Just to be sure there is no money left in play
            DepositMoneyInPlay();

            //Distribute all money pots
            while (Pots.Any())
            {
                var winners = Pots.Pop().Distribute(playersWithRank).ToArray();
                var wonPot = new WonPot(Pots.Count, winners.Select(x => x.Value).DefaultIfEmpty(0).Sum(), winners.Where(x => x.Key != null));

                MoneyAmount -= wonPot.TotalPotAmount;

                pots.Add(wonPot);
            }

            //Create a new empty moneypot ready for the next game
            Pots.Push(new MoneyPot());

            //Return pots so they are in good order
            return pots.OrderBy(x => x.PotId);
        }

        /// <summary>
        /// Gets all the money pots amount. If the number of money pot is under what is expected, pots are added with an amount of 0
        /// </summary>
        /// <param name="nbTotal">Nb of pots expected</param>
        /// <returns>All the money pots amount</returns>
        public IEnumerable<int> PotAmountsPadded(int nbTotal)
        {
           return Pots.Select(pot => pot.MoneyAmount).Reverse().Concat(Enumerable.Repeat(0, nbTotal - Pots.Count)); 
        }

        #endregion Public Methods

        #region Private Methods

        private void RepayDebt(PlayerInfo p, int amount)
        {
            if (Debts.ContainsKey(p))
                Debts[p] = Math.Max(0, Debts[p] - amount);
        }

        #endregion Private Methods
    }
}