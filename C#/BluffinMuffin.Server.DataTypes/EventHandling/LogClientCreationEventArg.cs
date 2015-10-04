using System.Net.Sockets;
using BluffinMuffin.Server.DataTypes.Protocol;

namespace BluffinMuffin.Server.DataTypes.EventHandling
{
    public class LogClientCreationEventArg : LogClientEventArg
    {
        public LogClientCreationEventArg(TcpClient endpoint, IBluffinClient client) : base(client)
        {
            Endpoint = endpoint;
        }

        public TcpClient Endpoint { get; }
    }
}
