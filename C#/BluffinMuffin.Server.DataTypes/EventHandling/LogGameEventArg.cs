using System;
using BluffinMuffin.Protocol;
using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Server.DataTypes.Protocol;

namespace BluffinMuffin.Server.DataTypes.EventHandling
{
    public class LogGameEventArg : EventArgs
    {
        public LogGameEventArg(int id)
        {
            Id = id;
        }
        public int Id { get; }
    }
}
