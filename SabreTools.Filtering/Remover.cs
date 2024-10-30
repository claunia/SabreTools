using System.Collections.Generic;
using SabreTools.Core.Filter;
using SabreTools.DatFiles;
using SabreTools.IO.Logging;

namespace SabreTools.Filtering
{
    /// <summary>
    /// Represents the removal operations that need to be performed on a set of items, usually a DAT
    /// </summary>
    public class Remover
    {
        #region Fields

        /// <summary>
        /// List of header fields to exclude from writing
        /// </summary>
        public readonly List<string> HeaderFieldNames = [];

        /// <summary>
        /// List of machine fields to exclude from writing
        /// </summary>
        public readonly List<string> MachineFieldNames = [];

        /// <summary>
        /// List of fields to exclude from writing
        /// </summary>
        public readonly Dictionary<string, List<string>> ItemFieldNames = [];

        #endregion

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

        #region Population

        /// <summary>
        /// Populate the exclusion objects using a field name
        /// </summary>
        /// <param name="field">Field name</param>
        public void PopulateExclusions(string field)
            => PopulateExclusionsFromList([field]);

        /// <summary>
        /// Populate the exclusion objects using a set of field names
        /// </summary>
        /// <param name="fields">List of field names</param>
        public void PopulateExclusionsFromList(List<string>? fields)
        {
            // If the list is null or empty, just return
            if (fields == null || fields.Count == 0)
                return;

            var watch = new InternalStopwatch("Populating removals from list");

            foreach (string field in fields)
            {
                bool removerSet = SetRemover(field);
                if (!removerSet)
                    logger.Warning($"The value {field} did not match any known field names. Please check the wiki for more details on supported field names.");
            }

            watch.Stop();
        }

        /// <summary>
        /// Set remover from a value
        /// </summary>
        /// <param name="field">Key for the remover to be set</param>
        private bool SetRemover(string field)
        {
            // If the key is null or empty, return false
            if (string.IsNullOrEmpty(field))
                return false;

            // Get the parser pair out of it, if possible
            try
            {
                var key = new FilterKey(field);
                switch (key.ItemName)
                {
                    case Models.Metadata.MetadataFile.HeaderKey:
                        HeaderFieldNames.Add(key.FieldName);
                        return true;

                    case Models.Metadata.MetadataFile.MachineKey:
                        MachineFieldNames.Add(key.FieldName);
                        return true;

                    default:
                        if (!ItemFieldNames.ContainsKey(key.ItemName))
                            ItemFieldNames[key.ItemName] = [];

                        ItemFieldNames[key.ItemName].Add(key.FieldName);
                        return true;
                }
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Running

        /// <summary>
        /// Remove fields from a DatFile
        /// </summary>
        /// <param name="datFile">Current DatFile object to run operations on</param>
        public void ApplyRemovals(DatFile datFile)
        {
            InternalStopwatch watch = new("Applying removals to DAT");
            datFile.ApplyRemovals(HeaderFieldNames, MachineFieldNames, ItemFieldNames);
            watch.Stop();
        }

        #endregion
    }
}