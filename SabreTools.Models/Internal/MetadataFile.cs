namespace SabreTools.Models.Internal
{
    /// <summary>
    /// Format-agnostic representation of a full metadata file
    /// </summary>
    public class MetadataFile : DictionaryBase
    {
        #region Keys

        /// <remarks>Machine[]</remarks>
        public const string MachineKey = "machine";

        /// <remarks>Header</remarks>
        public const string HeaderKey = "header";

        #endregion
    }
}