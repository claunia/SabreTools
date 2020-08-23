using System.Collections.Generic;

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
    public class ListXmlAdjuster
    {
        public string Name { get; set; }
        public bool? Default { get; set; }
        public List<ListXmlCondition> Conditions { get; set; }

        public ListXmlAdjuster()
        {
            Conditions = new List<ListXmlCondition>();
        }
    }

    /// <summary>
    /// Represents one ListXML analog
    /// </summary>
    public class ListXmlAnalog
    {
        public string Mask { get; set; }
    }

    /// <summary>
    /// Represents one ListXML chip
    /// </summary>
    public class ListXmlChip
    {
        public string Name { get; set; }
        public string Tag { get; set; }
        public string Type { get; set; } // TODO: (cpu|audio)
        public string Clock { get; set; }
    }

    /// <summary>
    /// Represents one ListXML condition
    /// </summary>
    public class ListXmlCondition
    {
        public string Tag { get; set; }
        public string Mask { get; set; }
        public string Relation { get; set; } // TODO: (eq|ne|gt|le|lt|ge)
        public string Value { get; set; }
    }

    /// <summary>
    /// Represents one ListXML configuration
    /// </summary>
    public class ListXmlConfiguration
    {
        public string Name { get; set; }
        public string Tag { get; set; }
        public string Mask { get; set; }
        public List<ListXmlConfLocation> Locations { get; set; }
        public List<ListXmlConfSetting> Settings { get; set; }

        public ListXmlConfiguration()
        {
            Locations = new List<ListXmlConfLocation>();
            Settings = new List<ListXmlConfSetting>();
        }
    }

    /// <summary>
    /// Represents one ListXML conflocation
    /// </summary>
    public class ListXmlConfLocation
    {
        public string Name { get; set; }
        public string Number { get; set; }
        public bool? Inverted { get; set; }
    }

    /// <summary>
    /// Represents one ListXML confsetting
    /// </summary>
    public class ListXmlConfSetting
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public bool? Default { get; set; }
    }

    /// <summary>
    /// Represents one ListXML control
    /// </summary>
    public class ListXmlControl
    {
        public string Type { get; set; }
        public string Player { get; set; } // TODO: Int32?
        public string Buttons { get; set; } // TODO: Int32?
        public string RegButtons { get; set; } // TODO: Int32?
        public string Minimum { get; set; } // TODO: Int32? Float?
        public string Maximum { get; set; } // TODO: Int32? Float?
        public string Sensitivity { get; set; } // TODO: Int32? Float?
        public string KeyDelta { get; set; } // TODO: Int32? Float?
        public bool? Reverse { get; set; }
        public string Ways { get; set; } // TODO: Int32? Float?
        public string Ways2 { get; set; } // TODO: Int32? Float?
        public string Ways3 { get; set; } // TODO: Int32? Float?
    }

    /// <summary>
    /// Represents one ListXML device
    /// </summary>
    public class ListXmlDevice
    {
        public string Type { get; set; }
        public string Tag { get; set; }
        public string FixedImage { get; set; }
        public string Mandatory { get; set; } // TODO: bool?
        public string Interface { get; set; }
        public List<ListXmlInstance> Instances { get; set; }
        public List<ListXmlExtension> Extensions { get; set; }

        public ListXmlDevice()
        {
            Instances = new List<ListXmlInstance>();
            Extensions = new List<ListXmlExtension>();
        }
    }

    /// <summary>
    /// Represents one ListXML deviceref
    /// </summary>
    public class ListXmlDeviceReference
    {
        public string Name { get; set; }
    }

    /// <summary>
    /// Represents one ListXML display
    /// </summary>
    public class ListXmlDisplay
    {
        public string Tag { get; set; }
        public string Type { get; set; } // TODO: (raster|vector|lcd|svg|unknown)
        public string Rotate { get; set; } // TODO: (0|90|180|270) Int32?
        public bool? FlipX { get; set; }
        public string Width { get; set; } // TODO: Int32?
        public string Height { get; set; } // TODO: Int32?
        public string Refresh { get; set; } // TODO: Int32? Float?
        public string PixClock { get; set; } // TODO: Int32? Float?
        public string HTotal { get; set; } // TODO: Int32? Float?
        public string HBend { get; set; } // TODO: Int32? Float?
        public string HStart { get; set; } // TODO: Int32? Float?
        public string VTotal { get; set; } // TODO: Int32? Float?
        public string VBend { get; set; } // TODO: Int32? Float?
        public string VStart { get; set; } // TODO: Int32? Float?
    }

    /// <summary>
    /// Represents one ListXML dipswitch
    /// </summary>
    /// <remarks>Also used by SoftwareList</remarks>
    public class ListXmlDipSwitch
    {
        public string Name { get; set; }
        public string Tag { get; set; }
        public string Mask { get; set; }
        public List<ListXmlDipLocation> Locations { get; set; }
        public List<ListXmlDipValue> Values { get; set; }

        public ListXmlDipSwitch()
        {
            Locations = new List<ListXmlDipLocation>();
            Values = new List<ListXmlDipValue>();
        }
    }

    /// <summary>
    /// Represents one ListXML diplocation
    /// </summary>
    public class ListXmlDipLocation
    {
        public string Name { get; set; }
        public string Number { get; set; }
        public bool? Inverted { get; set; }
    }

    /// <summary>
    /// Represents one ListXML dipvalue
    /// </summary>
    /// <remarks>Also used by SoftwareList</remarks>
    public class ListXmlDipValue
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public bool? Default { get; set; }
    }

    /// <summary>
    /// Represents one ListXML driver
    /// </summary>
    public class ListXmlDriver
    {
        public string Status { get; set; } // TODO: (good|imperfect|preliminary)
        public string Emulation { get; set; } // TODO: (good|imperfect|preliminary)
        public string Cocktail { get; set; } // TODO: bool? (good|imperfect|preliminary)?
        public string SaveState { get; set; } // TODO: (supported|unsupported)
    }

    /// <summary>
    /// Represents one ListXML extension
    /// </summary>
    public class ListXmlExtension
    {
        public string Name { get; set; }
    }

    /// <summary>
    /// Represents one ListXML feature
    /// </summary>
    public class ListXmlFeature
    {
        public string Type { get; set; } // TODO: (protection|palette|graphics|sound|controls|keyboard|mouse|microphone|camera|disk|printer|lan|wan|timing)
        public string Status { get; set; } // TODO: (unemulated|imperfect)
        public string Overall { get; set; } // TODO: (unemulated|imperfect)
    }

    /// <summary>
    /// Represents one ListXML info
    /// </summary>
    public class ListXmlInfo
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    /// <summary>
    /// Represents one ListXML input
    /// </summary>
    public class ListXmlInput
    {
        public bool? Service { get; set; }
        public bool? Tilt { get; set; }
        public string Players { get; set; } // TODO: Int32?
        public string Coins { get; set; } // TODO: Int32?
        public List<ListXmlControl> Controls { get; set; }

        public ListXmlInput()
        {
            Controls = new List<ListXmlControl>();
        }
    }

    /// <summary>
    /// Represents one ListXML instance
    /// </summary>
    public class ListXmlInstance
    {
        public string Name { get; set; }
        public string BriefName { get; set; }
    }

    /// <summary>
    /// Represents one ListXML port
    /// </summary>
    public class ListXmlPort
    {
        public string Tag { get; set; }
        public List<ListXmlAnalog> Analogs { get; set; }

        public ListXmlPort()
        {
            Analogs = new List<ListXmlAnalog>();
        }
    }

    /// <summary>
    /// Represents one ListXML ramoption
    /// </summary>
    public class ListXmlRamOption
    {
        public bool? Default { get; set; }
    }

    /// <summary>
    /// Represents one ListXML slot
    /// </summary>
    public class ListXmlSlot
    {
        public string Name { get; set; }
        public List<ListXmlSlotOption> SlotOptions { get; set; }

        public ListXmlSlot()
        {
            SlotOptions = new List<ListXmlSlotOption>();
        }
    }

    /// <summary>
    /// Represents one ListXML slotoption
    /// </summary>
    public class ListXmlSlotOption
    {
        public string Name { get; set; }
        public string DeviceName { get; set; }
        public bool? Default { get; set; }
    }

    /// <summary>
    /// Represents one ListXML softwarelist
    /// </summary>
    public class ListXmlSoftwareList
    {
        public string Name { get; set; }
        public string Status { get; set; } // TODO: (original|compatible)
        public string Filter { get; set; }
    }

    /// <summary>
    /// Represents one ListXML sound
    /// </summary>
    public class ListXmlSound
    {
        public string Channels { get; set; } // TODO: Int32?
    }

    #endregion

    #region OpenMSX

    /// <summary>
    /// Represents the OpenMSX original value
    /// </summary>
    public class OpenMSXOriginal
    {
        public string Name { get; set; }
        public bool? Value { get; set; }
    }

    #endregion

    #region SoftwareList

    /// <summary>
    /// Represents one SoftwareList shared feature object
    /// </summary>
    public class SoftwareListSharedFeature
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public SoftwareListSharedFeature(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }

    #endregion

    #endregion // Machine

    #region DatItem

    #region SoftwareList

    /// <summary>
    /// Represents one SoftwareList feature object
    /// </summary>
    public class SoftwareListFeature
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public SoftwareListFeature(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }

    #endregion

    #endregion //DatItem
}
