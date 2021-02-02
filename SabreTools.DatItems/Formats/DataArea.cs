using System.Xml.Serialization;

using SabreTools.Core;
using Newtonsoft.Json;

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
        [JsonProperty("name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// Total size of the area
        /// </summary>
        [JsonProperty("size", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("size")]
        public long? Size { get; set; }

        [JsonIgnore]
        public bool SizeSpecified { get { return Size != null; } }

        /// <summary>
        /// Word width for the area
        /// </summary>
        [JsonProperty("width", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("width")]
        public long? Width { get; set; }

        [JsonIgnore]
        public bool WidthSpecified { get { return Width != null; } }

        /// <summary>
        /// Byte endianness of the area
        /// </summary>
        [JsonProperty("endianness", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("endianness")]
        public Endianness Endianness { get; set; }

        [JsonIgnore]
        public bool EndiannessSpecified { get { return Endianness != Endianness.NULL; } }

        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override string GetName() => Name;

        /// <inheritdoc/>
        public override void SetName(string name) => Name = name;

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

        public override object Clone()
        {
            return new DataArea()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine,
                Source = this.Source.Clone() as Source,
                Remove = this.Remove,

                Name = this.Name,
                Size = this.Size,
                Width = this.Width,
                Endianness = this.Endianness,
            };
        }

        #endregion

        #region Comparision Methods

        public override bool Equals(DatItem other)
        {
            // If we don't have a DataArea, return false
            if (ItemType != other.ItemType)
                return false;

            // Otherwise, treat it as a DataArea
            DataArea newOther = other as DataArea;

            // If the DataArea information matches
            return (Name == newOther.Name
                && Size == newOther.Size
                && Width == newOther.Width
                && Endianness == newOther.Endianness);
        }

        #endregion
    }
}
