using System;
using System.Collections.Generic;
using System.Linq;
using BluffinMuffin.Protocol;
using BluffinMuffin.Protocol.DataTypes.Enums;
using BluffinMuffin.Protocol.DataTypes.EventHandling;
using BluffinMuffin.Server.DataTypes.EventHandling;
using BluffinMuffin.Server.Logic;
using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Protocol.Game;
using BluffinMuffin.Server.Protocol.DataTypes;

namespace BluffinMuffin.Server.Protocol
{
    public class RemotePlayer
    {
        public PokerGame Game { get; }
        public PlayerInfo Player { get; }
        public IBluffinClient Client { get; }
        public int TableId { get; }

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
            Game.Observer.PlayerJoined += OnPlayerJoined;
            Game.Observer.SeatUpdated += OnSeatUpdated;
            Game.Observer.DiscardActionNeeded += OnDiscardActionNeeded;
        }

        void OnDiscardActionNeeded(object sender, MinMaxEventArgs e)
        {
            Send(new DiscardRoundStartedCommand()
            {
                MinimumCardsToDiscard = e.Minimum,
                MaximumCardsToDiscard = e.Maximum,
            });
        }
        #region PokerObserver Event Handling
        void OnGameBettingRoundEnded(object sender, EventArgs e)
        {
            Send(new BetTurnEndedCommand()
            {
                PotsAmounts = Game.Table.PotAmountsPadded.ToList(),
            });
        }

        void OnPlayerHoleCardsChanged(object sender, PlayerInfoEventArgs e)
        {
            var p = e.Player;

            Send(new PlayerHoleCardsChangedCommand()
            {
                NoSeat = p.NoSeat,
                PlayerState = p.State,
                FaceDownCards = p.NoSeat == Player.NoSeat ? p.FaceDownCards : p.FaceDownCards.Select(x => "??").ToArray(),
                FaceUpCards = p.FaceUpCards
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
                WinningCards = playerInfo.Hand?.Cards.SelectMany(x => x).Take(5).Select(x => x.ToString()).ToArray() ?? new string[0],
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

        void OnPlayerActionNeeded(object sender, ActionNeededEventArgs e)
        {
            Send(new PlayerTurnBeganCommand()
            {
                NoSeat = e.Player.NoSeat,
                AmountNeeded = e.AmountNeeded,
                MinimumRaiseAmount = e.MinimumRaiseAmount,
                MaximumRaiseAmount = e.MaximumRaiseAmount,
                CanFold = e.CanFold
            });
        }

        void OnGameBlindNeeded(object sender, EventArgs e)
        {
            Send(new GameStartedCommand()
            {
                NeededBlindAmount = Game.Table.GetBlindNeeded(Player),
                Seats = AllSeats().ToList()
            });
        }

        public IEnumerable<SeatInfo> AllSeats()
        {
            for (var i = 0; i < Game.Table.Params.MaxPlayers; ++i)
            {
                var si = new SeatInfo() { NoSeat = i };
                var gameSeat = Game.Table.Seats[i];
                if (!gameSeat.IsEmpty)
                {
                    si.Player = gameSeat.Player.Clone();
                    PepareCardsForSending(si);

                    si.SeatAttributes = gameSeat.SeatAttributes;
                }
                yield return si;
            }
        }

        void OnGameBettingRoundStarted(object sender, EventArgs e)
        {
            Send(new BetTurnStartedCommand()
            {
                Cards = Game.Table.Cards.Select(x => x.ToString()).ToArray(),
                BettingRoundId = Game.Table.BettingRoundId,
                Seats = AllSeats().ToList()
            });
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
                if (!e.Seat.IsEmpty)
                    PepareCardsForSending(e.Seat);

                Send(new SeatUpdatedCommand()
                {
                    Seat = e.Seat,
                });
            }
        }

        private void PepareCardsForSending(SeatInfo si)
        {
            if (si.Player.FaceDownCards == null)
                si.Player.FaceDownCards = new string[0];

            if (si.Player.FaceUpCards == null)
                si.Player.FaceUpCards = new string[0];

            //If we are not sending the info about the player who is receiving, don't show the cards
            if (si.NoSeat != Player.NoSeat)
                si.Player.FaceDownCards = si.Player.FaceDownCards.Select(x => "??").ToArray();
        }

        #endregion PokerObserver Event Handling

        private void Send(AbstractGameCommand c)
        {
            c.TableId = TableId;
            Client.SendCommand(c);
        }
    }
}
