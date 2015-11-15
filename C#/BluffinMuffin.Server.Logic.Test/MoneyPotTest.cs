using System.Linq;
using BluffinMuffin.HandEvaluator;
using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Server.DataTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BluffinMuffin.Server.Logic.Test
{
    [TestClass]
    public class MoneyPotTest
    {
        [TestMethod]
        public void ContributingTakesMoneyFromPlayerBetAmount()
        {
            //Arrange
            var pot = new MoneyPot();
            var p = new PlayerInfo { MoneyBetAmnt = 142 };

            //Act
            pot.Contribute(p, 42);

            //Assert
            Assert.AreEqual(42, pot.MoneyAmount);
            Assert.AreEqual(100, p.MoneyBetAmnt);
        }
        [TestMethod]
        public void DistributingGivesMoneyToPlayerSafeAmount()
        {
            //Arrange
            var pot = new MoneyPot();
            var p = new PlayerInfo { MoneyBetAmnt = 142, MoneySafeAmnt = 1000 };
            pot.Contribute(p, 42);

            //Act
            var res = pot.Distribute(new[] { PlayerWithRank(p, 1) }).ToArray();

            //Assert
            Assert.AreEqual(0, pot.MoneyAmount);
            Assert.AreEqual(100, p.MoneyBetAmnt);
            Assert.AreEqual(1042, p.MoneySafeAmnt);
            Assert.AreEqual(1, res.Length);
            Assert.AreEqual(p, res.First().Key.CardsHolder.Player);
            Assert.AreEqual(42, res.First().Value);
        }
        [TestMethod]
        public void DistributingGivesMoneyToNobodyIfNoContributingPlayerInTheRankingList()
        {
            //Arrange
            var pot = new MoneyPot();
            var p1 = new PlayerInfo { MoneyBetAmnt = 142, MoneySafeAmnt = 1000 };
            var p2 = new PlayerInfo { MoneyBetAmnt = 0, MoneySafeAmnt = 5000 };
            pot.Contribute(p1, 42);

            //Act
            var res = pot.Distribute(new[] { PlayerWithRank(p2, 1) }).ToArray();

            //Assert
            Assert.AreEqual(0, pot.MoneyAmount);
            Assert.AreEqual(100, p1.MoneyBetAmnt);
            Assert.AreEqual(1000, p1.MoneySafeAmnt);
            Assert.AreEqual(0, p2.MoneyBetAmnt);
            Assert.AreEqual(5000, p2.MoneySafeAmnt);
            Assert.AreEqual(1, res.Length);
            Assert.AreEqual(null, res.First().Key);
            Assert.AreEqual(42, res.First().Value);
        }
        [TestMethod]
        public void DistributingGivesMoneyToLowestInRankingList()
        {
            //Arrange
            var pot = new MoneyPot();
            var p1 = new PlayerInfo { MoneyBetAmnt = 142, MoneySafeAmnt = 1000 };
            var p2 = new PlayerInfo { MoneyBetAmnt = 221, MoneySafeAmnt = 5000 };
            pot.Contribute(p1, 42);
            pot.Contribute(p2, 21);

            //Act
            var res = pot.Distribute(new[] { PlayerWithRank(p1, 2), PlayerWithRank(p2, 1) }).ToArray();

            //Assert
            Assert.AreEqual(0, pot.MoneyAmount);
            Assert.AreEqual(100, p1.MoneyBetAmnt);
            Assert.AreEqual(1000, p1.MoneySafeAmnt);
            Assert.AreEqual(200, p2.MoneyBetAmnt);
            Assert.AreEqual(5063, p2.MoneySafeAmnt);
            Assert.AreEqual(1, res.Length);
            Assert.AreEqual(p2, res.First().Key.CardsHolder.Player);
            Assert.AreEqual(63, res.First().Value);
        }
        [TestMethod]
        public void DistributingSplitsMoneyToLowestInRankingList()
        {
            //Arrange
            var pot = new MoneyPot();
            var p1 = new PlayerInfo { MoneyBetAmnt = 142, MoneySafeAmnt = 1000 };
            var p2 = new PlayerInfo { MoneyBetAmnt = 221, MoneySafeAmnt = 5000 };
            pot.Contribute(p1, 42);
            pot.Contribute(p2, 21);

            //Act
            var res = pot.Distribute(new[] { PlayerWithRank(p1, 1), PlayerWithRank(p2, 1) }).ToArray();

            //Assert
            Assert.AreEqual(0, pot.MoneyAmount);
            Assert.AreEqual(100, p1.MoneyBetAmnt);
            Assert.AreEqual(1031, p1.MoneySafeAmnt);
            Assert.AreEqual(200, p2.MoneyBetAmnt);
            Assert.AreEqual(5031, p2.MoneySafeAmnt);
            Assert.AreEqual(3, res.Length);
            Assert.AreEqual(p1, res.First().Key.CardsHolder.Player);
            Assert.AreEqual(31, res.First().Value); // 63 / 2 = 31.5: 31 is given
            Assert.AreEqual(p2, res.Skip(1).First().Key.CardsHolder.Player);
            Assert.AreEqual(31, res.Skip(1).First().Value); // 63 / 2 = 31.5: 31 is given
            Assert.AreEqual(null, res.Skip(2).First().Key);
            Assert.AreEqual(1, res.Skip(2).First().Value); // 63 - (31*2) = 1: 1 buck for the casino !
        }

        private EvaluatedCardHolder<PlayerCardHolder> PlayerWithRank(PlayerInfo p, int rank)
        {
            return new EvaluatedCardHolder<PlayerCardHolder>(new PlayerCardHolder(p, new string[0]), new EvaluationParams()) { Rank = rank };
        }
    }
}
