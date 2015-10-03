using System;
using System.Collections.Generic;
using BluffinMuffin.Server.Protocol;
using Com.Ericmas001.Util;
using System.IO;
using System.Reflection;
using BluffinMuffin.Logger.DBAccess;
using BluffinMuffin.Protocol;
using BluffinMuffin.Protocol.Enums;
using BluffinMuffin.Server.DataTypes.Protocol;

namespace BluffinMuffin.Server
{
    public static class Program
    {
        static StreamWriter m_SwNormal;
        static StreamWriter m_SwDebug;
        static StreamWriter m_SwVerbose;
        private static BluffinServer m_Server;
        private static Logger.DBAccess.Server m_LogServer;
        private static readonly Dictionary<int, Table> m_LogTables = new Dictionary<int, Table>();
        private static readonly Dictionary<int, Game> m_LogGames = new Dictionary<int, Game>();
        private static readonly Dictionary<int, bool> m_LogGamesStatus = new Dictionary<int, bool>();
        private static readonly Dictionary<IBluffinClient, Client> m_LogClients = new Dictionary<IBluffinClient, Client>();
        static void Main(string[] args)
        {

            Database.InitDatabase("turnsol.arvixe.com", "BluffinMuffin_Logger_Test", "1ti3gre2", "BluffinMuffin_Logs_Test");

            m_LogServer = new Logger.DBAccess.Server($"{Assembly.GetEntryAssembly().GetName().Name} {Assembly.GetEntryAssembly().GetName().Version.ToString(3)}", Assembly.GetAssembly(typeof(AbstractCommand)).GetName().Version);
            m_LogServer.RegisterServer();

            DataTypes.Logger.CommandSent += OnLogCommandSent;
            DataTypes.Logger.CommandReceived += OnLogCommandReceived;
            DataTypes.Logger.TableCreated += OnLogTableCreated;
            DataTypes.Logger.GameCreated += OnLogGameCreated;
            DataTypes.Logger.GameEnded += OnLogGameEnded;
            DataTypes.Logger.ClientCreated += OnLogClientCreated;

            LogManager.MessageLogged += LogManager_MessageLogged;
            if ((args.Length % 2) == 0)
            {
                try
                {

                    var map = new Dictionary<string, string>();
                    for (var i = 0; i < args.Length; i += 2)
                        map.Add(args[i].ToLower(), args[i + 1]);
                    var port = 4242;
                    if (map.ContainsKey("-p"))
                        port = int.Parse(map["-p"]);
                    if(map.ContainsKey("-log"))
                    {
                    var uri = new Uri(Assembly.GetExecutingAssembly().CodeBase);
                    var path = Path.GetDirectoryName(uri.LocalPath + uri.Fragment) + "\\log";
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                        var logName = DateTime.Now.ToString("yyyy-MM-dd.HH-mm-ss");
                        var logType = map["-log"];
                        if(logType == "normal" || logType == "debug" || logType == "verbose")
                        {
                            m_SwNormal = File.CreateText(path + "\\server." + logName + ".normal.txt");
                            m_SwNormal.AutoFlush = true;
                            LogManager.MessageLogged += LogManager_MessageLoggedToFileNormal;
                            if (logType == "debug" || logType == "verbose")
                            {
                                m_SwDebug = File.CreateText(path + "\\server." + logName + ".debug.txt");
                                m_SwDebug.AutoFlush = true;
                                LogManager.MessageLogged += LogManager_MessageLoggedToFileDebug;
                                if (logType == "verbose")
                                {
                                    m_SwVerbose = File.CreateText(path + "\\server." + logName + ".verbose.txt");
                                    m_SwVerbose.AutoFlush = true;
                                    LogManager.MessageLogged += LogManager_MessageLoggedToFileVerbose;
                                }
                            }
                        }

                    }
                    m_Server = new BluffinServer(port);
                    m_Server.Start();
                    LogManager.Log(LogLevel.Message, "BluffinMuffin.Server", "Server started on port {0}", port);
                }
                catch
                {
                    LogManager.Log(LogLevel.Error, "Program.Main", "Can't start server !!");
                }
            }
            else
                LogManager.Log(LogLevel.Error, "Program.Main", "Incorrect number of application arguments");
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
            if (!m_LogGamesStatus[e.Id])
            {
                m_LogGamesStatus[e.Id] = true;
                m_LogGames[e.Id] = new Game(m_LogTables[e.Id]);
                m_LogGames[e.Id].RegisterGame();
            }
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

        private static void LogManager_MessageLogged(string from, string message, int level)
        {
            // ATTENTION: This must contain "LogLevel.Message" for RELEASE
            //                              "LogLevel.MessageLow" for DEBUGGING
            //                              "LogLevel.MessageVeryLow" for XTREM DEBUGGING
            LogManager.LogInConsole(from, message, level, LogLevel.MessageVeryLow);
        }

        private static void LogManager_MessageLoggedToFileNormal(string from, string message, int level)
        {
            LogManager.LogInFile(m_SwNormal, from, message, level, LogLevel.Message);
        }

        private static void LogManager_MessageLoggedToFileDebug(string from, string message, int level)
        {
            LogManager.LogInFile(m_SwDebug, from, message, level, LogLevel.MessageLow);
        }

        private static void LogManager_MessageLoggedToFileVerbose(string from, string message, int level)
        {
            LogManager.LogInFile(m_SwVerbose, from, message, level, LogLevel.MessageVeryLow);
        }
    }
}
