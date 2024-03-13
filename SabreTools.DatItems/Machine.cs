using System;
using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.Filter;

namespace SabreTools.DatItems
{
    /// <summary>
    /// Represents the information specific to a set/game/machine
    /// </summary>
    [JsonObject("machine"), XmlRoot("machine")]
    public class Machine : ICloneable
    {
        #region Constants

        /// <summary>
        /// Trurip/EmuArc Machine developer
        /// </summary>
        public const string DeveloperKey = "DEVELOPER";

        /// <summary>
        /// Trurip/EmuArc Game genre
        /// </summary>
        public const string GenreKey = "GENRE";

        /// <summary>
        /// Trurip/EmuArc Title ID
        /// </summary>
        public const string TitleIDKey = "TITLEID";

        #endregion

        #region Fields

        /// <summary>
        /// Internal Machine model
        /// </summary>
        [JsonIgnore]
        private Models.Metadata.Machine _machine = [];

        #endregion

        #region Constructors

        public Machine() { }

        public Machine(Models.Metadata.Machine machine)
        {
            // Get all fields to automatically copy without processing
            var nonItemFields = TypeHelper.GetConstants(typeof(Models.Metadata.Machine));
            if (nonItemFields == null)
                return;

            // Populate the internal machine from non-filter fields
            _machine = [];
            foreach (string fieldName in nonItemFields)
            {
                if (machine.ContainsKey(fieldName))
                    _machine[fieldName] = machine[fieldName];
            }
        }

        #endregion

        #region Accessors

        /// <summary>
        /// Get the value from a field based on the type provided
        /// </summary>
        /// <typeparam name="T">Type of the value to get from the internal model</typeparam>
        /// <param name="fieldName">Field to retrieve</param>
        /// <returns>Value from the field, if possible</returns>
        public T? GetFieldValue<T>(string? fieldName)
        {
            // Invalid field cannot be processed
            if (string.IsNullOrEmpty(fieldName))
                return default;

            // Get the value based on the type
            return _machine.Read<T>(fieldName!);
        }

        /// <summary>
        /// Get the value from a field based on the type provided
        /// </summary>
        /// <param name="fieldName">Field to retrieve</param>
        /// <returns>Value from the field, if possible</returns>
        public bool? GetBoolFieldValue(string? fieldName)
        {
            // Invalid field cannot be processed
            if (string.IsNullOrEmpty(fieldName) || !_machine.ContainsKey(fieldName!))
                return default;

            // Get the value based on the type
            return _machine.ReadBool(fieldName!);
        }

        /// <summary>
        /// Get the value from a field based on the type provided
        /// </summary>
        /// <param name="fieldName">Field to retrieve</param>
        /// <returns>Value from the field, if possible</returns>
        public double? GetDoubleFieldValue(string? fieldName)
        {
            // Invalid field cannot be processed
            if (string.IsNullOrEmpty(fieldName) || !_machine.ContainsKey(fieldName!))
                return default;

            // Try to parse directly
            double? doubleValue = _machine.ReadDouble(fieldName!);
            if (doubleValue != null)
                return doubleValue;

            // Try to parse from the string
            string? stringValue = _machine.ReadString(fieldName!);
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
            if (string.IsNullOrEmpty(fieldName) || !_machine.ContainsKey(fieldName!))
                return default;

            // Try to parse directly
            long? longValue = _machine.ReadLong(fieldName!);
            if (longValue != null)
                return longValue;

            // Try to parse from the string
            string? stringValue = _machine.ReadString(fieldName!);
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
            if (string.IsNullOrEmpty(fieldName) || !_machine.ContainsKey(fieldName!))
                return default;

            // Get the value based on the type
            return _machine.ReadString(fieldName!);
        }

        /// <summary>
        /// Get the value from a field based on the type provided
        /// </summary>
        /// <param name="fieldName">Field to retrieve</param>
        /// <returns>Value from the field, if possible</returns>
        public string[]? GetStringArrayFieldValue(string? fieldName)
        {
            // Invalid field cannot be processed
            if (string.IsNullOrEmpty(fieldName) || !_machine.ContainsKey(fieldName!))
                return default;

            // Get the value based on the type
            return _machine.ReadStringArray(fieldName!);
        }

        /// <summary>
        /// Set the value from a field based on the type provided
        /// </summary>
        /// <typeparam name="T">Type of the value to set in the internal model</typeparam>
        /// <param name="fieldName">Field to set</param>
        /// <param name="value">Value to set</param>
        /// <returns>True if the value was set, false otherwise</returns>
        public bool SetFieldValue<T>(string? fieldName, T? value)
        {
            // Invalid field cannot be processed
            if (string.IsNullOrEmpty(fieldName))
                return false;

            // Set the value based on the type
            _machine[fieldName!] = value;
            return true;
        }

        /// <summary>
        /// Get a clone of the current internal model
        /// </summary>
        public Models.Metadata.Machine GetInternalClone() => (_machine.Clone() as Models.Metadata.Machine)!;

        #endregion

        #region Cloning methods

        /// <summary>
        /// Create a clone of the current machine
        /// </summary>
        /// <returns>New machine with the same values as the current one</returns>
        public object Clone()
        {
            return new Machine()
            {
                _machine = this._machine.Clone() as Models.Metadata.Machine ?? [],
            };
        }

        #endregion

        #region Manipulation

        /// <summary>
        /// Runs a filter and determines if it passes or not
        /// </summary>
        /// <param name="filterRunner">Filter runner to use for checking</param>
        /// <returns>True if the Machine passes the filter, false otherwise</returns>
        public bool PassesFilter(FilterRunner filterRunner) => filterRunner.Run(_machine);

        /// <summary>
        /// Remove a field from the Machine
        /// </summary>
        /// <param name="fieldName">Field to remove</param>
        /// <returns>True if the removal was successful, false otherwise</returns>
        public bool RemoveField(string? fieldName)
            => FieldManipulator.RemoveField(_machine, fieldName);

        /// <summary>
        /// Replace a field from another Machine
        /// </summary>
        /// <param name="other">Machine to replace field from</param>
        /// <param name="fieldName">Field to replace</param>
        /// <returns>True if the replacement was successful, false otherwise</returns>
        public bool ReplaceField(Machine? other, string? fieldName)
            => FieldManipulator.ReplaceField(other?._machine, _machine, fieldName);

        /// <summary>
        /// Set a field in the Machine from a mapping string
        /// </summary>
        /// <param name="fieldName">Field to set</param>
        /// <param name="value">String representing the value to set</param>
        /// <returns>True if the setting was successful, false otherwise</returns>
        /// <remarks>This only performs minimal validation before setting</remarks>
        public bool SetField(string? fieldName, string value)
            => FieldManipulator.SetField(_machine, fieldName, value);

        #endregion
    }
}
