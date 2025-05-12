using System.Collections.Generic;
using SabreTools.Core.Tools;

namespace SabreTools.DatItems
{
    public static class Extensions
    {
        #region Private Maps

        /// <summary>
        /// Set of enum to string mappings for ChipType
        /// </summary>
        private static readonly Dictionary<string, ChipType> _chipTypeMap = Converters.GenerateToEnum<ChipType>();

        /// <summary>
        /// Set of enum to string mappings for ControlType
        /// </summary>
        private static readonly Dictionary<string, ControlType> _controlTypeMap = Converters.GenerateToEnum<ControlType>();

        /// <summary>
        /// Set of enum to string mappings for DeviceType
        /// </summary>
        private static readonly Dictionary<string, DeviceType> _deviceTypeMap = Converters.GenerateToEnum<DeviceType>();

        /// <summary>
        /// Set of enum to string mappings for DisplayType
        /// </summary>
        private static readonly Dictionary<string, DisplayType> _displayTypeMap = Converters.GenerateToEnum<DisplayType>();

        /// <summary>
        /// Set of enum to string mappings for Endianness
        /// </summary>
        private static readonly Dictionary<string, Endianness> _endiannessMap = Converters.GenerateToEnum<Endianness>();

        /// <summary>
        /// Set of enum to string mappings for FeatureStatus
        /// </summary>
        private static readonly Dictionary<string, FeatureStatus> _featureStatusMap = Converters.GenerateToEnum<FeatureStatus>();

        /// <summary>
        /// Set of enum to string mappings for FeatureType
        /// </summary>
        private static readonly Dictionary<string, FeatureType> _featureTypeMap = Converters.GenerateToEnum<FeatureType>();

        /// <summary>
        /// Set of enum to string mappings for ItemStatus
        /// </summary>
        private static readonly Dictionary<string, ItemStatus> _itemStatusMap = Converters.GenerateToEnum<ItemStatus>();

        /// <summary>
        /// Set of enum to string mappings for ItemType
        /// </summary>
        private static readonly Dictionary<string, ItemType> _itemTypeMap = Converters.GenerateToEnum<ItemType>();

        /// <summary>
        /// Set of enum to string mappings for LoadFlag
        /// </summary>
        private static readonly Dictionary<string, LoadFlag> _loadFlagMap = Converters.GenerateToEnum<LoadFlag>();

        /// <summary>
        /// Set of enum to string mappings for MachineType
        /// </summary>
        private static readonly Dictionary<string, MachineType> _machineTypeMap = Converters.GenerateToEnum<MachineType>();

        /// <summary>
        /// Set of enum to string mappings for OpenMSXSubType
        /// </summary>
        private static readonly Dictionary<string, OpenMSXSubType> _openMSXSubTypeMap = Converters.GenerateToEnum<OpenMSXSubType>();

        /// <summary>
        /// Set of enum to string mappings for Relation
        /// </summary>
        private static readonly Dictionary<string, Relation> _relationMap = Converters.GenerateToEnum<Relation>();

        /// <summary>
        /// Set of enum to string mappings for Runnable
        /// </summary>
        private static readonly Dictionary<string, Runnable> _runnableMap = Converters.GenerateToEnum<Runnable>();

        /// <summary>
        /// Set of enum to string mappings for SoftwareListStatus
        /// </summary>
        private static readonly Dictionary<string, SoftwareListStatus> _softwareListStatusMap = Converters.GenerateToEnum<SoftwareListStatus>();

        /// <summary>
        /// Set of enum to string mappings for Supported
        /// </summary>
        private static readonly Dictionary<string, Supported> _supportedMap = Converters.GenerateToEnum<Supported>();

        /// <summary>
        /// Set of enum to string mappings for SupportStatus
        /// </summary>
        private static readonly Dictionary<string, SupportStatus> _supportStatusMap = Converters.GenerateToEnum<SupportStatus>();

        #endregion

        #region String to Enum

        /// <summary>
        /// Get the enum value for an input string, if possible
        /// </summary>
        /// <param name="value">String value to parse/param>
        /// <typeparam name="T">Enum type that is expected</typeparam>
        /// <returns>Enum value representing the input, default on error</returns>
        public static ChipType AsChipType(this string? value)
        {
            // Normalize the input value
            value = value?.ToLowerInvariant();
            if (value == null)
                return default;

            // Try to get the value from the mappings
            if (_chipTypeMap.ContainsKey(value))
                return _chipTypeMap[value];

            // Otherwise, return the default value for the enum
            return default;
        }

