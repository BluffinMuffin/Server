using System.Linq;
using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Protocol.DataTypes.Enums;
using BluffinMuffin.Protocol.DataTypes.Options;

namespace BluffinMuffin.Server.Logic.Test.PokerGameTests.DataTypes
{
    class ModularGameMock
    {
        private TableParams Parms { get; }

        public ModularGameMock(params IGameModule[] modules)
        {

            Parms = new TableParams()
            {
                MaxPlayers = 2,
                GameSize = 10,
                Blind = BlindTypeEnum.Blinds,
                Lobby = new LobbyOptionsRegisteredMode()
                {
                    IsMaximumBuyInLimited = false
                }
            };

            foreach (var m in modules)
                m.ExecuteModule(Parms);
        }
        public GameMockInfo Empty()
        {
            return new GameMockInfo()
            {
                Game = new PokerGame(
                    new PokerTable(Parms))
            };
        }
        public GameMockInfo EmptyButStarted()
        {
            var nfo = Empty();
            nfo.Game.Start();

            return nfo;
        }
        public GameMockInfo WithP1Seated()
        {
            var nfo = EmptyButStarted();
            nfo.P1 = new ModularPlayerMock(nfo, PlayerNames.P1, new JoinGameModule(), new SitInGameModule()).Player;

            return nfo;
        }
        public GameMockInfo WithP1P2Seated()
        {
            var nfo = WithP1Seated();
            nfo.P2 = new ModularPlayerMock(nfo, PlayerNames.P2, new JoinGameModule(), new SitInGameModule()).Player;

            return nfo;
        }
        public GameMockInfo WithP1P2P3Seated()
        {
            var nfo = WithP1P2Seated();
            nfo.P3 = new ModularPlayerMock(nfo, PlayerNames.P3, new JoinGameModule(), new SitInGameModule()).Player;

            return nfo;
        }
        public GameMockInfo WithP1P2P3P4Seated()
        {
            var nfo = WithP1P2P3Seated();
            nfo.P4 = new ModularPlayerMock(nfo, PlayerNames.P4, new JoinGameModule(), new SitInGameModule()).Player;

            return nfo;
        }
        public GameMockInfo WithAllPlayersSeated()
        {
            var nfo = WithP1Seated();

            if (Parms.MaxPlayers >= 2)
                nfo.P2 = new ModularPlayerMock(nfo, PlayerNames.P2, new JoinGameModule(), new SitInGameModule()).Player;

            if (Parms.MaxPlayers >= 3)
                nfo.P3 = new ModularPlayerMock(nfo, PlayerNames.P3, new JoinGameModule(), new SitInGameModule()).Player;

            if (Parms.MaxPlayers >= 4)
                nfo.P4 = new ModularPlayerMock(nfo, PlayerNames.P4, new JoinGameModule(), new SitInGameModule()).Player;
            
            return nfo;
        }
        public GameMockInfo BlindsPosted()
        {
            var nfo = WithAllPlayersSeated();

            foreach(var p in nfo.Players)
                nfo.PutBlinds(p);

            return nfo;
        }
        public GameMockInfo AfterPreflop()
        {
            var nfo = BlindsPosted();


            for (int i = 0; i < nfo.Players.Count(); ++i)
                nfo.CurrentPlayerCalls();

            return nfo;
        }
        public GameMockInfo AfterFlop()
        {
            var nfo = AfterPreflop();

            for (int i = 0; i < nfo.Players.Count(); ++i)
                nfo.CurrentPlayerCalls();

            return nfo;
        }
        public GameMockInfo AfterTurn()
        {
            var nfo = AfterFlop();

            for (int i = 0; i < nfo.Players.Count(); ++i)
                nfo.CurrentPlayerCalls();

            return nfo;
        }
    }
    interface IGameModule
    {
        void ExecuteModule(TableParams parms);
    }

    class NbPlayersModule : IGameModule
    {
        private readonly int m_NbMin;
        private readonly int m_NbMax;

        public NbPlayersModule(int nb) : this(nb, nb)
        {
        }
        public NbPlayersModule(int nbMin, int nbMax)
        {
            m_NbMin = nbMin;
            m_NbMax = nbMax;
        }
        public void ExecuteModule(TableParams parms)
        {
            parms.MaxPlayers = m_NbMax;
            parms.MinPlayersToStart = m_NbMin;
        }
    }

    class BlindModule : IGameModule
    {
        private readonly BlindTypeEnum m_Blind;

        public BlindModule(BlindTypeEnum blind)
        {
            m_Blind = blind;
        }
        public void ExecuteModule(TableParams parms)
        {
            parms.Blind = m_Blind;
        }
    }

    internal class LimitedBuyInModule : IGameModule
    {
        private readonly bool m_Limited;

        public LimitedBuyInModule(bool limited)
        {
            m_Limited = limited;
        }

        public void ExecuteModule(TableParams parms)
        {
            parms.Lobby = new LobbyOptionsRegisteredMode()
            {
                IsMaximumBuyInLimited = m_Limited
            };
        }
    }
}
