using System;

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
