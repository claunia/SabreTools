using System;
using System.Linq;
using SabreTools.Core.Tools;
using SabreTools.Models.Metadata;

namespace SabreTools.Core.Filter
{
    public static class FieldManipulator
    {
        /// <summary>
        /// Remove a field from a given DictionaryBase
        /// </summary>
        public static bool RemoveField(DictionaryBase? dictionaryBase, string? fieldName)
        {
            // If the item or field name are missing, we can't do anything
            if (dictionaryBase == null || string.IsNullOrEmpty(fieldName))
                return false;

            // If the key doesn't exist, then it's already removed
            if (!dictionaryBase.ContainsKey(fieldName!))
                return true;

            // Remove the key
            dictionaryBase.Remove(fieldName!);
            return true;
        }

        /// <summary>
        /// Replace a field from one DictionaryBase to another
        /// </summary>
        public static bool ReplaceField(DictionaryBase? from, DictionaryBase? to, string? fieldName)
        {
            // If the items or field name are missing, we can't do anything
            if (from == null || to == null || string.IsNullOrEmpty(fieldName))
                return false;

            // If the types of the items are not the same, we can't do anything
            if (from.GetType() != to.GetType())
                return false;

            // If the key doesn't exist in the source, we can't do anything
            if (!from.ContainsKey(fieldName!))
                return false;

            // Set the key
            to[fieldName!] = from[fieldName!];
            return true;
        }

        /// <summary>
        /// Set a field in a given DictionaryBase
        /// </summary>
        public static bool SetField(DictionaryBase? dictionaryBase, string? fieldName, object value)
        {
            // If the item or field name are missing, we can't do anything
            if (dictionaryBase == null || string.IsNullOrEmpty(fieldName))
                return false;

            // Retrieve the list of valid fields for the item and validate
            var constants = TypeHelper.GetConstants(dictionaryBase.GetType());
            if (constants == null || !constants.Any(c => string.Equals(c, fieldName, StringComparison.OrdinalIgnoreCase)))
                return false;

            // Set the field with the new value
            dictionaryBase[fieldName!] = value;
            return true;
        }
    }
}