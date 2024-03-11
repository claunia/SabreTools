using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents one ListXML dipvalue
    /// </summary>
    [JsonObject("dipvalue"), XmlRoot("dipvalue")]
    public sealed class DipValue : DatItem<Models.Metadata.DipValue>
    {
        #region Fields

        /// <inheritdoc>/>
        protected override ItemType ItemType => ItemType.DipValue;

        /// <inheritdoc>/>
        protected override string? NameKey => Models.Metadata.DipValue.NameKey;

        [JsonIgnore]
        public bool ConditionsSpecified
        {
            get
            {
                var conditions = GetFieldValue<Condition[]?>(Models.Metadata.DipValue.ConditionKey);
                return conditions != null && conditions.Length > 0;
            }
        }

        #endregion

        #region Constructors

        public DipValue() : base() { }
        public DipValue(Models.Metadata.DipValue item) : base(item) { }

        #endregion
    }
}
