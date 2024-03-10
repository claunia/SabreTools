using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// SoftwareList dataarea information
    /// </summary>
    /// <remarks>One DataArea can contain multiple Rom items</remarks>
    [JsonObject("dataarea"), XmlRoot("dataarea")]
    public class DataArea : DatItem
    {
        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => GetFieldValue<string>(Models.Metadata.DataArea.NameKey);

        /// <inheritdoc/>
        public override void SetName(string? name) => SetFieldValue(Models.Metadata.DataArea.NameKey, name);

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty DataArea object
        /// </summary>
        public DataArea()
        {
            _internal = new Models.Metadata.DataArea();

            SetName(string.Empty);
            SetFieldValue<ItemType>(Models.Metadata.DatItem.TypeKey, ItemType.DataArea);
            SetFieldValue<Machine>(DatItem.MachineKey, new Machine());
        }

        /// <summary>
        /// Create a DataArea object from the internal model
        /// </summary>
        public DataArea(Models.Metadata.DataArea item)
        {
            _internal = item;

            SetFieldValue<ItemType>(Models.Metadata.DatItem.TypeKey, ItemType.DataArea);
            SetFieldValue<Machine>(DatItem.MachineKey, new Machine());
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new DataArea()
            {
                _internal = this._internal?.Clone() as Models.Metadata.DataArea ?? [],
            };
        }

        #endregion
    }
}
