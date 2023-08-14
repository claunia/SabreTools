using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;
using SabreTools.Core.Tools;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// SoftwareList dataarea information
    /// </summary>
    /// <remarks>One DataArea can contain multiple Rom items</remarks>
    [JsonObject("dataarea"), XmlRoot("dataarea")]
    public class DataArea : DatItem
    {
        #region Fields

        /// <summary>
        /// Name of the item
        /// </summary>
        [JsonProperty("name", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("name")]
        public string? Name
        {
            get => _dataArea.ReadString(Models.Internal.DataArea.NameKey);
            set => _dataArea[Models.Internal.DataArea.NameKey] = value;
        }

        /// <summary>
        /// Total size of the area
        /// </summary>
        [JsonProperty("size", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("size")]
        public long? Size
        {
            get => _dataArea.ReadLong(Models.Internal.DataArea.SizeKey);
            set => _dataArea[Models.Internal.DataArea.SizeKey] = value;
        }

        [JsonIgnore]
        public bool SizeSpecified { get { return Size != null; } }

        /// <summary>
        /// Word width for the area
        /// </summary>
        [JsonProperty("width", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("width")]
        public long? Width
        {
            get => _dataArea.ReadLong(Models.Internal.DataArea.WidthKey);
            set => _dataArea[Models.Internal.DataArea.WidthKey] = value;
        }

        [JsonIgnore]
        public bool WidthSpecified { get { return Width != null; } }

        /// <summary>
        /// Byte endianness of the area
        /// </summary>
        [JsonProperty("endianness", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("endianness")]
        public Endianness Endianness
        {
            get => _dataArea.ReadString(Models.Internal.DataArea.WidthKey).AsEndianness();
            set => _dataArea[Models.Internal.DataArea.WidthKey] = value.FromEndianness();
        }

        [JsonIgnore]
        public bool EndiannessSpecified { get { return Endianness != Endianness.NULL; } }

        /// <summary>
        /// Internal DataArea model
        /// </summary>
        [JsonIgnore]
        private Models.Internal.DataArea _dataArea = new();

        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => Name;

        /// <inheritdoc/>
        public override void SetName(string? name) => Name = name;

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty DataArea object
        /// </summary>
        public DataArea()
        {
            Name = string.Empty;
            ItemType = ItemType.DataArea;
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new DataArea()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine?.Clone() as Machine,
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _dataArea = this._dataArea?.Clone() as Models.Internal.DataArea ?? new Models.Internal.DataArea(),
            };
        }

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
            // If we don't have a DataArea, return false
            if (ItemType != other?.ItemType || other is not DataArea otherInternal)
                return false;

            // Compare the internal models
            return _dataArea.EqualTo(otherInternal._dataArea);
        }

        #endregion
    }
}
