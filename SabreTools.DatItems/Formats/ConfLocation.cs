using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core.Tools;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents one conflocation
    /// </summary>
    [JsonObject("conflocation"), XmlRoot("conflocation")]
    public sealed class ConfLocation : DatItem<Models.Metadata.ConfLocation>
    {
        #region Fields

        /// <inheritdoc>/>
        protected override ItemType ItemType => ItemType.ConfLocation;

        /// <inheritdoc>/>
        protected override string? NameKey => Models.Metadata.ConfLocation.NameKey;

        #endregion

        #region Constructors

        public ConfLocation() : base() { }

        public ConfLocation(Models.Metadata.ConfLocation item) : base(item)
        {
            // Process flag values
            if (GetBoolFieldValue(Models.Metadata.ConfLocation.InvertedKey) != null)
                SetFieldValue<string?>(Models.Metadata.ConfLocation.InvertedKey, GetBoolFieldValue(Models.Metadata.ConfLocation.InvertedKey).FromYesNo());
        }

        #endregion
    }
}
