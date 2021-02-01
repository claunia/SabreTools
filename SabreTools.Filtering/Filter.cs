using System;
using System.Collections.Generic;
using System.Linq;

using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.DatFiles;
using SabreTools.DatItems;
using SabreTools.Logging;

namespace SabreTools.Filtering
{
    /// <summary>
    /// Represents the filtering operations that need to be performed on a set of items, usually a DAT
    /// </summary>
    public class Filter
    {
        #region Constants

        #region Byte (1000-based) size comparisons

        private const long KiloByte = 1000;
        private readonly static long MegaByte = (long)Math.Pow(KiloByte, 2);
        private readonly static long GigaByte = (long)Math.Pow(KiloByte, 3);
        private readonly static long TeraByte = (long)Math.Pow(KiloByte, 4);
        private readonly static long PetaByte = (long)Math.Pow(KiloByte, 5);
        private readonly static long ExaByte = (long)Math.Pow(KiloByte, 6);
        private readonly static long ZettaByte = (long)Math.Pow(KiloByte, 7);
        private readonly static long YottaByte = (long)Math.Pow(KiloByte, 8);

        #endregion

        #region Byte (1024-based) size comparisons

        private const long KibiByte = 1024;
        private readonly static long MibiByte = (long)Math.Pow(KibiByte, 2);
        private readonly static long GibiByte = (long)Math.Pow(KibiByte, 3);
        private readonly static long TibiByte = (long)Math.Pow(KibiByte, 4);
        private readonly static long PibiByte = (long)Math.Pow(KibiByte, 5);
        private readonly static long ExiByte = (long)Math.Pow(KibiByte, 6);
        private readonly static long ZittiByte = (long)Math.Pow(KibiByte, 7);
        private readonly static long YittiByte = (long)Math.Pow(KibiByte, 8);

        #endregion

        #endregion

         #region Fields

        /// <summary>
        /// Filter for DatItem fields
        /// </summary>
        public DatItemFilter DatItemFilter { get; set; }

        /// <summary>
        /// Filter for Machine fields
        /// </summary>
        public MachineFilter MachineFilter { get; set; }

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
        public Filter()
        {
            logger = new Logger(this);
        }

        #endregion

        #region Population

        /// <summary>
        /// Populate the filters objects using a set of key:value filters
        /// </summary>
        /// <param name="filters">List of key:value where ~key/!key is negated</param>
        public void PopulateFiltersFromList(List<string> filters)
        {
            // Instantiate the filters, if necessary
            MachineFilter ??= new MachineFilter();
            DatItemFilter ??= new DatItemFilter();

            // If the list is null or empty, just return
            if (filters == null || filters.Count == 0)
                return;

            foreach (string filterPair in filters)
            {
                (string field, string value, bool negate) = ProcessFilterPair(filterPair);
                
                // If we don't even have a possible filter pair
                if (field == null && value == null)
                    continue;

                // Machine fields
                MachineField machineField = field.AsMachineField();
                if (machineField != MachineField.NULL)
                {
                    MachineFilter.SetFilter(machineField, value, negate);
                    continue;
                }

                // DatItem fields
                DatItemField datItemField = field.AsDatItemField();
                if (datItemField != DatItemField.NULL)
                {
                    DatItemFilter.SetFilter(datItemField, value, negate);
                    continue;
                }

                // If we didn't match anything, log an error
                logger.Warning($"The value {field} did not match any filterable field names. Please check the wiki for more details on supported field names.");
            }
        }

