using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BluffinMuffin.HandEvaluator.Enums;
using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Protocol.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes.Attributes;
using BluffinMuffin.Server.DataTypes.Enums;
using BluffinMuffin.Server.Logic.GameModules;

namespace BluffinMuffin.Server.Logic.GameVariants
{
    [GameVariant(GameVariantEnum.FiveCardsDraw)]
    public class FiveCardsDrawVariant : AbstractGameVariant
    {
        public override int NbCardsInHand
        {
            get { return 5; }
        }

        public override GameTypeEnum GameType => GameTypeEnum.DrawPoker;

        public override CardSelectionEnum CardSelectionType
        {
            get { return CardSelectionEnum.AllPlayerAndAllCommunity; }
        }

        public override Type InitModuleType
        {
            get { return typeof(InitFiveCardsDrawGameModule); }
        }
    }
}
