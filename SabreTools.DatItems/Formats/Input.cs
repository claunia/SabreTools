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
            Machine = new Machine();

            ItemType = ItemType.Input;
        }

        /// <summary>
        /// Create an Input object from the internal model
        /// </summary>
        public Input(Models.Metadata.Input? item)
        {
            _internal = item ?? [];
            Machine = new Machine();

            ItemType = ItemType.Input;
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new Input()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine ?? new Machine(),
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _internal = this._internal?.Clone() as Models.Metadata.Input ?? [],
            };
        }

        #endregion
    }
}
