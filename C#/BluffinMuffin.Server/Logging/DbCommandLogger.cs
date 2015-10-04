using System;
using System.Collections.Generic;
using System.Reflection;
using BluffinMuffin.Logger.DBAccess;
using BluffinMuffin.Protocol;
using BluffinMuffin.Protocol.Enums;
using BluffinMuffin.Server.Configuration;
using BluffinMuffin.Server.DataTypes.Protocol;

namespace BluffinMuffin.Server.Logging
{
    public class DbCommandLogger
    {
        private static Logger.DBAccess.Server m_LogServer;
        private static readonly Dictionary<int, Table> m_LogTables = new Dictionary<int, Table>();
        private static readonly Dictionary<int, Game> m_LogGames = new Dictionary<int, Game>();
        private static readonly Dictionary<int, bool> m_LogGamesStatus = new Dictionary<int, bool>();
        private static readonly Dictionary<IBluffinClient, Client> m_LogClients = new Dictionary<IBluffinClient, Client>();

        public static void Init(BluffinMuffinDataSection config)
        {
            if (!config.Logging.DbCommand.HasIt)
                return;

            Database.InitDatabase(config.Logging.DbCommand.Url, config.Logging.DbCommand.User, config.Logging.DbCommand.Password, config.Logging.DbCommand.Database);
            
            DataTypes.Logger.CommandSent += OnLogCommandSent;
            DataTypes.Logger.CommandReceived += OnLogCommandReceived;
            DataTypes.Logger.TableCreated += OnLogTableCreated;
            DataTypes.Logger.GameCreated += OnLogGameCreated;
            DataTypes.Logger.GameEnded += OnLogGameEnded;
            DataTypes.Logger.ClientCreated += OnLogClientCreated;

            m_LogServer = new Logger.DBAccess.Server($"{Assembly.GetEntryAssembly().GetName().Name} {Assembly.GetEntryAssembly().GetName().Version.ToString(3)}", Assembly.GetAssembly(typeof(AbstractCommand)).GetName().Version);
            m_LogServer.RegisterServer();

        }

        private static void OnLogClientCreated(object sender, DataTypes.EventHandling.LogClientCreationEventArg e)
        {
            m_LogClients[e.Client] = new Client(e.Endpoint.Client.RemoteEndPoint.ToString());
            m_LogClients[e.Client].RegisterClient();
        }

        private static void OnLogGameEnded(object sender, DataTypes.EventHandling.LogGameEventArg e)
        {
            m_LogGamesStatus[e.Id] = false;
        }

        private static void OnLogGameCreated(object sender, DataTypes.EventHandling.LogGameEventArg e)
        {
            if (m_LogGamesStatus[e.Id])
                return;

            m_LogGamesStatus[e.Id] = true;
            m_LogGames[e.Id] = new Game(m_LogTables[e.Id]);
            m_LogGames[e.Id].RegisterGame();
        }

        private static void OnLogTableCreated(object sender, DataTypes.EventHandling.LogTableCreationEventArg e)
        {
            var p = e.Params;
            m_LogTables[e.Id] = new Table(p.TableName, (Logger.DBAccess.Enums.GameSubTypeEnum)Enum.Parse(typeof(Logger.DBAccess.Enums.GameSubTypeEnum), p.Variant.ToString()), p.MinPlayersToStart, p.MaxPlayers, (Logger.DBAccess.Enums.BlindTypeEnum)Enum.Parse(typeof(Logger.DBAccess.Enums.BlindTypeEnum), p.Blind.ToString()), (Logger.DBAccess.Enums.LobbyTypeEnum)Enum.Parse(typeof(Logger.DBAccess.Enums.LobbyTypeEnum), p.Lobby.OptionType.ToString()), (Logger.DBAccess.Enums.LimitTypeEnum)Enum.Parse(typeof(Logger.DBAccess.Enums.LimitTypeEnum), p.Limit.ToString()), m_LogServer);
            m_LogTables[e.Id].RegisterTable();
            m_LogGamesStatus[e.Id] = false;
        }

        private static void OnLogCommandSent(object sender, DataTypes.EventHandling.LogCommandEventArg e)
        {
            switch (e.Command.CommandType)
            {
                case BluffinCommandEnum.General:
                    Command.RegisterGeneralCommandFromServer(e.Command.CommandName, m_LogServer, m_LogClients[e.Client], e.CommandData);
                    break;
                case BluffinCommandEnum.Lobby:
                    Command.RegisterLobbyCommandFromServer(e.Command.CommandName, m_LogServer, m_LogClients[e.Client], e.CommandData);
                    break;
                case BluffinCommandEnum.Game:
                    Command.RegisterGameCommandFromServer(e.Command.CommandName, m_LogGames[((IGameCommand)e.Command).TableId], m_LogClients[e.Client], e.CommandData);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void OnLogCommandReceived(object sender, DataTypes.EventHandling.LogCommandEventArg e)
        {
            switch (e.Command.CommandType)
            {
                case BluffinCommandEnum.General:
                    Command.RegisterGeneralCommandFromClient(e.Command.CommandName, m_LogServer, m_LogClients[e.Client], e.CommandData);
                    break;
                case BluffinCommandEnum.Lobby:
                    Command.RegisterLobbyCommandFromClient(e.Command.CommandName, m_LogServer, m_LogClients[e.Client], e.CommandData);
                    break;
                case BluffinCommandEnum.Game:
                    Command.RegisterGameCommandFromClient(e.Command.CommandName, m_LogGames[((IGameCommand)e.Command).TableId], m_LogClients[e.Client], e.CommandData);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
