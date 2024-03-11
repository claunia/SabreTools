using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents one shared feature object
    /// </summary>
    [JsonObject("sharedfeat"), XmlRoot("sharedfeat")]
    public sealed class SharedFeat : DatItem<Models.Metadata.SharedFeat>
    {
        #region Fields

        /// <inheritdoc>/>
        protected override ItemType ItemType => ItemType.SharedFeat;

        /// <inheritdoc>/>
        protected override string? NameKey => Models.Metadata.SharedFeat.NameKey;

        #endregion

        #region Constructors

        public SharedFeat() : base() { }
        public SharedFeat(Models.Metadata.SharedFeat item) : base(item) { }

        #endregion
    }
}
