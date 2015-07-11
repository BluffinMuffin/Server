using BluffinMuffin.Server.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes.EventHandling;

namespace BluffinMuffin.Server.Logic.GameModules
{
    public class DiscardRoundModule : AbstractGameModule
    {
        private int m_Minimum;
        private int m_Maximum;
        public DiscardRoundModule(PokerGameObserver o, PokerTable table, int minimum, int maximum)
            : base(o, table)
        {
            m_Minimum = minimum;
            m_Maximum = maximum;
        }

        public override GameStateEnum GameState
        {
            get { return GameStateEnum.Playing; }
        }

        public override void InitModule()
        {
            Observer.RaiseDiscardActionNeeded(m_Minimum, m_Maximum);
        }
    }
}
