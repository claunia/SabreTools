using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

using SabreTools.Core;
using SabreTools.Filtering;
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

        #region Accessors

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="mappings">Mappings dictionary</param>
        public override void SetFields(Dictionary<Field, string> mappings)
        {
            // Set base fields
            base.SetFields(mappings);

            // Handle Feature-specific fields
            if (mappings.Keys.Contains(Field.DatItem_SupportStatus))
                Status = mappings[Field.DatItem_SupportStatus].AsSupportStatus();

            if (mappings.Keys.Contains(Field.DatItem_EmulationStatus))
                Emulation = mappings[Field.DatItem_EmulationStatus].AsSupportStatus();

            if (mappings.Keys.Contains(Field.DatItem_CocktailStatus))
                Cocktail = mappings[Field.DatItem_CocktailStatus].AsSupportStatus();

            if (mappings.Keys.Contains(Field.DatItem_SaveStateStatus))
                SaveState = mappings[Field.DatItem_SaveStateStatus].AsSupported();
        }

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

            // Filter on status
            if (filter.DatItem_SupportStatus.MatchesPositive(SupportStatus.NULL, Status) == false)
                return false;
            if (filter.DatItem_SupportStatus.MatchesNegative(SupportStatus.NULL, Status) == true)
                return false;

            // Filter on emulation
            if (filter.DatItem_EmulationStatus.MatchesPositive(SupportStatus.NULL, Emulation) == false)
                return false;
            if (filter.DatItem_EmulationStatus.MatchesNegative(SupportStatus.NULL, Emulation) == true)
                return false;

            // Filter on cocktail
            if (filter.DatItem_CocktailStatus.MatchesPositive(SupportStatus.NULL, Cocktail) == false)
                return false;
            if (filter.DatItem_CocktailStatus.MatchesNegative(SupportStatus.NULL, Cocktail) == true)
                return false;

            // Filter on savestate
            if (filter.DatItem_SaveStateStatus.MatchesPositive(Supported.NULL, SaveState) == false)
                return false;
            if (filter.DatItem_SaveStateStatus.MatchesNegative(Supported.NULL, SaveState) == true)
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
            if (fields.Contains(Field.DatItem_SupportStatus))
                Status = SupportStatus.NULL;

            if (fields.Contains(Field.DatItem_EmulationStatus))
                Emulation = SupportStatus.NULL;

            if (fields.Contains(Field.DatItem_CocktailStatus))
                Cocktail = SupportStatus.NULL;

            if (fields.Contains(Field.DatItem_SaveStateStatus))
                SaveState = Supported.NULL;
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

            // If we don't have a Driver to replace from, ignore specific fields
            if (item.ItemType != ItemType.Driver)
                return;

            // Cast for easier access
            Driver newItem = item as Driver;

            // Replace the fields
            if (fields.Contains(Field.DatItem_SupportStatus))
                Status = newItem.Status;

            if (fields.Contains(Field.DatItem_EmulationStatus))
                Emulation = newItem.Emulation;

            if (fields.Contains(Field.DatItem_CocktailStatus))
                Cocktail = newItem.Cocktail;

            if (fields.Contains(Field.DatItem_SaveStateStatus))
                SaveState = newItem.SaveState;
        }

        #endregion
    }
}
