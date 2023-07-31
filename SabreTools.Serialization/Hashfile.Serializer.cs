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
                writer.WriteValues(new string[] { spamsum.Hash, spamsum.File });
                writer.Flush();
            }
        }
    }
}