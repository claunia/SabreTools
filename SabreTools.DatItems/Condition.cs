using System.Collections.Generic;
using System.Xml.Serialization;

using SabreTools.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SabreTools.DatItems
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

        #region Sorting and Merging

        /// <inheritdoc/>
        public override void ReplaceFields(
            DatItem item,
            List<DatItemField> datItemFields,
            List<MachineField> machineFields)
        {
            // Replace common fields first
            base.ReplaceFields(item, datItemFields, machineFields);

            // If we don't have a Condition to replace from, ignore specific fields
            if (item.ItemType != ItemType.Condition)
                return;

            // Cast for easier access
            Condition newItem = item as Condition;

            // Replace the fields
            if (datItemFields.Contains(DatItemField.Tag))
                Tag = newItem.Tag;
            else if (datItemFields.Contains(DatItemField.Condition_Tag))
                Tag = newItem.Tag;

            if (datItemFields.Contains(DatItemField.Mask))
                Mask = newItem.Mask;
            else if (datItemFields.Contains(DatItemField.Condition_Mask))
                Mask = newItem.Mask;

            if (datItemFields.Contains(DatItemField.Relation))
                Relation = newItem.Relation;
            else if (datItemFields.Contains(DatItemField.Condition_Relation))
                Relation = newItem.Relation;

            if (datItemFields.Contains(DatItemField.Value))
                Value = newItem.Value;
            else if (datItemFields.Contains(DatItemField.Condition_Value))
                Value = newItem.Value;
        }

        #endregion
    }
}
