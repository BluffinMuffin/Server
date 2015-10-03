using System;
using BluffinMuffin.Protocol;
using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Server.DataTypes.Protocol;

namespace BluffinMuffin.Server.DataTypes.EventHandling
{
    public class LogTableCreationEventArg : LogGameEventArg
    {
        public LogTableCreationEventArg(int id, TableParams p) : base(id)
        {
            Params = p;
        }

        public TableParams Params { get; }
    }
}
