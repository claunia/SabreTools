using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SabreTools.Core.Tools
{
    public static class Sanitizer
    {
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
        /// Remove all chars that are considered path unsafe
        /// </summary>
        /// <param name="s">Input string to clean</param>
        /// <returns>Cleaned string</returns>
        public static string RemovePathUnsafeCharacters(string s)
        {
            List<char> invalidPath = Path.GetInvalidPathChars().ToList();
            return new string(s.Where(c => !invalidPath.Contains(c)).ToArray());
        }
    }
}
