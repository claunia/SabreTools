using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents one conflocation
    /// </summary>
    [JsonObject("conflocation"), XmlRoot("conflocation")]
    public class ConfLocation : DatItem
    {
        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => GetFieldValue<string>(Models.Metadata.ConfLocation.NameKey);

        /// <inheritdoc/>
        public override void SetName(string? name) => SetFieldValue(Models.Metadata.ConfLocation.NameKey, name);

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty ConfLocation object
        /// </summary>
        public ConfLocation()
        {
            _internal = new Models.Metadata.ConfLocation();

            SetName(string.Empty);
            SetFieldValue<ItemType>(Models.Metadata.DatItem.TypeKey, ItemType.ConfLocation);
            SetFieldValue<Machine>(DatItem.MachineKey, new Machine());
        }

        /// <summary>
        /// Create a ConfLocation object from the internal model
        /// </summary>
        public ConfLocation(Models.Metadata.ConfLocation item)
        {
            _internal = item;

            SetFieldValue<ItemType>(Models.Metadata.DatItem.TypeKey, ItemType.ConfLocation);
            SetFieldValue<Machine>(DatItem.MachineKey, new Machine());
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new ConfLocation()
            {
                _internal = this._internal?.Clone() as Models.Metadata.ConfLocation ?? [],
            };
        }

        #endregion
    }
}
