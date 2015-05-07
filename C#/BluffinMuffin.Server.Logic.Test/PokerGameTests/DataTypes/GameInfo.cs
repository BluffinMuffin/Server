using System.Collections.Generic;
using System.Linq;
using BluffinMuffin.Protocol.DataTypes;

namespace BluffinMuffin.Server.Logic.Test.PokerGameTests.DataTypes
{
    public class GameInfo
    {
        public PokerGame Game { get; set; }
        public PlayerInfo P1 { get; set; }
        public PlayerInfo P2 { get; set; }
        public PlayerInfo P3 { get; set; }
        public PlayerInfo P4 { get; set; }
        

        public IEnumerable<PlayerInfo> Players{ get { return Game.GameTable.PlayingPlayers; }}
        public PlayerInfo CurrentPlayer { get { return Game.Table.CurrentPlayer; } }
        public PlayerInfo PoorestPlayer { get { return Players.OrderBy(x => x.MoneySafeAmnt).First(); } }

        public PlayerInfo CalculatedSmallBlind { get { return Players.Where(x => BlindNeeded(x) > 0).OrderBy(BlindNeeded).First(); } }
        public PlayerInfo CalculatedBigBlind { get { return Players.Where(x => BlindNeeded(x) > 0).OrderBy(BlindNeeded).Last(); } }
        public PlayerInfo Dealer { get { return Game.GameTable.DealerSeat.Player; } }


        public bool CurrentPlayerPlays(int amount)
        {
            return Game.PlayMoney(CurrentPlayer, amount);
        }
        public bool CurrentPlayerChecks()
        {
            return CurrentPlayerPlays(0);
        }
        public bool CurrentPlayerFolds()
        {
            return CurrentPlayerPlays(-1);
        }
        public bool CurrentPlayerCalls()
        {
            return CurrentPlayerPlays(Game.Table.CallAmnt(CurrentPlayer));
        }
        public bool CurrentPlayerRaisesMinimum()
        {
            return CurrentPlayerPlays(Game.Table.MinRaiseAmnt(CurrentPlayer));
        }
        public PlayerInfo SitInGame(PlayerInfo p)
        {
            Game.JoinGame(p);
            Game.GameTable.SitIn(p, -1);
            Game.AfterPlayerSat(p);
            return p;
        }

        public int BlindNeeded(PlayerInfo p)
        {
            return Game.GameTable.GetBlindNeeded(p);
        }
        public void PutBlinds(PlayerInfo p)
        {
            Game.PlayMoney(p, BlindNeeded(p));
        }

        public PlayerInfo PlayerNextTo(PlayerInfo p)
        {
            return Game.GameTable.GetSeatOfPlayingPlayerNextTo(Game.GameTable.Seats.Single(x => x.Player == p)).Player;
        }
    }
}
