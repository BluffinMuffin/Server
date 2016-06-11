using System;
using System.Diagnostics.CodeAnalysis;
using BluffinMuffin.Server.Configuration;
using Com.Ericmas001.Common;

namespace BluffinMuffin.Server.Logging
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class ConsoleLogger
    {
        private static void WriteLine(string message, ConsoleColor color)
        {
            var bkpForeground = Console.ForegroundColor;
            Console.ForegroundColor = color;

            Console.WriteLine(message);

            Console.ForegroundColor = bkpForeground;
        }
        public static void Init(BluffinMuffinDataSection config)
        {
            DataTypes.Logger.ErrorLogged += (sender, args) => LogError(args.Info);
            DataTypes.Logger.WarningLogged += (sender, args) => LogWarning(args.Info);

            switch (config.Logging.Console.Level)
            {
                case ConsoleLoggerConfigElement.LVL_VERBOSE:
                    DataTypes.Logger.VerboseInformationLogged += (sender, args) => LogVerboseInformation(args.Info);
                    goto case ConsoleLoggerConfigElement.LVL_DEBUG;
                case ConsoleLoggerConfigElement.LVL_DEBUG:
                    DataTypes.Logger.DebugInformationLogged += (sender, args) => LogDebugInformation(args.Info);
                    goto case ConsoleLoggerConfigElement.LVL_NORMAL;
                case ConsoleLoggerConfigElement.LVL_NORMAL:
                    DataTypes.Logger.InformationLogged += (sender, args) => LogInformation(args.Info);
                    break;
            }
        }

        private static void LogError(string message)
        {
            WriteLine($"ERROR: {message}", ConsoleColor.Red);
        }

        private static void LogWarning(string message)
        {
            WriteLine($"WARNING: {message}", ConsoleColor.Yellow);
        }

        private static void LogInformation(string message)
        {
            WriteLine(message, ConsoleColor.White);
        }

        private static void LogDebugInformation(string message)
        {
            WriteLine($"DEBUG: {message}", ConsoleColor.Cyan);
        }

        private static void LogVerboseInformation(string message)
        {
            WriteLine($"DEBUG: {message}", ConsoleColor.DarkCyan);
        }
    }
}
