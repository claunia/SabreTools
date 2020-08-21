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
