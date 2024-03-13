using SabreTools.Core;

namespace SabreTools.Logging
{
    /// <summary>
    /// Severity of the logging statement
    /// </summary>
    public enum LogLevel
    {
        [Mapping("verbose")]
        VERBOSE = 0,

        [Mapping("user")]
        USER,

        [Mapping("warning")]
        WARNING,

        [Mapping("error")]
        ERROR,
    }
}
