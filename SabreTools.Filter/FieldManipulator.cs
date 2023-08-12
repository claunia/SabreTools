using System;
using System.Linq;
using SabreTools.Models.Internal;

namespace SabreTools.Filter
{
    public static class FieldManipulator
    {
        /// <summary>
        /// Set a field in a given DictionaryBase
        /// </summary>
        public static bool SetField(DictionaryBase? dictionaryBase, string? fieldName, object value)
        {
            if (dictionaryBase == null || fieldName == null)
                return false;

            var constants = TypeHelper.GetConstants(dictionaryBase.GetType());
            if (constants == null || !constants.Any(c => string.Equals(c, fieldName, StringComparison.InvariantCultureIgnoreCase)))
                return false;

            dictionaryBase[fieldName] = value;
            return true;
        }

        /// <summary>
        /// Remove a field from a given DictionaryBase
        /// </summary>
        public static bool RemoveField(DictionaryBase? dictionaryBase, string? fieldName)
        {
            if (dictionaryBase == null || fieldName == null)
                return false;

            if (!dictionaryBase.ContainsKey(fieldName))
                return false;

            dictionaryBase.Remove(fieldName);
            return true;
        }
    }
}