using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

using SabreTools.Library.Logging;

namespace SabreTools.Library.Data
{
    /// <summary>
    /// Globally-accessible objects for the library
    /// </summary>
    public class Globals
    {
        #region Private implementations

        private static Logger _logger = null;

        #endregion

        #region Public accessors

        /// <summary>
        /// Command line arguments passed in to the parent program
        /// </summary>
        public static string CommandLineArgs => string.Join(" ", Environment.GetCommandLineArgs());

        /// <summary>
        /// Directory path for the current executable
        /// </summary>
        public static string ExeDir => Path.GetDirectoryName(ExeName);

        /// <summary>
        /// File path for the current executable
        /// </summary>
        public static string ExeName => new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase).LocalPath;

        /// <summary>
        /// Logging object for writing to file and console
        /// </summary>
        public static Logger Logger
        {
            get
            {
                if (_logger == null)
                    _logger = new Logger();

                return _logger;
            }
            set { _logger = value; }
        }

        /// <summary>
        /// Maximum threads to use during parallel operations
        /// </summary>
        public static int MaxThreads { get; set; } = Environment.ProcessorCount;

        /// <summary>
        /// ParallelOptions object for use in parallel operations
        /// </summary>
        public static ParallelOptions ParallelOptions => new ParallelOptions()
        {
            MaxDegreeOfParallelism = MaxThreads
        };

        /// <summary>
        /// Temporary directory location
        /// </summary>
        public static string TempDir { get; set; } = Path.GetTempPath();

        /// <summary>
        /// Whether to throw an exception from the library if an error is found
        /// </summary>
        public static bool ThrowOnError { get; set; } = false;

        #endregion
    }
}
