using System.Collections.Generic;
using SabreTools.Core.Tools;

namespace SabreTools.DatFiles
{
    public static class Extensions
    {
        #region Private Maps

        /// <summary>
        /// Set of enum to string mappings for MergingFlag
        /// </summary>
        private static readonly Dictionary<string, MergingFlag> _mergingFlagMap = Converters.GenerateToEnum<MergingFlag>();

        /// <summary>
        /// Set of enum to string mappings for NodumpFlag
        /// </summary>
        private static readonly Dictionary<string, NodumpFlag> _nodumpFlagMap = Converters.GenerateToEnum<NodumpFlag>();

        /// <summary>
        /// Set of enum to string mappings for PackingFlag
        /// </summary>
        private static readonly Dictionary<string, PackingFlag> _packingFlagMap = Converters.GenerateToEnum<PackingFlag>();

        #endregion

        #region String to Enum

        /// <summary>
        /// Get the enum value for an input string, if possible
        /// </summary>
        /// <param name="value">String value to parse/param>
        /// <typeparam name="T">Enum type that is expected</typeparam>
        /// <returns>Enum value representing the input, default on error</returns>
        public static MergingFlag AsMergingFlag(this string? value)
        {
            // Normalize the input value
            value = value?.ToLowerInvariant();
            if (value == null)
                return default;

            // Try to get the value from the mappings
            if (_mergingFlagMap.ContainsKey(value))
                return _mergingFlagMap[value];

            // Otherwise, return the default value for the enum
            return default;
        }

        /// <summary>
        /// Get the enum value for an input string, if possible
        /// </summary>
        /// <param name="value">String value to parse/param>
        /// <typeparam name="T">Enum type that is expected</typeparam>
        /// <returns>Enum value representing the input, default on error</returns>
        public static NodumpFlag AsNodumpFlag(this string? value)
        {
            // Normalize the input value
            value = value?.ToLowerInvariant();
            if (value == null)
                return default;

            // Try to get the value from the mappings
            if (_nodumpFlagMap.ContainsKey(value))
                return _nodumpFlagMap[value];

            // Otherwise, return the default value for the enum
            return default;
        }

        /// <summary>
        /// Get the enum value for an input string, if possible
        /// </summary>
        /// <param name="value">String value to parse/param>
        /// <typeparam name="T">Enum type that is expected</typeparam>
        /// <returns>Enum value representing the input, default on error</returns>
        public static PackingFlag AsPackingFlag(this string? value)
        {
            // Normalize the input value
            value = value?.ToLowerInvariant();
            if (value == null)
                return default;

            // Try to get the value from the mappings
            if (_packingFlagMap.ContainsKey(value))
                return _packingFlagMap[value];

            // Otherwise, return the default value for the enum
            return default;
        }

        #endregion
    }
}