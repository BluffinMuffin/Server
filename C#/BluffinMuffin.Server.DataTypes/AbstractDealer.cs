using System.Collections.Generic;
using BluffinMuffin.HandEvaluator;

namespace BluffinMuffin.Server.DataTypes
{
    public abstract class AbstractDealer
    {
        protected Stack<PlayingCard> m_Deck;

        protected AbstractDealer()
        {
            FreshDeck();
        }

        public abstract PlayingCard[] DealCards(int nbCards);
        public abstract PlayingCard DealCard();

        public abstract void FreshDeck();
    }
}
