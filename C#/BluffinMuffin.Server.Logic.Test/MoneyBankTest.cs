using System.Collections.Generic;
using System.Linq;
using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Protocol.DataTypes.Enums;
using BluffinMuffin.Server.Logic.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BluffinMuffin.Server.Logic.Test
{
    [TestClass]
    public class MoneyBankTest
    {
        [TestMethod]
        public void AtBeginningNoMoneyAndNoDebts()
        {
            //Arrange
            var bank = new MoneyBank();

            //Act

            //Assert
            Assert.AreEqual(0, bank.MoneyAmount);
            Assert.AreEqual(0, bank.TotalDebtAmount);
        }
        [TestMethod]
        public void CantCollectIfNotEnoughMoney()
        {
            //Arrange
            var bank = new MoneyBank();
            var p = new PlayerInfo { MoneySafeAmnt = 1042 };

            //Act
            var res = bank.CollectMoneyFromPlayer(p, 5000);

            //Assert
            Assert.IsFalse(res);
        }
        [TestMethod]
        public void CanCollectIfEnoughMoney()
        {
            //Arrange
            var bank = new MoneyBank();
            var p = new PlayerInfo { MoneySafeAmnt = 1042 };

            //Act
            var res = bank.CollectMoneyFromPlayer(p, 42);

            //Assert
            Assert.IsTrue(res);
            Assert.AreEqual(42, bank.MoneyAmount);
            Assert.AreEqual(1000, p.MoneySafeAmnt);
            Assert.AreEqual(42, p.MoneyBetAmnt);
        }
        [TestMethod]
        public void ADebtIsRememberedCorrectly()
        {
            //Arrange
            var bank = new MoneyBank();
            var p = new PlayerInfo { MoneySafeAmnt = 1042 };

            //Act
            bank.AddDebt(p, 42);

            //Assert
            Assert.AreEqual(0, bank.MoneyAmount);
            Assert.AreEqual(42, bank.TotalDebtAmount);
            Assert.AreEqual(42, bank.DebtAmount(p));
            Assert.AreEqual(1042, p.MoneySafeAmnt);
            Assert.AreEqual(0, p.MoneyBetAmnt);
        }
        [TestMethod]
        public void ADebtIsLessIfSomeAmountIsPaid()
        {
            //Arrange
            var bank = new MoneyBank();
            var p = new PlayerInfo { MoneySafeAmnt = 1042 };
            bank.AddDebt(p, 142);

            //Act
            var res = bank.CollectMoneyFromPlayer(p, 42);

            //Assert
            Assert.IsTrue(res);
            Assert.AreEqual(42, bank.MoneyAmount);
            Assert.AreEqual(100, bank.TotalDebtAmount);
            Assert.AreEqual(100, bank.DebtAmount(p));
            Assert.AreEqual(1000, p.MoneySafeAmnt);
            Assert.AreEqual(42, p.MoneyBetAmnt);
        }
        [TestMethod]
        public void DistributingGivesMoneyToLowestInRankingList()
        {
            //Arrange
            var bank = new MoneyBank();
            var p1 = new PlayerInfo { MoneySafeAmnt = 1042 };
            var p2 = new PlayerInfo { MoneySafeAmnt = 5021 };
            bank.CollectMoneyFromPlayer(p1, 42);
            bank.CollectMoneyFromPlayer(p2, 21);

            //Act
            var res = bank.DistributeMoney(new Dictionary<PlayerInfo, int> { { p1, 2 }, { p2, 1 } }).ToArray();

            //Assert
            Assert.AreEqual(0, bank.MoneyAmount);
            Assert.AreEqual(0, p1.MoneyBetAmnt);
            Assert.AreEqual(1000, p1.MoneySafeAmnt);
            Assert.AreEqual(0, p2.MoneyBetAmnt);
            Assert.AreEqual(5063, p2.MoneySafeAmnt);
            Assert.AreEqual(1, res.Length);
            Assert.AreEqual(0, res.First().PotId);
            Assert.AreEqual(63, res.First().TotalPotAmount);
            Assert.AreEqual(1, res.First().Winners.Count());
            Assert.AreEqual(p2, res.First().Winners.First().Key);
            Assert.AreEqual(63, res.First().Winners.First().Value);
        }
        [TestMethod]
        public void DistributingSplitsMoneyToLowestInRankingList()
        {
            //Arrange
            var bank = new MoneyBank();
            var p1 = new PlayerInfo { MoneySafeAmnt = 1042 };
            var p2 = new PlayerInfo { MoneySafeAmnt = 5021 };
            bank.CollectMoneyFromPlayer(p1, 42);
            bank.CollectMoneyFromPlayer(p2, 21);

            //Act
            var res = bank.DistributeMoney(new Dictionary<PlayerInfo, int> { { p1, 1 }, { p2, 1 } }).ToArray();

            //Assert
            Assert.AreEqual(0, bank.MoneyAmount);
            Assert.AreEqual(0, p1.MoneyBetAmnt);
            Assert.AreEqual(1031, p1.MoneySafeAmnt);
            Assert.AreEqual(0, p2.MoneyBetAmnt);
            Assert.AreEqual(5031, p2.MoneySafeAmnt);
            Assert.AreEqual(1, res.Length);
            Assert.AreEqual(0, res.First().PotId);
            Assert.AreEqual(63, res.First().TotalPotAmount);
            Assert.AreEqual(2, res.First().Winners.Count());
            Assert.AreEqual(p1, res.First().Winners.First().Key);
            Assert.AreEqual(31, res.First().Winners.First().Value); // 63 / 2 = 31.5: 31 is given
            Assert.AreEqual(p2, res.First().Winners.Skip(1).First().Key);
            Assert.AreEqual(31, res.First().Winners.Skip(1).First().Value); // 63 / 2 = 31.5: 31 is given
        }
        [TestMethod]
        public void DistributingSplitsBetweenWhatAllInPlayersCanGet()
        {
            //Arrange
            var bank = new MoneyBank();
            var p1 = new PlayerInfo { Name = "P1", MoneySafeAmnt = 21 };
            var p2 = new PlayerInfo { Name = "P2", MoneySafeAmnt = 1163 };
            var p3 = new PlayerInfo { Name = "P3", MoneySafeAmnt = 142 };
            var p4 = new PlayerInfo { Name = "P4", MoneySafeAmnt = 5163 };
            bank.CollectMoneyFromPlayer(p1, 21);
            bank.CollectMoneyFromPlayer(p2, 1163);
            bank.CollectMoneyFromPlayer(p3, 142);
            bank.CollectMoneyFromPlayer(p4, 2163);
            p1.State = PlayerStateEnum.AllIn;
            p2.State = PlayerStateEnum.AllIn;
            p3.State = PlayerStateEnum.AllIn;
            p4.State = PlayerStateEnum.Playing;

            //Act
            var res = bank.DistributeMoney(new Dictionary<PlayerInfo, int> { { p1, 1 }, { p2, 3 }, { p3, 2 }, { p4, 4 } }).ToArray();

            //Assert
            Assert.AreEqual(0, bank.MoneyAmount);
            Assert.AreEqual(0, p1.MoneyBetAmnt);
            Assert.AreEqual(0, p2.MoneyBetAmnt);
            Assert.AreEqual(0, p3.MoneyBetAmnt);
            Assert.AreEqual(0, p4.MoneyBetAmnt);
            Assert.AreEqual(84, p1.MoneySafeAmnt); //21 * 4
            Assert.AreEqual(2042, p2.MoneySafeAmnt); //(1163 - 142) * 2
            Assert.AreEqual(363, p3.MoneySafeAmnt); //(142 - 21) * 3
            Assert.AreEqual(4000, p4.MoneySafeAmnt); //3000 + (2163 - 1163)
            Assert.AreEqual(4, res.Length);
            Assert.AreEqual(0, res.First().PotId);
            Assert.AreEqual(84, res.First().TotalPotAmount);
            Assert.AreEqual(1, res.First().Winners.Count());
            Assert.AreEqual(p1, res.First().Winners.First().Key);
            Assert.AreEqual(84, res.First().Winners.First().Value);
            Assert.AreEqual(1, res.Skip(1).First().PotId);
            Assert.AreEqual(363, res.Skip(1).First().TotalPotAmount);
            Assert.AreEqual(1, res.Skip(1).First().Winners.Count());
            Assert.AreEqual(p3, res.Skip(1).First().Winners.First().Key);
            Assert.AreEqual(363, res.Skip(1).First().Winners.First().Value);
            Assert.AreEqual(2, res.Skip(2).First().PotId);
            Assert.AreEqual(2042, res.Skip(2).First().TotalPotAmount);
            Assert.AreEqual(1, res.Skip(2).First().Winners.Count());
            Assert.AreEqual(p2, res.Skip(2).First().Winners.First().Key);
            Assert.AreEqual(2042, res.Skip(2).First().Winners.First().Value);
            Assert.AreEqual(3, res.Skip(3).First().PotId);
            Assert.AreEqual(1000, res.Skip(3).First().TotalPotAmount);
            Assert.AreEqual(1, res.Skip(3).First().Winners.Count());
            Assert.AreEqual(p4, res.Skip(3).First().Winners.First().Key);
            Assert.AreEqual(1000, res.Skip(3).First().Winners.First().Value);
        }
        [TestMethod]
        public void PotsAmountPadded()
        {
            //Arrange
            var bank = new MoneyBank();
            var p1 = new PlayerInfo { Name = "P1", MoneySafeAmnt = 21 };
            var p2 = new PlayerInfo { Name = "P2", MoneySafeAmnt = 1163 };
            var p3 = new PlayerInfo { Name = "P3", MoneySafeAmnt = 142 };
            var p4 = new PlayerInfo { Name = "P4", MoneySafeAmnt = 5163 };
            bank.CollectMoneyFromPlayer(p1, 21);
            bank.CollectMoneyFromPlayer(p2, 1163);
            bank.CollectMoneyFromPlayer(p3, 142);
            bank.CollectMoneyFromPlayer(p4, 2163);
            p1.State = PlayerStateEnum.AllIn;
            p2.State = PlayerStateEnum.AllIn;
            p3.State = PlayerStateEnum.AllIn;
            p4.State = PlayerStateEnum.Playing;
            bank.DepositMoneyInPlay();

            //Act
            var res = bank.PotAmountsPadded(6).ToArray();

            //Assert
            Assert.AreEqual(6, res.Length);
            Assert.AreEqual(84, res[0]);
            Assert.AreEqual(363, res[1]);
            Assert.AreEqual(2042, res[2]);
            Assert.AreEqual(1000, res[3]);
            Assert.AreEqual(0, res[4]);
            Assert.AreEqual(0, res[5]);
        }
    }
}
