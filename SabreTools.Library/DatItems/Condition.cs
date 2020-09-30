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
    /// Represents a condition on a machine or other item
    /// </summary>
    [JsonObject("condition"), XmlRoot("condition")]
    public class Condition : DatItem
    {
        #region Fields

        /// <summary>
        /// Condition tag value
        /// </summary>
        [JsonProperty("tag", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("tag")]
        public string Tag { get; set; }

        /// <summary>
        /// Condition mask
        /// </summary>
        [JsonProperty("mask", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("mask")]
        public string Mask { get; set; }

        /// <summary>
        /// Condition relationship
        /// </summary>
        [JsonProperty("relation", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonConverter(typeof(StringEnumConverter))]
        [XmlElement("relation")]
        public Relation Relation { get; set; }

        [JsonIgnore]
        public bool RelationSpecified { get { return Relation != Relation.NULL; } }

        /// <summary>
        /// Condition value
        /// </summary>
        [JsonProperty("value", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("value")]
        public string Value { get; set; }

        #endregion

        #region Accessors

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="mappings">Mappings dictionary</param>
        public override void SetFields(Dictionary<Field, string> mappings)
        {
            SetFields(mappings, false);
        }

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="mappings">Mappings dictionary</param>
        /// <param name="sub">True if this is a subitem, false otherwise</param>
        public void SetFields(Dictionary<Field, string> mappings, bool sub)
        {
            // Set base fields
            base.SetFields(mappings);

            // Handle Condition-specific fields
            if (sub)
            {
                if (mappings.Keys.Contains(Field.DatItem_Condition_Tag))
                    Tag = mappings[Field.DatItem_Condition_Tag];

                if (mappings.Keys.Contains(Field.DatItem_Condition_Mask))
                    Mask = mappings[Field.DatItem_Condition_Mask];

                if (mappings.Keys.Contains(Field.DatItem_Condition_Relation))
                    Relation = mappings[Field.DatItem_Condition_Relation].AsRelation();

                if (mappings.Keys.Contains(Field.DatItem_Condition_Value))
                    Value = mappings[Field.DatItem_Condition_Value];
            }
            else
            {
                if (mappings.Keys.Contains(Field.DatItem_Tag))
                    Tag = mappings[Field.DatItem_Tag];

                if (mappings.Keys.Contains(Field.DatItem_Mask))
                    Mask = mappings[Field.DatItem_Mask];

                if (mappings.Keys.Contains(Field.DatItem_Relation))
                    Relation = mappings[Field.DatItem_Relation].AsRelation();

                if (mappings.Keys.Contains(Field.DatItem_Value))
                    Value = mappings[Field.DatItem_Value];
            }
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

                Machine = this.Machine.Clone() as Machine,
                Source = this.Source.Clone() as Source,
                Remove = this.Remove,

                Tag = this.Tag,
                Mask = this.Mask,
                Relation = this.Relation,
                Value = this.Value,
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
                && Value == newOther.Value);
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
            return PassesFilter(filter, false);
        }

        /// <summary>
        /// Check to see if a DatItem passes the filter
        /// </summary>
        /// <param name="filter">Filter to check against</param>
        /// <param name="sub">True if this is a subitem, false otherwise</param>
        /// <returns>True if the item passed the filter, false otherwise</returns>
        public bool PassesFilter(Filter filter, bool sub)
        {
            if (sub)
            {
                // Filter on tag
                if (!filter.PassStringFilter(filter.DatItem_Condition_Tag, Tag))
                    return false;

                // Filter on mask
                if (!filter.PassStringFilter(filter.DatItem_Condition_Mask, Mask))
                    return false;

                // Filter on relation
                if (filter.DatItem_Condition_Relation.MatchesPositive(Relation.NULL, Relation) == false)
                    return false;
                if (filter.DatItem_Condition_Relation.MatchesNegative(Relation.NULL, Relation) == true)
                    return false;

                // Filter on value
                if (!filter.PassStringFilter(filter.DatItem_Condition_Value, Value))
                    return false;
            }
            else
            {
                // Check common fields first
                if (!base.PassesFilter(filter))
                    return false;

                // Filter on tag
                if (!filter.PassStringFilter(filter.DatItem_Tag, Tag))
                    return false;

                // Filter on mask
                if (!filter.PassStringFilter(filter.DatItem_Mask, Mask))
                    return false;

                // Filter on relation
                if (filter.DatItem_Relation.MatchesPositive(Relation.NULL, Relation) == false)
                    return false;
                if (filter.DatItem_Relation.MatchesNegative(Relation.NULL, Relation) == true)
                    return false;

                // Filter on value
                if (!filter.PassStringFilter(filter.DatItem_Value, Value))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Remove fields from the DatItem
        /// </summary>
        /// <param name="fields">List of Fields to remove</param>
        public override void RemoveFields(List<Field> fields)
        {
            RemoveFields(fields, false);
        }

        /// <summary>
        /// Remove fields from the DatItem
        /// </summary>
        /// <param name="fields">List of Fields to remove</param>
        /// <param name="sub">True if this is a subitem, false otherwise</param>
        public void RemoveFields(List<Field> fields, bool sub)
        {
            // Remove common fields first
            base.RemoveFields(fields);

            // Remove the fields
            if (sub)
            {
                if (fields.Contains(Field.DatItem_Condition_Tag))
                    Tag = null;

                if (fields.Contains(Field.DatItem_Condition_Mask))
                    Mask = null;

                if (fields.Contains(Field.DatItem_Condition_Relation))
                    Relation = Relation.NULL;

                if (fields.Contains(Field.DatItem_Condition_Value))
                    Value = null;
            }
            else
            {
                if (fields.Contains(Field.DatItem_Tag))
                    Tag = null;

                if (fields.Contains(Field.DatItem_Mask))
                    Mask = null;

                if (fields.Contains(Field.DatItem_Relation))
                    Relation = Relation.NULL;

                if (fields.Contains(Field.DatItem_Value))
                    Value = null;
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

            if (fields.Contains(Field.DatItem_Value))
                Value = newItem.Value;
            else if (fields.Contains(Field.DatItem_Condition_Value))
                Value = newItem.Value;
        }

        #endregion
    }
}
