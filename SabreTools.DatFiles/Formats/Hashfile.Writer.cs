using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SabreTools.Core;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;

namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents writing a hashfile such as an SFV, MD5, or SHA-1 file
    /// </summary>
    internal partial class Hashfile : DatFile
    {
        /// <inheritdoc/>
        protected override ItemType[] GetSupportedTypes()
        {
            return
            [
                ItemType.Disk,
                ItemType.Media,
                ItemType.Rom
            ];
        }

        /// <inheritdoc/>
        protected override List<string>? GetMissingRequiredFields(DatItem datItem)
        {
            var missingFields = new List<string>();

            // Check item name
            if (string.IsNullOrEmpty(datItem.GetName()))
                missingFields.Add(Models.Metadata.Rom.NameKey);

            // Check hash linked to specific Hashfile type
            switch (_hash)
            {
                case Serialization.Hash.CRC:
                    switch (datItem)
                    {
                        case Rom rom:
                            if (string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.CRCKey)))
                                missingFields.Add(Models.Metadata.Rom.CRCKey);
                            break;
                        default:
                            missingFields.Add(Models.Metadata.Rom.CRCKey);
                            break;
                    }
                    break;
                case Serialization.Hash.MD5:
                    switch (datItem)
                    {
                        case Disk disk:
                            if (string.IsNullOrEmpty(disk.GetStringFieldValue(Models.Metadata.Disk.MD5Key)))
                                missingFields.Add(Models.Metadata.Disk.MD5Key);
                            break;
                        case Media medium:
                            if (string.IsNullOrEmpty(medium.GetStringFieldValue(Models.Metadata.Media.MD5Key)))
                                missingFields.Add(Models.Metadata.Media.MD5Key);
                            break;
                        case Rom rom:
                            if (string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.MD5Key)))
                                missingFields.Add(Models.Metadata.Rom.MD5Key);
                            break;
                        default:
                            missingFields.Add(Models.Metadata.Rom.MD5Key);
                            break;
                    }
                    break;
                case Serialization.Hash.SHA1:
                    switch (datItem)
                    {
                        case Disk disk:
                            if (string.IsNullOrEmpty(disk.GetStringFieldValue(Models.Metadata.Disk.SHA1Key)))
                                missingFields.Add(Models.Metadata.Disk.SHA1Key);
                            break;
                        case Media medium:
                            if (string.IsNullOrEmpty(medium.GetStringFieldValue(Models.Metadata.Media.SHA1Key)))
                                missingFields.Add(Models.Metadata.Media.SHA1Key);
                            break;
                        case Rom rom:
                            if (string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.SHA1Key)))
                                missingFields.Add(Models.Metadata.Rom.SHA1Key);
                            break;
                        default:
                            missingFields.Add(Models.Metadata.Rom.SHA1Key);
                            break;
                    }
                    break;
                case Serialization.Hash.SHA256:
                    switch (datItem)
                    {
                        case Media medium:
                            if (string.IsNullOrEmpty(medium.GetStringFieldValue(Models.Metadata.Media.SHA256Key)))
                                missingFields.Add(Models.Metadata.Media.SHA256Key);
                            break;
                        case Rom rom:
                            if (string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.SHA256Key)))
                                missingFields.Add(Models.Metadata.Rom.SHA256Key);
                            break;
                        default:
                            missingFields.Add(Models.Metadata.Rom.SHA256Key);
                            break;
                    }
                    break;
                case Serialization.Hash.SHA384:
                    switch (datItem)
                    {
                        case Rom rom:
                            if (string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.SHA384Key)))
                                missingFields.Add(Models.Metadata.Rom.SHA384Key);
                            break;
                        default:
                            missingFields.Add(Models.Metadata.Rom.SHA384Key);
                            break;
                    }
                    break;
                case Serialization.Hash.SHA512:
                    switch (datItem)
                    {
                        case Rom rom:
                            if (string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.SHA512Key)))
                                missingFields.Add(Models.Metadata.Rom.SHA512Key);
                            break;
                        default:
                            missingFields.Add(Models.Metadata.Rom.SHA512Key);
                            break;
                    }
                    break;
                case Serialization.Hash.SpamSum:
                    switch (datItem)
                    {
                        case Media medium:
                            if (string.IsNullOrEmpty(medium.GetStringFieldValue(Models.Metadata.Media.SpamSumKey)))
                                missingFields.Add(Models.Metadata.Media.SpamSumKey);
                            break;
                        case Rom rom:
                            if (string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.SpamSumKey)))
                                missingFields.Add(Models.Metadata.Rom.SpamSumKey);
                            break;
                        default:
                            missingFields.Add(Models.Metadata.Rom.SpamSumKey);
                            break;
                    }
                    break;
            }

            return missingFields;
        }

        /// <inheritdoc/>
        public override bool WriteToFile(string outfile, bool ignoreblanks = false, bool throwOnError = false)
        {
            try
            {
                logger.User($"Writing to '{outfile}'...");

                var hashfile = CreateHashFile(ignoreblanks);
                if (!(new Serialization.Files.Hashfile().Serialize(hashfile, outfile, _hash)))
                {
                    logger.Warning($"File '{outfile}' could not be written! See the log for more details.");
                    return false;
                }
            }
            catch (Exception ex) when (!throwOnError)
            {
                logger.Error(ex);
                return false;
            }

            logger.User($"'{outfile}' written!{Environment.NewLine}");
            return true;
        }

        #region Converters

        /// <summary>
        /// Create a Hashfile from the current internal information
        /// <summary>
        /// <param name="ignoreblanks">True if blank roms should be skipped on output, false otherwise</param>
        private Models.Hashfile.Hashfile CreateHashFile(bool ignoreblanks)
        {
            var hashfile = new Models.Hashfile.Hashfile();

            switch (_hash)
            {
                case Serialization.Hash.CRC:
                    hashfile.SFV = CreateSFV(ignoreblanks);
                    break;
                case Serialization.Hash.MD5:
                    hashfile.MD5 = CreateMD5(ignoreblanks);
                    break;
                case Serialization.Hash.SHA1:
                    hashfile.SHA1 = CreateSHA1(ignoreblanks);
                    break;
                case Serialization.Hash.SHA256:
                    hashfile.SHA256 = CreateSHA256(ignoreblanks);
                    break;
                case Serialization.Hash.SHA384:
                    hashfile.SHA384 = CreateSHA384(ignoreblanks);
                    break;
                case Serialization.Hash.SHA512:
                    hashfile.SHA512 = CreateSHA512(ignoreblanks);
                    break;
                case Serialization.Hash.SpamSum:
                    hashfile.SpamSum = CreateSpamSum(ignoreblanks);
                    break;
            }

            return hashfile;
        }

        /// <summary>
        /// Create an array of SFV
        /// <summary>
        /// <param name="ignoreblanks">True if blank roms should be skipped on output, false otherwise</param>
        private Models.Hashfile.SFV[]? CreateSFV(bool ignoreblanks)
        {
            // Create a list of hold the SFVs
            var sfvs = new List<Models.Hashfile.SFV>();

            // Loop through the sorted items and create items for them
            foreach (string key in Items.SortedKeys)
            {
                var items = Items.FilteredItems(key);
                if (items == null || !items.Any())
                    continue;

                // Loop through and convert the items to respective lists
                for (int index = 0; index < items.Count; index++)
                {
                    // Get the item
                    var item = items[index];
                    if (item == null)
                        continue;

                    // Check for a "null" item
                    item = ProcessNullifiedItem(item);

                    // Skip if we're ignoring the item
                    if (ShouldIgnore(item, ignoreblanks))
                        continue;

                    string name = string.Empty;
                    if (Header.GetBoolFieldValue(DatHeader.GameNameKey) == true && item.GetFieldValue<Machine>(DatItem.MachineKey) != null)
                        name = $"{item.GetFieldValue<Machine>(DatItem.MachineKey)!.GetStringFieldValue(Models.Metadata.Machine.NameKey)}{Path.DirectorySeparatorChar}";

                    switch (item)
                    {
                        case Rom rom:
                            sfvs.Add(new Models.Hashfile.SFV
                            {
                                File = name + rom.GetName(),
                                Hash = rom.GetStringFieldValue(Models.Metadata.Rom.CRCKey),
                            });
                            break;
                    }
                }
            }

            return [.. sfvs];
        }

        /// <summary>
        /// Create an array of MD5
        /// <summary>
        /// <param name="ignoreblanks">True if blank roms should be skipped on output, false otherwise</param>
        private Models.Hashfile.MD5[]? CreateMD5(bool ignoreblanks)
        {
            // Create a list of hold the MD5s
            var md5s = new List<Models.Hashfile.MD5>();

            // Loop through the sorted items and create items for them
            foreach (string key in Items.SortedKeys)
            {
                var items = Items.FilteredItems(key);
                if (items == null || !items.Any())
                    continue;

                // Loop through and convert the items to respective lists
                for (int index = 0; index < items.Count; index++)
                {
                    // Get the item
                    var item = items[index];
                    if (item == null)
                        continue;

                    // Check for a "null" item
                    item = ProcessNullifiedItem(item);

                    // Skip if we're ignoring the item
                    if (ShouldIgnore(item, ignoreblanks))
                        continue;

                    string name = string.Empty;
                    if (Header.GetBoolFieldValue(DatHeader.GameNameKey) == true && item.GetFieldValue<Machine>(DatItem.MachineKey) != null)
                        name = $"{item.GetFieldValue<Machine>(DatItem.MachineKey)!.GetStringFieldValue(Models.Metadata.Machine.NameKey)}{Path.DirectorySeparatorChar}";

                    switch (item)
                    {
                        case Disk disk:
                            md5s.Add(new Models.Hashfile.MD5
                            {
                                Hash = disk.GetStringFieldValue(Models.Metadata.Disk.MD5Key),
                                File = name + disk.GetName(),
                            });
                            break;

                        case Media media:
                            md5s.Add(new Models.Hashfile.MD5
                            {
                                Hash = media.GetStringFieldValue(Models.Metadata.Media.MD5Key),
                                File = name + media.GetName(),
                            });
                            break;

                        case Rom rom:
                            md5s.Add(new Models.Hashfile.MD5
                            {
                                Hash = rom.GetStringFieldValue(Models.Metadata.Rom.MD5Key),
                                File = name + rom.GetName(),
                            });
                            break;
                    }
                }
            }

            return [.. md5s];
        }

        /// <summary>
        /// Create an array of SHA1
        /// <summary>
        /// <param name="ignoreblanks">True if blank roms should be skipped on output, false otherwise</param>
        private Models.Hashfile.SHA1[]? CreateSHA1(bool ignoreblanks)
        {
            // Create a list of hold the SHA1s
            var sha1s = new List<Models.Hashfile.SHA1>();

            // Loop through the sorted items and create items for them
            foreach (string key in Items.SortedKeys)
            {
                var items = Items.FilteredItems(key);
                if (items == null || !items.Any())
                    continue;

                // Loop through and convert the items to respective lists
                for (int index = 0; index < items.Count; index++)
                {
                    // Get the item
                    var item = items[index];
                    if (item == null)
                        continue;

                    // Check for a "null" item
                    item = ProcessNullifiedItem(item);

                    // Skip if we're ignoring the item
                    if (ShouldIgnore(item, ignoreblanks))
                        continue;

                    string name = string.Empty;
                    if (Header.GetBoolFieldValue(DatHeader.GameNameKey) == true && item.GetFieldValue<Machine>(DatItem.MachineKey) != null)
                        name = $"{item.GetFieldValue<Machine>(DatItem.MachineKey)!.GetStringFieldValue(Models.Metadata.Machine.NameKey)}{Path.DirectorySeparatorChar}";

                    switch (item)
                    {
                        case Disk disk:
                            sha1s.Add(new Models.Hashfile.SHA1
                            {
                                Hash = disk.GetStringFieldValue(Models.Metadata.Disk.SHA1Key),
                                File = name + disk.GetName(),
                            });
                            break;

                        case Media media:
                            sha1s.Add(new Models.Hashfile.SHA1
                            {
                                Hash = media.GetStringFieldValue(Models.Metadata.Media.SHA1Key),
                                File = name + media.GetName(),
                            });
                            break;

                        case Rom rom:
                            sha1s.Add(new Models.Hashfile.SHA1
                            {
                                Hash = rom.GetStringFieldValue(Models.Metadata.Rom.SHA1Key),
                                File = name + rom.GetName(),
                            });
                            break;
                    }
                }
            }

            return [.. sha1s];
        }

        /// <summary>
        /// Create an array of SHA256
        /// <summary>
        /// <param name="ignoreblanks">True if blank roms should be skipped on output, false otherwise</param>
        private Models.Hashfile.SHA256[]? CreateSHA256(bool ignoreblanks)
        {
            // Create a list of hold the SHA256s
            var sha256s = new List<Models.Hashfile.SHA256>();

            // Loop through the sorted items and create items for them
            foreach (string key in Items.SortedKeys)
            {
                var items = Items.FilteredItems(key);
                if (items == null || !items.Any())
                    continue;

                // Loop through and convert the items to respective lists
                for (int index = 0; index < items.Count; index++)
                {
                    // Get the item
                    var item = items[index];
                    if (item == null)
                        continue;

                    // Check for a "null" item
                    item = ProcessNullifiedItem(item);

                    // Skip if we're ignoring the item
                    if (ShouldIgnore(item, ignoreblanks))
                        continue;

                    string name = string.Empty;
                    if (Header.GetBoolFieldValue(DatHeader.GameNameKey) == true && item.GetFieldValue<Machine>(DatItem.MachineKey) != null)
                        name = $"{item.GetFieldValue<Machine>(DatItem.MachineKey)!.GetStringFieldValue(Models.Metadata.Machine.NameKey)}{Path.DirectorySeparatorChar}";

                    switch (item)
                    {
                        case Media media:
                            sha256s.Add(new Models.Hashfile.SHA256
                            {
                                Hash = media.GetStringFieldValue(Models.Metadata.Media.SHA256Key),
                                File = name + media.GetName(),
                            });
                            break;

                        case Rom rom:
                            sha256s.Add(new Models.Hashfile.SHA256
                            {
                                Hash = rom.GetStringFieldValue(Models.Metadata.Rom.SHA256Key),
                                File = name + rom.GetName(),
                            });
                            break;
                    }
                }
            }

            return [.. sha256s];
        }

        /// <summary>
        /// Create an array of SHA384
        /// <summary>
        /// <param name="ignoreblanks">True if blank roms should be skipped on output, false otherwise</param>
        private Models.Hashfile.SHA384[]? CreateSHA384(bool ignoreblanks)
        {
            // Create a list of hold the SHA384s
            var sha384s = new List<Models.Hashfile.SHA384>();

            // Loop through the sorted items and create items for them
            foreach (string key in Items.SortedKeys)
            {
                var items = Items.FilteredItems(key);
                if (items == null || !items.Any())
                    continue;

                // Loop through and convert the items to respective lists
                for (int index = 0; index < items.Count; index++)
                {
                    // Get the item
                    var item = items[index];
                    if (item == null)
                        continue;

                    // Check for a "null" item
                    item = ProcessNullifiedItem(item);

                    // Skip if we're ignoring the item
                    if (ShouldIgnore(item, ignoreblanks))
                        continue;

                    string name = string.Empty;
                    if (Header.GetBoolFieldValue(DatHeader.GameNameKey) == true && item.GetFieldValue<Machine>(DatItem.MachineKey) != null)
                        name = $"{item.GetFieldValue<Machine>(DatItem.MachineKey)!.GetStringFieldValue(Models.Metadata.Machine.NameKey)}{Path.DirectorySeparatorChar}";

                    switch (item)
                    {
                        case Rom rom:
                            sha384s.Add(new Models.Hashfile.SHA384
                            {
                                Hash = rom.GetStringFieldValue(Models.Metadata.Rom.SHA384Key),
                                File = name + rom.GetName(),
                            });
                            break;
                    }
                }
            }

            return [.. sha384s];
        }

        /// <summary>
        /// Create an array of SHA512
        /// <summary>
        /// <param name="ignoreblanks">True if blank roms should be skipped on output, false otherwise</param>
        private Models.Hashfile.SHA512[]? CreateSHA512(bool ignoreblanks)
        {
            // Create a list of hold the SHA512s
            var sha512s = new List<Models.Hashfile.SHA512>();

            // Loop through the sorted items and create items for them
            foreach (string key in Items.SortedKeys)
            {
                var items = Items.FilteredItems(key);
                if (items == null || !items.Any())
                    continue;

                // Loop through and convert the items to respective lists
                for (int index = 0; index < items.Count; index++)
                {
                    // Get the item
                    var item = items[index];
                    if (item == null)
                        continue;

                    // Check for a "null" item
                    item = ProcessNullifiedItem(item);

                    // Skip if we're ignoring the item
                    if (ShouldIgnore(item, ignoreblanks))
                        continue;

                    string name = string.Empty;
                    if (Header.GetBoolFieldValue(DatHeader.GameNameKey) == true && item.GetFieldValue<Machine>(DatItem.MachineKey) != null)
                        name = $"{item.GetFieldValue<Machine>(DatItem.MachineKey)!.GetStringFieldValue(Models.Metadata.Machine.NameKey)}{Path.DirectorySeparatorChar}";

                    switch (item)
                    {
                        case Rom rom:
                            sha512s.Add(new Models.Hashfile.SHA512
                            {
                                Hash = rom.GetStringFieldValue(Models.Metadata.Rom.SHA512Key),
                                File = name + rom.GetName(),
                            });
                            break;
                    }
                }
            }

            return [.. sha512s];
        }

        /// <summary>
        /// Create an array of SpamSum
        /// <summary>
        /// <param name="ignoreblanks">True if blank roms should be skipped on output, false otherwise</param>
        private Models.Hashfile.SpamSum[]? CreateSpamSum(bool ignoreblanks)
        {
            // Create a list of hold the SpamSums
            var spamsums = new List<Models.Hashfile.SpamSum>();

            // Loop through the sorted items and create items for them
            foreach (string key in Items.SortedKeys)
            {
                var items = Items.FilteredItems(key);
                if (items == null || !items.Any())
                    continue;

                // Loop through and convert the items to respective lists
                for (int index = 0; index < items.Count; index++)
                {
                    // Get the item
                    var item = items[index];
                    if (item == null)
                        continue;

                    // Check for a "null" item
                    item = ProcessNullifiedItem(item);

                    // Skip if we're ignoring the item
                    if (ShouldIgnore(item, ignoreblanks))
                        continue;

                    string name = string.Empty;
                    if (Header.GetBoolFieldValue(DatHeader.GameNameKey) == true && item.GetFieldValue<Machine>(DatItem.MachineKey) != null)
                        name = $"{item.GetFieldValue<Machine>(DatItem.MachineKey)!.GetStringFieldValue(Models.Metadata.Machine.NameKey)}{Path.DirectorySeparatorChar}";

                    switch (item)
                    {
                        case Media media:
                            spamsums.Add(new Models.Hashfile.SpamSum
                            {
                                Hash = media.GetStringFieldValue(Models.Metadata.Media.SpamSumKey),
                                File = name + media.GetName(),
                            });
                            break;

                        case Rom rom:
                            spamsums.Add(new Models.Hashfile.SpamSum
                            {
                                Hash = rom.GetStringFieldValue(Models.Metadata.Rom.SpamSumKey),
                                File = name + rom.GetName(),
                            });
                            break;
                    }
                }
            }

            return [.. spamsums];
        }

        #endregion
    }
}
