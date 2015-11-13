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

        public static void MoveAttributeTo(this IEnumerable<SeatInfo> seats, SeatInfo receiver, SeatAttributeEnum att)
        {
            seats.ClearAttribute(att);
            receiver?.AddAttribute(att);
        }
        public static void ClearAttribute(this IEnumerable<SeatInfo> seats, SeatAttributeEnum att)
        {
            seats.WithAttribute(att).ToList().ForEach(x => x.RemoveAttribute(att));
        }
        public static SeatInfo SeatOfDealer(this IEnumerable<SeatInfo> seats)
        {
            return seats.WithAttribute(SeatAttributeEnum.Dealer).SingleOrDefault();
        }
        public static SeatInfo SeatOfFirstTalker(this IEnumerable<SeatInfo> seats)
        {
            return seats.WithAttribute(SeatAttributeEnum.FirstTalker).SingleOrDefault();
        }
        public static int NoSeatOfCurrentPlayer(this IEnumerable<SeatInfo> seats)
        {
            return seats.SeatOfCurrentPlayer()?.NoSeat ?? -1;
        }
        public static PlayerInfo CurrentPlayer(this IEnumerable<SeatInfo> seats)
        {
            return seats.SeatOfCurrentPlayer()?.Player;
        }
        public static SeatInfo SeatOfCurrentPlayer(this IEnumerable<SeatInfo> seats)
        {
            return seats.WithAttribute(SeatAttributeEnum.CurrentPlayer).SingleOrDefault();
        }
        public static SeatInfo SeatOfPlayingPlayerNextTo(this IEnumerable<SeatInfo> seats, SeatInfo seat)
        {
            var arrSeats = seats.ToArray();
            Queue<SeatInfo> seatsBefore = new Queue<SeatInfo>();
            Queue<SeatInfo> seatsAfter = new Queue<SeatInfo>();
            bool foundMe = false;

            foreach (SeatInfo s in arrSeats)
            {
                if (!foundMe)
                    seatsBefore.Enqueue(s);
                else
                    seatsAfter.Enqueue(s);

                if (s == seat)
                    foundMe = true;
            }

            while(seatsBefore.Any())
                seatsAfter.Enqueue(seatsBefore.Dequeue());

            while (seatsAfter.Any())
            {
                SeatInfo s = seatsAfter.Dequeue();
                if (s.HasPlayerPlaying())
                    return s;
            }

            return null;
        }
        public static SeatInfo SeatOfPlayingPlayerJustBefore(this IEnumerable<SeatInfo> seats, SeatInfo seat)
        {
            return seats.Reverse().SeatOfPlayingPlayerNextTo(seat);
        }
        public static IEnumerable<PlayerInfo> Players(this IEnumerable<SeatInfo> seats)
        {
            return seats.Where(s => !s.IsEmpty).Select(s => s.Player);
        }

        public static IEnumerable<PlayerInfo> PlayingPlayers(this IEnumerable<SeatInfo> seats)
        {
            return seats.Where(s => s.HasPlayerPlaying()).Select(s => s.Player);
        }

        public static IEnumerable<PlayerInfo> PlayingAndAllInPlayers(this IEnumerable<SeatInfo> seats)
        {
            return seats.Where(s => s.HasPlayerPlayingOrAllIn()).Select(s => s.Player);
        }

        public static IEnumerable<int> RemainingSeatIds(this IEnumerable<SeatInfo> seats)
        {
            return seats.Where(x => x.IsEmpty).Select(x => x.NoSeat);
        }
    }
}
