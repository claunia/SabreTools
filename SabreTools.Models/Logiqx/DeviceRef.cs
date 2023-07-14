using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Models.Logiqx
{
    [XmlRoot("device_ref")]
    public class DeviceRef : ItemBase
    {
        [XmlAttribute("name")]
        public string Name { get; set; }
    }
}