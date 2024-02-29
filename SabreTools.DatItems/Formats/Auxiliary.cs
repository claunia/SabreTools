using System.Xml.Serialization;
using Newtonsoft.Json;

/// <summary>
/// This holds all of the auxiliary types needed for proper parsing
/// </summary>
namespace SabreTools.DatItems.Formats
{
    #region DatItem

    #region OpenMSX

    /// <summary>
    /// Represents the OpenMSX original value
    /// </summary>
    [JsonObject("original"), XmlRoot("original")]
    public class Original
    {
        [JsonProperty("value"), XmlElement("value")]
        public bool? Value
        {
            get => _internal.ReadBool(Models.Metadata.Original.ValueKey);
            set => _internal[Models.Metadata.Original.ValueKey] = value;
        }

        [JsonProperty("content"), XmlElement("content")]
        public string? Content
        {
            get => _internal.ReadString(Models.Metadata.Original.ContentKey);
            set => _internal[Models.Metadata.Original.ContentKey] = value;
        }

        /// <summary>
        /// Internal Original model
        /// </summary>
        [JsonIgnore]
        private readonly Models.Metadata.Original _internal = [];
    }

    #endregion

    #endregion //DatItem
}
