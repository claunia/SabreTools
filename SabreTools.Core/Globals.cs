using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace SabreTools.Core
{
    /// <summary>
    /// Globally-accessible objects for the library
    /// </summary>
    public class Globals
    {
        #region Public accessors

        /// <summary>
        /// Directory path for the current executable
        /// </summary>
        public static string ExeDir => Path.GetDirectoryName(ExeName);

        /// <summary>
        /// File path for the current executable
        /// </summary>
        public static string ExeName => new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase).LocalPath;

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
        /// <remarks>TODO: Find a way to get rid of this as a global variable and put it in DatFile</remarks>
        public static string TempDir { get; set; } = Path.GetTempPath();

        #endregion
    }
}
