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
        public void EmptySeatDoesntHavePlayerAllIn()
        {
            //Arrange
            var s = new SeatInfo();

            //Act
            var res = s.HasPlayerAllIn();

            //Assert
            Assert.AreEqual(false, res);
        }
        [TestMethod]
        public void SeatWithPlayingPlayerDoesntHavePlayerAllIn()
        {
            //Arrange
            var s = new SeatInfo { Player = new PlayerInfo { State = PlayerStateEnum.Playing } };

            //Act
            var res = s.HasPlayerAllIn();

            //Assert
            Assert.AreEqual(false, res);
        }
        [TestMethod]
        public void SeatWithSitInPlayerDoesntHavePlayerAllIn()
        {
            //Arrange
            var s = new SeatInfo { Player = new PlayerInfo { State = PlayerStateEnum.SitIn } };

            //Act
            var res = s.HasPlayerAllIn();

            //Assert
            Assert.AreEqual(false, res);
        }
        [TestMethod]
        public void SeatWithJoinedPlayerDoesntHavePlayerAllIn()
        {
            //Arrange
            var s = new SeatInfo { Player = new PlayerInfo { State = PlayerStateEnum.Joined } };

            //Act
            var res = s.HasPlayerAllIn();

            //Assert
            Assert.AreEqual(false, res);
        }
        [TestMethod]
        public void SeatWithZombiePlayerDoesntHavePlayerAllIn()
        {
            //Arrange
            var s = new SeatInfo { Player = new PlayerInfo { State = PlayerStateEnum.Zombie } };

            //Act
            var res = s.HasPlayerAllIn();

            //Assert
            Assert.AreEqual(false, res);
        }
        [TestMethod]
        public void SeatWithAllInPlayerHasPlayerAllIn()
        {
            //Arrange
            var s = new SeatInfo { Player = new PlayerInfo { State = PlayerStateEnum.AllIn } };

            //Act
            var res = s.HasPlayerAllIn();

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

        [TestMethod]
        public void SeatContainingAttribute()
        {
            //Arrange
            var s = new SeatInfo { SeatAttributes = new[] { SeatAttributeEnum.Dealer } };

            //Act

            //Assert
            Assert.IsTrue(s.HasAttribute(SeatAttributeEnum.Dealer));
        }

        [TestMethod]
        public void SeatNotContainingAttribute()
        {
            //Arrange
            var s = new SeatInfo { SeatAttributes = new[] { SeatAttributeEnum.Dealer } };

            //Act

            //Assert
            Assert.IsFalse(s.HasAttribute(SeatAttributeEnum.FirstTalker));
        }


        [TestMethod]
        public void AddAttributeThatWasNotThereShouldAdd()
        {
            //Arrange
            var s = new SeatInfo { SeatAttributes = new[] { SeatAttributeEnum.Dealer } };

            //Act
            s.AddAttribute(SeatAttributeEnum.FirstTalker);

            //Assert
            Assert.AreEqual(2, s.SeatAttributes.Length);
            Assert.IsTrue(s.HasAttribute(SeatAttributeEnum.Dealer));
            Assert.IsTrue(s.HasAttribute(SeatAttributeEnum.FirstTalker));
        }

        [TestMethod]
        public void AddAttributeThatWasAlreadyThereShouldDoNothing()
        {
            //Arrange
            var s = new SeatInfo { SeatAttributes = new[] { SeatAttributeEnum.Dealer } };

            //Act
            s.AddAttribute(SeatAttributeEnum.Dealer);

            //Assert
            Assert.AreEqual(1, s.SeatAttributes.Length);
            Assert.IsTrue(s.HasAttribute(SeatAttributeEnum.Dealer));
        }


        [TestMethod]
        public void RemoveAttributeThatWasNotThereShouldDoNothing()
        {
            //Arrange
            var s = new SeatInfo { SeatAttributes = new[] { SeatAttributeEnum.Dealer } };

            //Act
            s.RemoveAttribute(SeatAttributeEnum.FirstTalker);

            //Assert
            Assert.AreEqual(1, s.SeatAttributes.Length);
            Assert.IsFalse(s.HasAttribute(SeatAttributeEnum.FirstTalker));
            Assert.IsTrue(s.HasAttribute(SeatAttributeEnum.Dealer));
        }

        [TestMethod]
        public void RemoveAttributeThatWasAlreadyThereShouldRemove()
        {
            //Arrange
            var s = new SeatInfo { SeatAttributes = new[] { SeatAttributeEnum.Dealer } };

            //Act
            s.RemoveAttribute(SeatAttributeEnum.Dealer);

            //Assert
            Assert.AreEqual(0, s.SeatAttributes.Length);
            Assert.IsFalse(s.HasAttribute(SeatAttributeEnum.Dealer));
        }

    }
}
