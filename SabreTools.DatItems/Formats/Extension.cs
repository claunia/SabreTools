using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents a matchable extension
    /// </summary>
    [JsonObject("extension"), XmlRoot("extension")]
    public class Extension : DatItem
    {
        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => GetFieldValue<string>(Models.Metadata.Extension.NameKey);

        /// <inheritdoc/>
        public override void SetName(string? name) => SetFieldValue(Models.Metadata.Extension.NameKey, name);

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Extension object
        /// </summary>
        public Extension()
        {
            _internal = new Models.Metadata.Extension();

            SetName(string.Empty);
            SetFieldValue<ItemType>(Models.Metadata.DatItem.TypeKey, ItemType.Extension);
            SetFieldValue<Machine>(DatItem.MachineKey, new Machine());
        }

        /// <summary>
        /// Create an Extension object from the internal model
        /// </summary>
        public Extension(Models.Metadata.Extension item)
        {
            _internal = item;

            SetFieldValue<ItemType>(Models.Metadata.DatItem.TypeKey, ItemType.Extension);
            SetFieldValue<Machine>(DatItem.MachineKey, new Machine());
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new Extension()
            {
                _internal = this._internal?.Clone() as Models.Metadata.Extension ?? [],
            };
        }

        #endregion
    }
}
