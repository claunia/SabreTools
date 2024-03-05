using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.Filter;

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
            get => _internal.ReadString(Models.Metadata.DataArea.NameKey);
            set => _internal[Models.Metadata.DataArea.NameKey] = value;
        }

        /// <summary>
        /// Total size of the area
        /// </summary>
        [JsonProperty("size", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("size")]
        public long? Size
        {
            get => _internal.ReadLong(Models.Metadata.DataArea.SizeKey);
            set => _internal[Models.Metadata.DataArea.SizeKey] = value;
        }

        [JsonIgnore]
        public bool SizeSpecified { get { return Size != null; } }

        /// <summary>
        /// Word width for the area
        /// </summary>
        [JsonProperty("width", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("width")]
        public long? Width
        {
            get => _internal.ReadLong(Models.Metadata.DataArea.WidthKey);
            set => _internal[Models.Metadata.DataArea.WidthKey] = value;
        }

        [JsonIgnore]
        public bool WidthSpecified { get { return Width != null; } }

        /// <summary>
        /// Byte endianness of the area
        /// </summary>
        [JsonProperty("endianness", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("endianness")]
        public Endianness Endianness
        {
            get => _internal.ReadString(Models.Metadata.DataArea.WidthKey).AsEnumValue<Endianness>();
            set => _internal[Models.Metadata.DataArea.WidthKey] = value.AsStringValue<Endianness>();
        }

        [JsonIgnore]
        public bool EndiannessSpecified { get { return Endianness != Endianness.NULL; } }

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
            _internal = new Models.Metadata.DataArea();
            Machine = new Machine();

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

                Machine = this.Machine.Clone() as Machine ?? new Machine(),
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _internal = this._internal?.Clone() as Models.Metadata.DataArea ?? [],
            };
        }

        #endregion
    }
}
