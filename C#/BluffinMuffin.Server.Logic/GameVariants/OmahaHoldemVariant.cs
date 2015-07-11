using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BluffinMuffin.HandEvaluator.Enums;
using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Server.DataTypes.Attributes;
using BluffinMuffin.Server.DataTypes.Enums;

namespace BluffinMuffin.Server.Logic.GameVariants
{
    [GameVariant(GameVariantEnum.OmahaHoldem)]
    public class OmahaHoldemVariant : AbstractGameVariant
    {
        public override int NbCardsInHand
        {
            get { return 4; }
        }

        public override CardSelectionEnum CardSelectionType
        {
            get { return CardSelectionEnum.TwoPlayersAndThreeCommunity; }
        }
    }
}
