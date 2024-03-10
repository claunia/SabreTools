using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents the sound output for a machine
    /// </summary>
    [JsonObject("sound"), XmlRoot("sound")]
    public class Sound : DatItem
    {
        #region Constructors

        /// <summary>
        /// Create a default, empty Sound object
        /// </summary>
        public Sound()
        {
            _internal = new Models.Metadata.Sound();

            SetFieldValue<ItemType>(Models.Metadata.DatItem.TypeKey, ItemType.Sound);
            SetFieldValue<Machine>(DatItem.MachineKey, new Machine());
        }

        /// <summary>
        /// Create a Sound object from the internal model
        /// </summary>
        public Sound(Models.Metadata.Sound? item)
        {
            _internal = item ?? [];

            SetFieldValue<ItemType>(Models.Metadata.DatItem.TypeKey, ItemType.Sound);
            SetFieldValue<Machine>(DatItem.MachineKey, new Machine());
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new Sound()
            {
                _internal = this._internal?.Clone() as Models.Metadata.Sound ?? [],
            };
        }

        #endregion
    }
}
