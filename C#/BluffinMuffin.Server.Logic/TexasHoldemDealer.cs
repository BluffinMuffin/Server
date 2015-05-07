using Com.Ericmas001.Games;
using BluffinMuffin.Server.DataTypes;

namespace BluffinMuffin.Server.Logic
{
    public class TexasHoldemDealer : AbstractDealer
    {
        public override GameCard[] DealHoles()
        {
            var set = new GameCard[2];
            set[0] = m_Deck.Pop();
            set[1] = m_Deck.Pop();
            return set;
        }

        public override GameCard[] DealFlop()
        {
            var set = new GameCard[3];
            set[0] = m_Deck.Pop();
            set[1] = m_Deck.Pop();
            set[2] = m_Deck.Pop();
            return set;
        }

        public override GameCard DealTurn()
        {
            return m_Deck.Pop();
        }

        public override GameCard DealRiver()
        {
            return m_Deck.Pop();
        }

        public override void FreshDeck()
        {
            m_Deck = GameCardUtility.GetShuffledDeck(false);
        }
    }
}
