using BluffinMuffin.Server.Logic.Test.PokerGameTests.DataTypes;
using BluffinMuffin.Protocol.DataTypes;

namespace BluffinMuffin.Server.Logic.Test.PokerGameTests.Mocks
{
    public static class PlayerMock
    {
        public static PlayerInfo GenerateP1()
        {
            return new PlayerInfo("p1", 5000);
        }
        public static PlayerInfo GenerateP1ReallyPoor()
        {
            return new PlayerInfo("p1", 100);
        }
        public static PlayerInfo GenerateP2()
        {
            return new PlayerInfo("p2", 5000);
        }
        public static PlayerInfo GenerateP2ReallyReallyPoor()
        {
            return new PlayerInfo("p2", 2);
        }
        public static PlayerInfo GenerateP2Poor()
        {
            return new PlayerInfo("p2", 1000);
        }
        public static PlayerInfo GenerateP3()
        {
            return new PlayerInfo("p3", 5000);
        }
        public static PlayerInfo GenerateP4()
        {
            return new PlayerInfo("p4", 5000);
        }
        public static PlayerInfo GenerateP1Seated(GameInfo nfo)
        {
            return nfo.SitInGame(GenerateP1());
        }
        public static PlayerInfo GenerateP2Seated(GameInfo nfo)
        {
            return nfo.SitInGame(GenerateP2());
        }
        public static PlayerInfo GenerateP2PoorSeated(GameInfo nfo)
        {
            return nfo.SitInGame(GenerateP2ReallyReallyPoor());
        }

        internal static PlayerInfo GenerateP3Seated(GameInfo nfo)
        {
            return nfo.SitInGame(GenerateP3());
        }

        internal static PlayerInfo GenerateP4Seated(GameInfo nfo)
        {
            return nfo.SitInGame(GenerateP4());
        }
    }
}
