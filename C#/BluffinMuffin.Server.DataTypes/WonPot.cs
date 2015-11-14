using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BluffinMuffin.Protocol.DataTypes;

namespace BluffinMuffin.Server.DataTypes
{
    public class WonPot
    {
        public WonPot(int potId, int totalPotAmount, IEnumerable<KeyValuePair<PlayerInfo, int>> winners)
        {
            PotId = potId;
            TotalPotAmount = totalPotAmount;
            Winners = winners;
        }

        public int PotId { get; }
        public int TotalPotAmount { get; }

        public IEnumerable<KeyValuePair<PlayerInfo,int>> Winners { get; } 
    }
}
