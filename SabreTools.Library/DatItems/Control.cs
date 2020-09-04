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
        /// Control type
        /// </summary>
        [JsonProperty("type", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string ControlType { get; set; }

        /// <summary>
        /// Player ID
        /// </summary>
        [JsonProperty("player", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public long? Player { get; set; }

        /// <summary>
        /// Button count
        /// </summary>
        [JsonProperty("buttons", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public long? Buttons { get; set; }

        /// <summary>
        /// Regular button count
        /// </summary>
        /// <remarks>Couldn't find this used in newest ListXML</remarks>
        [JsonProperty("regbuttons", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string RegButtons { get; set; } // TODO: Int32?

        /// <summary>
        /// Minimum value
        /// </summary>
        [JsonProperty("minimum", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public long? Minimum { get; set; }

        /// <summary>
        /// Maximum value
        /// </summary>
        [JsonProperty("maximum", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public long? Maximum { get; set; }

        /// <summary>
        /// Sensitivity value
        /// </summary>
        [JsonProperty("sensitivity", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public long? Sensitivity { get; set; }

        /// <summary>
        /// Keypress delta
        /// </summary>
        [JsonProperty("keydelta", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public long? KeyDelta { get; set; }

        /// <summary>
        /// Determines if the control is reversed
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
                ControlType = mappings[Field.DatItem_Control_Type];

            if (mappings.Keys.Contains(Field.DatItem_Control_Player))
            {
                if (Int64.TryParse(mappings[Field.DatItem_Control_Player], out long player))
                    Player = player;
            }

            if (mappings.Keys.Contains(Field.DatItem_Control_Buttons))
            {
                if (Int64.TryParse(mappings[Field.DatItem_Control_Buttons], out long buttons))
                    Buttons = buttons;
            }

            if (mappings.Keys.Contains(Field.DatItem_Control_RegButtons))
                RegButtons = mappings[Field.DatItem_Control_RegButtons];

            if (mappings.Keys.Contains(Field.DatItem_Control_Minimum))
            {
                if (Int64.TryParse(mappings[Field.DatItem_Control_Minimum], out long minimum))
                    Minimum = minimum;
            }

            if (mappings.Keys.Contains(Field.DatItem_Control_Maximum))
            {
                if (Int64.TryParse(mappings[Field.DatItem_Control_Maximum], out long maximum))
                    Maximum = maximum;
            }

            if (mappings.Keys.Contains(Field.DatItem_Control_Sensitivity))
            {
                if (Int64.TryParse(mappings[Field.DatItem_Control_Sensitivity], out long sensitivity))
                    Sensitivity = sensitivity;
            }

            if (mappings.Keys.Contains(Field.DatItem_Control_KeyDelta))
            {
                if (Int64.TryParse(mappings[Field.DatItem_Control_KeyDelta], out long keyDelta))
                    KeyDelta = keyDelta;
            }

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
                RegButtons = this.RegButtons,
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
                && RegButtons == newOther.RegButtons
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
            if (filter.DatItem_Control_Type.MatchesPositiveSet(ControlType) == false)
                return false;
            if (filter.DatItem_Control_Type.MatchesNegativeSet(ControlType) == true)
                return false;

            // Filter on display type
            if (filter.DatItem_Control_Player.MatchesPositive(null, Player) == false)
                return false;
            if (filter.DatItem_Control_Player.MatchesNegative(null, Player) == true)
                return false;

            // Filter on buttons
            if (filter.DatItem_Control_Buttons.MatchesPositive(null, Buttons) == false)
                return false;
            if (filter.DatItem_Control_Buttons.MatchesNegative(null, Buttons) == true)
                return false;

            // Filter on regbuttons
            if (filter.DatItem_Control_RegButtons.MatchesPositiveSet(RegButtons) == false)
                return false;
            if (filter.DatItem_Control_RegButtons.MatchesNegativeSet(RegButtons) == true)
                return false;

            // Filter on minimum
            if (filter.DatItem_Control_Minimum.MatchesPositive(null, Minimum) == false)
                return false;
            if (filter.DatItem_Control_Minimum.MatchesNegative(null, Minimum) == true)
                return false;

            // Filter on maximum
            if (filter.DatItem_Control_Maximum.MatchesPositive(null, Maximum) == false)
                return false;
            if (filter.DatItem_Control_Maximum.MatchesNegative(null, Maximum) == true)
                return false;

            // Filter on sensitivity
            if (filter.DatItem_Control_Sensitivity.MatchesPositive(null, Sensitivity) == false)
                return false;
            if (filter.DatItem_Control_Sensitivity.MatchesNegative(null, Sensitivity) == true)
                return false;

            // Filter on keydelta
            if (filter.DatItem_Control_KeyDelta.MatchesPositive(null, KeyDelta) == false)
                return false;
            if (filter.DatItem_Control_KeyDelta.MatchesNegative(null, KeyDelta) == true)
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
                ControlType = null;

            if (fields.Contains(Field.DatItem_Control_Player))
                Player = null;

            if (fields.Contains(Field.DatItem_Control_Buttons))
                Buttons = null;

            if (fields.Contains(Field.DatItem_Control_RegButtons))
                RegButtons = null;

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

            if (fields.Contains(Field.DatItem_Control_RegButtons))
                RegButtons = newItem.RegButtons;

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
