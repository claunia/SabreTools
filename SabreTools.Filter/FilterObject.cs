using System;
using SabreTools.Models.Internal;

namespace SabreTools.Filter
{
    /// <summary>
    /// Represents a single filtering object
    /// </summary>
    public class FilterObject
    {
        /// <summary>
        /// Key name for the filter
        /// </summary>
        public string Key { get; init; }

        /// <summary>
        /// Value to match in the filter
        /// </summary>
        public string? Value { get; init; }

        /// <summary>
        /// Operation on how to match the filter
        /// </summary>
        public Operation Operation { get; init; }

        public FilterObject(string keyValue, string? operation)
        {
            (string? itemName, string? fieldName) = FilterParser.ParseFilterId(keyValue);
            if (itemName == null)
                throw new ArgumentOutOfRangeException(nameof(keyValue));

            this.Key = itemName;
            this.Value = fieldName;
            this.Operation = GetOperation(operation);
        }

        public FilterObject(string keyValue, Operation operation)
        {
            (string? itemName, string? fieldName) = FilterParser.ParseFilterId(keyValue);
            if (itemName == null)
                throw new ArgumentOutOfRangeException(nameof(keyValue));

            this.Key = itemName;
            this.Value = fieldName;
            this.Operation = operation;
        }

        public FilterObject(string key, string? value, string? operation)
        {
            (string? itemName, string? fieldName) = FilterParser.ParseFilterId(key, value);
            if (itemName == null)
                throw new ArgumentOutOfRangeException(nameof(fieldName));

            this.Key = itemName;
            this.Value = fieldName;
            this.Operation = GetOperation(operation);
        }

        public FilterObject(string key, string? value, Operation operation)
        {
            (string? itemName, string? fieldName) = FilterParser.ParseFilterId(key, value);
            if (itemName == null)
                throw new ArgumentOutOfRangeException(nameof(fieldName));

            this.Key = itemName;
            this.Value = fieldName;
            this.Operation = operation;
        }

        /// <summary>
        /// Derive an operation from the input string, if possible
        /// </summary>
        private static Operation GetOperation(string? operation)
        {
            return operation?.ToLowerInvariant() switch
            {
                "=" => Operation.Equals,
                "==" => Operation.Equals,
                ":" => Operation.Equals,
                "::" => Operation.Equals,

                "!" => Operation.NotEquals,
                "!=" => Operation.NotEquals,
                "!:" => Operation.NotEquals,

                ">" => Operation.GreaterThan,
                ">=" => Operation.GreaterThanOrEqual,

                "<" => Operation.LessThan,
                "<=" => Operation.LessThanOrEqual,

                _ => Operation.NONE,
            };
        }

        #region Matching

        /// <summary>
        /// Determine if a DictionaryBase object matches the key and value
        /// </summary>
        public bool Matches(DictionaryBase dictionaryBase)
        {
            return this.Operation switch
            {
                Operation.Equals => MatchesEqual(dictionaryBase),
                Operation.NotEquals => MatchesNotEqual(dictionaryBase),
                Operation.GreaterThan => MatchesGreaterThan(dictionaryBase),
                Operation.GreaterThanOrEqual => MatchesGreaterThanOrEqual(dictionaryBase),
                Operation.LessThan => MatchesLessThan(dictionaryBase),
                Operation.LessThanOrEqual => MatchesLessThanOrEqual(dictionaryBase),
                _ => false,
            };
        }

        /// <summary>
        /// Determines if a value matches exactly
        /// </summary>
        /// <remarks>TODO: Add regex matching to this method</remarks>
        private bool MatchesEqual(DictionaryBase dictionaryBase)
        {
            if (!dictionaryBase.ContainsKey(this.Key))
                return this.Value == null;

            string? checkValue = dictionaryBase.ReadString(this.Key);
            return checkValue == this.Value;
        }

        /// <summary>
        /// Determines if a value does not match exactly
        /// </summary>
        /// <remarks>TODO: Add regex matching to this method</remarks>
        private bool MatchesNotEqual(DictionaryBase dictionaryBase)
        {
            if (!dictionaryBase.ContainsKey(this.Key))
                return this.Value != null;

            string? checkValue = dictionaryBase.ReadString(this.Key);
            return checkValue != this.Value;
        }

        /// <summary>
        /// Determines if a value is strictly greater than
        /// </summary>
        private bool MatchesGreaterThan(DictionaryBase dictionaryBase)
        {
            if (!dictionaryBase.ContainsKey(this.Key))
                return false;

            long? checkLongValue = dictionaryBase.ReadLong(this.Key);
            if (checkLongValue != null)
            {
                if (!long.TryParse(this.Value, out long matchValue))
                    return false;

                return checkLongValue > matchValue;
            }

            double? checkDoubleValue = dictionaryBase.ReadDouble(this.Key);
            if (checkDoubleValue != null)
            {
                if (!double.TryParse(this.Value, out double matchValue))
                    return false;

                return checkDoubleValue > matchValue;
            }

            return false;
        }

        /// <summary>
        /// Determines if a value is greater than or equal
        /// </summary>
        private bool MatchesGreaterThanOrEqual(DictionaryBase dictionaryBase)
        {
            if (!dictionaryBase.ContainsKey(this.Key))
                return false;

            long? checkLongValue = dictionaryBase.ReadLong(this.Key);
            if (checkLongValue != null)
            {
                if (!long.TryParse(this.Value, out long matchValue))
                    return false;

                return checkLongValue >= matchValue;
            }

            double? checkDoubleValue = dictionaryBase.ReadDouble(this.Key);
            if (checkDoubleValue != null)
            {
                if (!double.TryParse(this.Value, out double matchValue))
                    return false;

                return checkDoubleValue >= matchValue;
            }

            return false;
        }
    
        /// <summary>
        /// Determines if a value is strictly less than
        /// </summary>
        private bool MatchesLessThan(DictionaryBase dictionaryBase)
        {
            if (!dictionaryBase.ContainsKey(this.Key))
                return false;

            long? checkLongValue = dictionaryBase.ReadLong(this.Key);
            if (checkLongValue != null)
            {
                if (!long.TryParse(this.Value, out long matchValue))
                    return false;

                return checkLongValue < matchValue;
            }

            double? checkDoubleValue = dictionaryBase.ReadDouble(this.Key);
            if (checkDoubleValue != null)
            {
                if (!double.TryParse(this.Value, out double matchValue))
                    return false;

                return checkDoubleValue < matchValue;
            }

            return false;
        }

        /// <summary>
        /// Determines if a value is less than or equal
        /// </summary>
        private bool MatchesLessThanOrEqual(DictionaryBase dictionaryBase)
        {
            if (!dictionaryBase.ContainsKey(this.Key))
                return false;

            long? checkLongValue = dictionaryBase.ReadLong(this.Key);
            if (checkLongValue != null)
            {
                if (!long.TryParse(this.Value, out long matchValue))
                    return false;

                return checkLongValue <= matchValue;
            }

            double? checkDoubleValue = dictionaryBase.ReadDouble(this.Key);
            if (checkDoubleValue != null)
            {
                if (!double.TryParse(this.Value, out double matchValue))
                    return false;

                return checkDoubleValue <= matchValue;
            }

            return false;
        }
    
        #endregion
    }
}