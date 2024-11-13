using System.Collections.Generic;
using SabreTools.Core.Tools;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;

namespace SabreTools.DatTools
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
                    if (!onlySame || (onlySame && machine.GetStringFieldValue(Models.Metadata.Machine.NameKey) == machine.GetStringFieldValue(Models.Metadata.Machine.DescriptionKey)))
                        machine.SetFieldValue<string?>(Models.Metadata.Machine.DescriptionKey, repMachine.GetStringFieldValue(Models.Metadata.Machine.DescriptionKey));

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

            if (datItem.GetStringFieldValue(Models.Metadata.DatItem.TypeKey) != repDatItem.GetStringFieldValue(Models.Metadata.DatItem.TypeKey))
                return;

            // If there are no field names for this type or generic, return
            string? itemType = datItem.GetStringFieldValue(Models.Metadata.DatItem.TypeKey).AsEnumValue<ItemType>().AsStringValue();
            if (itemType == null || (!itemFieldNames.ContainsKey(itemType) && !itemFieldNames.ContainsKey("item")))
                return;

            // Get the combined list of fields to remove
            var fieldNames = new HashSet<string>();
            if (itemFieldNames.ContainsKey(itemType))
                fieldNames.IntersectWith(itemFieldNames[itemType]);
            if (itemFieldNames.ContainsKey("item"))
                fieldNames.IntersectWith(itemFieldNames["item"]);

            // If the field specifically contains Name, set it separately
            if (fieldNames.Contains(Models.Metadata.Rom.NameKey))
                datItem.SetName(repDatItem.GetName());

            #endregion

            #region Item-Specific

            // Handle normal sets first
            foreach (var fieldName in fieldNames)
            {
                datItem.ReplaceField(repDatItem, fieldName);
            }

            // TODO: Filter out hashes before here so these checks actually work
            // Handle special cases
            switch (datItem, repDatItem)
            {
                case (Disk disk, Disk repDisk): ReplaceFields(disk, repDisk, [.. fieldNames]); break;
                case (Media media, Media repMedia): ReplaceFields(media, repMedia, [.. fieldNames]); break;
                case (Rom rom, Rom repRom): ReplaceFields(rom, repRom, [.. fieldNames]); break;
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
                if (string.IsNullOrEmpty(disk.GetStringFieldValue(Models.Metadata.Disk.MD5Key)) && !string.IsNullOrEmpty(newItem.GetStringFieldValue(Models.Metadata.Disk.MD5Key)))
                    disk.SetFieldValue<string?>(Models.Metadata.Disk.MD5Key, newItem.GetStringFieldValue(Models.Metadata.Disk.MD5Key));
            }

            if (datItemFields.Contains(Models.Metadata.Disk.SHA1Key))
            {
                if (string.IsNullOrEmpty(disk.GetStringFieldValue(Models.Metadata.Disk.SHA1Key)) && !string.IsNullOrEmpty(newItem.GetStringFieldValue(Models.Metadata.Disk.SHA1Key)))
                    disk.SetFieldValue<string?>(Models.Metadata.Disk.SHA1Key, newItem.GetStringFieldValue(Models.Metadata.Disk.SHA1Key));
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
                if (string.IsNullOrEmpty(media.GetStringFieldValue(Models.Metadata.Media.MD5Key)) && !string.IsNullOrEmpty(newItem.GetStringFieldValue(Models.Metadata.Media.MD5Key)))
                    media.SetFieldValue<string?>(Models.Metadata.Media.MD5Key, newItem.GetStringFieldValue(Models.Metadata.Media.MD5Key));
            }

            if (datItemFields.Contains(Models.Metadata.Media.SHA1Key))
            {
                if (string.IsNullOrEmpty(media.GetStringFieldValue(Models.Metadata.Media.SHA1Key)) && !string.IsNullOrEmpty(newItem.GetStringFieldValue(Models.Metadata.Media.SHA1Key)))
                    media.SetFieldValue<string?>(Models.Metadata.Media.SHA1Key, newItem.GetStringFieldValue(Models.Metadata.Media.SHA1Key));
            }

            if (datItemFields.Contains(Models.Metadata.Media.SHA256Key))
            {
                if (string.IsNullOrEmpty(media.GetStringFieldValue(Models.Metadata.Media.SHA256Key)) && !string.IsNullOrEmpty(newItem.GetStringFieldValue(Models.Metadata.Media.SHA256Key)))
                    media.SetFieldValue<string?>(Models.Metadata.Media.SHA256Key, newItem.GetStringFieldValue(Models.Metadata.Media.SHA256Key));
            }

            if (datItemFields.Contains(Models.Metadata.Media.SpamSumKey))
            {
                if (string.IsNullOrEmpty(media.GetStringFieldValue(Models.Metadata.Media.SpamSumKey)) && !string.IsNullOrEmpty(newItem.GetStringFieldValue(Models.Metadata.Media.SpamSumKey)))
                    media.SetFieldValue<string?>(Models.Metadata.Media.SpamSumKey, newItem.GetStringFieldValue(Models.Metadata.Media.SpamSumKey));
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
                if (string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.CRCKey)) && !string.IsNullOrEmpty(newItem.GetStringFieldValue(Models.Metadata.Rom.CRCKey)))
                    rom.SetFieldValue<string?>(Models.Metadata.Rom.CRCKey, newItem.GetStringFieldValue(Models.Metadata.Rom.CRCKey));
            }

            if (datItemFields.Contains(Models.Metadata.Rom.MD5Key))
            {
                if (string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.MD5Key)) && !string.IsNullOrEmpty(newItem.GetStringFieldValue(Models.Metadata.Rom.MD5Key)))
                    rom.SetFieldValue<string?>(Models.Metadata.Rom.MD5Key, newItem.GetStringFieldValue(Models.Metadata.Rom.MD5Key));
            }

            if (datItemFields.Contains(Models.Metadata.Rom.SHA1Key))
            {
                if (string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.SHA1Key)) && !string.IsNullOrEmpty(newItem.GetStringFieldValue(Models.Metadata.Rom.SHA1Key)))
                    rom.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, newItem.GetStringFieldValue(Models.Metadata.Rom.SHA1Key));
            }

            if (datItemFields.Contains(Models.Metadata.Rom.SHA256Key))
            {
                if (string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.SHA256Key)) && !string.IsNullOrEmpty(newItem.GetStringFieldValue(Models.Metadata.Rom.SHA256Key)))
                    rom.SetFieldValue<string?>(Models.Metadata.Rom.SHA256Key, newItem.GetStringFieldValue(Models.Metadata.Rom.SHA256Key));
            }

            if (datItemFields.Contains(Models.Metadata.Rom.SHA384Key))
            {
                if (string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.SHA384Key)) && !string.IsNullOrEmpty(newItem.GetStringFieldValue(Models.Metadata.Rom.SHA384Key)))
                    rom.SetFieldValue<string?>(Models.Metadata.Rom.SHA384Key, newItem.GetStringFieldValue(Models.Metadata.Rom.SHA384Key));
            }

            if (datItemFields.Contains(Models.Metadata.Rom.SHA512Key))
            {
                if (string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.SHA512Key)) && !string.IsNullOrEmpty(newItem.GetStringFieldValue(Models.Metadata.Rom.SHA512Key)))
                    rom.SetFieldValue<string?>(Models.Metadata.Rom.SHA512Key, newItem.GetStringFieldValue(Models.Metadata.Rom.SHA512Key));
            }

            if (datItemFields.Contains(Models.Metadata.Rom.SpamSumKey))
            {
                if (string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.SpamSumKey)) && !string.IsNullOrEmpty(newItem.GetStringFieldValue(Models.Metadata.Rom.SpamSumKey)))
                    rom.SetFieldValue<string?>(Models.Metadata.Rom.SpamSumKey, newItem.GetStringFieldValue(Models.Metadata.Rom.SpamSumKey));
            }
        }
    }
}