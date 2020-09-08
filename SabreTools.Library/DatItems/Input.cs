using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

using SabreTools.Library.Filtering;
using SabreTools.Library.Tools;
using Newtonsoft.Json;

namespace SabreTools.Library.DatItems
{
    /// <summary>
    /// Represents one ListXML input
    /// </summary>
    [JsonObject("input")]
    [XmlRoot("input")]
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

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="mappings">Mappings dictionary</param>
        public override void SetFields(Dictionary<Field, string> mappings)
        {
            // Set base fields
            base.SetFields(mappings);

            // Handle Input-specific fields
            if (mappings.Keys.Contains(Field.DatItem_Service))
                Service = mappings[Field.DatItem_Service].AsYesNo();

            if (mappings.Keys.Contains(Field.DatItem_Tilt))
                Tilt = mappings[Field.DatItem_Tilt].AsYesNo();

            if (mappings.Keys.Contains(Field.DatItem_Players))
                Players = Sanitizer.CleanLong(mappings[Field.DatItem_Players]);

            if (mappings.Keys.Contains(Field.DatItem_Coins))
                Coins = Sanitizer.CleanLong(mappings[Field.DatItem_Coins]);

            if (Controls != null)
            {
                foreach (Control control in Controls)
                {
                    control.SetFields(mappings);
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
            if (Controls != null)
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

            // Filter on service
            if (filter.DatItem_Service.MatchesNeutral(null, Service) == false)
                return false;

            // Filter on tilt
            if (filter.DatItem_Tilt.MatchesNeutral(null, Tilt) == false)
                return false;

            // Filter on players
            if (filter.DatItem_Players.MatchesNeutral(null, Players) == false)
                return false;
            else if (filter.DatItem_Players.MatchesPositive(null, Players) == false)
                return false;
            else if (filter.DatItem_Players.MatchesNegative(null, Players) == false)
                return false;

            // Filter on coins
            if (filter.DatItem_Coins.MatchesNeutral(null, Coins) == false)
                return false;
            else if (filter.DatItem_Coins.MatchesPositive(null, Coins) == false)
                return false;
            else if (filter.DatItem_Coins.MatchesNegative(null, Coins) == false)
                return false;

            // Filter on individual controls
            if (Controls != null)
            {
                foreach (Control control in Controls)
                {
                    if (!control.PassesFilter(filter))
                        return false;
                }
            }

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
            if (fields.Contains(Field.DatItem_Service))
                Service = null;

            if (fields.Contains(Field.DatItem_Tilt))
                Tilt = null;

            if (fields.Contains(Field.DatItem_Players))
                Players = 0;

            if (fields.Contains(Field.DatItem_Coins))
                Coins = null;

            if (Controls != null)
            {
                foreach (Control control in Controls)
                {
                    control.RemoveFields(fields);
                }
            }
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

            // If we don't have a Input to replace from, ignore specific fields
            if (item.ItemType != ItemType.Input)
                return;

            // Cast for easier access
            Input newItem = item as Input;

            // Replace the fields
            if (fields.Contains(Field.DatItem_Service))
                Service = newItem.Service;

            if (fields.Contains(Field.DatItem_Tilt))
                Tilt = newItem.Tilt;

            if (fields.Contains(Field.DatItem_Players))
                Players = newItem.Players;

            if (fields.Contains(Field.DatItem_Coins))
                Coins = newItem.Coins;

            // DatItem_Control_* doesn't make sense here
            // since not every control under the other item
            // can replace every control under this item
        }

        #endregion
    }
}
