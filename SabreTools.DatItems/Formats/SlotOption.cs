using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core.Tools;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents one ListXML slotoption
    /// </summary>
    [JsonObject("slotoption"), XmlRoot("slotoption")]
    public sealed class SlotOption : DatItem<Models.Metadata.SlotOption>
    {
        #region Fields

        /// <inheritdoc>/>
        protected override ItemType ItemType => ItemType.SlotOption;

        /// <inheritdoc>/>
        protected override string? NameKey => Models.Metadata.SlotOption.NameKey;

        #endregion

        #region Constructors

        public SlotOption() : base() { }

        public SlotOption(Models.Metadata.SlotOption item) : base(item)
        {
            // Process flag values
            if (GetBoolFieldValue(Models.Metadata.SlotOption.DefaultKey) != null)
                SetFieldValue<string?>(Models.Metadata.SlotOption.DefaultKey, GetBoolFieldValue(Models.Metadata.SlotOption.DefaultKey).FromYesNo());
        }

        #endregion
    }
}
