using System;
using System.Text.RegularExpressions;
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
        public string[] Key { get; init; }

        /// <summary>
        /// Value to match in the filter
        /// </summary>
        public string? Value { get; init; }

        /// <summary>
        /// Operation on how to match the filter
        /// </summary>
        public Operation Operation { get; init; }

        public FilterObject(string filterString)
        {
            (string? keyItem, Operation operation, string? value) = SplitFilterString(filterString);
            if (keyItem == null)
                throw new ArgumentOutOfRangeException(nameof(filterString));

            (string? itemName, string? fieldName) = FilterParser.ParseFilterId(keyItem);
            if (itemName == null || fieldName == null)
                throw new ArgumentOutOfRangeException(nameof(filterString));

            this.Key = new string[] { itemName, fieldName };
            this.Value = value;
            this.Operation = operation;
        }

        public FilterObject(string itemField, string? value, string? operation)
        {
            (string? itemName, string? fieldName) = FilterParser.ParseFilterId(itemField);
            if (itemName == null || fieldName == null)
                throw new ArgumentOutOfRangeException(nameof(value));

            this.Key = new string[] { itemName, fieldName };
            this.Value = value;
            this.Operation = GetOperation(operation);
        }

        public FilterObject(string itemField, string? value, Operation operation)
        {
            (string? itemName, string? fieldName) = FilterParser.ParseFilterId(itemField);
            if (itemName == null || fieldName == null)
                throw new ArgumentOutOfRangeException(nameof(value));

            this.Key = new string[] { itemName, fieldName };
            this.Value = value;
            this.Operation = operation;
        }

        #region Matching

        /// <summary>
        /// Determine if a DictionaryBase object matches the key and value
        /// </summary>
        public bool Matches(DictionaryBase dictionaryBase)
        {
            // TODO: Add validation of dictionary base type from this.Key[0]
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
        /// <remarks>
        /// TODO: Add regex matching to this method
        /// TODO: Add logic to convert SI suffixes and hex
        /// </remarks>
        private bool MatchesEqual(DictionaryBase dictionaryBase)
        {
            if (!dictionaryBase.ContainsKey(this.Key[1]))
                return this.Value == null;

            string? checkValue = dictionaryBase.ReadString(this.Key[1]);
            return checkValue == this.Value;
        }

        /// <summary>
        /// Determines if a value does not match exactly
        /// </summary>
        /// <remarks>
        /// TODO: Add regex matching to this method
        /// TODO: Add logic to convert SI suffixes and hex
        /// </remarks>
        private bool MatchesNotEqual(DictionaryBase dictionaryBase)
        {
            if (!dictionaryBase.ContainsKey(this.Key[1]))
                return this.Value != null;

            string? checkValue = dictionaryBase.ReadString(this.Key[1]);
            return checkValue != this.Value;
        }

        /// <summary>
        /// Determines if a value is strictly greater than
        /// </summary>
        /// <remarks>TODO: Add logic to convert SI suffixes and hex</remarks>
        private bool MatchesGreaterThan(DictionaryBase dictionaryBase)
        {
            if (!dictionaryBase.ContainsKey(this.Key[1]))
                return false;

            long? checkLongValue = dictionaryBase.ReadLong(this.Key[1]);
            if (checkLongValue != null)
            {
                if (!long.TryParse(this.Value, out long matchValue))
                    return false;

                return checkLongValue > matchValue;
            }

            double? checkDoubleValue = dictionaryBase.ReadDouble(this.Key[1]);
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
        /// <remarks>TODO: Add logic to convert SI suffixes and hex</remarks>
        private bool MatchesGreaterThanOrEqual(DictionaryBase dictionaryBase)
        {
            if (!dictionaryBase.ContainsKey(this.Key[1]))
                return false;

            long? checkLongValue = dictionaryBase.ReadLong(this.Key[1]);
            if (checkLongValue != null)
            {
                if (!long.TryParse(this.Value, out long matchValue))
                    return false;

                return checkLongValue >= matchValue;
            }

            double? checkDoubleValue = dictionaryBase.ReadDouble(this.Key[1]);
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
        /// <remarks>TODO: Add logic to convert SI suffixes and hex</remarks>
        private bool MatchesLessThan(DictionaryBase dictionaryBase)
        {
            if (!dictionaryBase.ContainsKey(this.Key[1]))
                return false;

            long? checkLongValue = dictionaryBase.ReadLong(this.Key[1]);
            if (checkLongValue != null)
            {
                if (!long.TryParse(this.Value, out long matchValue))
                    return false;

                return checkLongValue < matchValue;
            }

            double? checkDoubleValue = dictionaryBase.ReadDouble(this.Key[1]);
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
        /// <remarks>TODO: Add logic to convert SI suffixes and hex</remarks>
        private bool MatchesLessThanOrEqual(DictionaryBase dictionaryBase)
        {
            if (!dictionaryBase.ContainsKey(this.Key[1]))
                return false;

            long? checkLongValue = dictionaryBase.ReadLong(this.Key[1]);
            if (checkLongValue != null)
            {
                if (!long.TryParse(this.Value, out long matchValue))
                    return false;

                return checkLongValue <= matchValue;
            }

            double? checkDoubleValue = dictionaryBase.ReadDouble(this.Key[1]);
            if (checkDoubleValue != null)
            {
                if (!double.TryParse(this.Value, out double matchValue))
                    return false;

                return checkDoubleValue <= matchValue;
            }

            return false;
        }

        #endregion

        #region Helpers

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

        /// <summary>
        /// Derive a key, operation, and value from the input string, if possible
        /// </summary>
        private static (string?, Operation, string?) SplitFilterString(string? filterString)
        {
            if (filterString == null)
                return (null, Operation.NONE, null);

            // Split the string using regex
            var match = Regex.Match(filterString, @"^(?<itemField>[a-zA-Z.]+)(?<operation>[=!:><]{1,2})(?<value>.*)$");
            if (!match.Success)
                return (null, Operation.NONE, null);

            string itemField = match.Groups["itemField"].Value;
            Operation operation = GetOperation(match.Groups["operation"].Value);
            string value = match.Groups["value"].Value;

            return (itemField, operation, value);
        }

        #endregion
    }
}