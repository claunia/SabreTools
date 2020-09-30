using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

using SabreTools.Library.Filtering;
using SabreTools.Library.Tools;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SabreTools.Library.DatItems
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

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="mappings">Mappings dictionary</param>
        public override void SetFields(Dictionary<Field, string> mappings)
        {
            // Set base fields
            base.SetFields(mappings);

            // Handle Control-specific fields
            if (mappings.Keys.Contains(Field.DatItem_Control_Type))
                ControlType = mappings[Field.DatItem_Control_Type].AsControlType();

            if (mappings.Keys.Contains(Field.DatItem_Control_Player))
                Player = Sanitizer.CleanLong(mappings[Field.DatItem_Control_Player]);

            if (mappings.Keys.Contains(Field.DatItem_Control_Buttons))
                Buttons = Sanitizer.CleanLong(mappings[Field.DatItem_Control_Buttons]);

            if (mappings.Keys.Contains(Field.DatItem_Control_RequiredButtons))
                RequiredButtons = Sanitizer.CleanLong(mappings[Field.DatItem_Control_RequiredButtons]);

            if (mappings.Keys.Contains(Field.DatItem_Control_Minimum))
                Minimum = Sanitizer.CleanLong(mappings[Field.DatItem_Control_Minimum]);

            if (mappings.Keys.Contains(Field.DatItem_Control_Maximum))
                Maximum = Sanitizer.CleanLong(mappings[Field.DatItem_Control_Maximum]);

            if (mappings.Keys.Contains(Field.DatItem_Control_Sensitivity))
                Sensitivity = Sanitizer.CleanLong(mappings[Field.DatItem_Control_Sensitivity]);

            if (mappings.Keys.Contains(Field.DatItem_Control_KeyDelta))
                KeyDelta = Sanitizer.CleanLong(mappings[Field.DatItem_Control_KeyDelta]);

            if (mappings.Keys.Contains(Field.DatItem_Control_Reverse))
                Reverse = mappings[Field.DatItem_Control_Reverse].AsYesNo();

            if (mappings.Keys.Contains(Field.DatItem_Control_Ways))
                Ways = mappings[Field.DatItem_Control_Ways];

            if (mappings.Keys.Contains(Field.DatItem_Control_Ways2))
                Ways2 = mappings[Field.DatItem_Control_Ways2];

            if (mappings.Keys.Contains(Field.DatItem_Control_Ways3))
                Ways3 = mappings[Field.DatItem_Control_Ways3];
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

        #region Filtering

        /// <summary>
        /// Check to see if a DatItem passes the filter
        /// </summary>
        /// <param name="filter">Filter to check against</param>
        /// <param name="sub">True if this is a subitem, false otherwise</param>
        /// <returns>True if the item passed the filter, false otherwise</returns>
        public override bool PassesFilter(Filter filter, bool sub = false)
        {
            // Check common fields first
            if (!base.PassesFilter(filter, sub))
                return false;

            // Filter on control type
            if (filter.DatItem_Control_Type.MatchesPositive(ControlType.NULL, ControlType) == false)
                return false;
            if (filter.DatItem_Control_Type.MatchesNegative(ControlType.NULL, ControlType) == true)
                return false;

            // Filter on player
            if (!filter.PassLongFilter(filter.DatItem_Control_Player, Player))
                return false;

            // Filter on buttons
            if (!filter.PassLongFilter(filter.DatItem_Control_Buttons, Buttons))
                return false;

            // Filter on reqbuttons
            if (!filter.PassLongFilter(filter.DatItem_Control_ReqButtons, RequiredButtons))
                return false;

            // Filter on minimum
            if (!filter.PassLongFilter(filter.DatItem_Control_Minimum, Minimum))
                return false;

            // Filter on maximum
            if (!filter.PassLongFilter(filter.DatItem_Control_Maximum, Maximum))
                return false;

            // Filter on sensitivity
            if (!filter.PassLongFilter(filter.DatItem_Control_Sensitivity, Sensitivity))
                return false;

            // Filter on keydelta
            if (!filter.PassLongFilter(filter.DatItem_Control_KeyDelta, KeyDelta))
                return false;

            // Filter on reverse
            if (!filter.PassBoolFilter(filter.DatItem_Control_Reverse, Reverse))
                return false;

            // Filter on ways
            if (!filter.PassStringFilter(filter.DatItem_Control_Ways, Ways))
                return false;

            // Filter on ways2
            if (!filter.PassStringFilter(filter.DatItem_Control_Ways2, Ways2))
                return false;

            // Filter on ways3
            if (!filter.PassStringFilter(filter.DatItem_Control_Ways3, Ways3))
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
            if (fields.Contains(Field.DatItem_Control_Type))
                ControlType = ControlType.NULL;

            if (fields.Contains(Field.DatItem_Control_Player))
                Player = null;

            if (fields.Contains(Field.DatItem_Control_Buttons))
                Buttons = null;

            if (fields.Contains(Field.DatItem_Control_RequiredButtons))
                RequiredButtons = null;

            if (fields.Contains(Field.DatItem_Control_Minimum))
                Minimum = null;

            if (fields.Contains(Field.DatItem_Control_Maximum))
                Maximum = null;

            if (fields.Contains(Field.DatItem_Control_Sensitivity))
                Sensitivity = null;

            if (fields.Contains(Field.DatItem_Control_KeyDelta))
                KeyDelta = null;

            if (fields.Contains(Field.DatItem_Control_Reverse))
                Reverse = null;

            if (fields.Contains(Field.DatItem_Control_Ways))
                Ways = null;

            if (fields.Contains(Field.DatItem_Control_Ways2))
                Ways2 = null;

            if (fields.Contains(Field.DatItem_Control_Ways3))
                Ways3 = null;
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

            // If we don't have a Control to replace from, ignore specific fields
            if (item.ItemType != ItemType.Control)
                return;

            // Cast for easier access
            Control newItem = item as Control;

            // Replace the fields
            if (fields.Contains(Field.DatItem_Control_Type))
                ControlType = newItem.ControlType;

            if (fields.Contains(Field.DatItem_Control_Player))
                Player = newItem.Player;

            if (fields.Contains(Field.DatItem_Control_Buttons))
                Buttons = newItem.Buttons;

            if (fields.Contains(Field.DatItem_Control_RequiredButtons))
                RequiredButtons = newItem.RequiredButtons;

            if (fields.Contains(Field.DatItem_Control_Minimum))
                Minimum = newItem.Minimum;

            if (fields.Contains(Field.DatItem_Control_Maximum))
                Maximum = newItem.Maximum;

            if (fields.Contains(Field.DatItem_Control_Sensitivity))
                Sensitivity = newItem.Sensitivity;

            if (fields.Contains(Field.DatItem_Control_KeyDelta))
                KeyDelta = newItem.KeyDelta;

            if (fields.Contains(Field.DatItem_Control_Reverse))
                Reverse = newItem.Reverse;

            if (fields.Contains(Field.DatItem_Control_Ways))
                Ways = newItem.Ways;

            if (fields.Contains(Field.DatItem_Control_Ways2))
                Ways2 = newItem.Ways2;

            if (fields.Contains(Field.DatItem_Control_Ways3))
                Ways3 = newItem.Ways3;
        }

        #endregion
    }
}
