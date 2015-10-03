using System.Collections.Generic;
using System.Linq;
using BluffinMuffin.HandEvaluator;
using BluffinMuffin.HandEvaluator.Enums;
using Com.Ericmas001.Portable.Util;
using static BluffinMuffin.HandEvaluator.Enums.NominalValueEnum;
using static BluffinMuffin.HandEvaluator.Enums.SuitEnum;

namespace BluffinMuffin.Server.DataTypes
{
    public abstract class AbstractDealer
    {
        protected Stack<PlayingCard> Deck { get; set; }
        
        public virtual PlayingCard[] DealCards(int nbCards)
        {
            var set = new PlayingCard[nbCards];
            for (int i = 0; i < nbCards; ++i)
                set[i] = Deck.Pop();
            return set;
        }

        public virtual void FreshDeck()
        {
            Deck = GetShuffledDeck();
        }

        public virtual IEnumerable<NominalValueEnum> UsedValues => new[] { Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King, Ace };
        public virtual IEnumerable<SuitEnum> UsedSuits => new[] { Clubs, Diamonds, Hearts, Spades };

        protected Stack<PlayingCard> GetShuffledDeck()
        {
            var deck = new Stack<PlayingCard>();
            var restantes = GetSortedDeck();
            while (restantes.Count > 0)
            {
                var id = Hasard.RandomWithMax(restantes.Count - 1);
                deck.Push(restantes[id]);
                restantes.RemoveAt(id);
            }
            return deck;
        }
        protected List<PlayingCard> GetSortedDeck()
        {
            return (from s in UsedSuits from v in UsedValues select new PlayingCard(v, s)).ToList();
        }
    }
}
