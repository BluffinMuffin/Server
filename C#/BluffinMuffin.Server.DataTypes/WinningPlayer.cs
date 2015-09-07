using BluffinMuffin.HandEvaluator;
using BluffinMuffin.Protocol.DataTypes;

namespace BluffinMuffin.Server.DataTypes
{
    public class WinningPlayer
    {
        public PlayerInfo Player { get; set; }
        public HandEvaluationResult Hand { get; set; }
    }
}
