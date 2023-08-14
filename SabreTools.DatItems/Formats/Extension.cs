using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents a matchable extension
    /// </summary>
    [JsonObject("extension"), XmlRoot("extension")]
    public class Extension : DatItem
    {
        #region Fields

        /// <summary>
        /// Name of the item
        /// </summary>
        [JsonProperty("name"), XmlElement("name")]
        public string? Name
        {
            get => _extension.ReadString(Models.Internal.Extension.NameKey);
            set => _extension[Models.Internal.Extension.NameKey] = value;
        }

        /// <summary>
        /// Internal Extension model
        /// </summary>
        [JsonIgnore]
        private Models.Internal.Extension _extension = new();

        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => Name;

        /// <inheritdoc/>
        public override void SetName(string? name) => Name = name;

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Extension object
        /// </summary>
        public Extension()
        {
            Name = string.Empty;
            ItemType = ItemType.Extension;
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new Extension()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine?.Clone() as Machine,
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _extension = this._extension?.Clone() as Models.Internal.Extension ?? new Models.Internal.Extension(),
            };
        }

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
            // If we don't have a Adjuster, return false
            if (ItemType != other?.ItemType || other is not Extension otherInternal)
                return false;

            // Compare the internal models
            return _extension.EqualTo(otherInternal._extension);
        }

        #endregion
    }
}
