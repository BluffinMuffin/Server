using System;
using BluffinMuffin.Protocol;
using BluffinMuffin.Server.DataTypes.Protocol;

namespace BluffinMuffin.Server.DataTypes.EventHandling
{
    public class LogCommandEventArg : EventArgs
    {
        public AbstractCommand Command { get; set; }
        public string CommandData { get; set; }
        public IBluffinClient Client { get; set; }
        public LogCommandEventArg(AbstractCommand command, string commandData, IBluffinClient client)
        {
            Command = command;
            CommandData = commandData;
            Client = client;
        }
    }
}