        /// <summary>
        /// Get the enum value for an input string, if possible
        /// </summary>
        /// <param name="value">String value to parse/param>
        /// <typeparam name="T">Enum type that is expected</typeparam>
        /// <returns>Enum value representing the input, default on error</returns>
        public static ControlType AsControlType(this string? value)
        {
            // Normalize the input value
            value = value?.ToLowerInvariant();
            if (value == null)
                return default;

            // Try to get the value from the mappings
            if (_controlTypeMap.ContainsKey(value))
                return _controlTypeMap[value];

            // Otherwise, return the default value for the enum
            return default;
        }

        /// <summary>
        /// Get the enum value for an input string, if possible
        /// </summary>
        /// <param name="value">String value to parse/param>
        /// <typeparam name="T">Enum type that is expected</typeparam>
        /// <returns>Enum value representing the input, default on error</returns>
        public static DeviceType AsDeviceType(this string? value)
        {
            // Normalize the input value
            value = value?.ToLowerInvariant();
            if (value == null)
                return default;

            // Try to get the value from the mappings
            if (_deviceTypeMap.ContainsKey(value))
                return _deviceTypeMap[value];

            // Otherwise, return the default value for the enum
            return default;
        }

        /// <summary>
        /// Get the enum value for an input string, if possible
        /// </summary>
        /// <param name="value">String value to parse/param>
        /// <typeparam name="T">Enum type that is expected</typeparam>
        /// <returns>Enum value representing the input, default on error</returns>
        public static DisplayType AsDisplayType(this string? value)
        {
            // Normalize the input value
            value = value?.ToLowerInvariant();
            if (value == null)
                return default;

            // Try to get the value from the mappings
            if (_displayTypeMap.ContainsKey(value))
                return _displayTypeMap[value];

            // Otherwise, return the default value for the enum
            return default;
        }

        /// <summary>
        /// Get the enum value for an input string, if possible
        /// </summary>
        /// <param name="value">String value to parse/param>
        /// <typeparam name="T">Enum type that is expected</typeparam>
        /// <returns>Enum value representing the input, default on error</returns>
        public static Endianness AsEndianness(this string? value)
        {
            // Normalize the input value
            value = value?.ToLowerInvariant();
            if (value == null)
                return default;

            // Try to get the value from the mappings
            if (_endiannessMap.ContainsKey(value))
                return _endiannessMap[value];

            // Otherwise, return the default value for the enum
            return default;
        }

        /// <summary>
        /// Get the enum value for an input string, if possible
        /// </summary>
        /// <param name="value">String value to parse/param>
        /// <typeparam name="T">Enum type that is expected</typeparam>
        /// <returns>Enum value representing the input, default on error</returns>
        public static FeatureStatus AsFeatureStatus(this string? value)
        {
            // Normalize the input value
            value = value?.ToLowerInvariant();
            if (value == null)
                return default;

            // Try to get the value from the mappings
            if (_featureStatusMap.ContainsKey(value))
                return _featureStatusMap[value];

            // Otherwise, return the default value for the enum
            return default;
        }

        /// <summary>
        /// Get the enum value for an input string, if possible
        /// </summary>
        /// <param name="value">String value to parse/param>
        /// <typeparam name="T">Enum type that is expected</typeparam>
        /// <returns>Enum value representing the input, default on error</returns>
        public static FeatureType AsFeatureType(this string? value)
        {
            // Normalize the input value
            value = value?.ToLowerInvariant();
            if (value == null)
                return default;

            // Try to get the value from the mappings
            if (_featureTypeMap.ContainsKey(value))
                return _featureTypeMap[value];

            // Otherwise, return the default value for the enum
            return default;
        }

        /// <summary>
        /// Get the enum value for an input string, if possible
        /// </summary>
        /// <param name="value">String value to parse/param>
        /// <typeparam name="T">Enum type that is expected</typeparam>
        /// <returns>Enum value representing the input, default on error</returns>
        public static ItemStatus AsItemStatus(this string? value)
        {
            // Normalize the input value
            value = value?.ToLowerInvariant();
            if (value == null)
                return default;

            // Try to get the value from the mappings
            if (_itemStatusMap.ContainsKey(value))
                return _itemStatusMap[value];

            // Otherwise, return the default value for the enum
            return default;
        }

        /// <summary>
        /// Get the enum value for an input string, if possible
        /// </summary>
        /// <param name="value">String value to parse/param>
        /// <typeparam name="T">Enum type that is expected</typeparam>
        /// <returns>Enum value representing the input, default on error</returns>
        public static ItemType AsItemType(this string? value)
        {
            // Normalize the input value
            value = value?.ToLowerInvariant();
            if (value == null)
                return default;

            // Try to get the value from the mappings
            if (_itemTypeMap.ContainsKey(value))
                return _itemTypeMap[value];

            // Otherwise, return the default value for the enum
            return default;
        }

