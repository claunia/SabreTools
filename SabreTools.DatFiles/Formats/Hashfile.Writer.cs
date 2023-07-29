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
            return new ItemType[]
            {
                ItemType.Disk,
                ItemType.Media,
                ItemType.Rom
            };
        }

        /// <inheritdoc/>
        protected override List<DatItemField> GetMissingRequiredFields(DatItem datItem)
        {
            List<DatItemField> missingFields = new();

            // Check item name
            if (string.IsNullOrWhiteSpace(datItem.GetName()))
                missingFields.Add(DatItemField.Name);

            // Check hash linked to specific Hashfile type
            switch (_hash)
            {
                case Hash.CRC:
                    switch (datItem.ItemType)
                    {
                        case ItemType.Rom:
                            if (!string.IsNullOrEmpty((datItem as Rom)?.CRC))
                                missingFields.Add(DatItemField.CRC);
                            break;
                        default:
                            missingFields.Add(DatItemField.CRC);
                            break;
                    }
                    break;
                case Hash.MD5:
                    switch (datItem.ItemType)
                    {
                        case ItemType.Disk:
                            if (!string.IsNullOrEmpty((datItem as Disk)?.MD5))
                                missingFields.Add(DatItemField.MD5);
                            break;
                        case ItemType.Media:
                            if (!string.IsNullOrEmpty((datItem as Media)?.MD5))
                                missingFields.Add(DatItemField.MD5);
                            break;
                        case ItemType.Rom:
                            if (!string.IsNullOrEmpty((datItem as Rom)?.MD5))
                                missingFields.Add(DatItemField.MD5);
                            break;
                        default:
                            missingFields.Add(DatItemField.MD5);
                            break;
                    }
                    break;
                case Hash.SHA1:
                    switch (datItem.ItemType)
                    {
                        case ItemType.Disk:
                            if (!string.IsNullOrEmpty((datItem as Disk)?.SHA1))
                                missingFields.Add(DatItemField.SHA1);
                            break;
                        case ItemType.Media:
                            if (!string.IsNullOrEmpty((datItem as Media)?.SHA1))
                                missingFields.Add(DatItemField.SHA1);
                            break;
                        case ItemType.Rom:
                            if (!string.IsNullOrEmpty((datItem as Rom)?.SHA1))
                                missingFields.Add(DatItemField.SHA1);
                            break;
                        default:
                            missingFields.Add(DatItemField.SHA1);
                            break;
                    }
                    break;
                case Hash.SHA256:
                    switch (datItem.ItemType)
                    {
                        case ItemType.Media:
                            if (!string.IsNullOrEmpty((datItem as Media)?.SHA256))
                                missingFields.Add(DatItemField.SHA256);
                            break;
                        case ItemType.Rom:
                            if (!string.IsNullOrEmpty((datItem as Rom)?.SHA256))
                                missingFields.Add(DatItemField.SHA256);
                            break;
                        default:
                            missingFields.Add(DatItemField.SHA256);
                            break;
                    }
                    break;
                case Hash.SHA384:
                    switch (datItem.ItemType)
                    {
                        case ItemType.Rom:
                            if (!string.IsNullOrEmpty((datItem as Rom)?.SHA384))
                                missingFields.Add(DatItemField.SHA384);
                            break;
                        default:
                            missingFields.Add(DatItemField.SHA384);
                            break;
                    }
                    break;
                case Hash.SHA512:
                    switch (datItem.ItemType)
                    {
                        case ItemType.Rom:
                            if (!string.IsNullOrEmpty((datItem as Rom)?.SHA512))
                                missingFields.Add(DatItemField.SHA512);
                            break;
                        default:
                            missingFields.Add(DatItemField.SHA512);
                            break;
                    }
                    break;
                case Hash.SpamSum:
                    switch (datItem.ItemType)
                    {
                        case ItemType.Media:
                            if (!string.IsNullOrEmpty((datItem as Media)?.SpamSum))
                                missingFields.Add(DatItemField.SpamSum);
                            break;
                        case ItemType.Rom:
                            if (!string.IsNullOrEmpty((datItem as Rom)?.SpamSum))
                                missingFields.Add(DatItemField.SpamSum);
                            break;
                        default:
                            missingFields.Add(DatItemField.SpamSum);
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

                var hashfile = CreateHashFile();
                if (!Serialization.Hashfile.SerializeToFile(hashfile, outfile, _hash))
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

            return true;
        }

        /// <summary>
        /// Create a Hashfile from the current internal information
        /// <summary>
        private Models.Hashfile.Hashfile CreateHashFile()
        {
            var hashfile = new Models.Hashfile.Hashfile();

            switch (_hash)
            {
                case Hash.CRC:
                    hashfile.SFV = CreateSFV();
                    break;
                case Hash.MD5:
                    hashfile.MD5 = CreateMD5();
                    break;
                case Hash.SHA1:
                    hashfile.SHA1 = CreateSHA1();
                    break;
                case Hash.SHA256:
                    hashfile.SHA256 = CreateSHA256();
                    break;
                case Hash.SHA384:
                    hashfile.SHA384 = CreateSHA384();
                    break;
                case Hash.SHA512:
                    hashfile.SHA512 = CreateSHA512();
                    break;
                case Hash.SpamSum:
                    hashfile.SpamSum = CreateSpamSum();
                    break;
            }

            return hashfile;
        }

        /// <summary>
        /// Create an array of SFV
        /// <summary>
        private Models.Hashfile.SFV[]? CreateSFV()
        {
            // Create a list of hold the SFVs
            var sfvs = new List<Models.Hashfile.SFV>();

            // Loop through the sorted items and create items for them
            foreach (string key in Items.SortedKeys)
            {
                var items = Items.FilteredItems(key);
                if (items == null || !items.Any())
                    continue;

                foreach (var item in items)
                {
                    string name = string.Empty;
                    if (Header.GameName)
                        name = $"{item.Machine.Name}{Path.DirectorySeparatorChar}";

                    switch (item)
                    {
                        case Rom rom:
                            sfvs.Add(new Models.Hashfile.SFV
                            {
                                File = name + rom.Name,
                                Hash = rom.CRC,
                            });
                            break;
                    }
                }
            }

            return sfvs.ToArray();
        }

        /// <summary>
        /// Create an array of MD5
        /// <summary>
        private Models.Hashfile.MD5[]? CreateMD5()
        {
            // Create a list of hold the MD5s
            var md5s = new List<Models.Hashfile.MD5>();

            // Loop through the sorted items and create items for them
            foreach (string key in Items.SortedKeys)
            {
                var items = Items.FilteredItems(key);
                if (items == null || !items.Any())
                    continue;

                foreach (var item in items)
                {
                    string name = string.Empty;
                    if (Header.GameName)
                        name = $"{item.Machine.Name}{Path.DirectorySeparatorChar}";

                    switch (item)
                    {
                        case Disk disk:
                            md5s.Add(new Models.Hashfile.MD5
                            {
                                Hash = disk.MD5,
                                File = name + disk.Name,
                            });
                            break;

                        case Media media:
                            md5s.Add(new Models.Hashfile.MD5
                            {
                                Hash = media.MD5,
                                File = name + media.Name,
                            });
                            break;

                        case Rom rom:
                            md5s.Add(new Models.Hashfile.MD5
                            {
                                Hash = rom.MD5,
                                File = name + rom.Name,
                            });
                            break;
                    }
                }
            }

            return md5s.ToArray();
        }

        /// <summary>
        /// Create an array of SHA1
        /// <summary>
        private Models.Hashfile.SHA1[]? CreateSHA1()
        {
            // Create a list of hold the SHA1s
            var sha1s = new List<Models.Hashfile.SHA1>();

            // Loop through the sorted items and create items for them
            foreach (string key in Items.SortedKeys)
            {
                var items = Items.FilteredItems(key);
                if (items == null || !items.Any())
                    continue;

                foreach (var item in items)
                {
                    string name = string.Empty;
                    if (Header.GameName)
                        name = $"{item.Machine.Name}{Path.DirectorySeparatorChar}";

                    switch (item)
                    {
                        case Disk disk:
                            sha1s.Add(new Models.Hashfile.SHA1
                            {
                                Hash = disk.SHA1,
                                File = name + disk.Name,
                            });
                            break;

                        case Media media:
                            sha1s.Add(new Models.Hashfile.SHA1
                            {
                                Hash = media.SHA1,
                                File = name + media.Name,
                            });
                            break;

                        case Rom rom:
                            sha1s.Add(new Models.Hashfile.SHA1
                            {
                                Hash = rom.SHA1,
                                File = name + rom.Name,
                            });
                            break;
                    }
                }
            }

            return sha1s.ToArray();
        }

        /// <summary>
        /// Create an array of SHA256
        /// <summary>
        private Models.Hashfile.SHA256[]? CreateSHA256()
        {
            // Create a list of hold the SHA256s
            var sha256s = new List<Models.Hashfile.SHA256>();

            // Loop through the sorted items and create items for them
            foreach (string key in Items.SortedKeys)
            {
                var items = Items.FilteredItems(key);
                if (items == null || !items.Any())
                    continue;

                foreach (var item in items)
                {
                    string name = string.Empty;
                    switch (item)
                    {
                        case Media media:
                            sha256s.Add(new Models.Hashfile.SHA256
                            {
                                Hash = media.SHA256,
                                File = name + media.Name,
                            });
                            break;

                        case Rom rom:
                            sha256s.Add(new Models.Hashfile.SHA256
                            {
                                Hash = rom.SHA256,
                                File = name + rom.Name,
                            });
                            break;
                    }
                }
            }

            return sha256s.ToArray();
        }

        /// <summary>
        /// Create an array of SHA384
        /// <summary>
        private Models.Hashfile.SHA384[]? CreateSHA384()
        {
            // Create a list of hold the SHA384s
            var sha384s = new List<Models.Hashfile.SHA384>();

            // Loop through the sorted items and create items for them
            foreach (string key in Items.SortedKeys)
            {
                var items = Items.FilteredItems(key);
                if (items == null || !items.Any())
                    continue;

                foreach (var item in items)
                {
                    string name = string.Empty;
                    switch (item)
                    {
                        case Rom rom:
                            sha384s.Add(new Models.Hashfile.SHA384
                            {
                                Hash = rom.SHA384,
                                File = name + rom.Name,
                            });
                            break;
                    }
                }
            }

            return sha384s.ToArray();
        }

        /// <summary>
        /// Create an array of SHA512
        /// <summary>
        private Models.Hashfile.SHA512[]? CreateSHA512()
        {
            // Create a list of hold the SHA512s
            var sha512s = new List<Models.Hashfile.SHA512>();

            // Loop through the sorted items and create items for them
            foreach (string key in Items.SortedKeys)
            {
                var items = Items.FilteredItems(key);
                if (items == null || !items.Any())
                    continue;

                foreach (var item in items)
                {
                    string name = string.Empty;
                    switch (item)
                    {
                        case Rom rom:
                            sha512s.Add(new Models.Hashfile.SHA512
                            {
                                Hash = rom.SHA512,
                                File = name + rom.Name,
                            });
                            break;
                    }
                }
            }

            return sha512s.ToArray();
        }

        /// <summary>
        /// Create an array of SpamSum
        /// <summary>
        private Models.Hashfile.SpamSum[]? CreateSpamSum()
        {
            // Create a list of hold the SpamSums
            var spamsums = new List<Models.Hashfile.SpamSum>();

            // Loop through the sorted items and create items for them
            foreach (string key in Items.SortedKeys)
            {
                var items = Items.FilteredItems(key);
                if (items == null || !items.Any())
                    continue;

                foreach (var item in items)
                {
                    string name = string.Empty;
                    switch (item)
                    {
                        case Media media:
                            spamsums.Add(new Models.Hashfile.SpamSum
                            {
                                Hash = media.SpamSum,
                                File = name + media.Name,
                            });
                            break;

                        case Rom rom:
                            spamsums.Add(new Models.Hashfile.SpamSum
                            {
                                Hash = rom.SpamSum,
                                File = name + rom.Name,
                            });
                            break;
                    }
                }
            }

            return spamsums.ToArray();
        }
    }
}
