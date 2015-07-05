using System.Collections.Generic;
using BluffinMuffin.HandEvaluator;

namespace BluffinMuffin.Server.DataTypes
{
    public abstract class AbstractDealer
    {
        protected Stack<PlayingCard> m_Deck;

        public abstract PlayingCard[] DealCards(int nbCards);

        public abstract void FreshDeck();
    }
}
