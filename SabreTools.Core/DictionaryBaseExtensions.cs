using System;
using System.Linq;
using SabreTools.Models.Internal;

namespace SabreTools.Core
{
    public static class DictionaryBaseExtensions
    {
        #region Cloning

        /// <summary>
        /// Deep clone a DictionaryBase object
        /// </summary>
        public static DictionaryBase? Clone(this DictionaryBase dictionaryBase)
        {
            // Create a new object of the same type
            var clone = dictionaryBase
                .GetType()
                .GetConstructor(System.Reflection.BindingFlags.Public, Array.Empty<Type>())?
                .Invoke(null) as DictionaryBase;

            // If construction failed, we can't do anything
            if (clone == null)
                return null;

            // Loop through and clone per type
            foreach (string key in dictionaryBase.Keys)
            {
                object? value = dictionaryBase[key];
                switch (value)
                {
                    // Primative types
                    case bool:
                    case long:
                    case double:
                    case string:
                        clone[key] = value;
                        break;

                    // DictionaryBase types
                    case DictionaryBase db:
                        clone[key] = db.Clone();
                        break;

                    // Enumerable types
                    case byte[] bytArr:
                        clone[key] = bytArr.Clone();
                        break;
                    case string[] strArr:
                        clone[key] = strArr.Clone();
                        break;
                    case DictionaryBase[] enDb:
                        clone[key] = enDb.Select(Clone).ToArray();
                        break;

                    // Everything else just copies
                    default:
                        clone[key] = value;
                        break;
                }
            }

            return clone;
        }

        #endregion

        #region Conversion
        
        /// <summary>
        /// Convert a Disk to a Rom
        /// </summary>
        public static Rom? ConvertToRom(this Disk? disk)
        {
            // If the Disk is missing, we can't do anything
            if (disk == null)
                return null;

            return new Rom
            {
                [Rom.NameKey] = disk.ReadString(Disk.NameKey) + ".chd",
                [Rom.MergeKey] = disk.ReadString(Disk.MergeKey),
                [Rom.RegionKey] = disk.ReadString(Disk.RegionKey),
                [Rom.StatusKey] = disk.ReadString(Disk.StatusKey),
                [Rom.OptionalKey] = disk.ReadString(Disk.OptionalKey),
                [Rom.MD5Key] = disk.ReadString(Disk.MD5Key),
                [Rom.SHA1Key] = disk.ReadString(Disk.SHA1Key),
            };
        }

        /// <summary>
        /// Convert a Media to a Rom
        /// </summary>
        public static Rom? ConvertToRom(this Media? media)
        {
            // If the Media is missing, we can't do anything
            if (media == null)
                return null;

            return new Rom
            {
                [Rom.NameKey] = media.ReadString(Media.NameKey) + ".aaruf",
                [Rom.MD5Key] = media.ReadString(Media.MD5Key),
                [Rom.SHA1Key] = media.ReadString(Media.SHA1Key),
                [Rom.SHA256Key] = media.ReadString(Media.SHA256Key),
                [Rom.SpamSumKey] = media.ReadString(Media.SpamSumKey),
            };
        }

        #endregion

        #region Equality Checking

        /// <summary>
        /// Check equality of two DictionaryBase objects
        /// </summary>
        public static bool EqualTo(this DictionaryBase self, DictionaryBase other)
        {
            // Check types first
            if (self.GetType() != other.GetType())
                return false;

            // Check based on the item type
            return (self, other) switch
            {
                (Disk diskSelf, Disk diskOther) => EqualsImpl(diskSelf, diskOther),
                (Media mediaSelf, Media mediaOther) => EqualsImpl(mediaSelf, mediaOther),
                (Rom romSelf, Rom romOther) => EqualsImpl(romSelf, romOther),
                _ => EqualsImpl(self, other),
            };
        }