        /// <summary>
        /// Split the parts of a filter statement
        /// </summary>
        /// <param name="filter">key:value where ~key/!key is negated</param>
        protected (string field, string value, bool negate) ProcessFilterPair(string filter)
        {
            // If we don't even have a possible filter pair
            if (!filter.Contains(":"))
            {
                logger.Warning($"'{filter}` is not a valid filter string. Valid filter strings are of the form 'key:value'. Please refer to README.1ST or the help feature for more details.");
                return (null, null, false);
            }

            string filterTrimmed = filter.Trim('"', ' ', '\t');
            bool negate = filterTrimmed.StartsWith("!")
                || filterTrimmed.StartsWith("~")
                || filterTrimmed.StartsWith("not-");
            filterTrimmed = filterTrimmed.TrimStart('!', '~');
            filterTrimmed = filterTrimmed.StartsWith("not-") ? filterTrimmed[4..] : filterTrimmed;

            string filterFieldString = filterTrimmed.Split(':')[0].ToLowerInvariant().Trim('"', ' ', '\t');
            string filterValue = filterTrimmed[(filterFieldString.Length + 1)..].Trim('"', ' ', '\t');
        
            return (filterFieldString, filterValue, negate);
        }

        /// <summary>
        /// Set a bool? filter
        /// </summary>
        /// <param name="filterItem">FilterItem to populate</param>
        /// <param name="value">String value to add</param>
        /// <param name="negate">True to set negative filter, false otherwise</param>
        protected void SetBooleanFilter(FilterItem<bool?> filterItem, string value, bool negate)
        {
            if (negate || value.Equals("false", StringComparison.OrdinalIgnoreCase))
                filterItem.Neutral = false;
            else
                filterItem.Neutral = true;
        }

        /// <summary>
        /// Set a long? filter
        /// </summary>
        /// <param name="filterItem">FilterItem to populate</param>
        /// <param name="value">String value to add</param>
        /// <param name="negate">True to set negative filter, false otherwise</param>
        protected void SetDoubleFilter(FilterItem<double?> filterItem, string value, bool negate)
        {
            bool? operation = null;
            if (value.StartsWith(">"))
                operation = true;
            else if (value.StartsWith("<"))
                operation = false;
            else if (value.StartsWith("="))
                operation = null;

            string valueString = value.TrimStart('>', '<', '=');
            if (!Double.TryParse(valueString, out double valueDouble))
                return;

            // Equal
            if (operation == null && !negate)
            {
                filterItem.Neutral = valueDouble;
            }

            // Not Equal
            else if (operation == null && negate)
            {
                filterItem.Negative = valueDouble - 1;
                filterItem.Positive = valueDouble + 1;
            }

            // Greater Than or Equal
            else if (operation == true && !negate)
            {
                filterItem.Positive = valueDouble;
            }

            // Strictly Less Than
            else if (operation == true && negate)
            {
                filterItem.Negative = valueDouble - 1;
            }

            // Less Than or Equal
            else if (operation == false && !negate)
            {
                filterItem.Negative = valueDouble;
            }

            // Strictly Greater Than
            else if (operation == false && negate)
            {
                filterItem.Positive = valueDouble + 1;
            }
        }

        /// <summary>
        /// Set a long? filter
        /// </summary>
        /// <param name="filterItem">FilterItem to populate</param>
        /// <param name="value">String value to add</param>
        /// <param name="negate">True to set negative filter, false otherwise</param>
        protected void SetLongFilter(FilterItem<long?> filterItem, string value, bool negate)
        {
            bool? operation = null;
            if (value.StartsWith(">"))
                operation = true;
            else if (value.StartsWith("<"))
                operation = false;
            else if (value.StartsWith("="))
                operation = null;

            string valueString = value.TrimStart('>', '<', '=');
            long? valueLong = ToSize(valueString);
            if (valueLong == null)
                return;

            // Equal
            if (operation == null && !negate)
            {
                filterItem.Neutral = valueLong;
            }

            // Not Equal
            else if (operation == null && negate)
            {
                filterItem.Negative = valueLong - 1;
                filterItem.Positive = valueLong + 1;
            }

            // Greater Than or Equal
            else if (operation == true && !negate)
            {
                filterItem.Positive = valueLong;
            }

            // Strictly Less Than
            else if (operation == true && negate)
            {
                filterItem.Negative = valueLong - 1;
            }

            // Less Than or Equal
            else if (operation == false && !negate)
            {
                filterItem.Negative = valueLong;
            }

            // Strictly Greater Than
            else if (operation == false && negate)
            {
                filterItem.Positive = valueLong + 1;
            }
        }

