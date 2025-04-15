using System.Collections.Generic;
using SabreTools.DatFiles;
using SabreTools.IO.Logging;

namespace SabreTools.Reports
{
    /// <summary>
    /// Base class for a report output format
    /// </summary>
    public abstract class BaseReport
    {
        #region Logging

        /// <summary>
        /// Logging object
        /// </summary>
        protected readonly Logger _logger = new();

        #endregion

        /// <summary>
        /// Set of DatStatistics objects to use for formatting
        /// </summary>
        protected List<DatStatistics> _statistics;

        /// <summary>
        /// Create a new report from the filename
        /// </summary>
        /// <param name="statsList">List of statistics objects to set</param>
        public BaseReport(List<DatStatistics> statsList)
        {
            _statistics = statsList;
        }

        /// <summary>
        /// Create and open an output file for writing direct from a set of statistics
        /// </summary>
        /// <param name="outfile">Name of the file to write to</param>
        /// <param name="baddumpCol">True if baddumps should be included in output, false otherwise</param>
        /// <param name="nodumpCol">True if nodumps should be included in output, false otherwise</param>
        /// <param name="throwOnError">True if the error that is thrown should be thrown back to the caller, false otherwise</param>
        /// <returns>True if the report was written correctly, false otherwise</returns>
        public abstract bool WriteToFile(string? outfile, bool baddumpCol, bool nodumpCol, bool throwOnError = false);
    }
}
