using System.Collections.Generic;
using System.Linq;

using SabreTools.Library.Filtering;
using Newtonsoft.Json;
using SabreTools.Library.Tools;

namespace SabreTools.Library.DatItems
{
    /// <summary>
    /// Represents the a feature of the machine
    /// </summary>
    [JsonObject("feature")]
    public class Feature : DatItem
    {
        #region Fields

        /// <summary>
        /// Type of feature
        /// </summary>
        [JsonProperty("type")]
        public FeatureType Type { get; set; }

        /// <summary>
        /// Emulation status
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; } // TODO: (unemulated|imperfect)

        /// <summary>
        /// Overall status
        /// </summary>
        [JsonProperty("overall")]
        public string Overall { get; set; } // TODO: (unemulated|imperfect)

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
            if (mappings.Keys.Contains(Field.DatItem_FeatureType))
                Type = mappings[Field.DatItem_FeatureType].AsFeatureType();

            if (mappings.Keys.Contains(Field.DatItem_FeatureStatus))
                Status = mappings[Field.DatItem_FeatureStatus];

            if (mappings.Keys.Contains(Field.DatItem_FeatureOverall))
                Overall = mappings[Field.DatItem_FeatureOverall];
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Feature object
        /// </summary>
        public Feature()
        {
            ItemType = ItemType.Feature;
        }

        #endregion

        #region Cloning Methods

        public override object Clone()
        {
            return new Feature()
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

                Type = this.Type,
                Status = this.Status,
                Overall = this.Overall,
            };
        }

        #endregion

        #region Comparision Methods

        public override bool Equals(DatItem other)
        {
            // If we don't have a Feature, return false
            if (ItemType != other.ItemType)
                return false;

            // Otherwise, treat it as a Feature
            Feature newOther = other as Feature;

            // If the Feature information matches
            return (Type == newOther.Type && Status == newOther.Status && Overall == newOther.Overall);
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

            // Filter on type
            if (filter.DatItem_FeatureType.MatchesPositive(FeatureType.NULL, Type) == false)
                return false;
            if (filter.DatItem_FeatureType.MatchesNegative(FeatureType.NULL, Type) == true)
                return false;

            // Filter on status
            if (filter.DatItem_FeatureStatus.MatchesPositiveSet(Status) == false)
                return false;
            if (filter.DatItem_FeatureStatus.MatchesNegativeSet(Status) == true)
                return false;

            // Filter on overall
            if (filter.DatItem_FeatureOverall.MatchesPositiveSet(Overall) == false)
                return false;
            if (filter.DatItem_FeatureOverall.MatchesNegativeSet(Overall) == true)
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
            if (fields.Contains(Field.DatItem_FeatureType))
                Type = FeatureType.NULL;

            if (fields.Contains(Field.DatItem_FeatureStatus))
                Status = null;

            if (fields.Contains(Field.DatItem_FeatureOverall))
                Overall = null;
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

            // If we don't have a Feature to replace from, ignore specific fields
            if (item.ItemType != ItemType.Feature)
                return;

            // Cast for easier access
            Feature newItem = item as Feature;

            // Replace the fields
            if (fields.Contains(Field.DatItem_FeatureType))
                Type = newItem.Type;

            if (fields.Contains(Field.DatItem_FeatureStatus))
                Status = newItem.Status;

            if (fields.Contains(Field.DatItem_FeatureOverall))
                Overall = newItem.Overall;
        }

        #endregion
    }
}
