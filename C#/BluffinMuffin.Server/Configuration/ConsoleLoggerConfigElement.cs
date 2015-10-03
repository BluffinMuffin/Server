using System.Configuration;

namespace BluffinMuffin.Server.Configuration
{
    public class ConsoleLoggerConfigElement : ConfigurationElement
    {
        public const string NAME = "consoleLogger";
        public bool HasIt => !string.IsNullOrEmpty(Level);

        private const string LEVEL = "level";
        [ConfigurationProperty(LEVEL, IsRequired = true)]
        public string Level
        {
            get { return (string)this[LEVEL]; }
            set { this[LEVEL] = value; }
        }
    }
}
