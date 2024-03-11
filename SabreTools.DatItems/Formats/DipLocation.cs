using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents one diplocation
    /// </summary>
    [JsonObject("diplocation"), XmlRoot("diplocation")]
    public sealed class DipLocation : DatItem<Models.Metadata.DipLocation>
    {
        #region Fields

        /// <inheritdoc>/>
        protected override ItemType ItemType => ItemType.DipLocation;

        /// <inheritdoc>/>
        protected override string? NameKey => Models.Metadata.DipLocation.NameKey;

        #endregion

        #region Constructors

        public DipLocation() : base() { }
        public DipLocation(Models.Metadata.DipLocation item) : base(item) { }

        #endregion
    }
}
