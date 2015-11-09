using System.Collections.Generic;
using System.Linq;
using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Protocol.DataTypes.Enums;

namespace BluffinMuffin.Server.Logic.Extensions
{
    public static class EnumerableOfSeatInfoExtensions
    {
        public static IEnumerable<SeatInfo> WithAttribute(this IEnumerable<SeatInfo> seats, SeatAttributeEnum att)
        {
            return seats.Where(s => s.HasAttribute(att));
        }
        public static SeatInfo Dealer(this IEnumerable<SeatInfo> seats)
        {
            return seats.WithAttribute(SeatAttributeEnum.Dealer).SingleOrDefault();
        }
        public static SeatInfo FirstTalker(this IEnumerable<SeatInfo> seats)
        {
            return seats.WithAttribute(SeatAttributeEnum.FirstTalker).SingleOrDefault();
        }
        public static SeatInfo CurrentPlayer(this IEnumerable<SeatInfo> seats)
        {
            return seats.WithAttribute(SeatAttributeEnum.CurrentPlayer).SingleOrDefault();
        }
    }
}
