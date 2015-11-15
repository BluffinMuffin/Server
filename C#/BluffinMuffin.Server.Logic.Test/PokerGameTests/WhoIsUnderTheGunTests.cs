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
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.None)).WithAllPlayersSeated();

            //Act
            var res = nfo.PlayerNextTo(nfo.Dealer);

            //Assert
            Assert.AreEqual(nfo.CurrentPlayer, res, "Player next to dealer should be under the gun on preflop");
        }
        [TestMethod]
        public void Game2PNoBlindsUtgIsNextToDealerOnFlop()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.None)).AfterPreflop();

            //Act
            var res = nfo.PlayerNextTo(nfo.Dealer);

            //Assert
            Assert.AreEqual(nfo.CurrentPlayer, res, "Player next to dealer should be under the gun on flop");
        }
        [TestMethod]
        public void Game2PUtgIsDealerOnPreflop()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).BlindsPosted();

            //Act
            var res = nfo.Dealer;

            //Assert
            Assert.AreEqual(nfo.CurrentPlayer, res, "SeatOfDealer should be under the gun on preflop");
        }
        [TestMethod]
        public void Game2PUtgIsNextToDealerOnFlop()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).AfterPreflop();

            //Act
            var res = nfo.PlayerNextTo(nfo.Dealer);

            //Assert
            Assert.AreEqual(nfo.CurrentPlayer, res, "Player next to dealer should be under the gun on flop");
        }

        [TestMethod]
        public void Game2PAntesUtgIsNextDealerOnPreflop()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Antes)).BlindsPosted();

            //Act
            var res = nfo.PlayerNextTo(nfo.Dealer);

            //Assert
            Assert.AreEqual(nfo.CurrentPlayer, res, "Player next to dealer should be under the gun on preflop");
        }
        [TestMethod]
        public void Game2PAntesUtgIsNextToDealerOnFlop()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Antes)).AfterPreflop();

            //Act
            var res = nfo.PlayerNextTo(nfo.Dealer);

            //Assert
            Assert.AreEqual(nfo.CurrentPlayer, res, "Player next to dealer should be under the gun on flop");
        }
        [TestMethod]
        public void Game3PNoBlindsUtgIsNextToDealerOnPreflop()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.None), new NbPlayersModule(3)).WithAllPlayersSeated();

            //Act
            var res = nfo.PlayerNextTo(nfo.Dealer);

            //Assert
            Assert.AreEqual(nfo.CurrentPlayer, res, "Player next to dealer should be under the gun on preflop");
        }
        [TestMethod]
        public void Game3PNoBlindsUtgIsNextToDealerOnFlop()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.None), new NbPlayersModule(3)).AfterPreflop();

            //Act
            var res = nfo.PlayerNextTo(nfo.Dealer);

            //Assert
            Assert.AreEqual(nfo.CurrentPlayer, res, "Player next to dealer should be under the gun on flop");
        }
        [TestMethod]
        public void Game3PUtgIsNextToBigBlindOnPreflop()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds), new NbPlayersModule(3)).BlindsPosted();

            //Act
            var res = nfo.PlayerNextTo(nfo.PlayerNextTo(nfo.PlayerNextTo(nfo.Dealer)));

            //Assert
            Assert.AreEqual(nfo.CurrentPlayer, res, "Player next to the big blind should be under the gun on preflop");
        }
        [TestMethod]
        public void Game3PUtgIsNextToDealerOnFlop()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds), new NbPlayersModule(3)).AfterPreflop();

            //Act
            var res = nfo.PlayerNextTo(nfo.Dealer);

            //Assert
            Assert.AreEqual(nfo.CurrentPlayer, res, "Player next to dealer should be under the gun on flop");
        }
        [TestMethod]
        public void Game3PAntesUtgIsNextToDealerOnPreflop()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Antes), new NbPlayersModule(3)).BlindsPosted();

            //Act
            var res = nfo.PlayerNextTo(nfo.Dealer);

            //Assert
            Assert.AreEqual(nfo.CurrentPlayer, res, "Player next to dealer should be under the gun on preflop");
        }
        [TestMethod]
        public void Game3PAntesUtgIsNextToDealerOnFlop()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Antes), new NbPlayersModule(3)).AfterPreflop();

            //Act
            var res = nfo.PlayerNextTo(nfo.Dealer);

            //Assert
            Assert.AreEqual(nfo.CurrentPlayer, res, "Player next to dealer should be under the gun on flop");
        }
        [TestMethod]
        public void Game4PNoBlindsUtgIsNextToDealerOnPreflop()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.None), new NbPlayersModule(4)).WithAllPlayersSeated();

            //Act
            var res = nfo.PlayerNextTo(nfo.Dealer);

            //Assert
            Assert.AreEqual(nfo.CurrentPlayer, res, "Player next to dealer should be under the gun on preflop");
        }
        [TestMethod]
        public void Game4PNoBlindsUtgIsNextToDealerOnFlop()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.None), new NbPlayersModule(4)).AfterPreflop();

            //Act
            var res = nfo.PlayerNextTo(nfo.Dealer);

            //Assert
            Assert.AreEqual(nfo.CurrentPlayer, res, "Player next to dealer should be under the gun on flop");
        }
        [TestMethod]
        public void Game4PUtgIsNextToBigBlindOnPreflop()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds), new NbPlayersModule(4)).BlindsPosted();

            //Act
            var res = nfo.PlayerNextTo(nfo.PlayerNextTo(nfo.PlayerNextTo(nfo.Dealer)));

            //Assert
            Assert.AreEqual(nfo.CurrentPlayer, res, "Player next to big blind should be under the gun on preflop");
        }
        [TestMethod]
        public void Game4PUtgIsNextToDealerOnFlop()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds), new NbPlayersModule(4)).AfterPreflop();

            //Act
            var res = nfo.PlayerNextTo(nfo.Dealer);

            //Assert
            Assert.AreEqual(nfo.CurrentPlayer, res, "Player next to dealer should be under the gun on flop");
        }
        [TestMethod]
        public void Game4PAntesUtgIsNextToDealerOnPreflop()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Antes), new NbPlayersModule(4)).BlindsPosted();

            //Act
            var res = nfo.PlayerNextTo(nfo.Dealer);

            //Assert
            Assert.AreEqual(nfo.CurrentPlayer, res, "Player next to dealer should be under the gun on preflop");
        }
        [TestMethod]
        public void Game4PAntesUtgIsNextToDealerOnFlop()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Antes), new NbPlayersModule(4)).AfterPreflop();

            //Act
            var res = nfo.PlayerNextTo(nfo.Dealer);

            //Assert
            Assert.AreEqual(nfo.CurrentPlayer, res, "Player next to dealer should be under the gun on flop");
        }
    }
}
