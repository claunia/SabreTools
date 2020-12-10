using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using SabreTools.Core;
using SabreTools.IO;

// This file represents all methods related to writing to a file
namespace SabreTools.DatFiles
{
    // TODO: Re-evaluate if these should be made static instead of instanced
    public partial class DatTool
    {
        /// <summary>
        /// Create and open an output file for writing direct from a dictionary
        /// </summary>
        /// <param name="datFile">Current DatFile object to write from</param>
        /// <param name="outDir">Set the output directory (current directory on null)</param>
        /// <param name="overwrite">True if files should be overwritten (default), false if they should be renamed instead</param>
        /// <param name="ignoreblanks">True if blank roms should be skipped on output, false otherwise (default)</param>
        /// <param name="quotes">True if quotes are assumed in supported types (default), false otherwise</param>
        /// <param name="throwOnError">True if the error that is thrown should be thrown back to the caller, false otherwise</param>
        /// <returns>True if the DAT was written correctly, false otherwise</returns>
        public bool Write(
            DatFile datFile,
            string outDir,
            bool overwrite = true,
            bool ignoreblanks = false,
            bool quotes = true,
            bool throwOnError = false)
        {
            // If we have nothing writable, abort
            if (!HasWritable(datFile))
            {
                logger.User("There were no items to write out!");
                return false;
            }

            // Ensure the output directory is set and created
            outDir = DirectoryExtensions.Ensure(outDir, create: true);

            // If the DAT has no output format, default to XML
            if (datFile.Header.DatFormat == 0)
            {
                logger.Verbose("No DAT format defined, defaulting to XML");
                datFile.Header.DatFormat = DatFormat.Logiqx;
            }

            // Make sure that the three essential fields are filled in
            EnsureHeaderFields();

            // Bucket roms by game name, if not already
            datFile.Items.BucketBy(Field.Machine_Name, DedupeType.None);

            // Output the number of items we're going to be writing
            logger.User($"A total of {datFile.Items.TotalCount - datFile.Items.RemovedCount} items will be written out to '{datFile.Header.FileName}'");

            // Get the outfile names
            Dictionary<DatFormat, string> outfiles = datFile.Header.CreateOutFileNames(outDir, overwrite);

            try
            {
                // Write out all required formats
                Parallel.ForEach(outfiles.Keys, Globals.ParallelOptions, datFormat =>
                {
                    string outfile = outfiles[datFormat];
                    try
                    {
                        DatFile.Create(datFormat, datFile, quotes)?.WriteToFile(outfile, ignoreblanks, throwOnError);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex, $"Datfile {outfile} could not be written out");
                        if (throwOnError) throw ex;
                    }

                });
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                if (throwOnError) throw ex;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Ensure that FileName, Name, and Description are filled with some value
        /// </summary>
        /// <param name="datFile">Current DatFile object to write from</param>
        private void EnsureHeaderFields(DatFile datFile)
        {
            // Empty FileName
            if (string.IsNullOrWhiteSpace(datFile.Header.FileName))
            {
                if (string.IsNullOrWhiteSpace(datFile.Header.Name) && string.IsNullOrWhiteSpace(datFile.Header.Description))
                    datFile.Header.FileName = datFile.Header.Name = datFile.Header.Description = "Default";

                else if (string.IsNullOrWhiteSpace(datFile.Header.Name) && !string.IsNullOrWhiteSpace(datFile.Header.Description))
                    datFile.Header.FileName = datFile.Header.Name = datFile.Header.Description;

                else if (!string.IsNullOrWhiteSpace(datFile.Header.Name) && string.IsNullOrWhiteSpace(datFile.Header.Description))
                    datFile.Header.FileName = datFile.Header.Description = datFile.Header.Name;

                else if (!string.IsNullOrWhiteSpace(datFile.Header.Name) && !string.IsNullOrWhiteSpace(datFile.Header.Description))
                    datFile.Header.FileName = datFile.Header.Description;
            }

            // Filled FileName
            else
            {
                if (string.IsNullOrWhiteSpace(datFile.Header.Name) && string.IsNullOrWhiteSpace(datFile.Header.Description))
                    datFile.Header.Name = datFile.Header.Description = datFile.Header.FileName;

                else if (string.IsNullOrWhiteSpace(datFile.Header.Name) && !string.IsNullOrWhiteSpace(datFile.Header.Description))
                    datFile.Header.Name = datFile.Header.Description;

                else if (!string.IsNullOrWhiteSpace(datFile.Header.Name) && string.IsNullOrWhiteSpace(datFile.Header.Description))
                    datFile.Header.Description = datFile.Header.Name;
            }
        }

        /// <summary>
        /// Get if the DatFile has any writable items
        /// </summary>
        /// <param name="datFile">Current DatFile object to write from</param>
        /// <returns>True if there are any writable items, false otherwise</returns>
        private bool HasWritable(DatFile datFile)
        {
            // Force a statistics recheck, just in case
            datFile.Items.RecalculateStats();

            // If there's nothing there, abort
            if (datFile.Items.TotalCount == 0)
                return false;

            // If every item is removed, abort
            if (datFile.Items.TotalCount == datFile.Items.RemovedCount)
                return false;

            return true;
        }    
    }
}