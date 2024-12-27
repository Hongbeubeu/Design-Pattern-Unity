using System;
using UnityEngine;

namespace Logger
{
    public static class Logs
    {
        private static ILogCategory _defaultCategory;
        public static ILogCategory DefaultCategory => _defaultCategory ??= new LogCategory("Debug", "DEBUG", "#FFFFFF", "#000000");

        private static ILogCategory _editorCategory;
        public static ILogCategory EditorCategory => _editorCategory ??= new LogCategory("Editor", "EDITOR", "#FFFFFF", "#000000");

        private static ILogCategory _infoCategory;
        public static ILogCategory InfoCategory => _infoCategory ??= new LogCategory("Info", "INFO", "#00FF00", "#00FF00");

        private static ILogCategory _criticalCategory;
        public static ILogCategory CriticalCategory => _criticalCategory ??= new LogCategory("Critical", "CRITICAL", "#FF0000", "#FF0000");

        private static Logger _logger;
        public static Logger Logger => _logger ??= new Logger();

        [StripFromStackTrace]
        private static void Log<T>(LogSeverity severity, string message, ILogCategory category = null)
        {
            Logger.Log(severity, $"[{typeof(T).Name}] {message}", category ?? (Application.isPlaying ? DefaultCategory : EditorCategory));
        }

        [StripFromStackTrace]
        public static void Log<T>(string message)
        {
            Log<T>(LogSeverity.Log, message, DefaultCategory);
        }

        [StripFromStackTrace]
        public static void LogInfo<T>(string message)
        {
            Log<T>(LogSeverity.Log, message, InfoCategory);
        }

        [StripFromStackTrace]
        public static void LogWarning<T>(string message)
        {
            Log<T>(LogSeverity.Warning, message, DefaultCategory);
        }

        [StripFromStackTrace]
        public static void LogError<T>(string message)
        {
            Log<T>(LogSeverity.Error, message, CriticalCategory);
        }

        [StripFromStackTrace]
        public static void HandleException(Exception exception, ILogCategory category = null)
        {
            Logger.HanldeException(exception, category ?? (Application.isPlaying ? CriticalCategory : EditorCategory));
        }
    }
}