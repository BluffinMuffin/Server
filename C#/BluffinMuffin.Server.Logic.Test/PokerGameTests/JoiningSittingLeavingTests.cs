using BluffinMuffin.Server.Logic.Test.PokerGameTests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BluffinMuffin.Server.Logic.Test.PokerGameTests
{
    [TestClass]
    public class JoiningSittingLeavingTests
    {

        [TestMethod]
        public void EnterGameThatRequiresMoreMoney()
        {
            var nfo = Simple2PlayersBlindsGameMock.EmptyWithBuyInsSetted();
            nfo.Game.Start();
            nfo.P1 = PlayerMock.GenerateP1ReallyPoor();

            Assert.AreEqual(true, nfo.Game.JoinGame(nfo.P1), "You should be able to join a game without enough money");
        }

        [TestMethod]
        public void ObtainASeatInGameThatRequiresMoreMoney()
        {
            var nfo = Simple2PlayersBlindsGameMock.EmptyWithBuyInsSetted();
            nfo.Game.Start();
            nfo.P1 = PlayerMock.GenerateP1ReallyPoor();
            nfo.Game.JoinGame(nfo.P1);

            Assert.AreEqual(null, nfo.Game.Table.SitIn(nfo.P1, -1), "You should not be able to obtain a seat without enough money");
        }

        [TestMethod]
        public void EnterGameThatIAmToRichToPlay()
        {
            var nfo = Simple2PlayersBlindsGameMock.EmptyWithBuyInsSetted();
            nfo.Game.Start();
            nfo.P1 = PlayerMock.GenerateP1();

            Assert.AreEqual(true, nfo.Game.JoinGame(nfo.P1), "You should be able to join a game with too much money");
        }

        [TestMethod]
        public void ObtainASeatInGameThatIAmToRichToPlay()
        {
            var nfo = Simple2PlayersBlindsGameMock.EmptyWithBuyInsSetted();
            nfo.Game.Start();
            nfo.P1 = PlayerMock.GenerateP1();
            nfo.Game.JoinGame(nfo.P1);

            Assert.AreEqual(null, nfo.Game.Table.SitIn(nfo.P1, -1), "You should not be able to obtain a seat with too much money");
        }

        [TestMethod]
        public void EnterNonStartedGame()
        {
            var nfo = Simple2PlayersBlindsGameMock.Empty();
            nfo.P1 = PlayerMock.GenerateP1();

            Assert.AreEqual(false, nfo.Game.JoinGame(nfo.P1), "You should not enter a non-started game");
        }

        [TestMethod]
        public void EnterStartedGame()
        {
            var nfo = Simple2PlayersBlindsGameMock.EmptyButStarted();
            nfo.P1 = PlayerMock.GenerateP1();

            Assert.AreEqual(true, nfo.Game.JoinGame(nfo.P1), "You should be able to enter a started game with no players");
        }

        [TestMethod]
        public void EnterStartedGameTwice()
        {
            var nfo = Simple2PlayersBlindsGameMock.EmptyButStarted();
            nfo.P1 = PlayerMock.GenerateP1();
            nfo.Game.JoinGame(nfo.P1);

            Assert.AreEqual(false, nfo.Game.JoinGame(nfo.P1), "You should not be able to enter a game while you are in it");
        }

        [TestMethod]
        public void EnterStartedGameWithPlayerThatHaveMyNameAlreadyInIt()
        {
            var nfo = Simple2PlayersBlindsGameMock.EmptyButStarted();
            nfo.P1 = PlayerMock.GenerateP1();
            nfo.Game.JoinGame(nfo.P1);
            nfo.P2 = PlayerMock.GenerateP1();

            Assert.AreEqual(false, nfo.Game.JoinGame(nfo.P2), "You should not be able to enter a game while you are in it");
        }

        [TestMethod]
        public void ObtainSeatWhenFirst()
        {
            var nfo = Simple2PlayersBlindsGameMock.EmptyButStarted();
            nfo.P1 = PlayerMock.GenerateP1();
            nfo.Game.JoinGame(nfo.P1);

            Assert.AreNotEqual(null, nfo.Game.Table.SitIn(nfo.P1, -1), "You should be able to obtain a seat in a game with all the seats available");
        }

        [TestMethod]
        public void SitWhenFirst()
        {
            var nfo = Simple2PlayersBlindsGameMock.EmptyButStarted();
            nfo.P1 = PlayerMock.GenerateP1();
            nfo.Game.JoinGame(nfo.P1);
            nfo.Game.Table.SitIn(nfo.P1, -1);

            Assert.AreNotEqual(-1, nfo.Game.AfterPlayerSat(nfo.P1), "You should be able to sit in a game with all the seats available");
        }

        [TestMethod]
        public void ObtainSeatWhenAlreadySeated()
        {
            var nfo = Simple2PlayersBlindsGameMock.WithOnlyP1Seated();

            Assert.AreEqual(null, nfo.Game.Table.SitIn(nfo.P1, -1), "You should not be able to obtain a seat in twice");
        }

        [TestMethod]
        public void EnterStartedGameWith1PSat()
        {
            var nfo = Simple2PlayersBlindsGameMock.WithOnlyP1Seated();
            nfo.P2 = PlayerMock.GenerateP2();

            Assert.AreEqual(true, nfo.Game.JoinGame(nfo.P2), "You should be able to enter a started game with only 1 player");
        }

        [TestMethod]
        public void ObtainSeatWhenOnly1P()
        {
            var nfo = Simple2PlayersBlindsGameMock.WithOnlyP1Seated();
            nfo.P2 = PlayerMock.GenerateP2();
            nfo.Game.JoinGame(nfo.P2);

            Assert.AreNotEqual(null, nfo.Game.Table.SitIn(nfo.P2, -1), "You should be able to obtain a seat in a game with only 1 seated player");
        }

        [TestMethod]
        public void SitWhenOnly1P()
        {
            var nfo = Simple2PlayersBlindsGameMock.WithOnlyP1Seated();
            nfo.P2 = PlayerMock.GenerateP2();
            nfo.Game.JoinGame(nfo.P2);
            nfo.Game.Table.SitIn(nfo.P2, -1);

            Assert.AreNotEqual(-1, nfo.Game.AfterPlayerSat(nfo.P2), "You should be able to sit in a game with only 1 seated player");
        }

        [TestMethod]
        public void EnterStartedGameWith2PSatWithMaxSeat2()
        {
            var nfo = Simple2PlayersBlindsGameMock.WithBothPlayersSeated();
            nfo.P3 = PlayerMock.GenerateP3();

            Assert.AreEqual(true, nfo.Game.JoinGame(nfo.P3), "You should always be able to enter a started game even if full (MaxSeats=2)");
        }

        [TestMethod]
        public void ObtainSeatWhen2PSatWithMaxSeat2()
        {
            var nfo = Simple2PlayersBlindsGameMock.WithBothPlayersSeated();
            nfo.P3 = PlayerMock.GenerateP3();
            nfo.Game.JoinGame(nfo.P3);

            Assert.AreEqual(null, nfo.Game.Table.SitIn(nfo.P3, -1), "You should not be able to obtain a seat in a game that is full (MaxSeats=2)");
        }

        [TestMethod]
        public void SitWhen2PSatWithMaxSeat2()
        {
            var nfo = Simple2PlayersBlindsGameMock.WithBothPlayersSeated();
            nfo.P3 = PlayerMock.GenerateP3();
            nfo.Game.JoinGame(nfo.P3);
            nfo.Game.Table.SitIn(nfo.P3, -1);

            Assert.AreEqual(-1, nfo.Game.AfterPlayerSat(nfo.P3), "You should not be able to sit in a game that is full (MaxSeats=2)");
        }

        [TestMethod]
        public void ObtainSeatWhen2PSatWithMaxSeat2But1PLeft()
        {
            var nfo = Simple2PlayersBlindsGameMock.WithBothPlayersSeated();
            nfo.P3 = PlayerMock.GenerateP3();
            nfo.Game.JoinGame(nfo.P3);
            nfo.Game.LeaveGame(nfo.P1);

            Assert.AreNotEqual(null, nfo.Game.Table.SitIn(nfo.P3, -1), "You should be able to obtain a seat a game that is now with only 1 seated player");
        }

        [TestMethod]
        public void ObtainSeatWhen2PSatWithMaxSeat2But1PSatOut()
        {
            var nfo = Simple2PlayersBlindsGameMock.WithBothPlayersSeated();
            nfo.P3 = PlayerMock.GenerateP3();
            nfo.Game.JoinGame(nfo.P3);
            nfo.Game.SitOut(nfo.P1);

            Assert.AreNotEqual(null, nfo.Game.Table.SitIn(nfo.P3, -1), "You should be able to obtain a seat a game that is now with only 1 seated player");
        }

        [TestMethod]
        public void EnterStartedGameThatEverybodyLeft()
        {
            var nfo = Simple2PlayersBlindsGameMock.WithBothPlayersSeated();
            nfo.P3 = PlayerMock.GenerateP3();
            nfo.Game.LeaveGame(nfo.P1);
            nfo.Game.LeaveGame(nfo.P2);
            Assert.AreEqual(false, nfo.Game.JoinGame(nfo.P3), "You should not enter an ended game");
        }
    }
}
