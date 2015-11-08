using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Protocol.DataTypes.Enums;

namespace BluffinMuffin.Server.Logic.Extensions
{
    public static class SeatInfoExtensions
    {
        public static bool HasPlayerPlaying(this SeatInfo si) => !si.IsEmpty && si.Player.State == PlayerStateEnum.Playing;
        public static bool HasPlayerPlayingOrAllIn(this SeatInfo si) => !si.IsEmpty && si.Player.IsPlayingOrAllIn();
    }
}
