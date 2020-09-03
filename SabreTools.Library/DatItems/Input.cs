using System.Collections.Generic;
using System.Linq;
using SabreTools.Library.Filtering;
using Newtonsoft.Json;
using SabreTools.Library.Tools;

namespace SabreTools.Library.DatItems
{
    /// <summary>
    /// Represents one ListXML input
    /// </summary>
    [JsonObject("input")]
    public class Input : DatItem
    {
        #region Fields

        /// <summary>
        /// Input service ID
        /// </summary>
        [JsonProperty("service", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool? Service { get; set; }

        /// <summary>
        /// Determins if this has a tilt sensor
        /// </summary>
        [JsonProperty("tilt", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool? Tilt { get; set; }

        /// <summary>
        /// Number of players on the input
        /// </summary>
        [JsonProperty("players", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Players { get; set; } // TODO: Int32?

        /// <summary>
        /// Number of coins required
        /// </summary>
        [JsonProperty("coins", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Coins { get; set; } // TODO: Int32?

        /// <summary>
        /// Set of controls for the input
        /// </summary>
        [JsonProperty("controls", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<Control> Controls { get; set; }

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
                Players = mappings[Field.DatItem_Players];

            if (mappings.Keys.Contains(Field.DatItem_Coins))
                Coins = mappings[Field.DatItem_Coins];

            // TODO: Handle DatItem_Control*
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

                AltName = this.AltName,
                AltTitle = this.AltTitle,

                Original = this.Original,
                OpenMSXSubType = this.OpenMSXSubType,
                OpenMSXType = this.OpenMSXType,
                Remark = this.Remark,
                Boot = this.Boot,

                Part = this.Part,
                Features = this.Features,
                AreaName = this.AreaName,
                AreaSize = this.AreaSize,
                AreaWidth = this.AreaWidth,
                AreaEndianness = this.AreaEndianness,
                Value = this.Value,
                LoadFlag = this.LoadFlag,

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
            return (Service == newOther.Service
                && Tilt == newOther.Tilt
                && Players == newOther.Players
                && Coins == newOther.Coins);

            // TODO: Handle DatItem_Control*
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
            if (filter.DatItem_Players.MatchesPositiveSet(Players) == false)
                return false;
            if (filter.DatItem_Players.MatchesNegativeSet(Players) == true)
                return false;

            // Filter on coins
            if (filter.DatItem_Coins.MatchesPositiveSet(Coins) == false)
                return false;
            if (filter.DatItem_Coins.MatchesNegativeSet(Coins) == true)
                return false;

            // TODO: Handle DatItem_Control*

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
                Players = null;

            if (fields.Contains(Field.DatItem_Coins))
                Coins = null;

            if (fields.Contains(Field.DatItem_Controls))
                Controls = null;

            // TODO: Handle DatItem_Control*
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

            if (fields.Contains(Field.DatItem_Controls))
                Controls = newItem.Controls;

            // TODO: Handle DatItem_Control*
        }

        #endregion
    }
}
