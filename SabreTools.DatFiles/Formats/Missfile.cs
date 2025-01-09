using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SabreTools.DatItems;

namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents a Missfile
    /// </summary>
    internal sealed class Missfile : DatFile
    {
        /// <inheritdoc/>
        public override ItemType[] SupportedTypes
            => Enum.GetValues(typeof(ItemType)) as ItemType[] ?? [];

        /// <summary>
        /// Constructor designed for casting a base DatFile
        /// </summary>
        /// <param name="datFile">Parent DatFile to copy from</param>
        public Missfile(DatFile? datFile) : base(datFile)
        {
        }

        /// <inheritdoc/>
        /// <remarks>There is no consistent way to parse a missfile</remarks>
        public override void ParseFile(string filename, int indexId, bool keep, bool statsOnly = false, bool throwOnError = false)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        protected override List<string>? GetMissingRequiredFields(DatItem datItem)
        {
            // TODO: Check required fields
            return null;
        }

        /// <inheritdoc/>
        public override bool WriteToFile(string outfile, bool ignoreblanks = false, bool throwOnError = false)
        {
            try
            {
                _logger.User($"Writing to '{outfile}'...");
                FileStream fs = File.Create(outfile);

                // If we get back null for some reason, just log and return
                if (fs == null)
                {
                    _logger.Warning($"File '{outfile}' could not be created for writing! Please check to see if the file is writable");
                    return false;
                }

                StreamWriter sw = new(fs, new UTF8Encoding(false));

                // Write out each of the machines and roms
                string? lastgame = null;

                // Use a sorted list of games to output
                foreach (string key in Items.SortedKeys)
                {
                    List<DatItem> datItems = Items.FilteredItems(key);

                    // If this machine doesn't contain any writable items, skip
                    if (!ContainsWritable(datItems))
                        continue;

                    // Resolve the names in the block
                    datItems = ResolveNames(datItems);

                    for (int index = 0; index < datItems.Count; index++)
                    {
                        DatItem datItem = datItems[index];

                        // Check for a "null" item
                        datItem = ProcessNullifiedItem(datItem);

                        // Write out the item if we're using machine names or we're not ignoring
                        if (!Header.GetBoolFieldValue(DatHeader.UseRomNameKey) == true || !ShouldIgnore(datItem, ignoreblanks))
                            WriteDatItem(sw, datItem, lastgame);

                        // Set the new data to compare against
                        lastgame = datItem.GetFieldValue<Machine>(DatItem.MachineKey)!.GetStringFieldValue(Models.Metadata.Machine.NameKey);
                    }
                }

                _logger.User($"'{outfile}' written!{Environment.NewLine}");
                sw.Dispose();
                fs.Dispose();
            }
            catch (Exception ex) when (!throwOnError)
            {
                _logger.Error(ex);
                return false;
            }

            return true;
        }

        /// <inheritdoc/>
        public override bool WriteToFileDB(string outfile, bool ignoreblanks = false, bool throwOnError = false)
        {
            try
            {
                _logger.User($"Writing to '{outfile}'...");
                FileStream fs = File.Create(outfile);

                // If we get back null for some reason, just log and return
                if (fs == null)
                {
                    _logger.Warning($"File '{outfile}' could not be created for writing! Please check to see if the file is writable");
                    return false;
                }

                StreamWriter sw = new(fs, new UTF8Encoding(false));

                // Write out each of the machines and roms
                string? lastgame = null;

                // Use a sorted list of games to output
                foreach (string key in ItemsDB.SortedKeys)
                {
                    // If this machine doesn't contain any writable items, skip
                    var itemsDict = ItemsDB.GetItemsForBucket(key, filter: true);
                    if (itemsDict == null || !ContainsWritable([.. itemsDict.Values]))
                        continue;

                    // Resolve the names in the block
                    var items = ResolveNamesDB([.. itemsDict]);

                    foreach (var kvp in items)
                    {
                        // Check for a "null" item
                        var datItem = new KeyValuePair<long, DatItem>(kvp.Key, ProcessNullifiedItem(kvp.Value));

                        // Get the machine for the item
                        var machine = ItemsDB.GetMachineForItem(datItem.Key);

                        // Write out the item if we're using machine names or we're not ignoring
                        if (!Header.GetBoolFieldValue(DatHeader.UseRomNameKey) == true
                            || !ShouldIgnore(datItem.Value, ignoreblanks))
                        {
                            WriteDatItemDB(sw, datItem, lastgame);
                        }

                        // Set the new data to compare against
                        lastgame = machine.Value!.GetStringFieldValue(Models.Metadata.Machine.NameKey);
                    }
                }

                _logger.User($"'{outfile}' written!{Environment.NewLine}");
                sw.Dispose();
                fs.Dispose();
            }
            catch (Exception ex) when (!throwOnError)
            {
                _logger.Error(ex);
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
        private void WriteDatItem(StreamWriter sw, DatItem datItem, string? lastgame)
        {
            // Get the machine for the item
            var machine = datItem.GetFieldValue<Machine>(DatItem.MachineKey);

            // Process the item name
            ProcessItemName(datItem, machine, false, forceRomName: false);

            // Romba mode automatically uses item name
            if (Header.GetFieldValue<DepotInformation?>(DatHeader.OutputDepotKey)?.IsActive == true || Header.GetBoolFieldValue(DatHeader.UseRomNameKey) == true)
                sw.Write($"{datItem.GetName() ?? string.Empty}\n");
            else if (!Header.GetBoolFieldValue(DatHeader.UseRomNameKey) == true && datItem.GetFieldValue<Machine>(DatItem.MachineKey)!.GetStringFieldValue(Models.Metadata.Machine.NameKey) != lastgame)
                sw.Write($"{datItem.GetFieldValue<Machine>(DatItem.MachineKey)!.GetStringFieldValue(Models.Metadata.Machine.NameKey) ?? string.Empty}\n");

            sw.Flush();
        }

        /// <summary>
        /// Write out DatItem using the supplied StreamWriter
        /// </summary>
        /// <param name="sw">StreamWriter to output to</param>
        /// <param name="datItem">DatItem object to be output</param>
        /// <param name="lastgame">The name of the last game to be output</param>
        private void WriteDatItemDB(StreamWriter sw, KeyValuePair<long, DatItem> datItem, string? lastgame)
        {
            // Get the machine for the item
            var machine = ItemsDB.GetMachineForItem(datItem.Key);

            // Process the item name
            ProcessItemName(datItem.Value, machine.Value, forceRemoveQuotes: false, forceRomName: false);

            // Romba mode automatically uses item name
            if (Header.GetFieldValue<DepotInformation?>(DatHeader.OutputDepotKey)?.IsActive == true
                || Header.GetBoolFieldValue(DatHeader.UseRomNameKey) == true)
            {
                sw.Write($"{datItem.Value.GetName() ?? string.Empty}\n");
            }
            else if (!Header.GetBoolFieldValue(DatHeader.UseRomNameKey) == true
                && machine.Value!.GetStringFieldValue(Models.Metadata.Machine.NameKey) != lastgame)
            {
                sw.Write($"{machine.Value!.GetStringFieldValue(Models.Metadata.Machine.NameKey) ?? string.Empty}\n");
            }

            sw.Flush();
        }
    }
}
