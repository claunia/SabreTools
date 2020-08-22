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
        public string Original { get; set; }
        public bool? Value { get; set; }

        public OpenMSXOriginal(string original, bool? value)
        {
            Original = original;
            Value = value;
        }
    }

    #endregion

    #region ListXML

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

        public ListXMLDipSwitch(string name, string tag, string mask)
        {
            Name = name;
            Tag = tag;
            Mask = mask;
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

        public ListXMLDipLocation(string name, string number, bool? inverted)
        {
            Name = name;
            Number = number;
            Inverted = inverted;
        }
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

        public ListXMLDipValue(string name, string value, bool? def)
        {
            Name = name;
            Value = value;
            Default = def;
        }
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
