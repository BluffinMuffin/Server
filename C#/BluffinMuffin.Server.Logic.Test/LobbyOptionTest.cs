using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Protocol.DataTypes.Enums;
using BluffinMuffin.Protocol.DataTypes.Options;
using BluffinMuffin.Server.Logic.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BluffinMuffin.Server.Logic.Test
{
    [TestClass]
    public class LobbyOptionTest
    {

        [TestMethod]
        public void UnlimitedMinimumBuyInMeansZero()
        {
            //Arrange
            var lo = new LobbyOptionStub
            {
                MinimumBuyInParameterMock = BuyInParameterEnum.Unlimited,
                MinimumBuyInValueMock = 42
            };
            var gameSize = 10;

            //Act
            var res = lo.MinimumBuyInAmount(gameSize);

            //Assert
            Assert.AreEqual(0, res);
        }

        [TestMethod]
        public void FixedMinimumBuyInMeansValue()
        {
            //Arrange
            var lo = new LobbyOptionStub
            {
                MinimumBuyInParameterMock = BuyInParameterEnum.FixedAmount,
                MinimumBuyInValueMock = 42
            };
            var gameSize = 10;

            //Act
            var res = lo.MinimumBuyInAmount(gameSize);

            //Assert
            Assert.AreEqual(lo.MinimumBuyInValue, res);
        }

        [TestMethod]
        public void MultiplicatorMinimumBuyInMeansValueByGameSize()
        {
            //Arrange
            var lo = new LobbyOptionStub
            {
                MinimumBuyInParameterMock = BuyInParameterEnum.Multiplicator,
                MinimumBuyInValueMock = 42
            };
            var gameSize = 10;

            //Act
            var res = lo.MinimumBuyInAmount(gameSize);

            //Assert
            Assert.AreEqual(lo.MinimumBuyInValue * gameSize, res);
        }

        [TestMethod]
        public void UnlimitedMaximumBuyInMeansIntMax()
        {
            //Arrange
            var lo = new LobbyOptionStub
            {
                MaximumBuyInParameterMock = BuyInParameterEnum.Unlimited,
                MaximumBuyInValueMock = 42
            };
            var gameSize = 10;

            //Act
            var res = lo.MaximumBuyInAmount(gameSize);

            //Assert
            Assert.AreEqual(int.MaxValue, res);
        }

        [TestMethod]
        public void FixedMaximumBuyInMeansValue()
        {
            //Arrange
            var lo = new LobbyOptionStub
            {
                MaximumBuyInParameterMock = BuyInParameterEnum.FixedAmount,
                MaximumBuyInValueMock = 42
            };
            var gameSize = 10;

            //Act
            var res = lo.MaximumBuyInAmount(gameSize);

            //Assert
            Assert.AreEqual(lo.MaximumBuyInValue, res);
        }

        [TestMethod]
        public void MultiplicatorMaximumBuyInMeansValueByGameSize()
        {
            //Arrange
            var lo = new LobbyOptionStub
            {
                MaximumBuyInParameterMock = BuyInParameterEnum.Multiplicator,
                MaximumBuyInValueMock = 42
            };
            var gameSize = 10;

            //Act
            var res = lo.MaximumBuyInAmount(gameSize);

            //Assert
            Assert.AreEqual(lo.MaximumBuyInValue * gameSize, res);
        }
        private class LobbyOptionStub : LobbyOptions
        {
            public override LobbyTypeEnum OptionType => LobbyTypeEnum.QuickMode;
            public override BuyInParameterEnum MinimumBuyInParameter => MinimumBuyInParameterMock;
            public BuyInParameterEnum MinimumBuyInParameterMock { get; set; }
            public override int MinimumBuyInValue => MinimumBuyInValueMock;
            public int MinimumBuyInValueMock { get; set; }
            public override BuyInParameterEnum MaximumBuyInParameter => MaximumBuyInParameterMock;
            public BuyInParameterEnum MaximumBuyInParameterMock { get; set; }
            public override int MaximumBuyInValue => MaximumBuyInValueMock;
            public int MaximumBuyInValueMock { get; set; }
        }
    }
}
