using System.Collections.Generic;
using System.Linq;
using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;

namespace SabreTools.Filtering
{
    /// <summary>
    /// Replace fields in DatItems
    /// </summary>
    public static class Replacer
    {
        /// <summary>
        /// Replace fields with given values
        /// </summary>
        /// <param name="machine">Machine to replace fields in</param>
        /// <param name="repMachine">Machine to pull new information from</param>
        /// <param name="machineFieldNames">List of fields representing what should be updated</param>
        /// <param name="onlySame">True if descriptions should only be replaced if the game name is the same, false otherwise</param>
        public static void ReplaceFields(Machine machine, Machine repMachine, List<string> machineFieldNames, bool onlySame)
        {
            // If we have an invalid input, return
            if (machine == null || repMachine == null || machineFieldNames == null)
                return;

            // Loop through and replace fields
            foreach (string fieldName in machineFieldNames)
            {
                // Special case for description
                if (machineFieldNames.Contains(Models.Metadata.Machine.DescriptionKey))
                {
                    if (!onlySame || (onlySame && machine.Name == machine.Description))
                        machine.Description = repMachine.Description;

                    continue;
                }

                machine.ReplaceField(repMachine, fieldName);
            }
        }

        /// <summary>
        /// Replace fields with given values
        /// </summary>
        /// <param name="datItem">DatItem to replace fields in</param>
        /// <param name="repDatItem">DatItem to pull new information from</param>
        /// <param name="itemFieldNames">List of fields representing what should be updated</param>
        public static void ReplaceFields(DatItem datItem, DatItem repDatItem, Dictionary<string, List<string>> itemFieldNames)
        {
            // If we have an invalid input, return
            if (datItem == null || repDatItem == null || itemFieldNames == null)
                return;

            #region Common

            if (datItem.ItemType != repDatItem.ItemType)
                return;

            // If there are no field names for this type or generic, return
            string? itemType = datItem.ItemType.AsStringValue<ItemType>();
            if (itemType == null || (!itemFieldNames.ContainsKey(itemType) && !itemFieldNames.ContainsKey("item")))
                return;

            // Get the combined list of fields to remove
            var fieldNames = new List<string>();
            if (itemFieldNames.ContainsKey(itemType))
                fieldNames.AddRange(itemFieldNames[itemType]);
            if (itemFieldNames.ContainsKey("item"))
                fieldNames.AddRange(itemFieldNames["item"]);
            fieldNames = fieldNames.Distinct().ToList();

            // If the field specifically contains Name, set it separately
            if (fieldNames.Contains(Models.Metadata.Rom.NameKey))
                datItem.SetName(repDatItem.GetName());

            #endregion

            #region Item-Specific

            // Handle unnested sets first
            foreach (var fieldName in fieldNames)
            {
                datItem.ReplaceField(repDatItem, fieldName);
            }

            // Handle nested sets
            switch (datItem, repDatItem)
            {
                case (Disk disk, Disk repDisk): ReplaceFields(disk, repDisk, fieldNames); break;
                case (Media media, Media repMedia): ReplaceFields(media, repMedia, fieldNames); break;
                case (Rom rom, Rom repRom): ReplaceFields(rom, repRom, fieldNames); break;
            }

            #endregion
        }

        /// <summary>
        /// Replace fields with given values
        /// </summary>
        /// <param name="disk">Disk to remove replace fields in</param>
        /// <param name="newItem">Disk to pull new information from</param>
        /// <param name="datItemFields">List of fields representing what should be updated</param>
        private static void ReplaceFields(Disk disk, Disk newItem, List<string> datItemFields)
        {
            if (datItemFields.Contains(Models.Metadata.Disk.MD5Key))
            {
                if (string.IsNullOrEmpty(disk.MD5) && !string.IsNullOrEmpty(newItem.MD5))
                    disk.MD5 = newItem.MD5;
            }

            if (datItemFields.Contains(Models.Metadata.Disk.SHA1Key))
            {
                if (string.IsNullOrEmpty(disk.SHA1) && !string.IsNullOrEmpty(newItem.SHA1))
                    disk.SHA1 = newItem.SHA1;
            }
        }

