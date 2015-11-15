using System.Linq;
using BluffinMuffin.Protocol.DataTypes.Enums;
using BluffinMuffin.Server.Logic.Extensions;
using BluffinMuffin.Server.Logic.Test.PokerGameTests.DataTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BluffinMuffin.Server.Logic.Test.PokerGameTests
{
    [TestClass]
    public class BlindsTests
    {
        [TestMethod]
        public void AntesGameAllPlayerNeedsToPutTheSameBlind()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Antes), new NbPlayersModule(4)).WithAllPlayersSeated();
            
            //Act
            var res = nfo.Players.All(x => nfo.BlindNeeded(x) == nfo.Game.Table.Params.AnteAmount());

            //Assert
            Assert.AreEqual(true, res, "The game should need the same blind for everybody (Antes)");
        }

        [TestMethod]
        public void StartGameAndCheckNeededBlindP1()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).WithAllPlayersSeated();

            //Act
            var res = nfo.BlindNeeded(nfo.P1);

            //Assert
            Assert.AreNotEqual(0, res, "The game should need a blind from p1");
        }

        [TestMethod]
        public void StartGameAndCheckNeededBlindP2()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).WithAllPlayersSeated();

            //Act
            var res = nfo.BlindNeeded(nfo.P2);

            //Assert
            Assert.AreNotEqual(0, res, "The game should need a blind from p2");
        }

        [TestMethod]
        public void StartGameAndTryPutBlindMoreThanNeeded()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).WithAllPlayersSeated();

            //Act
            var res = nfo.Game.PlayMoney(nfo.P1, nfo.BlindNeeded(nfo.P1) + 1);

            //Assert
            Assert.AreEqual(false, res, "The game should not accept any blind that is over what is needed");
        }

        [TestMethod]
        public void StartGameAndTryPutBlindLessThanNeeded()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).WithAllPlayersSeated();

            //Act
            var res = nfo.Game.PlayMoney(nfo.P1, nfo.BlindNeeded(nfo.P1) - 1);

            //Assert
            Assert.AreEqual(false, res, "The game should not accept any blind that is under what is needed unless that is all the player got");
        }

        [TestMethod]
        public void StartGameAndTryPutBlindLessThanNeededWithPoorPlayer()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).WithAllPlayersSeated();
            nfo.P2.MoneySafeAmnt = 2;

            //Act
            var res = nfo.Game.PlayMoney(nfo.P2, nfo.P2.MoneySafeAmnt);

            //Assert
            Assert.AreEqual(true, res, "The game should accept a blind that is under what is needed if that is all the player got");
        }

        [TestMethod]
        public void StartGameAndTryPutBlind()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).WithAllPlayersSeated();

            //Act
            var res = nfo.Game.PlayMoney(nfo.P1, nfo.BlindNeeded(nfo.P1));

            //Assert
            Assert.AreEqual(true, res, "The game should accept a perfect blind");
        }

        [TestMethod]
        public void StartGameAndCheckNeededBlindP1AfterP1PutHis()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).WithAllPlayersSeated();
            nfo.PutBlinds(nfo.P1);

            //Act
            var res = nfo.BlindNeeded(nfo.P1);

            //Assert
            Assert.AreEqual(0, res, "The game should not need a blind from p1 anymore");
        }

        [TestMethod]
        public void StartGameAndCheckNeededBlindP2AfterP1PutHis()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).WithAllPlayersSeated();
            nfo.PutBlinds(nfo.P1);

            //Act
            var res = nfo.BlindNeeded(nfo.P2);

            //Assert
            Assert.AreNotEqual(0, res, "The game should still need a blind from p2");
        }

        [TestMethod]
        public void LeaveGameBeforePuttingBlindShouldStillSubstractTheAmountFromMoney()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).WithAllPlayersSeated();

            //Act
            var safeMoneyBefore = nfo.P1.MoneySafeAmnt;
            nfo.Game.LeaveGame(nfo.P1);
            var res = nfo.P1.MoneySafeAmnt < safeMoneyBefore;

            //Assert
            Assert.AreEqual(true, res, "The player should have less money then before, since blinds were posted automatically before he left");
        }

        [TestMethod]
        public void StartSecondGameAndCheckNeededBlindOfBothPlayers()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).BlindsPosted();

            //Act
            nfo.CurrentPlayerFolds(); // This will start a new game !
            var res = nfo.BlindNeeded(nfo.P1) != nfo.BlindNeeded(nfo.P2);

            //Assert
            Assert.AreEqual(true, res, "The second game should need a big blind and a small blind, not two big blinds");
        }
    }
}
