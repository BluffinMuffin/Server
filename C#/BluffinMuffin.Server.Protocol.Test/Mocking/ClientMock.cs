using System;
using BluffinMuffin.Logger.DBAccess;
using BluffinMuffin.Protocol;
using BluffinMuffin.Server.Protocol.DataTypes;

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

        public void AddPlayer(RemotePlayer p)
        {
            throw new NotImplementedException();
        }

        public void RemovePlayer(RemotePlayer p)
        {
            throw new NotImplementedException();
        }

        public Client LogClient => null;
    }
}
