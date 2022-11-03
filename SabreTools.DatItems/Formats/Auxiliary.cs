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
        public bool? Value { get; set; }

        [JsonProperty("content"), XmlElement("content")]
        public string Content { get; set; }
    }

    #endregion

    #endregion //DatItem
}
