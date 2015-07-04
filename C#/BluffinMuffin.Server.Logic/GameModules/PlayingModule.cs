using System;
using System.Linq;
using System.Threading;
using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Protocol.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes.EventHandling;
using Com.Ericmas001.Util;

namespace BluffinMuffin.Server.Logic.GameModules
{
    class PlayingModule : AbstractGameModule
    {
        private RoundStateEnum m_RoundState; // L'etat de la game pour chaque round
        public PlayingModule(PokerGameObserver o, PokerTable table)
            : base(o, table)
        {
        }

        public override GameStateEnum GameState
        {
            get { return GameStateEnum.Playing; }
        }

        public override void InitModule()
        {
            Table.Round = RoundTypeEnum.Preflop;
            m_RoundState = RoundStateEnum.Cards;
            StartRound();
        }

        private void AdvanceToNextRound()
        {
            if (Table.Round == RoundTypeEnum.River)
                RaiseCompleted(); //Advancing to Showdown State
            else
            {
                m_RoundState = RoundStateEnum.Cards;
                Table.ChangeCurrentPlayerTo(Table.DealerSeat);
                Table.Round = (RoundTypeEnum)(((int)Table.Round) + 1);
                StartRound();
            }
        }
        private void AdvanceToNextRoundState()
        {
            if (m_RoundState == RoundStateEnum.Cumul)
                return;

            m_RoundState = (RoundStateEnum)(((int)m_RoundState) + 1);
            StartRound();
        }
        private void StartRound()
        {
            switch (m_RoundState)
            {
                case RoundStateEnum.Cards:
                    StartCardRound();
                    break;
                case RoundStateEnum.Betting:
                    StartBettingRound();
                    break;
                case RoundStateEnum.Cumul:
                    StartCumulRound();
                    break;
            }
        }
        private void StartCumulRound()
        {
            Table.ManagePotsRoundEnd();

            Observer.RaiseGameBettingRoundEnded(Table.Round);

            if (Table.NbPlayingAndAllIn <= 1)
                RaiseCompleted();
            else
                AdvanceToNextRound(); //Advancing to Next Round
        }
        private void StartBettingRound()
        {
            Observer.RaiseGameBettingRoundStarted(Table.Round);

            //We Put the current player just before the starting player, then we will take the next player and he will be the first
            Table.ChangeCurrentPlayerTo(Table.GetSeatOfPlayingPlayerJustBefore(Table.SeatOfTheFirstPlayer));
            Table.NbPlayed = 0;
            Table.MinimumRaiseAmount = Table.Params.MoneyUnit;

            WaitALittle(Table.Params.WaitingTimes.AfterBoardDealed);

            if (Table.NbPlaying <= 1)
                EndBettingRound();
            else
                ContinueBettingRound();
        }
        private void FoldPlayer(PlayerInfo p)
        {
            if (p.State != PlayerStateEnum.Zombie)
                p.State = PlayerStateEnum.SitIn;

            WaitALittle(Table.Params.WaitingTimes.AfterPlayerAction);

            Observer.RaisePlayerActionTaken(p, GameActionEnum.Fold, -1);
        }
        private void CallPlayer(PlayerInfo p, int played)
        {
            Table.NbPlayed++;

            WaitALittle(Table.Params.WaitingTimes.AfterPlayerAction);

            Observer.RaisePlayerActionTaken(p, GameActionEnum.Call, played);
        }
        private void RaisePlayer(PlayerInfo p, int played)
        {
            // Since every raise "restart" the round, 
            // the number of players who played is the number of AllIn players plus the raising player
            Table.NbPlayed = Table.NbAllIn;
            if (!p.IsAllIn)
                Table.NbPlayed++;

            Table.HigherBet = p.MoneyBetAmnt;

            WaitALittle(Table.Params.WaitingTimes.AfterPlayerAction);

            Observer.RaisePlayerActionTaken(p, GameActionEnum.Raise, played);
        }
        private void WaitALittle(int waitingTime)
        {
            Thread.Sleep(waitingTime);
        }
        private void ContinueBettingRound()
        {
            if (Table.NbPlayingAndAllIn == 1 || Table.NbPlayed >= Table.NbPlayingAndAllIn)
                EndBettingRound();
            else
                ChooseNextPlayer();
        }
        private void EndBettingRound()
        {
            AdvanceToNextRoundState(); // Advance to Cumul Round State
        }
        private void ChooseNextPlayer()
        {
            var next = Table.GetSeatOfPlayingPlayerNextTo(Table.CurrentPlayerSeat);

            Table.ChangeCurrentPlayerTo(next);

            Observer.RaisePlayerActionNeeded(next.Player);

            if (next.Player.IsZombie)
            {
                if (Table.CanCheck(next.Player))
                    OnMoneyPlayed(next.Player, 0);
                else
                    OnMoneyPlayed(next.Player, -1);
            }
        }
        private void StartCardRound()
        {
            switch (Table.Round)
            {
                case RoundTypeEnum.Preflop:
                    DealHole();
                    break;
                case RoundTypeEnum.Flop:
                    DealFlop();
                    break;
                case RoundTypeEnum.Turn:
                    DealTurn();
                    break;
                case RoundTypeEnum.River:
                    DealRiver();
                    break;
            }

            AdvanceToNextRoundState(); // Advance to Betting Round State
        }
        private void DealRiver()
        {
            Table.AddCards(Table.Dealer.DealRiver().ToString());
        }
        private void DealTurn()
        {
            Table.AddCards(Table.Dealer.DealTurn().ToString());
        }
        private void DealFlop()
        {
            Table.AddCards(Table.Dealer.DealFlop().Select(x => x.ToString()).ToArray());
        }
        private void DealHole()
        {
            foreach (var p in Table.PlayingAndAllInPlayers)
            {
                p.HoleCards = Table.Dealer.DealHoles().Select(x => x.ToString()).ToArray();
                Observer.RaisePlayerHoleCardsChanged(p);
            }
        }
        public override bool OnMoneyPlayed(PlayerInfo p, int amnt)
        {
            LogManager.Log(LogLevel.MessageVeryLow, "PokerGame.PlayMoney", "Currently, we need {0} minimum money from this player", Table.CallAmnt(p));

            //Validation: Is it the player's turn to play ?
            if (p.NoSeat != Table.NoSeatCurrentPlayer)
            {
                LogManager.Log(LogLevel.Warning, "PokerGame.PlayMoney", "{0} just played but it wasn't his turn", p.Name);
                return false;
            }

            //The Player is Folding
            if (amnt == -1)
            {
                LogManager.Log(LogLevel.MessageLow, "PokerGame.PlayMoney", "{0} FOLDED", p.Name);
                FoldPlayer(p);
                ContinueBettingRound();
                return true;
            }

            var amntNeeded = Table.CallAmnt(p);

            //Validation: Is the player betting under what he needs to Call ?
            if (amnt < amntNeeded)
            {
                //Validation: Can the player bet more ? If yes, this is not fair.
                if (p.CanBet(amnt + 1))
                {
                    LogManager.Log(LogLevel.Warning, "PokerGame.PlayMoney", "{0} needed to play at least {1} and tried {2}", p.Name, amntNeeded, amnt);
                    return false;
                }

                //Else, well, that's ok, the player is All-In !
                amntNeeded = amnt;
            }

            if (amnt > amntNeeded && amnt < Table.MinRaiseAmnt(p))
            {
                LogManager.Log(LogLevel.Warning, "PokerGame.PlayMoney", "{0} needed to play at least {1} to raise and tried {2}", p.Name, amntNeeded, amnt);
                return false;
            }

            //Let's hope the player has enough money ! Time to Bet!
            if (!p.TryBet(amnt))
            {
                LogManager.Log(LogLevel.Warning, "PokerGame.PlayMoney", "{0} just put more money than he actually have ({1} > {2})", p.Name, amnt, p.MoneySafeAmnt);
                return false;
            }

            //Update the MinimumRaiseAmount
            Table.MinimumRaiseAmount = Math.Max(Table.MinimumRaiseAmount, p.MoneyBetAmnt);

            //Is the player All-In?
            if (p.MoneySafeAmnt == 0)
            {
                LogManager.Log(LogLevel.MessageVeryLow, "PokerGame.PlayMoney", "Player now All-In !");
                p.State = PlayerStateEnum.AllIn;
                Table.NbAllIn++;
                Table.AddAllInCap(p.MoneyBetAmnt);
            }

            //Hmmm ... More Money !! 
            Table.TotalPotAmnt += amnt;

            //Was it a CALL or a RAISE ?
            if (amnt == amntNeeded)
            {
                LogManager.Log(LogLevel.MessageLow, "PokerGame.PlayMoney", "{0} CALLED WITH ${1}", p.Name, amnt);
                CallPlayer(p, amnt);
            }
            else
            {
                LogManager.Log(LogLevel.MessageLow, "PokerGame.PlayMoney", "{0} RAISED WITH ${1}", p.Name, amnt);
                RaisePlayer(p, amnt);
            }

            // Ok this player received enough attention !
            ContinueBettingRound();

            return true;
        }
    }
}
