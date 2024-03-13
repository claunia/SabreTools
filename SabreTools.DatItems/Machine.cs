using System;
using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;
using SabreTools.Filter;

namespace SabreTools.DatItems
{
    /// <summary>
    /// Represents the information specific to a set/game/machine
    /// </summary>
    [JsonObject("machine"), XmlRoot("machine")]
    public sealed class Machine : ModelBackedItem<Models.Metadata.Machine>, ICloneable
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

        #region Constructors

        public Machine() { }

        public Machine(Models.Metadata.Machine machine)
        {
            // Get all fields to automatically copy without processing
            var nonItemFields = TypeHelper.GetConstants(typeof(Models.Metadata.Machine));
            if (nonItemFields == null)
                return;

            // Populate the internal machine from non-filter fields
            _internal = new Models.Metadata.Machine();
            foreach (string fieldName in nonItemFields)
            {
                if (machine.ContainsKey(fieldName))
                    _internal[fieldName] = machine[fieldName];
            }
        }

        #endregion

        #region Accessors

        /// <summary>
        /// Get a clone of the current internal model
        /// </summary>
        public Models.Metadata.Machine GetInternalClone() => (_internal.Clone() as Models.Metadata.Machine)!;

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
                _internal = this._internal.Clone() as Models.Metadata.Machine ?? [],
            };
        }

        #endregion

        #region Manipulation

        /// <summary>
        /// Runs a filter and determines if it passes or not
        /// </summary>
        /// <param name="filterRunner">Filter runner to use for checking</param>
        /// <returns>True if the Machine passes the filter, false otherwise</returns>
        public bool PassesFilter(FilterRunner filterRunner) => filterRunner.Run(_internal);

        /// <summary>
        /// Remove a field from the Machine
        /// </summary>
        /// <param name="fieldName">Field to remove</param>
        /// <returns>True if the removal was successful, false otherwise</returns>
        public bool RemoveField(string? fieldName)
            => FieldManipulator.RemoveField(_internal, fieldName);

        /// <summary>
        /// Replace a field from another Machine
        /// </summary>
        /// <param name="other">Machine to replace field from</param>
        /// <param name="fieldName">Field to replace</param>
        /// <returns>True if the replacement was successful, false otherwise</returns>
        public bool ReplaceField(Machine? other, string? fieldName)
            => FieldManipulator.ReplaceField(other?._internal, _internal, fieldName);

        /// <summary>
        /// Set a field in the Machine from a mapping string
        /// </summary>
        /// <param name="fieldName">Field to set</param>
        /// <param name="value">String representing the value to set</param>
        /// <returns>True if the setting was successful, false otherwise</returns>
        /// <remarks>This only performs minimal validation before setting</remarks>
        public bool SetField(string? fieldName, string value)
            => FieldManipulator.SetField(_internal, fieldName, value);

        #endregion
    }
}
