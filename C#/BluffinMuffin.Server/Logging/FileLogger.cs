using System;
using System.IO;
using System.Reflection;
using BluffinMuffin.Server.Configuration;

namespace BluffinMuffin.Server.Logging
{
    public class FileLogger
    {
        public static void Init(BluffinMuffinDataSection config)
        {
            var uri = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            var path = $"{Path.GetDirectoryName(uri.LocalPath + uri.Fragment)}\\log";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var logName = DateTime.Now.ToString("yyyy-MM-dd.HH-mm-ss");
            switch (config.Logging.File.Level)
            {
                case FileLoggerConfigElement.LVL_VERBOSE:
                {
                    var sw = File.CreateText($"{path}\\server.{logName}.verbose.txt");
                    sw.AutoFlush = true;
                    DataTypes.Logger.ErrorLogged += (sender, args) => LogError(sw, args.Str);
                    DataTypes.Logger.WarningLogged += (sender, args) => LogWarning(sw, args.Str);
                    DataTypes.Logger.InformationLogged += (sender, args) => LogInformation(sw, args.Str);
                    DataTypes.Logger.DebugInformationLogged += (sender, args) => LogDebugInformation(sw, args.Str);
                    DataTypes.Logger.VerboseInformationLogged += (sender, args) => LogDebugInformation(sw, args.Str);
                    goto case FileLoggerConfigElement.LVL_DEBUG;
                }
                case FileLoggerConfigElement.LVL_DEBUG:
                {
                    var sw = File.CreateText($"{path}\\server.{logName}.debug.txt");
                    sw.AutoFlush = true;
                    DataTypes.Logger.ErrorLogged += (sender, args) => LogError(sw, args.Str);
                    DataTypes.Logger.WarningLogged += (sender, args) => LogWarning(sw, args.Str);
                    DataTypes.Logger.InformationLogged += (sender, args) => LogInformation(sw, args.Str);
                    DataTypes.Logger.DebugInformationLogged += (sender, args) => LogDebugInformation(sw, args.Str);
                    goto case FileLoggerConfigElement.LVL_NORMAL;
                }
                case FileLoggerConfigElement.LVL_NORMAL:
                {
                    var sw = File.CreateText($"{path}\\server.{logName}.txt");
                    sw.AutoFlush = true;
                    DataTypes.Logger.ErrorLogged += (sender, args) => LogError(sw, args.Str);
                    DataTypes.Logger.WarningLogged += (sender, args) => LogWarning(sw, args.Str);
                    DataTypes.Logger.InformationLogged += (sender, args) => LogInformation(sw, args.Str);
                    break;
                }
            }
        }

        private static void LogError(TextWriter sw, string message)
        {
            sw.WriteLine($"ERROR: {message}");
        }

        private static void LogWarning(TextWriter sw, string message)
        {
            sw.WriteLine($"WARNING: {message}");
        }

        private static void LogInformation(TextWriter sw, string message)
        {
            sw.WriteLine(message);
        }

        private static void LogDebugInformation(TextWriter sw, string message)
        {
            sw.WriteLine($"DEBUG: {message}");
        }
    }
}
