using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Protocol.DataTypes.Enums;
using BluffinMuffin.Protocol.Lobby;
using BluffinMuffin.Server.Protocol.DataTypes;
using BluffinMuffin.Server.Protocol.Workers;
using Com.Ericmas001.Util;
using System.Linq;
using System.Reflection;
using BluffinMuffin.Logger.DBAccess;
using BluffinMuffin.Protocol;
using BluffinMuffin.Server.Logic;

namespace BluffinMuffin.Server.Protocol
{
    public class BluffinServer : IBluffinServer, IBluffinLobby
    {
        public BlockingCollection<CommandEntry> LobbyCommands { get; }
        public BlockingCollection<GameCommandEntry> GameCommands { get; }

        public Logger.DBAccess.Server LogServer { get; }

        private readonly LocalTcpServer m_TcpServer;

        private readonly List<string> m_UsedNames = new List<string>();
        private readonly Dictionary<int, PokerGame> m_Games = new Dictionary<int, PokerGame>();
        private readonly Dictionary<int, Game> m_LogGames = new Dictionary<int, Game>();
        private readonly Dictionary<int, bool> m_LogGamesStatus = new Dictionary<int, bool>();
        private readonly Dictionary<int, Table> m_LogTables = new Dictionary<int, Table>();

        private int m_LastUsedId;

        public PokerGame GetGame(int id)
        {
            return m_Games[id];
        }
        public Game LogGame(int id)
        {
            return m_LogGames[id];
        }
        public void KillGame(int id)
        {
            m_LogGamesStatus[id] = false;
        }
        public void StartGame(int id)
        {
            if (!m_LogGamesStatus[id])
            {
                m_LogGamesStatus[id] = true;
                m_LogGames[id] = new Game(m_LogTables[id]);
                m_LogGames[id].RegisterGame();
            }
        }
        public Table LogTable(int id)
        {
            return m_LogTables[id];
        }

        public BluffinServer(int port)
        {
            LogManager.Log(LogLevel.Message, "BluffinServerLobby", "Server started on port {0} !", port);
            LogServer = new Logger.DBAccess.Server($"{Assembly.GetEntryAssembly().GetName().Name} {Assembly.GetEntryAssembly().GetName().Version.ToString(3)}",Assembly.GetAssembly(typeof(AbstractCommand)).GetName().Version);
            LogServer.RegisterServer();
            LobbyCommands = new BlockingCollection<CommandEntry>();
            GameCommands = new BlockingCollection<GameCommandEntry>();
            Task.Factory.StartNew(new BluffinLobbyWorker(this, this).Start);
            Task.Factory.StartNew(new BluffinGameWorker(this).Start);
            m_TcpServer = new LocalTcpServer(port, this);
        }

        public bool IsNameUsed(string name)
        {
            return m_UsedNames.Any(s => s.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }

        public void AddName(string name)
        {
            m_UsedNames.Add(name);
        }

        public void RemoveName(string name)
        {
            m_UsedNames.Remove(name);
        }

        public void Start()
        {
            m_TcpServer.Run().Wait();
        }

        public int CreateTable(CreateTableCommand c)
        {
            ListTables();

            m_LastUsedId++;
            while (m_Games.ContainsKey(m_LastUsedId))
                m_LastUsedId++;

            var game = new PokerGame(new PokerTable(c.Params));

            m_Games.Add(m_LastUsedId, game);

            var p = game.Table.Params;
            m_LogTables[m_LastUsedId] = new Table(p.TableName, (Logger.DBAccess.Enums.GameSubTypeEnum)Enum.Parse(typeof(Logger.DBAccess.Enums.GameSubTypeEnum), p.Variant.ToString()), p.MinPlayersToStart, p.MaxPlayers, (Logger.DBAccess.Enums.BlindTypeEnum)Enum.Parse(typeof(Logger.DBAccess.Enums.BlindTypeEnum), p.Blind.ToString()), (Logger.DBAccess.Enums.LobbyTypeEnum)Enum.Parse(typeof(Logger.DBAccess.Enums.LobbyTypeEnum), p.Lobby.OptionType.ToString()), (Logger.DBAccess.Enums.LimitTypeEnum)Enum.Parse(typeof(Logger.DBAccess.Enums.LimitTypeEnum), p.Limit.ToString()), LogServer);
            m_LogTables[m_LastUsedId].RegisterTable();
            m_LogGamesStatus[m_LastUsedId] = false;
            StartGame(m_LastUsedId);
            game.Start();

            return m_LastUsedId;
        }

        public List<TupleTable> ListTables(params LobbyTypeEnum[] lobbyTypes)
        {
            // Remove non-running tables
            m_Games.Where(kvp => !kvp.Value.IsRunning).Select(kvp => kvp.Key).ToList().ForEach(i => m_Games.Remove(i));

            //List Tables
            return (from kvp in m_Games.Where(kvp => kvp.Value.IsRunning)
                let t = kvp.Value.Table
                where lobbyTypes.Length == 0 || lobbyTypes.Contains(t.Params.Lobby.OptionType)
                select new TupleTable()
                {
                    IdTable = kvp.Key, 
                    Params = t.Params, 
                    NbPlayers = t.Players.Count, 
                    PossibleAction = LobbyActionEnum.None,
                }).ToList();
        }
    }
}
