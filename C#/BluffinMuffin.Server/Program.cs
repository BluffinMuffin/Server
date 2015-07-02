using System;
using System.Collections.Generic;
using BluffinMuffin.Server.Protocol;
using Com.Ericmas001.Util;
using System.IO;
using System.Reflection;

namespace BluffinMuffin.Server
{
    public static class Program
    {
        static StreamWriter m_SwNormal;
        static StreamWriter m_SwDebug;
        static StreamWriter m_SwVerbose;
        static void Main(string[] args)
        {
            LogManager.MessageLogged += LogManager_MessageLogged;
            if ((args.Length % 2) == 0)
            {
                try
                {

                    var map = new Dictionary<string, string>();
                    for (var i = 0; i < args.Length; i += 2)
                        map.Add(args[i].ToLower(), args[i + 1]);
                    var port = 4242;
                    if (map.ContainsKey("-p"))
                        port = int.Parse(map["-p"]);
                    if(map.ContainsKey("-log"))
                    {
                    var uri = new Uri(Assembly.GetExecutingAssembly().CodeBase);
                    var path = Path.GetDirectoryName(uri.LocalPath + uri.Fragment) + "\\log";
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                        var logName = DateTime.Now.ToString("yyyy-MM-dd.HH-mm-ss");
                        var logType = map["-log"];
                        if(logType == "normal" || logType == "debug" || logType == "verbose")
                        {
                            m_SwNormal = File.CreateText(path + "\\server." + logName + ".normal.txt");
                            m_SwNormal.AutoFlush = true;
                            LogManager.MessageLogged += LogManager_MessageLoggedToFileNormal;
                            if (logType == "debug" || logType == "verbose")
                            {
                                m_SwDebug = File.CreateText(path + "\\server." + logName + ".debug.txt");
                                m_SwDebug.AutoFlush = true;
                                LogManager.MessageLogged += LogManager_MessageLoggedToFileDebug;
                                if (logType == "verbose")
                                {
                                    m_SwVerbose = File.CreateText(path + "\\server." + logName + ".verbose.txt");
                                    m_SwVerbose.AutoFlush = true;
                                    LogManager.MessageLogged += LogManager_MessageLoggedToFileVerbose;
                                }
                            }
                        }

                    }
                    var server = new BluffinServer(port);
                    server.Start();
                    LogManager.Log(LogLevel.Message, "BluffinMuffin.Server", "Server started on port {0}", port);
                }
                catch
                {
                    LogManager.Log(LogLevel.Error, "Program.Main", "Can't start server !!");
                }
            }
            else
                LogManager.Log(LogLevel.Error, "Program.Main", "Incorrect number of application arguments");
        }

        static void LogManager_MessageLogged(string from, string message, int level)
        {
            // ATTENTION: This must contain "LogLevel.Message" for RELEASE
            //                              "LogLevel.MessageLow" for DEBUGGING
            //                              "LogLevel.MessageVeryLow" for XTREM DEBUGGING
            LogManager.LogInConsole(from, message, level, LogLevel.MessageVeryLow);
        }

        static void LogManager_MessageLoggedToFileNormal(string from, string message, int level)
        {
            LogManager.LogInFile(m_SwNormal, from, message, level, LogLevel.Message);
        }

        static void LogManager_MessageLoggedToFileDebug(string from, string message, int level)
        {
            LogManager.LogInFile(m_SwDebug, from, message, level, LogLevel.MessageLow);
        }

        static void LogManager_MessageLoggedToFileVerbose(string from, string message, int level)
        {
            LogManager.LogInFile(m_SwVerbose, from, message, level, LogLevel.MessageVeryLow);
        }
    }
}
