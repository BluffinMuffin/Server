using System.Collections.Concurrent;

namespace BluffinMuffin.Server.DataTypes.Protocol
{
    public interface IBluffinServer
    {
        BlockingCollection<CommandEntry> LobbyCommands { get; }
        BlockingCollection<GameCommandEntry> GameCommands { get; }
    }
}
