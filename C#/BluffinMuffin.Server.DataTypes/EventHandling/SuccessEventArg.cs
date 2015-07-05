using System;

namespace BluffinMuffin.Server.DataTypes.EventHandling
{
    public class SuccessEventArg : EventArgs
    {
        public bool Success { get; set; }
    }
}
