using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Protocol.DataTypes.Enums;
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
    }
}
