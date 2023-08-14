using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents one shared feature object
    /// </summary>
    [JsonObject("sharedfeat"), XmlRoot("sharedfeat")]
    public class SharedFeature : DatItem
    {
        #region Fields

        /// <summary>
        /// Name of the item
        /// </summary>
        [JsonProperty("name"), XmlElement("name")]
        public string? Name
        {
            get => _sharedFeat.ReadString(Models.Internal.SharedFeat.NameKey);
            set => _sharedFeat[Models.Internal.SharedFeat.NameKey] = value;
        }

        /// <summary>
        /// SharedFeature value
        /// </summary>
        [JsonProperty("value"), XmlElement("value")]
        public string? Value
        {
            get => _sharedFeat.ReadString(Models.Internal.SharedFeat.ValueKey);
            set => _sharedFeat[Models.Internal.SharedFeat.ValueKey] = value;
        }

        /// <summary>
        /// Internal SharedFeat model
        /// </summary>
        [JsonIgnore]
        private Models.Internal.SharedFeat _sharedFeat = new();

        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => Name;

        /// <inheritdoc/>
        public override void SetName(string? name) => Name = name;

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty SharedFeature object
        /// </summary>
        public SharedFeature()
        {
            Name = string.Empty;
            ItemType = ItemType.SharedFeature;
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new SharedFeature()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine?.Clone() as Machine,
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _sharedFeat = this._sharedFeat?.Clone() as Models.Internal.SharedFeat ?? new Models.Internal.SharedFeat(),
            };
        }

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
            // If we don't have a SharedFeature, return false
            if (ItemType != other?.ItemType || other is not SharedFeature otherInternal)
                return false;

            // Compare the internal models
            return _sharedFeat.EqualTo(otherInternal._sharedFeat);
        }

        #endregion
    }
}
