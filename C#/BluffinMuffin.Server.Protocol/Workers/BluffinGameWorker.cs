using System;
using System.Collections.Generic;
using System.Linq;
using BluffinMuffin.Protocol;
using BluffinMuffin.Protocol.DataTypes.Options;
using BluffinMuffin.Server.DataTypes;
using BluffinMuffin.Server.Persistance;
using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Protocol.DataTypes.Enums;
using BluffinMuffin.Protocol.Enums;
using BluffinMuffin.Protocol.Game;
using BluffinMuffin.Server.Protocol.DataTypes;
using Com.Ericmas001.Util;

namespace BluffinMuffin.Server.Protocol.Workers
{
    public class BluffinGameWorker
    {
        private readonly KeyValuePair<Type, Action<AbstractCommand, IBluffinClient, RemotePlayer>>[] m_Methods;

        private IBluffinServer Server { get; set; }
        public BluffinGameWorker(IBluffinServer server)
        {
            Server = server;
            m_Methods = new[]
            {
                //General
                new KeyValuePair<Type, Action<AbstractCommand, IBluffinClient, RemotePlayer>>(typeof(AbstractCommand), OnCommandReceived), 
                new KeyValuePair<Type, Action<AbstractCommand, IBluffinClient, RemotePlayer>>(typeof(DisconnectCommand), OnDisconnectCommandReceived), 
                
                //Game
                new KeyValuePair<Type, Action<AbstractCommand, IBluffinClient, RemotePlayer>>(typeof(PlayerPlayMoneyCommand), OnPlayerPlayMoneyCommandReceived), 
                new KeyValuePair<Type, Action<AbstractCommand, IBluffinClient, RemotePlayer>>(typeof(PlayerDiscardActionCommand), OnPlayerDiscardActionCommandReceived), 
                new KeyValuePair<Type, Action<AbstractCommand, IBluffinClient, RemotePlayer>>(typeof(PlayerSitOutCommand), OnPlayerSitOutCommandReceived), 
                new KeyValuePair<Type, Action<AbstractCommand, IBluffinClient, RemotePlayer>>(typeof(PlayerSitInCommand), OnPlayerSitInCommandReceived)
                
            };
        }

        public void Start()
        {
            foreach (GameCommandEntry entry in Server.GameCommands.GetConsumingEnumerable())
            {
                GameCommandEntry e = entry;
                var liste = m_Methods.Where(x => e.Command.GetType().IsSubclassOf(x.Key) || x.Key == e.Command.GetType()).ToList();
                    liste.ForEach(x => x.Value(e.Command, e.Client, e.Player));
            }
        }

        private void OnCommandReceived(AbstractCommand command, IBluffinClient client, RemotePlayer p)
        {
            LogManager.Log(LogLevel.MessageVeryLow, "BluffinGameWorker.OnCommandReceived", "GameWorker RECV from {0} [{1}]", client.PlayerName, command.Encode());
            LogManager.Log(LogLevel.MessageVeryLow, "BluffinGameWorker.OnCommandReceived", "-------------------------------------------");
        }

        void OnDisconnectCommandReceived(AbstractCommand command, IBluffinClient client, RemotePlayer p)
        {
            if (p.Game.Table.Params.Lobby.OptionType == LobbyTypeEnum.RegisteredMode)
                DataManager.Persistance.Get(p.Client.PlayerName).TotalMoney += p.Player.MoneySafeAmnt;

            client.RemovePlayer(p);

            p.Player.State = PlayerStateEnum.Zombie;

            var t = p.Game.Table;
            LogManager.Log(LogLevel.Message, "BluffinGameWorker.OnDisconnectCommandReceived", "> Client '{0}' left table: {2}:{1}", p.Player.Name, t.Params.TableName, p.TableId);

            p.Game.LeaveGame(p.Player);
            
        }

        private void OnPlayerSitInCommandReceived(AbstractCommand command, IBluffinClient client, RemotePlayer p)
        {
            UserInfo userInfo = null;
            var c = (PlayerSitInCommand)command;
            if (p.Game.Table.Params.Lobby.OptionType == LobbyTypeEnum.QuickMode)
                p.Player.MoneySafeAmnt = ((LobbyOptionsQuickMode)p.Game.Table.Params.Lobby).StartingAmount;
            else
            {
                int money = c.MoneyAmount;
                userInfo = DataManager.Persistance.Get(p.Client.PlayerName);
                if (userInfo == null || userInfo.TotalMoney < money)
                    p.Player.MoneySafeAmnt = -1;
                else
                {
                    userInfo.TotalMoney -= money;
                    p.Player.MoneySafeAmnt = money;
                }
            }
            var seat = p.Game.Table.SitIn(p.Player, c.NoSeat);
            if (seat == null)
            {
                client.SendCommand(c.ResponseFailure(BluffinMessageId.NoMoreSeats, "No seats available"));
                if (userInfo != null)
                    userInfo.TotalMoney += p.Player.MoneySafeAmnt; 
            }
            else
            {
                var r = (seat.NoSeat != c.NoSeat) ? c.ResponseSuccess(BluffinMessageId.SeatChanged, "The asked seat wasn't available, the server gave you another one.") : c.ResponseSuccess();
                r.NoSeat = seat.NoSeat;
                client.SendCommand(r);
                p.Game.AfterPlayerSat(p.Player);
            }
        }

        private void OnPlayerSitOutCommandReceived(AbstractCommand command, IBluffinClient client, RemotePlayer p)
        {
            var c = (PlayerSitOutCommand)command;
            client.SendCommand(c.ResponseSuccess());
            p.Game.SitOut(p.Player);
        }

        private void OnPlayerPlayMoneyCommandReceived(AbstractCommand command, IBluffinClient client, RemotePlayer p)
        {
            var c = (PlayerPlayMoneyCommand)command;
            p.Game.PlayMoney(p.Player, c.AmountPlayed);
        }

        private void OnPlayerDiscardActionCommandReceived(AbstractCommand command, IBluffinClient client, RemotePlayer p)
        {
            var c = (PlayerDiscardActionCommand)command;
            p.Game.Discard(p.Player,c.CardsDiscarded);
        }
    }
}
