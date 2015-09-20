using System.Collections.Generic;
using BluffinMuffin.HandEvaluator.Enums;
using BluffinMuffin.Server.DataTypes;
using static BluffinMuffin.HandEvaluator.Enums.NominalValueEnum;

namespace BluffinMuffin.Server.Logic
{
    public class Shuffled28HighCardsDealer : AbstractDealer
    {
        public override IEnumerable<NominalValueEnum> UsedValues => new[] { Eight, Nine, Ten, Jack, Queen, King, Ace };
    }
}
