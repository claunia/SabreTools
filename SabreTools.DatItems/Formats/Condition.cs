using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.Filter;

namespace SabreTools.DatItems.Formats
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
        [JsonProperty("tag", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("tag")]
        public string? Tag
        {
            get => _internal.ReadString(Models.Metadata.Condition.TagKey);
            set => _internal[Models.Metadata.Condition.TagKey] = value;
        }

        /// <summary>
        /// Condition mask
        /// </summary>
        [JsonProperty("mask", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("mask")]
        public string? Mask
        {
            get => _internal.ReadString(Models.Metadata.Condition.MaskKey);
            set => _internal[Models.Metadata.Condition.MaskKey] = value;
        }

        /// <summary>
        /// Condition relationship
        /// </summary>
        [JsonProperty("relation", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("relation")]
        [JsonConverter(typeof(StringEnumConverter))]
        public Relation Relation
        {
            get => _internal.ReadString(Models.Metadata.Condition.RelationKey).AsEnumValue<Relation>();
            set => _internal[Models.Metadata.Condition.RelationKey] = value.AsStringValue<Relation>();
        }

        [JsonIgnore]
        public bool RelationSpecified { get { return Relation != Core.Relation.NULL; } }

        /// <summary>
        /// Condition value
        /// </summary>
        [JsonProperty("value", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("value")]
        public string? Value
        {
            get => _internal.ReadString(Models.Metadata.Condition.ValueKey);
            set => _internal[Models.Metadata.Condition.ValueKey] = value;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Condition object
        /// </summary>
        public Condition()
        {
            _internal = new Models.Metadata.Condition();
            Machine = new Machine();

            ItemType = ItemType.Condition;
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new Condition()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine ?? new Machine(),
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _internal = this._internal?.Clone() as Models.Metadata.Condition ?? [],
            };
        }

        #endregion
    
        #region Manipulation

        /// <inheritdoc/>
        public override bool RemoveField(DatItemField datItemField)
        {
            // Get the correct internal field name
            string? fieldName = datItemField switch
            {
                DatItemField.Mask => Models.Metadata.Condition.MaskKey,
                DatItemField.Condition_Mask => Models.Metadata.Condition.MaskKey,
                DatItemField.Relation => Models.Metadata.Condition.RelationKey,
                DatItemField.Condition_Relation => Models.Metadata.Condition.RelationKey,
                DatItemField.Tag => Models.Metadata.Condition.TagKey,
                DatItemField.Condition_Tag => Models.Metadata.Condition.TagKey,
                DatItemField.Value => Models.Metadata.Condition.ValueKey,
                DatItemField.Condition_Value => Models.Metadata.Condition.ValueKey,
                _ => null,
            };

            // Remove the field and return
            return FieldManipulator.RemoveField(_internal, fieldName);
        }

        /// <inheritdoc/>
        public override bool SetField(DatItemField datItemField, string value)
        {
           // Get the correct internal field name
            string? fieldName = datItemField switch
            {
                DatItemField.Mask => Models.Metadata.Condition.MaskKey,
                DatItemField.Condition_Mask => Models.Metadata.Condition.MaskKey,
                DatItemField.Relation => Models.Metadata.Condition.RelationKey,
                DatItemField.Condition_Relation => Models.Metadata.Condition.RelationKey,
                DatItemField.Tag => Models.Metadata.Condition.TagKey,
                DatItemField.Condition_Tag => Models.Metadata.Condition.TagKey,
                DatItemField.Value => Models.Metadata.Condition.ValueKey,
                DatItemField.Condition_Value => Models.Metadata.Condition.ValueKey,
                _ => null,
            };

            // Set the field and return
            return FieldManipulator.SetField(_internal, fieldName, value);
        }

        #endregion
    }
}
