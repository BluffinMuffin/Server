using BluffinMuffin.Protocol.DataTypes.Options;
using BluffinMuffin.Server.Logic.Test.PokerGameTests.DataTypes;
using BluffinMuffin.Protocol.DataTypes;

namespace BluffinMuffin.Server.Logic.Test.PokerGameTests.Mocks
{
    public static class Simple4PlayersBlindsGameMock
    {
        public static GameInfo Empty()
        {
            return new GameInfo()
            {
                Game = new PokerGame(
                    new PokerTable(
                        new TableParams()
                        {
                            MaxPlayers = 4,
                            MinPlayersToStart = 4,
                            Blind = new BlindOptionsBlinds()
                            {
                                MoneyUnit = 10
                            },
                            Lobby = new LobbyOptionsRegisteredMode()
                            {
                                IsMaximumBuyInLimited = false,
                                MoneyUnit = 0 // Little trick to not get bothered.
                            }
                        }))
            };
        }
        public static GameInfo EmptyButStarted()
        {
            var nfo = Empty();
            nfo.Game.Start();

            return nfo;
        }
        public static GameInfo WithOnlyP1Seated()
        {
            var nfo = EmptyButStarted();
            nfo.P1 = PlayerMock.GenerateP1Seated(nfo);

            return nfo;
        }
        public static GameInfo WithOnlyP1P2Seated()
        {
            var nfo = WithOnlyP1Seated();
            nfo.P2 = PlayerMock.GenerateP2Seated(nfo);

            return nfo;
        }
        public static GameInfo WithOnlyP1P2P3Seated()
        {
            var nfo = WithOnlyP1P2Seated();
            nfo.P3 = PlayerMock.GenerateP3Seated(nfo);

            return nfo;
        }
        public static GameInfo WithAllPlayersSeated()
        {
            var nfo = WithOnlyP1P2P3Seated();
            nfo.P4 = PlayerMock.GenerateP4Seated(nfo);

            return nfo;
        }
        public static GameInfo BlindsPosted()
        {
            var nfo = WithAllPlayersSeated();

            nfo.PutBlinds(nfo.CalculatedSmallBlind);
            nfo.PutBlinds(nfo.CalculatedBigBlind);

            return nfo;
        }
        public static GameInfo AfterPreflop()
        {
            var nfo = BlindsPosted();

            nfo.CurrentPlayerCalls();
            nfo.CurrentPlayerCalls();
            nfo.CurrentPlayerCalls();
            nfo.CurrentPlayerCalls();

            return nfo;
        }
        public static GameInfo AfterFlop()
        {
            var nfo = AfterPreflop();

            nfo.CurrentPlayerCalls();
            nfo.CurrentPlayerCalls();
            nfo.CurrentPlayerCalls();
            nfo.CurrentPlayerCalls();

            return nfo;
        }
        public static GameInfo AfterTurn()
        {
            var nfo = AfterFlop();

            nfo.CurrentPlayerCalls();
            nfo.CurrentPlayerCalls();
            nfo.CurrentPlayerCalls();
            nfo.CurrentPlayerCalls();

            return nfo;
        }
    }
}
