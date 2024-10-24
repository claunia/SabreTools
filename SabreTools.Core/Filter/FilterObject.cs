using System;
using System.Text.RegularExpressions;
using SabreTools.Core.Tools;
using SabreTools.Models.Metadata;

namespace SabreTools.Core.Filter
{
    /// <summary>
    /// Represents a single filtering object
    /// </summary>
    /// <remarks>TODO: Add ability to have a set of values that are accepted</remarks>
    public class FilterObject
    {
        /// <summary>
        /// Item key associated with the filter
        /// </summary>
        public readonly FilterKey Key;

        /// <summary>
        /// Value to match in the filter
        /// </summary>
        public readonly string? Value;

        /// <summary>
        /// Operation on how to match the filter
        /// </summary>
        public readonly Operation Operation;

        public FilterObject(string filterString)
        {
            if (!SplitFilterString(filterString, out var keyItem, out Operation operation, out var value))
                throw new ArgumentOutOfRangeException(nameof(filterString));

            if (!FilterParser.ParseFilterId(keyItem, out string itemName, out string fieldName))
                throw new ArgumentOutOfRangeException(nameof(filterString));

            Key = new FilterKey(itemName, fieldName);
            Value = value;
            Operation = operation;
        }

        public FilterObject(string itemField, string? value, string? operation)
        {
            if (!FilterParser.ParseFilterId(itemField, out string itemName, out string fieldName))
                throw new ArgumentOutOfRangeException(nameof(value));

            Key = new FilterKey(itemName, fieldName);
            Value = value;
            Operation = GetOperation(operation);
        }

        public FilterObject(string itemField, string? value, Operation operation)
        {
            if (!FilterParser.ParseFilterId(itemField, out string itemName, out string fieldName))
                throw new ArgumentOutOfRangeException(nameof(value));

            Key = new FilterKey(itemName, fieldName);
            Value = value;
            Operation = operation;
        }

        #region Matching

