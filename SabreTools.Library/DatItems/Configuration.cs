using System.Collections.Generic;
using System.Linq;

using SabreTools.Library.Filtering;
using Newtonsoft.Json;

namespace SabreTools.Library.DatItems
{
    /// <summary>
    /// Represents which Configuration(s) is associated with a set
    /// </summary>
    [JsonObject("configuration")]
    public class Configuration : DatItem
    {
        #region Fields

        /// <summary>
        /// Tag associated with the configuration
        /// </summary>
        [JsonProperty("tag", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Tag { get; set; }

        /// <summary>
        /// Mask associated with the configuration
        /// </summary>
        [JsonProperty("mask", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Mask { get; set; }

        /// <summary>
        /// Conditions associated with the configuration
        /// </summary>
        [JsonProperty("conditions")]
        public List<Condition> Conditions { get; set; }

        /// <summary>
        /// Locations associated with the configuration
        /// </summary>
        [JsonProperty("locations")]
        public List<Location> Locations { get; set; }

        /// <summary>
        /// Settings associated with the configuration
        /// </summary>
        [JsonProperty("settings")]
        public List<Setting> Settings { get; set; }

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

            // Handle Configuration-specific fields
            if (mappings.Keys.Contains(Field.DatItem_Tag))
                Tag = mappings[Field.DatItem_Tag];

            if (mappings.Keys.Contains(Field.DatItem_Mask))
                Mask = mappings[Field.DatItem_Mask];

            // TODO: Handle DatItem_Condition*
            // TODO: Handle DatItem_Location*
            // TODO: Handle DatItem_Setting*
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Configuration object
        /// </summary>
        public Configuration()
        {
            Name = string.Empty;
            ItemType = ItemType.Configuration;
        }

        #endregion

        #region Cloning Methods

        public override object Clone()
        {
            return new Configuration()
            {
                Name = this.Name,
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

                Tag = this.Tag,
                Mask = this.Mask,
                Conditions = this.Conditions,
                Locations = this.Locations,
                Settings = this.Settings,
            };
        }

        #endregion

        #region Comparision Methods

        public override bool Equals(DatItem other)
        {
            // If we don't have a Configuration, return false
            if (ItemType != other.ItemType)
                return false;

            // Otherwise, treat it as a Configuration
            Configuration newOther = other as Configuration;

            // If the Configuration information matches
            return (Name == newOther.Name && Tag == newOther.Tag && Mask == newOther.Mask);
            
            // TODO: Handle DatItem_Condition*
            // TODO: Handle DatItem_Location*
            // TODO: Handle DatItem_Setting*
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

            // Filter on tag
            if (filter.DatItem_Tag.MatchesPositiveSet(Tag) == false)
                return false;
            if (filter.DatItem_Tag.MatchesNegativeSet(Tag) == true)
                return false;

            // Filter on mask
            if (filter.DatItem_Mask.MatchesPositiveSet(Mask) == false)
                return false;
            if (filter.DatItem_Mask.MatchesNegativeSet(Mask) == true)
                return false;

            // TODO: Handle DatItem_Condition*
            // TODO: Handle DatItem_Location*
            // TODO: Handle DatItem_Setting*

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
            if (fields.Contains(Field.DatItem_Tag))
                Tag = null;

            if (fields.Contains(Field.DatItem_Mask))
                Mask = null;

            if (fields.Contains(Field.DatItem_Conditions))
                Conditions = null;

            if (fields.Contains(Field.DatItem_Locations))
                Locations = null;

            if (fields.Contains(Field.DatItem_Settings))
                Settings = null;

            // TODO: Handle DatItem_Condition*
            // TODO: Handle DatItem_Location*
            // TODO: Handle DatItem_Setting*
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

            // If we don't have a Configuration to replace from, ignore specific fields
            if (item.ItemType != ItemType.Configuration)
                return;

            // Cast for easier access
            Configuration newItem = item as Configuration;

            // Replace the fields
            if (fields.Contains(Field.DatItem_Tag))
                Tag = newItem.Tag;

            if (fields.Contains(Field.DatItem_Mask))
                Mask = newItem.Mask;

            if (fields.Contains(Field.DatItem_Conditions))
                Conditions = newItem.Conditions;

            if (fields.Contains(Field.DatItem_Locations))
                Locations = newItem.Locations;

            if (fields.Contains(Field.DatItem_Settings))
                Settings = newItem.Settings;

            // TODO: Handle DatItem_Condition*
            // TODO: Handle DatItem_Location*
            // TODO: Handle DatItem_Setting*
        }

        #endregion
    }
}
