using System;
using System.Linq;
using System.Text.RegularExpressions;
using SabreTools.Models.Internal;

namespace SabreTools.Filter
{
    public static class FieldManipulator
    {
        /// <summary>
        /// Regex pattern to match scene dates
        /// </summary>
        private const string _sceneDateRegex = @"([0-9]{2}\.[0-9]{2}\.[0-9]{2}-)(.*?-.*?)";

        /// <summary>
        /// Replace the machine name with the description
        /// </summary>
        public static (bool Success, string? OriginalName) DescriptionToName(Machine? machine)
        {
            // If the machine is missing, we can't do anything
            if (machine == null)
                return (false, null);

            // Get both the current name and description
            string? name = machine.ReadString(Header.NameKey);
            string? description = machine.ReadString(Header.DescriptionKey);

            // Replace the name with the description
            machine[Header.NameKey] = description;
            return (true, name);
        }

        /// <summary>
        /// Remove a field from a given DictionaryBase
        /// </summary>
        public static bool RemoveField(DictionaryBase? dictionaryBase, string? fieldName)
        {
            // If the item or field name are missing, we can't do anything
            if (dictionaryBase == null || string.IsNullOrWhiteSpace(fieldName))
                return false;

            // If the key doesn't exist, then it's already removed
            if (!dictionaryBase.ContainsKey(fieldName))
                return true;

            // Remove the key
            dictionaryBase.Remove(fieldName);
            return true;
        }

        /// <summary>
        /// Set a field in a given DictionaryBase
        /// </summary>
        public static bool SetField(DictionaryBase? dictionaryBase, string? fieldName, object value)
        {
            // If the item or field name are missing, we can't do anything
            if (dictionaryBase == null || string.IsNullOrWhiteSpace(fieldName))
                return false;

            // Retrieve the list of valid fields for the item and validate
            var constants = TypeHelper.GetConstants(dictionaryBase.GetType());
            if (constants == null || !constants.Any(c => string.Equals(c, fieldName, StringComparison.OrdinalIgnoreCase)))
                return false;

            // Set the field with the new value
            dictionaryBase[fieldName] = value;
            return true;
        }

        /// <summary>
        /// Strip the dates from the beginning of scene-style machine name and description
        /// </summary>
        public static bool StripSceneDates(Machine? machine)
        {
            // If the machine is missing, we can't do anything
            if (machine == null)
                return false;

            // Strip dates from the name field
            string? name = machine.ReadString(Header.NameKey);
            if (name != null && Regex.IsMatch(name, _sceneDateRegex))
                machine[Header.NameKey] = Regex.Replace(name, _sceneDateRegex, @"$2");

            // Strip dates from the description field
            string? description = machine.ReadString(Header.DescriptionKey);
            if (description != null && Regex.IsMatch(description, _sceneDateRegex))
                machine[Header.DescriptionKey] = Regex.Replace(description, _sceneDateRegex, @"$2");

            return true;
        }
    }
}