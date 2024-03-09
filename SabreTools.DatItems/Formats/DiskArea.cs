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
        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => GetFieldValue<string>(Models.Metadata.DiskArea.NameKey);

        /// <inheritdoc/>
        public override void SetName(string? name) => SetFieldValue(Models.Metadata.DiskArea.NameKey, name);

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty DiskArea object
        /// </summary>
        public DiskArea()
        {
            _internal = new Models.Metadata.DiskArea();
            Machine = new Machine();

            SetName(string.Empty);
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

                Machine = this.Machine.Clone() as Machine ?? new Machine(),
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _internal = this._internal?.Clone() as Models.Metadata.DiskArea ?? [],
            };
        }

        #endregion
    }
}