        /// <summary>
        /// Check equality of two DictionaryBase objects
        /// </summary>
        private static bool EqualsImpl(this DictionaryBase self, DictionaryBase other)
        {
            // If the number of key-value pairs doesn't match, they can't match
            if (self.Count != other.Count)
                return false;

            // If any keys are missing on either side, they can't match
            if (self.Keys.Except(other.Keys).Any())
                return false;
            if (other.Keys.Except(self.Keys).Any())
                return false;

            // Check all pairs to see if they're equal
            foreach (var kvp in self)
            {
                switch (kvp.Value, other[kvp.Key])
                {
                    case (string selfString, string otherString):
                        if (!string.Equals(selfString, otherString, StringComparison.OrdinalIgnoreCase))
                            return false;
                        break;

                    case (DictionaryBase selfDb, DictionaryBase otherDb):
                        if (!selfDb.Equals(otherDb))
                            return false;
                        break;

                    // TODO: Make this case-insensitive
                    case (string[] selfStrArr, string[] otherStrArr):
                        if (selfStrArr.Length != otherStrArr.Length)
                            return false;
                        if (selfStrArr.Except(otherStrArr).Any())
                            return false;
                        if (otherStrArr.Except(selfStrArr).Any())
                            return false;
                        break;

                    // TODO: Fix the logic here for real equality
                    case (DictionaryBase[] selfDbArr, DictionaryBase[] otherDbArr):
                        if (selfDbArr.Length != otherDbArr.Length)
                            return false;
                        if (selfDbArr.Except(otherDbArr).Any())
                            return false;
                        if (otherDbArr.Except(selfDbArr).Any())
                            return false;
                        break;

                    default:
                        if (kvp.Value != other[kvp.Key])
                            return false;
                        break;
                }
            }

            return true;
        }

        /// <summary>
        /// Check equality of two Disk objects
        /// </summary>
        private static bool EqualsImpl(this Disk self, Disk other)
        {
            string? selfStatus = self.ReadString(Disk.StatusKey);
            string? otherStatus = other.ReadString(Disk.StatusKey);

            string? selfName = self.ReadString(Disk.NameKey);
            string? otherName = other.ReadString(Disk.NameKey);

            // If all hashes are empty but they're both nodump and the names match, then they're dupes
            if (string.Equals(selfStatus, "nodump", StringComparison.OrdinalIgnoreCase)
                && string.Equals(otherStatus, "nodump", StringComparison.OrdinalIgnoreCase)
                && string.Equals(selfName, otherName, StringComparison.OrdinalIgnoreCase)
                && !self.HasHashes()
                && !other.HasHashes())
            {
                return true;
            }

            // If we get a partial match
            if (self.HashMatch(other))
                return true;

            // All other cases fail
            return false;
        }

        /// <summary>
        /// Check equality of two Media objects
        /// </summary>
        private static bool EqualsImpl(this Media self, Media other)
        {
            // If we get a partial match
            if (self.HashMatch(other))
                return true;

            // All other cases fail
            return false;
        }

        /// <summary>
        /// Check equality of two Rom objects
        /// </summary>
        private static bool EqualsImpl(this Rom self, Rom other)
        {
            string? selfStatus = self.ReadString(Rom.StatusKey);
            string? otherStatus = other.ReadString(Rom.StatusKey);

            string? selfName = self.ReadString(Rom.NameKey);
            string? otherName = other.ReadString(Rom.NameKey);

            long? selfSize = self.ReadLong(Rom.SizeKey);
            long? otherSize = other.ReadLong(Rom.SizeKey);

            // If all hashes are empty but they're both nodump and the names match, then they're dupes
            if (string.Equals(selfStatus, "nodump", StringComparison.OrdinalIgnoreCase)
                && string.Equals(otherStatus, "nodump", StringComparison.OrdinalIgnoreCase)
                && string.Equals(selfName, otherName, StringComparison.OrdinalIgnoreCase)
                && !self.HasHashes()
                && !other.HasHashes())
            {
                return true;
            }

            // If we have a file that has no known size, rely on the hashes only
            if (selfSize == null && self.HashMatch(other))

                // If we get a partial match
                if (selfSize == otherSize && self.HashMatch(other))
                    return true;

            // All other cases fail
            return false;
        }

        #endregion

        #region Hash Checking

