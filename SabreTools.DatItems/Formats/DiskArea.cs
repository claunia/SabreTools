using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// SoftwareList diskarea information
    /// </summary>
    /// <remarks>One DiskArea can contain multiple Disk items</remarks>
    [JsonObject("diskarea"), XmlRoot("diskarea")]
    public class DiskArea : DatItem
    {
        #region Fields

        /// <summary>
        /// Name of the item
        /// </summary>
        [JsonProperty("name", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("name")]
        public string? Name
        {
            get => _diskArea.ReadString(Models.Internal.DiskArea.NameKey);
            set => _diskArea[Models.Internal.DiskArea.NameKey] = value;
        }

        /// <summary>
        /// Internal DiskArea model
        /// </summary>
        [JsonIgnore]
        private Models.Internal.DiskArea _diskArea = new();

        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => Name;

        /// <inheritdoc/>
        public override void SetName(string? name) => Name = name;

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty DiskArea object
        /// </summary>
        public DiskArea()
        {
            Name = string.Empty;
            ItemType = ItemType.DiskArea;
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new DiskArea()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine?.Clone() as Machine,
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _diskArea = this._diskArea?.Clone() as Models.Internal.DiskArea ?? new Models.Internal.DiskArea(),
            };
        }

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
            // If we don't have a DiskArea, return false
            if (ItemType != other?.ItemType || other is not DiskArea otherInternal)
                return false;

            // Compare the internal models
            return _diskArea.EqualTo(otherInternal._diskArea);
        }

        #endregion
    }
}
