using System;

namespace Logger
{
    [Flags]
    public enum LogSeverity
    {
        None = 0b0000_0000,
        Log = 0b0000_0001,
        Warning = 0b0000_0010,
        Error = 0b0000_0100,
        Exception = 0b0000_1000,
        Fatal = Error | Exception,
        All = Log | Warning | Error | Exception
    }
}