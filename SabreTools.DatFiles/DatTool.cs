using SabreTools.Logging;

// TODO: Should each of the individual pieces of partial classes be their own classes?
// TODO: Is a single, consistent internal state needed for this class at all?
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
        private static readonly Logger logger = new Logger();

        #endregion
    }
}
