using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Models.OpenMSX
{
    [XmlRoot("original")]
    public class Original
    {
        [XmlElement("value")]
        public bool Value { get; set; }

        public string? Content { get; set; }
    }
}