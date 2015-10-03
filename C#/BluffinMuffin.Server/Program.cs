using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using BluffinMuffin.Server.Protocol;
using System.IO;
using System.Reflection;
using BluffinMuffin.Logger.DBAccess;
using BluffinMuffin.Protocol;
using BluffinMuffin.Protocol.Enums;
using BluffinMuffin.Server.Configuration;
using BluffinMuffin.Server.DataTypes.Protocol;
using Com.Ericmas001.Portable.Util;

namespace BluffinMuffin.Server
{
    public static class Program
    {
        private static BluffinMuffinDataSection m_Config;
        static StreamWriter m_SwNormal;
        static StreamWriter m_SwDebug;
        static StreamWriter m_SwVerbose;
        private static BluffinServer m_Server;
        private static Logger.DBAccess.Server m_LogServer;
        private static readonly Dictionary<int, Table> m_LogTables = new Dictionary<int, Table>();
        private static readonly Dictionary<int, Game> m_LogGames = new Dictionary<int, Game>();
        private static readonly Dictionary<int, bool> m_LogGamesStatus = new Dictionary<int, bool>();
        private static readonly Dictionary<IBluffinClient, Client> m_LogClients = new Dictionary<IBluffinClient, Client>();

        private static void Main()
        {
            InitConfiguration();

            InitDbLogging();

            RegisterToLogEvents();

            InitConsoleLogging();

            InitFileLogging();

            RegisterServerOnDb();

            StartServer();
        }

        private static void StartServer()
        {
            try
            {
                m_Server = new BluffinServer(m_Config.Port);
                m_Server.Start();
            }
            catch
            {
                DataTypes.Logger.LogError("Can't start server !!");
            }
        }

        private static void RegisterServerOnDb()
        {
            if (!m_Config.Logging.DbCommand.HasIt)
                return;

            m_LogServer = new Logger.DBAccess.Server($"{Assembly.GetEntryAssembly().GetName().Name} {Assembly.GetEntryAssembly().GetName().Version.ToString(3)}", Assembly.GetAssembly(typeof (AbstractCommand)).GetName().Version);
            m_LogServer.RegisterServer();
        }

        private static void InitFileLogging()
        {
            var uri = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            var path = Path.GetDirectoryName(uri.LocalPath + uri.Fragment) + "\\log";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var logName = DateTime.Now.ToString("yyyy-MM-dd.HH-mm-ss");
            switch (m_Config.Logging.File.Level)
            {
                case "VERBOSE":
                    m_SwVerbose = File.CreateText(path + "\\server." + logName + ".verbose.txt");
                    m_SwVerbose.AutoFlush = true;
                    LogManager.MessageLogged += (from, message, level) => LogManager.LogInFile(m_SwVerbose, @from, message, level, LogLevel.MessageVeryLow);
                    goto case "DEBUG";
                case "DEBUG":
                    m_SwDebug = File.CreateText(path + "\\server." + logName + ".debug.txt");
                    m_SwDebug.AutoFlush = true;
                    LogManager.MessageLogged += (from, message, level) => LogManager.LogInFile(m_SwDebug, @from, message, level, LogLevel.MessageLow);
                    goto case "NORMAL";
                case "NORMAL":
                    m_SwNormal = File.CreateText(path + "\\server." + logName + ".normal.txt");
                    m_SwNormal.AutoFlush = true;
                    LogManager.MessageLogged += (from, message, level) => LogManager.LogInFile(m_SwNormal, @from, message, level, LogLevel.Message);
                    break;
            }
        }

        private static void InitConsoleLogging()
        {
            switch (m_Config.Logging.Console.Level)
            {
                case "VERBOSE":
                    LogManager.MessageLogged += (from, message, level) => LogInConsole(@from, message, level, LogLevel.MessageVeryLow);
                    break;
                case "DEBUG":
                    LogManager.MessageLogged += (from, message, level) => LogInConsole(@from, message, level, LogLevel.MessageLow);
                    break;
                case "NORMAL":
                    LogManager.MessageLogged += (from, message, level) => LogInConsole(@from, message, level, LogLevel.Message);
                    break;
                default:
                    LogManager.MessageLogged += (from, message, level) => LogInConsole(@from, message, level, LogLevel.WarningLow);
                    break;
            }
        }

        private static void RegisterToLogEvents()
        {
            DataTypes.Logger.CommandSent += OnLogCommandSent;
            DataTypes.Logger.CommandReceived += OnLogCommandReceived;
            DataTypes.Logger.TableCreated += OnLogTableCreated;
            DataTypes.Logger.GameCreated += OnLogGameCreated;
            DataTypes.Logger.GameEnded += OnLogGameEnded;
            DataTypes.Logger.ClientCreated += OnLogClientCreated;
            DataTypes.Logger.DebugInformationLogged += OnDebugInformationLogged;
            DataTypes.Logger.InformationLogged += OnInformationLogged;
            DataTypes.Logger.WarningLogged += OnWarningLogged;
            DataTypes.Logger.ErrorLogged += OnErrorLogged;
        }

        private static void InitDbLogging()
        {
            if (m_Config.Logging.DbCommand.HasIt)
                Database.InitDatabase(m_Config.Logging.DbCommand.Url, m_Config.Logging.DbCommand.User, m_Config.Logging.DbCommand.Password, m_Config.Logging.DbCommand.Database);
        }

        private static void InitConfiguration()
        {
            m_Config = ConfigurationManager.GetSection(BluffinMuffinDataSection.NAME) as BluffinMuffinDataSection;
            if (m_Config == null)
                throw new Exception("No configuration found !!!");
        }

