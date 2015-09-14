using System.Collections.Generic;
using BluffinMuffin.HandEvaluator;
using BluffinMuffin.HandEvaluator.Enums;
using BluffinMuffin.Server.DataTypes;
using Com.Ericmas001.Util;

namespace BluffinMuffin.Server.Logic
{
    public class Shuffled28HighCardsDealer : AbstractDealer
    {
        public override PlayingCard[] DealCards(int nbCards)
        {
            var set = new PlayingCard[nbCards];
            for(int i = 0; i < nbCards; ++i)
                set[i] = Deck.Pop();
            return set;
        }

        public override void FreshDeck()
        {
            Deck = GetShuffledDeck();
        }

        private static Stack<PlayingCard> GetShuffledDeck()
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
        private static List<PlayingCard> GetSortedDeck()
        {
            var deck = new List<PlayingCard>();
            for (var i = 0; i < 4; ++i)
                for (var j = 6; j < 13; ++j)
                    deck.Add(new PlayingCard((NominalValueEnum)j,(SuitEnum)i));
            return deck;
        }
    }
}
