using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Protocol.DataTypes.Enums;
using BluffinMuffin.Server.Logic.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BluffinMuffin.Server.Logic.Test
{
    [TestClass]
    public class SeatInfoTests
    {
        [TestMethod]
        public void EmptySeatDoesntHavePlayerPlaying()
        {
            //Arrange
            var s = new SeatInfo();

            //Act
            var res = s.HasPlayerPlaying();

            //Assert
            Assert.AreEqual(false, res);
        }
        [TestMethod]
        public void SeatWithAllInPlayerDoesntHavePlayerPlaying()
        {
            //Arrange
            var s = new SeatInfo { Player = new PlayerInfo { State = PlayerStateEnum.AllIn } };

            //Act
            var res = s.HasPlayerPlaying();

            //Assert
            Assert.AreEqual(false, res);
        }
        [TestMethod]
        public void SeatWithSitInPlayerDoesntHavePlayerPlaying()
        {
            //Arrange
            var s = new SeatInfo { Player = new PlayerInfo { State = PlayerStateEnum.SitIn } };

            //Act
            var res = s.HasPlayerPlaying();

            //Assert
            Assert.AreEqual(false, res);
        }
        [TestMethod]
        public void SeatWithJoinedPlayerDoesntHavePlayerPlaying()
        {
            //Arrange
            var s = new SeatInfo { Player = new PlayerInfo { State = PlayerStateEnum.Joined } };

            //Act
            var res = s.HasPlayerPlaying();

            //Assert
            Assert.AreEqual(false, res);
        }
        [TestMethod]
        public void SeatWithZombiePlayerDoesntHavePlayerPlaying()
        {
            //Arrange
            var s = new SeatInfo { Player = new PlayerInfo { State = PlayerStateEnum.Zombie } };

            //Act
            var res = s.HasPlayerPlaying();

            //Assert
            Assert.AreEqual(false, res);
        }
        [TestMethod]
        public void SeatWithPlayingPlayerHasPlayerPlaying()
        {
            //Arrange
            var s = new SeatInfo { Player = new PlayerInfo { State = PlayerStateEnum.Playing } };

            //Act
            var res = s.HasPlayerPlaying();

            //Assert
            Assert.AreEqual(true, res);
        }
        [TestMethod]
        public void EmptySeatDoesntHavePlayerPlayingOrAllIn()
        {
            //Arrange
            var s = new SeatInfo();

            //Act
            var res = s.HasPlayerPlayingOrAllIn();

            //Assert
            Assert.AreEqual(false, res);
        }
        [TestMethod]
        public void SeatWithAllInPlayerDoesntHavePlayerPlayingOrAllIn()
        {
            //Arrange
            var s = new SeatInfo { Player = new PlayerInfo { State = PlayerStateEnum.AllIn } };

            //Act
            var res = s.HasPlayerPlayingOrAllIn();

            //Assert
            Assert.AreEqual(true, res);
        }
        [TestMethod]
        public void SeatWithSitInPlayerDoesntHavePlayerPlayingOrAllIn()
        {
            //Arrange
            var s = new SeatInfo { Player = new PlayerInfo { State = PlayerStateEnum.SitIn } };

            //Act
            var res = s.HasPlayerPlayingOrAllIn();

            //Assert
            Assert.AreEqual(false, res);
        }
        [TestMethod]
        public void SeatWithJoinedPlayerDoesntHavePlayerPlayingOrAllIn()
        {
            //Arrange
            var s = new SeatInfo { Player = new PlayerInfo { State = PlayerStateEnum.Joined } };

            //Act
            var res = s.HasPlayerPlayingOrAllIn();

            //Assert
            Assert.AreEqual(false, res);
        }
        [TestMethod]
        public void SeatWithZombiePlayerDoesntHavePlayerPlayingOrAllIn()
        {
            //Arrange
            var s = new SeatInfo { Player = new PlayerInfo { State = PlayerStateEnum.Zombie } };

            //Act
            var res = s.HasPlayerPlayingOrAllIn();

            //Assert
            Assert.AreEqual(false, res);
        }
        [TestMethod]
        public void SeatWithPlayingPlayerHasPlayerPlayingOrAllIn()
        {
            //Arrange
            var s = new SeatInfo { Player = new PlayerInfo { State = PlayerStateEnum.Playing } };

            //Act
            var res = s.HasPlayerPlayingOrAllIn();

            //Assert
            Assert.AreEqual(true, res);
        }
    }
}
