using System;
using BluffinMuffin.HandEvaluator;

namespace BluffinMuffin.Server.DataTypes.EventHandling
{
    public class PotWonEventArgs : EventArgs
    {
        public EvaluatedCardHolder<PlayerCardHolder> Player { get; }
        public int AmountWon { get; }

        public int PotId { get; }
        public int TotalPotAmount { get; }

        public PotWonEventArgs(EvaluatedCardHolder<PlayerCardHolder> player, int amountwon, int potId, int totalPotAmount)
        {
            Player = player;
            AmountWon = amountwon;
            PotId = potId;
            TotalPotAmount = totalPotAmount;
        }
    }
}
