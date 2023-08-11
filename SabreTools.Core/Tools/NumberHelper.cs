using System;

namespace SabreTools.Core.Tools
{
    public static class NumberHelper
    {
        #region Constants

        #region Byte (1000-based) size comparisons

        private const long KiloByte = 1000;
        private readonly static long MegaByte = (long)Math.Pow(KiloByte, 2);
        private readonly static long GigaByte = (long)Math.Pow(KiloByte, 3);
        private readonly static long TeraByte = (long)Math.Pow(KiloByte, 4);
        private readonly static long PetaByte = (long)Math.Pow(KiloByte, 5);
        private readonly static long ExaByte = (long)Math.Pow(KiloByte, 6);
        private readonly static long ZettaByte = (long)Math.Pow(KiloByte, 7);
        private readonly static long YottaByte = (long)Math.Pow(KiloByte, 8);

        #endregion

        #region Byte (1024-based) size comparisons

        private const long KibiByte = 1024;
        private readonly static long MibiByte = (long)Math.Pow(KibiByte, 2);
        private readonly static long GibiByte = (long)Math.Pow(KibiByte, 3);
        private readonly static long TibiByte = (long)Math.Pow(KibiByte, 4);
        private readonly static long PibiByte = (long)Math.Pow(KibiByte, 5);
        private readonly static long ExiByte = (long)Math.Pow(KibiByte, 6);
        private readonly static long ZittiByte = (long)Math.Pow(KibiByte, 7);
        private readonly static long YittiByte = (long)Math.Pow(KibiByte, 8);

        #endregion

        #endregion

        /// <summary>
        /// Convert a string to a Double
        /// </summary>
        public static double? ConvertToDouble(string? numeric)
        {
            // If we don't have a valid string, we can't do anything
            if (string.IsNullOrWhiteSpace(numeric))
                return null;

            if (!double.TryParse(numeric, out double doubleValue))
                return null;

            return doubleValue;
        }

        /// <summary>
        /// Convert a string to an Int64
        /// </summary>
        public static long? ConvertToInt64(string? numeric)
        {
            // If we don't have a valid string, we can't do anything
            if (string.IsNullOrWhiteSpace(numeric))
                return null;

            // Normalize the string for easier comparison
            numeric = numeric.ToLowerInvariant();

            // Get the multiplication modifier and trim characters
            long multiplier = DetermineMultiplier(numeric);
            numeric = numeric.TrimEnd(new char[] { 'k', 'm', 'g', 't', 'p', 'e', 'z', 'y', 'i', 'b', ' ' });

            // Parse the numeric string, if possible
            long value;
            if (numeric.StartsWith("0x"))
                value = Convert.ToInt64(numeric, 16);
            else if (long.TryParse(numeric, out long longValue))
                value = longValue;
            else
                return null;

            // Apply the multiplier and return
            return value * multiplier;
        }

        /// <summary>
        /// Determine the multiplier from a numeric string
        /// </summary>
        private static long DetermineMultiplier(string numeric)
        {
            long multiplier = 1;
            if (numeric.EndsWith("k") || numeric.EndsWith("kb"))
                multiplier = KiloByte;
            else if (numeric.EndsWith("ki") || numeric.EndsWith("kib"))
                multiplier = KibiByte;
            else if (numeric.EndsWith("m") || numeric.EndsWith("mb"))
                multiplier = MegaByte;
            else if (numeric.EndsWith("mi") || numeric.EndsWith("mib"))
                multiplier = MibiByte;
            else if (numeric.EndsWith("g") || numeric.EndsWith("gb"))
                multiplier = GigaByte;
            else if (numeric.EndsWith("gi") || numeric.EndsWith("gib"))
                multiplier = GibiByte;
            else if (numeric.EndsWith("t") || numeric.EndsWith("tb"))
                multiplier = TeraByte;
            else if (numeric.EndsWith("ti") || numeric.EndsWith("tib"))
                multiplier = TibiByte;
            else if (numeric.EndsWith("p") || numeric.EndsWith("pb"))
                multiplier = PetaByte;
            else if (numeric.EndsWith("pi") || numeric.EndsWith("pib"))
                multiplier = PibiByte;
            else if (numeric.EndsWith("e") || numeric.EndsWith("eb"))
                multiplier = ExaByte;
            else if (numeric.EndsWith("ei") || numeric.EndsWith("eib"))
                multiplier = ExiByte;
            else if (numeric.EndsWith("z") || numeric.EndsWith("zb"))
                multiplier = ZettaByte;
            else if (numeric.EndsWith("zi") || numeric.EndsWith("zib"))
                multiplier = ZittiByte;
            else if (numeric.EndsWith("y") || numeric.EndsWith("yb"))
                multiplier = YottaByte;
            else if (numeric.EndsWith("yi") || numeric.EndsWith("yib"))
                multiplier = YittiByte;

            return multiplier;
        }
    }
}