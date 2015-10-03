using System;
using BluffinMuffin.Protocol;
using BluffinMuffin.Server.DataTypes;
using BluffinMuffin.Server.DataTypes.Protocol;

namespace BluffinMuffin.Server.Protocol.Test.Mocking
{
    public class ClientMock : IBluffinClient
    {
        private readonly ServerMock m_Server;
        public ClientMock(ServerMock server)
        {
            m_Server = server;
            PlayerName = "SpongeBob";
        }

        public string PlayerName { get; set; }
        public void SendCommand(AbstractCommand command)
        {
            m_Server.ServerSendedCommands.Add(new CommandEntry() { Client = this, Command = command });
        }

        public void AddPlayer(IPokerPlayer p)
        {
            throw new NotImplementedException();
        }

        public void RemovePlayer(IPokerPlayer p)
        {
            throw new NotImplementedException();
        }
    }
}
