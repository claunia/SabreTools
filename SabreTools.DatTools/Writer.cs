using System;
using System.Collections.Generic;
#if NET40_OR_GREATER || NETCOREAPP
using System.Threading.Tasks;
#endif
using SabreTools.DatFiles;
using SabreTools.DatItems;
using SabreTools.IO.Extensions;
using SabreTools.IO.Logging;
using SabreTools.Reports;

namespace SabreTools.DatTools
{
    /// <summary>
    /// Helper methods for writing from DatFiles
    /// </summary>
    public class Writer
    {
        #region Logging

        /// <summary>
        /// Logging object
        /// </summary>
        private static readonly Logger _staticLogger = new();

        #endregion

        /// <summary>
        /// Create and open an output file for writing direct from a dictionary
        /// </summary>
        /// <param name="datFile">Current DatFile object to write from</param>
        /// <param name="outDir">Set the output directory (current directory on null)</param>
        /// <returns>True if the DAT was written correctly, false otherwise</returns>
        public static bool Write(DatFile datFile, string? outDir)
            => Write(datFile, outDir, overwrite: true, throwOnError: false);

        /// <summary>
        /// Create and open an output file for writing direct from a dictionary
        /// </summary>
        /// <param name="datFile">Current DatFile object to write from</param>
        /// <param name="outDir">Set the output directory (current directory on null)</param>
        /// <param name="overwrite">True if files should be overwritten, false if they should be renamed instead</param>
        /// <returns>True if the DAT was written correctly, false otherwise</returns>
        public static bool Write(DatFile datFile, string? outDir, bool overwrite)
            => Write(datFile, outDir, overwrite, throwOnError: false);

        /// <summary>
        /// Create and open an output file for writing direct from a dictionary
        /// </summary>
        /// <param name="datFile">Current DatFile object to write from</param>
        /// <param name="outDir">Set the output directory (current directory on null)</param>
        /// <param name="overwrite">True if files should be overwritten, false if they should be renamed instead</param>
        /// <param name="throwOnError">True if the error that is thrown should be thrown back to the caller, false otherwise</param>
        /// <returns>True if the DAT was written correctly, false otherwise</returns>
        public static bool Write(DatFile datFile, string? outDir, bool overwrite, bool throwOnError)
        {
            // If we have nothing writable, abort
            if (!HasWritable(datFile))
            {
                _staticLogger.User("There were no items to write out!");
                return false;
            }

            // Ensure the output directory is set and created
            outDir = outDir.Ensure(create: true);

            InternalStopwatch watch = new($"Writing out internal dat to '{outDir}'");

            // If the DAT has no output format, default to XML
            if (datFile.Header.GetFieldValue<DatFormat>(DatHeader.DatFormatKey) == 0)
            {
                _staticLogger.Verbose("No DAT format defined, defaulting to XML");
                datFile.Header.SetFieldValue<DatFormat>(DatHeader.DatFormatKey, DatFormat.Logiqx);
            }

            // Make sure that the three essential fields are filled in
            EnsureHeaderFields(datFile);

            // Bucket roms by game name, if not already
            datFile.BucketBy(ItemKey.Machine);

            // Output the number of items we're going to be writing
            _staticLogger.User($"A total of {datFile.DatStatistics.TotalCount - datFile.DatStatistics.RemovedCount} items will be written out to '{datFile.Header.GetStringFieldValue(DatHeader.FileNameKey)}'");

            // Get the outfile names
            Dictionary<DatFormat, string> outfiles = datFile.Header.CreateOutFileNames(outDir!, overwrite);

            try
            {
                // Write out all required formats
#if NET452_OR_GREATER || NETCOREAPP
                Parallel.ForEach(outfiles.Keys, Core.Globals.ParallelOptions, datFormat =>
#elif NET40_OR_GREATER
                Parallel.ForEach(outfiles.Keys, datFormat =>
#else
                foreach (var datFormat in outfiles.Keys)
#endif
                {
                    string outfile = outfiles[datFormat];
                    try
                    {
                        DatFile writingDatFile = DatFileTool.CreateDatFile(datFormat, datFile);
                        writingDatFile.WriteToFile(outfile, ignoreblanks: true, throwOnError);
                    }
                    catch (Exception ex) when (!throwOnError)
                    {
                        _staticLogger.Error(ex, $"Datfile '{outfile}' could not be written out");
                    }
#if NET40_OR_GREATER || NETCOREAPP
                });
#else
                }
#endif
            }
            catch (Exception ex) when (!throwOnError)
            {
                _staticLogger.Error(ex);
                return false;
            }
            finally
            {
                watch.Stop();
            }

            return true;
        }

