using System;
using System.Reflection;
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
        /// <summary>
        /// The current toolset version to be used by all child applications
        /// </summary>
        public readonly static string? Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString();

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
    }
}
