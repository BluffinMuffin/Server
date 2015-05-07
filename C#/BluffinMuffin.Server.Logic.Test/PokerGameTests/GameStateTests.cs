using BluffinMuffin.Server.Logic.Test.PokerGameTests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BluffinMuffin.Server.DataTypes.Enums;

namespace BluffinMuffin.Server.Logic.Test.PokerGameTests
{
    [TestClass]
    public class GameStateTests
    {
        [TestMethod]
        public void NotStartedStateIsInit()
        {
            var nfo = Simple2PlayersBlindsGameMock.Empty();

            Assert.AreEqual(GameStateEnum.Init, nfo.Game.State, "The game should not be started");
        }
        [TestMethod]
        public void AfterStartedStateIsWaitForPlayers()
        {
            var nfo = Simple2PlayersBlindsGameMock.EmptyButStarted();

            Assert.AreEqual(GameStateEnum.WaitForPlayers, nfo.Game.State, "The game should wait for players");
        }
        [TestMethod]
        public void After1PlayerSeatedStateIsStillWaitForPlayers()
        {
            var nfo = Simple2PlayersBlindsGameMock.WithOnlyP1Seated();

            Assert.AreEqual(GameStateEnum.WaitForPlayers, nfo.Game.State, "The game should still wait for players to sit in when only 1 is seated");
        }
        [TestMethod]
        public void AfterBothPlayerSeatedGameWithoutBlindsStateIsPlaying()
        {
            var nfo = Simple2PlayersNoBlindsGameMock.WithBothPlayersSeated();

            Assert.AreEqual(GameStateEnum.Playing, nfo.Game.State, "The game should now be in the playing state");
        }
        [TestMethod]
        public void AfterBothPlayerSeatedStateIsWaitForBlinds()
        {
            var nfo = Simple2PlayersBlindsGameMock.WithBothPlayersSeated();

            Assert.AreEqual(GameStateEnum.WaitForBlinds, nfo.Game.State, "The game should now wait for blinds");
        }
        [TestMethod]
        public void AfterBothPlayerSeatedAntesStateIsWaitForBlinds()
        {
            var nfo = Simple2PlayersAntesGameMock.WithBothPlayersSeated();

            Assert.AreEqual(GameStateEnum.WaitForBlinds, nfo.Game.State, "The game should now wait for blinds");
        }
        [TestMethod]
        public void AfterFirstBlindStateIsStillWaitForBlinds()
        {
            var nfo = Simple2PlayersBlindsGameMock.WithBothPlayersSeated();
            nfo.PutBlinds(nfo.P1);

            Assert.AreEqual(GameStateEnum.WaitForBlinds, nfo.Game.State, "The game should still wait for blinds, missing the one from p2");
        }

        [TestMethod]
        public void AfterBlindsGameStateIsPlaying()
        {
            var nfo = Simple2PlayersBlindsGameMock.BlindsPosted();

            Assert.AreEqual(GameStateEnum.Playing, nfo.Game.State, "The game should now be in the playing state");
        }
        [TestMethod]
        public void AfterPlayerFoldStateIsWaitForBlinds()
        {
            var nfo = Simple2PlayersBlindsGameMock.BlindsPosted();
            nfo.CurrentPlayerFolds();
            Assert.AreEqual(GameStateEnum.WaitForBlinds, nfo.Game.State, "The game should be back waiting for blinds sincepot was won and it's starting over");
        }
        [TestMethod]
        public void AfterPlayerLeftStateIsWaitForPlayers()
        {
            var nfo = Simple2PlayersBlindsGameMock.BlindsPosted();
            nfo.Game.LeaveGame(nfo.CurrentPlayer);
            Assert.AreEqual(GameStateEnum.WaitForPlayers, nfo.Game.State, "The game should be back waiting for players since only one player is left");
        }
        [TestMethod]
        public void AfterPlayerLeftThenJoinedStateIsWaitForBlinds()
        {
            var nfo = Simple2PlayersBlindsGameMock.BlindsPosted();
            var curPlayer = nfo.CurrentPlayer;
            nfo.Game.LeaveGame(curPlayer);
            nfo.SitInGame(curPlayer);
            Assert.AreEqual(GameStateEnum.WaitForBlinds, nfo.Game.State, "The game should be back waiting for blinds since enough players are there to play");
        }
        [TestMethod]
        public void IfOnelayerLeftDuringBlindsStateIsStillWaitForBlinds()
        {
            var nfo = Simple2PlayersBlindsGameMock.WithBothPlayersSeated();
            nfo.Game.LeaveGame(nfo.P1);
            Assert.AreEqual(GameStateEnum.WaitForBlinds, nfo.Game.State, "The game should still be waiting for blinds waiting for P2 blind");
        }
        [TestMethod]
        public void IfOnePlayerLeftDuringBlindsAndP2PostBlindStateIsWaitForPlayers()
        {
            var nfo = Simple2PlayersBlindsGameMock.WithBothPlayersSeated();
            nfo.Game.LeaveGame(nfo.P1);
            nfo.PutBlinds(nfo.P2);
            Assert.AreEqual(GameStateEnum.WaitForPlayers, nfo.Game.State, "The game should now be waiting for players: p2 put his blind, the game started, p2 wins the pot, and the game goes back to waiting for players");
        }
        [TestMethod]
        public void IfPlayingPlayerLeftStateIsWaitForPlayers()
        {
            var nfo = Simple2PlayersBlindsGameMock.BlindsPosted();
            var cp = nfo.CurrentPlayer;
            nfo.Game.LeaveGame(cp);
            Assert.AreEqual(GameStateEnum.WaitForPlayers, nfo.Game.State, "The game should now be waiting for players: cp left (folded), other player wins the pot, and the game goes back to waiting for players");
        }
        [TestMethod]
        public void IfNotPlayingPlayerLeftStateIsStillPlaying()
        {
            var nfo = Simple2PlayersBlindsGameMock.BlindsPosted();
            var np = nfo.Game.Table.GetSeatOfPlayingPlayerNextTo(nfo.Game.Table.Seats[nfo.CurrentPlayer.NoSeat]).Player;
            nfo.Game.LeaveGame(np);
            Assert.AreEqual(GameStateEnum.Playing, nfo.Game.State, "The game should be still in playing mode since it wasn't the playing player.");
        }
        [TestMethod]
        public void IfNotPlayingPlayerLeftThenOtherPlaysStateIsNowWaitingForPlayers()
        {
            var nfo = Simple2PlayersBlindsGameMock.BlindsPosted();
            var np = nfo.Game.Table.GetSeatOfPlayingPlayerNextTo(nfo.Game.Table.Seats[nfo.CurrentPlayer.NoSeat]).Player;
            nfo.Game.LeaveGame(np);
            nfo.CurrentPlayerCalls();
            Assert.AreEqual(GameStateEnum.WaitForPlayers, nfo.Game.State, "The game should be back waiting for players: cp plays, np folds since he left, player wins the pot, and the game goes back to waiting for players");
        }
        [TestMethod]
        public void IfEverybodyLeaveStateIsNowEnded()
        {
            var nfo = Simple2PlayersBlindsGameMock.BlindsPosted();
            nfo.Game.LeaveGame(nfo.P1);
            nfo.Game.LeaveGame(nfo.P2);
            Assert.AreEqual(GameStateEnum.End, nfo.Game.State, "The game should be ended");
        }
    }
}
