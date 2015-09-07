using System.Linq;
using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Protocol.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes.EventHandling;

namespace BluffinMuffin.Server.Logic.GameModules
{
    public class WaitForPlayerModule : AbstractGameModule
    {
        public WaitForPlayerModule(PokerGameObserver o, PokerTable table) : base(o, table)
        {
        }

        public override GameStateEnum GameState
        {
            get { return GameStateEnum.WaitForPlayers; }
        }

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
                if (p.IsZombie && SitOut(p))
                    Table.LeaveTable(p);
                else if (p.CanPlay)
                {
                    p.State = PlayerStateEnum.Playing;
                    p.IsShowingCards = false;
                }
                else
                    p.State = PlayerStateEnum.SitIn;
            }
            if (Table.HadPlayers && Table.NbPlaying == 0)
                RaiseAborted();
            else if (Table.NbPlaying >= Table.Params.MinPlayersToStart)
            {
                Table.Params.MinPlayersToStart = 2;
                Table.InitTable();
                Table.Dealer.FreshDeck();
                RaiseCompleted();
                Observer.RaiseGameBlindNeeded();
            }
            else
            {
                if (Table.DealerSeat != null)
                    Table.DealerSeat.SeatAttributes = Table.DealerSeat.SeatAttributes.Except(new[] { SeatAttributeEnum.Dealer }).ToArray();
                Table.PlayingPlayers.ForEach(x => x.State = PlayerStateEnum.SitIn);
            }
        }

        private bool SitOut(PlayerInfo p)
        {
            var oldSeat = p.NoSeat;
            if (oldSeat == -1)
                return true;

            p.State = PlayerStateEnum.Zombie;
            if (Table.SeatsContainsPlayer(p) && Table.SitOut(p))
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
