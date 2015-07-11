using System;
using System.Threading;
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

        public virtual void EndModule()
        {

        }

        public virtual void EndErrorModule()
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

        public virtual bool OnCardDiscarded(PlayerInfo p, string[] cards)
        {
            return false;
        }

        protected void RaiseCompleted()
        {
            EndModule();
            ModuleCompleted(this, new SuccessEventArg() { Success = true });
        }
        protected void RaiseAborted()
        {
            EndErrorModule();
            ModuleCompleted(this, new SuccessEventArg() { Success = false });
        }

        protected void AddModule(IGameModule module)
        {
            ModuleGenerated(this, new ModuleEventArg() {Module = module});
        }
        protected void WaitALittle(int waitingTime)
        {
            Thread.Sleep(waitingTime);
        }
    }
}
