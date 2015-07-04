using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluffinMuffin.Server.Logic
{
    public class SuccessEventArg : EventArgs
    {
        public bool Success { get; set; }
    }
}
