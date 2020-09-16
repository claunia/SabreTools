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
            if (addDate)
                Filename = $"{Path.GetFileNameWithoutExtension(filename)} ({DateTime.Now:yyyy-MM-dd HH-mm-ss}).{PathExtensions.GetNormalizedExtension(filename)}";
            else
                Filename = filename;

            if (!string.IsNullOrEmpty(LogDirectory) && !Directory.Exists(LogDirectory))
                Directory.CreateDirectory(LogDirectory);

            Start();
        }

        /// <summary>
        /// Start logging by opening output file (if necessary)
        /// </summary>
        /// <returns>True if the logging was started correctly, false otherwise</returns>
        public bool Start()
        {
            StartTime = DateTime.Now;
            if (!LogToFile)
                return true;

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

        /// <summary>
        /// Write the given string to the log output
        /// </summary>
        /// <param name="output">String to be written log</param>
        /// <param name="loglevel">Severity of the information being logged</param>
        /// <param name="appendPrefix">True if the level and datetime should be prepended to each statement, false otherwise</param>
        /// <returns>True if the output could be written, false otherwise</returns>
        private bool Log(string output, LogLevel loglevel, bool appendPrefix)
        {
            // If the log level is less than the filter level, we skip it but claim we didn't
            if (loglevel < LowestLogLevel)
                return true;

            // USER and ERROR writes to console
            if (loglevel == LogLevel.USER || loglevel == LogLevel.ERROR)
                Console.WriteLine((loglevel == LogLevel.ERROR && appendPrefix ? loglevel.ToString() + " " : string.Empty) + output);

            // If we're writing to file, use the existing stream
            if (LogToFile)
            {
                try
                {
                    lock(_lock)
                    {
                        _log.WriteLine((appendPrefix ? $"{loglevel} - {DateTime.Now} - " : string.Empty) + output);
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

        /// <summary>
        /// Write the given exact string to the log output
        /// </summary>
        /// <param name="output">String to be written log</param>
        /// <param name="line">Line number to write out to</param>
        /// <param name="column">Column number to write out to</param>
        /// <returns>True if the output could be written, false otherwise</returns>
        public bool WriteExact(string output, int line, int column)
        {
            // Set the cursor position (if not being redirected)
            if (!Console.IsOutputRedirected)
            {
                Console.CursorTop = line;
                Console.CursorLeft = column;
            }

            // Write out to the console
            Console.Write(output);

            // If we're writing to file, use the existing stream
            if (LogToFile)
            {
                try
                {
                    lock (_lock)
                    {
                        _log.Write($"{DateTime.Now} - {output}");
                    }
                }
                catch
                {
                    Console.WriteLine("Could not write to log file!");
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Write the given exception as a verbose message to the log output
        /// </summary>
        /// <param name="ex">Exception to be written log</param>
        /// <param name="output">String to be written log</param>
        /// <param name="appendPrefix">True if the level and datetime should be prepended to each statement (default), false otherwise</param>
        /// <returns>True if the output could be written, false otherwise</returns>
        public bool Verbose(Exception ex, string output = null, bool appendPrefix = true)
        {
            if (ThrowOnError) throw ex;
            return Verbose($"{(output != null ? output + ": " : string.Empty)}{ex}", appendPrefix);
        }

        /// <summary>
        /// Write the given string as a verbose message to the log output
        /// </summary>
        /// <param name="output">String to be written log</param>
        /// <param name="appendPrefix">True if the level and datetime should be prepended to each statement (default), false otherwise</param>
        /// <returns>True if the output could be written, false otherwise</returns>
        public bool Verbose(string output, bool appendPrefix = true)
        {
            return Log(output, LogLevel.VERBOSE, appendPrefix);
        }

        /// <summary>
        /// Write the given exception as a user message to the log output
        /// </summary>
        /// <param name="ex">Exception to be written log</param>
        /// <param name="output">String to be written log</param>
        /// <param name="appendPrefix">True if the level and datetime should be prepended to each statement (default), false otherwise</param>
        /// <returns>True if the output could be written, false otherwise</returns>
        public bool User(Exception ex, string output = null, bool appendPrefix = true)
        {
            if (ThrowOnError) throw ex;
            return User($"{(output != null ? output + ": " : string.Empty)}{ex}", appendPrefix);
        }

        /// <summary>
        /// Write the given string as a user message to the log output
        /// </summary>
        /// <param name="output">String to be written log</param>
        /// <param name="appendPrefix">True if the level and datetime should be prepended to each statement (default), false otherwise</param>
        /// <returns>True if the output could be written, false otherwise</returns>
        public bool User(string output, bool appendPrefix = true)
        {
            return Log(output, LogLevel.USER, appendPrefix);
        }

        /// <summary>
        /// Write the given exception as a warning to the log output
        /// </summary>
        /// <param name="ex">Exception to be written log</param>
        /// <param name="output">String to be written log</param>
        /// <param name="appendPrefix">True if the level and datetime should be prepended to each statement (default), false otherwise</param>
        /// <returns>True if the output could be written, false otherwise</returns>
        public bool Warning(Exception ex, string output = null, bool appendPrefix = true)
        {
            if (ThrowOnError) throw ex;
            return Warning($"{(output != null ? output + ": " : string.Empty)}{ex}", appendPrefix);
        }

        /// <summary>
        /// Write the given string as a warning to the log output
        /// </summary>
        /// <param name="output">String to be written log</param>
        /// <param name="appendPrefix">True if the level and datetime should be prepended to each statement (default), false otherwise</param>
        /// <returns>True if the output could be written, false otherwise</returns>
        public bool Warning(string output, bool appendPrefix = true)
        {
            LoggedWarnings = true;
            return Log(output, LogLevel.WARNING, appendPrefix);
        }

        /// <summary>
        /// Writes the given exception as an error in the log
        /// </summary>
        /// <param name="ex">Exception to be written log</param>
        /// <param name="output">String to be written log</param>
        /// <param name="appendPrefix">True if the level and datetime should be prepended to each statement (default), false otherwise</param>
        /// <returns>True if the output could be written, false otherwise</returns>
        public bool Error(Exception ex, string output = null, bool appendPrefix = true)
        {
            if (ThrowOnError) throw ex;
            return Error($"{(output != null ? output + ": " : string.Empty)}{ex}", appendPrefix);
        }

        /// <summary>
        /// Writes the given string as an error in the log
        /// </summary>
        /// <param name="output">String to be written log</param>
        /// <param name="appendPrefix">True if the level and datetime should be prepended to each statement (default), false otherwise</param>
        /// <returns>True if the output could be written, false otherwise</returns>
        public bool Error(string output, bool appendPrefix = true)
        {
            LoggedErrors = true;
            return Log(output, LogLevel.ERROR, appendPrefix);
        }

        /// <summary>
        /// Clear lines beneath the given line in the console
        /// </summary>
        /// <param name="line">Line number to clear beneath</param>
        /// <returns>True</returns>
        public bool ClearBeneath(int line)
        {
            if (!Console.IsOutputRedirected)
            {
                for (int i = line; i < Console.WindowHeight; i++)
                {
                    // http://stackoverflow.com/questions/8946808/can-console-clear-be-used-to-only-clear-a-line-instead-of-whole-console
                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.Write(new string(' ', Console.WindowWidth));
                    Console.SetCursorPosition(0, i);
                }
            }
            return true;
        }
    }
}