        /// <summary>
        /// Set a string filter
        /// </summary>
        /// <param name="filterItem">FilterItem to populate</param>
        /// <param name="value">String value to add</param>
        /// <param name="negate">True to set negative filter, false otherwise</param>
        protected void SetStringFilter(FilterItem<string> filterItem, string value, bool negate)
        {
            if (negate)
                filterItem.NegativeSet.Add(value);
            else
                filterItem.PositiveSet.Add(value);
        }

        /// <summary>
        /// Get the multiplier to be used with the size given
        /// </summary>
        /// <param name="sizestring">String with possible size with extension</param>
        /// <returns>Tuple of multiplier to use on final size and fixed size string</returns>
        private static long? ToSize(string sizestring)
        {
            // If the string is null or empty, we return -1
            if (string.IsNullOrWhiteSpace(sizestring))
                return null;

            // Make sure the string is in lower case
            sizestring = sizestring.ToLowerInvariant();

            // Get any trailing size identifiers
            long multiplier = 1;
            if (sizestring.EndsWith("k") || sizestring.EndsWith("kb"))
                multiplier = KiloByte;
            else if (sizestring.EndsWith("ki") || sizestring.EndsWith("kib"))
                multiplier = KibiByte;
            else if (sizestring.EndsWith("m") || sizestring.EndsWith("mb"))
                multiplier = MegaByte;
            else if (sizestring.EndsWith("mi") || sizestring.EndsWith("mib"))
                multiplier = MibiByte;
            else if (sizestring.EndsWith("g") || sizestring.EndsWith("gb"))
                multiplier = GigaByte;
            else if (sizestring.EndsWith("gi") || sizestring.EndsWith("gib"))
                multiplier = GibiByte;
            else if (sizestring.EndsWith("t") || sizestring.EndsWith("tb"))
                multiplier = TeraByte;
            else if (sizestring.EndsWith("ti") || sizestring.EndsWith("tib"))
                multiplier = TibiByte;
            else if (sizestring.EndsWith("p") || sizestring.EndsWith("pb"))
                multiplier = PetaByte;
            else if (sizestring.EndsWith("pi") || sizestring.EndsWith("pib"))
                multiplier = PibiByte;
            else if (sizestring.EndsWith("e") || sizestring.EndsWith("eb"))
                multiplier = ExaByte;
            else if (sizestring.EndsWith("ei") || sizestring.EndsWith("eib"))
                multiplier = ExiByte;
            else if (sizestring.EndsWith("z") || sizestring.EndsWith("zb"))
                multiplier = ZettaByte;
            else if (sizestring.EndsWith("zi") || sizestring.EndsWith("zib"))
                multiplier = ZittiByte;
            else if (sizestring.EndsWith("y") || sizestring.EndsWith("yb"))
                multiplier = YottaByte;
            else if (sizestring.EndsWith("yi") || sizestring.EndsWith("yib"))
                multiplier = YittiByte;

            // Remove any trailing identifiers
            sizestring = sizestring.TrimEnd(new char[] { 'k', 'm', 'g', 't', 'p', 'e', 'z', 'y', 'i', 'b', ' ' });

            // Now try to get the size from the string
            if (!Int64.TryParse(sizestring, out long size))
                return null;
            else
                return size * multiplier;
        }

        #endregion

        #region Running

