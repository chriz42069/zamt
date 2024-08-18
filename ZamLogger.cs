using AOSharp.Core.UI;
using System;
using ZamTools.Models;

namespace ZamTools
{
    /// <summary>
    /// Utility to improve the information and consistency to the chat.
    /// Setting the log style to quiet will mute all log statements except for error regardless of the log level.
    /// </summary>
    public class ZamLogger
    {
        private string instanceName;
        private static LogLevel logLevel = Config.LOG_LEVEL;
        private static LogStyle logStyle = Config.LOG_STYLE;

        private ZamLogger(string _instanceName) { 
            instanceName = _instanceName; 
        }
        
        public static ZamLogger GetLogger(string _instanceName) {
            return new ZamLogger(_instanceName); 
        }

        //Error logs will not check the log level
        public void error(string _message) {
            string prefix = logStyle.Equals(LogStyle.VERBOSE) ? $"{DateTime.Now}-{instanceName}-" : "";
            Chat.WriteLine($"{prefix}[ERROR]: {_message}");
        }
        
        //info logs will only be printed if the logLevel is set to Debug or Info
        public void info(string _message) {
            if ((!logStyle.Equals(LogStyle.QUIET)) && (logLevel.Equals(LogLevel.INFO) || logLevel.Equals(LogLevel.DEBUG))) {
                string prefix = logStyle.Equals(LogStyle.VERBOSE) ? $"{DateTime.Now}-{instanceName}-" : "";
                Chat.WriteLine($"{prefix}[INFO]: {_message}");
            } 
        }

        //Debug logs will print only if the debug level is enabled
        public void debug(string _message) { 
            if ((!logStyle.Equals(LogStyle.QUIET)) && logLevel.Equals(LogLevel.DEBUG)) {
                string prefix = logStyle.Equals(LogStyle.VERBOSE) ? $"{DateTime.Now}-{instanceName}-" : "";
                Chat.WriteLine($"{prefix}[DEBUG]: {_message}");
            } 
        }
    }
    public enum LogStyle
    {
        QUIET,
        SIMPLE,
        VERBOSE,
    }

    public enum LogLevel {
        INFO,
        DEBUG,
        ERROR,
    }
}
