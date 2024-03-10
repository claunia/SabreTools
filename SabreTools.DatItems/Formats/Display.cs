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

            SetFieldValue<ItemType>(Models.Metadata.DatItem.TypeKey, ItemType.Display);
            SetFieldValue<Machine>(DatItem.MachineKey, new Machine());
        }

        /// <summary>
        /// Create a Display object from the internal model
        /// </summary>
        public Display(Models.Metadata.Display item)
        {
            _internal = item;

            SetFieldValue<ItemType>(Models.Metadata.DatItem.TypeKey, ItemType.Display);
            SetFieldValue<Machine>(DatItem.MachineKey, new Machine());
        }

        /// <summary>
        /// Create a Display object from the internal model
        /// </summary>
        public Display(Models.Metadata.Video item)
        {
            // TODO: Determine what transformation is needed here
            _internal = item;

            SetFieldValue<ItemType>(Models.Metadata.DatItem.TypeKey, ItemType.Display);
            SetFieldValue<Machine>(DatItem.MachineKey, new Machine());
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new Display()
            {
                _internal = this._internal?.Clone() as Models.Metadata.Display ?? [],
            };
        }

        #endregion
    }
}
