using System;

namespace SabreTools.Library.Tools
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

        /// Indicates whether the specified array is null or has a length of zero
        /// </summary>
        /// <param name="array">The array to test</param>
        /// <returns>true if the array parameter is null or has a length of zero; otherwise, false.</returns>
        /// <link>https://stackoverflow.com/questions/8560106/isnullorempty-equivalent-for-array-c-sharp</link>
        public static bool IsNullOrEmpty(this Array array)
        {
            return (array == null || array.Length == 0);
        }
    }
}
