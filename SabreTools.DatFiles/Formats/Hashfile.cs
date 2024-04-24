using System;
using System.Collections.Generic;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;
using SabreTools.Hashing;

namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents a hashfile such as an SFV, MD5, or SHA-1 file
    /// </summary>
    internal abstract class Hashfile : SerializableDatFile<Models.Hashfile.Hashfile, Serialization.Deserializers.Hashfile, Serialization.Serializers.Hashfile, Serialization.CrossModel.Hashfile>
    {
        // Private instance variables specific to Hashfile DATs
        protected HashType _hash;

        /// <summary>
        /// Constructor designed for casting a base DatFile
        /// </summary>
        /// <param name="datFile">Parent DatFile to copy from</param>
        public Hashfile(DatFile? datFile)
            : base(datFile)
        {
        }

        /// <inheritdoc/>
        public override void ParseFile(string filename, int indexId, bool keep, bool statsOnly = false, bool throwOnError = false)
        {
            try
            {
                // Deserialize the input file
                var hashfile = Serialization.Deserializers.Hashfile.DeserializeFile(filename, _hash);
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
                case HashType.CRC32:
                case HashType.CRC32_ISO:
                case HashType.CRC32_Naive:
                case HashType.CRC32_Optimized:
                case HashType.CRC32_Parallel:
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
                case HashType.MD5:
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
                case HashType.SHA1:
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
                case HashType.SHA256:
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
                case HashType.SHA384:
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
                case HashType.SHA512:
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
                case HashType.SpamSum:
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
                if (!(Serialization.Serializers.Hashfile.SerializeFile(hashfile, outfile, _hash)))
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

    /// <summary>
    /// Represents an SFV (CRC-32) hashfile
    /// </summary>
    internal sealed class SfvFile : Hashfile
    {
        /// <summary>
        /// Constructor designed for casting a base DatFile
        /// </summary>
        /// <param name="datFile">Parent DatFile to copy from</param>
        public SfvFile(DatFile? datFile)
            : base(datFile)
        {
            _hash = HashType.CRC32;
        }
    }

    /// <summary>
    /// Represents an MD5 hashfile
    /// </summary>
    internal sealed class Md5File : Hashfile
    {
        /// <summary>
        /// Constructor designed for casting a base DatFile
        /// </summary>
        /// <param name="datFile">Parent DatFile to copy from</param>
        public Md5File(DatFile? datFile)
            : base(datFile)
        {
            _hash = HashType.MD5;
        }
    }

    /// <summary>
    /// Represents an SHA-1 hashfile
    /// </summary>
    internal sealed class Sha1File : Hashfile
    {
        /// <summary>
        /// Constructor designed for casting a base DatFile
        /// </summary>
        /// <param name="datFile">Parent DatFile to copy from</param>
        public Sha1File(DatFile? datFile)
            : base(datFile)
        {
            _hash = HashType.SHA1;
        }
    }

    /// <summary>
    /// Represents an SHA-256 hashfile
    /// </summary>
    internal sealed class Sha256File : Hashfile
    {
        /// <summary>
        /// Constructor designed for casting a base DatFile
        /// </summary>
        /// <param name="datFile">Parent DatFile to copy from</param>
        public Sha256File(DatFile? datFile)
            : base(datFile)
        {
            _hash = HashType.SHA256;
        }
    }

    /// <summary>
    /// Represents an SHA-384 hashfile
    /// </summary>
    internal sealed class Sha384File : Hashfile
    {
        /// <summary>
        /// Constructor designed for casting a base DatFile
        /// </summary>
        /// <param name="datFile">Parent DatFile to copy from</param>
        public Sha384File(DatFile? datFile)
            : base(datFile)
        {
            _hash = HashType.SHA384;
        }
    }

    /// <summary>
    /// Represents an SHA-512 hashfile
    /// </summary>
    internal sealed class Sha512File : Hashfile
    {
        /// <summary>
        /// Constructor designed for casting a base DatFile
        /// </summary>
        /// <param name="datFile">Parent DatFile to copy from</param>
        public Sha512File(DatFile? datFile)
            : base(datFile)
        {
            _hash = HashType.SHA512;
        }
    }

    /// <summary>
    /// Represents an SpamSum hashfile
    /// </summary>
    internal sealed class SpamSumFile : Hashfile
    {
        /// <summary>
        /// Constructor designed for casting a base DatFile
        /// </summary>
        /// <param name="datFile">Parent DatFile to copy from</param>
        public SpamSumFile(DatFile? datFile)
            : base(datFile)
        {
            _hash = HashType.SpamSum;
        }
    }
}
