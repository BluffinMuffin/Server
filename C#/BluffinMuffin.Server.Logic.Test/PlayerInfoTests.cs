using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Protocol.DataTypes.Enums;
using BluffinMuffin.Server.Logic.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BluffinMuffin.Server.Logic.Test
{
    [TestClass]
    public class PlayerInfoTests
    {
        [TestMethod]
        public void IsReadyToPlayWhenSeatAndMoney()
        {
            //Arrange
            var p = new PlayerInfo() { MoneySafeAmnt = 2000, NoSeat = 7 };

            //Act
            var res = p.IsReadyToPlay();

            //Assert
            Assert.AreEqual(true, res);
        }
        [TestMethod]
        public void IsNotReadyToPlayWhenSeatAndNoMoney()
        {
            //Arrange
            var p = new PlayerInfo() { MoneySafeAmnt = 0, NoSeat = 7 };

            //Act
            var res = p.IsReadyToPlay();

            //Assert
            Assert.AreEqual(false, res);
        }
        [TestMethod]
        public void IsReadyToPlayWhenNoSeatAndMoney()
        {
            //Arrange
            var p = new PlayerInfo() { MoneySafeAmnt = 2000, NoSeat = -1 };

            //Act
            var res = p.IsReadyToPlay();

            //Assert
            Assert.AreEqual(false, res);
        }
        [TestMethod]
        public void IsReadyToPlayWhenNoSeatAndNoMoney()
        {
            //Arrange
            var p = new PlayerInfo() { MoneySafeAmnt = 0, NoSeat = -1 };

            //Act
            var res = p.IsReadyToPlay();

            //Assert
            Assert.AreEqual(false, res);
        }
        [TestMethod]
        public void NoChangeIfTriedBetWithNotEnoughMoney()
        {
            //Arrange
            var p = new PlayerInfo() { MoneyBetAmnt = 500, MoneySafeAmnt = 2000 };

            //Act
            var res = p.TryBet(4242);

            //Assert
            Assert.AreEqual(false, res);
            Assert.AreEqual(500, p.MoneyBetAmnt);
            Assert.AreEqual(2000, p.MoneySafeAmnt);
        }
        [TestMethod]
        public void MoneyChangeIfTriedBetWithEnoughMoney()
        {
            //Arrange
            var p = new PlayerInfo() { MoneyBetAmnt = 500, MoneySafeAmnt = 5000 };

            //Act
            var res = p.TryBet(4242);

            //Assert
            Assert.AreEqual(true, res);
            Assert.AreEqual(500 + 4242, p.MoneyBetAmnt);
            Assert.AreEqual(5000 - 4242, p.MoneySafeAmnt);
        }

        [TestMethod]
        public void CantBetIfNotEnoughMoney()
        {
            //Arrange
            var p = new PlayerInfo() { MoneySafeAmnt = 2000 };

            //Act
            var res = p.CanBet(4242);

            //Assert
            Assert.AreEqual(false, res);
        }

        [TestMethod]
        public void CanBetIfEnoughMoney()
        {
            //Arrange
            var p = new PlayerInfo() { MoneySafeAmnt = 5000 };

            //Act
            var res = p.CanBet(4242);

            //Assert
            Assert.AreEqual(true, res);
        }

        [TestMethod]
        public void JoinedPlayerIsNotPlayingNorAllIn()
        {
            //Arrange
            var p = new PlayerInfo() { State = PlayerStateEnum.Joined };

            //Act
            var res = p.IsPlayingOrAllIn();

            //Assert
            Assert.AreEqual(false, res);
        }

        [TestMethod]
        public void SitInPlayerIsNotPlayingNorAllIn()
        {
            //Arrange
            var p = new PlayerInfo() { State = PlayerStateEnum.SitIn };

            //Act
            var res = p.IsPlayingOrAllIn();

            //Assert
            Assert.AreEqual(false, res);
        }

        [TestMethod]
        public void AllInPlayerIsPlayingOrAllIn()
        {
            //Arrange
            var p = new PlayerInfo() { State = PlayerStateEnum.AllIn };

            //Act
            var res = p.IsPlayingOrAllIn();

            //Assert
            Assert.AreEqual(true, res);
        }

        [TestMethod]
        public void PlayingPlayerIsPlayingOrAllIn()
        {
            //Arrange
            var p = new PlayerInfo() { State = PlayerStateEnum.Playing };

            //Act
            var res = p.IsPlayingOrAllIn();

            //Assert
            Assert.AreEqual(true, res);
        }
    }
}