        /// <summary>
        /// Returns if any hashes are common
        /// </summary>
        public static bool HashMatch(this Disk self, Disk other)
        {
            // If either have no hashes, we return false, otherwise this would be a false positive
            if (!self.HasHashes() || !other.HasHashes())
                return false;

            // If neither have hashes in common, we return false, otherwise this would be a false positive
            if (!self.HasCommonHash(other))
                return false;

            // Return if all hashes match according to merge rules
            return Tools.Utilities.ConditionalHashEquals(self.ReadString(Disk.MD5Key), other.ReadString(Disk.MD5Key))
                && Tools.Utilities.ConditionalHashEquals(self.ReadString(Disk.SHA1Key), other.ReadString(Disk.SHA1Key));
        }

        /// <summary>
        /// Returns if any hashes are common
        /// </summary>
        public static bool HashMatch(this Media self, Media other)
        {
            // If either have no hashes, we return false, otherwise this would be a false positive
            if (!self.HasHashes() || !other.HasHashes())
                return false;

            // If neither have hashes in common, we return false, otherwise this would be a false positive
            if (!self.HasCommonHash(other))
                return false;

            // Return if all hashes match according to merge rules
            return Tools.Utilities.ConditionalHashEquals(self.ReadString(Media.MD5Key), other.ReadString(Media.MD5Key))
                && Tools.Utilities.ConditionalHashEquals(self.ReadString(Media.SHA1Key), other.ReadString(Media.SHA1Key))
                && Tools.Utilities.ConditionalHashEquals(self.ReadString(Media.SHA256Key), other.ReadString(Media.SHA256Key))
                && Tools.Utilities.ConditionalHashEquals(self.ReadString(Media.SpamSumKey), other.ReadString(Media.SpamSumKey));
        }

        /// <summary>
        /// Returns if any hashes are common
        /// </summary>
        public static bool HashMatch(this Rom self, Rom other)
        {
            // If either have no hashes, we return false, otherwise this would be a false positive
            if (!self.HasHashes() || !other.HasHashes())
                return false;

            // If neither have hashes in common, we return false, otherwise this would be a false positive
            if (!self.HasCommonHash(other))
                return false;

            // Return if all hashes match according to merge rules
            return Tools.Utilities.ConditionalHashEquals(self.ReadString(Rom.CRCKey), other.ReadString(Rom.CRCKey))
                && Tools.Utilities.ConditionalHashEquals(self.ReadString(Rom.MD5Key), other.ReadString(Rom.MD5Key))
                && Tools.Utilities.ConditionalHashEquals(self.ReadString(Rom.SHA1Key), other.ReadString(Rom.SHA1Key))
                && Tools.Utilities.ConditionalHashEquals(self.ReadString(Rom.SHA256Key), other.ReadString(Rom.SHA256Key))
                && Tools.Utilities.ConditionalHashEquals(self.ReadString(Rom.SHA384Key), other.ReadString(Rom.SHA384Key))
                && Tools.Utilities.ConditionalHashEquals(self.ReadString(Rom.SHA512Key), other.ReadString(Rom.SHA512Key))
                && Tools.Utilities.ConditionalHashEquals(self.ReadString(Rom.SpamSumKey), other.ReadString(Rom.SpamSumKey));
        }

        /// <summary>
        /// Returns if any hashes exist
        /// </summary>
        public static bool HasHashes(this Disk disk)
        {
            bool md5Null = string.IsNullOrWhiteSpace(disk.ReadString(Disk.MD5Key));
            bool sha1Null = string.IsNullOrWhiteSpace(disk.ReadString(Disk.SHA1Key));

            return !md5Null || !sha1Null;
        }

        /// <summary>
        /// Returns if any hashes exist
        /// </summary>
        public static bool HasHashes(this Media media)
        {
            bool md5Null = string.IsNullOrWhiteSpace(media.ReadString(Media.MD5Key));
            bool sha1Null = string.IsNullOrWhiteSpace(media.ReadString(Media.SHA1Key));
            bool sha256Null = string.IsNullOrWhiteSpace(media.ReadString(Media.SHA256Key));
            bool spamsumNull = string.IsNullOrWhiteSpace(media.ReadString(Media.SpamSumKey));

            return !md5Null || !sha1Null || !sha256Null || !spamsumNull;
        }

