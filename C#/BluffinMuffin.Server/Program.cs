using System;
using System.Configuration;
using BluffinMuffin.Server.Protocol;
using BluffinMuffin.Server.Configuration;
using BluffinMuffin.Server.Logging;

namespace BluffinMuffin.Server
{
    public static class Program
    {

        private static void Main()
        {
            var config = InitConfiguration();

            ConsoleLogger.Init(config);
            FileLogger.Init(config);
            DbCommandLogger.Init(config);

            StartServer(config);
        }

        private static BluffinMuffinDataSection InitConfiguration()
        {
            var config = ConfigurationManager.GetSection(BluffinMuffinDataSection.NAME) as BluffinMuffinDataSection;
            if (config == null)
                throw new Exception("No configuration found !!!");
            return config;
        }

        private static void StartServer(BluffinMuffinDataSection config)
        {
            try
            {
                var server = new BluffinServer(config.Port);
                server.Start();
            }
            catch
            {
                DataTypes.Logger.LogError("Can't start server !!");
            }
        }

    }
}
