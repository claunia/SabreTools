using System;
using System.IO;
using System.Linq;
using SabreTools.Core;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;

namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents parsing of a hashfile such as an SFV, MD5, or SHA-1 file
    /// </summary>
    internal partial class Hashfile : DatFile
    {
        /// <inheritdoc/>
        public override void ParseFile(string filename, int indexId, bool keep, bool statsOnly = false, bool throwOnError = false)
        {
            try
            {
                // Deserialize the input file
                var hashfile = new Serialization.Files.Hashfile().Deserialize(filename, _hash);

                // Convert items
                switch (_hash)
                {
                    case Serialization.Hash.CRC:
                        ConvertSFV(hashfile?.SFV, filename, indexId, statsOnly);
                        break;
                    case Serialization.Hash.MD5:
                        ConvertMD5(hashfile?.MD5, filename, indexId, statsOnly);
                        break;
                    case Serialization.Hash.SHA1:
                        ConvertSHA1(hashfile?.SHA1, filename, indexId, statsOnly);
                        break;
                    case Serialization.Hash.SHA256:
                        ConvertSHA256(hashfile?.SHA256, filename, indexId, statsOnly);
                        break;
                    case Serialization.Hash.SHA384:
                        ConvertSHA384(hashfile?.SHA384, filename, indexId, statsOnly);
                        break;
                    case Serialization.Hash.SHA512:
                        ConvertSHA512(hashfile?.SHA512, filename, indexId, statsOnly);
                        break;
                    case Serialization.Hash.SpamSum:
                        ConvertSpamSum(hashfile?.SpamSum, filename, indexId, statsOnly);
                        break;
                }
            }
            catch (Exception ex) when (!throwOnError)
            {
                string message = $"'{filename}' - An error occurred during parsing";
                logger.Error(ex, message);
            }
        }

        #region Converters

        /// <summary>
        /// Create a machine from the filename
        /// </summary>
        /// <param name="filename">Filename to derive from</param>
        /// <returns>Filled machine and new filename on success, null on error</returns>
        private static (Machine?, string?) DeriveMachine(string? filename)
        {
            // If the filename is missing, we can't do anything
            if (string.IsNullOrEmpty(filename))
                return (null, null);

            string machineName = Path.GetFileNameWithoutExtension(filename);
            if (filename.Contains('/'))
            {
                string[] split = filename!.Split('/');
                machineName = split[0];
                filename = filename.Substring(machineName.Length + 1);
            }
            else if (filename.Contains('\\'))
            {
                string[] split = filename!.Split('\\');
                machineName = split[0];
                filename = filename.Substring(machineName.Length + 1);
            }

            var machine = new Machine { Name = machineName };
            return (machine, filename);
        }

        /// <summary>
        /// Derive the item type from the filename
        /// </summary>
        /// <param name="filename">Filename to derive from</param>
        /// <returns>ItemType representing the item (Rom by default), ItemType.NULL on error</returns>
        private static ItemType DeriveItemType(string? filename)
        {
            // If the filename is missing, we can't do anything
            if (string.IsNullOrEmpty(filename))
                return ItemType.NULL;

            // If we end in the CHD extension
            if (filename!.EndsWith(".chd", StringComparison.OrdinalIgnoreCase))
                return ItemType.Disk;

            // If we end in an Aaruformat extension
            if (filename.EndsWith(".aaru", StringComparison.OrdinalIgnoreCase)
                || filename.EndsWith(".aaruf", StringComparison.OrdinalIgnoreCase)
                || filename.EndsWith(".aaruformat", StringComparison.OrdinalIgnoreCase)
                || filename.EndsWith(".aif", StringComparison.OrdinalIgnoreCase)
                || filename.EndsWith(".dicf", StringComparison.OrdinalIgnoreCase))
            {
                return ItemType.Media;
            }

            // Everything else is assumed to be a generic item
            return ItemType.Rom;
        }

        /// <summary>
        /// Convert SFV information
        /// </summary>
        /// <param name="sfvs">Array of deserialized models to convert</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ConvertSFV(Models.Hashfile.SFV[]? sfvs, string filename, int indexId, bool statsOnly)
        {
            // If the sfv array is missing, we can't do anything
            if (sfvs == null || !sfvs.Any())
                return;

            // Loop through and add the items
            foreach (var sfv in sfvs)
            {
                // Skip if we have an invalid item
                if (sfv == null)
                    continue;

                // Get the item type
                ItemType itemType = DeriveItemType(sfv.File);
                if (itemType == ItemType.NULL)
                    continue;

                (var machine, string? itemName) = DeriveMachine(sfv.File);
                switch (itemType)
                {
                    case ItemType.Disk: // Should not happen with CRC32 hashes
                    case ItemType.Media: // Should not happen with CRC32 hashes
                    case ItemType.Rom:
                        var rom = new Rom
                        {
                            Size = null,
                            CRC = sfv.Hash,

                            Source = new Source
                            {
                                Index = indexId,
                                Name = filename,
                            },
                        };
                        rom.SetName(itemName);

                        rom.CopyMachineInformation(machine);
                        ParseAddHelper(rom, statsOnly);
                        break;

                    default:
                        continue;
                }
            }
        }

        /// <summary>
        /// Convert MD5 information
        /// </summary>
        /// <param name="md5s">Array of deserialized models to convert</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ConvertMD5(Models.Hashfile.MD5[]? md5s, string filename, int indexId, bool statsOnly)
        {
            // If the md5 array is missing, we can't do anything
            if (md5s == null || !md5s.Any())
                return;

            // Loop through and add the items
            foreach (var md5 in md5s)
            {
                // Skip if we have an invalid item
                if (md5 == null)
                    continue;

                // Get the item type
                ItemType itemType = DeriveItemType(md5.File);
                if (itemType == ItemType.NULL)
                    continue;

                (var machine, string? itemName) = DeriveMachine(md5.File);
                switch (itemType)
                {
                    case ItemType.Disk:
                        var disk = new Disk
                        {
                            MD5 = md5.Hash,

                            Source = new Source
                            {
                                Index = indexId,
                                Name = filename,
                            },
                        };
                        disk.SetName(itemName);

                        disk.CopyMachineInformation(machine);
                        ParseAddHelper(disk, statsOnly);
                        break;

                    case ItemType.Media:
                        var media = new Media
                        {
                            MD5 = md5.Hash,

                            Source = new Source
                            {
                                Index = indexId,
                                Name = filename,
                            },
                        };
                        media.SetName(itemName);

                        media.CopyMachineInformation(machine);
                        ParseAddHelper(media, statsOnly);
                        break;

                    case ItemType.Rom:
                        var rom = new Rom
                        {
                            Size = null,
                            MD5 = md5.Hash,

                            Source = new Source
                            {
                                Index = indexId,
                                Name = filename,
                            },
                        };
                        rom.SetName(itemName);

                        rom.CopyMachineInformation(machine);
                        ParseAddHelper(rom, statsOnly);
                        break;

                    default:
                        continue;
                }
            }
        }

        /// <summary>
        /// Convert SHA1 information
        /// </summary>
        /// <param name="sha1s">Array of deserialized models to convert</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ConvertSHA1(Models.Hashfile.SHA1[]? sha1s, string filename, int indexId, bool statsOnly)
        {
            // If the sha1 array is missing, we can't do anything
            if (sha1s == null || !sha1s.Any())
                return;

            // Loop through and add the items
            foreach (var sha1 in sha1s)
            {
                // Skip if we have an invalid item
                if (sha1 == null)
                    continue;

                // Get the item type
                ItemType itemType = DeriveItemType(sha1.File);
                if (itemType == ItemType.NULL)
                    continue;

                (var machine, string? itemName) = DeriveMachine(sha1.File);
                switch (itemType)
                {
                    case ItemType.Disk:
                        var disk = new Disk
                        {
                            SHA1 = sha1.Hash,

                            Source = new Source
                            {
                                Index = indexId,
                                Name = filename,
                            },
                        };
                        disk.SetName(itemName);

                        disk.CopyMachineInformation(machine);
                        ParseAddHelper(disk, statsOnly);
                        break;

                    case ItemType.Media:
                        var media = new Media
                        {
                            SHA1 = sha1.Hash,

                            Source = new Source
                            {
                                Index = indexId,
                                Name = filename,
                            },
                        };
                        media.SetName(itemName);

                        media.CopyMachineInformation(machine);
                        ParseAddHelper(media, statsOnly);
                        break;

                    case ItemType.Rom:
                        var rom = new Rom
                        {
                            Size = null,
                            SHA1 = sha1.Hash,

                            Source = new Source
                            {
                                Index = indexId,
                                Name = filename,
                            },
                        };
                        rom.SetName(itemName);

                        rom.CopyMachineInformation(machine);
                        ParseAddHelper(rom, statsOnly);
                        break;

                    default:
                        continue;
                }
            }
        }

        /// <summary>
        /// Convert SHA256 information
        /// </summary>
        /// <param name="sha256s">Array of deserialized models to convert</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ConvertSHA256(Models.Hashfile.SHA256[]? sha256s, string filename, int indexId, bool statsOnly)
        {
            // If the sha256 array is missing, we can't do anything
            if (sha256s == null || !sha256s.Any())
                return;

            // Loop through and add the items
            foreach (var sha256 in sha256s)
            {
                // Skip if we have an invalid item
                if (sha256 == null)
                    continue;

                // Get the item type
                ItemType itemType = DeriveItemType(sha256.File);
                if (itemType == ItemType.NULL)
                    continue;

                (var machine, string? itemName) = DeriveMachine(sha256.File);
                switch (itemType)
                {
                    case ItemType.Media:
                        var media = new Media
                        {
                            SHA256 = sha256.Hash,

                            Source = new Source
                            {
                                Index = indexId,
                                Name = filename,
                            },
                        };
                        media.SetName(itemName);

                        media.CopyMachineInformation(machine);
                        ParseAddHelper(media, statsOnly);
                        break;

                    case ItemType.Disk: // Should not happen with SHA-256 hashes
                    case ItemType.Rom:
                        var rom = new Rom
                        {
                            Size = null,
                            SHA256 = sha256.Hash,

                            Source = new Source
                            {
                                Index = indexId,
                                Name = filename,
                            },
                        };
                        rom.SetName(itemName);

                        rom.CopyMachineInformation(machine);
                        ParseAddHelper(rom, statsOnly);
                        break;

                    default:
                        continue;
                }
            }
        }

        /// <summary>
        /// Convert SHA384 information
        /// </summary>
        /// <param name="sha384s">Array of deserialized models to convert</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ConvertSHA384(Models.Hashfile.SHA384[]? sha384s, string filename, int indexId, bool statsOnly)
        {
            // If the sha384 array is missing, we can't do anything
            if (sha384s == null || !sha384s.Any())
                return;

            // Loop through and add the items
            foreach (var sha384 in sha384s)
            {
                // Skip if we have an invalid item
                if (sha384 == null)
                    continue;

                // Get the item type
                ItemType itemType = DeriveItemType(sha384.File);
                if (itemType == ItemType.NULL)
                    continue;

                (var machine, string? itemName) = DeriveMachine(sha384.File);
                switch (itemType)
                {
                    case ItemType.Disk: // Should not happen with SHA-384 hashes
                    case ItemType.Media: // Should not happen with SHA-384 hashes
                    case ItemType.Rom:
                        var rom = new Rom
                        {
                            Size = null,
                            SHA384 = sha384.Hash,

                            Source = new Source
                            {
                                Index = indexId,
                                Name = filename,
                            },
                        };
                        rom.SetName(itemName);

                        rom.CopyMachineInformation(machine);
                        ParseAddHelper(rom, statsOnly);
                        break;

                    default:
                        continue;
                }
            }
        }

        /// <summary>
        /// Convert SHA512 information
        /// </summary>
        /// <param name="sha512s">Array of deserialized models to convert</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ConvertSHA512(Models.Hashfile.SHA512[]? sha512s, string filename, int indexId, bool statsOnly)
        {
            // If the sha512 array is missing, we can't do anything
            if (sha512s == null || !sha512s.Any())
                return;

            // Loop through and add the items
            foreach (var sha512 in sha512s)
            {
                // Skip if we have an invalid item
                if (sha512 == null)
                    continue;

                // Get the item type
                ItemType itemType = DeriveItemType(sha512.File);
                if (itemType == ItemType.NULL)
                    continue;

                (var machine, string? itemName) = DeriveMachine(sha512.File);
                switch (itemType)
                {
                    case ItemType.Disk: // Should not happen with SHA-512 hashes
                    case ItemType.Media: // Should not happen with SHA-512 hashes
                    case ItemType.Rom:
                        var rom = new Rom
                        {
                            Size = null,
                            SHA512 = sha512.Hash,

                            Source = new Source
                            {
                                Index = indexId,
                                Name = filename,
                            },
                        };
                        rom.SetName(itemName);

                        rom.CopyMachineInformation(machine);
                        ParseAddHelper(rom, statsOnly);
                        break;

                    default:
                        continue;
                }
            }
        }

        /// <summary>
        /// Convert SpamSum information
        /// </summary>
        /// <param name="spamsums">Array of deserialized models to convert</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ConvertSpamSum(Models.Hashfile.SpamSum[]? spamsums, string filename, int indexId, bool statsOnly)
        {
            // If the spamsum array is missing, we can't do anything
            if (spamsums == null || !spamsums.Any())
                return;

            // Loop through and add the items
            foreach (var spamsum in spamsums)
            {
                // Skip if we have an invalid item
                if (spamsum == null)
                    continue;

                // Get the item type
                ItemType itemType = DeriveItemType(spamsum.File);
                if (itemType == ItemType.NULL)
                    continue;

                (var machine, string? itemName) = DeriveMachine(spamsum.File);
                switch (itemType)
                {
                    case ItemType.Media:
                        var media = new Media
                        {
                            SpamSum = spamsum.Hash,

                            Source = new Source
                            {
                                Index = indexId,
                                Name = filename,
                            },
                        };
                        media.SetName(itemName);

                        media.CopyMachineInformation(machine);
                        ParseAddHelper(media, statsOnly);
                        break;

                    case ItemType.Disk: // Should not happen with SpamSum fuzzy hashes
                    case ItemType.Rom:
                        var rom = new Rom
                        {
                            Size = null,
                            SpamSum = spamsum.Hash,

                            Source = new Source
                            {
                                Index = indexId,
                                Name = filename,
                            },
                        };
                        rom.SetName(itemName);

                        rom.CopyMachineInformation(machine);
                        ParseAddHelper(rom, statsOnly);
                        break;

                    default:
                        continue;
                }
            }
        }

        #endregion
    }
}
