using System;
using System.Linq;
using System.Reflection;
using SabreTools.Models;
using SabreTools.Models.Internal;

namespace SabreTools.Filter
{
    public static class FilterParser
    {
        /// <summary>
        /// Parse a filter ID string into the item name and field name, if possible
        /// </summary>
        /// <remarks>TODO: Have validation of fields done automatically</remarks>
        public static (string?, string?) ParseFilterId(string filterId)
        {
            // If we don't have a filter ID, we can't do anything
            if (string.IsNullOrWhiteSpace(filterId))
                return (null, null);

            // If we only have one part, we can't do anything
            string[] splitFilter = filterId.Split('.');
            if (splitFilter.Length != 2)
                return (null, null);

            // Return santized values based on the split ID
            return splitFilter[0].ToLowerInvariant() switch
            {
                // Header
                "header" => ParseHeaderFilterId(splitFilter),

                // Machine
                "game" => ParseMachineFilterId(splitFilter),
                "machine" => ParseMachineFilterId(splitFilter),
                "resource" => ParseMachineFilterId(splitFilter),
                "set" => ParseMachineFilterId(splitFilter),

                // DatItem
                // TODO: Implement parsers for all item types
                _ => (null, null),
            };
        }

        /// <summary>
        /// Parse and validate header fields
        /// </summary>
        private static (string?, string?) ParseHeaderFilterId(string[] filterId)
        {
            // Get the set of constants
            var constants = GetConstants(typeof(Header));
            if (constants == null)
                return (null, null);

            // Get if there's a match to the constant
            string? constantMatch = constants.FirstOrDefault(c => string.Equals(c, filterId[1], StringComparison.OrdinalIgnoreCase));
            if (constantMatch == null)
                return (null, null);

            // Return the sanitized ID
            return (MetadataFile.HeaderKey, constantMatch);
        }

        /// <summary>
        /// Parse and validate machine/game fields
        /// </summary>
        private static (string?, string?) ParseMachineFilterId(string[] filterId)
        {
            // Get the set of constants
            var constants = GetConstants(typeof(Header));
            if (constants == null)
                return (null, null);

            // Get if there's a match to the constant
            string? constantMatch = constants.FirstOrDefault(c => string.Equals(c, filterId[1], StringComparison.OrdinalIgnoreCase));
            if (constantMatch == null)
                return (null, null);

            // Return the sanitized ID
            return (MetadataFile.MachineKey, constantMatch);
        }

        /// <summary>
        /// Get constant values for the given type, if possible
        /// </summary>
        private static string[]? GetConstants(Type? type)
        {
            if (type == null)
                return null;

            var fields = type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            if (fields == null)
                return null;

            return fields
                .Where(f => f.IsLiteral && !f.IsInitOnly)
                .Where(f => f.CustomAttributes.Any(a => a.AttributeType == typeof(NoFilterAttribute)))
                .Select(f => f.GetRawConstantValue() as string)
                .Where(v => v != null)
                .ToArray()!;
        }
    }
}
