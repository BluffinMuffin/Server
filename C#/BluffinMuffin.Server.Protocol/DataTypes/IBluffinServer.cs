using System.Collections.Concurrent;

namespace BluffinMuffin.Server.Protocol.DataTypes
{
    public interface IBluffinServer
    {
        BlockingCollection<CommandEntry> LobbyCommands { get; }
        BlockingCollection<GameCommandEntry> GameCommands { get; } 
    }
}
