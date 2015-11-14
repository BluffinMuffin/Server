using System;
using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Server.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes.EventHandling;

namespace BluffinMuffin.Server.DataTypes
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
        void OnCardDiscarded(PlayerInfo p, string[] cards);
    }
}
