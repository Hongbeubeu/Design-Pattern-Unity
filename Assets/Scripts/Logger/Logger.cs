using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Logger
{
    public sealed class Logger
    {
        private readonly StringBuilder _stringBuilder = new();

        private readonly HashSet<string> _disabledCategories = new();
        private LogEntry _currentEntry;
        private readonly List<LogEntry> _history = new();

        private event Action<Exception> OnExceptionHandled;
        private event Action<LogEntry> OnLogReceived;

        private bool _showTags = true;
        private bool _colorTags = true;
        private bool _useCustomStackTrace;
        private bool _throwExceptions = true;

        private readonly StackTraceLogType _defaultStackTraceLogType = Application.GetStackTraceLogType(LogType.Log);
        private readonly StackTraceLogType _defaultWarningStackTraceLogType = Application.GetStackTraceLogType(LogType.Warning);
        private readonly StackTraceLogType _defaultErrorStackTraceLogType = Application.GetStackTraceLogType(LogType.Error);

        public void Log(LogSeverity severity, string message, ILogCategory category)
        {
            if (IsLogDisabled(category, severity))
            {
                return;
            }

            _currentEntry = createEntry(category, severity, message);
            AddToHistory(_currentEntry);
            OutputLog(_currentEntry);
            OnLogReceived?.Invoke(_currentEntry);
        }

        public void ClearLogs(Func<LogEntry, bool> clearPredicate = null)
        {
            if (clearPredicate == null)
            {
                _history.Clear();
            }
            else
            {
                _history.RemoveAll(new Predicate<LogEntry>(clearPredicate));
            }
        }

        public List<LogEntry> GetLogs(LogSeverity severityMask, Func<LogEntry, bool> selectionPredicate = null)
        {
            var logEntries = new List<LogEntry>();
            foreach (var entry in _history)
            {
                if ((severityMask & entry.Severity) != 0 && (selectionPredicate == null || selectionPredicate(entry)))
                {
                    logEntries.Add(entry);
                }
            }

            return logEntries;
        }

        public void ToggleCategory(ILogCategory category, bool isActive) => ToggleCategoryId(category.Id, isActive);
        public void ToggleCustomStackTrace(bool useCustomStackTrace) => _useCustomStackTrace = useCustomStackTrace;
        public void ToggleTagsInLogs(bool showTags) => _showTags = showTags;
        public void ToggleTagColors(bool colorTags) => _colorTags = colorTags;
        public void ToggleThrowExceptions(bool throwExceptions) => _throwExceptions = throwExceptions;

        public void HanldeException(Exception exception, ILogCategory category)
        {
            OnExceptionHandled?.Invoke(exception);

            Log(_throwExceptions ? LogSeverity.Exception : LogSeverity.Error, exception.ToString(), category);

            if (_throwExceptions)
            {
                throw exception;
            }
        }

        private void ToggleCategoryId(string categoryId, bool isActive)
        {
            if (isActive)
            {
                _disabledCategories.Remove(categoryId);
            }
            else
            {
                _disabledCategories.Add(categoryId);
            }
        }

        private void OutputLog(LogEntry entry)
        {
            if (_useCustomStackTrace)
            {
                DisableDefaultStackTrace();
                LogWithStackTrace(entry.Severity, entry.Message, entry.StackTrace);
                ReenableDefaultStackTrace();
            }
            else
            {
                Application.logMessageReceived += HandleLogMessageReceived;
                LogWithSeverity(entry.Severity, entry.Message);
                Application.logMessageReceived -= HandleLogMessageReceived;
            }
        }

        private void HandleLogMessageReceived(string condition, string stacktrace, LogType type)
        {
            _currentEntry.StackTrace = stacktrace;
        }

        private void DisableDefaultStackTrace()
        {
            Application.SetStackTraceLogType(LogType.Error, StackTraceLogType.None);
            Application.SetStackTraceLogType(LogType.Warning, StackTraceLogType.None);
            Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
        }

        private void ReenableDefaultStackTrace()
        {
            Application.SetStackTraceLogType(LogType.Error, _defaultErrorStackTraceLogType);
            Application.SetStackTraceLogType(LogType.Warning, _defaultWarningStackTraceLogType);
            Application.SetStackTraceLogType(LogType.Log, _defaultStackTraceLogType);
        }


        private void LogWithStackTrace(LogSeverity severity, string message, string stackTrace)
        {
            message = string.Concat(message, "\n", stackTrace);
            LogWithSeverity(severity, message);
        }

        private void LogWithSeverity(LogSeverity severity, string message)
        {
            if ((severity & LogSeverity.Fatal) != 0)
            {
                LogError(message);
            }
            else if (severity.HasFlag(LogSeverity.Warning))
            {
                LogWarning(message);
            }
            else if (severity.HasFlag(LogSeverity.Log))
            {
                LogInfo(message);
            }
        }

        private void LogError(string message)
        {
            Debug.LogError(message);
        }

        private void LogWarning(string message)
        {
            Debug.LogWarning(message);
        }

        private void LogInfo(string message)
        {
            Debug.Log(message);
        }

        private void AddToHistory(LogEntry currentEntry)
        {
            _history.Add(currentEntry);
        }

        private LogEntry createEntry(ILogCategory category, LogSeverity severity, string message)
        {
            return new LogEntry(
                category,
                severity,
                BuildMessage(category, message),
                _useCustomStackTrace ? BuildCustomStackTrace() : ""
            );
        }

        private string BuildCustomStackTrace()
        {
            _stringBuilder.Clear();

            if (!_useCustomStackTrace) return _stringBuilder.ToString();
            var stackTrace = new StackTrace(skipFrames: 3, fNeedFileInfo: true);
            var stackFrames = stackTrace.GetFrames();

            if (stackFrames == null) return _stringBuilder.ToString();
            foreach (var stackFrame in stackFrames)
            {
                var method = stackFrame.GetMethod();

                if (method.GetCustomAttribute<StripFromStackTraceAttribute>() != null)
                {
                    continue;
                }

                var methodNamespace = method.DeclaringType?.Namespace;
                if (!string.IsNullOrEmpty(methodNamespace))
                {
#if UNITY_EDITOR
                    if (methodNamespace.StartsWith("NUnit")
                     || methodNamespace.StartsWith("UnityEngine.TestTools")
                     || methodNamespace.StartsWith("UnityEditor.TestRunner")
                     || methodNamespace.StartsWith("UnityEditor.TestTools")
                     || (methodNamespace.StartsWith("System.Reflection") && method.Name.EndsWith("Invoke"))
                     || (methodNamespace == "System" && method.Name.Contains("Invoke"))
                    )
                        continue;
#endif
                    _stringBuilder.Append(methodNamespace);
                    _stringBuilder.Append(".");
                }


                _stringBuilder.Append(method.DeclaringType?.Name);
                _stringBuilder.Append(":");
                _stringBuilder.Append(method.Name);

                _stringBuilder.Append(" (");
                var parameters = method.GetParameters();
                var isFirstElement = true;
                foreach (var parameterInfo in parameters)
                {
                    if (isFirstElement)
                    {
                        isFirstElement = false;
                    }
                    else
                    {
                        _stringBuilder.Append(", ");
                    }

                    _stringBuilder.Append(parameterInfo.ParameterType.Name);
                }

                _stringBuilder.Append(")");

                //File location
                var fileName = stackFrame.GetFileName();
                if (!string.IsNullOrEmpty(fileName))
                {
                    _stringBuilder.Append(" (at ");
                    _stringBuilder.Append(fileName.Replace("\\", "/"));
                    _stringBuilder.Append(":");
                    _stringBuilder.Append(stackFrame.GetFileLineNumber());
                    _stringBuilder.Append(")");
                }
            }

            return _stringBuilder.ToString();
        }

        private string BuildMessage(ILogCategory category, string message)
        {
            _stringBuilder.Clear();
            if (_showTags)
            {
                if (_colorTags)
                {
                    _stringBuilder.Append($"<color=");
#if UNITY_EDITOR
                    _stringBuilder.Append(EditorGUIUtility.isProSkin ? category.HexColorDarkTheme : category.HexColorLightTheme);
#else
                    _stringBuilder.Append(category.HexColorDarkTheme);
#endif
                    _stringBuilder.Append(">");
                }

                _stringBuilder.Append("[");
                _stringBuilder.Append(category.Tag);
                _stringBuilder.Append("] ");

                if (_colorTags)
                {
                    _stringBuilder.Append("</color>");
                }
                else
                {
                    _stringBuilder.Append(" ");
                }
            }

            _stringBuilder.Append(message);
            return _stringBuilder.ToString();
        }

        private bool IsLogDisabled(ILogCategory category, LogSeverity severity)
        {
            if (severity == LogSeverity.None)
            {
                return true;
            }

            if (_disabledCategories.Contains(category.Id))
            {
                return true;
            }

            return false;
        }
    }
}