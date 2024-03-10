using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents one machine display
    /// </summary>
    [JsonObject("display"), XmlRoot("display")]
    public class Display : DatItem
    {
        #region Constructors

        /// <summary>
        /// Create a default, empty Display object
        /// </summary>
        public Display()
        {
            _internal = new Models.Metadata.Display();
            Machine = new Machine();

            ItemType = ItemType.Display;
        }

        /// <summary>
        /// Create a Display object from the internal model
        /// </summary>
        public Display(Models.Metadata.Display? item)
        {
            _internal = item ?? [];
            Machine = new Machine();

            ItemType = ItemType.Display;
        }

        /// <summary>
        /// Create a Display object from the internal model
        /// </summary>
        public Display(Models.Metadata.Video? item)
        {
            // TODO: Determine what transformation is needed here
            _internal = item ?? [];
            Machine = new Machine();

            ItemType = ItemType.Display;
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new Display()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine ?? new Machine(),
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _internal = this._internal?.Clone() as Models.Metadata.Display ?? [],
            };
        }

        #endregion
    }
}
