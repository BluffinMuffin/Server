﻿using System.Linq;
using BluffinMuffin.HandEvaluator;
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

        public override void InitModule()
        {
            foreach (var pot in Table.Bank.DistributeMoney(HandEvaluators.Evaluate(Table.Seats.PlayingAndAllInPlayers().Select(x => new PlayerCardHolder(x, Table.Cards)), Table.Variant.EvaluationParms).SelectMany(x => x)))
            {
                foreach (var winner in pot.Winners)
                {
                    Observer.RaisePlayerWonPot(winner.Key, winner.Value, pot.PotId, pot.TotalPotAmount);
                    WaitALittle(Table.Params.WaitingTimes.AfterPotWon);
                }
            }

            Table.Seats.ClearAllAttributes();
            RaiseCompleted();
        }
    }
}
