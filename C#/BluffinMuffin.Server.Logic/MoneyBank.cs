using System;
using System.Collections.Generic;
using System.Linq;
using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Protocol.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes;
using BluffinMuffin.Server.Logic.Extensions;

namespace BluffinMuffin.Server.Logic
{
    public class MoneyBank
    {
        public int MoneyAmount { get; private set; } = 0;
        private List<PlayerInfo> PlayersWithMoneyAmountInPlay { get; } = new List<PlayerInfo>();

        private Stack<MoneyPot> Pots { get; } = new Stack<MoneyPot>() {}; 

        private Dictionary<PlayerInfo, int> Debts { get; } = new Dictionary<PlayerInfo, int>();

        public MoneyBank()
        {
            Pots.Push(new MoneyPot());
        }

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

        public void DepositMoneyInPlay()
        {
            var allInPlayers = PlayersWithMoneyAmountInPlay.Where(p => p.State == PlayerStateEnum.AllIn).ToArray();
            if (allInPlayers.Any())
            {
                int given = 0;
                foreach (var cap in allInPlayers.Select(p => p.MoneyBetAmnt).OrderBy(a => a))
                {
                    PlayersWithMoneyAmountInPlay.ForEach(p => Pots.Peek().Contribute(p, cap - given));
                    given += cap;
                    Pots.Push(new MoneyPot());
                }
            }
            PlayersWithMoneyAmountInPlay.ForEach(p => Pots.Peek().Contribute(p, p.MoneyBetAmnt));
        }

        private void RepayDebt(PlayerInfo p, int amount)
        {
            if (Debts.ContainsKey(p))
                Debts[p] = Math.Max(0, Debts[p] - amount);
        }

        public void AddDebt(PlayerInfo p, int amount)
        {
            if (!Debts.ContainsKey(p))
                Debts.Add(p, 0);

            Debts[p] += amount;
        }

        public int DebtAmount(PlayerInfo p)
        {
            return Debts.ContainsKey(p) ? Debts[p] : 0;
        }

        public WonPot DistributeCurrentPot(Dictionary<PlayerInfo, int> playersWithRank )
        {
            var winners = Pots.Pop().Distribute(playersWithRank).ToArray();
            var wonPot = new WonPot(Pots.Count, winners.Select(x => x.Value).DefaultIfEmpty(0).Sum(), winners.Where(x => x.Key != null));
            MoneyAmount -= wonPot.TotalPotAmount;
            if (!Pots.Any())
                Pots.Push(new MoneyPot());

            return wonPot;
        }

        public int TotalDebtAmount => Debts.Values.Sum();

        public IEnumerable<int> PotAmountsPadded(int nbTotal) => Pots.Select(pot => pot.MoneyAmount).Union(Enumerable.Repeat(0, nbTotal - Pots.Count));
    }
}