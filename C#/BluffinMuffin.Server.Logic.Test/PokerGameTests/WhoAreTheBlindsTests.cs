using System.Linq;
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
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Antes)).WithAllPlayersSeated();

            Assert.AreEqual(2, nfo.Players.Count(x => nfo.BlindNeeded(x) > 0), "Dealer should be the small blind");
        }
        [TestMethod]
        public void AnteGame3PEverybodyIsBlind()
        {
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Antes), new NbPlayersModule(3)).WithAllPlayersSeated();

            Assert.AreEqual(3, nfo.Players.Count(x => nfo.BlindNeeded(x) > 0), "Dealer should be the small blind");
        }
        [TestMethod]
        public void AnteGame4PEverybodyIsBlind()
        {
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Antes), new NbPlayersModule(4)).WithAllPlayersSeated();

            Assert.AreEqual(4, nfo.Players.Count(x => nfo.BlindNeeded(x) > 0), "Dealer should be the small blind");
        }
        [TestMethod]
        public void Game2PSmallIsDealer()
        {
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).WithAllPlayersSeated();

            Assert.AreEqual(nfo.CalculatedSmallBlind, nfo.Dealer, "Dealer should be the small blind");
        }
        [TestMethod]
        public void Game2PBigIsNextToDealer()
        {
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).WithAllPlayersSeated();

            Assert.AreEqual(nfo.CalculatedBigBlind, nfo.PlayerNextTo(nfo.Dealer), "Player Next To Dealer should be the big blind");
        }
        [TestMethod]
        public void Game3PSmallIsNextToDealer()
        {
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds), new NbPlayersModule(3)).WithAllPlayersSeated();

            Assert.AreEqual(nfo.CalculatedSmallBlind, nfo.PlayerNextTo(nfo.Dealer), "Player Next To Dealer should be the small blind");
        }
        [TestMethod]
        public void Game3PBigIsNextToSmall()
        {
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds), new NbPlayersModule(3)).WithAllPlayersSeated();

            Assert.AreEqual(nfo.CalculatedBigBlind, nfo.PlayerNextTo(nfo.CalculatedSmallBlind), "Player Next To CalculatedSmallBlind should be the big blind");
        }
        [TestMethod]
        public void Game4PSmallIsNextToDealer()
        {
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds), new NbPlayersModule(4)).WithAllPlayersSeated();

            Assert.AreEqual(nfo.CalculatedSmallBlind, nfo.PlayerNextTo(nfo.Dealer), "Player Next To Dealer should be the small blind");
        }
        [TestMethod]
        public void Game4PBigIsNextToSmall()
        {
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds), new NbPlayersModule(4)).WithAllPlayersSeated();

            Assert.AreEqual(nfo.CalculatedBigBlind, nfo.PlayerNextTo(nfo.CalculatedSmallBlind), "Player Next To CalculatedSmallBlind should be the big blind");
        }
    }
}
