using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core.Tools;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents the a feature of the machine
    /// </summary>
    [JsonObject("feature"), XmlRoot("feature")]
    public sealed class Feature : DatItem<Models.Metadata.Feature>
    {
        #region Fields

        /// <inheritdoc>/>
        protected override ItemType ItemType => ItemType.Feature;

        /// <inheritdoc>/>
        protected override string? NameKey => null;

        #endregion

        #region Constructors

        public Feature() : base() { }
        public Feature(Models.Metadata.Feature item) : base(item)
        {
            // Process flag values
            if (GetStringFieldValue(Models.Metadata.Feature.OverallKey) != null)
                SetFieldValue<string?>(Models.Metadata.Feature.OverallKey, GetStringFieldValue(Models.Metadata.Feature.OverallKey).AsEnumValue<FeatureStatus>().AsStringValue());
            if (GetStringFieldValue(Models.Metadata.Feature.StatusKey) != null)
                SetFieldValue<string?>(Models.Metadata.Feature.StatusKey, GetStringFieldValue(Models.Metadata.Feature.StatusKey).AsEnumValue<FeatureStatus>().AsStringValue());
            if (GetStringFieldValue(Models.Metadata.Feature.FeatureTypeKey) != null)
                SetFieldValue<string?>(Models.Metadata.Feature.FeatureTypeKey, GetStringFieldValue(Models.Metadata.Feature.FeatureTypeKey).AsEnumValue<FeatureType>().AsStringValue());
        }

        #endregion
    }
}