        /// <summary>
        /// Get the enum value for an input string, if possible
        /// </summary>
        /// <param name="value">String value to parse/param>
        /// <typeparam name="T">Enum type that is expected</typeparam>
        /// <returns>Enum value representing the input, default on error</returns>
        public static LoadFlag AsLoadFlag(this string? value)
        {
            // Normalize the input value
            value = value?.ToLowerInvariant();
            if (value == null)
                return default;

            // Try to get the value from the mappings
            if (_loadFlagMap.ContainsKey(value))
                return _loadFlagMap[value];

            // Otherwise, return the default value for the enum
            return default;
        }

        /// <summary>
        /// Get the enum value for an input string, if possible
        /// </summary>
        /// <param name="value">String value to parse/param>
        /// <typeparam name="T">Enum type that is expected</typeparam>
        /// <returns>Enum value representing the input, default on error</returns>
        public static MachineType AsMachineType(this string? value)
        {
            // Normalize the input value
            value = value?.ToLowerInvariant();
            if (value == null)
                return default;

            // Try to get the value from the mappings
            if (_machineTypeMap.ContainsKey(value))
                return _machineTypeMap[value];

            // Otherwise, return the default value for the enum
            return default;
        }

        /// <summary>
        /// Get the enum value for an input string, if possible
        /// </summary>
        /// <param name="value">String value to parse/param>
        /// <typeparam name="T">Enum type that is expected</typeparam>
        /// <returns>Enum value representing the input, default on error</returns>
        public static OpenMSXSubType AsOpenMSXSubType(this string? value)
        {
            // Normalize the input value
            value = value?.ToLowerInvariant();
            if (value == null)
                return default;

            // Try to get the value from the mappings
            if (_openMSXSubTypeMap.ContainsKey(value))
                return _openMSXSubTypeMap[value];

            // Otherwise, return the default value for the enum
            return default;
        }

        /// <summary>
        /// Get the enum value for an input string, if possible
        /// </summary>
        /// <param name="value">String value to parse/param>
        /// <typeparam name="T">Enum type that is expected</typeparam>
        /// <returns>Enum value representing the input, default on error</returns>
        public static Relation AsRelation(this string? value)
        {
            // Normalize the input value
            value = value?.ToLowerInvariant();
            if (value == null)
                return default;

            // Try to get the value from the mappings
            if (_relationMap.ContainsKey(value))
                return _relationMap[value];

            // Otherwise, return the default value for the enum
            return default;
        }

        /// <summary>
        /// Get the enum value for an input string, if possible
        /// </summary>
        /// <param name="value">String value to parse/param>
        /// <typeparam name="T">Enum type that is expected</typeparam>
        /// <returns>Enum value representing the input, default on error</returns>
        public static Runnable AsRunnable(this string? value)
        {
            // Normalize the input value
            value = value?.ToLowerInvariant();
            if (value == null)
                return default;

            // Try to get the value from the mappings
            if (_runnableMap.ContainsKey(value))
                return _runnableMap[value];

            // Otherwise, return the default value for the enum
            return default;
        }

        /// <summary>
        /// Get the enum value for an input string, if possible
        /// </summary>
        /// <param name="value">String value to parse/param>
        /// <typeparam name="T">Enum type that is expected</typeparam>
        /// <returns>Enum value representing the input, default on error</returns>
        public static SoftwareListStatus AsSoftwareListStatus(this string? value)
        {
            // Normalize the input value
            value = value?.ToLowerInvariant();
            if (value == null)
                return default;

            // Try to get the value from the mappings
            if (_softwareListStatusMap.ContainsKey(value))
                return _softwareListStatusMap[value];

            // Otherwise, return the default value for the enum
            return default;
        }

        /// <summary>
        /// Get the enum value for an input string, if possible
        /// </summary>
        /// <param name="value">String value to parse/param>
        /// <typeparam name="T">Enum type that is expected</typeparam>
        /// <returns>Enum value representing the input, default on error</returns>
        public static Supported AsSupported(this string? value)
        {
            // Normalize the input value
            value = value?.ToLowerInvariant();
            if (value == null)
                return default;

            // Try to get the value from the mappings
            if (_supportedMap.ContainsKey(value))
                return _supportedMap[value];

            // Otherwise, return the default value for the enum
            return default;
        }

        /// <summary>
        /// Get the enum value for an input string, if possible
        /// </summary>
        /// <param name="value">String value to parse/param>
        /// <typeparam name="T">Enum type that is expected</typeparam>
        /// <returns>Enum value representing the input, default on error</returns>
        public static SupportStatus AsSupportStatus(this string? value)
        {
            // Normalize the input value
            value = value?.ToLowerInvariant();
            if (value == null)
                return default;

            // Try to get the value from the mappings
            if (_supportStatusMap.ContainsKey(value))
                return _supportStatusMap[value];

            // Otherwise, return the default value for the enum
            return default;
        }

        #endregion
    }
}