using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluffinMuffin.Server.Configuration
{
    public class DbCommandConfigElement : ConfigurationElement
    {
        public const string NAME = "dbCommandLogger";
        public bool HasIt => !string.IsNullOrEmpty(Url);

        private const string URL = "url";
        [ConfigurationProperty(URL, IsRequired = true)]
        public string Url
        {
            get { return (string)this[URL]; }
            set { this[URL] = value; }
        }

        private const string USER = "user";
        [ConfigurationProperty(USER, IsRequired = true)]
        public string User
        {
            get { return (string)this[USER]; }
            set { this[USER] = value; }
        }

        private const string PASSWORD = "password";
        [ConfigurationProperty(PASSWORD, IsRequired = true)]
        public string Password
        {
            get { return (string)this[PASSWORD]; }
            set { this[PASSWORD] = value; }
        }

        private const string DATABASE = "database";
        [ConfigurationProperty(DATABASE, IsRequired = true)]
        public string Database
        {
            get { return (string)this[DATABASE]; }
            set { this[DATABASE] = value; }
        }
    }
}
