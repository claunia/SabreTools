using System;
using System.IO;
using System.Linq;
using System.Text;
using SabreTools.Core;
using SabreTools.IO.Writers;
using SabreTools.Models.Hashfile;

namespace SabreTools.Serialization
{
    /// <summary>
    /// Serializer for hashfile variants
    /// </summary>
    public partial class Hashfile
    {
        /// <summary>
        /// Serializes the defined type to a hashfile variant file
        /// </summary>
        /// <param name="hashfile">Data to serialize</param>
        /// <param name="path">Path to the file to serialize to</param>
        /// <param name="hash">Hash corresponding to the hashfile variant</param>
        /// <returns>True on successful serialization, false otherwise</returns>
        public static bool SerializeToFile(Models.Hashfile.Hashfile? hashfile, string path, Hash hash)
        {
            using var stream = SerializeToStream(hashfile, hash);
            if (stream == null)
                return false;

            using var fs = File.OpenWrite(path);
            stream.CopyTo(fs);
            return true;
        }

        /// <summary>
        /// Serializes the defined type to a stream
        /// </summary>
        /// <param name="hashfile">Data to serialize</param>
        /// <param name="hash">Hash corresponding to the hashfile variant</param>
        /// <returns>Stream containing serialized data on success, null otherwise</returns>
        public static Stream? SerializeToStream(Models.Hashfile.Hashfile? hashfile, Hash hash)
        {
            // If the metadata file is null
            if (hashfile == null)
                return null;

            // Setup the writer and output
            var stream = new MemoryStream();
            var writer = new SeparatedValueWriter(stream, Encoding.UTF8)
            {
                Separator = ' ',
                Quotes = false,
                VerifyFieldCount = false,
            };

            // Write out the items, if they exist
            switch (hash)
            {
                case Hash.CRC:
                    WriteSFV(hashfile.SFV, writer);
                    break;
                case Hash.MD5:
                    WriteMD5(hashfile.MD5, writer);
                    break;
                case Hash.SHA1:
                    WriteSHA1(hashfile.SHA1, writer);
                    break;
                case Hash.SHA256:
                    WriteSHA256(hashfile.SHA256, writer);
                    break;
                case Hash.SHA384:
                    WriteSHA384(hashfile.SHA384, writer);
                    break;
                case Hash.SHA512:
                    WriteSHA512(hashfile.SHA512, writer);
                    break;
                case Hash.SpamSum:
                    WriteSpamSum(hashfile.SpamSum, writer);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(hash));
            }

            // Return the stream
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        /// <summary>
        /// Write SFV information to the current writer
        /// </summary>
        /// <param name="sfvs">Array of SFV objects representing the files</param>
        /// <param name="writer">SeparatedValueWriter representing the output</param>
        private static void WriteSFV(SFV[]? sfvs, SeparatedValueWriter writer)
        {
            // If the item information is missing, we can't do anything
            if (sfvs == null || !sfvs.Any())
                return;

            // Loop through and write out the items
            foreach (var sfv in sfvs)
            {
                if (sfv == null)
                    continue;
                if (string.IsNullOrWhiteSpace(sfv.File) || string.IsNullOrWhiteSpace(sfv.Hash))
                    continue;

                writer.WriteValues(new string[] { sfv.File, sfv.Hash });
                writer.Flush();
            }
        }

        /// <summary>
        /// Write MD5 information to the current writer
        /// </summary>
        /// <param name="md5s">Array of MD5 objects representing the files</param>
        /// <param name="writer">SeparatedValueWriter representing the output</param>
        private static void WriteMD5(MD5[]? md5s, SeparatedValueWriter writer)
        {
            // If the item information is missing, we can't do anything
            if (md5s == null || !md5s.Any())
                return;

            // Loop through and write out the items
            foreach (var md5 in md5s)
            {
                if (md5 == null)
                    continue;
                if (string.IsNullOrWhiteSpace(md5.Hash) || string.IsNullOrWhiteSpace(md5.File))
                    continue;

                writer.WriteValues(new string[] { md5.Hash, md5.File });
                writer.Flush();
            }
        }

        /// <summary>
        /// Write SHA1 information to the current writer
        /// </summary>
        /// <param name="sha1s">Array of SHA1 objects representing the files</param>
        /// <param name="writer">SeparatedValueWriter representing the output</param>
        private static void WriteSHA1(SHA1[]? sha1s, SeparatedValueWriter writer)
        {
            // If the item information is missing, we can't do anything
            if (sha1s == null || !sha1s.Any())
                return;

            // Loop through and write out the items
            foreach (var sha1 in sha1s)
            {
                if (sha1 == null)
                    continue;
                if (string.IsNullOrWhiteSpace(sha1.Hash) || string.IsNullOrWhiteSpace(sha1.File))
                    continue;

                writer.WriteValues(new string[] { sha1.Hash, sha1.File });
                writer.Flush();
            }
        }

