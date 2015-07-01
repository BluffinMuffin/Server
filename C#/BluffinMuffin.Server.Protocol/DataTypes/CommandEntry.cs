using BluffinMuffin.Protocol;

namespace BluffinMuffin.Server.Protocol.DataTypes
{
    public class CommandEntry
    {
        public AbstractCommand Command { get; set; }
        public IBluffinClient Client { get; set; }
    }
}
