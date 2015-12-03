using System;
using System.Collections.Generic;
using System.Linq;
using BluffinMuffin.Protocol;
using BluffinMuffin.Server.DataTypes;
using BluffinMuffin.Server.Logic;
using BluffinMuffin.Server.Persistance;
using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Protocol.DataTypes.Enums;
using BluffinMuffin.Protocol.Enums;
using BluffinMuffin.Protocol.Lobby;
using BluffinMuffin.Protocol.Lobby.RegisteredMode;
using BluffinMuffin.Protocol.Lobby.QuickMode;
using BluffinMuffin.Server.DataTypes.Protocol;
using BluffinMuffin.Server.Logic.Extensions;

namespace BluffinMuffin.Server.Protocol.Workers
{
    public class BluffinLobbyWorker
    {
        private readonly KeyValuePair<Type, Action<AbstractCommand, IBluffinClient>>[] m_Methods;

        private IBluffinServer Server { get; }
        private IBluffinLobby Lobby { get; }
        public BluffinLobbyWorker(IBluffinServer server, IBluffinLobby lobby)
        {
            Server = server;
            Lobby = lobby;
            m_Methods = new[]
            {
                //General 
                new KeyValuePair<Type, Action<AbstractCommand, IBluffinClient>>(typeof(DisconnectCommand), OnDisconnectCommandReceived), 
                
                //Lobby
                new KeyValuePair<Type, Action<AbstractCommand, IBluffinClient>>(typeof(ListTableCommand), OnListTableCommandReceived), 
                new KeyValuePair<Type, Action<AbstractCommand, IBluffinClient>>(typeof(CheckCompatibilityCommand), OnCheckCompatibilityCommandReceived), 
                new KeyValuePair<Type, Action<AbstractCommand, IBluffinClient>>(typeof(JoinTableCommand), OnJoinTableCommandReceived), 
                new KeyValuePair<Type, Action<AbstractCommand, IBluffinClient>>(typeof(LeaveTableCommand), OnLeaveTableCommandReceived), 
                new KeyValuePair<Type, Action<AbstractCommand, IBluffinClient>>(typeof(CreateTableCommand), OnCreateTableCommandReceived), 
                
                //Lobby QuickMode
                new KeyValuePair<Type, Action<AbstractCommand, IBluffinClient>>(typeof(IdentifyCommand), OnIdentifyCommandReceived), 

                //Lobby RegisteredMode
                new KeyValuePair<Type, Action<AbstractCommand, IBluffinClient>>(typeof(CheckDisplayExistCommand), OnCheckDisplayExistCommandReceived), 
                new KeyValuePair<Type, Action<AbstractCommand, IBluffinClient>>(typeof(CheckUserExistCommand), OnCheckUserExistCommandReceived), 
                new KeyValuePair<Type, Action<AbstractCommand, IBluffinClient>>(typeof(CreateUserCommand), OnCreateUserCommandReceived), 
                new KeyValuePair<Type, Action<AbstractCommand, IBluffinClient>>(typeof(AuthenticateUserCommand), OnAuthenticateUserCommandReceived), 
                new KeyValuePair<Type, Action<AbstractCommand, IBluffinClient>>(typeof(GetUserCommand), OnGetUserCommandReceived)
                
            };
        }

        public void Start()
        {
            foreach (CommandEntry entry in Server.LobbyCommands.GetConsumingEnumerable())
            {
                CommandEntry e = entry;
                m_Methods.Where(x => e.Command.GetType().IsSubclassOf(x.Key) || x.Key == e.Command.GetType()).ToList().ForEach(x => x.Value(e.Command, e.Client));
            }
        }

        void OnIdentifyCommandReceived(AbstractCommand command, IBluffinClient client)
        {
            var c = (IdentifyCommand)command;
            var ok = !Lobby.IsNameUsed(c.Name) && !DataManager.Persistance.IsDisplayNameExist(c.Name);
            Logger.LogInformation("> Client indentifying QuickMode server as : {0}. Success={1}", c.Name, ok);
            if (ok)
            {
                client.PlayerName = c.Name;
                Logger.LogClientIdentified(client);
                client.SendCommand(c.ResponseSuccess());
                Lobby.AddName(c.Name);
            }
            else
            {
                client.SendCommand(c.ResponseFailure(BluffinMessageId.NameAlreadyUsed,"The name is already used on the server!"));
            }
        }

        void OnDisconnectCommandReceived(AbstractCommand command, IBluffinClient client)
        {
            Logger.LogInformation("> Client disconnected: {0}", client.PlayerName);
            Lobby.RemoveName(client.PlayerName);
        }

