using BluffinMuffin.Server.Logic.Test.PokerGameTests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BluffinMuffin.Server.Logic.Test.PokerGameTests
{
    [TestClass]
    public class WhoIsUnderTheGunTests
    {
        [TestMethod]
        public void Game2PNoBlindsUtgIsNextToDealerOnPreflop()
        {
            var nfo = Simple2PlayersNoBlindsGameMock.WithBothPlayersSeated();

            Assert.AreEqual(nfo.CurrentPlayer, nfo.PlayerNextTo(nfo.Dealer), "Player next to dealer should be under the gun on preflop");
        }
        [TestMethod]
        public void Game2PNoBlindsUtgIsNextToDealerOnFlop()
        {
            var nfo = Simple2PlayersNoBlindsGameMock.AfterPreflop();

            Assert.AreEqual(nfo.CurrentPlayer, nfo.PlayerNextTo(nfo.Dealer), "Player next to dealer should be under the gun on flop");
        }
        [TestMethod]
        public void Game2PUtgIsDealerOnPreflop()
        {
            var nfo = Simple2PlayersBlindsGameMock.BlindsPosted();

            Assert.AreEqual(nfo.CurrentPlayer, nfo.Dealer, "Dealer should be under the gun on preflop");
        }
        [TestMethod]
        public void Game2PUtgIsNextToDealerOnFlop()
        {
            var nfo = Simple2PlayersBlindsGameMock.AfterPreflop();

            Assert.AreEqual(nfo.CurrentPlayer, nfo.PlayerNextTo(nfo.Dealer), "Player next to dealer should be under the gun on flop");
        }

        [TestMethod]
        public void Game2PAntesUtgIsNextDealerOnPreflop()
        {
            var nfo = Simple2PlayersAntesGameMock.BlindsPosted();

            Assert.AreEqual(nfo.CurrentPlayer, nfo.PlayerNextTo(nfo.Dealer), "Player next to dealer should be under the gun on preflop");
        }
        [TestMethod]
        public void Game2PAntesUtgIsNextToDealerOnFlop()
        {
            var nfo = Simple2PlayersAntesGameMock.AfterPreflop();

            Assert.AreEqual(nfo.CurrentPlayer, nfo.PlayerNextTo(nfo.Dealer), "Player next to dealer should be under the gun on flop");
        }
        [TestMethod]
        public void Game3PNoBlindsUtgIsNextToDealerOnPreflop()
        {
            var nfo = Simple3PlayersNoBlindsGameMock.WithAllPlayersSeated();

            Assert.AreEqual(nfo.CurrentPlayer, nfo.PlayerNextTo(nfo.Dealer), "Player next to dealer should be under the gun on preflop");
        }
        [TestMethod]
        public void Game3PNoBlindsUtgIsNextToDealerOnFlop()
        {
            var nfo = Simple3PlayersNoBlindsGameMock.AfterPreflop();

            Assert.AreEqual(nfo.CurrentPlayer, nfo.PlayerNextTo(nfo.Dealer), "Player next to dealer should be under the gun on flop");
        }
        [TestMethod]
        public void Game3PUtgIsNextToBigBlindOnPreflop()
        {
            var nfo = Simple3PlayersBlindsGameMock.BlindsPosted();

            Assert.AreEqual(nfo.CurrentPlayer, nfo.PlayerNextTo(nfo.PlayerNextTo(nfo.PlayerNextTo(nfo.Dealer))), "Player next to the big blind should be under the gun on preflop");
        }
        [TestMethod]
        public void Game3PUtgIsNextToDealerOnFlop()
        {
            var nfo = Simple3PlayersBlindsGameMock.AfterPreflop();

            Assert.AreEqual(nfo.CurrentPlayer, nfo.PlayerNextTo(nfo.Dealer), "Player next to dealer should be under the gun on flop");
        }
        [TestMethod]
        public void Game3PAntesUtgIsNextToDealerOnPreflop()
        {
            var nfo = Simple3PlayersAntesGameMock.BlindsPosted();

            Assert.AreEqual(nfo.CurrentPlayer, nfo.PlayerNextTo(nfo.Dealer), "Player next to dealer should be under the gun on preflop");
        }
        [TestMethod]
        public void Game3PAntesUtgIsNextToDealerOnFlop()
        {
            var nfo = Simple3PlayersAntesGameMock.AfterPreflop();

            Assert.AreEqual(nfo.CurrentPlayer, nfo.PlayerNextTo(nfo.Dealer), "Player next to dealer should be under the gun on flop");
        }
        [TestMethod]
        public void Game4PNoBlindsUtgIsNextToDealerOnPreflop()
        {
            var nfo = Simple4PlayersNoBlindsGameMock.WithAllPlayersSeated();

            Assert.AreEqual(nfo.CurrentPlayer, nfo.PlayerNextTo(nfo.Dealer), "Player next to dealer should be under the gun on preflop");
        }
        [TestMethod]
        public void Game4PNoBlindsUtgIsNextToDealerOnFlop()
        {
            var nfo = Simple4PlayersNoBlindsGameMock.AfterPreflop();

            Assert.AreEqual(nfo.CurrentPlayer, nfo.PlayerNextTo(nfo.Dealer), "Player next to dealer should be under the gun on flop");
        }
        [TestMethod]
        public void Game4PUtgIsNextToBigBlindOnPreflop()
        {
            var nfo = Simple4PlayersBlindsGameMock.BlindsPosted();

            Assert.AreEqual(nfo.CurrentPlayer, nfo.PlayerNextTo(nfo.PlayerNextTo(nfo.PlayerNextTo(nfo.Dealer))), "Player next to big blind should be under the gun on preflop");
        }
        [TestMethod]
        public void Game4PUtgIsNextToDealerOnFlop()
        {
            var nfo = Simple4PlayersBlindsGameMock.AfterPreflop();

            Assert.AreEqual(nfo.CurrentPlayer, nfo.PlayerNextTo(nfo.Dealer), "Player next to dealer should be under the gun on flop");
        }
        [TestMethod]
        public void Game4PAntesUtgIsNextToDealerOnPreflop()
        {
            var nfo = Simple4PlayersAntesGameMock.BlindsPosted();

            Assert.AreEqual(nfo.CurrentPlayer, nfo.PlayerNextTo(nfo.Dealer), "Player next to dealer should be under the gun on preflop");
        }
        [TestMethod]
        public void Game4PAntesUtgIsNextToDealerOnFlop()
        {
            var nfo = Simple4PlayersAntesGameMock.AfterPreflop();

            Assert.AreEqual(nfo.CurrentPlayer, nfo.PlayerNextTo(nfo.Dealer), "Player next to dealer should be under the gun on flop");
        }
    }
}
