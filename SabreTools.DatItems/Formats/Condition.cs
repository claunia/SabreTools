using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SabreTools.Core;
using SabreTools.Core.Tools;

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
            get => _internal.ReadString(Models.Internal.Condition.TagKey);
            set => _internal[Models.Internal.Condition.TagKey] = value;
        }

        /// <summary>
        /// Condition mask
        /// </summary>
        [JsonProperty("mask", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("mask")]
        public string? Mask
        {
            get => _internal.ReadString(Models.Internal.Condition.MaskKey);
            set => _internal[Models.Internal.Condition.MaskKey] = value;
        }

        /// <summary>
        /// Condition relationship
        /// </summary>
        [JsonProperty("relation", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("relation")]
        [JsonConverter(typeof(StringEnumConverter))]
        public Relation Relation
        {
            get => _internal.ReadString(Models.Internal.Condition.RelationKey).AsRelation();
            set => _internal[Models.Internal.Condition.RelationKey] = value.FromRelation();
        }

        [JsonIgnore]
        public bool RelationSpecified { get { return Relation != Core.Relation.NULL; } }

        /// <summary>
        /// Condition value
        /// </summary>
        [JsonProperty("value", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("value")]
        public string? Value
        {
            get => _internal.ReadString(Models.Internal.Condition.ValueKey);
            set => _internal[Models.Internal.Condition.ValueKey] = value;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Condition object
        /// </summary>
        public Condition()
        {
            _internal = new Models.Internal.Condition();
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

                _internal = this._internal?.Clone() as Models.Internal.Condition ?? new Models.Internal.Condition(),
            };
        }

        #endregion
    }
}