        void OnListTableCommandReceived(AbstractCommand command, IBluffinClient client)
        {
            var c = (ListTableCommand)command;
            var r = c.ResponseSuccess();
            r.Tables = Lobby.ListTables(c.LobbyTypes);
            client.SendCommand(r);
        }

        private void OnCheckCompatibilityCommandReceived(AbstractCommand command, IBluffinClient client)
        {
            const string MINIMUM_CLIENT_VERSION = "3.0";
            const string CURRENT_SERVER_VERSION = "3.1.0";

            var c = (CheckCompatibilityCommand)command;
            Version vClient; 
            bool ok = Version.TryParse(c.ImplementedProtocolVersion,out vClient);
            if (ok)
                client.SupportedProtocol = vClient;
            client.ClientIdentification = c.ClientIdentification;
            Logger.LogClientAdditionalInfo(client);
            if (!ok || vClient < new Version(MINIMUM_CLIENT_VERSION))
            {
                var r = c.ResponseFailure(BluffinMessageId.NotSupported, "The client must implement at least protocol version " + MINIMUM_CLIENT_VERSION);
                r.ImplementedProtocolVersion = CURRENT_SERVER_VERSION;
                client.SendCommand(r);
            }
            else
            {
                var r = c.ResponseSuccess();
                r.ImplementedProtocolVersion = CURRENT_SERVER_VERSION;
                r.SupportedLobbyTypes = new[] {LobbyTypeEnum.QuickMode, LobbyTypeEnum.RegisteredMode};
                r.AvailableGames = new[]
                {
                    new GameInfo
                    {
                        AvailableBlinds = new [] {BlindTypeEnum.Blinds},
                        AvailableLimits = new []{LimitTypeEnum.NoLimit},
                        AvailableVariants = RuleFactory.Variants.Values.Where(x => x.GameType == GameTypeEnum.CommunityCardsPoker).Select(x => x.Variant).ToArray(),
                        GameType = GameTypeEnum.CommunityCardsPoker,
                        MaxPlayers = 10,
                        MinPlayers = 2
                    },
                    new GameInfo
                    {
                        AvailableBlinds = new [] {BlindTypeEnum.Antes},
                        AvailableLimits = new []{LimitTypeEnum.NoLimit},
                        AvailableVariants = RuleFactory.Variants.Values.Where(x => x.GameType == GameTypeEnum.StudPoker).Select(x => x.Variant).ToArray(),
                        GameType = GameTypeEnum.StudPoker,
                        MaxPlayers = 10,
                        MinPlayers = 2
                    },
                    new GameInfo
                    {
                        AvailableBlinds = new [] {BlindTypeEnum.Antes},
                        AvailableLimits = new []{LimitTypeEnum.NoLimit},
                        AvailableVariants = RuleFactory.Variants.Values.Where(x => x.GameType == GameTypeEnum.DrawPoker).Select(x => x.Variant).ToArray(),
                        GameType = GameTypeEnum.DrawPoker,
                        MaxPlayers = 10,
                        MinPlayers = 2
                    }
                };
                client.SendCommand(r);
            }
        }


        private void OnGetUserCommandReceived(AbstractCommand command, IBluffinClient client)
        {
            var c = (GetUserCommand)command;
            var u = DataManager.Persistance.Get(client.PlayerName);
            if(u == null)
                client.SendCommand(c.ResponseFailure(BluffinMessageId.UsernameNotFound, "Your username was not in the database. That's weird !"));
            else
            {
                var r = c.ResponseSuccess();
                r.Email = u.Email;
                r.DisplayName = u.DisplayName;
                r.Money = u.TotalMoney;
                client.SendCommand(r);
            }
        }

        private void OnAuthenticateUserCommandReceived(AbstractCommand command, IBluffinClient client)
        {
            var c = (AuthenticateUserCommand)command;
            var u = DataManager.Persistance.Get(c.Username);

            var ok = false;
            if (u != null)
            {
                client.PlayerName = u.DisplayName;
                if (DataManager.Persistance.Authenticate(c.Username, c.Password) != null)
                {
                    if (!Lobby.IsNameUsed(client.PlayerName))
                    {
                        Lobby.AddName(client.PlayerName);
                        ok = true;
                        client.SendCommand(c.ResponseSuccess());
                    }
                    else
                        client.SendCommand(c.ResponseFailure(BluffinMessageId.NameAlreadyUsed, "The name is already used on the server!"));
                }
                else
                    client.SendCommand(c.ResponseFailure(BluffinMessageId.InvalidPassword, "Wrong Password!"));
            }
            else
                client.SendCommand(c.ResponseFailure(BluffinMessageId.UsernameNotFound, "Your username was not in the database!"));
            Logger.LogInformation("> Client authenticate to RegisteredMode Server as : {0}. Success={1}", c.Username, ok);
        }

