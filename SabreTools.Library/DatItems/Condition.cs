using System.Collections.Generic;
using System.Linq;

using SabreTools.Library.Filtering;
using Newtonsoft.Json;

namespace SabreTools.Library.DatItems
{
    /// <summary>
    /// Represents a condition on a machine or other item
    /// </summary>
    [JsonObject("condition")]
    public class Condition : DatItem
    {
        #region Fields

        /// <summary>
        /// Condition tag value
        /// </summary>
        [JsonProperty("tag", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Tag { get; set; }

        /// <summary>
        /// Condition mask
        /// </summary>
        [JsonProperty("mask", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Mask { get; set; }

        /// <summary>
        /// Condition relationship
        /// </summary>
        [JsonProperty("relation", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Relation { get; set; } // TODO: (eq|ne|gt|le|lt|ge)

        /// <summary>
        /// Condition value
        /// </summary>
        [JsonProperty("value", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string ConditionValue { get; set; }

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

            // Handle Condition-specific fields
            if (mappings.Keys.Contains(Field.DatItem_Tag))
                Tag = mappings[Field.DatItem_Tag];
            else if (mappings.Keys.Contains(Field.DatItem_Condition_Tag))
                Tag = mappings[Field.DatItem_Condition_Tag];

            if (mappings.Keys.Contains(Field.DatItem_Mask))
                Mask = mappings[Field.DatItem_Mask];
            else if (mappings.Keys.Contains(Field.DatItem_Condition_Mask))
                Mask = mappings[Field.DatItem_Condition_Mask];

            if (mappings.Keys.Contains(Field.DatItem_Relation))
                Relation = mappings[Field.DatItem_Relation];
            else if (mappings.Keys.Contains(Field.DatItem_Condition_Relation))
                Relation = mappings[Field.DatItem_Condition_Relation];

            if (mappings.Keys.Contains(Field.DatItem_ConditionValue))
                ConditionValue = mappings[Field.DatItem_ConditionValue];
            else if (mappings.Keys.Contains(Field.DatItem_Condition_Value))
                ConditionValue = mappings[Field.DatItem_Condition_Value];
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Condition object
        /// </summary>
        public Condition()
        {
            ItemType = ItemType.Condition;
        }

        #endregion

        #region Cloning Methods

        public override object Clone()
        {
            return new Condition()
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

                Tag = this.Tag,
                Mask = this.Mask,
                Relation = this.Relation,
                ConditionValue = this.ConditionValue,
            };
        }

        #endregion

        #region Comparision Methods

        public override bool Equals(DatItem other)
        {
            // If we don't have a Condition, return false
            if (ItemType != other.ItemType)
                return false;

            // Otherwise, treat it as a Condition
            Condition newOther = other as Condition;

            // If the Feature information matches
            return (Tag == newOther.Tag
                && Mask == newOther.Mask
                && Relation == newOther.Relation
                && ConditionValue == newOther.ConditionValue);
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
            if (filter.DatItem_Condition_Tag.MatchesPositiveSet(Tag) == false)
                return false;
            if (filter.DatItem_Condition_Tag.MatchesNegativeSet(Tag) == true)
                return false;

            // Filter on mask
            if (filter.DatItem_Mask.MatchesPositiveSet(Mask) == false)
                return false;
            if (filter.DatItem_Mask.MatchesNegativeSet(Mask) == true)
                return false;
            if (filter.DatItem_Condition_Mask.MatchesPositiveSet(Mask) == false)
                return false;
            if (filter.DatItem_Condition_Mask.MatchesNegativeSet(Mask) == true)
                return false;

            // Filter on mask
            if (filter.DatItem_Relation.MatchesPositiveSet(Relation) == false)
                return false;
            if (filter.DatItem_Relation.MatchesNegativeSet(Relation) == true)
                return false;
            if (filter.DatItem_Condition_Relation.MatchesPositiveSet(Relation) == false)
                return false;
            if (filter.DatItem_Condition_Relation.MatchesNegativeSet(Relation) == true)
                return false;

            // Filter on value
            if (filter.DatItem_ConditionValue.MatchesPositiveSet(ConditionValue) == false)
                return false;
            if (filter.DatItem_ConditionValue.MatchesNegativeSet(ConditionValue) == true)
                return false;
            if (filter.DatItem_Condition_Value.MatchesPositiveSet(ConditionValue) == false)
                return false;
            if (filter.DatItem_Condition_Value.MatchesNegativeSet(ConditionValue) == true)
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
            if (fields.Contains(Field.DatItem_Tag))
                Tag = null;
            else if (fields.Contains(Field.DatItem_Condition_Tag))
                Tag = null;

            if (fields.Contains(Field.DatItem_Mask))
                Mask = null;
            else if (fields.Contains(Field.DatItem_Condition_Mask))
                Mask = null;

            if (fields.Contains(Field.DatItem_Relation))
                Relation = null;
            else if (fields.Contains(Field.DatItem_Condition_Relation))
                Relation = null;

            if (fields.Contains(Field.DatItem_ConditionValue))
                ConditionValue = null;
            else if (fields.Contains(Field.DatItem_Condition_Value))
                ConditionValue = null;
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

            // If we don't have a Condition to replace from, ignore specific fields
            if (item.ItemType != ItemType.Condition)
                return;

            // Cast for easier access
            Condition newItem = item as Condition;

            // Replace the fields
            if (fields.Contains(Field.DatItem_Tag))
                Tag = newItem.Tag;
            else if (fields.Contains(Field.DatItem_Condition_Tag))
                Tag = newItem.Tag;

            if (fields.Contains(Field.DatItem_Mask))
                Mask = newItem.Mask;
            else if (fields.Contains(Field.DatItem_Condition_Mask))
                Mask = newItem.Mask;

            if (fields.Contains(Field.DatItem_Relation))
                Relation = newItem.Relation;
            else if (fields.Contains(Field.DatItem_Condition_Relation))
                Relation = newItem.Relation;

            if (fields.Contains(Field.DatItem_ConditionValue))
                ConditionValue = newItem.ConditionValue;
            else if (fields.Contains(Field.DatItem_Condition_Value))
                ConditionValue = newItem.ConditionValue;
        }

        #endregion
    }
}
