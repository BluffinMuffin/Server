using System.Collections.Concurrent;
using System.Linq;
using System.Net.Sockets;
using BluffinMuffin.Protocol;
using Com.Ericmas001.Net.Protocol;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BluffinMuffin.Server.Protocol.Test
{
    public class RemoteTcpServer : RemoteTcpEntity 
    {
        public BlockingCollection<AbstractBluffinCommand> ReceivedCommands { get; private set; } 
        public RemoteTcpServer(TcpClient remoteEntity) : base(remoteEntity)
        {
            ReceivedCommands = new BlockingCollection<AbstractBluffinCommand>();
        }

        protected override void OnDataReceived(string data)
        {
            ReceivedCommands.Add(AbstractBluffinCommand.DeserializeCommand(data));
        }

        protected override void OnDataSent(string data)
        {
        }

        public void Send(AbstractBluffinCommand command)
        {
            Send(command.Encode());
        }

        public T WaitForNextCommand<T>() where T:AbstractBluffinCommand
        {
            var r = ReceivedCommands.GetConsumingEnumerable().First();
            var response = r as T;
            Assert.IsNotNull(response);
            return response;
        }

        public string Name { get; set; }
    }
}
