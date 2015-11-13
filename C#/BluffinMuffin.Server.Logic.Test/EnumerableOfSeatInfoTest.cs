using System.Linq;
using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Protocol.DataTypes.Enums;
using BluffinMuffin.Server.Logic.Extensions;
using BluffinMuffin.Server.Logic.Test.PokerGameTests.DataTypes;
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
            var seats = new[] { sDealer, sFirstTalker, sCurrentPlayer, sBigBlind };

            //Act
            var res = seats.SeatOfDealer();

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
            var seats = new[] { sDealer, sFirstTalker, sCurrentPlayer, sBigBlind };

            //Act
            var res = seats.SeatOfFirstTalker();

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
            var seats = new[] { sDealer, sFirstTalker, sCurrentPlayer, sBigBlind };

            //Act
            var res = seats.SeatOfCurrentPlayer();

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
            var seats = new[] { sDealer, sFirstTalker, sCurrentPlayer, sBigBlind };

            //Act
            var res = seats.WithAttribute(SeatAttributeEnum.BigBlind).ToArray();

            //Assert
            Assert.AreEqual(2, res.Length);
            Assert.IsTrue(res.Contains(sCurrentPlayer));
            Assert.IsTrue(res.Contains(sBigBlind));
        }
        [TestMethod]
        public void NextPlayerIsThePlayingPlayerAfterMeIfAskedWithMe()
        {
            //Arrange
            var me = new SeatInfo { Player = new PlayerInfo() { State = PlayerStateEnum.Playing}, NoSeat = 1 };
            var p1 = new SeatInfo { Player = new PlayerInfo() { State = PlayerStateEnum.Joined }, NoSeat = 2 };
            var p2 = new SeatInfo { Player = new PlayerInfo() { State = PlayerStateEnum.Playing }, NoSeat = 3 };
            var p3 = new SeatInfo { Player = new PlayerInfo() { State = PlayerStateEnum.Joined }, NoSeat = 4 };
            var p4 = new SeatInfo { Player = new PlayerInfo() { State = PlayerStateEnum.Playing }, NoSeat = 5 };
            var p5 = new SeatInfo { Player = new PlayerInfo() { State = PlayerStateEnum.Joined }, NoSeat = 6 };
            var seats = new[] { me, p1, p2, p3, p4, p5 };

            //Act
            var res = seats.SeatOfPlayingPlayerNextTo(me);

            //Assert
            Assert.AreEqual(p2, res);
        }
        [TestMethod]
        public void PreviousPlayerIsThePlayingPlayerBeforeMeIfAskedWithMe()
        {
            //Arrange
            var me = new SeatInfo { Player = new PlayerInfo() { State = PlayerStateEnum.Playing }, NoSeat = 1 };
            var p1 = new SeatInfo { Player = new PlayerInfo() { State = PlayerStateEnum.Joined }, NoSeat = 2 };
            var p2 = new SeatInfo { Player = new PlayerInfo() { State = PlayerStateEnum.Playing }, NoSeat = 3 };
            var p3 = new SeatInfo { Player = new PlayerInfo() { State = PlayerStateEnum.Joined }, NoSeat = 4 };
            var p4 = new SeatInfo { Player = new PlayerInfo() { State = PlayerStateEnum.Playing }, NoSeat = 5 };
            var p5 = new SeatInfo { Player = new PlayerInfo() { State = PlayerStateEnum.Joined }, NoSeat = 6 };
            var seats = new[] { me, p1, p2, p3, p4, p5 };

            //Act
            var res = seats.SeatOfPlayingPlayerJustBefore(me);

            //Assert
            Assert.AreEqual(p4, res);
        }
        [TestMethod]
        public void NextPlayerIsMeIfNoOnePlayingButMeAskedWithMe()
        {
            //Arrange
            var me = new SeatInfo { Player = new PlayerInfo() { State = PlayerStateEnum.Playing }, NoSeat = 1 };
            var p1 = new SeatInfo { Player = new PlayerInfo() { State = PlayerStateEnum.Joined }, NoSeat = 2 };
            var p2 = new SeatInfo { Player = new PlayerInfo() { State = PlayerStateEnum.Joined }, NoSeat = 3 };
            var p3 = new SeatInfo { Player = new PlayerInfo() { State = PlayerStateEnum.Joined }, NoSeat = 4 };
            var seats = new[] { me, p1, p2, p3 };

            //Act
            var res = seats.SeatOfPlayingPlayerNextTo(me);

            //Assert
            Assert.AreEqual(me, res);
        }
        [TestMethod]
        public void NextPlayerIsMeIfNoOnePlayingButMeAskedWithNull()
        {
            //Arrange
            var me = new SeatInfo { Player = new PlayerInfo() { State = PlayerStateEnum.Playing }, NoSeat = 1 };
            var p1 = new SeatInfo { Player = new PlayerInfo() { State = PlayerStateEnum.Joined }, NoSeat = 2 };
            var p2 = new SeatInfo { Player = new PlayerInfo() { State = PlayerStateEnum.Joined }, NoSeat = 3 };
            var p3 = new SeatInfo { Player = new PlayerInfo() { State = PlayerStateEnum.Joined }, NoSeat = 4 };
            var seats = new[] { me, p1, p2, p3 };

            //Act
            var res = seats.SeatOfPlayingPlayerNextTo(null);

            //Assert
            Assert.AreEqual(me, res);
        }
        [TestMethod]
        public void NextPlayerIsNullIfNoOnePlayingAskedWithMe()
        {
            //Arrange
            var me = new SeatInfo { Player = new PlayerInfo() { State = PlayerStateEnum.Joined }, NoSeat = 1 };
            var p1 = new SeatInfo { Player = new PlayerInfo() { State = PlayerStateEnum.Joined }, NoSeat = 2 };
            var p2 = new SeatInfo { Player = new PlayerInfo() { State = PlayerStateEnum.Joined }, NoSeat = 3 };
            var p3 = new SeatInfo { Player = new PlayerInfo() { State = PlayerStateEnum.Joined }, NoSeat = 4 };
            var seats = new[] { me, p1, p2, p3 };

            //Act
            var res = seats.SeatOfPlayingPlayerNextTo(me);

            //Assert
            Assert.IsNull(res);
        }
        [TestMethod]
        public void NextPlayerIsNullIfNoOnePlayingAskedWithNull()
        {
            //Arrange
            var seats = new SeatInfo[] { };

            //Act
            var res = seats.SeatOfPlayingPlayerNextTo(null);

            //Assert
            Assert.IsNull(res);
        }
        [TestMethod]
        public void NextPlayerIsNullIfNobodyAskedWithMePlaying()
        {
            //Arrange
            var me = new SeatInfo { Player = new PlayerInfo() { State = PlayerStateEnum.Playing } };
            var seats = new SeatInfo[] { };

            //Act
            var res = seats.SeatOfPlayingPlayerNextTo(me);

            //Assert
            Assert.IsNull(res);
        }
        [TestMethod]
        public void NextPlayerIsNullIfNobodyAskedWithMeNotPlaying()
        {
            //Arrange
            var me = new SeatInfo { Player = new PlayerInfo() { State = PlayerStateEnum.Joined } };
            var seats = new SeatInfo[] { };

            //Act
            var res = seats.SeatOfPlayingPlayerNextTo(me);

            //Assert
            Assert.IsNull(res);
        }
        [TestMethod]
        public void NextPlayerIsNullIfNobodyAskedWithNull()
        {
            //Arrange
            var seats = new SeatInfo[] { };

            //Act
            var res = seats.SeatOfPlayingPlayerNextTo(null);

            //Assert
            Assert.IsNull(res);
        }
        [TestMethod]
        public void CanListAllPlayingPlayers()
        {
            //Arrange
            var me = new SeatInfo { Player = new PlayerInfo() { State = PlayerStateEnum.Playing }, NoSeat = 1 };
            var p1 = new SeatInfo { Player = new PlayerInfo() { State = PlayerStateEnum.Joined }, NoSeat = 2 };
            var p2 = new SeatInfo { Player = new PlayerInfo() { State = PlayerStateEnum.Playing }, NoSeat = 3 };
            var p3 = new SeatInfo { Player = new PlayerInfo() { State = PlayerStateEnum.AllIn }, NoSeat = 4 };
            var p4 = new SeatInfo { Player = new PlayerInfo() { State = PlayerStateEnum.Playing }, NoSeat = 5 };
            var p5 = new SeatInfo { Player = new PlayerInfo() { State = PlayerStateEnum.Joined }, NoSeat = 6 };
            var seats = new[] { me, p1, p2, p3, p4, p5 };

            //Act
            var res = seats.PlayingPlayers().ToList();

            //Assert
            Assert.AreEqual(3, res.Count);
            Assert.IsTrue(res.Contains(me.Player));
            Assert.IsTrue(res.Contains(p2.Player));
            Assert.IsTrue(res.Contains(p4.Player));
        }
        [TestMethod]
        public void CanListAllPlayingAndAllInPlayers()
        {
            //Arrange
            var me = new SeatInfo { Player = new PlayerInfo() { State = PlayerStateEnum.Playing }, NoSeat = 1 };
            var p1 = new SeatInfo { Player = new PlayerInfo() { State = PlayerStateEnum.Joined }, NoSeat = 2 };
            var p2 = new SeatInfo { Player = new PlayerInfo() { State = PlayerStateEnum.Playing }, NoSeat = 3 };
            var p3 = new SeatInfo { Player = new PlayerInfo() { State = PlayerStateEnum.AllIn }, NoSeat = 4 };
            var p4 = new SeatInfo { Player = new PlayerInfo() { State = PlayerStateEnum.Playing }, NoSeat = 5 };
            var p5 = new SeatInfo { Player = new PlayerInfo() { State = PlayerStateEnum.Joined }, NoSeat = 6 };
            var seats = new[] { me, p1, p2, p3, p4, p5 };

            //Act
            var res = seats.PlayingAndAllInPlayers().ToList();

            //Assert
            Assert.AreEqual(4, res.Count);
            Assert.IsTrue(res.Contains(me.Player));
            Assert.IsTrue(res.Contains(p2.Player));
            Assert.IsTrue(res.Contains(p3.Player));
            Assert.IsTrue(res.Contains(p4.Player));
        }
        [TestMethod]
        public void CanListAllRemainingSeatIds()
        {
            //Arrange
            var s0 = new SeatInfo { Player = new PlayerInfo(), NoSeat = 0 };
            var s1 = new SeatInfo { NoSeat = 1 };
            var s2 = new SeatInfo { Player = new PlayerInfo(), NoSeat = 2 };
            var s3 = new SeatInfo { NoSeat = 3 };
            var s4 = new SeatInfo { Player = new PlayerInfo(), NoSeat = 4 };
            var s5 = new SeatInfo { NoSeat = 5 };
            var seats = new[] { s0, s1, s2, s3, s4, s5 };

            //Act
            var res = seats.RemainingSeatIds().ToList();

            //Assert
            Assert.AreEqual(3, res.Count);
            Assert.IsTrue(res.Contains(s1.NoSeat));
            Assert.IsTrue(res.Contains(s3.NoSeat));
            Assert.IsTrue(res.Contains(s5.NoSeat));
        }

    }
}
