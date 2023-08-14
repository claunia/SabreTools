using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents which RAM option(s) is associated with a set
    /// </summary>
    [JsonObject("ramoption"), XmlRoot("ramoption")]
    public class RamOption : DatItem
    {
        #region Fields

        /// <summary>
        /// Name of the item
        /// </summary>
        [JsonProperty("name"), XmlElement("name")]
        public string? Name
        {
            get => _ramOption.ReadString(Models.Internal.RamOption.NameKey);
            set => _ramOption[Models.Internal.RamOption.NameKey] = value;
        }

        /// <summary>
        /// Determine whether the RamOption is default
        /// </summary>
        [JsonProperty("default", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("default")]
        public bool? Default
        {
            get => _ramOption.ReadBool(Models.Internal.RamOption.DefaultKey);
            set => _ramOption[Models.Internal.RamOption.DefaultKey] = value;
        }

        [JsonIgnore]
        public bool DefaultSpecified { get { return Default != null; } }

        /// <summary>
        /// Determines the content of the RamOption
        /// </summary>
        [JsonProperty("content", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("content")]
        public string? Content
        {
            get => _ramOption.ReadString(Models.Internal.RamOption.ContentKey);
            set => _ramOption[Models.Internal.RamOption.ContentKey] = value;
        }

        /// <summary>
        /// Internal RamOption model
        /// </summary>
        [JsonIgnore]
        private Models.Internal.RamOption _ramOption = new();

        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => Name;

        /// <inheritdoc/>
        public override void SetName(string? name) => Name = name;

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty RamOption object
        /// </summary>
        public RamOption()
        {
            Name = string.Empty;
            ItemType = ItemType.RamOption;
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new RamOption()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine?.Clone() as Machine,
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _ramOption = this._ramOption?.Clone() as Models.Internal.RamOption ?? new Models.Internal.RamOption(),
            };
        }

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
           // If we don't have a RamOption, return false
            if (ItemType != other?.ItemType || other is not RamOption otherInternal)
                return false;

            // Compare the internal models
            return _ramOption.EqualTo(otherInternal._ramOption);
        }

        #endregion
    }
}
