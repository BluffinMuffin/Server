using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Protocol.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes.EventHandling;

namespace BluffinMuffin.Server.Logic.GameModules
{
    abstract class AbstractGameModule : IGameModule
    {
        public event EventHandler<SuccessEventArg> ModuleCompleted = delegate { };

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
    }
}
