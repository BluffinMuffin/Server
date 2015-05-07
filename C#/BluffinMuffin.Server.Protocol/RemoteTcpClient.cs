using System;
using System.Collections.Generic;
using System.Net.Sockets;
using BluffinMuffin.Protocol;
using BluffinMuffin.Protocol.Enums;
using BluffinMuffin.Server.Protocol.DataTypes;
using Com.Ericmas001.Net.Protocol;
using Com.Ericmas001.Util;

namespace BluffinMuffin.Server.Protocol
{
    public class RemoteTcpClient : RemoteTcpEntity, IBluffinClient
    {
        private readonly IBluffinServer m_BluffinServer;

        private readonly Dictionary<int, RemotePlayer> m_GamePlayers = new Dictionary<int, RemotePlayer>(); 

        public string PlayerName { get; set; }

        public RemoteTcpClient(TcpClient remoteEntity, IBluffinServer bluffinServer)
            : base(remoteEntity)
        {
            m_BluffinServer = bluffinServer;
        }

        protected override void OnDataReceived(string data)
        {
            if (!String.IsNullOrEmpty(data))
            {
                var command = AbstractBluffinCommand.DeserializeCommand(data);
                switch (command.CommandType)
                {
                    case BluffinCommandEnum.General:
                        m_BluffinServer.LobbyCommands.Add(new CommandEntry() { Client = this, Command = command });
                        lock (m_GamePlayers)
                        {
                            foreach(RemotePlayer p in m_GamePlayers.Values)
                                m_BluffinServer.GameCommands.Add(new GameCommandEntry() { Client = this, Command = command, Player = p });
                        }
                        break;
                    case BluffinCommandEnum.Lobby:
                        m_BluffinServer.LobbyCommands.Add(new CommandEntry() { Client = this, Command = command });
                        break;
                    case BluffinCommandEnum.Game:
                        var gc = (AbstractGameCommand) command;
                        lock (m_GamePlayers)
                        {
                            if(m_GamePlayers.ContainsKey(gc.TableId))
                                m_BluffinServer.GameCommands.Add(new GameCommandEntry() { Client = this, Command = command, Player = m_GamePlayers[gc.TableId] });
                        }
                        break;
                }
            }
        }

        protected override void OnDataSent(string data)
        {
        }

        public void OnConnectionLost()
        {
            OnDataReceived(new DisconnectCommand().Encode());
        }

        public void SendCommand(AbstractBluffinCommand command)
        {
            string line = command.Encode();
            LogManager.Log(LogLevel.MessageVeryLow, "ServerClientLobby.Send", "Server SEND to {0} [{1}]", PlayerName, line);
            LogManager.Log(LogLevel.MessageVeryLow, "ServerClientLobby.Send", "-------------------------------------------");
            Send(line);
        }

        public void AddPlayer(RemotePlayer p)
        {
            lock (m_GamePlayers)
            {
                m_GamePlayers.Add(p.TableId,p);
            }
        }

        public void RemovePlayer(RemotePlayer p)
        {
            lock (m_GamePlayers)
            {
                m_GamePlayers.Remove(p.TableId);
            }
        }
    }
}
