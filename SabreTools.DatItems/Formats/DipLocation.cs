using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents one diplocation
    /// </summary>
    [JsonObject("diplocation"), XmlRoot("diplocation")]
    public class DipLocation : DatItem
    {
        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => GetFieldValue<string>(Models.Metadata.DipLocation.NameKey);

        /// <inheritdoc/>
        public override void SetName(string? name) => SetFieldValue(Models.Metadata.DipLocation.NameKey, name);

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty DipLocation object
        /// </summary>
        public DipLocation()
        {
            _internal = new Models.Metadata.DipLocation();

            SetName(string.Empty);
            SetFieldValue<ItemType>(Models.Metadata.DatItem.TypeKey, ItemType.DipLocation);
            SetFieldValue<Machine>(DatItem.MachineKey, new Machine());
        }

        /// <summary>
        /// Create a DipLocation object from the internal model
        /// </summary>
        public DipLocation(Models.Metadata.DipLocation? item)
        {
            _internal = item ?? [];

            SetFieldValue<ItemType>(Models.Metadata.DatItem.TypeKey, ItemType.DipLocation);
            SetFieldValue<Machine>(DatItem.MachineKey, new Machine());
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new DipLocation()
            {
                _internal = this._internal?.Clone() as Models.Metadata.DipLocation ?? [],
            };
        }

        #endregion
    }
}
