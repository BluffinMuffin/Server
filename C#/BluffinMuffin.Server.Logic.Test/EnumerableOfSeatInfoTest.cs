using System.Linq;
using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Protocol.DataTypes.Enums;
using BluffinMuffin.Server.Logic.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BluffinMuffin.Server.Logic.Test
{
    [TestClass]
    public class EnumerableOfSeatInfoTest
    {
        [TestMethod]
        public void CanFindDealer()
        {
            //Arrange
            var sDealer = new SeatInfo { SeatAttributes = new[] { SeatAttributeEnum.Dealer } };
            var sFirstTalker = new SeatInfo { SeatAttributes = new[] { SeatAttributeEnum.FirstTalker } };
            var sCurrentPlayer = new SeatInfo { SeatAttributes = new[] { SeatAttributeEnum.CurrentPlayer, SeatAttributeEnum.BigBlind, } };
            var sBigBlind = new SeatInfo { SeatAttributes = new[] { SeatAttributeEnum.BigBlind } };
            var seats = new SeatInfo[] { sDealer, sFirstTalker, sCurrentPlayer, sBigBlind };

            //Act
            var res = seats.Dealer();

            //Assert
            Assert.AreEqual(sDealer, res);
        }
        [TestMethod]
        public void CanFindFirstTalker()
        {
            //Arrange
            var sDealer = new SeatInfo { SeatAttributes = new[] { SeatAttributeEnum.Dealer } };
            var sFirstTalker = new SeatInfo { SeatAttributes = new[] { SeatAttributeEnum.FirstTalker } };
            var sCurrentPlayer = new SeatInfo { SeatAttributes = new[] { SeatAttributeEnum.CurrentPlayer, SeatAttributeEnum.BigBlind, } };
            var sBigBlind = new SeatInfo { SeatAttributes = new[] { SeatAttributeEnum.BigBlind } };
            var seats = new SeatInfo[] { sDealer, sFirstTalker, sCurrentPlayer, sBigBlind };

            //Act
            var res = seats.FirstTalker();

            //Assert
            Assert.AreEqual(sFirstTalker, res);
        }
        [TestMethod]
        public void CanFindCurrentPlayer()
        {
            //Arrange
            var sDealer = new SeatInfo { SeatAttributes = new[] { SeatAttributeEnum.Dealer } };
            var sFirstTalker = new SeatInfo { SeatAttributes = new[] { SeatAttributeEnum.FirstTalker } };
            var sCurrentPlayer = new SeatInfo { SeatAttributes = new[] { SeatAttributeEnum.CurrentPlayer, SeatAttributeEnum.BigBlind, } };
            var sBigBlind = new SeatInfo { SeatAttributes = new[] { SeatAttributeEnum.BigBlind } };
            var seats = new SeatInfo[] { sDealer, sFirstTalker, sCurrentPlayer, sBigBlind };

            //Act
            var res = seats.CurrentPlayer();

            //Assert
            Assert.AreEqual(sCurrentPlayer, res);
        }
        [TestMethod]
        public void CanFindBigBlinds()
        {
            //Arrange
            var sDealer = new SeatInfo { SeatAttributes = new[] { SeatAttributeEnum.Dealer } };
            var sFirstTalker = new SeatInfo { SeatAttributes = new[] { SeatAttributeEnum.FirstTalker } };
            var sCurrentPlayer = new SeatInfo { SeatAttributes = new[] { SeatAttributeEnum.CurrentPlayer, SeatAttributeEnum.BigBlind, } };
            var sBigBlind = new SeatInfo { SeatAttributes = new[] { SeatAttributeEnum.BigBlind } };
            var seats = new SeatInfo[] { sDealer, sFirstTalker, sCurrentPlayer, sBigBlind };

            //Act
            var res = seats.WithAttribute(SeatAttributeEnum.BigBlind).ToArray();

            //Assert
            Assert.AreEqual(2, res.Length);
            Assert.IsTrue(res.Contains(sCurrentPlayer));
            Assert.IsTrue(res.Contains(sBigBlind));
        }

    }
}