        private static string GetCaller(object sender)
        {
            var sf = sender as StackFrame;
            if (sf == null)
                return sender?.ToString() ?? string.Empty;
            return $"{sf.GetMethod().DeclaringType?.FullName}.{sf.GetMethod().Name}";
        }

        private static void OnDebugInformationLogged(object sender, Com.Ericmas001.Net.Protocol.StringEventArgs e)
        {
            LogManager.Log(LogLevel.MessageLow, GetCaller(sender), e.Str);
        }
        private static void OnInformationLogged(object sender, Com.Ericmas001.Net.Protocol.StringEventArgs e)
        {
            LogManager.Log(LogLevel.Message, GetCaller(sender), e.Str);
        }
        private static void OnWarningLogged(object sender, Com.Ericmas001.Net.Protocol.StringEventArgs e)
        {
            LogManager.Log(LogLevel.Warning, GetCaller(sender), e.Str);
        }
        private static void OnErrorLogged(object sender, Com.Ericmas001.Net.Protocol.StringEventArgs e)
        {
            LogManager.Log(LogLevel.Error, GetCaller(sender), e.Str);
        }

        private static void OnLogClientCreated(object sender, DataTypes.EventHandling.LogClientCreationEventArg e)
        {
            if (!m_Config.Logging.DbCommand.HasIt)
                return;

            m_LogClients[e.Client] = new Client(e.Endpoint.Client.RemoteEndPoint.ToString());
            m_LogClients[e.Client].RegisterClient();
        }

        private static void OnLogGameEnded(object sender, DataTypes.EventHandling.LogGameEventArg e)
        {
            if (!m_Config.Logging.DbCommand.HasIt)
                return;

            m_LogGamesStatus[e.Id] = false;
        }

        private static void OnLogGameCreated(object sender, DataTypes.EventHandling.LogGameEventArg e)
        {
            if (!m_Config.Logging.DbCommand.HasIt)
                return;

            if (m_LogGamesStatus[e.Id])
                return;

            m_LogGamesStatus[e.Id] = true;
            m_LogGames[e.Id] = new Game(m_LogTables[e.Id]);
            m_LogGames[e.Id].RegisterGame();
        }

        private static void OnLogTableCreated(object sender, DataTypes.EventHandling.LogTableCreationEventArg e)
        {
            if (!m_Config.Logging.DbCommand.HasIt)
                return;

            var p = e.Params;
            m_LogTables[e.Id] = new Table(p.TableName, (Logger.DBAccess.Enums.GameSubTypeEnum)Enum.Parse(typeof(Logger.DBAccess.Enums.GameSubTypeEnum), p.Variant.ToString()), p.MinPlayersToStart, p.MaxPlayers, (Logger.DBAccess.Enums.BlindTypeEnum)Enum.Parse(typeof(Logger.DBAccess.Enums.BlindTypeEnum), p.Blind.ToString()), (Logger.DBAccess.Enums.LobbyTypeEnum)Enum.Parse(typeof(Logger.DBAccess.Enums.LobbyTypeEnum), p.Lobby.OptionType.ToString()), (Logger.DBAccess.Enums.LimitTypeEnum)Enum.Parse(typeof(Logger.DBAccess.Enums.LimitTypeEnum), p.Limit.ToString()), m_LogServer);
            m_LogTables[e.Id].RegisterTable();
            m_LogGamesStatus[e.Id] = false;
        }

        private static void OnLogCommandSent(object sender, DataTypes.EventHandling.LogCommandEventArg e)
        {
            LogManager.Log(LogLevel.MessageVeryLow, GetCaller(sender), "Server SEND to {0} [{1}]", m_LogClients[e.Client].DisplayName, e.CommandData);
            LogManager.Log(LogLevel.MessageVeryLow, GetCaller(sender), "-------------------------------------------");

            if (!m_Config.Logging.DbCommand.HasIt)
                return;

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
            LogManager.Log(LogLevel.MessageVeryLow, GetCaller(sender), "Server RECV from {0} [{1}]", m_LogClients[e.Client].DisplayName, e.CommandData);
            LogManager.Log(LogLevel.MessageVeryLow, GetCaller(sender), "-------------------------------------------");

            if (!m_Config.Logging.DbCommand.HasIt)
                return;

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

        public static void LogInConsole(string from, string message, int level, LogLevel minLevelToLog)
        {
            var fc = Console.ForegroundColor;
            var bc = Console.BackgroundColor;


            //Errors
            if (level >= (int)LogLevel.ErrorHigh)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.White;
                message = "ERROR: " + message;
            }
            if (level >= (int)LogLevel.Error)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                message = "ERROR: " + message;
            }
            else if (level >= (int)LogLevel.ErrorLow)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
            }


            //Warnings
            else if (level >= (int)LogLevel.Warning)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                message = "WARNING: " + message;
            }
            else if (level >= (int)LogLevel.WarningLow)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
            }


            //Messages
            else if (level >= (int)LogLevel.MessageVeryHigh)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                message = "IMPORTANT: " + message;
            }
            else if (level >= (int)LogLevel.Message)
            {
                Console.ForegroundColor = ConsoleColor.White;
            }
            else if (level >= (int)LogLevel.MessageLow)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                message = "DEBUG: " + message;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                message = "DEBUG: " + message;
            }


            //Let's Log!
            if (level >= (int)minLevelToLog)
                Console.WriteLine(message);

            Console.ForegroundColor = fc;
            Console.BackgroundColor = bc;
        }
    }
}
