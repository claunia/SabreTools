using System.Xml.Serialization;

namespace SabreTools.Models.Listxml
{
    [XmlRoot("game")]
    public class Game : GameBase
    {
        /// <remarks>Appears after Manufacturer</remarks>
        [XmlElement("history")]
        public string? History { get; set; }
    }
}