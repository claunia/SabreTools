using System;
using System.Linq;
using SabreTools.Models.Metadata;

namespace SabreTools.Core.Filter
{
    public static class FilterParser
    {
        /// <summary>
        /// Parse a filter ID string into the item name and field name, if possible
        /// </summary>
        public static (string?, string?) ParseFilterId(string itemFieldString)
        {
            // If we don't have a filter ID, we can't do anything
            if (string.IsNullOrEmpty(itemFieldString))
                return (null, null);

            // If we only have one part, we can't do anything
            string[] splitFilter = itemFieldString.Split('.');
            if (splitFilter.Length != 2)
                return (null, null);

            return ParseFilterId(splitFilter[0], splitFilter[1]);
        }

        /// <summary>
        /// Parse a filter ID string into the item name and field name, if possible
        /// </summary>
        public static (string?, string?) ParseFilterId(string itemName, string? fieldName)
        {
            // If we don't have a filter ID, we can't do anything
            if (string.IsNullOrEmpty(itemName) || string.IsNullOrEmpty(fieldName))
                return (null, null);

            // Return santized values based on the split ID
            return itemName.ToLowerInvariant() switch
            {
                // Header
                "header" => ParseHeaderFilterId(fieldName!),

                // Machine
                "game" => ParseMachineFilterId(fieldName!),
                "machine" => ParseMachineFilterId(fieldName!),
                "resource" => ParseMachineFilterId(fieldName!),
                "set" => ParseMachineFilterId(fieldName!),

                // DatItem
                "datitem" => ParseDatItemFilterId(fieldName!),
                "item" => ParseDatItemFilterId(fieldName!),
                _ => ParseDatItemFilterId(itemName, fieldName!),
            };
        }

        /// <summary>
        /// Parse and validate header fields
        /// </summary>
        private static (string?, string?) ParseHeaderFilterId(string fieldName)
        {
            // Get the set of constants
            var constants = TypeHelper.GetConstants(typeof(Header));
            if (constants == null)
                return (null, null);

            // Get if there's a match to the constant
            string? constantMatch = constants.FirstOrDefault(c => string.Equals(c, fieldName, StringComparison.OrdinalIgnoreCase));
            if (constantMatch == null)
                return (null, null);

            // Return the sanitized ID
            return (MetadataFile.HeaderKey, constantMatch);
        }

        /// <summary>
        /// Parse and validate machine/game fields
        /// </summary>
        private static (string?, string?) ParseMachineFilterId(string fieldName)
        {
            // Get the set of constants
            var constants = TypeHelper.GetConstants(typeof(Machine));
            if (constants == null)
                return (null, null);

            // Get if there's a match to the constant
            string? constantMatch = constants.FirstOrDefault(c => string.Equals(c, fieldName, StringComparison.OrdinalIgnoreCase));
            if (constantMatch == null)
                return (null, null);

            // Return the sanitized ID
            return (MetadataFile.MachineKey, constantMatch);
        }

        /// <summary>
        /// Parse and validate item fields
        /// </summary>
        private static (string?, string?) ParseDatItemFilterId(string fieldName)
        {
            // Get all item types
            var itemTypes = TypeHelper.GetDatItemTypeNames();

            // If we get any matches
            string? match = itemTypes.FirstOrDefault(t => t != null && ParseDatItemFilterId(t, fieldName) != (null, null));
            if (match != null)
                return ("item", ParseDatItemFilterId(match, fieldName).Item2);

            // Nothing was found
            return (null, null);
        }

        /// <summary>
        /// Parse and validate item fields
        /// </summary>
        private static (string?, string?) ParseDatItemFilterId(string itemName, string fieldName)
        {
            // Get the correct item type
            var itemType = TypeHelper.GetDatItemType(itemName.ToLowerInvariant());
            if (itemType == null)
                return (null, null);

            // Get the set of constants
            var constants = TypeHelper.GetConstants(itemType);
            if (constants == null)
                return (null, null);

            // Get if there's a match to the constant
            string? constantMatch = constants.FirstOrDefault(c => string.Equals(c, fieldName, StringComparison.OrdinalIgnoreCase));
            if (constantMatch == null)
                return (null, null);

            // Return the sanitized ID
            return (itemName.ToLowerInvariant(), constantMatch);
        }
    }
}
