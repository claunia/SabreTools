using System.Collections.Generic;
using System.Linq;
using SabreTools.Core;
using SabreTools.Models.Internal;

namespace SabreTools.Serialization
{
    /// <summary>
    /// Serializer for Hashfile models to internal structure
    /// </summary>
    public partial class Internal
    {
        #region Serialize

        /// <summary>
        /// Convert from <cref="Models.Hashfile.Hashfile"/> to <cref="MetadataFile"/>
        /// </summary>
        public static MetadataFile ConvertFromHashfile(Models.Hashfile.Hashfile item)
        {
            var metadataFile = new MetadataFile
            {
                [MetadataFile.HeaderKey] = ConvertHeaderFromHashfile(item),
            };

            var machine = ConvertMachineFromHashfile(item);
            metadataFile[MetadataFile.MachineKey] = new Machine[] { machine };

            return metadataFile;
        }

        /// <summary>
        /// Convert from <cref="Models.Hashfile.Hashfile"/> to <cref="Header"/>
        /// </summary>
        private static Header ConvertHeaderFromHashfile(Models.Hashfile.Hashfile item)
        {
            var header = new Header
            {
                [Header.NameKey] = "Hashfile",
            };
            return header;
        }

        /// <summary>
        /// Convert from <cref="Models.Hashfile.Hashfile"/> to <cref="Machine"/>
        /// </summary>
        private static Machine ConvertMachineFromHashfile(Models.Hashfile.Hashfile item)
        {
            var machine = new Machine();

            if (item.SFV != null && item.SFV.Any())
            {
                var roms = new List<Rom>();
                foreach (var sfv in item.SFV)
                {
                    roms.Add(ConvertFromSFV(sfv));
                }
                machine[Machine.RomKey] = roms.ToArray();
            }

            else if (item.MD5 != null && item.MD5.Any())
            {
                var roms = new List<Rom>();
                foreach (var md5 in item.MD5)
                {
                    roms.Add(ConvertFromMD5(md5));
                }
                machine[Machine.RomKey] = roms.ToArray();
            }

            else if (item.SHA1 != null && item.SHA1.Any())
            {
                var roms = new List<Rom>();
                foreach (var sha1 in item.SHA1)
                {
                    roms.Add(ConvertFromSHA1(sha1));
                }
                machine[Machine.RomKey] = roms.ToArray();
            }

            else if (item.SHA256 != null && item.SHA256.Any())
            {
                var roms = new List<Rom>();
                foreach (var sha256 in item.SHA256)
                {
                    roms.Add(ConvertFromSHA256(sha256));
                }
                machine[Machine.RomKey] = roms.ToArray();
            }

            else if (item.SHA384 != null && item.SHA384.Any())
            {
                var roms = new List<Rom>();
                foreach (var sha384 in item.SHA384)
                {
                    roms.Add(ConvertFromSHA384(sha384));
                }
                machine[Machine.RomKey] = roms.ToArray();
            }

            else if (item.SHA512 != null && item.SHA512.Any())
            {
                var roms = new List<Rom>();
                foreach (var sha512 in item.SHA512)
                {
                    roms.Add(ConvertFromSHA512(sha512));
                }
                machine[Machine.RomKey] = roms.ToArray();
            }

            else if (item.SpamSum != null && item.SpamSum.Any())
            {
                var roms = new List<Rom>();
                foreach (var spamSum in item.SpamSum)
                {
                    roms.Add(ConvertFromSpamSum(spamSum));
                }
                machine[Machine.RomKey] = roms.ToArray();
            }

            return machine;
        }

        /// <summary>
        /// Convert from <cref="Models.Hashfile.MD5"/> to <cref="Rom"/>
        /// </summary>
        private static Rom ConvertFromMD5(Models.Hashfile.MD5 item)
        {
            var rom = new Rom
            {
                [Rom.MD5Key] = item.Hash,
                [Rom.NameKey] = item.File,
            };
            return rom;
        }

        /// <summary>
        /// Convert from <cref="Models.Hashfile.SFV"/> to <cref="Rom"/>
        /// </summary>
        private static Rom ConvertFromSFV(Models.Hashfile.SFV item)
        {
            var rom = new Rom
            {
                [Rom.NameKey] = item.File,
                [Rom.CRCKey] = item.Hash,
            };
            return rom;
        }

        /// <summary>
        /// Convert from <cref="Models.Hashfile.SHA1"/> to <cref="Rom"/>
        /// </summary>
        private static Rom ConvertFromSHA1(Models.Hashfile.SHA1 item)
        {
            var rom = new Rom
            {
                [Rom.SHA1Key] = item.Hash,
                [Rom.NameKey] = item.File,
            };
            return rom;
        }

        /// <summary>
        /// Convert from <cref="Models.Hashfile.SHA256"/> to <cref="Rom"/>
        /// </summary>
        private static Rom ConvertFromSHA256(Models.Hashfile.SHA256 item)
        {
            var rom = new Rom
            {
                [Rom.SHA256Key] = item.Hash,
                [Rom.NameKey] = item.File,
            };
            return rom;
        }

        /// <summary>
        /// Convert from <cref="Models.Hashfile.SHA384"/> to <cref="Rom"/>
        /// </summary>
        private static Rom ConvertFromSHA384(Models.Hashfile.SHA384 item)
        {
            var rom = new Rom
            {
                [Rom.SHA384Key] = item.Hash,
                [Rom.NameKey] = item.File,
            };
            return rom;
        }

