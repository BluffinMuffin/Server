using System.Collections.Generic;
using Com.Ericmas001.Games;

namespace BluffinMuffin.Server.DataTypes
{
    public abstract class AbstractDealer
    {
        protected Stack<GameCard> m_Deck;

        protected AbstractDealer()
        {
            FreshDeck();
        }

        public abstract GameCard[] DealHoles();
        public abstract GameCard[] DealFlop();
        public abstract GameCard DealTurn();
        public abstract GameCard DealRiver();

        public abstract void FreshDeck();
    }
}
