using System;

namespace BluffinMuffin.Server.DataTypes.EventHandling
{
    public class PotWonEventArgs : EventArgs
    {
        public WinningPlayer Player { get; }
        public int AmountWon { get; }

        public int PotId { get; }
        public int TotalPotAmount { get; }

        public PotWonEventArgs(WinningPlayer player, int amountwon, int potId, int totalPotAmount)
        {
            Player = player;
            AmountWon = amountwon;
            PotId = potId;
            TotalPotAmount = totalPotAmount;
        }
    }
}
