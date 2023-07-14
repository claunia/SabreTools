using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SabreTools.Core;

namespace SabreTools.Serialization
{
    /// <summary>
    /// Serializer for hashfile variants
    /// </summary>
    public class Hashfile
    {
        /// <summary>
        /// Deserializes a hashfile variant to the defined type
        /// </summary>
        /// <param name="path">Path to the file to deserialize</param>
        /// <param name="hash">Hash corresponding to the hashfile variant</param>
        /// <returns>Deserialized data on success, null on failure</returns>
        public static Models.Hashfile.Hashfile? Deserialize(string path, Hash hash)
        {
            try
            {
                using var stream = PathProcessor.OpenStream(path);
                return Deserialize(stream, hash);
            }
            catch
            {
                // TODO: Handle logging the exception
                return default;
            }
        }

        /// <summary>
        /// Deserializes a hashfile variant in a stream to the defined type
        /// </summary>
        /// <param name="stream">Stream to deserialize</param>
        /// <param name="hash">Hash corresponding to the hashfile variant</param>
        /// <returns>Deserialized data on success, null on failure</returns>
        public static Models.Hashfile.Hashfile? Deserialize(Stream? stream, Hash hash)
        {
            try
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
                            var sfv = new Models.Hashfile.SFV
                            {
                                File = string.Join(" ", lineParts[..^1]),
                                Hash = string.Join(" ", lineParts[^1]),
                            };
                            hashes.Add(sfv);
                            break;
                        case Hash.MD5:
                            var md5 = new Models.Hashfile.MD5
                            {
                                Hash = lineParts[0],
                                File = string.Join(" ", lineParts[1..]),
                            };
                            hashes.Add(md5);
                            break;
                        case Hash.SHA1:
                            var sha1 = new Models.Hashfile.SHA1
                            {
                                Hash = lineParts[0],
                                File = string.Join(" ", lineParts[1..]),
                            };
                            hashes.Add(sha1);
                            break;
                        case Hash.SHA256:
                            var sha256 = new Models.Hashfile.SHA256
                            {
                                Hash = lineParts[0],
                                File = string.Join(" ", lineParts[1..]),
                            };
                            hashes.Add(sha256);
                            break;
                        case Hash.SHA384:
                            var sha384 = new Models.Hashfile.SHA384
                            {
                                Hash = lineParts[0],
                                File = string.Join(" ", lineParts[1..]),
                            };
                            hashes.Add(sha384);
                            break;
                        case Hash.SHA512:
                            var sha512 = new Models.Hashfile.SHA512
                            {
                                Hash = lineParts[0],
                                File = string.Join(" ", lineParts[1..]),
                            };
                            hashes.Add(sha512);
                            break;
                        case Hash.SpamSum:
                            var spamSum = new Models.Hashfile.SpamSum
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
                        dat.SFV = hashes.Cast<Models.Hashfile.SFV>().ToArray();
                        break;
                    case Hash.MD5:
                        dat.MD5 = hashes.Cast<Models.Hashfile.MD5>().ToArray();
                        break;
                    case Hash.SHA1:
                        dat.SHA1 = hashes.Cast<Models.Hashfile.SHA1>().ToArray();
                        break;
                    case Hash.SHA256:
                        dat.SHA256 = hashes.Cast<Models.Hashfile.SHA256>().ToArray();
                        break;
                    case Hash.SHA384:
                        dat.SHA384 = hashes.Cast<Models.Hashfile.SHA384>().ToArray();
                        break;
                    case Hash.SHA512:
                        dat.SHA512 = hashes.Cast<Models.Hashfile.SHA512>().ToArray();
                        break;
                    case Hash.SpamSum:
                        dat.SpamSum = hashes.Cast<Models.Hashfile.SpamSum>().ToArray();
                        break;
                }
                dat.ADDITIONAL_ELEMENTS = additional.ToArray();
                return dat;
            }
            catch
            {
                // TODO: Handle logging the exception
                return default;
            }
        }
    }
}