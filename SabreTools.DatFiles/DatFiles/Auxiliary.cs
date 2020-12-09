/// <summary>
/// This holds all of the auxiliary types needed for proper parsing
/// </summary>
namespace SabreTools.DatFiles
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
        public bool? InNamingOption { get; set; }
        public bool? Default { get; set; }
    }

    #endregion

    #endregion // DatHeader
}
