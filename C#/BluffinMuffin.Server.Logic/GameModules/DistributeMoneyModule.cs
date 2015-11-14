using System;
using System.Collections.Generic;
using System.Linq;
using BluffinMuffin.HandEvaluator;
using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Server.DataTypes;
using BluffinMuffin.Server.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes.EventHandling;
using BluffinMuffin.Server.Logic.Extensions;

namespace BluffinMuffin.Server.Logic.GameModules
{
    public class DistributeMoneyModule : AbstractGameModule
    {
        public DistributeMoneyModule(PokerGameObserver o, PokerTable table)
            : base(o, table)
        {
        }

        public override GameStateEnum GameState => GameStateEnum.DistributeMoney;

        private class PlayerCardHolder : IStringCardsHolder
        {
            public PlayerInfo Player { get; }
            public IEnumerable<string> PlayerCards => Player.Cards;
            public IEnumerable<string> CommunityCards { get; }

            public PlayerCardHolder(PlayerInfo p, IEnumerable<string> communityCards)
            {
                if (communityCards == null || p.Cards == null || communityCards.Union(p.Cards).Count(x => !string.IsNullOrEmpty(x)) < 5)
                    throw new Exception("No evaluation possible !!!");
                Player = p;
                CommunityCards = communityCards;
            }
        }

        public override void InitModule()
        {
            var rankedPlayers = HandEvaluators.Evaluate(Table.Seats.PlayingAndAllInPlayers().Select(x => new PlayerCardHolder(x, Table.Cards)).Cast<IStringCardsHolder>().ToArray(),Table.Variant.EvaluationParms).ToArray();
            var playerWithRanks = rankedPlayers.SelectMany(x => x.Select(p => new KeyValuePair<PlayerInfo, int>(((PlayerCardHolder)p.CardsHolder).Player, x.Key))).ToDictionary(x => x.Key, x => x.Value);
            var playerWithHands = rankedPlayers.SelectMany(x => x.Select(p => new KeyValuePair<PlayerInfo, HandEvaluationResult>(((PlayerCardHolder)p.CardsHolder).Player, p.Evaluation))).ToDictionary(x => x.Key, x => new WinningPlayer {Player=x.Key, Hand = x.Value});
            while (Table.Bank.MoneyAmount > 0)
            {
                var pot = Table.Bank.DistributeCurrentPot(playerWithRanks);
                foreach (var winner in pot.Winners)
                {
                    Observer.RaisePlayerWonPot(playerWithHands[winner.Key], winner.Value, pot.PotId, pot.TotalPotAmount);
                    WaitALittle(Table.Params.WaitingTimes.AfterPotWon);
                }
            }
            RaiseCompleted();
        }
    }
}
