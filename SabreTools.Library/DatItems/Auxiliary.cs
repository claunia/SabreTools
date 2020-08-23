using System.Collections.Generic;

/// <summary>
/// This holds all of the auxiliary types needed for proper parsing
/// </summary>
namespace SabreTools.Library.DatItems
{
    #region Machine

    #region ListXML

    /// <summary>
    /// Represents one ListXML info object
    /// </summary>
    public class ListXmlInfo
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public ListXmlInfo(string name, string value)
        {
            Name = name;
            Value = value;
        }
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

    #region ListXML

    /// <summary>
    /// Represents one ListXML chip
    /// </summary>
    public class ListXMLChip
    {
        public string Name { get; set; }
        public string Tag { get; set; }
        public string Type { get; set; } // TODO: (cpu|audio)
        public string Clock { get; set; }
    }

    /// <summary>
    /// Represents one ListXML condition
    /// </summary>
    public class ListXMLCondition
    {
        public string Tag { get; set; }
        public string Mask { get; set; }
        public string Relation { get; set; } // TODO: (eq|ne|gt|le|lt|ge)
        public string Value { get; set; }
    }

    /// <summary>
    /// Represents one ListXML control
    /// </summary>
    public class ListXMLControl
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
    /// Represents one ListXML display
    /// </summary>
    public class ListXMLDisplay
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
    public class ListXMLDipSwitch
    {
        public string Name { get; set; }
        public string Tag { get; set; }
        public string Mask { get; set; }
        public List<ListXMLDipLocation> Locations { get; set; }
        public List<ListXMLDipValue> Values { get; set; }

        public ListXMLDipSwitch()
        {
            Locations = new List<ListXMLDipLocation>();
            Values = new List<ListXMLDipValue>();
        }
    }

    /// <summary>
    /// Represents one ListXML diplocation
    /// </summary>
    public class ListXMLDipLocation
    {
        public string Name { get; set; }
        public string Number { get; set; }
        public bool? Inverted { get; set; }
    }

    /// <summary>
    /// Represents one ListXML dipvalue
    /// </summary>
    /// <remarks>Also used by SoftwareList</remarks>
    public class ListXMLDipValue
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public bool? Default { get; set; }
    }

    /// <summary>
    /// Represents one ListXML input
    /// </summary>
    public class ListXMLInput
    {
        public bool? Service { get; set; }
        public bool? Tilt { get; set; }
        public string Players { get; set; } // TODO: Int32?
        public string Coins { get; set; } // TODO: Int32?
        public List<ListXMLControl> Controls { get; set; }

        public ListXMLInput()
        {
            Controls = new List<ListXMLControl>();
        }
    }

    /// <summary>
    /// Represents one ListXML sound
    /// </summary>
    public class ListXMLSound
    {
        public string Channels { get; set; } // TODO: Int32?
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
