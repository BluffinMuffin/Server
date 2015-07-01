using BluffinMuffin.Protocol;

namespace BluffinMuffin.Server.Protocol.DataTypes
{
    public interface IBluffinClient
    {
        string PlayerName { get; set; }

        void SendCommand(AbstractCommand command);

        void AddPlayer(RemotePlayer p);
        void RemovePlayer(RemotePlayer p);
    }
}
