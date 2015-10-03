using System.Configuration;

namespace BluffinMuffin.Server.Configuration
{
    public class BluffinMuffinDataSection : ConfigurationSection
    {
        public const string NAME = "bluffinMuffin";

        [ConfigurationProperty(LoggingDataSection.NAME, IsRequired = true)]
        public LoggingDataSection Logging => (LoggingDataSection)base[LoggingDataSection.NAME];

        private const string PORT = "port";
        [ConfigurationProperty(PORT, IsRequired = true)]
        public int Port
        {
            get { return (int)this[PORT]; }
            set { this[PORT] = value; }
        }
    }
}