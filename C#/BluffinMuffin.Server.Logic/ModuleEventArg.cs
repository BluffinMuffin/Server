using System;
using BluffinMuffin.Server.Logic.GameModules;

namespace BluffinMuffin.Server.Logic
{
    public class ModuleEventArg : EventArgs
    {
        public IGameModule Module { get; set; }
    }
}