        /// <summary>
        /// Apply a set of Filters on the DatFile
        /// </summary>
        /// <param name="datFile">Current DatFile object to run operations on</param>
        /// <param name="perMachine">True if entire machines are considered, false otherwise (default)</param>
        /// <param name="throwOnError">True if the error that is thrown should be thrown back to the caller, false otherwise</param>
        /// <returns>True if the DatFile was filtered, false on error</returns>
        public bool ApplyFilters(DatFile datFile, bool perMachine = false, bool throwOnError = false)
        {
            // If we have null filters, return false
            if (MachineFilter == null || DatItemFilter == null)
                return false;

            // If we're filtering per machine, bucket by machine first
            if (perMachine)
                datFile.Items.BucketBy(ItemKey.Machine, DedupeType.None);

            try
            {
                // Loop over every key in the dictionary
                List<string> keys = datFile.Items.Keys.ToList();
                foreach (string key in keys)
                {
                    // For every item in the current key
                    bool machinePass = true;
                    List<DatItem> items = datFile.Items[key];
                    foreach (DatItem item in items)
                    {
                        // If we have a null item, we can't pass it
                        if (item == null)
                            continue;

                        // If the item is already filtered out, we skip
                        if (item.Remove)
                            continue;

                        // If the rom doesn't pass the filter, mark for removal
                        if (!PassesFilters(item))
                        {
                            item.Remove = true;

                            // If we're in machine mode, set and break
                            if (perMachine)
                            {
                                machinePass = false;
                                break;
                            }
                        }
                    }

                    // If we didn't pass and we're in machine mode, set all items as remove
                    if (perMachine && !machinePass)
                    {
                        foreach (DatItem item in items)
                        {
                            item.Remove = true;
                        }
                    }

                    // Assign back for caution
                    datFile.Items[key] = items;
                }
            }
            catch (Exception ex) when (!throwOnError)
            {
                logger.Error(ex);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Check to see if a DatItem passes the filters
        /// </summary>
        /// <param name="datItem">DatItem to check</param>
        /// <returns>True if the item passed the filter, false otherwise</returns>
        internal bool PassesFilters(DatItem datItem)
        {
            // Null item means it will never pass
            if (datItem == null)
                return false;

            // Filter on Machine fields
            if (!MachineFilter.PassesFilters(datItem.Machine))
                return false;

            // Filter on DatItem fields
            return DatItemFilter.PassesFilters(datItem);
        }

        /// <summary>
        /// Determines if a value passes a bool? filter
        /// </summary>
        /// <param name="filterItem">Filter item to check</param>
        /// <param name="value">Value to check</param>
        /// <returns>True if the value passes, false otherwise</returns>
        protected static bool PassBoolFilter(FilterItem<bool?> filterItem, bool? value)
        {
            if (filterItem.MatchesNeutral(null, value) == false)
                return false;

            return true;
        }

        /// <summary>
        /// Determines if a value passes a double? filter
        /// </summary>
        /// <param name="filterItem">Filter item to check</param>
        /// <param name="value">Value to check</param>
        /// <returns>True if the value passes, false otherwise</returns>
        protected static bool PassDoubleFilter(FilterItem<double?> filterItem, double? value)
        {
            if (filterItem.MatchesNeutral(null, value) == false)
                return false;
            else if (filterItem.MatchesPositive(null, value) == false)
                return false;
            else if (filterItem.MatchesNegative(null, value) == false)
                return false;

            return true;
        }

        /// <summary>
        /// Determines if a value passes a long? filter
        /// </summary>
        /// <param name="filterItem">Filter item to check</param>
        /// <param name="value">Value to check</param>
        /// <returns>True if the value passes, false otherwise</returns>
        protected static bool PassLongFilter(FilterItem<long?> filterItem, long? value)
        {
            if (filterItem.MatchesNeutral(null, value) == false)
                return false;
            else if (filterItem.MatchesPositive(null, value) == false)
                return false;
            else if (filterItem.MatchesNegative(null, value) == false)
                return false;

            return true;
        }

        /// <summary>
        /// Determines if a value passes a string filter
        /// </summary>
        /// <param name="filterItem">Filter item to check</param>
        /// <param name="value">Value to check</param>
        /// <returns>True if the value passes, false otherwise</returns>
        protected static bool PassStringFilter(FilterItem<string> filterItem, string value)
        {
            if (filterItem.MatchesPositiveSet(value) == false)
                return false;
            if (filterItem.MatchesNegativeSet(value) == true)
                return false;

            return true;
        }

        #endregion
    }
}
