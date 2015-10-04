using System;
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
