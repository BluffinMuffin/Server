using BluffinMuffin.Protocol.DataTypes.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BluffinMuffin.Server.DataTypes.Enums;
using BluffinMuffin.Server.Logic.Test.PokerGameTests.DataTypes;

namespace BluffinMuffin.Server.Logic.Test.PokerGameTests
{
    [TestClass]
    public class GameStateTests
    {
        [TestMethod]
        public void NotStartedStateIsInit()
        {
            //Arrange
            var nfo = new ModularGameMock().Empty();

            //Act

            //Assert
            Assert.AreEqual(GameStateEnum.Init, nfo.Game.State, "The game should not be started");
        }
        [TestMethod]
        public void AfterStartedStateIsWaitForPlayers()
        {
            //Arrange
            var nfo = new ModularGameMock().EmptyButStarted();

            //Act

            //Assert
            Assert.AreEqual(GameStateEnum.WaitForPlayers, nfo.Game.State, "The game should wait for players");
        }
        [TestMethod]
        public void After1PlayerSeatedStateIsStillWaitForPlayers()
        {
            //Arrange
            var nfo = new ModularGameMock().WithP1Seated();

            //Act

            //Assert
            Assert.AreEqual(GameStateEnum.WaitForPlayers, nfo.Game.State, "The game should still wait for players to sit in when only 1 is seated");
        }
        [TestMethod]
        public void AfterBothPlayerSeatedGameWithoutBlindsStateIsPlaying()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.None)).WithAllPlayersSeated();

            //Act

            //Assert
            Assert.AreEqual(GameStateEnum.Playing, nfo.Game.State, "The game should now be in the playing state");
        }
        [TestMethod]
        public void AfterBothPlayerSeatedStateIsWaitForBlinds()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).WithAllPlayersSeated();

            //Act

            //Assert
            Assert.AreEqual(GameStateEnum.WaitForBlinds, nfo.Game.State, "The game should now wait for blinds");
        }
        [TestMethod]
        public void AfterBothPlayerSeatedAntesStateIsWaitForBlinds()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Antes)).WithAllPlayersSeated();

            //Act

            //Assert
            Assert.AreEqual(GameStateEnum.WaitForBlinds, nfo.Game.State, "The game should now wait for blinds");
        }
        [TestMethod]
        public void AfterFirstBlindStateIsStillWaitForBlinds()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).WithAllPlayersSeated();

            //Act
            nfo.PutBlinds(nfo.P1);

            //Assert
            Assert.AreEqual(GameStateEnum.WaitForBlinds, nfo.Game.State, "The game should still wait for blinds, missing the one from p2");
        }

        [TestMethod]
        public void AfterBlindsGameStateIsPlaying()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).BlindsPosted();

            //Act

            //Assert
            Assert.AreEqual(GameStateEnum.Playing, nfo.Game.State, "The game should now be in the playing state");
        }
        [TestMethod]
        public void AfterPlayerFoldStateIsWaitForBlinds()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).BlindsPosted();

            //Act
            nfo.CurrentPlayerFolds();

            //Assert
            Assert.AreEqual(GameStateEnum.WaitForBlinds, nfo.Game.State, "The game should be back waiting for blinds sincepot was won and it's starting over");
        }
        [TestMethod]
        public void AfterPlayerLeftStateIsWaitForPlayers()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).BlindsPosted();

            //Act
            nfo.Game.LeaveGame(nfo.CurrentPlayer);

            //Assert
            Assert.AreEqual(GameStateEnum.WaitForPlayers, nfo.Game.State, "The game should be back waiting for players since only one player is left");
        }
        [TestMethod]
        public void AfterPlayerLeftThenJoinedStateIsWaitForBlinds()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).BlindsPosted();
            var curPlayer = nfo.CurrentPlayer;

            //Act
            nfo.Game.LeaveGame(curPlayer);
            nfo.Game.JoinGame(curPlayer);
            nfo.SitInGame(curPlayer);

            //Assert
            Assert.AreEqual(GameStateEnum.WaitForBlinds, nfo.Game.State, "The game should be back waiting for blinds since enough players are there to play");
        }
        [TestMethod]
        public void IfOnelayerLeftDuringBlindsStateIsStillWaitForBlinds()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).WithAllPlayersSeated();

            //Act
            nfo.Game.LeaveGame(nfo.P1);

            //Assert
            Assert.AreEqual(GameStateEnum.WaitForBlinds, nfo.Game.State, "The game should still be waiting for blinds waiting for P2 blind");
        }
        [TestMethod]
        public void IfOnePlayerLeftDuringBlindsAndP2PostBlindStateIsWaitForPlayers()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).WithAllPlayersSeated();
            nfo.Game.LeaveGame(nfo.P1);

            //Act
            nfo.PutBlinds(nfo.P2);

            //Assert
            Assert.AreEqual(GameStateEnum.WaitForPlayers, nfo.Game.State, "The game should now be waiting for players: p2 put his blind, the game started, p2 wins the pot, and the game goes back to waiting for players");
        }
        [TestMethod]
        public void IfPlayingPlayerLeftStateIsWaitForPlayers()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).BlindsPosted();

            //Act
            nfo.Game.LeaveGame(nfo.CurrentPlayer);

            //Assert
            Assert.AreEqual(GameStateEnum.WaitForPlayers, nfo.Game.State, "The game should now be waiting for players: cp left (folded), other player wins the pot, and the game goes back to waiting for players");
        }
        [TestMethod]
        public void IfOtherLeftStateIsStillPlaying()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).BlindsPosted();
            var otherPlayer = nfo.Game.Table.GetSeatOfPlayingPlayerNextTo(nfo.Game.Table.Seats[nfo.CurrentPlayer.NoSeat]).Player;

            //Act
            nfo.Game.LeaveGame(otherPlayer);

            //Assert
            Assert.AreEqual(GameStateEnum.Playing, nfo.Game.State, "The game should be still in playing mode since it wasn't the playing player.");
        }
        [TestMethod]
        public void IfOtherLeftThenCurrentPlaysStateIsNowWaitingForPlayers()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).BlindsPosted();
            var otherPlayer = nfo.Game.Table.GetSeatOfPlayingPlayerNextTo(nfo.Game.Table.Seats[nfo.CurrentPlayer.NoSeat]).Player;
            nfo.Game.LeaveGame(otherPlayer);

            //Act
            nfo.CurrentPlayerCalls();

            //Assert
            Assert.AreEqual(GameStateEnum.WaitForPlayers, nfo.Game.State, "The game should be back waiting for players: cp plays, np folds since he left, player wins the pot, and the game goes back to waiting for players");
        }
        [TestMethod]
        public void IfEverybodyLeaveStateIsNowEnded()
        {
            //Arrange
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).BlindsPosted();
            
            //Act
            nfo.Game.LeaveGame(nfo.P1);
            nfo.Game.LeaveGame(nfo.P2);

            //Assert
            Assert.AreEqual(GameStateEnum.End, nfo.Game.State, "The game should be ended");
        }
    }
}
