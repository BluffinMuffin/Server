using System.Linq;
using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Protocol.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes.EventHandling;
using BluffinMuffin.Server.Logic.Extensions;

namespace BluffinMuffin.Server.Logic.GameModules
{
    public class WaitForPlayerModule : AbstractGameModule
    {
        public WaitForPlayerModule(PokerGameObserver o, PokerTable table) : base(o, table)
        {
        }

        public override GameStateEnum GameState => GameStateEnum.WaitForPlayers;

        public override void OnSitIn()
        {
            TryToBegin();
            base.OnSitIn();
        }

        public override void OnSitOut()
        {
            TryToBegin();
            base.OnSitOut();
        }

        public override void InitModule()
        {
            base.InitModule();
            TryToBegin();
        }

        private void TryToBegin()
        {
            foreach (var p in Table.Players)
            {
                if (p.State==PlayerStateEnum.Zombie && SitOut(p))
                    Table.LeaveTable(p);
                else if (p.IsReadyToPlay())
                    p.State = PlayerStateEnum.Playing;
                else
                    p.State = PlayerStateEnum.SitIn;
            }
            if (Table.HadPlayers && Table.NbPlaying == 0)
                RaiseAborted();
            else if (Table.NbPlaying >= Table.Params.MinPlayersToStart)
            {
                Table.Params.MinPlayersToStart = 2;
                Table.InitTable();
                Table.Variant.Dealer.FreshDeck();
                RaiseCompleted();
                Observer.RaiseGameBlindNeeded();
            }
            else
            {
                Table.Seats.SeatOfDealer()?.RemoveAttribute(SeatAttributeEnum.Dealer);
                Table.Seats.PlayingPlayers().ToList().ForEach(x => x.State = PlayerStateEnum.SitIn);
            }
        }

        private bool SitOut(PlayerInfo p)
        {
            var oldSeat = p.NoSeat;
            if (oldSeat == -1)
                return true;

            p.State = PlayerStateEnum.Zombie;
            if (Table.Players.ContainsPlayerWithSameName(p) && Table.SitOut(p))
            {
                var seat = new SeatInfo()
                {
                    Player = null,
                    NoSeat = oldSeat,
                };
                Observer.RaiseSeatUpdated(seat);
                return true;
            }
            return false;
        }
    }
}
