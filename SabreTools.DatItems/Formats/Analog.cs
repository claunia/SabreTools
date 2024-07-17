using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents a single analog item
    /// </summary>
    [JsonObject("analog"), XmlRoot("analog")]
    public sealed class Analog : DatItem<Models.Metadata.Analog>
    {
        #region Fields

        /// <inheritdoc>/>
        protected override ItemType ItemType => ItemType.Analog;

        /// <inheritdoc>/>
        protected override string? NameKey => null;

        #endregion

        #region Constructors

        public Analog() : base() { }
        public Analog(Models.Metadata.Analog item) : base(item) { }

        #endregion
    }
}
