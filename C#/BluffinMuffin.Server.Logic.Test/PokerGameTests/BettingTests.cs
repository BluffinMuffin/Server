using BluffinMuffin.Protocol.DataTypes.Enums;
using BluffinMuffin.Server.Logic.Test.PokerGameTests.DataTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BluffinMuffin.Server.Logic.Test.PokerGameTests
{
    [TestClass]
    public class BettingTests
    {

        [TestMethod]
        public void AfterBlindsFirstPlayerCanCall()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).BlindsPosted();

            //Act
            var res = nfo.CurrentPlayerCalls();

            //Assert
            Assert.AreEqual(true, res, "The first player should be allowed to call");
        }

        [TestMethod]
        public void AfterFirstPlayerCallSecondPlayerCanCall()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).BlindsPosted();
            nfo.CurrentPlayerCalls();

            //Act
            var res = nfo.CurrentPlayerCalls();

            //Assert
            Assert.AreEqual(true, res, "The second player should be allowed to call");  
        }

        [TestMethod]
        public void AtStartOfBettingFirstPlayerChecks()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).AfterPreflop();

            //Act
            var res = nfo.CurrentPlayerChecks();

            //Assert
            Assert.AreEqual(true, res, "The first player should be allowed to call");
        }

        [TestMethod]
        public void AtStartOfBettingFirstPlayerBetsUnderMinimum()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).AfterPreflop();

            //Act
            var res = nfo.CurrentPlayerPlays(nfo.Game.Table.MinRaiseAmnt(nfo.CurrentPlayer) - 1);

            //Assert
            Assert.AreEqual(false, res, "The player should not be able to raise under the minimum");
        }

        [TestMethod]
        public void AtStartOfBettingFirstPlayerBetsMinimum()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).AfterPreflop();
            nfo.CurrentPlayerChecks();

            //Act
            var res = nfo.CurrentPlayerRaisesMinimum();

            //Assert
            Assert.AreEqual(true, res, "The player should be able to raise with the minimum");
        }

        [TestMethod]
        public void AtStartOfBettingFirstPlayerBetsOverMinimum()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).AfterPreflop();
            nfo.CurrentPlayerChecks();

            //Act
            var res = nfo.CurrentPlayerPlays(nfo.Game.Table.MinRaiseAmnt(nfo.CurrentPlayer) + 1);

            //Assert
            Assert.AreEqual(true, res, "The player should be able to raise with more than the minimum");
        }

        [TestMethod]
        public void AfterPlayerBetShouldNotBeAbleToCheck()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).AfterPreflop();
            nfo.CurrentPlayerRaisesMinimum();

            //Act
            var res = nfo.CurrentPlayerChecks();

            //Assert
            Assert.AreEqual(false, res, "The player should not be able to check after a bet");
        }

        [TestMethod]
        public void AllIn()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).AfterPreflop();
            if (nfo.CurrentPlayer == nfo.PoorestPlayer)
                nfo.CurrentPlayerChecks();
            nfo.CurrentPlayerPlays(nfo.PoorestPlayer.MoneySafeAmnt + 10);

            //Act
            var res = nfo.CurrentPlayerCalls();

            //Assert
            Assert.AreEqual(true, res, "The first player should be allowed to go all-in");
        }
    }
}
