using BluffinMuffin.Protocol.DataTypes.Enums;
using BluffinMuffin.Server.Logic.Test.PokerGameTests.DataTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BluffinMuffin.Server.Logic.Test.PokerGameTests
{
    [TestClass]
    public class BettingRoundsTests
    {
        [TestMethod]
        public void AfterSeatedInNoBlindsRoundIsPreflop()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.None)).WithAllPlayersSeated();

            //Act

            //Assert
            Assert.AreEqual(1, nfo.Game.Table.BettingRoundId, "The game should now be in the preflop round");
        }

        [TestMethod]
        public void AfterBlindsRoundIsPreflop()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).BlindsPosted();

            //Act

            //Assert
            Assert.AreEqual(1, nfo.Game.Table.BettingRoundId, "The game should now be in the preflop round");
        }

        [TestMethod]
        public void AfterAntesRoundIsPreflop()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Antes)).BlindsPosted();

            //Act

            //Assert
            Assert.AreEqual(1, nfo.Game.Table.BettingRoundId, "The game should now be in the preflop round");
        }
        [TestMethod]
        public void AfterPreflopRoundIsFlop()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).AfterPreflop();

            //Act

            //Assert
            Assert.AreEqual(2, nfo.Game.Table.BettingRoundId, "The game should now be in the flop round");
        }
        [TestMethod]
        public void AfterFlopRoundIsTurn()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).AfterFlop();

            //Act

            //Assert
            Assert.AreEqual(3, nfo.Game.Table.BettingRoundId, "The game should now be in the turn round");
        }
        [TestMethod]
        public void AfterTurnRoundIsRiver()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).AfterTurn();

            //Act

            //Assert
            Assert.AreEqual(4, nfo.Game.Table.BettingRoundId, "The game should now be in the river round");
        }

        [TestMethod]
        public void AfterFirstPlayerCallRoundStillPreflop()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).BlindsPosted();

            //Act
            nfo.CurrentPlayerCalls();

            //Assert
            Assert.AreEqual(1, nfo.Game.Table.BettingRoundId, "The game should still be in the preflop round");
        }

        [TestMethod]
        public void AfterBothPlayerCallOnPreflopRoundNowFlop()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).AfterPreflop();

            //Act

            //Assert
            Assert.AreEqual(2, nfo.Game.Table.BettingRoundId, "The game should now be in the flop round");
        }

        [TestMethod]
        public void AfterFirstPlayerCheckOnFlopRoundStillFlop()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).AfterPreflop();

            //Act
            nfo.CurrentPlayerChecks();

            //Assert
            Assert.AreEqual(2, nfo.Game.Table.BettingRoundId, "The game should still be in the flop round");
        }

        [TestMethod]
        public void AfterSecondPlayerBetOnFlopRoundStillFlop()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).AfterPreflop();

            //Act
            nfo.CurrentPlayerChecks();
            nfo.CurrentPlayerRaisesMinimum();

            //Assert
            Assert.AreEqual(2, nfo.Game.Table.BettingRoundId, "The game should still be in the flop round");
        }

        [TestMethod]
        public void AfterSecondPlayerChecksNowTurn()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).AfterPreflop();

            //Act
            nfo.CurrentPlayerChecks();
            nfo.CurrentPlayerChecks();

            //Assert
            Assert.AreEqual(3, nfo.Game.Table.BettingRoundId, "The game should now be in the Turn round");
        }

        [TestMethod]
        public void AfterSecondPlayerCallsNowTurn()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).AfterPreflop();

            //Act
            nfo.CurrentPlayerRaisesMinimum();
            nfo.CurrentPlayerCalls();

            //Assert
            Assert.AreEqual(3, nfo.Game.Table.BettingRoundId, "The game should now be in the Turn round");
        }

        [TestMethod]
        public void AfterOnlyRaiseShouldStayFlop()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).AfterPreflop();

            //Act
            nfo.CurrentPlayerRaisesMinimum();
            nfo.CurrentPlayerRaisesMinimum();
            nfo.CurrentPlayerRaisesMinimum();
            nfo.CurrentPlayerRaisesMinimum();
            nfo.CurrentPlayerRaisesMinimum();

            //Assert
            Assert.AreEqual(2, nfo.Game.Table.BettingRoundId, "The game should still be in the flop round");
        }

        [TestMethod]
        public void AfterRaisesThenCallShouldNowTurn()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).AfterPreflop();

            //Act
            nfo.CurrentPlayerRaisesMinimum();
            nfo.CurrentPlayerRaisesMinimum();
            nfo.CurrentPlayerRaisesMinimum();
            nfo.CurrentPlayerRaisesMinimum();
            nfo.CurrentPlayerCalls();

            //Assert
            Assert.AreEqual(3, nfo.Game.Table.BettingRoundId, "The game should now be in the Turn round");
        }
    }
}
