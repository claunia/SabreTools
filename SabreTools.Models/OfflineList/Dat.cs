using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Models.OfflineList
{
    [XmlRoot("dat")]
    public class Dat
    {
        [XmlElement("configuration")]
        public Configuration? Configuration { get; set; }

        [XmlElement("games")]
        public Games? Games { get; set; }
    }
}