using System;
using System.Collections.Generic;
using System.Linq;
using BluffinMuffin.Protocol;
using BluffinMuffin.Protocol.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes.EventHandling;
using BluffinMuffin.Server.Logic;
using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Protocol.Game;
using BluffinMuffin.Server.Protocol.DataTypes;

namespace BluffinMuffin.Server.Protocol
{
    public class RemotePlayer
    {
        public PokerGame Game { get; private set; }
        public PlayerInfo Player { get; private set; }
        public IBluffinClient Client { get; private set; }
        public int TableId { get; private set; }

        public RemotePlayer(PokerGame game, PlayerInfo player, IBluffinClient client, int tableId)
        {
            Game = game;
            Player = player;
            Client = client;
            TableId = tableId;
        }

        public bool JoinGame()
        {
            InitializePokerObserver();
            return Game.JoinGame(Player);
        }
        public void SendTableInfo()
        {
            var cmd = new TableInfoCommand
            {
                GameHasStarted = Game.IsPlaying
            };
            var table = Game.GameTable;
            lock (table)
            {
                var playerSendingTo = Player;

                cmd.BoardCards = table.Cards.Select(c => c.ToString()).ToArray();
                cmd.Seats = new List<SeatInfo>();

                cmd.Params = table.Params;

                cmd.TotalPotAmount = table.TotalPotAmnt;

                cmd.PotsAmount = table.PotAmountsPadded.ToList();

                for (var i = 0; i < cmd.Params.MaxPlayers; ++i)
                {
                    var si = new SeatInfo() { NoSeat = i };
                    cmd.Seats.Add(si);
                    var gameSeat = table.Seats[i];
                    if (gameSeat.IsEmpty)
                        continue;
                    si.Player = gameSeat.Player.Clone();

                    //If we are not sending the info about the player who is receiving, don't show the cards unless you can
                    if (i != playerSendingTo.NoSeat && si.Player.IsPlaying&& !si.Player.IsShowingCards)
                        si.Player.HoleCards = new[] {"??", "??"};

                    if (si.Player.HoleCards == null || si.Player.HoleCards.Length != 2)
                        si.Player.HoleCards = new[] { String.Empty, String.Empty };

                    si.SeatAttributes = gameSeat.SeatAttributes;
                }
            }
            Send(cmd);
        }

        private void InitializePokerObserver()
        {
            Game.Observer.GameBettingRoundEnded += OnGameBettingRoundEnded;
            Game.Observer.PlayerHoleCardsChanged += OnPlayerHoleCardsChanged;
            Game.Observer.GameEnded += OnGameEnded;
            Game.Observer.PlayerWonPot += OnPlayerWonPot;
            Game.Observer.PlayerActionTaken += OnPlayerActionTaken;
            Game.Observer.EverythingEnded += OnEverythingEnded;
            Game.Observer.PlayerActionNeeded += OnPlayerActionNeeded;
            Game.Observer.GameBlindNeeded += OnGameBlindNeeded;
            Game.Observer.GameBettingRoundStarted += OnGameBettingRoundStarted;
            Game.Observer.GameGenerallyUpdated += OnGameGenerallyUpdated;
            Game.Observer.PlayerJoined += OnPlayerJoined;
            Game.Observer.SeatUpdated += OnSeatUpdated;
        }
        #region PokerObserver Event Handling
        void OnGameBettingRoundEnded(object sender, RoundEventArgs e)
        {
            Send(new BetTurnEndedCommand()
            {
                PotsAmounts = Game.Table.PotAmountsPadded.ToList(),
                Round = e.Round,
            });
        }

        void OnPlayerHoleCardsChanged(object sender, PlayerInfoEventArgs e)
        {
            var p = e.Player;
            var holeCards = p.NoSeat == Player.NoSeat || p.IsShowingCards ? p.HoleCards : new[] {"??", "??"};

            Send(new PlayerHoleCardsChangedCommand()
            {
                NoSeat = p.NoSeat,
                PlayerState = p.State,
                Cards = holeCards,
            });
        }

        void OnGameEnded(object sender, EventArgs e)
        {
            Send(new GameEndedCommand());
        }

        void OnPlayerWonPot(object sender, PotWonEventArgs e)
        {
            var playerInfo = e.Player;
            var pot = e.Pot;
            Send(new PlayerWonPotCommand()
            {
                NoSeat = playerInfo.Player.NoSeat,
                PotId = pot.Id,
                WonAmount = e.AmountWon,
                TotalPlayerMoney = playerInfo.Player.MoneySafeAmnt,
                TotalPotAmount = pot.Amount,
                WinningCards = playerInfo.Hand == null ? new string[0] : playerInfo.Hand.Cards.SelectMany(x => x).Take(5).Select(x => x.ToString()).ToArray(),
                WinningHand = playerInfo.Hand == null ? PokerHandEnum.None : (PokerHandEnum)Enum.Parse(typeof(PokerHandEnum), playerInfo.Hand.Hand.ToString())
            });
        }

        void OnPlayerActionTaken(object sender, PlayerActionEventArgs e)
        {
            var p = e.Player;
            Send(new PlayerTurnEndedCommand()
            {
                NoSeat = p.NoSeat,
                TotalPlayedMoneyAmount = p.MoneyBetAmnt,
                TotalSafeMoneyAmount = p.MoneySafeAmnt,
                TotalPot = Game.Table.TotalPotAmnt,
                ActionTakenType = e.Action,
                ActionTakenAmount = e.AmountPlayed,
                PlayerState = p.State,
            });
        }

        void OnEverythingEnded(object sender, EventArgs e)
        {
            Send(new TableClosedCommand());
        }

        void OnPlayerActionNeeded(object sender, PlayerInfoEventArgs e)
        {
            Send(new PlayerTurnBeganCommand()
            {
                NoSeat = e.Player.NoSeat,
                MinimumRaiseAmount = Game.Table.MinimumRaiseAmount,
            });
        }

        void OnGameBlindNeeded(object sender, EventArgs e)
        {
            Send(new GameStartedCommand() { NeededBlindAmount = Game.GameTable.GetBlindNeeded(Player) });
        }

        void OnGameBettingRoundStarted(object sender, RoundEventArgs e)
        {
            Send(new BetTurnStartedCommand()
            {
                Round = e.Round,
                Cards = Game.Table.Cards.Select(x => x.ToString()).ToArray()
            });
        }

        private void OnGameGenerallyUpdated(object sender, EventArgs e)
        {
            SendTableInfo();
        }

        void OnPlayerJoined(object sender, PlayerInfoEventArgs e)
        {
            var p = e.Player;
            if(p != Player)
                Send(new PlayerJoinedCommand()
                {
                    PlayerName = p.Name,
                });
        }

        void OnSeatUpdated(object sender, SeatEventArgs e)
        {
            if (e.Seat.IsEmpty || Player.NoSeat != e.Seat.NoSeat)
            {
                if (!e.Seat.IsEmpty && Player.NoSeat != e.Seat.NoSeat && !e.Seat.Player.IsShowingCards)
                    e.Seat.Player.HoleCards = new[] {"??", "??"};

                Send(new SeatUpdatedCommand()
                {
                    Seat = e.Seat,
                });
            }
        }
        #endregion PokerObserver Event Handling

        private void Send(AbstractGameCommand c)
        {
            c.TableId = TableId;
            Client.SendCommand(c);
        }
    }
}