        /// <summary>
        /// Returns if any hashes exist
        /// </summary>
        public static bool HasHashes(this Rom rom)
        {
            bool crcNull = string.IsNullOrWhiteSpace(rom.ReadString(Rom.CRCKey));
            bool md5Null = string.IsNullOrWhiteSpace(rom.ReadString(Rom.MD5Key));
            bool sha1Null = string.IsNullOrWhiteSpace(rom.ReadString(Rom.SHA1Key));
            bool sha256Null = string.IsNullOrWhiteSpace(rom.ReadString(Rom.SHA256Key));
            bool sha384Null = string.IsNullOrWhiteSpace(rom.ReadString(Rom.SHA384Key));
            bool sha512Null = string.IsNullOrWhiteSpace(rom.ReadString(Rom.SHA512Key));
            bool spamsumNull = string.IsNullOrWhiteSpace(rom.ReadString(Rom.SpamSumKey));

            return !crcNull || !md5Null || !sha1Null || !sha256Null || !sha384Null || !sha512Null || !spamsumNull;
        }

        /// <summary>
        /// Returns if all of the hashes are set to their 0-byte values or null
        /// </summary>
        public static bool HasZeroHash(this Disk disk)
        {
            string? md5 = disk.ReadString(Disk.MD5Key);
            bool md5Null = string.IsNullOrWhiteSpace(md5) || string.Equals(md5, Constants.MD5Zero, StringComparison.OrdinalIgnoreCase);

            string? sha1 = disk.ReadString(Disk.SHA1Key);
            bool sha1Null = string.IsNullOrWhiteSpace(sha1) || string.Equals(sha1, Constants.SHA1Zero, StringComparison.OrdinalIgnoreCase);

            return md5Null && sha1Null;
        }

        /// <summary>
        /// Returns if all of the hashes are set to their 0-byte values or null
        /// </summary>
        public static bool HasZeroHash(this Media media)
        {
            string? md5 = media.ReadString(Media.MD5Key);
            bool md5Null = string.IsNullOrWhiteSpace(md5) || string.Equals(md5, Constants.MD5Zero, StringComparison.OrdinalIgnoreCase);

            string? sha1 = media.ReadString(Media.SHA1Key);
            bool sha1Null = string.IsNullOrWhiteSpace(sha1) || string.Equals(sha1, Constants.SHA1Zero, StringComparison.OrdinalIgnoreCase);

            string? sha256 = media.ReadString(Media.SHA256Key);
            bool sha256Null = string.IsNullOrWhiteSpace(sha256) || string.Equals(sha256, Constants.SHA256Zero, StringComparison.OrdinalIgnoreCase);

            string? spamsum = media.ReadString(Media.SpamSumKey);
            bool spamsumNull = string.IsNullOrWhiteSpace(spamsum) || string.Equals(spamsum, Constants.SpamSumZero, StringComparison.OrdinalIgnoreCase);

            return md5Null && sha1Null && sha256Null && spamsumNull;
        }

        /// <summary>
        /// Returns if all of the hashes are set to their 0-byte values or null
        /// </summary>
        public static bool HasZeroHash(this Rom rom)
        {
            string? crc = rom.ReadString(Rom.CRCKey);
            bool crcNull = string.IsNullOrWhiteSpace(crc) || string.Equals(crc, Constants.CRCZero, StringComparison.OrdinalIgnoreCase);

            string? md5 = rom.ReadString(Rom.MD5Key);
            bool md5Null = string.IsNullOrWhiteSpace(md5) || string.Equals(md5, Constants.MD5Zero, StringComparison.OrdinalIgnoreCase);

            string? sha1 = rom.ReadString(Rom.SHA1Key);
            bool sha1Null = string.IsNullOrWhiteSpace(sha1) || string.Equals(sha1, Constants.SHA1Zero, StringComparison.OrdinalIgnoreCase);

            string? sha256 = rom.ReadString(Rom.SHA256Key);
            bool sha256Null = string.IsNullOrWhiteSpace(sha256) || string.Equals(sha256, Constants.SHA256Zero, StringComparison.OrdinalIgnoreCase);

            string? sha384 = rom.ReadString(Rom.SHA384Key);
            bool sha384Null = string.IsNullOrWhiteSpace(sha384) || string.Equals(sha384, Constants.SHA384Zero, StringComparison.OrdinalIgnoreCase);

            string? sha512 = rom.ReadString(Rom.SHA512Key);
            bool sha512Null = string.IsNullOrWhiteSpace(sha512) || string.Equals(sha512, Constants.SHA512Zero, StringComparison.OrdinalIgnoreCase);

            string? spamsum = rom.ReadString(Rom.SpamSumKey);
            bool spamsumNull = string.IsNullOrWhiteSpace(spamsum) || string.Equals(spamsum, Constants.SpamSumZero, StringComparison.OrdinalIgnoreCase);

            return crcNull && md5Null && sha1Null && sha256Null && sha384Null && sha512Null && spamsumNull;
        }

