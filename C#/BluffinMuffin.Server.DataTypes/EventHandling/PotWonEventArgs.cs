using BluffinMuffin.Protocol.DataTypes;

namespace BluffinMuffin.Server.DataTypes.EventHandling
{
    public class PotWonEventArgs : PlayerInfoEventArgs
    {
        private readonly int m_Id;
        private readonly int m_AmountWon;

        public int Id { get { return m_Id; } }
        public int AmountWon { get { return m_AmountWon; } }

        public PotWonEventArgs(PlayerInfo p, int id, int amntWon)
            : base(p)
        {
            m_Id = id;
            m_AmountWon = amntWon;
        }
    }
}
