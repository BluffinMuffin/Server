using System.Collections.Generic;
using System.Linq;
using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Protocol.DataTypes.Enums;

namespace BluffinMuffin.Server.Logic.Extensions
{
    public static class EnumerableOfSeatInfoExtensions
    {

        /// <summary>
        /// Filters Seats that contains the attribute
        /// </summary>
        /// <param name="seats">Seat collection</param>
        /// <param name="att">Expected attribute</param>
        /// <returns>Seats that contains the attribute</returns>
        public static IEnumerable<SeatInfo> WithAttribute(this IEnumerable<SeatInfo> seats, SeatAttributeEnum att)
        {
            return seats.Where(s => s.HasAttribute(att));
        }

        /// <summary>
        /// Takes away the attribute from the seats having it, and gives it to the new one who needs it
        /// </summary>
        /// <param name="seats">Seat collection</param>
        /// <param name="receiver">Seat that needs the attribute</param>
        /// <param name="att">The attribute</param>
        public static void MoveAttributeTo(this IEnumerable<SeatInfo> seats, SeatInfo receiver, SeatAttributeEnum att)
        {
            seats.ClearAttribute(att);
            receiver?.AddAttribute(att);
        }

        /// <summary>
        /// Removes the attribute from every seats
        /// </summary>
        /// <param name="seats">Seat collection</param>
        /// <param name="att">The attribute</param>
        public static void ClearAttribute(this IEnumerable<SeatInfo> seats, SeatAttributeEnum att)
        {
            seats.WithAttribute(att).ToList().ForEach(x => x.RemoveAttribute(att));
        }

        /// <summary>
        /// Removes every attributes from every seats
        /// </summary>
        /// <param name="seats">Seat collection</param>
        public static void ClearAllAttributes(this IEnumerable<SeatInfo> seats)
        {
            seats.ToList().ForEach(s => s.SeatAttributes = new SeatAttributeEnum[0]);
        }

        /// <summary>
        /// Get the seat of the dealer
        /// </summary>
        /// <param name="seats">Seat collection</param>
        /// <returns>Seat of the dealer</returns>
        public static SeatInfo SeatOfDealer(this IEnumerable<SeatInfo> seats)
        {
            return seats.WithAttribute(SeatAttributeEnum.Dealer).SingleOrDefault();
        }

        /// <summary>
        /// Get the seat who should be the small blind, relative to the dealer
        /// </summary>
        /// <param name="seats">Seat collection</param>
        /// <returns>Seat who should be the small blind></returns>
        public static SeatInfo SeatOfShouldBeSmallBlind(this IEnumerable<SeatInfo> seats)
        {
            var ss = seats.ToArray();
            return ss.PlayingPlayers().Count() == 2 ? ss.SeatOfDealer() : ss.SeatOfPlayingPlayerNextTo(ss.SeatOfDealer());
        }

        /// <summary>
        /// Get the seat who should be the big blind, relative to the small blind
        /// </summary>
        /// <param name="seats">Seat collection</param>
        /// <returns>Seat who should be the big blind</returns>
        public static SeatInfo SeatOfShouldBeBigBlind(this IEnumerable<SeatInfo> seats)
        {
            var ss = seats.ToArray();
            return ss.SeatOfPlayingPlayerNextTo(ss.SeatOfShouldBeSmallBlind());
        }

        /// <summary>
        /// Get the seat of the first talker
        /// </summary>
        /// <param name="seats">Seat collection</param>
        /// <returns>Seat of the first talker</returns>
        public static SeatInfo SeatOfFirstTalker(this IEnumerable<SeatInfo> seats)
        {
            return seats.WithAttribute(SeatAttributeEnum.FirstTalker).SingleOrDefault();
        }

        /// <summary>
        /// Get the no seat of the current player
        /// </summary>
        /// <param name="seats">Seat collection</param>
        /// <returns>NoSeat of the current player</returns>
        public static int NoSeatOfCurrentPlayer(this IEnumerable<SeatInfo> seats)
        {
            return seats.SeatOfCurrentPlayer()?.NoSeat ?? -1;
        }

        /// <summary>
        /// Get the current player
        /// </summary>
        /// <param name="seats">Seat collection</param>
        /// <returns>Current player</returns>
        public static PlayerInfo CurrentPlayer(this IEnumerable<SeatInfo> seats)
        {
            return seats.SeatOfCurrentPlayer()?.Player;
        }

        /// <summary>
        /// Get the seat of the current player
        /// </summary>
        /// <param name="seats">Seat collection</param>
        /// <returns>Seat of the current player</returns>
        public static SeatInfo SeatOfCurrentPlayer(this IEnumerable<SeatInfo> seats)
        {
            return seats.WithAttribute(SeatAttributeEnum.CurrentPlayer).SingleOrDefault();
        }

        /// <summary>
        /// Get the seat of the next playing player after a specifc seat
        /// </summary>
        /// <param name="seats">Seat collection</param>
        /// <param name="seat">Specifc seat</param>
        /// <returns>Seat of the next playing player</returns>
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

        /// <summary>
        /// Get the seat of the previous playing player before a specifc seat
        /// </summary>
        /// <param name="seats">Seat collection</param>
        /// <param name="seat">Specifc seat</param>
        /// <returns>Seat of the previous playing player</returns>
        public static SeatInfo SeatOfPlayingPlayerJustBefore(this IEnumerable<SeatInfo> seats, SeatInfo seat)
        {
            return seats.Reverse().SeatOfPlayingPlayerNextTo(seat);
        }

        /// <summary>
        /// Gets all the players currently around the table
        /// </summary>
        /// <param name="seats">Seat collection</param>
        /// <returns>All players</returns>
        public static IEnumerable<PlayerInfo> Players(this IEnumerable<SeatInfo> seats)
        {
            return seats.Where(s => !s.IsEmpty).Select(s => s.Player);
        }

        /// <summary>
        /// Gets all the playing players currently around the table
        /// </summary>
        /// <param name="seats">Seat collection</param>
        /// <returns>All playing players</returns>
        public static IEnumerable<PlayerInfo> PlayingPlayers(this IEnumerable<SeatInfo> seats)
        {
            return seats.Where(s => s.HasPlayerPlaying()).Select(s => s.Player);
        }

        /// <summary>
        /// Gets all the all-in players currently around the table
        /// </summary>
        /// <param name="seats">Seat collection</param>
        /// <returns>All all-in players</returns>
        public static IEnumerable<PlayerInfo> AllInPlayers(this IEnumerable<SeatInfo> seats)
        {
            return seats.Where(s => s.HasPlayerAllIn()).Select(s => s.Player);
        }

        /// <summary>
        /// Gets all the playing and all-in players currently around the table
        /// </summary>
        /// <param name="seats">Seat collection</param>
        /// <returns>All playing and all-in players</returns>
        public static IEnumerable<PlayerInfo> PlayingAndAllInPlayers(this IEnumerable<SeatInfo> seats)
        {
            return seats.Where(s => s.HasPlayerPlayingOrAllIn()).Select(s => s.Player);
        }

        /// <summary>
        /// Gets all the empty seats around the table
        /// </summary>
        /// <param name="seats">Seat collection</param>
        /// <returns>All the empty seats</returns>
        public static IEnumerable<int> RemainingSeatIds(this IEnumerable<SeatInfo> seats)
        {
            return seats.Where(x => x.IsEmpty).Select(x => x.NoSeat);
        }
    }
}
