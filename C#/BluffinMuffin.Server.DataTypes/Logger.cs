using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using BluffinMuffin.Protocol;
using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Server.DataTypes.EventHandling;
using BluffinMuffin.Server.DataTypes.Protocol;

namespace BluffinMuffin.Server.DataTypes
{
    public class Logger
    {
        public static event EventHandler<LogCommandEventArg> CommandSent = delegate { };
        public static event EventHandler<LogCommandEventArg> CommandReceived = delegate { };
        public static event EventHandler<LogTableCreationEventArg> TableCreated = delegate { };
        public static event EventHandler<LogGameEventArg> GameCreated = delegate { };
        public static event EventHandler<LogGameEventArg> GameEnded = delegate { };
        public static event EventHandler<LogClientCreationEventArg> ClientCreated = delegate { };
        public static event EventHandler<LogClientEventArg> ClientIdentified = delegate { };

        public static void LogCommandSent(object sender, AbstractCommand cmd, IBluffinClient cli, string commandData)
        {
            CommandSent(sender, new LogCommandEventArg(cmd, commandData, cli));
        }
        public static void LogCommandReceived(object sender, AbstractCommand cmd, IBluffinClient cli, string commandData)
        {
            CommandReceived(sender, new LogCommandEventArg(cmd, commandData, cli ));
        }
        public static void LogTableCreated(object sender, int id, TableParams p)
        {
            TableCreated(sender, new LogTableCreationEventArg(id, p));
        }
        public static void LogGameCreated(object sender, int id)
        {
            GameCreated(sender, new LogGameEventArg(id));
        }
        public static void LogGameEnded(object sender, int id)
        {
            GameEnded(sender, new LogGameEventArg(id));
        }
        public static void LogClientCreated(object sender, TcpClient endpoint, IBluffinClient client)
        {
            ClientCreated(sender, new LogClientCreationEventArg(endpoint, client));
        }
        public static void LogClientIdentified(object sender, IBluffinClient client)
        {
            ClientIdentified(sender, new LogClientEventArg(client));
        }
    }
}
