using System.Linq;
using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Protocol.DataTypes.Enums;
using BluffinMuffin.Server.Logic.Test.PokerGameTests.DataTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BluffinMuffin.Server.Logic.Test.PokerGameTests
{
    [TestClass]
    public class WhoAreTheBlindsTests
    {
        [TestMethod]
        public void AnteGame2PEverybodyIsBlind()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Antes)).WithAllPlayersSeated();

            //Act
            var res = nfo.Players.Count(x => nfo.BlindNeeded(x) > 0);

            //Assert
            Assert.AreEqual(2, res, "Dealer should be the small blind");
        }
        [TestMethod]
        public void AnteGame3PEverybodyIsBlind()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Antes), new NbPlayersModule(3)).WithAllPlayersSeated();

            //Act
            var res = nfo.Players.Count(x => nfo.BlindNeeded(x) > 0);

            //Assert
            Assert.AreEqual(3, res, "Dealer should be the small blind");
        }
        [TestMethod]
        public void AnteGame4PEverybodyIsBlind()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Antes), new NbPlayersModule(4)).WithAllPlayersSeated();

            //Act
            var res = nfo.Players.Count(x => nfo.BlindNeeded(x) > 0);

            //Assert
            Assert.AreEqual(4, res, "Dealer should be the small blind");
        }
        [TestMethod]
        public void Game2PSmallIsDealer()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).WithAllPlayersSeated();

            //Act
            var res = nfo.Dealer;

            //Assert
            Assert.AreEqual(nfo.CalculatedSmallBlind, res, "Dealer should be the small blind");
        }
        [TestMethod]
        public void Game2PBigIsNextToDealer()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).WithAllPlayersSeated();

            //Act
            var res = nfo.PlayerNextTo(nfo.Dealer);

            //Assert
            Assert.AreEqual(nfo.CalculatedBigBlind, res, "Player Next To Dealer should be the big blind");
        }
        [TestMethod]
        public void Game3PSmallIsNextToDealer()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds), new NbPlayersModule(3)).WithAllPlayersSeated();

            //Act
            var res = nfo.PlayerNextTo(nfo.Dealer);

            //Assert
            Assert.AreEqual(nfo.CalculatedSmallBlind, res, "Player Next To Dealer should be the small blind");
        }
        [TestMethod]
        public void Game3PBigIsNextToSmall()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds), new NbPlayersModule(3)).WithAllPlayersSeated();

            //Act
            var res = nfo.PlayerNextTo(nfo.CalculatedSmallBlind);

            //Assert
            Assert.AreEqual(nfo.CalculatedBigBlind, res, "Player Next To CalculatedSmallBlind should be the big blind");
        }
        [TestMethod]
        public void Game4PSmallIsNextToDealer()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds), new NbPlayersModule(4)).WithAllPlayersSeated();

            //Act
            var res = nfo.PlayerNextTo(nfo.Dealer);

            //Assert
            Assert.AreEqual(nfo.CalculatedSmallBlind, res, "Player Next To Dealer should be the small blind");
        }
        [TestMethod]
        public void Game4PBigIsNextToSmall()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds), new NbPlayersModule(4)).WithAllPlayersSeated();

            //Act
            var res = nfo.PlayerNextTo(nfo.CalculatedSmallBlind);

            //Assert
            Assert.AreEqual(nfo.CalculatedBigBlind, res, "Player Next To CalculatedSmallBlind should be the big blind");
        }
    }
}
