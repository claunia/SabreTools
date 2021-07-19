using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SabreTools.Core;
using SabreTools.DatFiles;
using SabreTools.DatItems;
using SabreTools.Logging;

namespace SabreTools.Filtering
{
    /// <summary>
    /// Represents the removal operations that need to be performed on a set of items, usually a DAT
    /// </summary>
    public class Remover
    {
        #region Fields

        /// <summary>
        /// DatItemRemover to remove fields from DatHeaders
        /// </summary>
        public DatHeaderRemover DatHeaderRemover { get; set; }

        /// <summary>
        /// DatItemRemover to remove fields from DatItems
        /// </summary>
        public DatItemRemover DatItemRemover { get; set; }

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
        /// Populate the exclusion objects using a set of field names
        /// </summary>
        /// <param name="fields">List of field names</param>
        public void PopulateExclusionsFromList(List<string> fields)
        {
            // Instantiate the removers, if necessary
            DatHeaderRemover ??= new DatHeaderRemover();
            DatItemRemover ??= new DatItemRemover();

            // If the list is null or empty, just return
            if (fields == null || fields.Count == 0)
                return;

            InternalStopwatch watch = new InternalStopwatch("Populating removals from list");

            foreach (string field in fields)
            {                
                // If we don't even have a possible field name
                if (field == null)
                    continue;

                // DatHeader fields
                if (DatHeaderRemover.SetRemover(field))
                    continue;

                // Machine and DatItem fields
                if (DatItemRemover.SetRemover(field))
                    continue;

                // If we didn't match anything, log an error
                logger.Warning($"The value {field} did not match any known field names. Please check the wiki for more details on supported field names.");
            }

            watch.Stop();
        }

        #endregion

        #region Running

        /// <summary>
        /// Remove fields from a DatFile
        /// </summary>
        /// <param name="datFile">Current DatFile object to run operations on</param>
        public void ApplyRemovals(DatFile datFile)
        {
            // If the removers don't exist, we can't use it
            if (DatHeaderRemover == null && DatItemRemover == null)
                return;

            InternalStopwatch watch = new InternalStopwatch("Applying removals to DAT");

            // Remove DatHeader fields
            if (DatHeaderRemover != null && DatHeaderRemover.DatHeaderFields.Any())
                DatHeaderRemover.RemoveFields(datFile.Header);

            // Remove DatItem and Machine fields
            if (DatItemRemover != null && (DatItemRemover.MachineFields.Any() || DatItemRemover.DatItemFields.Any()))
            {
                Parallel.ForEach(datFile.Items.Keys, Globals.ParallelOptions, key =>
                {
                    ConcurrentList<DatItem> items = datFile.Items[key];
                    for (int j = 0; j < items.Count; j++)
                    {
                        DatItemRemover.RemoveFields(items[j]);
                    }

                    datFile.Items.Remove(key);
                    datFile.Items.AddRange(key, items);
                });
            }

            watch.Stop();
        }

        #endregion
    }
}