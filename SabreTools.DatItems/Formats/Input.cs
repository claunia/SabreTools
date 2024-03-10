using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents one ListXML input
    /// </summary>
    [JsonObject("input"), XmlRoot("input")]
    public class Input : DatItem
    {
        #region Fields

        [JsonIgnore]
        public bool ControlsSpecified
        {
            get
            {
                var controls = GetFieldValue<Control[]?>(Models.Metadata.Input.ControlKey);
                return controls != null && controls.Length > 0;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Input object
        /// </summary>
        public Input()
        {
            _internal = new Models.Metadata.Input();

            SetFieldValue<ItemType>(Models.Metadata.DatItem.TypeKey, ItemType.Input);
            SetFieldValue<Machine>(DatItem.MachineKey, new Machine());
        }

        /// <summary>
        /// Create an Input object from the internal model
        /// </summary>
        public Input(Models.Metadata.Input item)
        {
            _internal = item;

            SetFieldValue<ItemType>(Models.Metadata.DatItem.TypeKey, ItemType.Input);
            SetFieldValue<Machine>(DatItem.MachineKey, new Machine());
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new Input()
            {
                _internal = this._internal?.Clone() as Models.Metadata.Input ?? [],
            };
        }

        #endregion
    }
}
