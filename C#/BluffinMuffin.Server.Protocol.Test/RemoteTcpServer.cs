using System.Collections.Concurrent;
using System.Linq;
using System.Net.Sockets;
using BluffinMuffin.Protocol;
using Com.Ericmas001.Communication;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BluffinMuffin.Server.Protocol.Test
{
    public class RemoteTcpServer : RemoteTcpEntity 
    {
        public BlockingCollection<AbstractCommand> ReceivedCommands { get; } 
        public RemoteTcpServer(TcpClient remoteEntity) : base(remoteEntity)
        {
            ReceivedCommands = new BlockingCollection<AbstractCommand>();
        }

        protected override void OnDataReceived(string data)
        {
            ReceivedCommands.Add(AbstractCommand.DeserializeCommand(data));
        }

        protected override void OnDataSent(string data)
        {
        }

        public void Send(AbstractCommand command)
        {
            Send(command.Encode());
        }

        public T WaitForNextCommand<T>() where T:AbstractCommand
        {
            var r = ReceivedCommands.GetConsumingEnumerable().First();
            var response = r as T;
            Assert.IsNotNull(response);
            return response;
        }

        public string Name { get; set; }
    }
}
