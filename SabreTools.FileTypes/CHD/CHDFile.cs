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
            using var fs = File.OpenRead(filename);
            return Create(fs);
        }

        /// <summary>
        /// Create a new CHDFile from an input stream
        /// </summary>
        /// <param name="stream">Stream representing the CHD file</param>
        public static CHDFile? Create(Stream stream)
        {
            try
            {
                // Get the detected CHD version
                uint version = GetVersion(stream);

                // Read and return the current CHD
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
            catch
            {
                return null;
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Get the matching CHD version, if possible
        /// </summary>
        /// <returns>Matching version, 0 if none</returns>
        private static uint GetVersion(Stream stream)
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

        #endregion
    }
}
