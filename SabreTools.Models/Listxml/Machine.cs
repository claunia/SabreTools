using System.Xml.Serialization;

namespace SabreTools.Models.Listxml
{
    [XmlRoot("machine")]
    public class Machine : GameBase
    {
        /// <remarks>Appears after Name</remarks>

        [XmlAttribute("sourcefile")]
        public string? SourceFile { get; set; }

        /// <remarks>(yes|no) "no", Appears after SourceFile</remarks>
        [XmlAttribute("isbios")]
        public string? IsBios { get; set; }

        /// <remarks>(yes|no) "no", Appears after IsBios</remarks>
        [XmlAttribute("isdevice")]
        public string? IsDevice { get; set; }

        /// <remarks>(yes|no) "no", Appears after IsDevice</remarks>
        [XmlAttribute("ismechanical")]
        public string? IsMechanical { get; set; }
    }
}