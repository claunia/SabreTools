using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents one part feature object
    /// </summary>
    [JsonObject("part_feature"), XmlRoot("part_feature")]
    public class PartFeature : DatItem
    {
        #region Fields

        /// <summary>
        /// Name of the item
        /// </summary>
        [JsonProperty("name"), XmlElement("name")]
        public string? Name
        {
            get => _feature.ReadString(Models.Internal.Feature.NameKey);
            set => _feature[Models.Internal.Feature.NameKey] = value;
        }

        /// <summary>
        /// PartFeature value
        /// </summary>
        [JsonProperty("value"), XmlElement("value")]
        public string? Value
        {
            get => _feature.ReadString(Models.Internal.Feature.ValueKey);
            set => _feature[Models.Internal.Feature.ValueKey] = value;
        }

        /// <summary>
        /// Internal Feature model
        /// </summary>
        [JsonIgnore]
        private Models.Internal.Feature _feature = new();

        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => Name;

        /// <inheritdoc/>
        public override void SetName(string? name) => Name = name;

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty PartFeature object
        /// </summary>
        public PartFeature()
        {
            Name = string.Empty;
            ItemType = ItemType.PartFeature;
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new PartFeature()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine?.Clone() as Machine,
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _feature = this._feature?.Clone() as Models.Internal.Feature ?? new Models.Internal.Feature(),
            };
        }

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
            // If we don't have a PartFeature, return false
            if (ItemType != other?.ItemType || other is not PartFeature otherInternal)
                return false;

            // Compare the internal models
            return _feature.EqualTo(otherInternal._feature);
        }

        #endregion
    }
}
