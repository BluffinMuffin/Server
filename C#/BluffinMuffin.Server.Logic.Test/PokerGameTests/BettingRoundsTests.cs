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
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.None)).WithAllPlayersSeated();

            Assert.AreEqual(1, nfo.Game.Table.BettingRoundId, "The game should now be in the preflop round");
        }

        [TestMethod]
        public void AfterBlindsRoundIsPreflop()
        {
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).BlindsPosted();

            Assert.AreEqual(1, nfo.Game.Table.BettingRoundId, "The game should now be in the preflop round");
        }

        [TestMethod]
        public void AfterAntesRoundIsPreflop()
        {
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Antes)).BlindsPosted();

            Assert.AreEqual(1, nfo.Game.Table.BettingRoundId, "The game should now be in the preflop round");
        }
        [TestMethod]
        public void AfterPreflopRoundIsFlop()
        {
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).AfterPreflop();

            Assert.AreEqual(2, nfo.Game.Table.BettingRoundId, "The game should now be in the flop round");
        }
        [TestMethod]
        public void AfterFlopRoundIsTurn()
        {
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).AfterFlop();

            Assert.AreEqual(3, nfo.Game.Table.BettingRoundId, "The game should now be in the turn round");
        }
        [TestMethod]
        public void AfterTurnRoundIsRiver()
        {
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).AfterTurn();

            Assert.AreEqual(4, nfo.Game.Table.BettingRoundId, "The game should now be in the river round");
        }

        [TestMethod]
        public void AfterFirstPlayerCallRoundStillPreflop()
        {
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).BlindsPosted();

            nfo.CurrentPlayerCalls();

            Assert.AreEqual(1, nfo.Game.Table.BettingRoundId, "The game should still be in the preflop round");
        }

        [TestMethod]
        public void AfterBothPlayerCallOnPreflopRoundNowFlop()
        {
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).AfterPreflop();

            Assert.AreEqual(2, nfo.Game.Table.BettingRoundId, "The game should now be in the flop round");
        }

        [TestMethod]
        public void AfterFirstPlayerCheckOnFlopRoundStillFlop()
        {
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).AfterPreflop();

            nfo.CurrentPlayerChecks();

            Assert.AreEqual(2, nfo.Game.Table.BettingRoundId, "The game should still be in the flop round");
        }

        [TestMethod]
        public void AfterSecondPlayerBetOnFlopRoundStillFlop()
        {
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).AfterPreflop();

            nfo.CurrentPlayerChecks();
            nfo.CurrentPlayerRaisesMinimum();

            Assert.AreEqual(2, nfo.Game.Table.BettingRoundId, "The game should still be in the flop round");
        }

        [TestMethod]
        public void AfterSecondPlayerChecksNowTurn()
        {
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).AfterPreflop();

            nfo.CurrentPlayerChecks();
            nfo.CurrentPlayerChecks();

            Assert.AreEqual(3, nfo.Game.Table.BettingRoundId, "The game should now be in the Turn round");
        }

        [TestMethod]
        public void AfterSecondPlayerCallsNowTurn()
        {
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).AfterPreflop();

            nfo.CurrentPlayerRaisesMinimum();
            nfo.CurrentPlayerCalls();

            Assert.AreEqual(3, nfo.Game.Table.BettingRoundId, "The game should now be in the Turn round");
        }

        [TestMethod]
        public void AfterOnlyRaiseShouldStayFlop()
        {
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).AfterPreflop();

            nfo.CurrentPlayerRaisesMinimum();
            nfo.CurrentPlayerRaisesMinimum();
            nfo.CurrentPlayerRaisesMinimum();
            nfo.CurrentPlayerRaisesMinimum();
            nfo.CurrentPlayerRaisesMinimum();

            Assert.AreEqual(2, nfo.Game.Table.BettingRoundId, "The game should still be in the flop round");
        }

        [TestMethod]
        public void AfterRaisesThenCallShouldNowTurn()
        {
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).AfterPreflop();

            nfo.CurrentPlayerRaisesMinimum();
            nfo.CurrentPlayerRaisesMinimum();
            nfo.CurrentPlayerRaisesMinimum();
            nfo.CurrentPlayerRaisesMinimum();
            nfo.CurrentPlayerCalls();

            Assert.AreEqual(3, nfo.Game.Table.BettingRoundId, "The game should now be in the Turn round");
        }
    }
}
