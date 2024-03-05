using System;
#if NET452_OR_GREATER || NETCOREAPP
using System.Threading.Tasks;
#endif

namespace SabreTools.Core
{
    /// <summary>
    /// Globally-accessible objects for the library
    /// </summary>
    public class Globals
    {
        #region Public accessors

        /// <summary>
        /// Maximum threads to use during parallel operations
        /// </summary>
        public static int MaxThreads { get; set; } = Environment.ProcessorCount;

#if NET452_OR_GREATER || NETCOREAPP
        /// <summary>
        /// ParallelOptions object for use in parallel operations
        /// </summary>
        public static ParallelOptions ParallelOptions => new()
        {
            MaxDegreeOfParallelism = MaxThreads
        };
#endif

        #endregion
    }
}
