using System.Collections.Generic;
using System.Xml.Serialization;

using SabreTools.Core;
using Newtonsoft.Json;

namespace SabreTools.DatItems
{
    /// <summary>
    /// Represents one conflocation or diplocation
    /// </summary>
    [JsonObject("location"), XmlRoot("location")]
    public class Location : DatItem
    {
        #region Fields

        /// <summary>
        /// Location name
        /// </summary>
        [JsonProperty("name")]
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// Location ID
        /// </summary>
        [JsonProperty("number", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("number")]
        public long? Number { get; set; }

        [JsonIgnore]
        public bool NumberSpecified { get { return Number != null; } }

        /// <summary>
        /// Determines if location is inverted or not
        /// </summary>
        [JsonProperty("inverted", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("inverted")]
        public bool? Inverted { get; set; }

        [JsonIgnore]
        public bool InvertedSpecified { get { return Inverted != null; } }

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
        /// Create a default, empty Location object
        /// </summary>
        public Location()
        {
            Name = string.Empty;
            ItemType = ItemType.Location;
        }

        #endregion

        #region Cloning Methods

        public override object Clone()
        {
            return new Location()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine,
                Source = this.Source.Clone() as Source,
                Remove = this.Remove,

                Name = this.Name,
                Number = this.Number,
                Inverted = this.Inverted,
            };
        }

        #endregion

        #region Comparision Methods

        public override bool Equals(DatItem other)
        {
            // If we don't have a Location, return false
            if (ItemType != other.ItemType)
                return false;

            // Otherwise, treat it as a Location
            Location newOther = other as Location;

            // If the Location information matches
            return (Name == newOther.Name
                && Number == newOther.Number
                && Inverted == newOther.Inverted);
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

            // If we don't have a Location to replace from, ignore specific fields
            if (item.ItemType != ItemType.Location)
                return;

            // Cast for easier access
            Location newItem = item as Location;

            // Replace the fields
            if (datItemFields.Contains(DatItemField.Location_Name))
                Name = newItem.Name;

            if (datItemFields.Contains(DatItemField.Location_Number))
                Number = newItem.Number;

            if (datItemFields.Contains(DatItemField.Location_Inverted))
                Inverted = newItem.Inverted;
        }

        #endregion
    }
}
