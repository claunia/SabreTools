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

            SetName(string.Empty);
            SetFieldValue<ItemType>(Models.Metadata.DatItem.TypeKey, ItemType.DiskArea);
            SetFieldValue<Machine>(DatItem.MachineKey, new Machine());
        }

        /// <summary>
        /// Create a DiskArea object from the internal model
        /// </summary>
        public DiskArea(Models.Metadata.DiskArea? item)
        {
            _internal = item ?? [];

            SetFieldValue<ItemType>(Models.Metadata.DatItem.TypeKey, ItemType.DiskArea);
            SetFieldValue<Machine>(DatItem.MachineKey, new Machine());
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new DiskArea()
            {
                _internal = this._internal?.Clone() as Models.Metadata.DiskArea ?? [],
            };
        }

        #endregion
    }
}
