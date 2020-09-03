using System.Collections.Generic;
using System.Linq;

using SabreTools.Library.DatItems;
using SabreTools.Library.Filtering;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

/// <summary>
/// This holds all of the auxiliary types needed for proper parsing
/// </summary>
namespace SabreTools.Library.DatItems
{
    #region Machine

    #region ListXML

    /// <summary>
    /// Represents one ListXML control
    /// </summary>
    [JsonObject("control")]
    public class Control
    {
        #region Fields

        /// <summary>
        /// Control type
        /// </summary>
        [JsonProperty("type", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Type { get; set; }

        /// <summary>
        /// Player ID
        /// </summary>
        [JsonProperty("player", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Player { get; set; } // TODO: Int32?

        /// <summary>
        /// Button count
        /// </summary>
        [JsonProperty("buttons", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Buttons { get; set; } // TODO: Int32?

        /// <summary>
        /// Regular button count
        /// </summary>
        [JsonProperty("regbuttons", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string RegButtons { get; set; } // TODO: Int32?

        /// <summary>
        /// Minimum value
        /// </summary>
        [JsonProperty("minimum", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Minimum { get; set; } // TODO: Int32? Float?

        /// <summary>
        /// Maximum value
        /// </summary>
        [JsonProperty("maximum", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Maximum { get; set; } // TODO: Int32? Float?

        /// <summary>
        /// Sensitivity value
        /// </summary>
        [JsonProperty("sensitivity", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Sensitivity { get; set; } // TODO: Int32? Float?

        /// <summary>
        /// Keypress delta
        /// </summary>
        [JsonProperty("keydelta", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string KeyDelta { get; set; } // TODO: Int32? Float?

        /// <summary>
        /// Determines if the control is reversed
        /// </summary>
        [JsonProperty("reverse", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool? Reverse { get; set; }

        /// <summary>
        /// First set of ways
        /// </summary>
        [JsonProperty("ways", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Ways { get; set; } // TODO: Int32? Float?

        /// <summary>
        /// Second set of ways
        /// </summary>
        [JsonProperty("ways2", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Ways2 { get; set; } // TODO: Int32? Float?

        /// <summary>
        /// Third set of ways
        /// </summary>
        [JsonProperty("ways3", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Ways3 { get; set; } // TODO: Int32? Float?

        #endregion
    }

    /// <summary>
    /// Represents one ListXML display
    /// </summary>
    /// TODO: Promote to DatItem level
    [JsonObject("display")]
    public class Display
    {
        #region Fields

        /// <summary>
        /// Display tag
        /// </summary>
        [JsonProperty("tag", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Tag { get; set; }

        /// <summary>
        /// Display type
        /// </summary>
        [JsonProperty("type", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Type { get; set; } // TODO: (raster|vector|lcd|svg|unknown)

        /// <summary>
        /// Display rotation
        /// </summary>
        [JsonProperty("rotate", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Rotate { get; set; } // TODO: (0|90|180|270) Int32?

        /// <summary>
        /// Determines if display is flipped in the X-coordinates
        /// </summary>
        [JsonProperty("flipx", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool? FlipX { get; set; }

        /// <summary>
        /// Display width
        /// </summary>
        [JsonProperty("width", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Width { get; set; } // TODO: Int32?

        /// <summary>
        /// Display height
        /// </summary>
        [JsonProperty("height", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Height { get; set; } // TODO: Int32?

        /// <summary>
        /// Refresh rate
        /// </summary>
        [JsonProperty("refresh", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Refresh { get; set; } // TODO: Int32? Float?

        /// <summary>
        /// Pixel clock timer
        /// </summary>
        [JsonProperty("pixclock", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string PixClock { get; set; } // TODO: Int32? Float?

        /// <summary>
        /// Total horizontal lines
        /// </summary>
        [JsonProperty("htotal", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string HTotal { get; set; } // TODO: Int32? Float?

        /// <summary>
        /// Horizontal blank end
        /// </summary>
        [JsonProperty("hbend", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string HBEnd { get; set; } // TODO: Int32? Float?

        /// <summary>
        /// Horizontal blank start
        /// </summary>
        [JsonProperty("hbstart", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string HBStart { get; set; } // TODO: Int32? Float?

        /// <summary>
        /// Total vertical lines
        /// </summary>
        [JsonProperty("vtotal", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string VTotal { get; set; } // TODO: Int32? Float?

        /// <summary>
        /// Vertical blank end
        /// </summary>
        [JsonProperty("vbend", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string VBEnd { get; set; } // TODO: Int32? Float?

        /// <summary>
        /// Vertical blank start
        /// </summary>
        [JsonProperty("vbstart", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string VBStart { get; set; } // TODO: Int32? Float?

        #endregion
    }

    /// <summary>
    /// Represents one ListXML input
    /// </summary>
    /// TODO: Promote to DatItem level (contains list)
    [JsonObject("input")]
    public class Input
    {
        #region Fields

        /// <summary>
        /// Input service ID
        /// </summary>
        [JsonProperty("service", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool? Service { get; set; }

        /// <summary>
        /// Determins if this has a tilt sensor
        /// </summary>
        [JsonProperty("tilt", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool? Tilt { get; set; }

        /// <summary>
        /// Number of players on the input
        /// </summary>
        [JsonProperty("players", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Players { get; set; } // TODO: Int32?

        /// <summary>
        /// Number of coins required
        /// </summary>
        [JsonProperty("coins", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Coins { get; set; } // TODO: Int32?

        /// <summary>
        /// Set of controls for the input
        /// </summary>
        [JsonProperty("controls", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<Control> Controls { get; set; }

        #endregion
    }

    /// <summary>
    /// Represents one ListXML conflocation or diplocation
    /// </summary>
    [JsonObject("location")]
    public class Location
    {
        #region Fields

        /// <summary>
        /// Location name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Location ID
        /// </summary>
        [JsonProperty("number", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Number { get; set; }

        /// <summary>
        /// Determines if location is inverted or not
        /// </summary>
        [JsonProperty("inverted", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool? Inverted { get; set; }

        #endregion
    }

    /// <summary>
    /// Represents one ListXML port
    /// </summary>
    /// TODO: Promote to DatItem level (contains list)
    [JsonObject("port")]
    public class Port
    {
        #region Fields

        /// <summary>
        /// Tag for the port
        /// </summary>
        [JsonProperty("tag", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Tag { get; set; }

        /// <summary>
        /// List of analogs on the port
        /// </summary>
        [JsonProperty("analogs", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<Analog> Analogs { get; set; }

        #endregion
    }

    /// <summary>
    /// Represents one ListXML confsetting or dipvalue
    /// </summary>
    [JsonObject("setting")]
    public class Setting
    {
        #region Fields

        /// <summary>
        /// Setting name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Setting value
        /// </summary>
        [JsonProperty("value", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Value { get; set; }

        /// <summary>
        /// Determines if the setting is default or not
        /// </summary>
        [JsonProperty("default", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool? Default { get; set; }

        /// <summary>
        /// List of conditions on the setting
        /// </summary>
        [JsonProperty("conditions", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<Condition> Conditions { get; set; }

        #endregion
    }

    /// <summary>
    /// Represents one ListXML slotoption
    /// </summary>
    [JsonObject("slotoption")]
    public class SlotOption
    {
        #region Fields

        /// <summary>
        /// Slot option name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Referenced device name
        /// </summary>
        [JsonProperty("devname")]
        public string DeviceName { get; set; }

        /// <summary>
        /// Determines if this slot option is default or not
        /// </summary>
        [JsonProperty("default", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool? Default { get; set; }

        #endregion
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
