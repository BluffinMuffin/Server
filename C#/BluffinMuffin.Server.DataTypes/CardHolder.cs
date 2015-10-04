using System.Collections.Generic;
using BluffinMuffin.HandEvaluator;
using BluffinMuffin.Protocol.DataTypes;

namespace BluffinMuffin.Server.DataTypes
{
    public class CardHolder : IStringCardsHolder
    {
        public PlayerInfo Player { get; }
        public IEnumerable<string> PlayerCards { get; }
        public IEnumerable<string> CommunityCards { get; }

        public CardHolder(PlayerInfo p, IEnumerable<string> playerCards, IEnumerable<string> communityCards)
        {
            Player = p;
            PlayerCards = playerCards;
            CommunityCards = communityCards;
        }
    }
}
