using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents one ListXML input
    /// </summary>
    [JsonObject("input"), XmlRoot("input")]
    public sealed class Input : DatItem<Models.Metadata.Input>
    {
        #region Fields

        /// <inheritdoc>/>
        protected override ItemType ItemType => ItemType.Input;

        /// <inheritdoc>/>
        protected override string? NameKey => null;

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

        public Input() : base() { }
        public Input(Models.Metadata.Input item) : base(item) { }

        #endregion
    }
}
