using BluffinMuffin.Protocol.DataTypes.Enums;
using BluffinMuffin.Server.Logic.Test.PokerGameTests.DataTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BluffinMuffin.Server.Logic.Test.PokerGameTests
{
    [TestClass]
    public class WhoIsUnderTheGunTests
    {
        [TestMethod]
        public void Game2PNoBlindsUtgIsNextToDealerOnPreflop()
        {
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.None)).WithAllPlayersSeated();

            Assert.AreEqual(nfo.CurrentPlayer, nfo.PlayerNextTo(nfo.Dealer), "Player next to dealer should be under the gun on preflop");
        }
        [TestMethod]
        public void Game2PNoBlindsUtgIsNextToDealerOnFlop()
        {
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.None)).AfterPreflop();

            Assert.AreEqual(nfo.CurrentPlayer, nfo.PlayerNextTo(nfo.Dealer), "Player next to dealer should be under the gun on flop");
        }
        [TestMethod]
        public void Game2PUtgIsDealerOnPreflop()
        {
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).BlindsPosted();

            Assert.AreEqual(nfo.CurrentPlayer, nfo.Dealer, "Dealer should be under the gun on preflop");
        }
        [TestMethod]
        public void Game2PUtgIsNextToDealerOnFlop()
        {
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).AfterPreflop();

            Assert.AreEqual(nfo.CurrentPlayer, nfo.PlayerNextTo(nfo.Dealer), "Player next to dealer should be under the gun on flop");
        }

        [TestMethod]
        public void Game2PAntesUtgIsNextDealerOnPreflop()
        {
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Antes)).BlindsPosted();

            Assert.AreEqual(nfo.CurrentPlayer, nfo.PlayerNextTo(nfo.Dealer), "Player next to dealer should be under the gun on preflop");
        }
        [TestMethod]
        public void Game2PAntesUtgIsNextToDealerOnFlop()
        {
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Antes)).AfterPreflop();

            Assert.AreEqual(nfo.CurrentPlayer, nfo.PlayerNextTo(nfo.Dealer), "Player next to dealer should be under the gun on flop");
        }
        [TestMethod]
        public void Game3PNoBlindsUtgIsNextToDealerOnPreflop()
        {
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.None), new NbPlayersModule(3)).WithAllPlayersSeated();

            Assert.AreEqual(nfo.CurrentPlayer, nfo.PlayerNextTo(nfo.Dealer), "Player next to dealer should be under the gun on preflop");
        }
        [TestMethod]
        public void Game3PNoBlindsUtgIsNextToDealerOnFlop()
        {
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.None), new NbPlayersModule(3)).AfterPreflop();

            Assert.AreEqual(nfo.CurrentPlayer, nfo.PlayerNextTo(nfo.Dealer), "Player next to dealer should be under the gun on flop");
        }
        [TestMethod]
        public void Game3PUtgIsNextToBigBlindOnPreflop()
        {
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds), new NbPlayersModule(3)).BlindsPosted();

            Assert.AreEqual(nfo.CurrentPlayer, nfo.PlayerNextTo(nfo.PlayerNextTo(nfo.PlayerNextTo(nfo.Dealer))), "Player next to the big blind should be under the gun on preflop");
        }
        [TestMethod]
        public void Game3PUtgIsNextToDealerOnFlop()
        {
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds), new NbPlayersModule(3)).AfterPreflop();

            Assert.AreEqual(nfo.CurrentPlayer, nfo.PlayerNextTo(nfo.Dealer), "Player next to dealer should be under the gun on flop");
        }
        [TestMethod]
        public void Game3PAntesUtgIsNextToDealerOnPreflop()
        {
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Antes), new NbPlayersModule(3)).BlindsPosted();

            Assert.AreEqual(nfo.CurrentPlayer, nfo.PlayerNextTo(nfo.Dealer), "Player next to dealer should be under the gun on preflop");
        }
        [TestMethod]
        public void Game3PAntesUtgIsNextToDealerOnFlop()
        {
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Antes), new NbPlayersModule(3)).AfterPreflop();

            Assert.AreEqual(nfo.CurrentPlayer, nfo.PlayerNextTo(nfo.Dealer), "Player next to dealer should be under the gun on flop");
        }
        [TestMethod]
        public void Game4PNoBlindsUtgIsNextToDealerOnPreflop()
        {
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.None), new NbPlayersModule(4)).WithAllPlayersSeated();

            Assert.AreEqual(nfo.CurrentPlayer, nfo.PlayerNextTo(nfo.Dealer), "Player next to dealer should be under the gun on preflop");
        }
        [TestMethod]
        public void Game4PNoBlindsUtgIsNextToDealerOnFlop()
        {
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.None), new NbPlayersModule(4)).AfterPreflop();

            Assert.AreEqual(nfo.CurrentPlayer, nfo.PlayerNextTo(nfo.Dealer), "Player next to dealer should be under the gun on flop");
        }
        [TestMethod]
        public void Game4PUtgIsNextToBigBlindOnPreflop()
        {
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds), new NbPlayersModule(4)).BlindsPosted();

            Assert.AreEqual(nfo.CurrentPlayer, nfo.PlayerNextTo(nfo.PlayerNextTo(nfo.PlayerNextTo(nfo.Dealer))), "Player next to big blind should be under the gun on preflop");
        }
        [TestMethod]
        public void Game4PUtgIsNextToDealerOnFlop()
        {
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds), new NbPlayersModule(4)).AfterPreflop();

            Assert.AreEqual(nfo.CurrentPlayer, nfo.PlayerNextTo(nfo.Dealer), "Player next to dealer should be under the gun on flop");
        }
        [TestMethod]
        public void Game4PAntesUtgIsNextToDealerOnPreflop()
        {
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Antes), new NbPlayersModule(4)).BlindsPosted();

            Assert.AreEqual(nfo.CurrentPlayer, nfo.PlayerNextTo(nfo.Dealer), "Player next to dealer should be under the gun on preflop");
        }
        [TestMethod]
        public void Game4PAntesUtgIsNextToDealerOnFlop()
        {
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Antes), new NbPlayersModule(4)).AfterPreflop();

            Assert.AreEqual(nfo.CurrentPlayer, nfo.PlayerNextTo(nfo.Dealer), "Player next to dealer should be under the gun on flop");
        }
    }
}
