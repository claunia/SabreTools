using System;
using System.Collections.Generic;

using SabreTools.Logging;

namespace SabreTools.Filtering
{
    /// <summary>
    /// Represents the filtering operations that need to be performed on a set of items, usually a DAT
    /// </summary>
    public abstract class Filter
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
        public Filter()
        {
            logger = new Logger(this);
        }

        #endregion

        #region Filter Population

        /// <summary>
        /// Populate the filters object using a set of key:value filters
        /// </summary>
        /// <param name="filters">List of key:value where ~key/!key is negated</param>
        public abstract void PopulateFromList(List<string> filters);

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
            filterTrimmed = filterTrimmed.StartsWith("not-") ? filterTrimmed.Substring(4) : filterTrimmed;

            string filterFieldString = filterTrimmed.Split(':')[0].ToLowerInvariant().Trim('"', ' ', '\t');
            string filterValue = filterTrimmed.Substring(filterFieldString.Length + 1).Trim('"', ' ', '\t');
        
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
            if (!Int64.TryParse(valueString, out long valueLong))
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

        #endregion

        #region Filter Running

        /// <summary>
        /// Determines if a value passes a bool? filter
        /// </summary>
        /// <param name="filterItem">Filter item to check</param>
        /// <param name="value">Value to check</param>
        /// <returns>True if the value passes, false otherwise</returns>
        public static bool PassBoolFilter(FilterItem<bool?> filterItem, bool? value)
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
        public static bool PassDoubleFilter(FilterItem<double?> filterItem, double? value)
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
        public static bool PassLongFilter(FilterItem<long?> filterItem, long? value)
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
        public static bool PassStringFilter(FilterItem<string> filterItem, string value)
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
