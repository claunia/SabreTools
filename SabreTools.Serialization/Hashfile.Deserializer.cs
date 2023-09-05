using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SabreTools.Core;
using SabreTools.Models.Hashfile;

namespace SabreTools.Serialization
{
    /// <summary>
    /// Deserializer for hashfile variants
    /// </summary>
    public partial class Hashfile
    {
        /// <summary>
        /// Deserializes a hashfile variant to the defined type
        /// </summary>
        /// <param name="path">Path to the file to deserialize</param>
        /// <param name="hash">Hash corresponding to the hashfile variant</param>
        /// <returns>Deserialized data on success, null on failure</returns>
        public static Models.Hashfile.Hashfile? Deserialize(string path, Hash hash)
        {
            using var stream = PathProcessor.OpenStream(path);
            return Deserialize(stream, hash);
        }

        /// <summary>
        /// Deserializes a hashfile variant in a stream to the defined type
        /// </summary>
        /// <param name="stream">Stream to deserialize</param>
        /// <param name="hash">Hash corresponding to the hashfile variant</param>
        /// <returns>Deserialized data on success, null on failure</returns>
        public static Models.Hashfile.Hashfile? Deserialize(Stream? stream, Hash hash)
        {
            // If the stream is null
            if (stream == null)
                return default;

            // Setup the reader and output
            var reader = new StreamReader(stream);
            var dat = new Models.Hashfile.Hashfile();
            var additional = new List<string>();

            // Loop through the rows and parse out values
            var hashes = new List<object>();
            while (!reader.EndOfStream)
            {
                // Read and split the line
                string? line = reader.ReadLine();
                string[]? lineParts = line?.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if (lineParts == null)
                    continue;

                // Parse the line into a hash
                switch (hash)
                {
                    case Hash.CRC:
                        var sfv = new SFV
                        {
                            File = string.Join(" ", lineParts[..^1]),
                            Hash = string.Join(" ", lineParts[^1]),
                        };
                        hashes.Add(sfv);
                        break;
                    case Hash.MD5:
                        var md5 = new MD5
                        {
                            Hash = lineParts[0],
                            File = string.Join(" ", lineParts[1..]),
                        };
                        hashes.Add(md5);
                        break;
                    case Hash.SHA1:
                        var sha1 = new SHA1
                        {
                            Hash = lineParts[0],
                            File = string.Join(" ", lineParts[1..]),
                        };
                        hashes.Add(sha1);
                        break;
                    case Hash.SHA256:
                        var sha256 = new SHA256
                        {
                            Hash = lineParts[0],
                            File = string.Join(" ", lineParts[1..]),
                        };
                        hashes.Add(sha256);
                        break;
                    case Hash.SHA384:
                        var sha384 = new SHA384
                        {
                            Hash = lineParts[0],
                            File = string.Join(" ", lineParts[1..]),
                        };
                        hashes.Add(sha384);
                        break;
                    case Hash.SHA512:
                        var sha512 = new SHA512
                        {
                            Hash = lineParts[0],
                            File = string.Join(" ", lineParts[1..]),
                        };
                        hashes.Add(sha512);
                        break;
                    case Hash.SpamSum:
                        var spamSum = new SpamSum
                        {
                            Hash = lineParts[0],
                            File = string.Join(" ", lineParts[1..]),
                        };
                        hashes.Add(spamSum);
                        break;
                }
            }

            // Assign the hashes to the hashfile and return
            switch (hash)
            {
                case Hash.CRC:
                    dat.SFV = hashes.Cast<SFV>().ToArray();
                    break;
                case Hash.MD5:
                    dat.MD5 = hashes.Cast<MD5>().ToArray();
                    break;
                case Hash.SHA1:
                    dat.SHA1 = hashes.Cast<SHA1>().ToArray();
                    break;
                case Hash.SHA256:
                    dat.SHA256 = hashes.Cast<SHA256>().ToArray();
                    break;
                case Hash.SHA384:
                    dat.SHA384 = hashes.Cast<SHA384>().ToArray();
                    break;
                case Hash.SHA512:
                    dat.SHA512 = hashes.Cast<SHA512>().ToArray();
                    break;
                case Hash.SpamSum:
                    dat.SpamSum = hashes.Cast<SpamSum>().ToArray();
                    break;
            }
            dat.ADDITIONAL_ELEMENTS = additional.ToArray();
            return dat;
        }

