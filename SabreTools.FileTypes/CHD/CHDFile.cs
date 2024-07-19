using System;
using System.IO;
using System.Text;
using SabreTools.IO.Extensions;

namespace SabreTools.FileTypes.CHD
{
    /// <summary>
    /// This is code adapted from chd.h and chd.cpp in MAME
    /// Additional archival code from https://github.com/rtissera/libchdr/blob/master/src/chd.h
    /// </summary>
    public abstract class CHDFile : BaseFile
    {
        #region Private instance variables

        // Common header fields
        protected char[] tag = new char[8]; // 'MComprHD'
        protected uint length;              // length of header (including tag and length fields)
        protected uint version;             // drive format version

        #endregion

        #region Constructors

        /// <summary>
        /// Empty constructor
        /// </summary>
        public CHDFile()
        {
        }

        /// <summary>
        /// Create a new CHDFile from an input file
        /// </summary>
        /// <param name="filename">Filename respresenting the CHD file</param>
        public static CHDFile? Create(string filename)
        {
            using FileStream fs = File.OpenRead(filename);
            return Create(fs);
        }

        /// <summary>
        /// Create a new CHDFile from an input stream
        /// </summary>
        /// <param name="chdstream">Stream representing the CHD file</param>
        public static CHDFile? Create(Stream chdstream)
        {
            try
            {
                // Read the standard CHD headers
                (char[] tag, uint length, uint version) = GetHeaderValues(chdstream);
                chdstream.SeekIfPossible(); // Seek back to start

                // Validate that this is actually a valid CHD
                uint validatedVersion = ValidateHeader(tag, length, version);
                if (validatedVersion == 0)
                    return null;

                // Read and return the current CHD
                CHDFile? generated = ReadAsVersion(chdstream, version);
                if (generated != null)
                    generated.Type = FileType.CHD;

                return generated;
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Abstract functionality

        /// <summary>
        /// Return the best-available hash for a particular CHD version
        /// </summary>
        public abstract byte[] GetHash();

        #endregion

        #region Header Parsing

        /// <summary>
        /// Get the generic header values of a CHD, if possible
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private static (char[] tag, uint length, uint version) GetHeaderValues(Stream stream)
        {
            char[] parsedTag = new char[8];
            uint parsedLength = 0;
            uint parsedVersion = 0;

#if NET20 || NET35 || NET40
            using (BinaryReader br = new(stream, Encoding.Default))
#else
            using (BinaryReader br = new(stream, Encoding.Default, true))
#endif
            {
                parsedTag = br.ReadChars(8);
                parsedLength = br.ReadUInt32BigEndian();
                parsedVersion = br.ReadUInt32BigEndian();
            }

            return (parsedTag, parsedLength, parsedVersion);
        }

        /// <summary>
        /// Validate the header values
        /// </summary>
        /// <returns>Matching version, 0 if none</returns>
        private static uint ValidateHeader(char[] tag, uint length, uint version)
        {
            if (!string.Equals(new string(tag), "MComprHD", StringComparison.Ordinal))
                return 0;

            return version switch
            {
                1 => length == CHDFileV1.HeaderSize ? version : 0,
                2 => length == CHDFileV2.HeaderSize ? version : 0,
                3 => length == CHDFileV3.HeaderSize ? version : 0,
                4 => length == CHDFileV4.HeaderSize ? version : 0,
                5 => length == CHDFileV5.HeaderSize ? version : 0,
                _ => 0,
            };
        }

        /// <summary>
        /// Read a stream as a particular CHD version
        /// </summary>
        /// <param name="stream">CHD file as a stream</param>
        /// <param name="version">CHD version to parse</param>
        /// <returns>Populated CHD file, null on failure</returns>
        private static CHDFile? ReadAsVersion(Stream stream, uint version)
        {
            return version switch
            {
                1 => CHDFileV1.Deserialize(stream),
                2 => CHDFileV2.Deserialize(stream),
                3 => CHDFileV3.Deserialize(stream),
                4 => CHDFileV4.Deserialize(stream),
                5 => CHDFileV5.Deserialize(stream),
                _ => null,
            };
        }

        #endregion
    }
}
