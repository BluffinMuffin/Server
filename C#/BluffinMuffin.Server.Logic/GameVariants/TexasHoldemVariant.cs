using System;
using BluffinMuffin.HandEvaluator.Enums;
using BluffinMuffin.Protocol.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes.Attributes;
using BluffinMuffin.Server.DataTypes.Enums;
using BluffinMuffin.Server.Logic.GameModules;

namespace BluffinMuffin.Server.Logic.GameVariants
{
    [FavoriteGameVariant]
    [GameVariant(GameSubTypeEnum.TexasHoldem)]
    public class TexasHoldemVariant : AbstractGameVariant
    {
        public override int NbCardsInHand => 2;

        public override CardSelectionEnum CardSelectionType => CardSelectionEnum.AllPlayerAndAllCommunity;

        public override Type InitModuleType => typeof(InitHoldemGameModule);
    }
}
