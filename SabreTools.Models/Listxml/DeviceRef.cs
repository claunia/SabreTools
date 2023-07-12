using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Models.Listxml
{
    [XmlRoot("device_ref")]
    public class DeviceRef
    {
        [XmlAttribute("name")]
        public string Name { get; set; }
    }
}