﻿using System;
using System.Linq;
using BluffinMuffin.Protocol.DataTypes.Enums;
using BluffinMuffin.Server.Logic.Test.PokerGameTests.DataTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BluffinMuffin.Server.Logic.Test.PokerGameTests
{
    [TestClass]
    public class BlindsTests
    {
        [TestMethod]
        public void AntesGameAllPlayerNeedsToPutTheSameBlind()
        {
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Antes), new NbPlayersModule(4)).WithAllPlayersSeated();
            Assert.AreEqual(true, nfo.Players.All(x => nfo.BlindNeeded(x) == Math.Max(nfo.Game.Table.Params.GameSize/10,1)), "The game should need the same blind for everybody (Antes)");
        }

        [TestMethod]
        public void StartGameAndCheckNeededBlindP1()
        {
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).WithAllPlayersSeated();
            Assert.AreNotEqual(0, nfo.BlindNeeded(nfo.P1), "The game should need a blind from p1");
        }

        [TestMethod]
        public void StartGameAndCheckNeededBlindP2()
        {
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).WithAllPlayersSeated();
            Assert.AreNotEqual(0, nfo.BlindNeeded(nfo.P2), "The game should need a blind from p2");
        }

        [TestMethod]
        public void StartGameAndTryPutBlindMoreThanNeeded()
        {
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).WithAllPlayersSeated();
            Assert.AreEqual(false, nfo.Game.PlayMoney(nfo.P1, nfo.BlindNeeded(nfo.P1) + 1), "The game should not accept any blind that is over what is needed");
        }

        [TestMethod]
        public void StartGameAndTryPutBlindLessThanNeeded()
        {
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).WithAllPlayersSeated();
            Assert.AreEqual(false, nfo.Game.PlayMoney(nfo.P1, nfo.BlindNeeded(nfo.P1) - 1), "The game should not accept any blind that is under what is needed unless that is all the player got");
        }

        [TestMethod]
        public void StartGameAndTryPutBlindLessThanNeededWithPoorPlayer()
        {
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).WithP1Seated();
            nfo.P2 = new ModularPlayerMock(nfo, PlayerNames.P2, new JoinGameModule(), new SitInGameModule(), new MoneyModule(2)).Player;

            Assert.AreEqual(true, nfo.Game.PlayMoney(nfo.P2, nfo.P2.MoneySafeAmnt), "The game should accept a blind that is under what is needed if that is all the player got");
        }

        [TestMethod]
        public void StartGameAndTryPutBlind()
        {
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).WithAllPlayersSeated();

            Assert.AreEqual(true, nfo.Game.PlayMoney(nfo.P1, nfo.BlindNeeded(nfo.P1)), "The game should accept a perfect blind");
        }

        [TestMethod]
        public void StartGameAndCheckNeededBlindP1AfterP1PutHis()
        {
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).WithAllPlayersSeated();

            nfo.PutBlinds(nfo.P1);

            Assert.AreEqual(0, nfo.BlindNeeded(nfo.P1), "The game should not need a blind from p1 anymore");
        }

        [TestMethod]
        public void StartGameAndCheckNeededBlindP2AfterP1PutHis()
        {
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).WithAllPlayersSeated();

            nfo.PutBlinds(nfo.P1);

            Assert.AreNotEqual(0, nfo.BlindNeeded(nfo.P2), "The game should still need a blind from p2");
        }

        [TestMethod]
        public void LeaveGameBeforePuttingBlindShouldStillSubstractTheAmountFromMoney()
        {
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).WithAllPlayersSeated();
            var safeMoneyBefore = nfo.P1.MoneySafeAmnt;
            nfo.Game.LeaveGame(nfo.P1);
            Assert.AreEqual(true, nfo.P1.MoneySafeAmnt < safeMoneyBefore, "The player should have less money then before, since blinds were posted automatically before he left");
        }

        [TestMethod]
        public void StartSecondGameAndCheckNeededBlindOfBothPlayers()
        {
            var nfo = new ModularGameMock(new BlindModule(BlindTypeEnum.Blinds)).WithAllPlayersSeated();

            nfo.PutBlinds(nfo.P1);
            nfo.PutBlinds(nfo.P2);
            nfo.CurrentPlayerFolds();

            Assert.AreNotEqual(nfo.BlindNeeded(nfo.P1), nfo.BlindNeeded(nfo.P2), "The game should need a big blind and a small blind, not two big blinds");
        }
    }
}
