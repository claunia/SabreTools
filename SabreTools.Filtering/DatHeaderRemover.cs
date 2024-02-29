using System.Collections.Generic;

using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.DatFiles;

namespace SabreTools.Filtering
{
    /// <summary>
    /// Represents the removal operations that need to be performed on a DatHeader
    /// </summary>
    public class DatHeaderRemover
    {
        #region Fields

        /// <summary>
        /// List of DatHeader fields to exclude from writing
        /// </summary>
        public List<DatHeaderField> DatHeaderFields { get; private set; } = new List<DatHeaderField>();

        #endregion

        #region Population

        /// <summary>
        /// Set remover from a value
        /// </summary>
        /// <param name="field">Key for the remover to be set</param>
        public bool SetRemover(string field)
        {
            // If the key is null or empty, return false
            if (string.IsNullOrEmpty(field))
                return false;

            // If we have a DatHeader field
            DatHeaderField datHeaderField = field.AsDatHeaderField();
            if (datHeaderField != DatHeaderField.NULL)
            {
                DatHeaderFields.Add(datHeaderField);
                return true;
            }

            return false;
        }

        #endregion

        #region Running

        /// <summary>
        /// Remove fields with given values
        /// </summary>
        /// <param name="datItem">DatHeader to remove fields from</param>
        public void RemoveFields(DatHeader datHeader)
        {
            if (datHeader == null)
                return;

            #region Common

            if (DatHeaderFields == null)
                return;

            #endregion

            // TODO: Figure out how to properly implement DatHeader field removal
        }

        #endregion
    }
}