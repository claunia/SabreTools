using System;
using System.Linq;
using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;

namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents parsing a MAME Listrom file
    /// </summary>
    internal partial class Listrom : DatFile
    {
        /// <inheritdoc/>
        public override void ParseFile(string filename, int indexId, bool keep, bool statsOnly = false, bool throwOnError = false)
        {
            try
            {
                // Deserialize the input file
                var metadataFile = new Serialization.Files.Listrom().Deserialize(filename);

                // Convert the set data to the internal format
                ConvertSets(metadataFile?.Set, filename, indexId, statsOnly);
            }
            catch (Exception ex) when (!throwOnError)
            {
                string message = $"'{filename}' - An error occurred during parsing";
                logger.Error(ex, message);
            }
        }

        #region Converters

        /// <summary>
        /// Convert sets information
        /// </summary>
        /// <param name="sets">Array of deserialized models to convert</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ConvertSets(Models.Listrom.Set[]? sets, string filename, int indexId, bool statsOnly)
        {
            // If the rows array is missing, we can't do anything
            if (sets == null || !sets.Any())
                return;

            // Loop through the sets and add
            foreach (var set in sets)
            {
                ConvertSet(set, filename, indexId, statsOnly);
            }
        }

        /// <summary>
        /// Convert set information
        /// </summary>
        /// <param name="set">Deserialized model to convert</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ConvertSet(Models.Listrom.Set? set, string filename, int indexId, bool statsOnly)
        {
            // If the set is missing, we can't do anything
            if (set == null)
                return;

            // Create the machine
            Machine machine;
            if (!string.IsNullOrEmpty(set.Device))
            {
                machine = new Machine();
                machine.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, set.Device);
                machine.SetFieldValue<bool?>(Models.Metadata.Machine.IsDeviceKey, true);
            }
            else if (!string.IsNullOrEmpty(set.Driver))
            {
                machine = new Machine();
                machine.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, set.Driver);
            }
            else
            {
                return;
            }

            foreach (var row in set.Row ?? [])
            {
                ConvertRow(row, machine, filename, indexId, statsOnly);
            }
        }

        /// <summary>
        /// Convert row information
        /// </summary>
        /// <param name="row">Deserialized model to convert</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ConvertRow(Models.Listrom.Row? row, Machine machine, string filename, int indexId, bool statsOnly)
        {
            // If the row is missing, we can't do anything
            if (row == null)
                return;

            // Normal CHD
            if (row.Size == null
                && !row.NoGoodDumpKnown
                && !row.Bad
                && (!string.IsNullOrEmpty(row.MD5)
                    || !string.IsNullOrEmpty(row.SHA1)))
            {
                var disk = new Disk();
                disk.SetName(row.Name);
                disk.SetFieldValue<ItemStatus>(Models.Metadata.Disk.StatusKey, ItemStatus.None);
                disk.SetFieldValue<Source?>(DatItem.SourceKey, new Source { Index = indexId, Name = filename });

                if (!string.IsNullOrEmpty(row.MD5))
                    disk.SetFieldValue<string?>(Models.Metadata.Disk.MD5Key, row.MD5);
                else
                    disk.SetFieldValue<string?>(Models.Metadata.Disk.SHA1Key, row.SHA1);

                // Now process and add the item
                disk.CopyMachineInformation(machine);
                ParseAddHelper(disk, statsOnly);
            }

            // Normal ROM
            else if (row.Size != null
                && !row.NoGoodDumpKnown
                && !row.Bad)
            {
                var rom = new Rom();
                rom.SetName(row.Name);
                rom.SetFieldValue<string?>(Models.Metadata.Rom.CRCKey, row.CRC);
                rom.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, row.SHA1);
                rom.SetFieldValue<long?>(Models.Metadata.Rom.SizeKey, NumberHelper.ConvertToInt64(row.Size));
                rom.SetFieldValue<Source?>(DatItem.SourceKey, new Source { Index = indexId, Name = filename });
                rom.SetFieldValue<ItemStatus>(Models.Metadata.Rom.StatusKey, ItemStatus.None);

                // Now process and add the item
                rom.CopyMachineInformation(machine);
                ParseAddHelper(rom, statsOnly);
            }

            // Bad CHD
            else if (row.Size == null
                && !row.NoGoodDumpKnown
                && row.Bad
                && (!string.IsNullOrEmpty(row.MD5)
                    || !string.IsNullOrEmpty(row.SHA1)))
            {
                var disk = new Disk();
                disk.SetName(row.Name);
                disk.SetFieldValue<ItemStatus>(Models.Metadata.Disk.StatusKey, value: ItemStatus.BadDump);
                disk.SetFieldValue<Source?>(DatItem.SourceKey, new Source { Index = indexId, Name = filename });

                if (!string.IsNullOrEmpty(row.MD5))
                    disk.SetFieldValue<string?>(Models.Metadata.Disk.MD5Key, row.MD5);
                else
                    disk.SetFieldValue<string?>(Models.Metadata.Disk.SHA1Key, row.SHA1);

                // Now process and add the item
                disk.CopyMachineInformation(machine);
                ParseAddHelper(disk, statsOnly);
            }

            // Nodump CHD
            else if (row.Size == null
                && row.NoGoodDumpKnown)
            {
                var disk = new Disk();
                disk.SetName(row.Name);
                disk.SetFieldValue<string?>(Models.Metadata.Disk.MD5Key, null);
                disk.SetFieldValue<string?>(Models.Metadata.Disk.SHA1Key, null);
                disk.SetFieldValue<Source?>(DatItem.SourceKey, new Source { Index = indexId, Name = filename });
                disk.SetFieldValue<ItemStatus?>(Models.Metadata.Disk.StatusKey, ItemStatus.Nodump);

                // Now process and add the item
                disk.CopyMachineInformation(machine);
                ParseAddHelper(disk, statsOnly);
            }

            // Bad ROM
            else if (row.Size != null
                && !row.NoGoodDumpKnown
                && row.Bad)
            {
                var rom = new Rom();
                rom.SetName(row.Name);
                rom.SetFieldValue<string?>(Models.Metadata.Rom.CRCKey, row.CRC);
                rom.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, row.SHA1);
                rom.SetFieldValue<long?>(Models.Metadata.Rom.SizeKey, NumberHelper.ConvertToInt64(row.Size));
                rom.SetFieldValue<Source?>(DatItem.SourceKey, new Source { Index = indexId, Name = filename });
                rom.SetFieldValue<ItemStatus>(Models.Metadata.Rom.StatusKey, ItemStatus.BadDump);

                // Now process and add the item
                rom.CopyMachineInformation(machine);
                ParseAddHelper(rom, statsOnly);
            }

            // Nodump ROM
            else if (row.Size != null
                && row.NoGoodDumpKnown)
            {
                var rom = new Rom();
                rom.SetName(row.Name);
                rom.SetFieldValue<string?>(Models.Metadata.Rom.CRCKey, null);
                rom.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, null);
                rom.SetFieldValue<long?>(Models.Metadata.Rom.SizeKey, NumberHelper.ConvertToInt64(row.Size));
                rom.SetFieldValue<Source?>(DatItem.SourceKey, new Source { Index = indexId, Name = filename });
                rom.SetFieldValue<ItemStatus>(Models.Metadata.Rom.StatusKey, ItemStatus.Nodump);

                // Now process and add the item
                rom.CopyMachineInformation(machine);
                ParseAddHelper(rom, statsOnly);
            }
        }

        #endregion
    }
}
