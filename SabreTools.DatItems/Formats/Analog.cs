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
            Machine = new Machine();

            ItemType = ItemType.Analog;
        }

        /// <summary>
        /// Create an Analog object from the internal model
        /// </summary>
        public Analog(Models.Metadata.Analog? item)
        {
            _internal = item ?? [];
            Machine = new Machine();

            ItemType = ItemType.Analog;
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new Analog()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine ?? new Machine(),
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _internal = this._internal?.Clone() as Models.Metadata.Analog ?? [],
            };
        }

        #endregion
    }
}
