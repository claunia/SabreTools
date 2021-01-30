using SabreTools.Logging;

namespace SabreTools.Filtering
{
    /// <summary>
    /// Represents the removal operations that need to be performed on a set of items, usually a DAT
    /// </summary>
    public abstract class Remover
    {
        #region Logging

        /// <summary>
        /// Logging object
        /// </summary>
        protected Logger logger;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public Remover()
        {
            logger = new Logger(this);
        }

        #endregion

        #region Remover Population

        /// <summary>
        /// Set remover from a value
        /// </summary>
        /// <param name="field">Key for the remover to be set</param>
        public abstract bool SetRemover(string field);

        #endregion
    }
}