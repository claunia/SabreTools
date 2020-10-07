using System;
using System.IO;
using System.Text;

using SabreTools.Library.Data;
using SabreTools.Library.IO;

namespace SabreTools.Library.Logging
{
    /// <summary>
    /// Log either to file or to the console
    /// </summary>
    public class Logger
    {
        #region Fields

        /// <summary>
        /// Optional output filename for logs
        /// </summary>
        public string Filename { get; set; } = null;

        /// <summary>
        /// Determines if we're logging to file or not
        /// </summary>
        public bool LogToFile { get { return !string.IsNullOrWhiteSpace(Filename); } }

        /// <summary>
        /// Optional output log directory
        /// </summary>
        /// TODO: Make this either passed in or optional
        public string LogDirectory { get; set; } = Path.Combine(Globals.ExeDir, "logs") + Path.DirectorySeparatorChar;

        /// <summary>
        /// Determines the lowest log level to output
        /// </summary>
        public LogLevel LowestLogLevel { get; set; } = LogLevel.VERBOSE;

        /// <summary>
        /// Determines whether to prefix log lines with level and datetime
        /// </summary>
        public bool AppendPrefix { get; set; } = true;

        /// <summary>
        /// Determines whether to throw if an exception is logged
        /// </summary>
        public bool ThrowOnError { get; set; } = false;

        /// <summary>
        /// Logging start time for metrics
        /// </summary>
        public DateTime StartTime { get; private set; }

        /// <summary>
        /// Determines if there were errors logged
        /// </summary>
        public bool LoggedErrors { get; private set; } = false;

        /// <summary>
        /// Determines if there were warnings logged
        /// </summary>
        public bool LoggedWarnings { get; private set; } = false;

        #endregion

        #region Private instance variables

        /// <summary>
        /// StreamWriter representing the output log file
        /// </summary>
        private StreamWriter _log;

        /// <summary>
        /// Object lock for multithreaded logging
        /// </summary>
        private readonly object _lock = new object();

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize a console-only logger object
        /// </summary>
        public Logger()
        {
            Start();
        }

        /// <summary>
        /// Initialize a Logger object with the given information
        /// </summary>
        /// <param name="filename">Filename representing log location</param>
        /// <param name="addDate">True to add a date string to the filename (default), false otherwise</param>
        public Logger(string filename, bool addDate = true)
        {
            // Set and create the output
            if (addDate)
                Filename = $"{Path.GetFileNameWithoutExtension(filename)} ({DateTime.Now:yyyy-MM-dd HH-mm-ss}).{PathExtensions.GetNormalizedExtension(filename)}";
            else
                Filename = filename;

            if (!string.IsNullOrEmpty(LogDirectory) && !Directory.Exists(LogDirectory))
                Directory.CreateDirectory(LogDirectory);

            Start();
        }

        #endregion

        #region Control

