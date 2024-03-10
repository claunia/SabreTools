using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents a single analog item
    /// </summary>
    [JsonObject("analog"), XmlRoot("analog")]
    public class Analog : DatItem
    {
        #region Constructors

        /// <summary>
        /// Create a default, empty Analog object
        /// </summary>
        public Analog()
        {
            _internal = new Models.Metadata.Analog();

            SetFieldValue<ItemType>(Models.Metadata.DatItem.TypeKey, ItemType.Analog);
            SetFieldValue<Machine>(DatItem.MachineKey, new Machine());
        }

        /// <summary>
        /// Create an Analog object from the internal model
        /// </summary>
        public Analog(Models.Metadata.Analog? item)
        {
            _internal = item ?? [];

            SetFieldValue<ItemType>(Models.Metadata.DatItem.TypeKey, ItemType.Analog);
            SetFieldValue<Machine>(DatItem.MachineKey, new Machine());
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new Analog()
            {
                _internal = this._internal?.Clone() as Models.Metadata.Analog ?? [],
            };
        }

        #endregion
    }
}
