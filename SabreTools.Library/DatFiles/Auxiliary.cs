/// <summary>
/// This holds all of the auxiliary types needed for proper parsing
/// </summary>
namespace SabreTools.Library.DatFiles
{
    #region DatHeader

    #region OfflineList

    /// <summary>
    /// Represents one OfflineList infos object
    /// </summary>
    public class OfflineListInfo
    {
        public string Name { get; set; }
        public bool? Visible { get; set; }
        public bool? IsNamingOption { get; set; }
        public bool? Default { get; set; }

        public OfflineListInfo(string name, bool? visible, bool? isNamingOption, bool? def)
        {
            Name = name;
            Visible = visible;
            IsNamingOption = isNamingOption;
            Default = def;
        }
    }

    #endregion

    #endregion // DatHeader
}
