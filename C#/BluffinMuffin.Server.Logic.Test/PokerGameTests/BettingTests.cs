using BluffinMuffin.Server.Logic.Test.PokerGameTests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BluffinMuffin.Server.Logic.Test.PokerGameTests
{
    [TestClass]
    public class BettingTests
    {

        [TestMethod]
        public void AfterBlindsFirstPlayerCanCall()
        {
            var nfo = Simple2PlayersBlindsGameMock.BlindsPosted();

            Assert.AreEqual(true, nfo.CurrentPlayerCalls(), "The first player should be allowed to call");
        }

        [TestMethod]
        public void AfterFirstPlayerCallSecondPlayerCanCall()
        {
            var nfo = Simple2PlayersBlindsGameMock.BlindsPosted();
            nfo.CurrentPlayerCalls();

            Assert.AreEqual(true, nfo.CurrentPlayerCalls(), "The second player should be allowed to call");  
        }

        [TestMethod]
        public void AtStartOfBettingFirstPlayerChecks()
        {
            var nfo = Simple2PlayersBlindsGameMock.AfterPreflop();

            Assert.AreEqual(true, nfo.CurrentPlayerChecks(), "The first player should be allowed to call");
        }

        [TestMethod]
        public void AtStartOfBettingFirstPlayerBetsUnderMinimum()
        {
            var nfo = Simple2PlayersBlindsGameMock.AfterPreflop();

            Assert.AreEqual(false, nfo.CurrentPlayerPlays(nfo.Game.Table.MinRaiseAmnt(nfo.CurrentPlayer) - 1), "The player should not be able to raise under the minimum");
        }

        [TestMethod]
        public void AtStartOfBettingFirstPlayerBetsMinimum()
        {
            var nfo = Simple2PlayersBlindsGameMock.AfterPreflop();
            nfo.CurrentPlayerChecks();

            Assert.AreEqual(true, nfo.CurrentPlayerRaisesMinimum(), "The player should be able to raise with the minimum");
        }

        [TestMethod]
        public void AtStartOfBettingFirstPlayerBetsOverMinimum()
        {
            var nfo = Simple2PlayersBlindsGameMock.AfterPreflop();
            nfo.CurrentPlayerChecks();

            Assert.AreEqual(true, nfo.CurrentPlayerPlays(nfo.Game.Table.MinRaiseAmnt(nfo.CurrentPlayer) + 1), "The player should be able to raise with more than the minimum");
        }

        [TestMethod]
        public void AfterPlayerBetShouldNotBeAbleToCheck()
        {
            var nfo = Simple2PlayersBlindsGameMock.AfterPreflop();
            nfo.CurrentPlayerRaisesMinimum();

            Assert.AreEqual(false, nfo.CurrentPlayerChecks(), "The player should not be able to check after a bet");
        }

        [TestMethod]
        public void AllIn()
        {
            var nfo = Simple2PlayersBlindsGameMock.AfterPreflop();
            if (nfo.CurrentPlayer == nfo.PoorestPlayer)
                nfo.CurrentPlayerChecks();

            nfo.CurrentPlayerPlays(nfo.PoorestPlayer.MoneySafeAmnt + 10);

            Assert.AreEqual(true, nfo.CurrentPlayerCalls(), "The first player should be allowed to go all-in");
        }
    }
}