        /// <summary>
        /// Start logging by opening output file (if necessary)
        /// </summary>
        /// <returns>True if the logging was started correctly, false otherwise</returns>
        public bool Start()
        {
            // Setup the logging handler to always use the internal log
            LogEventHandler += HandleLogEvent;

            // Start the logging
            StartTime = DateTime.Now;
            if (!LogToFile)
                return true;

            // Setup file output and perform initial log
            try
            {
                FileStream logfile = FileExtensions.TryCreate(Path.Combine(LogDirectory, Filename));
                _log = new StreamWriter(logfile, Encoding.UTF8, (int)(4 * Constants.KibiByte), true)
                {
                    AutoFlush = true
                };

                _log.WriteLine($"Logging started {StartTime:yyyy-MM-dd HH:mm:ss}");
                _log.WriteLine($"Command run: {Globals.CommandLineArgs}");
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// End logging by closing output file (if necessary)
        /// </summary>
        /// <param name="suppress">True if all ending output is to be suppressed, false otherwise (default)</param>
        /// <returns>True if the logging was ended correctly, false otherwise</returns>
        public bool Close(bool suppress = false)
        {
            if (!suppress)
            {
                if (LoggedWarnings)
                    Console.WriteLine("There were warnings in the last run! Check the log for more details");

                if (LoggedErrors)
                    Console.WriteLine("There were errors in the last run! Check the log for more details");

                TimeSpan span = DateTime.Now.Subtract(StartTime);

                // Special case for multi-day runs
                string total;
                if (span >= TimeSpan.FromDays(1))
                    total = span.ToString(@"d\:hh\:mm\:ss");
                else
                    total = span.ToString(@"hh\:mm\:ss");

                if (!LogToFile)
                {
                    Console.WriteLine($"Total runtime: {total}");
                    return true;
                }

                try
                {
                    _log.WriteLine($"Logging ended {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                    _log.WriteLine($"Total runtime: {total}");
                    Console.WriteLine($"Total runtime: {total}");
                    _log.Close();
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                try
                {
                    _log.Close();
                }
                catch
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region Event Handling

        /// <summary>
        /// Handler for log events
        /// </summary>
        public static event LogEventHandler LogEventHandler = delegate { };

        /// <summary>
        /// Default log event handling
        /// </summary>
        public void HandleLogEvent(object sender, LogEventArgs args)
        {
            // Null args means we can't handle it
            if (args == null)
                return;

            // If we have an exception and we're throwing on that
            if (ThrowOnError && args.Exception != null)
                throw args.Exception;

            // If we have a warning or error, set the flags accordingly
            if (args.LogLevel == LogLevel.WARNING)
                LoggedWarnings = true;
            if (args.LogLevel == LogLevel.ERROR)
                LoggedErrors = true;

            // Setup the statement based on the inputs
            string logLine;
            if (args.Exception == null)
            {
                logLine = args.Statement ?? string.Empty;
            }
            else if (args.TotalCount != null && args.CurrentCount != null)
            {
                double percentage = (args.CurrentCount.Value / args.TotalCount.Value) * 100;
                logLine = $"{percentage:N2}%{(args.Statement != null ? ": " + args.Statement : string.Empty)}";
            }
            else
            {
                logLine = $"{(args.Statement != null ? args.Statement + ": " : string.Empty)}{args.Exception}";
            }

            // Then write to the log
            Log(logLine, args.LogLevel);
        }

        /// <summary>
        /// Write the given string to the log output
        /// </summary>
        /// <param name="output">String to be written log</param>
        /// <param name="loglevel">Severity of the information being logged</param>
        /// <returns>True if the output could be written, false otherwise</returns>
        private bool Log(string output, LogLevel loglevel)
        {
            // If the log level is less than the filter level, we skip it but claim we didn't
            if (loglevel < LowestLogLevel)
                return true;

            // USER and ERROR writes to console
            if (loglevel == LogLevel.USER || loglevel == LogLevel.ERROR)
                Console.WriteLine((loglevel == LogLevel.ERROR && this.AppendPrefix ? loglevel.ToString() + " " : string.Empty) + output);

            // If we're writing to file, use the existing stream
            if (LogToFile)
            {
                try
                {
                    lock (_lock)
                    {
                        _log.WriteLine((this.AppendPrefix ? $"{loglevel} - {DateTime.Now} - " : string.Empty) + output);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    Console.WriteLine("Could not write to log file!");
                    if (ThrowOnError) throw ex;
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region Log Event Triggers

        /// <summary>
        /// Write the given exception as a verbose message to the log output
        /// </summary>
        /// <param name="ex">Exception to be written log</param>
        /// <param name="output">String to be written log</param>
        /// <returns>True if the output could be written, false otherwise</returns>
        public void Verbose(Exception ex, string output = null)
        {
            LogEventHandler(this, new LogEventArgs(LogLevel.VERBOSE, output, ex));
        }

        /// <summary>
        /// Write the given string as a verbose message to the log output
        /// </summary>
        /// <param name="output">String to be written log</param>
        /// <returns>True if the output could be written, false otherwise</returns>
        public void Verbose(string output)
        {
            LogEventHandler(this, new LogEventArgs(LogLevel.VERBOSE, output, null));
        }

        /// <summary>
        /// Write the given exception as a user message to the log output
        /// </summary>
        /// <param name="ex">Exception to be written log</param>
        /// <param name="output">String to be written log</param>
        /// <returns>True if the output could be written, false otherwise</returns>
        public void User(Exception ex, string output = null)
        {
            LogEventHandler(this, new LogEventArgs(LogLevel.USER, output, ex));
        }

        /// <summary>
        /// Write the given string as a user message to the log output
        /// </summary>
        /// <param name="output">String to be written log</param>
        /// <returns>True if the output could be written, false otherwise</returns>
        public void User(string output)
        {
            LogEventHandler(this, new LogEventArgs(LogLevel.USER, output, null));
        }

        /// <summary>
        /// Write the given exception as a warning to the log output
        /// </summary>
        /// <param name="ex">Exception to be written log</param>
        /// <param name="output">String to be written log</param>
        /// <returns>True if the output could be written, false otherwise</returns>
        public void Warning(Exception ex, string output = null)
        {
            LogEventHandler(this, new LogEventArgs(LogLevel.WARNING, output, ex));
        }

        /// <summary>
        /// Write the given string as a warning to the log output
        /// </summary>
        /// <param name="output">String to be written log</param>
        /// <returns>True if the output could be written, false otherwise</returns>
        public void Warning(string output)
        {
            LogEventHandler(this, new LogEventArgs(LogLevel.WARNING, output, null));
        }

        /// <summary>
        /// Writes the given exception as an error in the log
        /// </summary>
        /// <param name="ex">Exception to be written log</param>
        /// <param name="output">String to be written log</param>
        /// <returns>True if the output could be written, false otherwise</returns>
        public void Error(Exception ex, string output = null)
        {
            LogEventHandler(this, new LogEventArgs(LogLevel.ERROR, output, ex));
        }

        /// <summary>
        /// Writes the given string as an error in the log
        /// </summary>
        /// <param name="output">String to be written log</param>
        /// <returns>True if the output could be written, false otherwise</returns>
        public void Error(string output)
        {
            LogEventHandler(this, new LogEventArgs(LogLevel.ERROR, output, null));
        }

        #endregion
    }
}
