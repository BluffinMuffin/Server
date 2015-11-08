using BluffinMuffin.Protocol.DataTypes;

namespace BluffinMuffin.Server.Logic.Test.PokerGameTests.DataTypes
{
    class ModularPlayerMock
    {
        private GameMockInfo Game { get; }
        public PlayerInfo Player { get; }

        public ModularPlayerMock(GameMockInfo nfo, PlayerNames name, params IPlayerModule[] modules)
        {
            Game = nfo;
            Player = new PlayerInfo(name.ToString(), 5000);
            foreach (var m in modules)
                m.ExecuteModule(Game, Player);
        }
    }
    enum PlayerNames
    {
        P1,
        P2,
        P3,
        P4
    }

    interface IPlayerModule
    {
        void ExecuteModule(GameMockInfo nfo, PlayerInfo p);
    }

    class MoneyModule : IPlayerModule
    {
        private readonly int m_Money;

        public MoneyModule(int money)
        {
            m_Money = money;
        }

        public void ExecuteModule(GameMockInfo nfo, PlayerInfo p)
        {
            p.MoneySafeAmnt = m_Money;
        }
    }
    class JoinGameModule : IPlayerModule
    {

        public void ExecuteModule(GameMockInfo nfo, PlayerInfo p)
        {
            nfo.Game.JoinGame(p);
        }
    }
    class SitInGameModule : IPlayerModule
    {

        public void ExecuteModule(GameMockInfo nfo, PlayerInfo p)
        {
            nfo.SitInGame(p);
        }
    }
}
