using System;
using System.Collections.Generic;
using System.IO;
using SabreTools.DatFiles;
using SabreTools.DatItems;
using SabreTools.Hashing;
using SabreTools.IO.Logging;

namespace SabreTools.Reports.Formats
{
    /// <summary>
    /// Textfile report format
    /// </summary>
    public class Textfile : BaseReport
    {
        protected bool _writeToConsole = false;

        /// <summary>
        /// Create a new report from the filename
        /// </summary>
        /// <param name="statsList">List of statistics objects to set</param>
        public Textfile(List<DatStatistics> statsList)
            : base(statsList)
        {
        }

        /// <inheritdoc/>
        public override bool WriteToFile(string? outfile, bool baddumpCol, bool nodumpCol, bool throwOnError = false)
        {
            InternalStopwatch watch = new($"Writing statistics to '{outfile}");

            try
            {
                // Try to create the output file
                Stream fs = _writeToConsole ? Console.OpenStandardOutput() : File.Create(outfile ?? string.Empty);
                if (fs == null)
                {
                    _logger.Warning($"File '{outfile}' could not be created for writing! Please check to see if the file is writable");
                    return false;
                }

                StreamWriter sw = new(fs);

                // Now process each of the statistics
                for (int i = 0; i < Statistics.Count; i++)
                {
                    // Get the current statistic
                    DatStatistics stat = Statistics[i];

                    // If we have a directory statistic
                    if (stat.IsDirectory)
                    {
                        WriteIndividual(sw, stat, baddumpCol, nodumpCol);

                        // If we have anything but the last value, write the separator
                        if (i < Statistics.Count - 1)
                            WriteFooterSeparator(sw);
                    }

                    // If we have a normal statistic
                    else
                    {
                        WriteIndividual(sw, stat, baddumpCol, nodumpCol);
                    }
                }

                sw.Dispose();
                fs.Dispose();
            }
            catch (Exception ex) when (!throwOnError)
            {
                _logger.Error(ex);
                return false;
            }
            finally
            {
                watch.Stop();
            }

            return true;
        }

        /// <summary>
        /// Write a single set of statistics
        /// </summary>
        /// <param name="sw">StreamWriter to write to</param>
        /// <param name="stat">DatStatistics object to write out</param>
        /// <param name="baddumpCol">True if baddumps should be included in output, false otherwise</param>
        /// <param name="nodumpCol">True if nodumps should be included in output, false otherwise</param>
        private static void WriteIndividual(StreamWriter sw, DatStatistics stat, bool baddumpCol, bool nodumpCol)
        {
            string line = @"'" + stat.DisplayName + @"':
--------------------------------------------------
    Uncompressed size:       " + GetBytesReadable(stat!.TotalSize) + @"
    Games found:             " + stat.MachineCount + @"
    Roms found:              " + stat.GetItemCount(ItemType.Rom) + @"
    Disks found:             " + stat.GetItemCount(ItemType.Disk) + @"
    Roms with CRC:           " + stat.GetHashCount(HashType.CRC32) + @"
    Roms with MD5:           " + stat.GetHashCount(HashType.MD5) + @"
    Roms with SHA-1:         " + stat.GetHashCount(HashType.SHA1) + @"
    Roms with SHA-256:       " + stat.GetHashCount(HashType.SHA256) + @"
    Roms with SHA-384:       " + stat.GetHashCount(HashType.SHA384) + @"
    Roms with SHA-512:       " + stat.GetHashCount(HashType.SHA512) + "\n";

            if (baddumpCol)
                line += "	Roms with BadDump status: " + stat.GetStatusCount(ItemStatus.BadDump) + "\n";

            if (nodumpCol)
                line += "	Roms with Nodump status: " + stat.GetStatusCount(ItemStatus.Nodump) + "\n";

            // For spacing between DATs
            line += "\n\n";

            sw.Write(line);
            sw.Flush();
        }

        /// <summary>
        /// Write out the footer-separator to the stream, if any exists
        /// </summary>
        /// <param name="sw">StreamWriter to write to</param>
        private static void WriteFooterSeparator(StreamWriter sw)
        {
            sw.Write("\n");
            sw.Flush();
        }
    }

    /// <summary>
    /// Console report format
    /// </summary>
    public sealed class ConsoleOutput : Textfile
    {
        /// <summary>
        /// Create a new report from the filename
        /// </summary>
        /// <param name="statsList">List of statistics objects to set</param>
        public ConsoleOutput(List<DatStatistics> statsList) : base(statsList)
        {
            _writeToConsole = true;
        }
    }
}
