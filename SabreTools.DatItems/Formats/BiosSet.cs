using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents which BIOS(es) is associated with a set
    /// </summary>
    [JsonObject("biosset"), XmlRoot("biosset")]
    public class BiosSet : DatItem
    {
        #region Fields

        /// <summary>
        /// Name of the item
        /// </summary>
        [JsonProperty("name"), XmlElement("name")]
        public string? Name
        {
            get => _biosSet.ReadString(Models.Internal.BiosSet.NameKey);
            set => _biosSet[Models.Internal.BiosSet.NameKey] = value;
        }

        /// <summary>
        /// Description of the BIOS
        /// </summary>
        [JsonProperty("description", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("description")]
        public string? Description
        {
            get => _biosSet.ReadString(Models.Internal.BiosSet.DescriptionKey);
            set => _biosSet[Models.Internal.BiosSet.DescriptionKey] = value;
        }

        /// <summary>
        /// Determine whether the BIOS is default
        /// </summary>
        [JsonProperty("default", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("default")]
        public bool? Default
        {
            get => _biosSet.ReadBool(Models.Internal.BiosSet.DefaultKey);
            set => _biosSet[Models.Internal.BiosSet.DefaultKey] = value;
        }

        [JsonIgnore]
        public bool DefaultSpecified { get { return Default != null; } }

        /// <summary>
        /// Internal BiosSet model
        /// </summary>
        [JsonIgnore]
        private Models.Internal.BiosSet _biosSet = new();

        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => Name;

        /// <inheritdoc/>
        public override void SetName(string? name) => Name = name;

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty BiosSet object
        /// </summary>
        public BiosSet()
        {
            Name = string.Empty;
            ItemType = ItemType.BiosSet;
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new BiosSet()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine?.Clone() as Machine,
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _biosSet = this._biosSet?.Clone() as Models.Internal.BiosSet ?? new Models.Internal.BiosSet(),
            };
        }

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
            // If we don't have a BiosSet, return false
            if (ItemType != other?.ItemType || other is not BiosSet otherInternal)
                return false;

            // Compare the internal models
            return _biosSet.EqualTo(otherInternal._biosSet);
        }

        #endregion
    }
}
