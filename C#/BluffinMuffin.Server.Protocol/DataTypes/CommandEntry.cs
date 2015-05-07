using BluffinMuffin.Protocol;

namespace BluffinMuffin.Server.Protocol.DataTypes
{
    public class CommandEntry
    {
        public AbstractBluffinCommand Command { get; set; }
        public IBluffinClient Client { get; set; }
    }
}
