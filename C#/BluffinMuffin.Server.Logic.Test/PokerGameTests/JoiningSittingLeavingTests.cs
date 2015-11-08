using BluffinMuffin.Server.Logic.Test.PokerGameTests.DataTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BluffinMuffin.Server.Logic.Test.PokerGameTests
{
    [TestClass]
    public class JoiningSittingLeavingTests
    {

        [TestMethod]
        public void EnterGameThatRequiresMoreMoney()
        {
            //Arrange
            var nfo = new ModularGameMock(new LimitedBuyInModule(true)).EmptyButStarted();
            nfo.P1 = new ModularPlayerMock(nfo,PlayerNames.P1,new MoneyModule(100)).Player;

            //Act
            var res = nfo.Game.JoinGame(nfo.P1);

            //Assert
            Assert.AreEqual(true, res, "You should be able to join a game without enough money");
        }

        [TestMethod]
        public void ObtainASeatInGameThatRequiresMoreMoney()
        {
            //Arrange
            var nfo = new ModularGameMock(new LimitedBuyInModule(true)).EmptyButStarted();
            nfo.P1 = new ModularPlayerMock(nfo, PlayerNames.P1, new MoneyModule(100), new JoinGameModule()).Player;

            //Act
            var res = nfo.Game.Table.SitIn(nfo.P1, -1);

            //Assert
            Assert.AreEqual(null, res, "You should not be able to obtain a seat without enough money");
        }

        [TestMethod]
        public void EnterGameThatIAmRichEnoughToPlay()
        {
            //Arrange
            var nfo = new ModularGameMock(new LimitedBuyInModule(true)).EmptyButStarted();
            nfo.P1 = new ModularPlayerMock(nfo, PlayerNames.P1, new MoneyModule(5000)).Player;

            //Act
            var res = nfo.Game.JoinGame(nfo.P1);

            //Assert
            Assert.AreEqual(true, res, "You should be able to join a game with too much money");
        }

        [TestMethod]
        public void ObtainASeatInGameThatIAmRichEnoughToPlay()
        {
            //Arrange
            var nfo = new ModularGameMock(new LimitedBuyInModule(true)).EmptyButStarted();
            nfo.P1 = new ModularPlayerMock(nfo, PlayerNames.P1, new MoneyModule(5000), new JoinGameModule()).Player;

            //Act
            var res = nfo.Game.Table.SitIn(nfo.P1, -1);

            //Assert
            Assert.AreEqual(null, res, "You should not be able to obtain a seat with too much money");
        }

        [TestMethod]
        public void EnterNonStartedGame()
        {
            //Arrange
            var nfo = new ModularGameMock().Empty();
            nfo.P1 = new ModularPlayerMock(nfo, PlayerNames.P1).Player;

            //Act
            var res = nfo.Game.JoinGame(nfo.P1);

            //Assert
            Assert.AreEqual(false, res, "You should not enter a non-started game");
        }

        [TestMethod]
        public void EnterStartedGame()
        {
            //Arrange
            var nfo = new ModularGameMock().EmptyButStarted();
            nfo.P1 = new ModularPlayerMock(nfo, PlayerNames.P1).Player;

            //Act
            var res = nfo.Game.JoinGame(nfo.P1);

            //Assert
            Assert.AreEqual(true, res, "You should be able to enter a started game with no players");
        }

        [TestMethod]
        public void EnterStartedGameTwice()
        {
            //Arrange
            var nfo = new ModularGameMock().EmptyButStarted();
            nfo.P1 = new ModularPlayerMock(nfo, PlayerNames.P1, new JoinGameModule()).Player;

            //Act
            var res = nfo.Game.JoinGame(nfo.P1);

            //Assert
            Assert.AreEqual(false, res, "You should not be able to enter a game while you are in it");
        }

        [TestMethod]
        public void EnterStartedGameWithPlayerThatHaveMyNameAlreadyInIt()
        {
            //Arrange
            var nfo = new ModularGameMock().EmptyButStarted();
            nfo.P1 = new ModularPlayerMock(nfo, PlayerNames.P1, new JoinGameModule()).Player;
            nfo.P2 = new ModularPlayerMock(nfo, PlayerNames.P1).Player;

            //Act
            var res = nfo.Game.JoinGame(nfo.P2);

            //Assert
            Assert.AreEqual(false, res, "You should not be able to enter a game while you are in it");
        }

        [TestMethod]
        public void ObtainSeatWhenFirst()
        {
            //Arrange
            var nfo = new ModularGameMock().EmptyButStarted();
            nfo.P1 = new ModularPlayerMock(nfo, PlayerNames.P1, new JoinGameModule()).Player;

            //Act
            var res = nfo.Game.Table.SitIn(nfo.P1, -1);

            //Assert
            Assert.AreNotEqual(null, res, "You should be able to obtain a seat in a game with all the seats available");
        }

        [TestMethod]
        public void SitWhenFirst()
        {
            //Arrange
            var nfo = new ModularGameMock().EmptyButStarted();
            nfo.P1 = new ModularPlayerMock(nfo, PlayerNames.P1, new JoinGameModule()).Player;
            nfo.Game.Table.SitIn(nfo.P1, -1);

            //Act
            var res = nfo.Game.AfterPlayerSat(nfo.P1);

            //Assert
            Assert.AreNotEqual(-1, res, "You should be able to sit in a game with all the seats available");
        }

        [TestMethod]
        public void ObtainSeatWhenAlreadySeated()
        {
            //Arrange
            var nfo = new ModularGameMock().WithP1Seated();

            //Act
            var res = nfo.Game.Table.SitIn(nfo.P1, -1);

            //Assert
            Assert.AreEqual(null, res, "You should not be able to obtain a seat in twice");
        }

        [TestMethod]
        public void EnterStartedGameWith1PSat()
        {
            //Arrange
            var nfo = new ModularGameMock().WithP1Seated();
            nfo.P2 = new ModularPlayerMock(nfo, PlayerNames.P2).Player;

            //Act
            var res = nfo.Game.JoinGame(nfo.P2);

            //Assert
            Assert.AreEqual(true, res, "You should be able to enter a started game with only 1 player");
        }

        [TestMethod]
        public void ObtainSeatWhenOnly1P()
        {
            //Arrange
            var nfo = new ModularGameMock().WithP1Seated();
            nfo.P2 = new ModularPlayerMock(nfo, PlayerNames.P2, new JoinGameModule()).Player;

            //Act
            var res = nfo.Game.Table.SitIn(nfo.P2, -1);

            //Assert
            Assert.AreNotEqual(null, res, "You should be able to obtain a seat in a game with only 1 seated player");
        }

        [TestMethod]
        public void SitWhenOnly1P()
        {
            //Arrange
            var nfo = new ModularGameMock().WithP1Seated();
            nfo.P2 = new ModularPlayerMock(nfo, PlayerNames.P2, new JoinGameModule()).Player;
            nfo.Game.Table.SitIn(nfo.P2, -1);

            //Act
            var res = nfo.Game.AfterPlayerSat(nfo.P2);

            //Assert
            Assert.AreNotEqual(-1, res, "You should be able to sit in a game with only 1 seated player");
        }

        [TestMethod]
        public void EnterStartedGameWith2PSatWithMaxSeat2()
        {
            //Arrange
            var nfo = new ModularGameMock().WithAllPlayersSeated();
            nfo.P3 = new ModularPlayerMock(nfo, PlayerNames.P3).Player;

            //Act
            var res = nfo.Game.JoinGame(nfo.P3);

            //Assert
            Assert.AreEqual(true, res, "You should always be able to enter a started game even if full (MaxSeats=2)");
        }

        [TestMethod]
        public void ObtainSeatWhen2PSatWithMaxSeat2()
        {
            //Arrange
            var nfo = new ModularGameMock().WithAllPlayersSeated();
            nfo.P3 = new ModularPlayerMock(nfo, PlayerNames.P3, new JoinGameModule()).Player;

            //Act
            var res = nfo.Game.Table.SitIn(nfo.P3, -1);

            //Assert
            Assert.AreEqual(null, res, "You should not be able to obtain a seat in a game that is full (MaxSeats=2)");
        }

        [TestMethod]
        public void SitWhen2PSatWithMaxSeat2()
        {
            //Arrange
            var nfo = new ModularGameMock().WithAllPlayersSeated();
            nfo.P3 = new ModularPlayerMock(nfo, PlayerNames.P3, new JoinGameModule()).Player;
            nfo.Game.Table.SitIn(nfo.P3, -1);

            //Act
            var res = nfo.Game.AfterPlayerSat(nfo.P3);

            //Assert
            Assert.AreEqual(-1, res, "You should not be able to sit in a game that is full (MaxSeats=2)");
        }

        [TestMethod]
        public void ObtainSeatWhen2PSatWithMaxSeat2But1PLeft()
        {
            //Arrange
            var nfo = new ModularGameMock().WithAllPlayersSeated();
            nfo.P3 = new ModularPlayerMock(nfo, PlayerNames.P3, new JoinGameModule()).Player;
            nfo.Game.LeaveGame(nfo.P1);

            //Act
            var res = nfo.Game.Table.SitIn(nfo.P3, -1);

            //Assert
            Assert.AreNotEqual(null, res, "You should be able to obtain a seat a game that is now with only 1 seated player");
        }

        [TestMethod]
        public void ObtainSeatWhen2PSatWithMaxSeat2But1PSatOut()
        {
            //Arrange
            var nfo = new ModularGameMock().WithAllPlayersSeated();
            nfo.P3 = new ModularPlayerMock(nfo, PlayerNames.P3, new JoinGameModule()).Player;
            nfo.Game.SitOut(nfo.P1);

            //Act
            var res = nfo.Game.Table.SitIn(nfo.P3, -1);

            //Assert
            Assert.AreNotEqual(null, res, "You should be able to obtain a seat a game that is now with only 1 seated player");
        }

        [TestMethod]
        public void EnterStartedGameThatEverybodyLeft()
        {
            //Arrange
            var nfo = new ModularGameMock().WithAllPlayersSeated();
            nfo.P3 = new ModularPlayerMock(nfo, PlayerNames.P3).Player;
            nfo.Game.LeaveGame(nfo.P1);
            nfo.Game.LeaveGame(nfo.P2);

            //Act
            var res = nfo.Game.JoinGame(nfo.P3);

            //Assert
            Assert.AreEqual(false, res, "You should not enter an ended game");
        }
    }
}
