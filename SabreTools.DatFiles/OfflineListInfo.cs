namespace SabreTools.DatFiles
{
    /// <summary>
    /// Represents one OfflineList infos object
    /// </summary>
    public class OfflineListInfo
    {
        [Models.Required]
        public string? Name { get; set; }
        public bool? Visible { get; set; }
        public bool? InNamingOption { get; set; }
        public bool? Default { get; set; }
    }
}
