using System;
using System.Collections.Generic;
using System.Linq;

using SabreTools.Library.Filtering;
using SabreTools.Library.Tools;
using Newtonsoft.Json;

namespace SabreTools.Library.DatItems
{
    /// <summary>
    /// Represents control for an input
    /// </summary>
    [JsonObject("control")]
    public class Control : DatItem
    {
        #region Fields

        /// <summary>
        /// General type of input
        /// </summary>
        [JsonProperty("type", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public ControlType ControlType { get; set; }

        /// <summary>
        /// Player which the input belongs to
        /// </summary>
        [JsonProperty("player", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public long? Player { get; set; }

        /// <summary>
        /// Total number of buttons
        /// </summary>
        [JsonProperty("buttons", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public long? Buttons { get; set; }

        /// <summary>
        /// Total number of non-optional buttons
        /// </summary>
        [JsonProperty("reqbuttons", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public long? RequiredButtons { get; set; }

        /// <summary>
        /// Analog minimum value
        /// </summary>
        [JsonProperty("minimum", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public long? Minimum { get; set; }

        /// <summary>
        /// Analog maximum value
        /// </summary>
        [JsonProperty("maximum", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public long? Maximum { get; set; }

        /// <summary>
        /// Default analog sensitivity
        /// </summary>
        [JsonProperty("sensitivity", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public long? Sensitivity { get; set; }

        /// <summary>
        /// Default analog keydelta
        /// </summary>
        [JsonProperty("keydelta", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public long? KeyDelta { get; set; }

        /// <summary>
        /// Default analog reverse setting
        /// </summary>
        [JsonProperty("reverse", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool? Reverse { get; set; }

        /// <summary>
        /// First set of ways
        /// </summary>
        [JsonProperty("ways", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Ways { get; set; }

        /// <summary>
        /// Second set of ways
        /// </summary>
        [JsonProperty("ways2", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Ways2 { get; set; }

        /// <summary>
        /// Third set of ways
        /// </summary>
        [JsonProperty("ways3", DefaultValueHandling = DefaultValueHandling.Ignore)]
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
        /// <returns>True if the item passed the filter, false otherwise</returns>
        public override bool PassesFilter(Filter filter)
        {
            // Check common fields first
            if (!base.PassesFilter(filter))
                return false;

            // Filter on control type
            if (filter.DatItem_Control_Type.MatchesPositive(ControlType.NULL, ControlType) == false)
                return false;
            if (filter.DatItem_Control_Type.MatchesNegative(ControlType.NULL, ControlType) == true)
                return false;

            // Filter on player
            if (filter.DatItem_Control_Player.MatchesNeutral(null, Player) == false)
                return false;
            else if (filter.DatItem_Control_Player.MatchesPositive(null, Player) == false)
                return false;
            else if (filter.DatItem_Control_Player.MatchesNegative(null, Player) == false)
                return false;

            // Filter on buttons
            if (filter.DatItem_Control_Buttons.MatchesNeutral(null, Buttons) == false)
                return false;
            else if (filter.DatItem_Control_Buttons.MatchesPositive(null, Buttons) == false)
                return false;
            else if (filter.DatItem_Control_Buttons.MatchesNegative(null, Buttons) == false)
                return false;

            // Filter on reqbuttons
            if (filter.DatItem_Control_ReqButtons.MatchesNeutral(null, RequiredButtons) == false)
                return false;
            else if (filter.DatItem_Control_ReqButtons.MatchesPositive(null, RequiredButtons) == false)
                return false;
            else if (filter.DatItem_Control_ReqButtons.MatchesNegative(null, RequiredButtons) == false)
                return false;

            // Filter on minimum
            if (filter.DatItem_Control_Minimum.MatchesNeutral(null, Minimum) == false)
                return false;
            else if (filter.DatItem_Control_Minimum.MatchesPositive(null, Minimum) == false)
                return false;
            else if (filter.DatItem_Control_Minimum.MatchesNegative(null, Minimum) == false)
                return false;

            // Filter on maximum
            if (filter.DatItem_Control_Maximum.MatchesNeutral(null, Maximum) == false)
                return false;
            else if (filter.DatItem_Control_Maximum.MatchesPositive(null, Maximum) == false)
                return false;
            else if (filter.DatItem_Control_Maximum.MatchesNegative(null, Maximum) == false)
                return false;

            // Filter on sensitivity
            if (filter.DatItem_Control_Sensitivity.MatchesNeutral(null, Sensitivity) == false)
                return false;
            else if (filter.DatItem_Control_Sensitivity.MatchesPositive(null, Sensitivity) == false)
                return false;
            else if (filter.DatItem_Control_Sensitivity.MatchesNegative(null, Sensitivity) == false)
                return false;

            // Filter on keydelta
            if (filter.DatItem_Control_KeyDelta.MatchesNeutral(null, KeyDelta) == false)
                return false;
            else if (filter.DatItem_Control_KeyDelta.MatchesPositive(null, KeyDelta) == false)
                return false;
            else if (filter.DatItem_Control_KeyDelta.MatchesNegative(null, KeyDelta) == false)
                return false;

            // Filter on reverse
            if (filter.DatItem_Control_Reverse.MatchesNeutral(null, Reverse) == false)
                return false;

            // Filter on ways
            if (filter.DatItem_Control_Ways.MatchesPositiveSet(Ways) == false)
                return false;
            if (filter.DatItem_Control_Ways.MatchesNegativeSet(Ways) == true)
                return false;

            // Filter on ways2
            if (filter.DatItem_Control_Ways2.MatchesPositiveSet(Ways2) == false)
                return false;
            if (filter.DatItem_Control_Ways2.MatchesNegativeSet(Ways2) == true)
                return false;

            // Filter on ways3
            if (filter.DatItem_Control_Ways3.MatchesPositiveSet(Ways3) == false)
                return false;
            if (filter.DatItem_Control_Ways3.MatchesNegativeSet(Ways3) == true)
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
