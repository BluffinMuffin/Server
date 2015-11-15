using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Protocol.DataTypes.Options;
using BluffinMuffin.Server.Logic.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BluffinMuffin.Server.Logic.Test
{
    [TestClass]
    public class TableParamTests
    {
        [TestMethod]
        public void BuyInUnderMinimumIsInvalid()
        {
            //Arrange (Minimum 20*GameSize=200, Maximum 100*GameSize-1999) 
            var t = new TableParams { GameSize = 10, Lobby = new LobbyOptionsRegisteredMode { IsMaximumBuyInLimited = true } };

            //Act
            var res = t.IsValidBuyIn(199);

            //Assert
            Assert.AreEqual(false, res);
        }
        [TestMethod]
        public void BuyInEqualMinimumIsValid()
        {
            //Arrange (Minimum 20*GameSize=200, Maximum 100*GameSize-1999) 
            var t = new TableParams { GameSize = 10, Lobby = new LobbyOptionsRegisteredMode { IsMaximumBuyInLimited = true } };

            //Act
            var res = t.IsValidBuyIn(200);

            //Assert
            Assert.AreEqual(true, res);
        }
        [TestMethod]
        public void BuyInBetweenMinimumAndMaximumIsValid()
        {
            //Arrange (Minimum 20*GameSize=200, Maximum 100*GameSize-1999) 
            var t = new TableParams { GameSize = 10, Lobby = new LobbyOptionsRegisteredMode { IsMaximumBuyInLimited = true } };

            //Act
            var res = t.IsValidBuyIn(500);

            //Assert
            Assert.AreEqual(true, res);
        }
        [TestMethod]
        public void BuyInEqualMaximumIsValid()
        {
            //Arrange (Minimum 20*GameSize=200, Maximum 100*GameSize-1999) 
            var t = new TableParams { GameSize = 10, Lobby = new LobbyOptionsRegisteredMode { IsMaximumBuyInLimited = true } };

            //Act
            var res = t.IsValidBuyIn(1000);

            //Assert
            Assert.AreEqual(true, res);
        }
        [TestMethod]
        public void BuyInOverMaximumIsInvalid()
        {
            //Arrange (Minimum 20*GameSize=200, Maximum 100*GameSize-1999) 
            var t = new TableParams { GameSize = 10, Lobby = new LobbyOptionsRegisteredMode { IsMaximumBuyInLimited = true } };

            //Act
            var res = t.IsValidBuyIn(1001);

            //Assert
            Assert.AreEqual(false, res);
        }
        [TestMethod]
        public void BigBlindEqualsGameSize()
        {
            //Arrange 
            var t = new TableParams { GameSize = 100 };

            //Act
            var res = t.BigBlindAmount();

            //Assert
            Assert.AreEqual(t.GameSize, res);
        }
        [TestMethod]
        public void SmallBlindEqualsHalfOfGameSize()
        {
            //Arrange 
            var t = new TableParams { GameSize = 100 };

            //Act
            var res = t.SmallBlindAmount();

            //Assert
            Assert.AreEqual(t.GameSize / 2, res);
        }
        [TestMethod]
        public void AnteEqualsATenthOfGameSize()
        {
            //Arrange 
            var t = new TableParams { GameSize = 100 };

            //Act
            var res = t.AnteAmount();

            //Assert
            Assert.AreEqual(t.GameSize / 10, res);
        }
        [TestMethod]
        public void BetEqualsGameSize()
        {
            //Arrange 
            var t = new TableParams { GameSize = 100 };

            //Act
            var res = t.BetAmount();

            //Assert
            Assert.AreEqual(t.GameSize, res);
        }
        [TestMethod]
        public void HalfBetEqualsHalfOfGameSize()
        {
            //Arrange 
            var t = new TableParams { GameSize = 100 };

            //Act
            var res = t.HalfBetAmount();

            //Assert
            Assert.AreEqual(t.GameSize / 2, res);
        }
        [TestMethod]
        public void MinimumOfTwoForBigBlind()
        {
            //Arrange 
            var t = new TableParams { GameSize = 1 };

            //Act
            var res = t.BigBlindAmount();

            //Assert
            Assert.AreEqual(2, res);
        }
        [TestMethod]
        public void MinimumOfTwoForBet()
        {
            //Arrange 
            var t = new TableParams { GameSize = 1 };

            //Act
            var res = t.BetAmount();

            //Assert
            Assert.AreEqual(2, res);
        }
        [TestMethod]
        public void MinimumOfOneForAnte()
        {
            //Arrange 
            var t = new TableParams { GameSize = 5 };

            //Act
            var res = t.AnteAmount();

            //Assert
            Assert.AreEqual(1, res);
        }
        [TestMethod]
        public void MinimumOfOneForSmallBlind()
        {
            //Arrange 
            var t = new TableParams { GameSize = 1 };

            //Act
            var res = t.SmallBlindAmount();

            //Assert
            Assert.AreEqual(1, res);
        }
        [TestMethod]
        public void MinimumOfOneForHalfBet()
        {
            //Arrange 
            var t = new TableParams { GameSize = 1 };

            //Act
            var res = t.HalfBetAmount();

            //Assert
            Assert.AreEqual(1, res);
        }
    }
}
