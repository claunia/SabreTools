using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents the sound output for a machine
    /// </summary>
    [JsonObject("sound"), XmlRoot("sound")]
    public sealed class Sound : DatItem<Models.Metadata.Sound>
    {
        #region Fields

        /// <inheritdoc>/>
        protected override ItemType ItemType => ItemType.Sound;

        /// <inheritdoc>/>
        protected override string? NameKey => null;

        #endregion

        #region Constructors

        public Sound() : base() { }
        public Sound(Models.Metadata.Sound item) : base(item) { }

        #endregion
    }
}