        /// <summary>
        /// Write SHA256 information to the current writer
        /// </summary>
        /// <param name="sha256s">Array of SHA256 objects representing the files</param>
        /// <param name="writer">SeparatedValueWriter representing the output</param>
        private static void WriteSHA256(SHA256[]? sha256s, SeparatedValueWriter writer)
        {
            // If the item information is missing, we can't do anything
            if (sha256s == null || !sha256s.Any())
                return;

            // Loop through and write out the items
            foreach (var sha256 in sha256s)
            {
                if (sha256 == null)
                    continue;
                if (string.IsNullOrWhiteSpace(sha256.Hash) || string.IsNullOrWhiteSpace(sha256.File))
                    continue;

                writer.WriteValues(new string[] { sha256.Hash, sha256.File });
                writer.Flush();
            }
        }

        /// <summary>
        /// Write SHA384 information to the current writer
        /// </summary>
        /// <param name="sha384s">Array of SHA384 objects representing the files</param>
        /// <param name="writer">SeparatedValueWriter representing the output</param>
        private static void WriteSHA384(SHA384[]? sha384s, SeparatedValueWriter writer)
        {
            // If the item information is missing, we can't do anything
            if (sha384s == null || !sha384s.Any())
                return;

            // Loop through and write out the items
            foreach (var sha384 in sha384s)
            {
                if (sha384 == null)
                    continue;
                if (string.IsNullOrWhiteSpace(sha384.Hash) || string.IsNullOrWhiteSpace(sha384.File))
                    continue;

                writer.WriteValues(new string[] { sha384.Hash, sha384.File });
                writer.Flush();
            }
        }

        /// <summary>
        /// Write SHA512 information to the current writer
        /// </summary>
        /// <param name="sha512s">Array of SHA512 objects representing the files</param>
        /// <param name="writer">SeparatedValueWriter representing the output</param>
        private static void WriteSHA512(SHA512[]? sha512s, SeparatedValueWriter writer)
        {
            // If the item information is missing, we can't do anything
            if (sha512s == null || !sha512s.Any())
                return;

            // Loop through and write out the items
            foreach (var sha512 in sha512s)
            {
                if (sha512 == null)
                    continue;
                if (string.IsNullOrWhiteSpace(sha512.Hash) || string.IsNullOrWhiteSpace(sha512.File))
                    continue;

                writer.WriteValues(new string[] { sha512.Hash, sha512.File });
                writer.Flush();
            }
        }

        /// <summary>
        /// Write SpamSum information to the current writer
        /// </summary>
        /// <param name="spamsums">Array of SpamSum objects representing the files</param>
        /// <param name="writer">SeparatedValueWriter representing the output</param>
        private static void WriteSpamSum(SpamSum[]? spamsums, SeparatedValueWriter writer)
        {
            // If the item information is missing, we can't do anything
            if (spamsums == null || !spamsums.Any())
                return;

            // Loop through and write out the items
            foreach (var spamsum in spamsums)
            {
                if (spamsum == null)
                    continue;
                if (string.IsNullOrWhiteSpace(spamsum.Hash) || string.IsNullOrWhiteSpace(spamsum.File))
                    continue;

                writer.WriteValues(new string[] { spamsum.Hash, spamsum.File });
                writer.Flush();
            }
        }
        
        #region Internal

        /// <summary>
        /// Convert from <cref="Models.Hashfile.Hashfile"/> to <cref="Models.Metadata.MetadataFile"/>
        /// </summary>
        public static Models.Metadata.MetadataFile? ConvertToInternalModel(Models.Hashfile.Hashfile? item)
        {
            if (item == null)
                return null;
            
            var metadataFile = new Models.Metadata.MetadataFile
            {
                [Models.Metadata.MetadataFile.HeaderKey] = ConvertHeaderToInternalModel(),
            };

            var machine = ConvertMachineToInternalModel(item);
            metadataFile[Models.Metadata.MetadataFile.MachineKey] = new Models.Metadata.Machine[] { machine };

            return metadataFile;
        }

        /// <summary>
        /// Convert from <cref="Models.Hashfile.Hashfile"/> to <cref="Models.Metadata.Header"/>
        /// </summary>
        private static Models.Metadata.Header ConvertHeaderToInternalModel()
        {
            var header = new Models.Metadata.Header
            {
                [Models.Metadata.Header.NameKey] = "Hashfile",
            };
            return header;
        }

