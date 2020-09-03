using System.Collections.Generic;
using System.Linq;

using SabreTools.Library.Filtering;
using Newtonsoft.Json;

namespace SabreTools.Library.DatItems
{
    /// <summary>
    /// Represents the sound output for a machine
    /// </summary>
    [JsonObject("sound")]
    public class Sound : DatItem
    {
        #region Fields

        /// <summary>
        /// Number of channels
        /// </summary>
        [JsonProperty("channels", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Channels { get; set; } // TODO: Int32?

        #endregion

        #region Accessors

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="mappings">Mappings dictionary</param>
        public override void SetFields(Dictionary<Field, string> mappings)
        {
            // Set base fields
            base.SetFields(mappings);

            // Handle Sample-specific fields
            if (mappings.Keys.Contains(Field.DatItem_Channels))
                Channels = mappings[Field.DatItem_Channels];
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Sound object
        /// </summary>
        public Sound()
        {
            ItemType = ItemType.Sound;
        }

        #endregion

        #region Cloning Methods

        public override object Clone()
        {
            return new Sound()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                AltName = this.AltName,
                AltTitle = this.AltTitle,

                Machine = this.Machine.Clone() as Machine,
                Source = this.Source.Clone() as Source,
                Remove = this.Remove,

                Channels = this.Channels,
            };
        }

        #endregion

        #region Comparision Methods

        public override bool Equals(DatItem other)
        {
            // If we don't have a Sound, return false
            if (ItemType != other.ItemType)
                return false;

            // Otherwise, treat it as a Sound
            Sound newOther = other as Sound;

            // If the Sound information matches
            return (Channels == newOther.Channels);
        }

        #endregion

        #region Filtering

        /// <summary>
        /// Check to see if a DatItem passes the filter
        /// </summary>
        /// <param name="filter">Filter to check against</param>
        /// <returns>True if the item passed the filter, false otherwise</returns>
        public override bool PassesFilter(Filter filter)
        {
            // Check common fields first
            if (!base.PassesFilter(filter))
                return false;

            // Filter on channels
            if (filter.DatItem_Channels.MatchesPositiveSet(Channels) == false)
                return false;
            if (filter.DatItem_Channels.MatchesNegativeSet(Channels) == true)
                return false;

            return true;
        }

        /// <summary>
        /// Remove fields from the DatItem
        /// </summary>
        /// <param name="fields">List of Fields to remove</param>
        public override void RemoveFields(List<Field> fields)
        {
            // Remove common fields first
            base.RemoveFields(fields);

            // Remove the fields
            if (fields.Contains(Field.DatItem_Channels))
                Channels = null;
        }

        #endregion

        #region Sorting and Merging

        /// <summary>
        /// Replace fields from another item
        /// </summary>
        /// <param name="item">DatItem to pull new information from</param>
        /// <param name="fields">List of Fields representing what should be updated</param>
        public override void ReplaceFields(DatItem item, List<Field> fields)
        {
            // Replace common fields first
            base.ReplaceFields(item, fields);

            // If we don't have a Sound to replace from, ignore specific fields
            if (item.ItemType != ItemType.Sound)
                return;

            // Cast for easier access
            Sound newItem = item as Sound;

            // Replace the fields
            if (fields.Contains(Field.DatItem_Channels))
                Channels = newItem.Channels;
        }

        #endregion
    }
}
