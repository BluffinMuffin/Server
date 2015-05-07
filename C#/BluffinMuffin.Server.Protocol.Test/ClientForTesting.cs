using System.Collections.Concurrent;
using System.Linq;
using System.Net.Sockets;
using Com.Ericmas001.Net.Protocol;

namespace BluffinMuffin.Server.Protocol.Test
{
    public class ClientForTesting : SimpleTcpClient
    {
        private BlockingCollection<RemoteTcpServer> m_Servers = new BlockingCollection<RemoteTcpServer>();
        public RemoteTcpServer ObtainTcpEntity()
        {
            return m_Servers.GetConsumingEnumerable().First();
        }
        public ClientForTesting() : base("127.0.0.1", 42084)
        {
        }

        protected override RemoteTcpEntity CreateServer(TcpClient tcpClient)
        {
            var server = new RemoteTcpServer(tcpClient);
            m_Servers.Add(server);
            m_Servers.CompleteAdding();
            return server;
        }

        protected override void OnServerConnected(RemoteTcpEntity client)
        {
        }

        protected override void OnServerDisconnected(RemoteTcpEntity client)
        {
        }
    }
}
