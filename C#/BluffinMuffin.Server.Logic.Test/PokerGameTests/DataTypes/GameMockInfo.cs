using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Server.Logic.Extensions;

namespace BluffinMuffin.Server.Logic.Test.PokerGameTests.DataTypes
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class GameMockInfo
    {
        public PokerGame Game { get; set; }
        public PlayerInfo P1 { get; set; }
        public PlayerInfo P2 { get; set; }
        public PlayerInfo P3 { get; set; }
        public PlayerInfo P4 { get; set; }


        public IEnumerable<PlayerInfo> Players => Game.Table.Seats.PlayingPlayers();
        public PlayerInfo CurrentPlayer => Game.Table.Seats.CurrentPlayer();
        public PlayerInfo PoorestPlayer { get { return Players.OrderBy(x => x.MoneySafeAmnt).First(); } }

        public PlayerInfo CalculatedSmallBlind { get { return Players.Where(x => BlindNeeded(x) > 0).OrderBy(BlindNeeded).First(); } }
        public PlayerInfo CalculatedBigBlind { get { return Players.Where(x => BlindNeeded(x) > 0).OrderBy(BlindNeeded).Last(); } }
        public PlayerInfo Dealer => Game.Table.Seats.SeatOfDealer().Player;


        public bool CurrentPlayerPlays(int amount)
        {
            return Game.PlayMoney(CurrentPlayer, amount);
        }
        public bool CurrentPlayerChecks()
        {
            return CurrentPlayerPlays(0);
        }
        public void CurrentPlayerFolds()
        {
            CurrentPlayerPlays(-1);
        }

        public bool CurrentPlayerCalls()
        {
            return CurrentPlayerPlays(Game.Table.NeededCallAmountForPlayer(CurrentPlayer));
        }
        public bool CurrentPlayerRaisesMinimum()
        {
            return CurrentPlayerPlays(Game.Table.MinRaiseAmountForPlayer(CurrentPlayer));
        }
        public void SitInGame(PlayerInfo p)
        {
            Game.Table.SitIn(p);
            Game.AfterPlayerSat(p);
        }

        public int BlindNeeded(PlayerInfo p)
        {
            return Game.Table.Bank.DebtAmount(p);
        }
        public void PutBlinds(PlayerInfo p)
        {
            if(BlindNeeded(p) > 0)
                Game.PlayMoney(p, BlindNeeded(p));
        }

        public PlayerInfo PlayerNextTo(PlayerInfo p)
        {
            return Game.Table.Seats.SeatOfPlayingPlayerNextTo(Game.Table.Seats.Single(x => x.Player == p)).Player;
        }
    }
}
