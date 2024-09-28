namespace Logger
{
    public struct LogEntry
    {
        ILogCategory Category { get; set; }
        public LogSeverity Severity { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }

        public LogEntry(ILogCategory category, LogSeverity severity, string message, string stackTrace) : this()
        {
            Category = category;
            Severity = severity;
            Message = message;
            StackTrace = stackTrace;
        }
    }
}