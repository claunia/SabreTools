using System;
using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core.Tools;

namespace SabreTools.Core
{
    /// <summary>
    /// Represents an item that's backed by a DictionaryBase item
    /// </summary>
    public abstract class ModelBackedItem<T> where T : Models.Metadata.DictionaryBase
    {
        /// <summary>
        /// Internal model wrapped by this DatItem
        /// </summary>
        [JsonIgnore, XmlIgnore]
        protected T _internal;

        #region Constructors

        public ModelBackedItem()
        {
            _internal = (T)Activator.CreateInstance(typeof(T))!;
        }

        #endregion

        #region Accessors

        /// <summary>
        /// Get the value from a field based on the type provided
        /// </summary>
        /// <typeparam name="U">Type of the value to get from the internal model</typeparam>
        /// <param name="fieldName">Field to retrieve</param>
        /// <returns>Value from the field, if possible</returns>
        public U? GetFieldValue<U>(string? fieldName)
        {
            // Invalid field cannot be processed
            if (fieldName == null || !_internal.ContainsKey(fieldName))
                return default;

            // Get the value based on the type
            return _internal.Read<U>(fieldName);
        }

        /// <summary>
        /// Get the value from a field based on the type provided
        /// </summary>
        /// <param name="fieldName">Field to retrieve</param>
        /// <returns>Value from the field, if possible</returns>
        public bool? GetBoolFieldValue(string? fieldName)
        {
            // Invalid field cannot be processed
            if (fieldName == null || !_internal.ContainsKey(fieldName))
                return default;

            // Get the value based on the type
            return _internal.ReadBool(fieldName);
        }

        /// <summary>
        /// Get the value from a field based on the type provided
        /// </summary>
        /// <param name="fieldName">Field to retrieve</param>
        /// <returns>Value from the field, if possible</returns>
        public double? GetDoubleFieldValue(string? fieldName)
        {
            // Invalid field cannot be processed
            if (fieldName == null || !_internal.ContainsKey(fieldName))
                return default;

            // Try to parse directly
            double? doubleValue = _internal.ReadDouble(fieldName);
            if (doubleValue != null)
                return doubleValue;

            // Try to parse from the string
            string? stringValue = _internal.ReadString(fieldName);
            return NumberHelper.ConvertToDouble(stringValue);
        }

        /// <summary>
        /// Get the value from a field based on the type provided
        /// </summary>
        /// <param name="fieldName">Field to retrieve</param>
        /// <returns>Value from the field, if possible</returns>
        public long? GetInt64FieldValue(string? fieldName)
        {
            // Invalid field cannot be processed
            if (fieldName == null || !_internal.ContainsKey(fieldName))
                return default;

            // Try to parse directly
            long? longValue = _internal.ReadLong(fieldName);
            if (longValue != null)
                return longValue;

            // Try to parse from the string
            string? stringValue = _internal.ReadString(fieldName);
            return NumberHelper.ConvertToInt64(stringValue);
        }

        /// <summary>
        /// Get the value from a field based on the type provided
        /// </summary>
        /// <param name="fieldName">Field to retrieve</param>
        /// <returns>Value from the field, if possible</returns>
        public string? GetStringFieldValue(string? fieldName)
        {
            // Invalid field cannot be processed
            if (fieldName == null || !_internal.ContainsKey(fieldName))
                return default;

            // Get the value based on the type
            return _internal.ReadString(fieldName);
        }

        /// <summary>
        /// Get the value from a field based on the type provided
        /// </summary>
        /// <param name="fieldName">Field to retrieve</param>
        /// <returns>Value from the field, if possible</returns>
        public string[]? GetStringArrayFieldValue(string? fieldName)
        {
            // Invalid field cannot be processed
            if (fieldName == null || !_internal.ContainsKey(fieldName))
                return default;

            // Get the value based on the type
            return _internal.ReadStringArray(fieldName);
        }

        /// <summary>
        /// Set the value from a field based on the type provided
        /// </summary>
        /// <typeparam name="U">Type of the value to set in the internal model</typeparam>
        /// <param name="fieldName">Field to set</param>
        /// <param name="value">Value to set</param>
        /// <returns>True if the value was set, false otherwise</returns>
        public bool SetFieldValue<U>(string? fieldName, U? value)
        {
            // Invalid field cannot be processed
            if (fieldName == null)
                return false;

            // Set the value based on the type
            _internal[fieldName] = value;
            return true;
        }

        #endregion
    }
}
