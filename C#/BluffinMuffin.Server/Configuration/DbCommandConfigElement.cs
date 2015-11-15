using System.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace BluffinMuffin.Server.Configuration
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class DbCommandConfigElement : ConfigurationElement
    {
        public const string NAME = "dbCommandLogger";
        public bool HasIt => !string.IsNullOrEmpty(Url);

        private const string URL = "url";
        [ConfigurationProperty(URL, IsRequired = true)]
        public string Url => (string)this[URL];

        private const string USER = "user";
        [ConfigurationProperty(USER, IsRequired = true)]
        public string User => (string)this[USER];

        private const string PASSWORD = "password";
        [ConfigurationProperty(PASSWORD, IsRequired = true)]
        public string Password => (string)this[PASSWORD];

        private const string DATABASE = "database";
        [ConfigurationProperty(DATABASE, IsRequired = true)]
        public string Database => (string)this[DATABASE];
    }
}
