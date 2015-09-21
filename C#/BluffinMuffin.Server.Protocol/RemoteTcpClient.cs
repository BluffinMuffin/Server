using System.Collections.Generic;
using System.Net.Sockets;
using System.Reflection;
using BluffinMuffin.Logger.DBAccess;
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

        public Client LogClient { get; }

        public RemoteTcpClient(TcpClient remoteEntity, IBluffinServer bluffinServer)
            : base(remoteEntity)
        {
            m_BluffinServer = bluffinServer;
            LogClient = new Client($"{Assembly.GetEntryAssembly().GetName().Name} {Assembly.GetEntryAssembly().GetName().Version.ToString(3)}", Assembly.GetAssembly(typeof(BluffinMuffin.Protocol.AbstractCommand)).GetName().Version, remoteEntity.Client.RemoteEndPoint.ToString());
            LogClient.RegisterClient();
        }

        protected override void OnDataReceived(string data)
        {
            if (!string.IsNullOrEmpty(data))
            {
                var command = BluffinMuffin.Protocol.AbstractCommand.DeserializeCommand(data);
                switch (command.CommandType)
                {
                    case BluffinCommandEnum.General:
                        Command.RegisterGeneralCommandFromClient(command.CommandName,m_BluffinServer.LogServer,LogClient,data);
                        m_BluffinServer.LobbyCommands.Add(new CommandEntry() { Client = this, Command = command });
                        lock (m_GamePlayers)
                        {
                            foreach(RemotePlayer p in m_GamePlayers.Values)
                                m_BluffinServer.GameCommands.Add(new GameCommandEntry() { Client = this, Command = command, Player = p });
                        }
                        break;
                    case BluffinCommandEnum.Lobby:
                        Command.RegisterLobbyCommandFromClient(command.CommandName, m_BluffinServer.LogServer, LogClient, data);
                        m_BluffinServer.LobbyCommands.Add(new CommandEntry() { Client = this, Command = command });
                        break;
                    case BluffinCommandEnum.Game:
                        var gc = (AbstractGameCommand) command;
                        lock (m_GamePlayers)
                        {
                            if (m_GamePlayers.ContainsKey(gc.TableId))
                            {
                                Command.RegisterGameCommandFromClient(command.CommandName, m_BluffinServer.LogGame(gc.TableId), LogClient, data);
                                m_BluffinServer.GameCommands.Add(new GameCommandEntry() {Client = this, Command = command, Player = m_GamePlayers[gc.TableId]});
                            }
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
            string line = command.Encode();
            switch (command.CommandType)
            {
                case BluffinCommandEnum.General:
                    Command.RegisterGeneralCommandFromServer(command.CommandName, m_BluffinServer.LogServer, LogClient, line);
                    break;
                case BluffinCommandEnum.Lobby:
                    Command.RegisterLobbyCommandFromServer(command.CommandName, m_BluffinServer.LogServer, LogClient, line);
                    break;
                case BluffinCommandEnum.Game:
                    break;
            }
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
