using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Protocol.DataTypes.Enums;
using System;
using BluffinMuffin.Protocol.DataTypes.EventHandling;

namespace BluffinMuffin.Server.DataTypes.EventHandling
{
    public class PokerGameObserver
    {
        private readonly IPokerGame m_Game;

        public event EventHandler EverythingEnded = delegate { };
        public event EventHandler GameBlindNeeded = delegate { };
        public event EventHandler GameEnded = delegate { };
        public event EventHandler GameBettingRoundStarted = delegate { };
        public event EventHandler GameBettingRoundEnded = delegate { };
        public event EventHandler<PlayerInfoEventArgs> PlayerJoined = delegate { };
        public event EventHandler<PlayerInfoEventArgs> PlayerHoleCardsChanged = delegate { };
        public event EventHandler<SeatEventArgs> SeatUpdated = delegate { };
        public event EventHandler<ActionNeededEventArgs> PlayerActionNeeded = delegate { };
        public event EventHandler<PotWonEventArgs> PlayerWonPot = delegate { };
        public event EventHandler<PlayerActionEventArgs> PlayerActionTaken = delegate { };
        public event EventHandler<MinMaxEventArgs> DiscardActionNeeded = delegate { };

        public PokerGameObserver(IPokerGame game)
        {
            m_Game = game;
        }

        public void RaiseEverythingEnded()
        {
            EverythingEnded(m_Game, new EventArgs());
        }
        public void RaiseGameBlindNeeded()
        {
            GameBlindNeeded(m_Game, new EventArgs());
        }
        public void RaiseGameEnded()
        {
            GameEnded(m_Game, new EventArgs());
        }
        public void RaiseGameBettingRoundStarted()
        {
            GameBettingRoundStarted(m_Game, new EventArgs());
        }
        public void RaiseGameBettingRoundEnded()
        {
            GameBettingRoundEnded(m_Game, new EventArgs());
        }
        public void RaisePlayerJoined(PlayerInfo p)
        {
            PlayerJoined(m_Game, new PlayerInfoEventArgs(p));
        }
        public void RaisePlayerHoleCardsChanged(PlayerInfo p)
        {
            PlayerHoleCardsChanged(m_Game, new PlayerInfoEventArgs(p));
        }
        public void RaiseSeatUpdated(SeatInfo s)
        {
            SeatUpdated(m_Game, new SeatEventArgs(s));
        }
        public void RaisePlayerActionNeeded(PlayerInfo p, int amountNeeded, bool canFold, int minimumRaiseAmount, int maximumRaiseAmount)
        {
            PlayerActionNeeded(m_Game, new ActionNeededEventArgs(p, amountNeeded, canFold, minimumRaiseAmount, maximumRaiseAmount));
        }
        public void RaiseDiscardActionNeeded(int min, int max)
        {
            DiscardActionNeeded(m_Game, new MinMaxEventArgs(min,max));
        }
        public void RaisePlayerWonPot(WinningPlayer player, MoneyPot pot, int amntWon)
        {
            PlayerWonPot(m_Game, new PotWonEventArgs(player, pot, amntWon));
        }
        public void RaisePlayerActionTaken(PlayerInfo p, GameActionEnum action, int amnt)
        {
            PlayerActionTaken(m_Game, new PlayerActionEventArgs(p, action, amnt));
        }
    }
}
