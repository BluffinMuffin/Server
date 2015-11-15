using System;
using BluffinMuffin.Protocol;
using BluffinMuffin.Server.DataTypes.Protocol;

namespace BluffinMuffin.Server.DataTypes.EventHandling
{
    public class LogCommandEventArg : EventArgs
    {
        public AbstractCommand Command { get; }
        public string CommandData { get; }
        public IBluffinClient Client { get; }
        public LogCommandEventArg(AbstractCommand command, string commandData, IBluffinClient client)
        {
            Command = command;
            CommandData = commandData;
            Client = client;
        }
    }
}
