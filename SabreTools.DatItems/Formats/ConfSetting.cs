using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents one ListXML confsetting
    /// </summary>
    [JsonObject("confsetting"), XmlRoot("confsetting")]
    public sealed class ConfSetting : DatItem<Models.Metadata.ConfSetting>
    {
        #region Fields

        /// <inheritdoc>/>
        protected override ItemType ItemType => ItemType.ConfSetting;

        /// <inheritdoc>/>
        protected override string? NameKey => Models.Metadata.ConfSetting.NameKey;

        [JsonIgnore]
        public bool ConditionsSpecified
        {
            get
            {
                var conditions = GetFieldValue<Condition[]?>(Models.Metadata.ConfSetting.ConditionKey);
                return conditions != null && conditions.Length > 0;
            }
        }

        #endregion

        #region Constructors

        public ConfSetting() : base() { }
        public ConfSetting(Models.Metadata.ConfSetting item) : base(item) { }

        #endregion
    }
}
