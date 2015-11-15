using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Server.Logic.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BluffinMuffin.Server.Logic.Test
{
    [TestClass]
    public class EnumerableOfPlayerInfoTest
    {
        [TestMethod]
        public void ContainsSomeoneNamedSpongeBob()
        {
            //Arrange
            const string NAME = "Spongebob Squarepants";
            PlayerInfo p = new PlayerInfo(NAME, 4242);
            PlayerInfo[] players = { p };

            //Act
            var res = players.ContainsPlayerNamed(NAME);

            //Assert
            Assert.IsTrue(res);
        }
        [TestMethod]
        public void ContainsSomeoneNamedSpongeBobCaseUnsensitive()
        {
            //Arrange
            PlayerInfo p = new PlayerInfo("Spongebob Squarepants", 4242);
            PlayerInfo[] players = { p };

            //Act
            var res = players.ContainsPlayerNamed("SpOnGeBoB SqUaRePaNtS");

            //Assert
            Assert.IsTrue(res);
        }
        [TestMethod]
        public void DoesNotContainSomeoneNamedDora()
        {
            //Arrange
            PlayerInfo p = new PlayerInfo("Spongebob Squarepants", 4242);
            PlayerInfo[] players = { p };

            //Act
            var res = players.ContainsPlayerNamed("Dora the Explorer");

            //Assert
            Assert.IsFalse(res);
        }
        [TestMethod]
        public void AlreadyContainsThePlayer()
        {
            //Arrange
            PlayerInfo p = new PlayerInfo("Spongebob Squarepants", 4242);
            PlayerInfo[] players = { p };

            //Act
            var res = players.ContainsPlayerWithSameName(p);

            //Assert
            Assert.IsTrue(res);
        }
        [TestMethod]
        public void AlreadyContainsSomeoneWithExactSameName()
        {
            //Arrange
            const string NAME = "Spongebob Squarepants";
            PlayerInfo p = new PlayerInfo(NAME, 4242);
            PlayerInfo[] players = { p };

            //Act
            var res = players.ContainsPlayerWithSameName(new PlayerInfo(NAME, 5000));

            //Assert
            Assert.IsTrue(res);
        }
        [TestMethod]
        public void AlreadyContainsSomeoneWithSameNameCaseUnsensitive()
        {
            //Arrange
            PlayerInfo p = new PlayerInfo("Spongebob Squarepants", 4242);
            PlayerInfo[] players = { p };

            //Act
            var res = players.ContainsPlayerWithSameName(new PlayerInfo("SpOnGeBoB SqUaRePaNtS", 5000));

            //Assert
            Assert.IsTrue(res);
        }
        [TestMethod]
        public void DoesNotContainSomeoneWithSameNameCaseUnsensitive()
        {
            //Arrange
            PlayerInfo p = new PlayerInfo("Spongebob Squarepants", 4242);
            PlayerInfo[] players = { p };

            //Act
            var res = players.ContainsPlayerWithSameName(new PlayerInfo("Dora the Explorer", 5000));

            //Assert
            Assert.IsFalse(res);
        }

    }
}
