using System.Collections.Generic;
using System.Xml.Serialization;

using SabreTools.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SabreTools.DatItems
{
    /// <summary>
    /// Represents the a driver of the machine
    /// </summary>
    [JsonObject("driver"), XmlRoot("driver")]
    public class Driver : DatItem
    {
        #region Fields

        /// <summary>
        /// Overall driver status
        /// </summary>
        [JsonProperty("status", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonConverter(typeof(StringEnumConverter))]
        [XmlElement("status")]
        public SupportStatus Status { get; set; }

        [JsonIgnore]
        public bool StatusSpecified { get { return Status != SupportStatus.NULL; } }

        /// <summary>
        /// Driver emulation status
        /// </summary>
        [JsonProperty("emulation", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonConverter(typeof(StringEnumConverter))]
        [XmlElement("emulation")]
        public SupportStatus Emulation { get; set; }

        [JsonIgnore]
        public bool EmulationSpecified { get { return Emulation != SupportStatus.NULL; ; } }

        /// <summary>
        /// Cocktail orientation status
        /// </summary>
        [JsonProperty("cocktail", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonConverter(typeof(StringEnumConverter))]
        [XmlElement("cocktail")]
        public SupportStatus Cocktail { get; set; }

        [JsonIgnore]
        public bool CocktailSpecified { get { return Cocktail != SupportStatus.NULL; ; } }

        /// <summary>
        /// Save state support status
        /// </summary>
        [JsonProperty("savestate", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonConverter(typeof(StringEnumConverter))]
        [XmlElement("savestate")]
        public Supported SaveState { get; set; }

        [JsonIgnore]
        public bool SaveStateSpecified { get { return SaveState != Supported.NULL; } }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Driver object
        /// </summary>
        public Driver()
        {
            ItemType = ItemType.Driver;
        }

        #endregion

        #region Cloning Methods

        public override object Clone()
        {
            return new Driver()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine,
                Source = this.Source.Clone() as Source,
                Remove = this.Remove,

                Status = this.Status,
                Emulation = this.Emulation,
                Cocktail = this.Cocktail,
                SaveState = this.SaveState,
            };
        }

        #endregion

        #region Comparision Methods

        public override bool Equals(DatItem other)
        {
            // If we don't have a Driver, return false
            if (ItemType != other.ItemType)
                return false;

            // Otherwise, treat it as a Driver
            Driver newOther = other as Driver;

            // If the Feature information matches
            return (Status == newOther.Status
                && Emulation == newOther.Emulation
                && Cocktail == newOther.Cocktail
                && SaveState == newOther.SaveState);
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

            // If we don't have a Driver to replace from, ignore specific fields
            if (item.ItemType != ItemType.Driver)
                return;

            // Cast for easier access
            Driver newItem = item as Driver;

            // Replace the fields
            if (datItemFields.Contains(DatItemField.SupportStatus))
                Status = newItem.Status;

            if (datItemFields.Contains(DatItemField.EmulationStatus))
                Emulation = newItem.Emulation;

            if (datItemFields.Contains(DatItemField.CocktailStatus))
                Cocktail = newItem.Cocktail;

            if (datItemFields.Contains(DatItemField.SaveStateStatus))
                SaveState = newItem.SaveState;
        }

        #endregion
    }
}
