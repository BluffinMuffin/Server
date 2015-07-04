using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Server.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes.EventHandling;
using Com.Ericmas001.Net.Protocol.Annotations;

namespace BluffinMuffin.Server.Logic.GameModules
{
    public interface IGameModule
    {
        event EventHandler<SuccessEventArg> ModuleCompleted;
        event EventHandler<ModuleEventArg> ModuleGenerated;
        GameStateEnum GameState { get; }

        void InitModule();
        void OnSitOut();
        void OnSitIn();
        bool OnMoneyPlayed(PlayerInfo p, int amount);
    }
}
