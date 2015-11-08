using BluffinMuffin.Protocol.DataTypes.Options;
using BluffinMuffin.Server.Logic.Test.PokerGameTests.DataTypes;
using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Protocol.DataTypes.Enums;

namespace BluffinMuffin.Server.Logic.Test.PokerGameTests.Mocks
{
    public static class Simple3PlayersBlindsGameMock
    {
        public static GameMockInfo Empty()
        {
            return new GameMockInfo()
            {
                Game = new PokerGame(
                    new PokerTable(
                        new TableParams()
                        {
                            MaxPlayers = 3,
                            MinPlayersToStart = 3,
                            Blind = BlindTypeEnum.Blinds,
                            Lobby = new LobbyOptionsRegisteredMode()
                            {
                                IsMaximumBuyInLimited = false
                            }
                        }))
            };
        }
        public static GameMockInfo EmptyButStarted()
        {
            var nfo = Empty();
            nfo.Game.Start();

            return nfo;
        }
        public static GameMockInfo WithOnlyP1Seated()
        {
            var nfo = EmptyButStarted();
            nfo.P1 = new ModularPlayerMock(nfo, PlayerNames.P1, new JoinGameModule(), new SitInGameModule()).Player;

            return nfo;
        }
        public static GameMockInfo WithOnlyP1P2Seated()
        {
            var nfo = WithOnlyP1Seated();
            nfo.P2 = new ModularPlayerMock(nfo, PlayerNames.P2, new JoinGameModule(), new SitInGameModule()).Player;

            return nfo;
        }
        public static GameMockInfo WithAllPlayersSeated()
        {
            var nfo = WithOnlyP1P2Seated();
            nfo.P3 = new ModularPlayerMock(nfo, PlayerNames.P3, new JoinGameModule(), new SitInGameModule()).Player;

            return nfo;
        }
        public static GameMockInfo BlindsPosted()
        {
            var nfo = WithAllPlayersSeated();

            nfo.PutBlinds(nfo.CalculatedSmallBlind);
            nfo.PutBlinds(nfo.CalculatedBigBlind);

            return nfo;
        }
        public static GameMockInfo AfterPreflop()
        {
            var nfo = BlindsPosted();

            nfo.CurrentPlayerCalls();
            nfo.CurrentPlayerCalls();
            nfo.CurrentPlayerCalls();

            return nfo;
        }
        public static GameMockInfo AfterFlop()
        {
            var nfo = AfterPreflop();

            nfo.CurrentPlayerCalls();
            nfo.CurrentPlayerCalls();
            nfo.CurrentPlayerCalls();

            return nfo;
        }
        public static GameMockInfo AfterTurn()
        {
            var nfo = AfterFlop();

            nfo.CurrentPlayerCalls();
            nfo.CurrentPlayerCalls();
            nfo.CurrentPlayerCalls();

            return nfo;
        }
    }
}