        /// <summary>
        /// Write the stats out to console for the current DatFile
        /// </summary>
        /// <param name="datFile">Current DatFile object to write from</param>
        public static void WriteStatsToConsole(DatFile datFile)
        {
            long diskCount = datFile.DatStatistics.GetItemCount(ItemType.Disk);
            long mediaCount = datFile.DatStatistics.GetItemCount(ItemType.Media);
            long romCount = datFile.DatStatistics.GetItemCount(ItemType.Rom);

            if (diskCount + mediaCount + romCount == 0)
                datFile.RecalculateStats();

            datFile.BucketBy(ItemKey.Machine, norename: true);

            datFile.DatStatistics.DisplayName = datFile.Header.GetStringFieldValue(DatHeader.FileNameKey);
            datFile.DatStatistics.MachineCount = datFile.Items.SortedKeys.Length;

            List<DatStatistics> statsList =
            [
                datFile.DatStatistics,
            ];
            var consoleOutput = BaseReport.Create(StatReportFormat.None, statsList);
            consoleOutput!.WriteToFile(null, true, true);
        }

        /// <summary>
        /// Ensure that FileName, Name, and Description are filled with some value
        /// </summary>
        /// <param name="datFile">Current DatFile object to write from</param>
        private static void EnsureHeaderFields(DatFile datFile)
        {
            // Empty FileName
            if (string.IsNullOrEmpty(datFile.Header.GetStringFieldValue(DatHeader.FileNameKey)))
            {
                if (string.IsNullOrEmpty(datFile.Header.GetStringFieldValue(Models.Metadata.Header.NameKey)) && string.IsNullOrEmpty(datFile.Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey)))
                {
                    datFile.Header.SetFieldValue<string?>(DatHeader.FileNameKey, "Default");
                    datFile.Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, "Default");
                    datFile.Header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, "Default");
                }

                else if (string.IsNullOrEmpty(datFile.Header.GetStringFieldValue(Models.Metadata.Header.NameKey)) && !string.IsNullOrEmpty(datFile.Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey)))
                {
                    datFile.Header.SetFieldValue<string?>(DatHeader.FileNameKey, datFile.Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey));
                    datFile.Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, datFile.Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey));
                }

                else if (!string.IsNullOrEmpty(datFile.Header.GetStringFieldValue(Models.Metadata.Header.NameKey)) && string.IsNullOrEmpty(datFile.Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey)))
                {
                    datFile.Header.SetFieldValue<string?>(DatHeader.FileNameKey, datFile.Header.GetStringFieldValue(Models.Metadata.Header.NameKey));
                    datFile.Header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, datFile.Header.GetStringFieldValue(Models.Metadata.Header.NameKey));
                }

                else if (!string.IsNullOrEmpty(datFile.Header.GetStringFieldValue(Models.Metadata.Header.NameKey)) && !string.IsNullOrEmpty(datFile.Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey)))
                {
                    datFile.Header.SetFieldValue<string?>(DatHeader.FileNameKey, datFile.Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey));
                }
            }

            // Filled FileName
            else
            {
                if (string.IsNullOrEmpty(datFile.Header.GetStringFieldValue(Models.Metadata.Header.NameKey)) && string.IsNullOrEmpty(datFile.Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey)))
                {
                    datFile.Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, datFile.Header.GetStringFieldValue(DatHeader.FileNameKey));
                    datFile.Header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, datFile.Header.GetStringFieldValue(DatHeader.FileNameKey));
                }

                else if (string.IsNullOrEmpty(datFile.Header.GetStringFieldValue(Models.Metadata.Header.NameKey)) && !string.IsNullOrEmpty(datFile.Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey)))
                {
                    datFile.Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, datFile.Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey));
                }

                else if (!string.IsNullOrEmpty(datFile.Header.GetStringFieldValue(Models.Metadata.Header.NameKey)) && string.IsNullOrEmpty(datFile.Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey)))
                {
                    datFile.Header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, datFile.Header.GetStringFieldValue(Models.Metadata.Header.NameKey));
                }
            }
        }

        /// <summary>
        /// Get if the DatFile has any writable items
        /// </summary>
        /// <param name="datFile">Current DatFile object to write from</param>
        /// <returns>True if there are any writable items, false otherwise</returns>
        private static bool HasWritable(DatFile datFile)
        {
            // Force a statistics recheck, just in case
            datFile.RecalculateStats();

            // If there's nothing there, abort
            if (datFile.DatStatistics.TotalCount == 0)
                return false;
            // if (datFile.ItemsDB.DatStatistics.TotalCount == 0)
            //     return false;

            // If every item is removed, abort
            if (datFile.DatStatistics.TotalCount == datFile.DatStatistics.RemovedCount)
                return false;
            // if (datFile.ItemsDB.DatStatistics.TotalCount == datFile.ItemsDB.DatStatistics.RemovedCount)
            //     return false;

            return true;
        }
    }
}