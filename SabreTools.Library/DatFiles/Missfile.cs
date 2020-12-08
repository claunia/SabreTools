using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using SabreTools.DatItems;

namespace SabreTools.Library.DatFiles
{
    /// <summary>
    /// Represents parsing and writing of a Missfile
    /// </summary>
    internal class Missfile : DatFile
    {
        /// <summary>
        /// Constructor designed for casting a base DatFile
        /// </summary>
        /// <param name="datFile">Parent DatFile to copy from</param>
        public Missfile(DatFile datFile)
            : base(datFile)
        {
        }

        /// <summary>
        /// Parse a Missfile and return all found games and roms within
        /// </summary>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="keep">True if full pathnames are to be kept, false otherwise (default)</param>
        /// <param name="throwOnError">True if the error that is thrown should be thrown back to the caller, false otherwise</param>
        protected override void ParseFile(string filename, int indexId, bool keep, bool throwOnError = false)
        {
            // There is no consistent way to parse a missfile...
            throw new NotImplementedException();
        }

        /// <summary>
        /// Create and open an output file for writing direct from a dictionary
        /// </summary>
        /// <param name="outfile">Name of the file to write to</param>
        /// <param name="ignoreblanks">True if blank roms should be skipped on output, false otherwise (default)</param>
        /// <param name="throwOnError">True if the error that is thrown should be thrown back to the caller, false otherwise</param>
        /// <returns>True if the DAT was written correctly, false otherwise</returns>
        public override bool WriteToFile(string outfile, bool ignoreblanks = false, bool throwOnError = false)
        {
            try
            {
                logger.User($"Opening file for writing: {outfile}");
                FileStream fs = File.Create(outfile);

                // If we get back null for some reason, just log and return
                if (fs == null)
                {
                    logger.Warning($"File '{outfile}' could not be created for writing! Please check to see if the file is writable");
                    return false;
                }

                StreamWriter sw = new StreamWriter(fs, new UTF8Encoding(false));

                // Write out each of the machines and roms
                string lastgame = null;

                // Use a sorted list of games to output
                foreach (string key in Items.SortedKeys)
                {
                    List<DatItem> datItems = Items.FilteredItems(key);

                    // If this machine doesn't contain any writable items, skip
                    if (!ContainsWritable(datItems))
                        continue;

                    // Resolve the names in the block
                    datItems = DatItem.ResolveNames(datItems);

                    for (int index = 0; index < datItems.Count; index++)
                    {
                        DatItem datItem = datItems[index];

                        // Check for a "null" item
                        datItem = ProcessNullifiedItem(datItem);

                        // Write out the item if we're using machine names or we're not ignoring
                        if (!Header.UseRomName || !ShouldIgnore(datItem, ignoreblanks))
                            WriteDatItem(sw, datItem, lastgame);

                        // Set the new data to compare against
                        lastgame = datItem.Machine.Name;
                    }
                }

                logger.Verbose("File written!" + Environment.NewLine);
                sw.Dispose();
                fs.Dispose();
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
        /// Write out DatItem using the supplied StreamWriter
        /// </summary>
        /// <param name="sw">StreamWriter to output to</param>
        /// <param name="datItem">DatItem object to be output</param>
        /// <param name="lastgame">The name of the last game to be output</param>
        private void WriteDatItem(StreamWriter sw, DatItem datItem, string lastgame)
        {
            // Process the item name
            ProcessItemName(datItem, false, forceRomName: false);

            // Romba mode automatically uses item name
            if (Header.OutputDepot?.IsActive == true || Header.UseRomName)
                sw.Write($"{datItem.GetName() ?? string.Empty}\n");
            else if (!Header.UseRomName && datItem.Machine.Name != lastgame)
                sw.Write($"{datItem.Machine.Name}\n");

            sw.Flush();
        }
    }
}
