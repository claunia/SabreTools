using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SabreTools.Core;
using SabreTools.DatItems;

namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents writing a Missfile
    /// </summary>
    internal partial class Missfile : DatFile
    {
        /// <inheritdoc/>
        protected override List<DatItemField> GetMissingRequiredFields(DatItem datItem)
        {
            // TODO: Check required fields
            return null;
        }

        /// <inheritdoc/>
        public override bool WriteToFile(string outfile, bool ignoreblanks = false, bool throwOnError = false)
        {
            try
            {
                logger.User($"Writing to '{outfile}'...");
                FileStream fs = File.Create(outfile);

                // If we get back null for some reason, just log and return
                if (fs == null)
                {
                    logger.Warning($"File '{outfile}' could not be created for writing! Please check to see if the file is writable");
                    return false;
                }

                StreamWriter sw = new(fs, new UTF8Encoding(false));

                // Write out each of the machines and roms
                string lastgame = null;

                // Use a sorted list of games to output
                foreach (string key in Items.SortedKeys)
                {
                    ConcurrentList<DatItem> datItems = Items.FilteredItems(key);

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

                logger.User($"'{outfile}' written!{Environment.NewLine}");
                sw.Dispose();
                fs.Dispose();
            }
            catch (Exception ex) when (!throwOnError)
            {
                logger.Error(ex);
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
