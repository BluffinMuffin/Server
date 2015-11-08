using System;
using System.Linq;
using BluffinMuffin.HandEvaluator;
using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Protocol.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes;
using BluffinMuffin.Server.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes.EventHandling;
using BluffinMuffin.Server.Logic.Extensions;

namespace BluffinMuffin.Server.Logic.GameModules
{
    public class BettingRoundModule : AbstractGameModule
    {
        public BettingRoundModule(PokerGameObserver o, PokerTable table)
            : base(o, table)
        {
        }

        public override GameStateEnum GameState => GameStateEnum.Playing;

        protected virtual void InitModuleSpecific()
        {
            
        }

        public override void InitModule()
        {
            if (Table.NoMoreRoundsNeeded)
            {
                RaiseCompleted();
                return;
            }
            Table.BettingRoundId++;

            if (Table.FirstTalkerSeat != null)
                Table.FirstTalkerSeat.SeatAttributes = Table.FirstTalkerSeat.SeatAttributes.Except(new[] { SeatAttributeEnum.FirstTalker }).ToArray();

            if (Table.FirstTalkerSeat != null)
                Table.FirstTalkerSeat.SeatAttributes = Table.FirstTalkerSeat.SeatAttributes.Except(new[] { SeatAttributeEnum.FirstTalker }).ToArray();

            var firstPlayer = GetSeatOfTheFirstPlayer();

            if (Table.Params.Options.OptionType == GameTypeEnum.StudPoker)
                firstPlayer.SeatAttributes = firstPlayer.SeatAttributes.Union(new[] { SeatAttributeEnum.FirstTalker }).ToArray();

            Table.ChangeCurrentPlayerTo(null);
            Observer.RaiseGameBettingRoundStarted();

            //We Put the current player just before the starting player, then we will take the next player and he will be the first
            Table.ChangeCurrentPlayerTo(Table.GetSeatOfPlayingPlayerJustBefore(firstPlayer));


            Table.NbPlayed = 0;
            Table.MinimumRaiseAmount = Table.Params.GameSize;
            InitModuleSpecific();

            WaitALittle(Table.Params.WaitingTimes.AfterBoardDealed);

            if (Table.NbPlaying <= 1 || Table.NbPlayingAndAllIn == 1 || Table.NbPlayed >= Table.NbPlayingAndAllIn)
                EndBettingRound();
            else
                ChooseNextPlayer();
        }

        protected virtual bool CanFold()
        {
            return true;
        }

        protected virtual SeatInfo GetSeatOfTheFirstPlayer()
        {
            if (Table.Params.Options.OptionType == GameTypeEnum.StudPoker)
            {
                return Table.Seats[HandEvaluators.Evaluate(Table.PlayingPlayers.Select(p => new CardHolder(p, p.FaceUpCards, new string[0])).Cast<IStringCardsHolder>().ToArray(), new EvaluationParams {UseSuitRanking = true}).First().Select(x => x.CardsHolder).Cast<CardHolder>().First().Player.NoSeat];
            }

            return Table.GetSeatOfPlayingPlayerNextTo(Table.DealerSeat);
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
            if (p.State != PlayerStateEnum.AllIn)
                Table.NbPlayed++;

            Table.HigherBet = p.MoneyBetAmnt;

            WaitALittle(Table.Params.WaitingTimes.AfterPlayerAction);

            Observer.RaisePlayerActionTaken(p, GameActionEnum.Raise, played);
        }
        protected virtual void ContinueBettingRound()
        {
            if (Table.NbPlayingAndAllIn == 1 || Table.NbPlayed >= Table.NbPlayingAndAllIn)
                EndBettingRound();
            else
                ChooseNextPlayer();
        }
        private void EndBettingRound()
        {
            RaiseCompleted();
        }
        protected virtual void ChooseNextPlayer()
        {
            var next = Table.GetSeatOfPlayingPlayerNextTo(Table.CurrentPlayerSeat);

            Table.ChangeCurrentPlayerTo(next);

            Observer.RaisePlayerActionNeeded(next.Player, Table.CallAmnt(next.Player), CanFold(), Table.MinimumRaiseAmount, int.MaxValue);

            if (next.Player.State==PlayerStateEnum.Zombie)
            {
                if (Table.CanCheck(next.Player))
                    OnMoneyPlayed(next.Player, 0);
                else
                    OnMoneyPlayed(next.Player, -1);
            }
        }

        public override bool OnMoneyPlayed(PlayerInfo p, int amnt)
        {
            Logger.LogDebugInformation("Currently, we need {0} minimum money from this player", Table.CallAmnt(p));

            //Validation: Is it the player's turn to play ?
            if (p.NoSeat != Table.NoSeatCurrentPlayer)
            {
                Logger.LogWarning("{0} just played but it wasn't his turn", p.Name);
                return false;
            }

            //The Player is Folding
            if (amnt == -1)
            {
                Logger.LogDebugInformation("{0} FOLDED", p.Name);
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
                    Logger.LogWarning("{0} needed to play at least {1} and tried {2}", p.Name, amntNeeded, amnt);
                    return false;
                }

                //Else, well, that's ok, the player is All-In !
                amntNeeded = amnt;
            }

            if (amnt > amntNeeded && amnt < Table.MinRaiseAmnt(p))
            {
                Logger.LogWarning("{0} needed to play at least {1} to raise (CallAmount + MinRaiseAmount) and tried {2}", p.Name, Table.MinRaiseAmnt(p), amnt);
                return false;
            }

            //Let's hope the player has enough money ! Time to Bet!
            if (!p.TryBet(amnt))
            {
                Logger.LogWarning("{0} just put more money than he actually have ({1} > {2})", p.Name, amnt, p.MoneySafeAmnt);
                return false;
            }

            //Update the MinimumRaiseAmount
            Table.MinimumRaiseAmount = Math.Max(Table.MinimumRaiseAmount, amnt - amntNeeded);

            //Is the player All-In?
            if (p.MoneySafeAmnt == 0)
            {
                Logger.LogDebugInformation("Player now All-In !");
                p.State = PlayerStateEnum.AllIn;
                Table.NbAllIn++;
                Table.AddAllInCap(p.MoneyBetAmnt);
            }

            //Hmmm ... More Money !! 
            Table.TotalPotAmnt += amnt;

            //Was it a CALL or a RAISE ?
            if (amnt == amntNeeded)
            {
                Logger.LogDebugInformation("{0} CALLED WITH ${1}", p.Name, amnt);
                CallPlayer(p, amnt);
            }
            else
            {
                Logger.LogDebugInformation("{0} RAISED WITH ${1}", p.Name, amnt);
                RaisePlayer(p, amnt);
            }

            // Ok this player received enough attention !
            ContinueBettingRound();

            return true;
        }
    }
}
