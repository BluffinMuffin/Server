using System.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace BluffinMuffin.Server.Configuration
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class FileLoggerConfigElement : ConfigurationElement
    {
        public const string NAME = "fileLogger";
        public const string LVL_NORMAL = "NORMAL";
        public const string LVL_DEBUG = "DEBUG";
        public const string LVL_VERBOSE = "VERBOSE";

        private const string LEVEL = "level";
        [ConfigurationProperty(LEVEL, IsRequired = true)]
        public string Level => (string)this[LEVEL];
    }
}
