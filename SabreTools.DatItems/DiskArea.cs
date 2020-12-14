using System.Xml.Serialization;

using SabreTools.Core;
using Newtonsoft.Json;

namespace SabreTools.DatItems
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
        [JsonProperty("name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("name")]
        public string Name { get; set; }

        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override string GetName() => Name;

        /// <inheritdoc/>
        public override void SetName(string name) => Name = name;

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

        public override object Clone()
        {
            return new DiskArea()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine,
                Source = this.Source.Clone() as Source,
                Remove = this.Remove,

                Name = this.Name,
            };
        }

        #endregion

        #region Comparision Methods

        public override bool Equals(DatItem other)
        {
            // If we don't have a DiskArea, return false
            if (ItemType != other.ItemType)
                return false;

            // Otherwise, treat it as a DiskArea
            DiskArea newOther = other as DiskArea;

            // If the DiskArea information matches
            return (Name == newOther.Name);
        }

        #endregion
    }
}
