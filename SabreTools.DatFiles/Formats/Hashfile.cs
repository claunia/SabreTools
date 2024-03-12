using System;
using System.Collections.Generic;
using SabreTools.Core;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;
using SabreTools.Hashing;

namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents a hashfile such as an SFV, MD5, or SHA-1 file
    /// </summary>
    internal sealed class Hashfile : SerializableDatFile<Models.Hashfile.Hashfile, Serialization.Files.Hashfile, Serialization.CrossModel.Hashfile>
    {
        // Private instance variables specific to Hashfile DATs
        private readonly Serialization.Hash _hash;

        /// <summary>
        /// Constructor designed for casting a base DatFile
        /// </summary>
        /// <param name="datFile">Parent DatFile to copy from</param>
        /// <param name="hash">Type of hash that is associated with this DAT</param> 
        public Hashfile(DatFile? datFile, HashType hash)
            : base(datFile)
        {
            _hash = ConvertHash(hash);
        }

        /// <summary>
        /// Convert hash types between internal and Serialization
        /// </summary>
        private static Serialization.Hash ConvertHash(HashType hash)
        {
            return hash switch
            {
                HashType.CRC32 => Serialization.Hash.CRC,
                HashType.MD5 => Serialization.Hash.MD5,
                HashType.SHA1 => Serialization.Hash.SHA1,
                HashType.SHA256 => Serialization.Hash.SHA256,
                HashType.SHA384 => Serialization.Hash.SHA384,
                HashType.SHA512 => Serialization.Hash.SHA512,
                HashType.SpamSum => Serialization.Hash.SpamSum,
                _ => throw new System.ArgumentOutOfRangeException(nameof(hash)),
            };
        }

        /// <inheritdoc/>
        public override void ParseFile(string filename, int indexId, bool keep, bool statsOnly = false, bool throwOnError = false)
        {
            try
            {
                // Deserialize the input file
                var hashfile = new Serialization.Files.Hashfile().Deserialize(filename, _hash);
                var metadata = new Serialization.CrossModel.Hashfile().Serialize(hashfile);

                // Convert to the internal format
                ConvertMetadata(metadata, filename, indexId, keep, statsOnly);
            }
            catch (Exception ex) when (!throwOnError)
            {
                string message = $"'{filename}' - An error occurred during parsing";
                logger.Error(ex, message);
            }
        }

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

                // Serialize the input file
                var metadata = ConvertMetadata(ignoreblanks);
                var hashfile = new Serialization.CrossModel.Hashfile().Deserialize(metadata, _hash);
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
    }
}
