using System;
using System.Collections.Generic;
using SabreTools.Logging;

namespace SabreTools.Core.Tools
{
    public static class Converters
    {
        #region String to Enum

        /// <summary>
        /// Get the LogLevel value for an input string, if possible
        /// </summary>
        /// <param name="value">String value to parse/param>
        /// <returns></returns>
        public static LogLevel AsLogLevel(this string? value)
        {
            return value?.ToLowerInvariant() switch
            {
                "verbose" => LogLevel.VERBOSE,
                "user" => LogLevel.USER,
                "warning" => LogLevel.WARNING,
                "error" => LogLevel.ERROR,
                _ => LogLevel.VERBOSE,
            };
        }

        /// <summary>
        /// Get bool? value from input string
        /// </summary>
        /// <param name="yesno">String to get value from</param>
        /// <returns>bool? corresponding to the string</returns>
        public static bool? AsYesNo(this string? yesno)
        {
            return yesno?.ToLowerInvariant() switch
            {
                "yes" or "true" => true,
                "no" or "false" => false,
                _ => null,
            };
        }

        /// <summary>
        /// Get the enum value for an input string, if possible
        /// </summary>
        /// <param name="value">String value to parse/param>
        /// <typeparam name="T">Enum type that is expected</typeparam>
        /// <returns>Enum value representing the input, default on error</returns>
        public static T? AsEnumValue<T>(this string? value)
        {
            // Get the mapping dictionary
            var mappings = GenerateToEnum<T>();

            // Normalize the input value
            value = value?.ToLowerInvariant();
            if (value == null)
                return default;

            // Try to get the value from the mappings
            if (mappings.ContainsKey(value))
                return mappings[value];

            // Otherwise, return the default value for the enum
            return default;
        }

        /// <summary>
        /// Get a set of mappings from strings to enum values
        /// </summary>
        /// <typeparam name="T">Enum type that is expected</typeparam>
        /// <returns>Dictionary of string to enum values</returns>
        internal static Dictionary<string, T> GenerateToEnum<T>()
        {
            try
            {
                // Get all of the values for the enum type
                var values = Enum.GetValues(typeof(T));

                // Build the output dictionary
                Dictionary<string, T> mappings = [];
                foreach (T? value in values)
                {
                    // If the value is null
                    if (value == null)
                        continue;

                    // Try to get the mapping attribute
                    MappingAttribute? attr = AttributeHelper<T>.GetAttribute(value);
                    if (attr?.Mappings == null || attr.Mappings.Length == 0)
                        continue;

                    // Loop through the mappings and add each
                    foreach (string mapString in attr.Mappings)
                    {
                        if (mapString != null)
                            mappings[mapString] = value;
                    }
                }

                // Return the output dictionary
                return mappings;
            }
            catch
            {
                // This should not happen, only if the type was not an enum
                return [];
            }
        }

        #endregion

        #region Enum to String

        /// <summary>
        /// Get string value from input bool?
        /// </summary>
        /// <param name="yesno">bool? to get value from</param>
        /// <returns>String corresponding to the bool?</returns>
        public static string? FromYesNo(this bool? yesno)
        {
            return yesno switch
            {
                true => "yes",
                false => "no",
                _ => null,
            };
        }

        /// <summary>
        /// Get the string value for an input enum, if possible
        /// </summary>
        /// <param name="value">Enum value to parse/param>
        /// <param name="useSecond">True to use the second mapping option, if it exists</param>
        /// <typeparam name="T">Enum type that is expected</typeparam>
        /// <returns>String value representing the input, default on error</returns>
        public static string? AsStringValue<T>(this T value, bool useSecond = false) where T : notnull
        {
            // Get the mapping dictionary
            var mappings = GenerateToString<T>(useSecond);

            // Try to get the value from the mappings
            if (mappings.ContainsKey(value))
                return mappings[value];

            // Otherwise, return null
            return null;
        }

        /// <summary>
        /// Get a set of mappings from enum values to string
        /// </summary>
        /// <param name="useSecond">True to use the second mapping option, if it exists</param>
        /// <typeparam name="T">Enum type that is expected</typeparam>
        /// <returns>Dictionary of enum to string values</returns>
        internal static Dictionary<T, string> GenerateToString<T>(bool useSecond) where T : notnull
        {
            try
            {
                // Get all of the values for the enum type
                var values = Enum.GetValues(typeof(T));

                // Build the output dictionary
                Dictionary<T, string> mappings = [];
                foreach (T? value in values)
                {
                    // If the value is null
                    if (value == null)
                        continue;

                    // Try to get the mapping attribute
                    MappingAttribute? attr = AttributeHelper<T>.GetAttribute(value);
                    if (attr?.Mappings == null || attr.Mappings.Length == 0)
                        continue;

                    // Use either the first or second item in the list
                    if (attr.Mappings.Length > 1 && useSecond)
                        mappings[value] = attr.Mappings[1];
                    else
                        mappings[value] = attr.Mappings[0];
                }

                // Return the output dictionary
                return mappings;
            }
            catch
            {
                // This should not happen, only if the type was not an enum
                return [];
            }
        }

        #endregion
    }
}
