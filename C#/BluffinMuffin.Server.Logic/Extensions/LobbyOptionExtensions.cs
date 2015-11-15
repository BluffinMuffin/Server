using BluffinMuffin.Protocol.DataTypes.Enums;
using BluffinMuffin.Protocol.DataTypes.Options;

namespace BluffinMuffin.Server.Logic.Extensions
{
    public static class LobbyOptionExtensions
    {

        public static int MinimumBuyInAmount(this LobbyOptions lo, int gameSize)
        {
            switch (lo.MinimumBuyInParameter)
            {
                case BuyInParameterEnum.FixedAmount:
                    return lo.MinimumBuyInValue;
                case BuyInParameterEnum.Multiplicator:
                    return lo.MinimumBuyInValue * gameSize;
            }
            return 0;
        }

        public static int MaximumBuyInAmount(this LobbyOptions lo, int gameSize)
        {
            switch (lo.MaximumBuyInParameter)
            {
                case BuyInParameterEnum.FixedAmount:
                    return lo.MaximumBuyInValue;
                case BuyInParameterEnum.Multiplicator:
                    return lo.MaximumBuyInValue * gameSize;
            }
            return int.MaxValue;
        }
    }
}
