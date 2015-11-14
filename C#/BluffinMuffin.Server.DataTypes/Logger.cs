using System;
using System.Diagnostics;
using System.Net.Sockets;
using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Server.DataTypes.EventHandling;
using BluffinMuffin.Server.DataTypes.Protocol;
using Com.Ericmas001.Net.Protocol;
using AbstractCommand = BluffinMuffin.Protocol.AbstractCommand;

namespace BluffinMuffin.Server.DataTypes
{
    public static class Logger
    {
        public static event EventHandler<StringEventArgs> VerboseInformationLogged = delegate { };
        public static event EventHandler<StringEventArgs> DebugInformationLogged = delegate { };
        public static event EventHandler<StringEventArgs> InformationLogged = delegate { };
        public static event EventHandler<StringEventArgs> WarningLogged = delegate { };
        public static event EventHandler<StringEventArgs> ErrorLogged = delegate { };

        public static event EventHandler<StringEventArgs> MessageLogged = delegate { };

        public static event EventHandler<LogCommandEventArg> CommandSent = delegate { };
        public static event EventHandler<LogCommandEventArg> CommandReceived = delegate { };
        public static event EventHandler<LogTableCreationEventArg> TableCreated = delegate { };
        public static event EventHandler<LogGameEventArg> GameCreated = delegate { };
        public static event EventHandler<LogGameEventArg> GameEnded = delegate { };
        public static event EventHandler<LogClientCreationEventArg> ClientCreated = delegate { };
        public static event EventHandler<LogClientEventArg> ClientIdentified = delegate { };

        public static void LogVerboseInformation(string message, params object[] args)
        {
            MessageLogged(new StackFrame(1), new StringEventArgs(string.Format(message, args)));
            VerboseInformationLogged(new StackFrame(1), new StringEventArgs(string.Format(message, args)));
        }
        public static void LogDebugInformation(string message, params object[] args)
        {
            MessageLogged(new StackFrame(1), new StringEventArgs(string.Format(message, args)));
            DebugInformationLogged(new StackFrame(1), new StringEventArgs(string.Format(message, args)));
        }
        public static void LogInformation(string message, params object[] args)
        {
            MessageLogged(new StackFrame(1), new StringEventArgs(string.Format(message, args)));
            InformationLogged(new StackFrame(1), new StringEventArgs(string.Format(message, args)));
        }
        public static void LogWarning(string message, params object[] args)
        {
            MessageLogged(new StackFrame(1), new StringEventArgs(string.Format(message, args)));
            WarningLogged(new StackFrame(1), new StringEventArgs(string.Format(message, args)));
        }
        public static void LogError(string message, params object[] args)
        {
            MessageLogged(new StackFrame(1), new StringEventArgs(string.Format(message, args)));
            ErrorLogged(new StackFrame(1), new StringEventArgs(string.Format(message,args)));
        }

        public static void LogCommandSent(AbstractCommand cmd, IBluffinClient cli, string commandData)
        {
            CommandSent(new StackFrame(1), new LogCommandEventArg(cmd, commandData, cli));
            VerboseInformationLogged(new StackFrame(1), new StringEventArgs($"Server SEND to {cli.PlayerName} [{commandData}]"));
            VerboseInformationLogged(new StackFrame(1), new StringEventArgs("-------------------------------------------"));
        }
        public static void LogCommandReceived(AbstractCommand cmd, IBluffinClient cli, string commandData)
        {
            CommandReceived(new StackFrame(1), new LogCommandEventArg(cmd, commandData, cli ));
            VerboseInformationLogged(new StackFrame(1), new StringEventArgs($"Server RECV from {cli.PlayerName} [{commandData}]"));
            VerboseInformationLogged(new StackFrame(1), new StringEventArgs("-------------------------------------------"));
        }
        public static void LogTableCreated(int id, TableParams p)
        {
            TableCreated(new StackFrame(1), new LogTableCreationEventArg(id, p));
        }
        public static void LogGameCreated(int id)
        {
            GameCreated(new StackFrame(1), new LogGameEventArg(id));
        }
        public static void LogGameEnded(int id)
        {
            GameEnded(new StackFrame(1), new LogGameEventArg(id));
        }
        public static void LogClientCreated(TcpClient endpoint, IBluffinClient client)
        {
            ClientCreated(new StackFrame(1), new LogClientCreationEventArg(endpoint, client));
        }
        public static void LogClientIdentified(IBluffinClient client)
        {
            ClientIdentified(new StackFrame(1), new LogClientEventArg(client));
        }
    }
}
