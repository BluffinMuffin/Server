using System;
using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Server.DataTypes;
using BluffinMuffin.Server.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes.EventHandling;

namespace BluffinMuffin.Server.Logic.GameModules
{
    public abstract class AbstractGameModule : IGameModule
    {
        public event EventHandler<SuccessEventArg> ModuleCompleted = delegate { };
        public event EventHandler<ModuleEventArg> ModuleGenerated = delegate { };

        public abstract GameStateEnum GameState { get; }

        protected PokerGameObserver Observer { get; private set; }
        protected PokerTable Table { get; private set; }

        public AbstractGameModule(PokerGameObserver o, PokerTable table)
        {
            Observer = o;
            Table = table;
        }

        public virtual void InitModule()
        {

        }
        public virtual void OnSitIn()
        {

        }
        public virtual void OnSitOut()
        {

        }

        public virtual bool OnMoneyPlayed(PlayerInfo p, int amount)
        {
            return false;
        }

        protected void RaiseCompleted()
        {
            ModuleCompleted(this, new SuccessEventArg() { Success = true });
        }
        protected void RaiseAborted()
        {
            ModuleCompleted(this, new SuccessEventArg() { Success = false });
        }

        protected void AddModule(IGameModule module)
        {
            ModuleGenerated(this, new ModuleEventArg() {Module = module});
        }
    }
}
