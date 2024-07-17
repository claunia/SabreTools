using System;

namespace SabreTools.Logging
{
    /// <summary>
    /// Per-class logging
    /// </summary>
    public class Logger
    {
        /// <summary>
        /// Instance associated with this logger
        /// </summary>
        /// TODO: Derive class name for this object, if possible
        private readonly object? instance;

        /// <summary>
        /// Constructor
        /// </summary>
        public Logger(object? instance = null)
        {
            this.instance = instance;
        }

        #region Log Event Triggers

        /// <summary>
        /// Write the given exception as a verbose message to the log output
        /// </summary>
        /// <param name="ex">Exception to be written log</param>
        /// <param name="output">String to be written log</param>
        /// <returns>True if the output could be written, false otherwise</returns>
        public void Verbose(Exception ex, string? output = null)
        {
            LoggerImpl.Verbose(this.instance, ex, output);
        }

        /// <summary>
        /// Write the given string as a verbose message to the log output
        /// </summary>
        /// <param name="output">String to be written log</param>
        /// <returns>True if the output could be written, false otherwise</returns>
        public void Verbose(string output)
        {
            LoggerImpl.Verbose(this.instance, output);
        }

        /// <summary>
        /// Write the given verbose progress message to the log output
        /// </summary>
        /// <param name="total">Total count for progress</param>
        /// <param name="current">Current count for progres</param>
        /// <param name="output">String to be written log</param>
        public void Verbose(long total, long current, string? output = null)
        {
            LoggerImpl.Verbose(instance, total, current, output);
        }

        /// <summary>
        /// Write the given exception as a user message to the log output
        /// </summary>
        /// <param name="ex">Exception to be written log</param>
        /// <param name="output">String to be written log</param>
        /// <returns>True if the output could be written, false otherwise</returns>
        public void User(Exception ex, string? output = null)
        {
            LoggerImpl.User(this.instance, ex, output);
        }

        /// <summary>
        /// Write the given string as a user message to the log output
        /// </summary>
        /// <param name="output">String to be written log</param>
        /// <returns>True if the output could be written, false otherwise</returns>
        public void User(string output)
        {
            LoggerImpl.User(this.instance, output);
        }

        /// <summary>
        /// Write the given user progress message to the log output
        /// </summary>
        /// <param name="total">Total count for progress</param>
        /// <param name="current">Current count for progres</param>
        /// <param name="output">String to be written log</param>
        public void User(long total, long current, string? output = null)
        {
            LoggerImpl.User(instance, total, current, output);
        }

        /// <summary>
        /// Write the given exception as a warning to the log output
        /// </summary>
        /// <param name="ex">Exception to be written log</param>
        /// <param name="output">String to be written log</param>
        /// <returns>True if the output could be written, false otherwise</returns>
        public void Warning(Exception ex, string? output = null)
        {
            LoggerImpl.Warning(this.instance, ex, output);
        }

        /// <summary>
        /// Write the given string as a warning to the log output
        /// </summary>
        /// <param name="output">String to be written log</param>
        /// <returns>True if the output could be written, false otherwise</returns>
        public void Warning(string output)
        {
            LoggerImpl.Warning(this.instance, output);
        }

        /// <summary>
        /// Write the given warning progress message to the log output
        /// </summary>
        /// <param name="total">Total count for progress</param>
        /// <param name="current">Current count for progres</param>
        /// <param name="output">String to be written log</param>
        public void Warning(long total, long current, string? output = null)
        {
            LoggerImpl.Warning(instance, total, current, output);
        }

        /// <summary>
        /// Writes the given exception as an error in the log
        /// </summary>
        /// <param name="ex">Exception to be written log</param>
        /// <param name="output">String to be written log</param>
        /// <returns>True if the output could be written, false otherwise</returns>
        public void Error(Exception ex, string? output = null)
        {
            LoggerImpl.Error(this.instance, ex, output);
        }

        /// <summary>
        /// Writes the given string as an error in the log
        /// </summary>
        /// <param name="output">String to be written log</param>
        /// <returns>True if the output could be written, false otherwise</returns>
        public void Error(string output)
        {
            LoggerImpl.Error(this.instance, output);
        }

        /// <summary>
        /// Write the given error progress message to the log output
        /// </summary>
        /// <param name="total">Total count for progress</param>
        /// <param name="current">Current count for progres</param>
        /// <param name="output">String to be written log</param>
        public void Error(long total, long current, string? output = null)
        {
            LoggerImpl.Error(instance, total, current, output);
        }

        #endregion
    }
}
