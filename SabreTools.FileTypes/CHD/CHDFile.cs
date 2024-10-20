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

        protected const string Signature = "MComprHD";

        /// <summary>
        /// Model representing the correct CHD header
        /// </summary>
        protected Models.CHD.Header? _header;

        #endregion

        #region Constructors

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
                // Validate that this is actually a valid CHD
                uint version = ValidateHeader(chdstream);
                if (version == 0)
                    return null;

                // Read and return the current CHD
                return ReadAsVersion(chdstream, version);
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
        /// Validate the header values
        /// </summary>
        /// <returns>Matching version, 0 if none</returns>
        private static uint ValidateHeader(Stream stream)
        {
            // Read the header values
            byte[] tagBytes = stream.ReadBytes(8);
            string tag = Encoding.ASCII.GetString(tagBytes);
            uint length = stream.ReadUInt32BigEndian();
            uint version = stream.ReadUInt32BigEndian();

            // Seek back to start
            stream.SeekIfPossible();

            // Check the signature
            if (!string.Equals(tag, Signature, StringComparison.Ordinal))
                return 0;

            // Match the version to header length
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
