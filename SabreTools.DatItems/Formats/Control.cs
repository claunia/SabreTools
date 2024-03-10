using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents control for an input
    /// </summary>
    [JsonObject("control"), XmlRoot("control")]
    public class Control : DatItem
    {
        #region Constructors

        /// <summary>
        /// Create a default, empty Control object
        /// </summary>
        public Control()
        {
            _internal = new Models.Metadata.Control();

            SetFieldValue<ItemType>(Models.Metadata.DatItem.TypeKey, ItemType.Control);
            SetFieldValue<Machine>(DatItem.MachineKey, new Machine());
        }

        /// <summary>
        /// Create a Control object from the internal model
        /// </summary>
        public Control(Models.Metadata.Control? item)
        {
            _internal = item ?? [];

            SetFieldValue<ItemType>(Models.Metadata.DatItem.TypeKey, ItemType.Control);
            SetFieldValue<Machine>(DatItem.MachineKey, new Machine());
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new Control()
            {
                _internal = this._internal?.Clone() as Models.Metadata.Control ?? [],
            };
        }

        #endregion
    }
}
