using System.Configuration;

namespace BluffinMuffin.Server.Configuration
{
    public class FileLoggerConfigElement : ConfigurationElement
    {
        public const string NAME = "fileLogger";
        public const string LVL_NORMAL = "NORMAL";
        public const string LVL_DEBUG = "DEBUG";
        public const string LVL_VERBOSE = "VERBOSE";

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
