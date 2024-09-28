using System;

namespace Logger
{
    /// <summary>
    /// This will remove any method with this attribute from a custom stack trace.
    /// </summary>
    public class StripFromStackTraceAttribute : Attribute
    {
    }
}