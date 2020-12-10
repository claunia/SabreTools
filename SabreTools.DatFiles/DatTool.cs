using SabreTools.Logging;

// TODO: What sort of internal state should this have? Would a single DatFile be appropriate?
// TODO: How much of the stuff currently in DatFile should be moved here?
// TODO: Can things like CreateAndParse be moved here?
namespace SabreTools.DatFiles
{
    /// <summary>
    /// Represents a format-agnostic DAT
    /// </summary>
    public partial class DatTool
    {
        #region Logging

        /// <summary>
        /// Logging object
        /// </summary>
        protected Logger logger;

        #endregion
    }
}
