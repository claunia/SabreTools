using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core.Tools;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents which Adjuster(s) is associated with a set
    /// </summary>
    [JsonObject("adjuster"), XmlRoot("adjuster")]
    public sealed class Adjuster : DatItem<Models.Metadata.Adjuster>
    {
        #region Fields

        /// <inheritdoc>/>
        protected override ItemType ItemType => ItemType.Adjuster;

        /// <inheritdoc>/>
        protected override string? NameKey => Models.Metadata.Adjuster.NameKey;

        [JsonIgnore]
        public bool ConditionsSpecified
        {
            get
            {
                var conditions = GetFieldValue<Condition[]?>(Models.Metadata.Adjuster.ConditionKey);
                return conditions != null && conditions.Length > 0;
            }
        }

        #endregion

        #region Constructors

        public Adjuster() : base() { }
        public Adjuster(Models.Metadata.Adjuster item) : base(item)
        {
            // Process flag values
            if (GetBoolFieldValue(Models.Metadata.Adjuster.DefaultKey) != null)
                SetFieldValue<string?>(Models.Metadata.Adjuster.DefaultKey, GetBoolFieldValue(Models.Metadata.Adjuster.DefaultKey).FromYesNo());

            // Handle subitems
            var condition = item.Read<Models.Metadata.Condition>(Models.Metadata.Adjuster.ConditionKey);
            if (condition != null)
                SetFieldValue(Models.Metadata.Adjuster.ConditionKey, new Condition(condition));
        }

        #endregion
    }
}
