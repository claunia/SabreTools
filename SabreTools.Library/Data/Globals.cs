using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

using SabreTools.Library.Tools;

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

        public static int MaxThreads { get; set; } = Environment.ProcessorCount;

        public static ParallelOptions ParallelOptions => new ParallelOptions()
        {
            MaxDegreeOfParallelism = MaxThreads
        };

        public static string ExeName => new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase).LocalPath;

        public static string ExeDir => Path.GetDirectoryName(ExeName);

        public static string CommandLineArgs => string.Join(" ", Environment.GetCommandLineArgs());

        #endregion
    }
}
