using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SabreTools.Core.Tools
{
    /// <summary>
    /// Static utility functions used throughout the library
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// Convert a byte array to a hex string
        /// </summary>
        /// <param name="bytes">Byte array to convert</param>
        /// <returns>Hex string representing the byte array</returns>
        /// <link>http://stackoverflow.com/questions/311165/how-do-you-convert-byte-array-to-hexadecimal-string-and-vice-versa</link>
        public static string ByteArrayToString(byte[] bytes)
        {
            // If we get null in, we send null out
            if (bytes == null)
                return null;

            try
            {
                string hex = BitConverter.ToString(bytes);
                return hex.Replace("-", string.Empty).ToLowerInvariant();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Convert a hex string to a byte array
        /// </summary>
        /// <param name="hex">Hex string to convert</param>
        /// <returns>Byte array represenging the hex string</returns>
        /// <link>http://stackoverflow.com/questions/311165/how-do-you-convert-byte-array-to-hexadecimal-string-and-vice-versa</link>
        public static byte[] StringToByteArray(string hex)
        {
            // If we get null in, we send null out
            if (hex == null)
                return null;

            try
            {
                int NumberChars = hex.Length;
                byte[] bytes = new byte[NumberChars / 2];
                for (int i = 0; i < NumberChars; i += 2)
                {
                    bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
                }

                return bytes;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Get a sanitized double from an input string
        /// </summary>
        /// <param name="input">String to get value from</param>
        /// <returns>Value as a double?, if possible</returns>
        public static double? CleanDouble(string input)
        {
            double? value = null;
            if (input != null)
            {
                if (Double.TryParse(input, out double doubleValue))
                    value = doubleValue;
            }

            return value;
        }

        /// <summary>
        /// Get a sanitized size from an input string
        /// </summary>
        /// <param name="input">String to get value from</param>
        /// <returns>Size as a long?, if possible</returns>
        public static long? CleanLong(string input)
        {
            long? size = null;
            if (input != null && input.Contains("0x"))
                size = Convert.ToInt64(input, 16);

            else if (input != null)
            {
                if (Int64.TryParse(input, out long longSize))
                    size = longSize;
            }

            return size;
        }

        /// <summary>
        /// Convert .NET DateTime to MS-DOS date format
        /// </summary>
        /// <param name="dateTime">.NET DateTime object to convert</param>
        /// <returns>UInt32 representing the MS-DOS date</returns>
        /// <remarks>
        /// Adapted from 7-zip Source Code: CPP/Windows/TimeUtils.cpp:FileTimeToDosTime
        /// </remarks>
        public static uint ConvertDateTimeToMsDosTimeFormat(DateTime dateTime)
        {
            uint year = (uint)((dateTime.Year - 1980) % 128);
            uint mon = (uint)dateTime.Month;
            uint day = (uint)dateTime.Day;
            uint hour = (uint)dateTime.Hour;
            uint min = (uint)dateTime.Minute;
            uint sec = (uint)dateTime.Second;

            return (year << 25) | (mon << 21) | (day << 16) | (hour << 11) | (min << 5) | (sec >> 1);
        }

        /// <summary>
        /// Convert MS-DOS date format to .NET DateTime
        /// </summary>
        /// <param name="msDosDateTime">UInt32 representing the MS-DOS date to convert</param>
        /// <returns>.NET DateTime object representing the converted date</returns>
        /// <remarks>
        /// Adapted from 7-zip Source Code: CPP/Windows/TimeUtils.cpp:DosTimeToFileTime
        /// </remarks>
        public static DateTime ConvertMsDosTimeFormatToDateTime(uint msDosDateTime)
        {
            return new DateTime((int)(1980 + (msDosDateTime >> 25)), (int)((msDosDateTime >> 21) & 0xF), (int)((msDosDateTime >> 16) & 0x1F),
                (int)((msDosDateTime >> 11) & 0x1F), (int)((msDosDateTime >> 5) & 0x3F), (int)((msDosDateTime & 0x1F) * 2));
        }

        /// <summary>
        /// Get a proper romba sub path
        /// </summary>
        /// <param name="hash">SHA-1 hash to get the path for</param>
        /// <param name="depth">Positive value representing the depth of the depot</param>
        /// <returns>Subfolder path for the given hash</returns>
        public static string GetDepotPath(string hash, int depth)
        {
            // If the hash is null or empty, then we return null
            if (string.IsNullOrEmpty(hash))
                return null;

            // If the hash isn't the right size, then we return null
            if (hash.Length != Constants.SHA1Length)
                return null;

            // Cap the depth between 0 and 20, for now
            if (depth < 0)
                depth = 0;
            else if (depth > Constants.SHA1ZeroBytes.Length)
                depth = Constants.SHA1ZeroBytes.Length;

            // Loop through and generate the subdirectory
            string path = string.Empty;
            for (int i = 0; i < depth; i++)
            {
                path += hash.Substring(i * 2, 2) + Path.DirectorySeparatorChar;
            }

            // Now append the filename
            path += $"{hash}.gz";
            return path;
        }

        /// <summary>
        /// Get if the given path has a valid DAT extension
        /// </summary>
        /// <param name="path">Path to check</param>
        /// <returns>True if the extension is valid, false otherwise</returns>
        public static bool HasValidDatExtension(string path)
        {
            // If the path is null or empty, then we return false
            if (string.IsNullOrEmpty(path))
                return false;

            // Get the extension from the path, if possible
            string ext = Path.GetExtension(path).TrimStart('.').ToLowerInvariant();

            // Check against the list of known DAT extensions
            switch (ext)
            {
                case "csv":
                case "dat":
                case "json":
                case "md5":
                case "sfv":
                case "sha1":
                case "sha256":
                case "sha384":
                case "sha512":
                case "spamsum":
                case "ssv":
                case "tsv":
                case "txt":
                case "xml":
                    return true;
                default:
                    return false;
            }
        }

        /// Indicates whether the specified array is null or has a length of zero
        /// </summary>
        /// <param name="array">The array to test</param>
        /// <returns>true if the array parameter is null or has a length of zero; otherwise, false.</returns>
        /// <link>https://stackoverflow.com/questions/8560106/isnullorempty-equivalent-for-array-c-sharp</link>
        public static bool IsNullOrEmpty(this Array array)
        {
            return array == null || array.Length == 0;
        }
    
        /// <summary>
        /// Remove all chars that are considered path unsafe
        /// </summary>
        /// <param name="s">Input string to clean</param>
        /// <returns>Cleaned string</returns>
        public static string RemovePathUnsafeCharacters(string s)
        {
            List<char> invalidPath = Path.GetInvalidPathChars().ToList();
            return new string(s.Where(c => !invalidPath.Contains(c)).ToArray());
        }
    
        /// <summary>
        /// Returns if the first byte array starts with the second array
        /// </summary>
        /// <param name="arr1">First byte array to compare</param>
        /// <param name="arr2">Second byte array to compare</param>
        /// <param name="exact">True if the input arrays should match exactly, false otherwise (default)</param>
        /// <returns>True if the first byte array starts with the second, false otherwise</returns>
        public static bool StartsWith(this byte[] arr1, byte[] arr2, bool exact = false)
        {
            // If we have any invalid inputs, we return false
            if (arr1 == null || arr2 == null
                || arr1.Length == 0 || arr2.Length == 0
                || arr2.Length > arr1.Length
                || (exact && arr1.Length != arr2.Length))
            {
                return false;
            }

            // Otherwise, loop through and see
            for (int i = 0; i < arr2.Length; i++)
            {
                if (arr1[i] != arr2[i])
                    return false;
            }

            return true;
        }
    }
}
