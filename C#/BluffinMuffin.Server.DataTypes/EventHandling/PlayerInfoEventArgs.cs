using System;
using BluffinMuffin.Protocol.DataTypes;

namespace BluffinMuffin.Server.DataTypes.EventHandling
{
    public class PlayerInfoEventArgs : EventArgs
    {
        private readonly PlayerInfo m_Player;
        public PlayerInfo Player { get { return m_Player; } }

        public PlayerInfoEventArgs(PlayerInfo p)
        {
            m_Player = p;
        }
    }
}
