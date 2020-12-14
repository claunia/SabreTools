using System.Collections.Generic;
using System.Xml.Serialization;

using SabreTools.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SabreTools.DatItems
{
    /// <summary>
    /// Represents which Chip(s) is associated with a set
    /// </summary>
    [JsonObject("chip"), XmlRoot("chip")]
    public class Chip : DatItem
    {
        #region Fields

        /// <summary>
        /// Name of the item
        /// </summary>
        [JsonProperty("name")]
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// Internal tag
        /// </summary>
        [JsonProperty("tag", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("tag")]
        public string Tag { get; set; }

        /// <summary>
        /// Type of the chip
        /// </summary>
        [JsonProperty("type", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonConverter(typeof(StringEnumConverter))]
        [XmlElement("type")]
        public ChipType ChipType { get; set; }

        [JsonIgnore]
        public bool ChipTypeSpecified { get { return ChipType != ChipType.NULL; } }

        /// <summary>
        /// Clock speed
        /// </summary>
        [JsonProperty("clock", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("clock")]
        public long? Clock { get; set; }

        [JsonIgnore]
        public bool ClockTypeSpecified { get { return Clock != null; } }

        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override string GetName()
        {
            return Name;
        }

        /// <inheritdoc/>
        public override void SetName(string name)
        {
            Name = name;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Chip object
        /// </summary>
        public Chip()
        {
            Name = string.Empty;
            ItemType = ItemType.Chip;
        }

        #endregion

        #region Cloning Methods

        public override object Clone()
        {
            return new Chip()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine,
                Source = this.Source.Clone() as Source,
                Remove = this.Remove,

                Name = this.Name,
                Tag = this.Tag,
                ChipType = this.ChipType,
                Clock = this.Clock,
            };
        }

        #endregion

        #region Comparision Methods

        public override bool Equals(DatItem other)
        {
            // If we don't have a chip, return false
            if (ItemType != other.ItemType)
                return false;

            // Otherwise, treat it as a chip
            Chip newOther = other as Chip;

            // If the chip information matches
            return (Name == newOther.Name
                && Tag == newOther.Tag
                && ChipType == newOther.ChipType
                && Clock == newOther.Clock);
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

            // If we don't have a Chip to replace from, ignore specific fields
            if (item.ItemType != ItemType.Chip)
                return;

            // Cast for easier access
            Chip newItem = item as Chip;

            // Replace the fields
            if (datItemFields.Contains(DatItemField.Name))
                Name = newItem.Name;

            if (datItemFields.Contains(DatItemField.Tag))
                Tag = newItem.Tag;

            if (datItemFields.Contains(DatItemField.ChipType))
                ChipType = newItem.ChipType;

            if (datItemFields.Contains(DatItemField.Clock))
                Clock = newItem.Clock;
        }

        #endregion
    }
}