        /// <summary>
        /// Returns if there are no, non-empty hashes in common
        /// </summary>
        private static bool HasCommonHash(this Disk self, Disk other)
        {
            bool md5Null = string.IsNullOrWhiteSpace(self.ReadString(Disk.MD5Key));
            md5Null ^= string.IsNullOrWhiteSpace(other.ReadString(Disk.MD5Key));

            bool sha1Null = string.IsNullOrWhiteSpace(self.ReadString(Disk.SHA1Key));
            sha1Null ^= string.IsNullOrWhiteSpace(other.ReadString(Disk.SHA1Key));

            return !md5Null || !sha1Null;
        }

        /// <summary>
        /// Returns if there are no, non-empty hashes in common
        /// </summary>
        private static bool HasCommonHash(this Media self, Media other)
        {
            bool md5Null = string.IsNullOrWhiteSpace(self.ReadString(Media.MD5Key));
            md5Null ^= string.IsNullOrWhiteSpace(other.ReadString(Media.MD5Key));

            bool sha1Null = string.IsNullOrWhiteSpace(self.ReadString(Media.SHA1Key));
            sha1Null ^= string.IsNullOrWhiteSpace(other.ReadString(Media.SHA1Key));

            bool sha256Null = string.IsNullOrWhiteSpace(self.ReadString(Media.SHA256Key));
            sha256Null ^= string.IsNullOrWhiteSpace(other.ReadString(Media.SHA256Key));

            bool spamsumNull = string.IsNullOrWhiteSpace(self.ReadString(Media.SpamSumKey));
            spamsumNull ^= string.IsNullOrWhiteSpace(other.ReadString(Media.SpamSumKey));

            return !md5Null || !sha1Null || !sha256Null || !spamsumNull;
        }

        /// <summary>
        /// Returns if there are no, non-empty hashes in common
        /// </summary>
        private static bool HasCommonHash(this Rom self, Rom other)
        {
            bool crcNull = string.IsNullOrWhiteSpace(self.ReadString(Rom.CRCKey));
            crcNull ^= string.IsNullOrWhiteSpace(other.ReadString(Rom.CRCKey));

            bool md5Null = string.IsNullOrWhiteSpace(self.ReadString(Rom.MD5Key));
            md5Null ^= string.IsNullOrWhiteSpace(other.ReadString(Rom.MD5Key));

            bool sha1Null = string.IsNullOrWhiteSpace(self.ReadString(Rom.SHA1Key));
            sha1Null ^= string.IsNullOrWhiteSpace(other.ReadString(Rom.SHA1Key));

            bool sha256Null = string.IsNullOrWhiteSpace(self.ReadString(Rom.SHA256Key));
            sha256Null ^= string.IsNullOrWhiteSpace(other.ReadString(Rom.SHA256Key));

            bool sha384Null = string.IsNullOrWhiteSpace(self.ReadString(Rom.SHA384Key));
            sha384Null ^= string.IsNullOrWhiteSpace(other.ReadString(Rom.SHA384Key));

            bool sha512Null = string.IsNullOrWhiteSpace(self.ReadString(Rom.SHA512Key));
            sha512Null ^= string.IsNullOrWhiteSpace(other.ReadString(Rom.SHA512Key));

            bool spamsumNull = string.IsNullOrWhiteSpace(self.ReadString(Rom.SpamSumKey));
            spamsumNull ^= string.IsNullOrWhiteSpace(other.ReadString(Rom.SpamSumKey));

            return !crcNull || !md5Null || !sha1Null || !sha256Null || !sha384Null || !sha512Null || !spamsumNull;
        }

        #endregion
    }
}