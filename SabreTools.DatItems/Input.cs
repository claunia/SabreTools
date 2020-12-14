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
    /// Represents one ListXML input
    /// </summary>
    [JsonObject("input"), XmlRoot("input")]
    public class Input : DatItem
    {
        #region Fields

        /// <summary>
        /// Input service ID
        /// </summary>
        [JsonProperty("service", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("service")]
        public bool? Service { get; set; }

        [JsonIgnore]
        public bool ServiceSpecified { get { return Service != null; } }

        /// <summary>
        /// Determins if this has a tilt sensor
        /// </summary>
        [JsonProperty("tilt", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("tilt")]
        public bool? Tilt { get; set; }

        [JsonIgnore]
        public bool TiltSpecified { get { return Tilt != null; } }

        /// <summary>
        /// Number of players on the input
        /// </summary>
        [JsonProperty("players", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("players")]
        public long? Players { get; set; }

        [JsonIgnore]
        public bool PlayersSpecified { get { return Players != null; } }

        /// <summary>
        /// Number of coins required
        /// </summary>
        [JsonProperty("coins", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("coins")]
        public long? Coins { get; set; }

        [JsonIgnore]
        public bool CoinsSpecified { get { return Coins != null; } }

        /// <summary>
        /// Set of controls for the input
        /// </summary>
        [JsonProperty("controls", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("controls")]
        public List<Control> Controls { get; set; }

        [JsonIgnore]
        public bool ControlsSpecified { get { return Controls != null && Controls.Count > 0; } }

        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override void SetFields(
            Dictionary<DatItemField, string> datItemMappings,
            Dictionary<MachineField, string> machineMappings)
        {
            // Set base fields
            base.SetFields(datItemMappings, machineMappings);

            // Handle Input-specific fields
            if (datItemMappings.Keys.Contains(DatItemField.Service))
                Service = datItemMappings[DatItemField.Service].AsYesNo();

            if (datItemMappings.Keys.Contains(DatItemField.Tilt))
                Tilt = datItemMappings[DatItemField.Tilt].AsYesNo();

            if (datItemMappings.Keys.Contains(DatItemField.Players))
                Players = Utilities.CleanLong(datItemMappings[DatItemField.Players]);

            if (datItemMappings.Keys.Contains(DatItemField.Coins))
                Coins = Utilities.CleanLong(datItemMappings[DatItemField.Coins]);

            if (ControlsSpecified)
            {
                foreach (Control control in Controls)
                {
                    control.SetFields(datItemMappings, machineMappings);
                }
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Input object
        /// </summary>
        public Input()
        {
            ItemType = ItemType.Input;
        }

        #endregion

        #region Cloning Methods

        public override object Clone()
        {
            return new Input()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine,
                Source = this.Source.Clone() as Source,
                Remove = this.Remove,

                Service = this.Service,
                Tilt = this.Tilt,
                Players = this.Players,
                Coins = this.Coins,
                Controls = this.Controls,
            };
        }

        #endregion

        #region Comparision Methods

        public override bool Equals(DatItem other)
        {
            // If we don't have a Input, return false
            if (ItemType != other.ItemType)
                return false;

            // Otherwise, treat it as a Input
            Input newOther = other as Input;

            // If the Input information matches
            bool match = (Service == newOther.Service
                && Tilt == newOther.Tilt
                && Players == newOther.Players
                && Coins == newOther.Coins);
            if (!match)
                return match;

            // If the controls match
            if (ControlsSpecified)
            {
                foreach (Control control in Controls)
                {
                    match &= newOther.Controls.Contains(control);
                }
            }

            return match;
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
            if (datItemFields.Contains(DatItemField.Service))
                Service = null;

            if (datItemFields.Contains(DatItemField.Tilt))
                Tilt = null;

            if (datItemFields.Contains(DatItemField.Players))
                Players = 0;

            if (datItemFields.Contains(DatItemField.Coins))
                Coins = null;

            if (ControlsSpecified)
            {
                foreach (Control control in Controls)
                {
                    control.RemoveFields(datItemFields, machineFields);
                }
            }
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

            // If we don't have a Input to replace from, ignore specific fields
            if (item.ItemType != ItemType.Input)
                return;

            // Cast for easier access
            Input newItem = item as Input;

            // Replace the fields
            if (datItemFields.Contains(DatItemField.Service))
                Service = newItem.Service;

            if (datItemFields.Contains(DatItemField.Tilt))
                Tilt = newItem.Tilt;

            if (datItemFields.Contains(DatItemField.Players))
                Players = newItem.Players;

            if (datItemFields.Contains(DatItemField.Coins))
                Coins = newItem.Coins;

            // DatItem_Control_* doesn't make sense here
            // since not every control under the other item
            // can replace every control under this item
        }

        #endregion
    }
}
