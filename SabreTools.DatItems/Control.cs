using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

using SabreTools.Core;
using SabreTools.Core.Tools;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SabreTools.DatItems
{
    /// <summary>
    /// Represents control for an input
    /// </summary>
    [JsonObject("control"), XmlRoot("control")]
    public class Control : DatItem
    {
        #region Fields

        /// <summary>
        /// General type of input
        /// </summary>
        [JsonProperty("type", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonConverter(typeof(StringEnumConverter))]
        [XmlElement("type")]
        public ControlType ControlType { get; set; }

        [JsonIgnore]
        public bool ControlTypeSpecified { get { return ControlType != ControlType.NULL; } }

        /// <summary>
        /// Player which the input belongs to
        /// </summary>
        [JsonProperty("player", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("player")]
        public long? Player { get; set; }

        [JsonIgnore]
        public bool PlayerSpecified { get { return Player != null; } }

        /// <summary>
        /// Total number of buttons
        /// </summary>
        [JsonProperty("buttons", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("buttons")]
        public long? Buttons { get; set; }

        [JsonIgnore]
        public bool ButtonsSpecified { get { return Buttons != null; } }

        /// <summary>
        /// Total number of non-optional buttons
        /// </summary>
        [JsonProperty("reqbuttons", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("reqbuttons")]
        public long? RequiredButtons { get; set; }

        [JsonIgnore]
        public bool RequiredButtonsSpecified { get { return RequiredButtons != null; } }

        /// <summary>
        /// Analog minimum value
        /// </summary>
        [JsonProperty("minimum", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("minimum")]
        public long? Minimum { get; set; }

        [JsonIgnore]
        public bool MinimumSpecified { get { return Minimum != null; } }

        /// <summary>
        /// Analog maximum value
        /// </summary>
        [JsonProperty("maximum", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("maximum")]
        public long? Maximum { get; set; }

        [JsonIgnore]
        public bool MaximumSpecified { get { return Maximum != null; } }

        /// <summary>
        /// Default analog sensitivity
        /// </summary>
        [JsonProperty("sensitivity", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("sensitivity")]
        public long? Sensitivity { get; set; }

        [JsonIgnore]
        public bool SensitivitySpecified { get { return Sensitivity != null; } }

        /// <summary>
        /// Default analog keydelta
        /// </summary>
        [JsonProperty("keydelta", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("keydelta")]
        public long? KeyDelta { get; set; }

        [JsonIgnore]
        public bool KeyDeltaSpecified { get { return KeyDelta != null; } }

        /// <summary>
        /// Default analog reverse setting
        /// </summary>
        [JsonProperty("reverse", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("reverse")]
        public bool? Reverse { get; set; }

        [JsonIgnore]
        public bool ReverseSpecified { get { return Reverse != null; } }

        /// <summary>
        /// First set of ways
        /// </summary>
        [JsonProperty("ways", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("ways")]
        public string Ways { get; set; }

        /// <summary>
        /// Second set of ways
        /// </summary>
        [JsonProperty("ways2", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("ways2")]
        public string Ways2 { get; set; }

        /// <summary>
        /// Third set of ways
        /// </summary>
        [JsonProperty("ways3", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("ways3")]
        public string Ways3 { get; set; }

        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override void SetFields(
            Dictionary<DatItemField, string> datItemMappings,
            Dictionary<MachineField, string> machineMappings)
        {
            // Set base fields
            base.SetFields(datItemMappings, machineMappings);

            // Handle Control-specific fields
            if (datItemMappings.Keys.Contains(DatItemField.Control_Type))
                ControlType = datItemMappings[DatItemField.Control_Type].AsControlType();

            if (datItemMappings.Keys.Contains(DatItemField.Control_Player))
                Player = Utilities.CleanLong(datItemMappings[DatItemField.Control_Player]);

            if (datItemMappings.Keys.Contains(DatItemField.Control_Buttons))
                Buttons = Utilities.CleanLong(datItemMappings[DatItemField.Control_Buttons]);

            if (datItemMappings.Keys.Contains(DatItemField.Control_RequiredButtons))
                RequiredButtons = Utilities.CleanLong(datItemMappings[DatItemField.Control_RequiredButtons]);

            if (datItemMappings.Keys.Contains(DatItemField.Control_Minimum))
                Minimum = Utilities.CleanLong(datItemMappings[DatItemField.Control_Minimum]);

            if (datItemMappings.Keys.Contains(DatItemField.Control_Maximum))
                Maximum = Utilities.CleanLong(datItemMappings[DatItemField.Control_Maximum]);

            if (datItemMappings.Keys.Contains(DatItemField.Control_Sensitivity))
                Sensitivity = Utilities.CleanLong(datItemMappings[DatItemField.Control_Sensitivity]);

            if (datItemMappings.Keys.Contains(DatItemField.Control_KeyDelta))
                KeyDelta = Utilities.CleanLong(datItemMappings[DatItemField.Control_KeyDelta]);

            if (datItemMappings.Keys.Contains(DatItemField.Control_Reverse))
                Reverse = datItemMappings[DatItemField.Control_Reverse].AsYesNo();

            if (datItemMappings.Keys.Contains(DatItemField.Control_Ways))
                Ways = datItemMappings[DatItemField.Control_Ways];

            if (datItemMappings.Keys.Contains(DatItemField.Control_Ways2))
                Ways2 = datItemMappings[DatItemField.Control_Ways2];

            if (datItemMappings.Keys.Contains(DatItemField.Control_Ways3))
                Ways3 = datItemMappings[DatItemField.Control_Ways3];
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Control object
        /// </summary>
        public Control()
        {
            ItemType = ItemType.Control;
        }

        #endregion

        #region Cloning Methods

        public override object Clone()
        {
            return new Control()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine,
                Source = this.Source.Clone() as Source,
                Remove = this.Remove,

                ControlType = this.ControlType,
                Player = this.Player,
                Buttons = this.Buttons,
                RequiredButtons = this.RequiredButtons,
                Minimum = this.Minimum,
                Maximum = this.Maximum,
                Sensitivity = this.Sensitivity,
                KeyDelta = this.KeyDelta,
                Reverse = this.Reverse,
                Ways = this.Ways,
                Ways2 = this.Ways2,
                Ways3 = this.Ways3,
            };
        }

        #endregion

        #region Comparision Methods

        public override bool Equals(DatItem other)
        {
            // If we don't have a Control, return false
            if (ItemType != other.ItemType)
                return false;

            // Otherwise, treat it as a Control
            Control newOther = other as Control;

            // If the Control information matches
            return (ControlType == newOther.ControlType
                && Player == newOther.Player
                && Buttons == newOther.Buttons
                && RequiredButtons == newOther.RequiredButtons
                && Minimum == newOther.Minimum
                && Maximum == newOther.Maximum
                && Sensitivity == newOther.Sensitivity
                && KeyDelta == newOther.KeyDelta
                && Reverse == newOther.Reverse
                && Ways == newOther.Ways
                && Ways2 == newOther.Ways2
                && Ways3 == newOther.Ways3);
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

            // If we don't have a Control to replace from, ignore specific fields
            if (item.ItemType != ItemType.Control)
                return;

            // Cast for easier access
            Control newItem = item as Control;

            // Replace the fields
            if (datItemFields.Contains(DatItemField.Control_Type))
                ControlType = newItem.ControlType;

            if (datItemFields.Contains(DatItemField.Control_Player))
                Player = newItem.Player;

            if (datItemFields.Contains(DatItemField.Control_Buttons))
                Buttons = newItem.Buttons;

            if (datItemFields.Contains(DatItemField.Control_RequiredButtons))
                RequiredButtons = newItem.RequiredButtons;

            if (datItemFields.Contains(DatItemField.Control_Minimum))
                Minimum = newItem.Minimum;

            if (datItemFields.Contains(DatItemField.Control_Maximum))
                Maximum = newItem.Maximum;

            if (datItemFields.Contains(DatItemField.Control_Sensitivity))
                Sensitivity = newItem.Sensitivity;

            if (datItemFields.Contains(DatItemField.Control_KeyDelta))
                KeyDelta = newItem.KeyDelta;

            if (datItemFields.Contains(DatItemField.Control_Reverse))
                Reverse = newItem.Reverse;

            if (datItemFields.Contains(DatItemField.Control_Ways))
                Ways = newItem.Ways;

            if (datItemFields.Contains(DatItemField.Control_Ways2))
                Ways2 = newItem.Ways2;

            if (datItemFields.Contains(DatItemField.Control_Ways3))
                Ways3 = newItem.Ways3;
        }

        #endregion
    }
}
