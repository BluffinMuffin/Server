using System;
using BluffinMuffin.Protocol.DataTypes.Enums;

namespace BluffinMuffin.Server.DataTypes.EventHandling
{
    public class RoundEventArgs : EventArgs
    {
        private readonly string m_Round;
        public string Round { get { return m_Round; } }

        public RoundEventArgs(string r)
        {
            m_Round = r;
        }
    }
}
