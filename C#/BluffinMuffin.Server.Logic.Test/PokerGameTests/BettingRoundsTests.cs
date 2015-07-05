using BluffinMuffin.Server.Logic.Test.PokerGameTests.Mocks;
using BluffinMuffin.Protocol.DataTypes.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BluffinMuffin.Server.Logic.Test.PokerGameTests
{
    [TestClass]
    public class BettingRoundsTests
    {
        [TestMethod]
        public void AfterSeatedInNoBlindsRoundIsPreflop()
        {
            var nfo = Simple2PlayersNoBlindsGameMock.WithBothPlayersSeated();

            Assert.AreEqual(1, nfo.Game.Table.BettingRoundId, "The game should now be in the preflop round");
        }

        [TestMethod]
        public void AfterBlindsRoundIsPreflop()
        {
            var nfo = Simple2PlayersBlindsGameMock.BlindsPosted();

            Assert.AreEqual(1, nfo.Game.Table.BettingRoundId, "The game should now be in the preflop round");
        }

        [TestMethod]
        public void AfterAntesRoundIsPreflop()
        {
            var nfo = Simple2PlayersAntesGameMock.BlindsPosted();

            Assert.AreEqual(1, nfo.Game.Table.BettingRoundId, "The game should now be in the preflop round");
        }
        [TestMethod]
        public void AfterPreflopRoundIsFlop()
        {
            var nfo = Simple2PlayersBlindsGameMock.AfterPreflop();

            Assert.AreEqual(2, nfo.Game.Table.BettingRoundId, "The game should now be in the flop round");
        }
        [TestMethod]
        public void AfterFlopRoundIsTurn()
        {
            var nfo = Simple2PlayersBlindsGameMock.AfterFlop();

            Assert.AreEqual(3, nfo.Game.Table.BettingRoundId, "The game should now be in the turn round");
        }
        [TestMethod]
        public void AfterTurnRoundIsRiver()
        {
            var nfo = Simple2PlayersBlindsGameMock.AfterTurn();

            Assert.AreEqual(4, nfo.Game.Table.BettingRoundId, "The game should now be in the river round");
        }

        [TestMethod]
        public void AfterFirstPlayerCallRoundStillPreflop()
        {
            var nfo = Simple2PlayersBlindsGameMock.BlindsPosted();

            nfo.CurrentPlayerCalls();

            Assert.AreEqual(1, nfo.Game.Table.BettingRoundId, "The game should still be in the preflop round");
        }

        [TestMethod]
        public void AfterBothPlayerCallOnPreflopRoundNowFlop()
        {
            var nfo = Simple2PlayersBlindsGameMock.AfterPreflop();

            Assert.AreEqual(2, nfo.Game.Table.BettingRoundId, "The game should now be in the flop round");
        }

        [TestMethod]
        public void AfterFirstPlayerCheckOnFlopRoundStillFlop()
        {
            var nfo = Simple2PlayersBlindsGameMock.AfterPreflop();

            nfo.CurrentPlayerChecks();

            Assert.AreEqual(2, nfo.Game.Table.BettingRoundId, "The game should still be in the flop round");
        }

        [TestMethod]
        public void AfterSecondPlayerBetOnFlopRoundStillFlop()
        {
            var nfo = Simple2PlayersBlindsGameMock.AfterPreflop();

            nfo.CurrentPlayerChecks();
            nfo.CurrentPlayerRaisesMinimum();

            Assert.AreEqual(2, nfo.Game.Table.BettingRoundId, "The game should still be in the flop round");
        }

        [TestMethod]
        public void AfterSecondPlayerChecksNowTurn()
        {
            var nfo = Simple2PlayersBlindsGameMock.AfterPreflop();

            nfo.CurrentPlayerChecks();
            nfo.CurrentPlayerChecks();

            Assert.AreEqual(3, nfo.Game.Table.BettingRoundId, "The game should now be in the Turn round");
        }

        [TestMethod]
        public void AfterSecondPlayerCallsNowTurn()
        {
            var nfo = Simple2PlayersBlindsGameMock.AfterPreflop();

            nfo.CurrentPlayerRaisesMinimum();
            nfo.CurrentPlayerCalls();

            Assert.AreEqual(3, nfo.Game.Table.BettingRoundId, "The game should now be in the Turn round");
        }

        [TestMethod]
        public void AfterOnlyRaiseShouldStayFlop()
        {
            var nfo = Simple2PlayersBlindsGameMock.AfterPreflop();

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
            var nfo = Simple2PlayersBlindsGameMock.AfterPreflop();

            nfo.CurrentPlayerRaisesMinimum();
            nfo.CurrentPlayerRaisesMinimum();
            nfo.CurrentPlayerRaisesMinimum();
            nfo.CurrentPlayerRaisesMinimum();
            nfo.CurrentPlayerCalls();

            Assert.AreEqual(3, nfo.Game.Table.BettingRoundId, "The game should now be in the Turn round");
        }
    }
}
