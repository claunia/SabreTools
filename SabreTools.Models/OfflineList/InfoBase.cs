using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Models.OfflineList
{
    public class InfoBase
    {
        [XmlAttribute("visible")]
        public bool? Visible { get; set; }

        [XmlAttribute("inNamingOption")]
        public bool? InNamingOption { get; set; }

        [XmlAttribute("default")]
        public bool? Default { get; set; }
    }

    [XmlRoot("title")]
    public class Title : InfoBase { }

    [XmlRoot("location")]
    public class Location : InfoBase { }

    [XmlRoot("publisher")]
    public class Publisher : InfoBase { }

    [XmlRoot("sourceRom")]
    public class SourceRom : InfoBase { }

    [XmlRoot("romSize")]
    public class RomSize : InfoBase { }

    [XmlRoot("releaseNumber")]
    public class ReleaseNumber : InfoBase { }

    [XmlRoot("imageNumber")]
    public class ImageNumber : InfoBase { }

    [XmlRoot("languageNumber")]
    public class LanguageNumber : InfoBase { }

    [XmlRoot("comment")]
    public class Comment : InfoBase { }

    [XmlRoot("romCRC")]
    public class RomCRC : InfoBase { }

    [XmlRoot("im1CRC")]
    public class Im1CRC : InfoBase { }

    [XmlRoot("im2CRC")]
    public class Im2CRC : InfoBase { }

    [XmlRoot("languages")]
    public class Languages : InfoBase { }
}