        /// <summary>
        /// Convert from <cref="Models.Hashfile.Hashfile"/> to <cref="Models.Metadata.Machine"/>
        /// </summary>
        private static Models.Metadata.Machine ConvertMachineToInternalModel(Models.Hashfile.Hashfile item)
        {
            var machine = new Models.Metadata.Machine();

            if (item.SFV != null && item.SFV.Any())
                machine[Models.Metadata.Machine.RomKey] = item.SFV.Select(ConvertToInternalModel).ToArray();
            else if (item.MD5 != null && item.MD5.Any())
                machine[Models.Metadata.Machine.RomKey] = item.MD5.Select(ConvertToInternalModel).ToArray();
            else if (item.SHA1 != null && item.SHA1.Any())
                machine[Models.Metadata.Machine.RomKey] = item.SHA1.Select(ConvertToInternalModel).ToArray();
            else if (item.SHA256 != null && item.SHA256.Any())
                machine[Models.Metadata.Machine.RomKey] = item.SHA256.Select(ConvertToInternalModel).ToArray();
            else if (item.SHA384 != null && item.SHA384.Any())
                machine[Models.Metadata.Machine.RomKey] = item.SHA384.Select(ConvertToInternalModel).ToArray();
            else if (item.SHA512 != null && item.SHA512.Any())
                machine[Models.Metadata.Machine.RomKey] = item.SHA512.Select(ConvertToInternalModel).ToArray();
            else if (item.SpamSum != null && item.SpamSum.Any())
                machine[Models.Metadata.Machine.RomKey] = item.SpamSum.Select(ConvertToInternalModel).ToArray();

            return machine;
        }

        /// <summary>
        /// Convert from <cref="Models.Hashfile.MD5"/> to <cref="Models.Metadata.Rom"/>
        /// </summary>
        private static Models.Metadata.Rom ConvertToInternalModel(MD5 item)
        {
            var rom = new Models.Metadata.Rom
            {
                [Models.Metadata.Rom.MD5Key] = item.Hash,
                [Models.Metadata.Rom.NameKey] = item.File,
            };
            return rom;
        }

        /// <summary>
        /// Convert from <cref="Models.Hashfile.SFV"/> to <cref="Models.Metadata.Rom"/>
        /// </summary>
        private static Models.Metadata.Rom ConvertToInternalModel(SFV item)
        {
            var rom = new Models.Metadata.Rom
            {
                [Models.Metadata.Rom.NameKey] = item.File,
                [Models.Metadata.Rom.CRCKey] = item.Hash,
            };
            return rom;
        }

        /// <summary>
        /// Convert from <cref="Models.Hashfile.SHA1"/> to <cref="Models.Metadata.Rom"/>
        /// </summary>
        private static Models.Metadata.Rom ConvertToInternalModel(SHA1 item)
        {
            var rom = new Models.Metadata.Rom
            {
                [Models.Metadata.Rom.SHA1Key] = item.Hash,
                [Models.Metadata.Rom.NameKey] = item.File,
            };
            return rom;
        }

        /// <summary>
        /// Convert from <cref="Models.Hashfile.SHA256"/> to <cref="Models.Metadata.Rom"/>
        /// </summary>
        private static Models.Metadata.Rom ConvertToInternalModel(SHA256 item)
        {
            var rom = new Models.Metadata.Rom
            {
                [Models.Metadata.Rom.SHA256Key] = item.Hash,
                [Models.Metadata.Rom.NameKey] = item.File,
            };
            return rom;
        }

        /// <summary>
        /// Convert from <cref="Models.Hashfile.SHA384"/> to <cref="Models.Metadata.Rom"/>
        /// </summary>
        private static Models.Metadata.Rom ConvertToInternalModel(SHA384 item)
        {
            var rom = new Models.Metadata.Rom
            {
                [Models.Metadata.Rom.SHA384Key] = item.Hash,
                [Models.Metadata.Rom.NameKey] = item.File,
            };
            return rom;
        }

        /// <summary>
        /// Convert from <cref="Models.Hashfile.SHA512"/> to <cref="Models.Metadata.Rom"/>
        /// </summary>
        private static Models.Metadata.Rom ConvertToInternalModel(SHA512 item)
        {
            var rom = new Models.Metadata.Rom
            {
                [Models.Metadata.Rom.SHA512Key] = item.Hash,
                [Models.Metadata.Rom.NameKey] = item.File,
            };
            return rom;
        }

        /// <summary>
        /// Convert from <cref="Models.Hashfile.SpamSum"/> to <cref="Models.Metadata.Rom"/>
        /// </summary>
        private static Models.Metadata.Rom ConvertToInternalModel(SpamSum item)
        {
            var rom = new Models.Metadata.Rom
            {
                [Models.Metadata.Rom.SpamSumKey] = item.Hash,
                [Models.Metadata.Rom.NameKey] = item.File,
            };
            return rom;
        }

        #endregion
    }
}