        private void OnCreateUserCommandReceived(AbstractCommand command, IBluffinClient client)
        {
            var c = (CreateUserCommand)command;
            var ok = false;
            if (!DataManager.Persistance.IsUsernameExist(c.Username))
            {
                if (!DataManager.Persistance.IsDisplayNameExist(c.DisplayName))
                {
                    DataManager.Persistance.Register(new UserInfo(c.Username, c.Password, c.Email, c.DisplayName, 7500));
                    ok = true;
                    client.SendCommand(c.ResponseSuccess());
                }
                else
                    client.SendCommand(c.ResponseFailure(BluffinMessageId.NameAlreadyUsed, "The display name is already used on the server!"));
            }
            else
                client.SendCommand(c.ResponseFailure(BluffinMessageId.UsernameAlreadyUsed, "The username is already used on the server!"));

            Logger.LogInformation("> Client register to RegisteredMode Server as : {0}. Success={1}", c.Username, ok);
        }

        private void OnCheckUserExistCommandReceived(AbstractCommand command, IBluffinClient client)
        {
            var c = (CheckUserExistCommand)command;
            var r = c.ResponseSuccess();
            r.Exist = DataManager.Persistance.IsUsernameExist(c.Username);
            client.SendCommand(r);
        }

        private void OnCheckDisplayExistCommandReceived(AbstractCommand command, IBluffinClient client)
        {
            var c = (CheckDisplayExistCommand)command;
            var r = c.ResponseSuccess();
            r.Exist = Lobby.IsNameUsed(c.DisplayName) || DataManager.Persistance.IsDisplayNameExist(c.DisplayName);
            client.SendCommand(r);
        }

        private void OnCreateTableCommandReceived(AbstractCommand command, IBluffinClient client)
        {
            var c = (CreateTableCommand)command;
            var res = Lobby.CreateTable(c);
            var r = c.ResponseSuccess();
            Logger.LogInformation("> Client '{0}' {3}: {2}:{1}", client.PlayerName, c.Params.TableName, res, c.Params.Lobby.OptionType);
            r.IdTable = res;
            client.SendCommand(r);
        }

        private void OnJoinTableCommandReceived(AbstractCommand command, IBluffinClient client)
        {
            var c = (JoinTableCommand)command;
            var game = (PokerGame)Lobby.GetGame(c.TableId);
            if (game == null || !game.IsRunning)
            {
                client.SendCommand(c.ResponseFailure(BluffinMessageId.WrongTableState, "You can't join a game that isn't running !"));
                return;
            }
            var table = game.Table;
            if (table.Seats.Players().ContainsPlayerNamed(client.PlayerName))
            {
                client.SendCommand(c.ResponseFailure(BluffinMessageId.NameAlreadyUsed, "Someone with your name is already in this game !"));
                return;
            }
            var rp = new RemotePlayer(game, new PlayerInfo(client.PlayerName, 0), client, c.TableId);
            if (!rp.JoinGame())
            {
                client.SendCommand(c.ResponseFailure(BluffinMessageId.SpecificServerMessage, "Unknown failure"));
                return;
            }

            client.AddPlayer(rp);

            Logger.LogInformation("> Client '{0}' joined {2}:{1}", client.PlayerName, table.Params.TableName, c.TableId, rp.Player.NoSeat);


            var r = c.ResponseSuccess();

            r.GameHasStarted = rp.Game.IsPlaying;
            r.BoardCards = rp.Game.Table.Cards.Select(x => x.ToString()).ToArray();
            r.Seats = rp.AllSeats().ToList();
            r.Params = rp.Game.Table.Params;
            r.TotalPotAmount = rp.Game.Table.Bank.MoneyAmount;
            r.PotsAmount = rp.Game.Table.Bank.PotAmountsPadded(rp.Game.Table.Params.MaxPlayers).ToList();

            client.SendCommand(r);
        }

        private void OnLeaveTableCommandReceived(AbstractCommand command, IBluffinClient client)
        {
            var c = (LeaveTableCommand)command;
            var game = (PokerGame)Lobby.GetGame(c.TableId);
            game.LeaveGame(game.Table.Seats.Players().Single(x => x.Name == client.PlayerName));
        }
    }
}