        #region Internal

        /// <summary>
        /// Convert from <cref="Models.Metadata.MetadataFile"/> to an array of <cref="Models.Hashfile.Hashfile"/>
        /// </summary>
        /// <remarks>TODO: Add machine name prefixes to all items</remarks>
        public static Models.Hashfile.Hashfile? ConvertFromInternalModel(Models.Metadata.MetadataFile? item, Hash hash)
        {
            if (item == null)
                return null;

            var machines = item.Read<Models.Metadata.Machine[]>(Models.Metadata.MetadataFile.MachineKey);
            if (machines == null || !machines.Any())
                return null;

            var hashfiles = machines
                .Where(m => m != null)
                .Select(machine => ConvertMachineFromInternalModel(machine, hash));

            var sfvs = new List<SFV>();
            var md5s = new List<MD5>();
            var sha1s = new List<SHA1>();
            var sha256s = new List<SHA256>();
            var sha384s = new List<SHA384>();
            var sha512s = new List<SHA512>();
            var spamsums = new List<SpamSum>();

            foreach (var hashfile in hashfiles)
            {
                if (hashfile.SFV != null && hashfile.SFV.Any())
                    sfvs.AddRange(hashfile.SFV);
                if (hashfile.MD5 != null && hashfile.MD5.Any())
                    md5s.AddRange(hashfile.MD5);
                if (hashfile.SHA1 != null && hashfile.SHA1.Any())
                    sha1s.AddRange(hashfile.SHA1);
                if (hashfile.SHA256 != null && hashfile.SHA256.Any())
                    sha256s.AddRange(hashfile.SHA256);
                if (hashfile.SHA384 != null && hashfile.SHA384.Any())
                    sha384s.AddRange(hashfile.SHA384);
                if (hashfile.SHA512 != null && hashfile.SHA512.Any())
                    sha512s.AddRange(hashfile.SHA512);
                if (hashfile.SpamSum != null && hashfile.SpamSum.Any())
                    spamsums.AddRange(hashfile.SpamSum);
            }

            var hashfileItem = new Models.Hashfile.Hashfile();

            if (sfvs.Any())
                hashfileItem.SFV = sfvs.ToArray();
            if (md5s.Any())
                hashfileItem.MD5 = md5s.ToArray();
            if (sha1s.Any())
                hashfileItem.SHA1 = sha1s.ToArray();
            if (sha256s.Any())
                hashfileItem.SHA256 = sha256s.ToArray();
            if (sha384s.Any())
                hashfileItem.SHA384 = sha384s.ToArray();
            if (sha512s.Any())
                hashfileItem.SHA512 = sha512s.ToArray();
            if (spamsums.Any())
                hashfileItem.SpamSum = spamsums.ToArray();

            return hashfileItem;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.MetadataFile"/> to an array of <cref="Models.Hashfile.Hashfile"/>
        /// </summary>
        public static Models.Hashfile.Hashfile[]? ConvertArrayFromInternalModel(Models.Metadata.MetadataFile? item, Hash hash)
        {
            if (item == null)
                return null;

            var machines = item.Read<Models.Metadata.Machine[]>(Models.Metadata.MetadataFile.MachineKey);
            if (machines != null && machines.Any())
            {
                return machines
                    .Where(m => m != null)
                    .Select(machine => ConvertMachineFromInternalModel(machine, hash))
                    .ToArray();
            }

            return null;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Machine"/> to <cref="Models.Hashfile.Hashfile"/>
        /// </summary>
        private static Models.Hashfile.Hashfile ConvertMachineFromInternalModel(Models.Metadata.Machine item, Hash hash)
        {
            var roms = item.Read<Models.Metadata.Rom[]>(Models.Metadata.Machine.RomKey);
            if (roms == null)
                return new Models.Hashfile.Hashfile();

            return new Models.Hashfile.Hashfile
            {
                SFV = hash == Hash.CRC
                    ? roms
                        .Where(r => r != null)
                        .Select(ConvertToSFV)
                        .ToArray()
                    : null,
                MD5 = hash == Hash.MD5
                    ? roms
                        .Where(r => r != null)
                        .Select(ConvertToMD5)
                        .ToArray()
                    : null,
                SHA1 = hash == Hash.SHA1
                    ? roms
                        .Where(r => r != null)
                        .Select(ConvertToSHA1)
                        .ToArray()
                    : null,
                SHA256 = hash == Hash.SHA256
                    ? roms
                        .Where(r => r != null)
                        .Select(ConvertToSHA256)
                        .ToArray()
                    : null,
                SHA384 = hash == Hash.SHA384
                    ? roms
                        .Where(r => r != null)
                        .Select(ConvertToSHA384)
                        .ToArray()
                    : null,
                SHA512 = hash == Hash.SHA512
                    ? roms
                        .Where(r => r != null)
                        .Select(ConvertToSHA512)
                        .ToArray()
                    : null,
                SpamSum = hash == Hash.SpamSum
                    ? roms
                        .Where(r => r != null)
                        .Select(ConvertToSpamSum)
                        .ToArray()
                    : null,
            };
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Rom"/> to <cref="Models.Hashfile.MD5"/>
        /// </summary>
        private static MD5 ConvertToMD5(Models.Metadata.Rom item)
        {
            var md5 = new MD5
            {
                Hash = item.ReadString(Models.Metadata.Rom.MD5Key),
                File = item.ReadString(Models.Metadata.Rom.NameKey),
            };
            return md5;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Rom"/> to <cref="Models.Hashfile.SFV"/>
        /// </summary>
        private static SFV ConvertToSFV(Models.Metadata.Rom item)
        {
            var sfv = new SFV
            {
                File = item.ReadString(Models.Metadata.Rom.NameKey),
                Hash = item.ReadString(Models.Metadata.Rom.CRCKey),
            };
            return sfv;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Rom"/> to <cref="Models.Hashfile.SHA1"/>
        /// </summary>
        private static SHA1 ConvertToSHA1(Models.Metadata.Rom item)
        {
            var sha1 = new SHA1
            {
                Hash = item.ReadString(Models.Metadata.Rom.SHA1Key),
                File = item.ReadString(Models.Metadata.Rom.NameKey),
            };
            return sha1;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Rom"/> to <cref="Models.Hashfile.SHA256"/>
        /// </summary>
        private static SHA256 ConvertToSHA256(Models.Metadata.Rom item)
        {
            var sha256 = new SHA256
            {
                Hash = item.ReadString(Models.Metadata.Rom.SHA256Key),
                File = item.ReadString(Models.Metadata.Rom.NameKey),
            };
            return sha256;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Rom"/> to <cref="Models.Hashfile.SHA384"/>
        /// </summary>
        private static SHA384 ConvertToSHA384(Models.Metadata.Rom item)
        {
            var sha384 = new SHA384
            {
                Hash = item.ReadString(Models.Metadata.Rom.SHA384Key),
                File = item.ReadString(Models.Metadata.Rom.NameKey),
            };
            return sha384;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Rom"/> to <cref="Models.Hashfile.SHA512"/>
        /// </summary>
        private static SHA512 ConvertToSHA512(Models.Metadata.Rom item)
        {
            var sha512 = new SHA512
            {
                Hash = item.ReadString(Models.Metadata.Rom.SHA512Key),
                File = item.ReadString(Models.Metadata.Rom.NameKey),
            };
            return sha512;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Rom"/> to <cref="Models.Hashfile.SpamSum"/>
        /// </summary>
        private static SpamSum ConvertToSpamSum(Models.Metadata.Rom item)
        {
            var spamsum = new SpamSum
            {
                Hash = item.ReadString(Models.Metadata.Rom.SpamSumKey),
                File = item.ReadString(Models.Metadata.Rom.NameKey),
            };
            return spamsum;
        }

        #endregion
    }
}