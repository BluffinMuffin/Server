using System;

namespace BluffinMuffin.Server.DataTypes.EventHandling
{
    public class PotWonEventArgs : EventArgs
    {
        public WinningPlayer Player { get; private set; }
        public MoneyPot Pot { get; private set; }
        public int AmountWon { get; private set; }

        public PotWonEventArgs(WinningPlayer player, MoneyPot pot, int amountwon)
        {
            Player = player;
            Pot = pot;
            AmountWon = amountwon;
        }
    }
}
