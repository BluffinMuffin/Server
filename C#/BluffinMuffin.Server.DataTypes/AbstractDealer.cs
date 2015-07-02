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

        public abstract PlayingCard[] DealHoles();
        public abstract PlayingCard[] DealFlop();
        public abstract PlayingCard DealTurn();
        public abstract PlayingCard DealRiver();

        public abstract void FreshDeck();
    }
}
