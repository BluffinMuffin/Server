using System.Collections.Generic;
using BluffinMuffin.HandEvaluator;
using BluffinMuffin.HandEvaluator.Enums;
using BluffinMuffin.Server.DataTypes;
using Com.Ericmas001.Util;

namespace BluffinMuffin.Server.Logic
{
    public class TexasHoldemDealer : AbstractDealer
    {
        public override PlayingCard[] DealHoles()
        {
            var set = new PlayingCard[2];
            set[0] = m_Deck.Pop();
            set[1] = m_Deck.Pop();
            return set;
        }

        public override PlayingCard[] DealFlop()
        {
            var set = new PlayingCard[3];
            set[0] = m_Deck.Pop();
            set[1] = m_Deck.Pop();
            set[2] = m_Deck.Pop();
            return set;
        }

        public override PlayingCard DealTurn()
        {
            return m_Deck.Pop();
        }

        public override PlayingCard DealRiver()
        {
            return m_Deck.Pop();
        }

        public override void FreshDeck()
        {
            m_Deck = GetShuffledDeck();
        }

        public static Stack<PlayingCard> GetShuffledDeck()
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
        public static List<PlayingCard> GetSortedDeck()
        {
            var deck = new List<PlayingCard>();
            for (var i = 0; i < 4; ++i)
                for (var j = 0; j < 13; ++j)
                    deck.Add(new PlayingCard((NominalValueEnum)j,(SuitEnum)i));
            return deck;
        }
    }
}
