using System;
using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;
using SabreTools.Core.Filter;
using SabreTools.Core.Tools;

namespace SabreTools.DatItems
{
    /// <summary>
    /// Represents the information specific to a set/game/machine
    /// </summary>
    [JsonObject("machine"), XmlRoot("machine")]
    public sealed class Machine : ModelBackedItem<Models.Metadata.Machine>, ICloneable, IEquatable<Machine>
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

        public Machine()
        {
            _internal = new Models.Metadata.Machine();
        }

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

        #region Cloning methods

        /// <summary>
        /// Create a clone of the current machine
        /// </summary>
        /// <returns>New machine with the same values as the current one</returns>
        public object Clone()
        {
            return new Machine()
            {
                _internal = _internal.Clone() as Models.Metadata.Machine ?? [],
            };
        }

        /// <summary>
        /// Get a clone of the current internal model
        /// </summary>
        public Models.Metadata.Machine GetInternalClone() => (_internal.Clone() as Models.Metadata.Machine)!;

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(ModelBackedItem? other)
        {
            // If other is null
            if (other == null)
                return false;

            // If the type is mismatched
            if (other is not Machine otherItem)
                return false;

            // Compare internal models
            return _internal.EqualTo(otherItem._internal);
        }

        /// <inheritdoc/>
        public override bool Equals(ModelBackedItem<Models.Metadata.Machine>? other)
        {
            // If other is null
            if (other == null)
                return false;

            // If the type is mismatched
            if (other is not Machine otherItem)
                return false;

            // Compare internal models
            return _internal.EqualTo(otherItem._internal);
        }

        /// <inheritdoc/>
        public bool Equals(Machine? other)
        {
            // If other is null
            if (other == null)
                return false;

            // Compare internal models
            return _internal.EqualTo(other._internal);
        }

        #endregion

        #region Manipulation

        /// <summary>
        /// Runs a filter and determines if it passes or not
        /// </summary>
        /// <param name="filterRunner">Filter runner to use for checking</param>
        /// <returns>True if the Machine passes the filter, false otherwise</returns>
        public bool PassesFilter(FilterRunner filterRunner) => filterRunner.Run(_internal);

        #endregion
    }
}
