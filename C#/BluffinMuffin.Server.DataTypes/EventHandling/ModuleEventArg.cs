using System;

namespace BluffinMuffin.Server.DataTypes.EventHandling
{
    public class ModuleEventArg : EventArgs
    {
        public IGameModule Module { get; set; }
    }
}
