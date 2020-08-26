using System.Collections.Generic;

using Newtonsoft.Json;

/// <summary>
/// This holds all of the auxiliary types needed for proper parsing
/// </summary>
namespace SabreTools.Library.DatItems
{
    #region Machine

    #region ListXML

    /// <summary>
    /// Represents one ListXML adjuster
    /// </summary>
    [JsonObject("adjuster")]
    public class ListXmlAdjuster
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("default")]
        public bool? Default { get; set; }

        [JsonProperty("conditions")]
        public List<ListXmlCondition> Conditions { get; set; }
    }

    /// <summary>
    /// Represents one ListXML analog
    /// </summary>
    [JsonObject("analog")]
    public class ListXmlAnalog
    {
        [JsonProperty("mask")]
        public string Mask { get; set; }
    }

    /// <summary>
    /// Represents one ListXML condition
    /// </summary>
    [JsonObject("condition")]
    public class ListXmlCondition
    {
        [JsonProperty("tag")]
        public string Tag { get; set; }

        [JsonProperty("mask")]
        public string Mask { get; set; }

        [JsonProperty("relation")]
        public string Relation { get; set; } // TODO: (eq|ne|gt|le|lt|ge)

        [JsonProperty("value")]
        public string Value { get; set; }
    }

    /// <summary>
    /// Represents one ListXML configuration
    /// </summary>
    [JsonObject("configuration")]
    public class ListXmlConfiguration
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("tag")]
        public string Tag { get; set; }

        [JsonProperty("mask")]
        public string Mask { get; set; }

        [JsonProperty("locations")]
        public List<ListXmlConfLocation> Locations { get; set; }

        [JsonProperty("settings")]
        public List<ListXmlConfSetting> Settings { get; set; }
    }

    /// <summary>
    /// Represents one ListXML conflocation
    /// </summary>
    [JsonObject("conflocation")]
    public class ListXmlConfLocation
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("number")]
        public string Number { get; set; }

        [JsonProperty("inverted")]
        public bool? Inverted { get; set; }
    }

    /// <summary>
    /// Represents one ListXML confsetting
    /// </summary>
    [JsonObject("confsetting")]
    public class ListXmlConfSetting
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("default")]
        public bool? Default { get; set; }
    }

    /// <summary>
    /// Represents one ListXML control
    /// </summary>
    [JsonObject("control")]
    public class ListXmlControl
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("player")]
        public string Player { get; set; } // TODO: Int32?

        [JsonProperty("buttons")]
        public string Buttons { get; set; } // TODO: Int32?

        [JsonProperty("regbuttons")]
        public string RegButtons { get; set; } // TODO: Int32?

        [JsonProperty("minimum")]
        public string Minimum { get; set; } // TODO: Int32? Float?

        [JsonProperty("maximum")]
        public string Maximum { get; set; } // TODO: Int32? Float?

        [JsonProperty("sensitivity")]
        public string Sensitivity { get; set; } // TODO: Int32? Float?

        [JsonProperty("keydelta")]
        public string KeyDelta { get; set; } // TODO: Int32? Float?

        [JsonProperty("reverse")]
        public bool? Reverse { get; set; }

        [JsonProperty("ways")]
        public string Ways { get; set; } // TODO: Int32? Float?

        [JsonProperty("ways2")]
        public string Ways2 { get; set; } // TODO: Int32? Float?

        [JsonProperty("ways3")]
        public string Ways3 { get; set; } // TODO: Int32? Float?
    }

    /// <summary>
    /// Represents one ListXML device
    /// </summary>
    [JsonObject("device")]
    public class ListXmlDevice
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("tag")]
        public string Tag { get; set; }

        [JsonProperty("fixed_image")]
        public string FixedImage { get; set; }

        [JsonProperty("mandatory")]
        public string Mandatory { get; set; } // TODO: bool?

        [JsonProperty("interface")]
        public string Interface { get; set; }

        [JsonProperty("instances")]
        public List<ListXmlInstance> Instances { get; set; }

        [JsonProperty("extensions")]
        public List<ListXmlExtension> Extensions { get; set; }
    }

    /// <summary>
    /// Represents one ListXML deviceref
    /// </summary>
    /// TODO: Promote this to the same level as Sample
    [JsonObject("deviceref")]
    public class ListXmlDeviceReference
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }

    /// <summary>
    /// Represents one ListXML display
    /// </summary>
    [JsonObject("display")]
    public class ListXmlDisplay
    {
        [JsonProperty("tag")]
        public string Tag { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; } // TODO: (raster|vector|lcd|svg|unknown)

        [JsonProperty("rotate")]
        public string Rotate { get; set; } // TODO: (0|90|180|270) Int32?

        [JsonProperty("flipx")]
        public bool? FlipX { get; set; }

        [JsonProperty("width")]
        public string Width { get; set; } // TODO: Int32?

        [JsonProperty("height")]
        public string Height { get; set; } // TODO: Int32?

        [JsonProperty("refresh")]
        public string Refresh { get; set; } // TODO: Int32? Float?

        [JsonProperty("pixclock")]
        public string PixClock { get; set; } // TODO: Int32? Float?

        [JsonProperty("htotal")]
        public string HTotal { get; set; } // TODO: Int32? Float?

        [JsonProperty("hbend")]
        public string HBEnd { get; set; } // TODO: Int32? Float?

        [JsonProperty("hbstart")]
        public string HBStart { get; set; } // TODO: Int32? Float?

        [JsonProperty("vtotal")]
        public string VTotal { get; set; } // TODO: Int32? Float?

        [JsonProperty("vbend")]
        public string VBEnd { get; set; } // TODO: Int32? Float?

        [JsonProperty("vbstart")]
        public string VBStart { get; set; } // TODO: Int32? Float?
    }

    /// <summary>
    /// Represents one ListXML dipswitch
    /// </summary>
    /// <remarks>Also used by SoftwareList</remarks>
    [JsonObject("dipswitch")]
    public class ListXmlDipSwitch
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("tag")]
        public string Tag { get; set; }

        [JsonProperty("mask")]
        public string Mask { get; set; }

        [JsonProperty("locations")]
        public List<ListXmlDipLocation> Locations { get; set; }

        [JsonProperty("values")]
        public List<ListXmlDipValue> Values { get; set; }
    }

    /// <summary>
    /// Represents one ListXML diplocation
    /// </summary>
    [JsonObject("diplocation")]
    public class ListXmlDipLocation
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("number")]
        public string Number { get; set; }

        [JsonProperty("inverted")]
        public bool? Inverted { get; set; }
    }

    /// <summary>
    /// Represents one ListXML dipvalue
    /// </summary>
    /// <remarks>Also used by SoftwareList</remarks>
    [JsonObject("dipvalue")]
    public class ListXmlDipValue
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("default")]
        public bool? Default { get; set; }
    }

    /// <summary>
    /// Represents one ListXML driver
    /// </summary>
    [JsonObject("driver")]
    public class ListXmlDriver
    {
        [JsonProperty("status")]
        public string Status { get; set; } // TODO: (good|imperfect|preliminary)

        [JsonProperty("emulation")]
        public string Emulation { get; set; } // TODO: (good|imperfect|preliminary)

        [JsonProperty("cocktail")]
        public string Cocktail { get; set; } // TODO: bool? (good|imperfect|preliminary)?

        [JsonProperty("savestate")]
        public string SaveState { get; set; } // TODO: (supported|unsupported)
    }

    /// <summary>
    /// Represents one ListXML extension
    /// </summary>
    [JsonObject("extension")]
    public class ListXmlExtension
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }

    /// <summary>
    /// Represents one ListXML feature
    /// </summary>
    [JsonObject("feature")]
    public class ListXmlFeature
    {
        [JsonProperty("type")]
        public string Type { get; set; } // TODO: (protection|palette|graphics|sound|controls|keyboard|mouse|microphone|camera|disk|printer|lan|wan|timing)

        [JsonProperty("status")]
        public string Status { get; set; } // TODO: (unemulated|imperfect)

        [JsonProperty("overall")]
        public string Overall { get; set; } // TODO: (unemulated|imperfect)
    }

    /// <summary>
    /// Represents one ListXML input
    /// </summary>
    [JsonObject("input")]
    public class ListXmlInput
    {
        [JsonProperty("service")]
        public bool? Service { get; set; }

        [JsonProperty("tilt")]
        public bool? Tilt { get; set; }

        [JsonProperty("players")]
        public string Players { get; set; } // TODO: Int32?

        [JsonProperty("coins")]
        public string Coins { get; set; } // TODO: Int32?

        [JsonProperty("controls")]
        public List<ListXmlControl> Controls { get; set; }
    }

    /// <summary>
    /// Represents one ListXML instance
    /// </summary>
    [JsonObject("instance")]
    public class ListXmlInstance
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("briefname")]
        public string BriefName { get; set; }
    }

    /// <summary>
    /// Represents one ListXML port
    /// </summary>
    [JsonObject("port")]
    public class ListXmlPort
    {
        [JsonProperty("tag")]
        public string Tag { get; set; }

        [JsonProperty("analogs")]
        public List<ListXmlAnalog> Analogs { get; set; }
    }

    /// <summary>
    /// Represents one ListXML ramoption
    /// </summary>
    [JsonObject("ramoption")]
    public class ListXmlRamOption
    {
        [JsonProperty("default")]
        public bool? Default { get; set; }
    }

    /// <summary>
    /// Represents one ListXML slot
    /// </summary>
    [JsonObject("slot")]
    public class ListXmlSlot
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("slotoptions")]
        public List<ListXmlSlotOption> SlotOptions { get; set; }
    }

    /// <summary>
    /// Represents one ListXML slotoption
    /// </summary>
    [JsonObject("slotoption")]
    public class ListXmlSlotOption
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("devname")]
        public string DeviceName { get; set; }

        [JsonProperty("default")]
        public bool? Default { get; set; }
    }

    /// <summary>
    /// Represents one ListXML softwarelist
    /// </summary>
    /// TODO: Promote this to the same level as Sample?
    [JsonObject("softwarelist")]
    public class ListXmlSoftwareList
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; } // TODO: (original|compatible)

        [JsonProperty("filter")]
        public string Filter { get; set; }
    }

    /// <summary>
    /// Represents one ListXML sound
    /// </summary>
    [JsonObject("sound")]
    public class ListXmlSound
    {
        [JsonProperty("channels")]
        public string Channels { get; set; } // TODO: Int32?
    }

    #endregion

    #region OpenMSX

    /// <summary>
    /// Represents the OpenMSX original value
    /// </summary>
    [JsonObject("original")]
    public class OpenMSXOriginal
    {
        [JsonProperty("value")]
        public bool? Value { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }
    }

    #endregion

    #region SoftwareList

    /// <summary>
    /// Represents one SoftwareList info
    /// </summary>
    [JsonObject("info")]
    public class SoftwareListInfo
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }

    /// <summary>
    /// Represents one SoftwareList shared feature object
    /// </summary>
    [JsonObject("sharedfeat")]
    public class SoftwareListSharedFeature
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }

    #endregion

    #endregion // Machine

    #region DatItem

    #region SoftwareList

    /// <summary>
    /// Represents one SoftwareList feature object
    /// </summary>
    [JsonObject("feature")]
    public class SoftwareListFeature
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }

    /// <summary>
    /// Represents one SoftwareList part object
    /// </summary>
    [JsonObject("part")]
    public class SoftwareListPart
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("interface")]
        public string Interface { get; set; }
    }

    #endregion

    #endregion //DatItem
}
