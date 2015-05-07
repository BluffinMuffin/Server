using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Protocol.DataTypes.Enums;

namespace BluffinMuffin.Server.DataTypes.EventHandling
{
    public class PlayerActionEventArgs : PlayerInfoEventArgs
    {
        private readonly GameActionEnum m_Action;
        private readonly int m_AmountPlayed;

        public GameActionEnum Action { get { return m_Action; } }
        public int AmountPlayed { get { return m_AmountPlayed; } }

        public PlayerActionEventArgs(PlayerInfo p, GameActionEnum action, int amnt)
            : base(p)
        {
            m_Action = action;
            m_AmountPlayed = amnt;
        }
    }
}
