using System.Collections.Concurrent;
using BluffinMuffin.Logger.DBAccess;

namespace BluffinMuffin.Server.Protocol.DataTypes
{
    public interface IBluffinServer
    {
        BlockingCollection<CommandEntry> LobbyCommands { get; }
        BlockingCollection<GameCommandEntry> GameCommands { get; }

        Logger.DBAccess.Server LogServer { get; }
        Game LogGame(int id);
        Table LogTable(int id);
        void KillGame(int id);
        void StartGame(int id);
    }
}