        /// <summary>
        /// Convert from <cref="Models.Hashfile.SHA512"/> to <cref="Rom"/>
        /// </summary>
        private static Rom ConvertFromSHA512(Models.Hashfile.SHA512 item)
        {
            var rom = new Rom
            {
                [Rom.SHA512Key] = item.Hash,
                [Rom.NameKey] = item.File,
            };
            return rom;
        }

        /// <summary>
        /// Convert from <cref="Models.Hashfile.SpamSum"/> to <cref="Rom"/>
        /// </summary>
        private static Rom ConvertFromSpamSum(Models.Hashfile.SpamSum item)
        {
            var rom = new Rom
            {
                [Rom.SpamSumKey] = item.Hash,
                [Rom.NameKey] = item.File,
            };
            return rom;
        }

        #endregion

        #region Deserialize

        /// <summary>
        /// Convert from <cref="Machine"/> to <cref="Models.Hashfile.Hashfile"/>
        /// </summary>
        public static Models.Hashfile.Hashfile? ConvertMachineToHashfile(Machine? item, Hash hash)
        {
            if (item == null)
                return null;

            var roms = item.Read<Rom[]>(Machine.RomKey);
            return new Models.Hashfile.Hashfile
            {
                SFV = hash == Hash.CRC ? roms?.Select(ConvertToSFV)?.ToArray() : null,
                MD5 = hash == Hash.MD5 ? roms?.Select(ConvertToMD5)?.ToArray() : null,
                SHA1 = hash == Hash.SHA1 ? roms?.Select(ConvertToSHA1)?.ToArray() : null,
                SHA256 = hash == Hash.SHA256 ? roms?.Select(ConvertToSHA256)?.ToArray() : null,
                SHA384 = hash == Hash.SHA384 ? roms?.Select(ConvertToSHA384)?.ToArray() : null,
                SHA512 = hash == Hash.SHA512 ? roms?.Select(ConvertToSHA512)?.ToArray() : null,
                SpamSum = hash == Hash.SpamSum ? roms?.Select(ConvertToSpamSum)?.ToArray() : null,
            };
        }

        /// <summary>
        /// Convert from <cref="Rom"/> to <cref="Models.Hashfile.MD5"/>
        /// </summary>
        private static Models.Hashfile.MD5? ConvertToMD5(Rom? item)
        {
            if (item == null)
                return null;

            var md5 = new Models.Hashfile.MD5
            {
                Hash = item.ReadString(Rom.MD5Key),
                File = item.ReadString(Rom.NameKey),
            };
            return md5;
        }

        /// <summary>
        /// Convert from <cref="Rom"/> to <cref="Models.Hashfile.SFV"/>
        /// </summary>
        private static Models.Hashfile.SFV? ConvertToSFV(Rom? item)
        {
            if (item == null)
                return null;

            var sfv = new Models.Hashfile.SFV
            {
                File = item.ReadString(Rom.NameKey),
                Hash = item.ReadString(Rom.CRCKey),
            };
            return sfv;
        }

        /// <summary>
        /// Convert from <cref="Rom"/> to <cref="Models.Hashfile.SHA1"/>
        /// </summary>
        private static Models.Hashfile.SHA1? ConvertToSHA1(Rom? item)
        {
            if (item == null)
                return null;

            var sha1 = new Models.Hashfile.SHA1
            {
                Hash = item.ReadString(Rom.SHA1Key),
                File = item.ReadString(Rom.NameKey),
            };
            return sha1;
        }

        /// <summary>
        /// Convert from <cref="Rom"/> to <cref="Models.Hashfile.SHA256"/>
        /// </summary>
        private static Models.Hashfile.SHA256? ConvertToSHA256(Rom? item)
        {
            if (item == null)
                return null;

            var sha256 = new Models.Hashfile.SHA256
            {
                Hash = item.ReadString(Rom.SHA256Key),
                File = item.ReadString(Rom.NameKey),
            };
            return sha256;
        }

        /// <summary>
        /// Convert from <cref="Rom"/> to <cref="Models.Hashfile.SHA384"/>
        /// </summary>
        private static Models.Hashfile.SHA384? ConvertToSHA384(Rom? item)
        {
            if (item == null)
                return null;

            var sha384 = new Models.Hashfile.SHA384
            {
                Hash = item.ReadString(Rom.SHA384Key),
                File = item.ReadString(Rom.NameKey),
            };
            return sha384;
        }

        /// <summary>
        /// Convert from <cref="Rom"/> to <cref="Models.Hashfile.SHA512"/>
        /// </summary>
        private static Models.Hashfile.SHA512? ConvertToSHA512(Rom? item)
        {
            if (item == null)
                return null;

            var sha512 = new Models.Hashfile.SHA512
            {
                Hash = item.ReadString(Rom.SHA512Key),
                File = item.ReadString(Rom.NameKey),
            };
            return sha512;
        }

        /// <summary>
        /// Convert from <cref="Rom"/> to <cref="Models.Hashfile.SpamSum"/>
        /// </summary>
        private static Models.Hashfile.SpamSum? ConvertToSpamSum(Rom? item)
        {
            if (item == null)
                return null;

            var spamsum = new Models.Hashfile.SpamSum
            {
                Hash = item.ReadString(Rom.SpamSumKey),
                File = item.ReadString(Rom.NameKey),
            };
            return spamsum;
        }

        #endregion
    }
}