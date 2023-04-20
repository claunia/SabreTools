using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SabreTools.Core.Tools
{
    public static class Converters
    {
        #region Enum to Enum

        /// <summary>
        /// Get the DatItemFields associated with each hash type
        /// </summary>
        public static List<DatItemField> AsDatItemFields(this Hash hash)
        {
            List<DatItemField> fields = new();

            if (hash.HasFlag(Hash.CRC))
                fields.Add(DatItemField.CRC);
            if (hash.HasFlag(Hash.MD5))
                fields.Add(DatItemField.MD5);
            if (hash.HasFlag(Hash.SHA1))
                fields.Add(DatItemField.SHA1);
            if (hash.HasFlag(Hash.SHA256))
                fields.Add(DatItemField.SHA256);
            if (hash.HasFlag(Hash.SHA384))
                fields.Add(DatItemField.SHA384);
            if (hash.HasFlag(Hash.SHA512))
                fields.Add(DatItemField.SHA512);
            if (hash.HasFlag(Hash.SpamSum))
                fields.Add(DatItemField.SpamSum);

            return fields;
        }

        #endregion

        #region String to Enum

        /// <summary>
        /// Get ChipType value from input string
        /// </summary>
        /// <param name="chipType">String to get value from</param>
        /// <returns>ChipType value corresponding to the string</returns>
        public static ChipType AsChipType(this string chipType)
            => AsEnumValue<ChipType>(chipType);

        /// <summary>
        /// Get ControlType value from input string
        /// </summary>
        /// <param name="controlType">String to get value from</param>
        /// <returns>ControlType value corresponding to the string</returns>
        public static ControlType AsControlType(this string controlType)
            => AsEnumValue<ControlType>(controlType);

        /// <summary>
        /// Get DatHeaderField value from input string
        /// </summary>
        /// <param name="DatHeaderField">String to get value from</param>
        /// <returns>DatHeaderField value corresponding to the string</returns>
        public static DatHeaderField AsDatHeaderField(this string input)
        {
            // If the input is empty, we return null
            if (string.IsNullOrEmpty(input))
                return DatHeaderField.NULL;

            // Normalize the input
            input = input.ToLowerInvariant();

            // Create regex
            string headerRegex = @"^(dat|header|datheader)[.\-_\s]";

            // If we don't have a header field, skip
            if (!Regex.IsMatch(input, headerRegex))
                return DatHeaderField.NULL;

            // Replace the match and re-normalize
            string headerInput = Regex.Replace(input, headerRegex, string.Empty)
                .Replace(' ', '_')
                .Replace('-', '_')
                .Replace('.', '_');

            return AsEnumValue<DatHeaderField>(headerInput);
        }

        /// <summary>
        /// Get DatItemField value from input string
        /// </summary>
        /// <param name="input">String to get value from</param>
        /// <returns>DatItemField value corresponding to the string</returns>
        public static DatItemField AsDatItemField(this string input)
        {
            // If the input is empty, we return null
            if (string.IsNullOrEmpty(input))
                return DatItemField.NULL;

            // Normalize the input
            input = input.ToLowerInvariant();

            // Create regex
            string datItemRegex = @"^(item|datitem)[.\-_\s]";

            // If we don't have an item field, skip
            if (!Regex.IsMatch(input, datItemRegex))
                return DatItemField.NULL;

            // Replace the match and re-normalize
            string itemInput = Regex.Replace(input, datItemRegex, string.Empty)
                .Replace(' ', '_')
                .Replace('-', '_')
                .Replace('.', '_');

            return AsEnumValue<DatItemField>(itemInput);
        }

        /// <summary>
        /// Get DeviceType value from input string
        /// </summary>
        /// <param name="deviceType">String to get value from</param>
        /// <returns>DeviceType value corresponding to the string</returns>
        public static DeviceType AsDeviceType(this string deviceType)
            => AsEnumValue<DeviceType>(deviceType);

        /// <summary>
        /// Get DisplayType value from input string
        /// </summary>
        /// <param name="displayType">String to get value from</param>
        /// <returns>DisplayType value corresponding to the string</returns>
        public static DisplayType AsDisplayType(this string displayType)
            => AsEnumValue<DisplayType>(displayType);

        /// <summary>
        /// Get Endianness value from input string
        /// </summary>
        /// <param name="endianness">String to get value from</param>
        /// <returns>Endianness value corresponding to the string</returns>
        public static Endianness AsEndianness(this string endianness)
            => AsEnumValue<Endianness>(endianness);

        /// <summary>
        /// Get FeatureStatus value from input string
        /// </summary>
        /// <param name="featureStatus">String to get value from</param>
        /// <returns>FeatureStatus value corresponding to the string</returns>
        public static FeatureStatus AsFeatureStatus(this string featureStatus)
            => AsEnumValue<FeatureStatus>(featureStatus);

        /// <summary>
        /// Get FeatureType value from input string
        /// </summary>
        /// <param name="emulationStatus">String to get value from</param>
        /// <returns>FeatureType value corresponding to the string</returns>
        public static FeatureType AsFeatureType(this string featureType)
            => AsEnumValue<FeatureType>(featureType);

        /// <summary>
        /// Get ItemStatus value from input string
        /// </summary>
        /// <param name="status">String to get value from</param>
        /// <returns>ItemStatus value corresponding to the string</returns>
        public static ItemStatus AsItemStatus(this string status)
            => AsEnumValue<ItemStatus>(status);

        /// <summary>
        /// Get ItemType? value from input string
        /// </summary>
        /// <param name="itemType">String to get value from</param>
        /// <returns>ItemType? value corresponding to the string</returns>
        public static ItemType AsItemType(this string itemType)
            => AsEnumValue<ItemType>(itemType);

        /// <summary>
        /// Get LoadFlag value from input string
        /// </summary>
        /// <param name="loadFlag">String to get value from</param>
        /// <returns>LoadFlag value corresponding to the string</returns>
        public static LoadFlag AsLoadFlag(this string loadFlag)
            => AsEnumValue<LoadFlag>(loadFlag);

        /// <summary>
        /// Get LogLevel value from input string
        /// </summary>
        /// <param name="logLevel">String to get value from</param>
        /// <returns>LogLevel value corresponding to the string</returns>
        public static LogLevel AsLogLevel(this string logLevel)
            => AsEnumValue<LogLevel>(logLevel);

        /// <summary>
        /// Get MachineField value from input string
        /// </summary>
        /// <param name="input">String to get value from</param>
        /// <returns>MachineField value corresponding to the string</returns>
        public static MachineField AsMachineField(this string input)
        {
            // If the input is empty, we return null
            if (string.IsNullOrEmpty(input))
                return MachineField.NULL;

            // Normalize the input
            input = input.ToLowerInvariant();

            // Create regex
            string machineRegex = @"^(game|machine)[.\-_\s]";

            // If we don't have a machine field, skip
            if (!Regex.IsMatch(input, machineRegex))
                return MachineField.NULL;

            // Replace the match and re-normalize
            string machineInput = Regex.Replace(input, machineRegex, string.Empty)
                .Replace(' ', '_')
                .Replace('-', '_')
                .Replace('.', '_');

            return AsEnumValue<MachineField>(machineInput);
        }

        /// <summary>
        /// Get MachineType value from input string
        /// </summary>
        /// <param name="gametype">String to get value from</param>
        /// <returns>MachineType value corresponding to the string</returns>
        public static MachineType AsMachineType(this string gametype)
            => AsEnumValue<MachineType>(gametype);

        /// <summary>
        /// Get MergingFlag value from input string
        /// </summary>
        /// <param name="merging">String to get value from</param>
        /// <returns>MergingFlag value corresponding to the string</returns>
        public static MergingFlag AsMergingFlag(this string merging)
            => AsEnumValue<MergingFlag>(merging);

        /// <summary>
        /// Get NodumpFlag value from input string
        /// </summary>
        /// <param name="nodump">String to get value from</param>
        /// <returns>NodumpFlag value corresponding to the string</returns>
        public static NodumpFlag AsNodumpFlag(this string nodump)
            => AsEnumValue<NodumpFlag>(nodump);

        /// <summary>
        /// Get OpenMSXSubType value from input string
        /// </summary>
        /// <param name="subType">String to get value from</param>
        /// <returns>OpenMSXSubType value corresponding to the string</returns>
        public static OpenMSXSubType AsOpenMSXSubType(this string subType)
            => AsEnumValue<OpenMSXSubType>(subType);

        /// <summary>
        /// Get PackingFlag value from input string
        /// </summary>
        /// <param name="packing">String to get value from</param>
        /// <returns>PackingFlag value corresponding to the string</returns>
        public static PackingFlag AsPackingFlag(this string packing)
            => AsEnumValue<PackingFlag>(packing);

        /// <summary>
        /// Get Relation value from input string
        /// </summary>
        /// <param name="relation">String to get value from</param>
        /// <returns>Relation value corresponding to the string</returns>
        public static Relation AsRelation(this string relation)
            => AsEnumValue<Relation>(relation);

        /// <summary>
        /// Get Runnable value from input string
        /// </summary>
        /// <param name="runnable">String to get value from</param>
        /// <returns>Runnable value corresponding to the string</returns>
        public static Runnable AsRunnable(this string runnable)
            => AsEnumValue<Runnable>(runnable);

        /// <summary>
        /// Get SoftwareListStatus value from input string
        /// </summary>
        /// <param name="status">String to get value from</param>
        /// <returns>SoftwareListStatus value corresponding to the string</returns>
        public static SoftwareListStatus AsSoftwareListStatus(this string status)
            => AsEnumValue<SoftwareListStatus>(status);

        /// <summary>
        /// Get Supported value from input string
        /// </summary>
        /// <param name="supported">String to get value from</param>
        /// <returns>Supported value corresponding to the string</returns>
        public static Supported AsSupported(this string supported)
            => AsEnumValue<Supported>(supported);

        /// <summary>
        /// Get SupportStatus value from input string
        /// </summary>
        /// <param name="supported">String to get value from</param>
        /// <returns>SupportStatus value corresponding to the string</returns>
        public static SupportStatus AsSupportStatus(this string supportStatus)
            => AsEnumValue<SupportStatus>(supportStatus);

        /// <summary>
        /// Get bool? value from input string
        /// </summary>
        /// <param name="yesno">String to get value from</param>
        /// <returns>bool? corresponding to the string</returns>
        public static bool? AsYesNo(this string yesno)
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
        private static T AsEnumValue<T>(string value)
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
        private static Dictionary<string, T> GenerateToEnum<T>()
        {
            try
            {
                // Get all of the values for the enum type
                var values = Enum.GetValues(typeof(T));

                // Build the output dictionary
                Dictionary<string, T> mappings = new();
                foreach (T value in values)
                {
                    // Try to get the mapping attribute
                    MappingAttribute attr = AttributeHelper<T>.GetAttribute(value);
                    if (attr?.Mappings == null || !attr.Mappings.Any())
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
                return new Dictionary<string, T>();
            }
        }

        #endregion

        #region Enum to String

        /// <summary>
        /// Get string value from input ChipType
        /// </summary>
        /// <param name="chipType">ChipType to get value from</param>
        /// <returns>String value corresponding to the ChipType</returns>
        public static string FromChipType(this ChipType chipType)
            => AsStringValue(chipType);

        /// <summary>
        /// Get string value from input ControlType
        /// </summary>
        /// <param name="controlType">ControlType to get value from</param>
        /// <returns>String value corresponding to the ControlType</returns>
        public static string FromControlType(this ControlType controlType)
            => AsStringValue(controlType);

        /// <summary>
        /// Get string value from input DeviceType
        /// </summary>
        /// <param name="deviceType">vDeviceType to get value from</param>
        /// <returns>String value corresponding to the DeviceType</returns>
        public static string FromDeviceType(this DeviceType deviceType)
            => AsStringValue(deviceType);

        /// <summary>
        /// Get string value from input DisplayType
        /// </summary>
        /// <param name="displayType">DisplayType to get value from</param>
        /// <returns>String value corresponding to the DisplayType</returns>
        public static string FromDisplayType(this DisplayType displayType)
            => AsStringValue(displayType);

        /// <summary>
        /// Get string value from input Endianness
        /// </summary>
        /// <param name="endianness">Endianness to get value from</param>
        /// <returns>String value corresponding to the Endianness</returns>
        public static string FromEndianness(this Endianness endianness)
            => AsStringValue(endianness);

        /// <summary>
        /// Get string value from input FeatureStatus
        /// </summary>
        /// <param name="featureStatus">FeatureStatus to get value from</param>
        /// <returns>String value corresponding to the FeatureStatus</returns>
        public static string FromFeatureStatus(this FeatureStatus featureStatus)
            => AsStringValue(featureStatus);

        /// <summary>
        /// Get string value from input FeatureType
        /// </summary>
        /// <param name="featureType">FeatureType to get value from</param>
        /// <returns>String value corresponding to the FeatureType</returns>
        public static string FromFeatureType(this FeatureType featureType)
            => AsStringValue(featureType);

        /// <summary>
        /// Get string value from input ItemStatus
        /// </summary>
        /// <param name="status">ItemStatus to get value from</param>
        /// <param name="yesno">True to use Yes/No format instead</param>
        /// <returns>String value corresponding to the ItemStatus</returns>
        public static string FromItemStatus(this ItemStatus status, bool yesno)
            => AsStringValue(status, yesno);

        /// <summary>
        /// Get string value from input ItemType?
        /// </summary>
        /// <param name="itemType">ItemType? to get value from</param>
        /// <returns>String value corresponding to the ItemType?</returns>
        public static string FromItemType(this ItemType itemType)
            => AsStringValue(itemType);

        /// <summary>
        /// Get string value from input LoadFlag
        /// </summary>
        /// <param name="loadFlag">LoadFlag to get value from</param>
        /// <returns>String value corresponding to the LoadFlag</returns>
        public static string FromLoadFlag(this LoadFlag loadFlag)
            => AsStringValue(loadFlag);

        /// <summary>
        /// Get string value from input MachineType
        /// </summary>
        /// <param name="gametype">MachineType to get value from</param>
        /// <param name="romCenter">True to use old naming instead</param>
        /// <returns>String value corresponding to the MachineType</returns>
        public static string FromMachineType(this MachineType gametype, bool old)
            => AsStringValue(gametype, old);

        /// <summary>
        /// Get string value from input MergingFlag
        /// </summary>
        /// <param name="merging">MergingFlag to get value from</param>
        /// <param name="romCenter">True to use RomCenter naming instead</param>
        /// <returns>String value corresponding to the MergingFlag</returns>
        public static string FromMergingFlag(this MergingFlag merging, bool romCenter)
            => AsStringValue(merging, romCenter);

        /// <summary>
        /// Get string value from input NodumpFlag
        /// </summary>
        /// <param name="nodump">NodumpFlag to get value from</param>
        /// <returns>String value corresponding to the NodumpFlag</returns>
        public static string FromNodumpFlag(this NodumpFlag nodump)
            => AsStringValue(nodump);

        /// <summary>
        /// Get string value from input OpenMSXSubType
        /// </summary>
        /// <param name="subType">OpenMSXSubType to get value from</param>
        /// <returns>String value corresponding to the OpenMSXSubType</returns>
        public static string FromOpenMSXSubType(this OpenMSXSubType subType)
            => AsStringValue(subType);

        /// <summary>
        /// Get string value from input PackingFlag
        /// </summary>
        /// <param name="packing">PackingFlag to get value from</param>
        /// <param name="yesno">True to use Yes/No format instead</param>
        /// <returns>String value corresponding to the PackingFlag</returns>
        public static string FromPackingFlag(this PackingFlag packing, bool yesno)
            => AsStringValue(packing, yesno);

        /// <summary>
        /// Get string value from input Relation
        /// </summary>
        /// <param name="relation">Relation to get value from</param>
        /// <returns>String value corresponding to the Relation</returns>
        public static string FromRelation(this Relation relation)
            => AsStringValue(relation);

        /// <summary>
        /// Get string value from input Runnable
        /// </summary>
        /// <param name="runnable">Runnable to get value from</param>
        /// <returns>String value corresponding to the Runnable</returns>
        public static string FromRunnable(this Runnable runnable)
            => AsStringValue(runnable);

        /// <summary>
        /// Get string value from input SoftwareListStatus
        /// </summary>
        /// <param name="status">SoftwareListStatus to get value from</param>
        /// <returns>String value corresponding to the SoftwareListStatus</returns>
        public static string FromSoftwareListStatus(this SoftwareListStatus status)
            => AsStringValue(status);

        /// <summary>
        /// Get string value from input Supported
        /// </summary>
        /// <param name="supported">Supported to get value from</param>
        /// <param name="verbose">True to use verbose output, false otherwise</param>
        /// <returns>String value corresponding to the Supported</returns>
        public static string FromSupported(this Supported supported, bool verbose)
            => AsStringValue(supported, verbose);

        /// <summary>
        /// Get string value from input SupportStatus
        /// </summary>
        /// <param name="supportStatus">SupportStatus to get value from</param>
        /// <returns>String value corresponding to the SupportStatus</returns>
        public static string FromSupportStatus(this SupportStatus supportStatus)
            => AsStringValue(supportStatus);

        /// <summary>
        /// Get string value from input bool?
        /// </summary>
        /// <param name="yesno">bool? to get value from</param>
        /// <returns>String corresponding to the bool?</returns>
        public static string FromYesNo(this bool? yesno)
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
        private static string? AsStringValue<T>(T value, bool useSecond = false)
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
        private static Dictionary<T, string> GenerateToString<T>(bool useSecond)
        {
            try
            {
                // Get all of the values for the enum type
                var values = Enum.GetValues(typeof(T));

                // Build the output dictionary
                Dictionary<T, string> mappings = new();
                foreach (T value in values)
                {
                    // Try to get the mapping attribute
                    MappingAttribute attr = AttributeHelper<T>.GetAttribute(value);
                    if (attr?.Mappings == null || !attr.Mappings.Any())
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
                return new Dictionary<T, string>();
            }
        }

        #endregion
    }
}