        /// <summary>
        /// Replace fields with given values
        /// </summary>
        /// <param name="media">Media to remove replace fields in</param>
        /// <param name="newItem">Media to pull new information from</param>
        /// <param name="datItemFields">List of fields representing what should be updated</param>
        private static void ReplaceFields(Media media, Media newItem, List<string> datItemFields)
        {
            if (datItemFields.Contains(Models.Metadata.Media.MD5Key))
            {
                if (string.IsNullOrEmpty(media.MD5) && !string.IsNullOrEmpty(newItem.MD5))
                    media.MD5 = newItem.MD5;
            }

            if (datItemFields.Contains(Models.Metadata.Media.SHA1Key))
            {
                if (string.IsNullOrEmpty(media.SHA1) && !string.IsNullOrEmpty(newItem.SHA1))
                    media.SHA1 = newItem.SHA1;
            }

            if (datItemFields.Contains(Models.Metadata.Media.SHA256Key))
            {
                if (string.IsNullOrEmpty(media.SHA256) && !string.IsNullOrEmpty(newItem.SHA256))
                    media.SHA256 = newItem.SHA256;
            }

            if (datItemFields.Contains(Models.Metadata.Media.SpamSumKey))
            {
                if (string.IsNullOrEmpty(media.SpamSum) && !string.IsNullOrEmpty(newItem.SpamSum))
                    media.SpamSum = newItem.SpamSum;
            }
        }

        /// <summary>
        /// Replace fields with given values
        /// </summary>
        /// <param name="rom">Rom to remove replace fields in</param>
        /// <param name="newItem">Rom to pull new information from</param>
        /// <param name="datItemFields">List of fields representing what should be updated</param>
        private static void ReplaceFields(Rom rom, Rom newItem, List<string> datItemFields)
        {
            if (datItemFields.Contains(Models.Metadata.Rom.CRCKey))
            {
                if (string.IsNullOrEmpty(rom.CRC) && !string.IsNullOrEmpty(newItem.CRC))
                    rom.CRC = newItem.CRC;
            }

            if (datItemFields.Contains(Models.Metadata.Rom.MD5Key))
            {
                if (string.IsNullOrEmpty(rom.MD5) && !string.IsNullOrEmpty(newItem.MD5))
                    rom.MD5 = newItem.MD5;
            }

            if (datItemFields.Contains(Models.Metadata.Rom.SHA1Key))
            {
                if (string.IsNullOrEmpty(rom.SHA1) && !string.IsNullOrEmpty(newItem.SHA1))
                    rom.SHA1 = newItem.SHA1;
            }

            if (datItemFields.Contains(Models.Metadata.Rom.SHA256Key))
            {
                if (string.IsNullOrEmpty(rom.SHA256) && !string.IsNullOrEmpty(newItem.SHA256))
                    rom.SHA256 = newItem.SHA256;
            }

            if (datItemFields.Contains(Models.Metadata.Rom.SHA384Key))
            {
                if (string.IsNullOrEmpty(rom.SHA384) && !string.IsNullOrEmpty(newItem.SHA384))
                    rom.SHA384 = newItem.SHA384;
            }

            if (datItemFields.Contains(Models.Metadata.Rom.SHA512Key))
            {
                if (string.IsNullOrEmpty(rom.SHA512) && !string.IsNullOrEmpty(newItem.SHA512))
                    rom.SHA512 = newItem.SHA512;
            }

            if (datItemFields.Contains(Models.Metadata.Rom.SpamSumKey))
            {
                if (string.IsNullOrEmpty(rom.SpamSum) && !string.IsNullOrEmpty(newItem.SpamSum))
                    rom.SpamSum = newItem.SpamSum;
            }
        }
    }
}