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
            Machine = new Machine();

            ItemType = ItemType.Sound;
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new Sound()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine ?? new Machine(),
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _internal = this._internal?.Clone() as Models.Metadata.Sound ?? [],
            };
        }

        #endregion
    }
}
