using System.Collections.Generic;
using System.Net.Sockets;
using BluffinMuffin.Protocol;
using BluffinMuffin.Protocol.Enums;
using BluffinMuffin.Server.DataTypes;
using BluffinMuffin.Server.DataTypes.Protocol;
using Com.Ericmas001.Net.Protocol;

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
            Logger.LogClientCreated(remoteEntity, this);
        }

        protected override void OnDataReceived(string data)
        {
            if (!string.IsNullOrEmpty(data))
            {
                var command = BluffinMuffin.Protocol.AbstractCommand.DeserializeCommand(data);
                Logger.LogCommandReceived(command, this, data);
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
                            if (m_GamePlayers.ContainsKey(gc.TableId))
                                m_BluffinServer.GameCommands.Add(new GameCommandEntry() {Client = this, Command = command, Player = m_GamePlayers[gc.TableId]});
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

        public void SendCommand(BluffinMuffin.Protocol.AbstractCommand command)
        {
            var line = command.Encode();
            Logger.LogCommandSent(command, this, line);
            Send(line);
        }

        public void AddPlayer(IPokerPlayer p)
        {
            lock (m_GamePlayers)
            {
                m_GamePlayers.Add(p.TableId,(RemotePlayer)p);
            }
        }

        public void RemovePlayer(IPokerPlayer p)
        {
            lock (m_GamePlayers)
            {
                m_GamePlayers.Remove(p.TableId);
            }
        }
    }
}
