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
            PlayerInfo pZombie;
            Table.Zombies.ToArray().ToList().ForEach(p => Observer.RaisePlayerLeft(p));
            Table.Zombies.Clear();
            while ((pZombie = Table.Zombies.FirstOrDefault()) != null)
                Observer.RaisePlayerLeft(pZombie);

            foreach (var p in Table.Seats.Players())
                p.ChangeState(p.IsReadyToPlay() ? PlayerStateEnum.Playing : PlayerStateEnum.SitIn);

            if (Table.HadPlayers && !Table.Seats.PlayingPlayers().Any())
                RaiseAborted();
            else if (Table.Seats.PlayingPlayers().Count() >= Table.Params.MinPlayersToStart)
            {
                Table.Params.MinPlayersToStart = 2;
                Table.InitTable();
                Table.Variant.Dealer.FreshDeck();
                RaiseCompleted();
                Observer.RaiseGameBlindNeeded();
            }
            else
            {
                Table.Seats.ClearAttribute(SeatAttributeEnum.Dealer);
                Table.Seats.PlayingPlayers().ToList().ForEach(x => x.ChangeState(PlayerStateEnum.SitIn));
            }
        }
    }
}
