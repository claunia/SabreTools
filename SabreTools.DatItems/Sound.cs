using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

using SabreTools.Core;
using SabreTools.Core.Tools;
using Newtonsoft.Json;

namespace SabreTools.DatItems
{
    /// <summary>
    /// Represents the sound output for a machine
    /// </summary>
    [JsonObject("sound"), XmlRoot("sound")]
    public class Sound : DatItem
    {
        #region Fields

        /// <summary>
        /// Number of speakers or channels
        /// </summary>
        [JsonProperty("channels", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("channels")]
        public long? Channels { get; set; }

        [JsonIgnore]
        public bool ChannelsSpecified { get { return Channels != null; } }

        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override void SetFields(
            Dictionary<DatItemField, string> datItemMappings,
            Dictionary<MachineField, string> machineMappings)
        {
            // Set base fields
            base.SetFields(datItemMappings, machineMappings);

            // Handle Sound-specific fields
            if (datItemMappings.Keys.Contains(DatItemField.Channels))
                Channels = Utilities.CleanLong(datItemMappings[DatItemField.Channels]);
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

        /// <inheritdoc/>
        public override void RemoveFields(
            List<DatItemField> datItemFields,
            List<MachineField> machineFields)
        {
            // Remove common fields first
            base.RemoveFields(datItemFields, machineFields);

            // Remove the fields
            if (datItemFields.Contains(DatItemField.Channels))
                Channels = null;
        }

        #endregion

        #region Sorting and Merging

        /// <inheritdoc/>
        public override void ReplaceFields(
            DatItem item,
            List<DatItemField> datItemFields,
            List<MachineField> machineFields)
        {
            // Replace common fields first
            base.ReplaceFields(item, datItemFields, machineFields);

            // If we don't have a Sound to replace from, ignore specific fields
            if (item.ItemType != ItemType.Sound)
                return;

            // Cast for easier access
            Sound newItem = item as Sound;

            // Replace the fields
            if (datItemFields.Contains(DatItemField.Channels))
                Channels = newItem.Channels;
        }

        #endregion
    }
}