        /// <summary>
        /// Determine if a DictionaryBase object matches the key and value
        /// </summary>
        public bool Matches(DictionaryBase dictionaryBase)
        {
            // TODO: Add validation of dictionary base type from the key values
            return Operation switch
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
        private bool MatchesEqual(DictionaryBase dictionaryBase)
        {
            // If the key doesn't exist, we count it as null
            if (!dictionaryBase.ContainsKey(Key.FieldName))
                return Value == null;

            // If the value in the dictionary is null
            string? checkValue = dictionaryBase.ReadString(Key.FieldName);
            if (checkValue == null)
                return Value == null;

            // If we have both a potentally boolean check and value
            bool? checkValueBool = ConvertToBoolean(checkValue);
            bool? matchValueBool = ConvertToBoolean(Value);
            if (checkValueBool != null && matchValueBool != null)
                return checkValueBool == matchValueBool;

            // If we have both a potentially numeric check and value
            if (NumberHelper.IsNumeric(checkValue) && NumberHelper.IsNumeric(Value))
            {
                // Check Int64 values
                long? checkValueLong = NumberHelper.ConvertToInt64(checkValue);
                long? matchValueLong = NumberHelper.ConvertToInt64(Value);
                if (checkValueLong != null && matchValueLong != null)
                    return checkValueLong == matchValueLong;

                // Check Double values
                double? checkValueDouble = NumberHelper.ConvertToDouble(checkValue);
                double? matchValueDouble = NumberHelper.ConvertToDouble(Value);
                if (checkValueDouble != null && matchValueDouble != null)
                    return checkValueDouble == matchValueDouble;
            }

            // If the value might contain valid Regex
            if (Value != null && ContainsRegex(Value))
                return Regex.IsMatch(checkValue, Value);

            return string.Equals(checkValue, Value, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Determines if a value does not match exactly
        /// </summary>
        private bool MatchesNotEqual(DictionaryBase dictionaryBase)
        {
            // If the key doesn't exist, we count it as null
            if (!dictionaryBase.ContainsKey(Key.FieldName))
                return Value != null;

            // If the value in the dictionary is null
            string? checkValue = dictionaryBase.ReadString(Key.FieldName);
            if (checkValue == null)
                return Value == null;

            // If we have both a potentally boolean check and value
            bool? checkValueBool = ConvertToBoolean(checkValue);
            bool? matchValueBool = ConvertToBoolean(Value);
            if (checkValueBool != null && matchValueBool != null)
                return checkValueBool != matchValueBool;

            // If we have both a potentially numeric check and value
            if (NumberHelper.IsNumeric(checkValue) && NumberHelper.IsNumeric(Value))
            {
                // Check Int64 values
                long? checkValueLong = NumberHelper.ConvertToInt64(checkValue);
                long? matchValueLong = NumberHelper.ConvertToInt64(Value);
                if (checkValueLong != null && matchValueLong != null)
                    return checkValueLong != matchValueLong;

                // Check Double values
                double? checkValueDouble = NumberHelper.ConvertToDouble(checkValue);
                double? matchValueDouble = NumberHelper.ConvertToDouble(Value);
                if (checkValueDouble != null && matchValueDouble != null)
                    return checkValueDouble != matchValueDouble;
            }

            // If the value might contain valid Regex
            if (Value != null && ContainsRegex(Value))
                return !Regex.IsMatch(checkValue, Value);

            return !string.Equals(checkValue, Value, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Determines if a value is strictly greater than
        /// </summary>
        private bool MatchesGreaterThan(DictionaryBase dictionaryBase)
        {
            // If the key doesn't exist, we count it as null
            if (!dictionaryBase.ContainsKey(Key.FieldName))
                return false;

            // If the value in the dictionary is null
            string? checkValue = dictionaryBase.ReadString(Key.FieldName);
            if (checkValue == null)
                return false;

            // If we have both a potentially numeric check and value
            if (NumberHelper.IsNumeric(checkValue) && NumberHelper.IsNumeric(Value))
            {
                // Check Int64 values
                long? checkValueLong = NumberHelper.ConvertToInt64(checkValue);
                long? matchValueLong = NumberHelper.ConvertToInt64(Value);
                if (checkValueLong != null && matchValueLong != null)
                    return checkValueLong > matchValueLong;

                // Check Double values
                double? checkValueDouble = NumberHelper.ConvertToDouble(checkValue);
                double? matchValueDouble = NumberHelper.ConvertToDouble(Value);
                if (checkValueDouble != null && matchValueDouble != null)
                    return checkValueDouble > matchValueDouble;
            }

            return false;
        }

        /// <summary>
        /// Determines if a value is greater than or equal
        /// </summary>
        private bool MatchesGreaterThanOrEqual(DictionaryBase dictionaryBase)
        {
            // If the key doesn't exist, we count it as null
            if (!dictionaryBase.ContainsKey(Key.FieldName))
                return false;

            // If the value in the dictionary is null
            string? checkValue = dictionaryBase.ReadString(Key.FieldName);
            if (checkValue == null)
                return false;

            // If we have both a potentially numeric check and value
            if (NumberHelper.IsNumeric(checkValue) && NumberHelper.IsNumeric(Value))
            {
                // Check Int64 values
                long? checkValueLong = NumberHelper.ConvertToInt64(checkValue);
                long? matchValueLong = NumberHelper.ConvertToInt64(Value);
                if (checkValueLong != null && matchValueLong != null)
                    return checkValueLong >= matchValueLong;

                // Check Double values
                double? checkValueDouble = NumberHelper.ConvertToDouble(checkValue);
                double? matchValueDouble = NumberHelper.ConvertToDouble(Value);
                if (checkValueDouble != null && matchValueDouble != null)
                    return checkValueDouble >= matchValueDouble;
            }

            return false;
        }

        /// <summary>
        /// Determines if a value is strictly less than
        /// </summary>
        private bool MatchesLessThan(DictionaryBase dictionaryBase)
        {
            // If the key doesn't exist, we count it as null
            if (!dictionaryBase.ContainsKey(Key.FieldName))
                return false;

            // If the value in the dictionary is null
            string? checkValue = dictionaryBase.ReadString(Key.FieldName);
            if (checkValue == null)
                return false;

            // If we have both a potentially numeric check and value
            if (NumberHelper.IsNumeric(checkValue) && NumberHelper.IsNumeric(Value))
            {
                // Check Int64 values
                long? checkValueLong = NumberHelper.ConvertToInt64(checkValue);
                long? matchValueLong = NumberHelper.ConvertToInt64(Value);
                if (checkValueLong != null && matchValueLong != null)
                    return checkValueLong < matchValueLong;

                // Check Double values
                double? checkValueDouble = NumberHelper.ConvertToDouble(checkValue);
                double? matchValueDouble = NumberHelper.ConvertToDouble(Value);
                if (checkValueDouble != null && matchValueDouble != null)
                    return checkValueDouble < matchValueDouble;
            }

            return false;
        }

        /// <summary>
        /// Determines if a value is less than or equal
        /// </summary>
        private bool MatchesLessThanOrEqual(DictionaryBase dictionaryBase)
        {
            // If the key doesn't exist, we count it as null
            if (!dictionaryBase.ContainsKey(Key.FieldName))
                return false;

            // If the value in the dictionary is null
            string? checkValue = dictionaryBase.ReadString(Key.FieldName);
            if (checkValue == null)
                return false;

            // If we have both a potentially numeric check and value
            if (NumberHelper.IsNumeric(checkValue) && NumberHelper.IsNumeric(Value))
            {
                // Check Int64 values
                long? checkValueLong = NumberHelper.ConvertToInt64(checkValue);
                long? matchValueLong = NumberHelper.ConvertToInt64(Value);
                if (checkValueLong != null && matchValueLong != null)
                    return checkValueLong <= matchValueLong;

                // Check Double values
                double? checkValueDouble = NumberHelper.ConvertToDouble(checkValue);
                double? matchValueDouble = NumberHelper.ConvertToDouble(Value);
                if (checkValueDouble != null && matchValueDouble != null)
                    return checkValueDouble <= matchValueDouble;
            }

            return false;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Determine if a value may contain regex for matching
        /// </summary>
        /// <remarks>
        /// If a value contains one of the following characters:
        ///     ^ $ * ? +
        /// Then it will attempt to check if the value is regex or not.
        /// If none of those characters exist, then value will assumed
        /// not to be regex.
        /// </remarks>
        private static bool ContainsRegex(string? value)
        {
            // If the value is missing, it can't be regex
            if (value == null)
                return false;

            // If we find a special character, try parsing as regex
#if NETFRAMEWORK
            if (value.Contains("^")
                || value.Contains("$")
                || value.Contains("*")
                || value.Contains("?")
                || value.Contains("+"))
#else
            if (value.Contains('^')
                || value.Contains('$')
                || value.Contains('*')
                || value.Contains('?')
                || value.Contains('+'))
#endif
            {
                try
                {
                    _ = new Regex(value);
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// Convert a string to a Boolean
        /// </summary>
        private bool? ConvertToBoolean(string? value)
        {
            // If we don't have a valid string, we can't do anything
            if (string.IsNullOrEmpty(value))
                return null;

            return value!.ToLowerInvariant() switch
            {
                "true" or "yes" => true,
                "false" or "no" => false,
                _ => null,
            };
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

        /// <summary>
        /// Derive a key, operation, and value from the input string, if possible
        /// </summary>
        private static bool SplitFilterString(string? filterString, out string? key, out Operation operation, out string? value)
        {
            // Set default values
            key = null; operation = Operation.NONE; value = null;

            if (filterString == null)
                return false;

            // Trim quotations, if necessary
            if (filterString.StartsWith("\""))
                filterString = filterString.Substring(1, filterString.Length - 2);

            // Split the string using regex
            var match = Regex.Match(filterString, @"^(?<itemField>[a-zA-Z.]+)(?<operation>[=!:><]{1,2})(?<value>.*)$");
            if (!match.Success)
                return false;

            key = match.Groups["itemField"].Value;
            operation = GetOperation(match.Groups["operation"].Value);
            value = match.Groups["value"].Value;

            return true;
        }

        #endregion
    }
}