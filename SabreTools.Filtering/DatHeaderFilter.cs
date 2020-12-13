using System.Collections.Generic;

using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.Logging;

namespace SabreTools.Filtering
{
    /// <summary>
    /// Represents the filtering operations that need to be performed on a DatHeader
    /// </summary>
    /// TODO: Investigate how to reduce the amount of hardcoded filter statements
    /// TODO: Add DatHeader filters
    public class DatHeaderFilter : Filter
    {
        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public DatHeaderFilter()
        {
            logger = new Logger(this);
        }

        #endregion

        #region Filter Population

        /// <summary>
        /// Populate the filters object using a set of key:value filters
        /// </summary>
        /// <param name="filters">List of key:value where ~key/!key is negated</param>
        public override void PopulateFromList(List<string> filters)
        {
            foreach (string filterPair in filters)
            {
                (string field, string value, bool negate) = ProcessFilterPair(filterPair);
                
                // If we don't even have a possible filter pair
                if (field == null && value == null)
                    continue;

                DatHeaderField filterField = field.AsDatHeaderField();
                SetFilter(filterField, value, negate);
            }
        }

        /// <summary>
        /// Set multiple filters from key
        /// </summary>
        /// <param name="key">Key for the filter to be set</param>
        /// <param name="values">List of values for the filter</param>
        /// <param name="negate">True if negative filter, false otherwise</param>
        public void SetFilter(DatHeaderField key, List<string> values, bool negate)
        {
            foreach (string value in values)
            {
                SetFilter(key, value, negate);
            }
        }

        /// <summary>
        /// Set a single filter from key
        /// </summary>
        /// <param name="key">Key for the filter to be set</param>
        /// <param name="value">Value of the filter</param>
        /// <param name="negate">True if negative filter, false otherwise</param>
        public void SetFilter(DatHeaderField key, string value, bool negate)
        {
            switch (key)
            {
                // TODO: Add DatHeader filters
            }
        }

        #endregion
    }
}
