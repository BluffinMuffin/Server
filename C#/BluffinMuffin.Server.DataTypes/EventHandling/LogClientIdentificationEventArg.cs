using System;
using System.Net.Sockets;
using BluffinMuffin.Protocol;
using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Server.DataTypes.Protocol;

namespace BluffinMuffin.Server.DataTypes.EventHandling
{
    public class LogClientEventArg : EventArgs
    {
        public LogClientEventArg(IBluffinClient client)
        {
            Client = client;
        }

        public IBluffinClient Client { get; }
    }
}
