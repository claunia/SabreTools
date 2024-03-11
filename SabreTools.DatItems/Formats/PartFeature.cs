using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents one part feature object
    /// </summary>
    [JsonObject("part_feature"), XmlRoot("part_feature")]
    public sealed class PartFeature : DatItem<Models.Metadata.Feature>
    {
        #region Fields

        /// <inheritdoc>/>
        protected override ItemType ItemType => ItemType.PartFeature;

        /// <inheritdoc>/>
        protected override string? NameKey => Models.Metadata.Feature.NameKey;

        #endregion

        #region Constructors

        public PartFeature() : base() { }
        public PartFeature(Models.Metadata.Feature item) : base(item) { }

        #endregion
    }
}
