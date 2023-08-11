using System.Collections.Generic;

namespace SabreTools.Models.Internal
{
    /// <summary>
    /// Specialized dictionary base for item types
    /// </summary>
    public abstract class DictionaryBase : Dictionary<string, object?>
    {
        /// <summary>
        /// Read a key as the specified type, returning null on error
        /// </summary>
        public T? Read<T>(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return default;
            if (!ContainsKey(key))
                return default;
            return (T?)this[key];
        }

        /// <summary>
        /// Read a key as a bool, returning null on error
        /// </summary>
        public bool? ReadBool(string key)
        {
            bool? asBool = Read<bool>(key);
            if (asBool != null)
                return asBool;

            string? asString = Read<string>(key);
            return asString?.ToLowerInvariant() switch
            {
                "true" => true,
                "yes" => true,
                "false" => false,
                "no" => false,
                _ => null,
            };
        }

        /// <summary>
        /// Read a key as a double, returning null on error
        /// </summary>
        public double? ReadDouble(string key)
        {
            double? asDouble = Read<double>(key);
            if (asDouble != null)
                return asDouble;

            string? asString = Read<string>(key);
            if (asString != null && double.TryParse(asString, out double asStringDouble))
                return asStringDouble;

            return null;
        }

        /// <summary>
        /// Read a key as a long, returning null on error
        /// </summary>
        public long? ReadLong(string key)
        {
            long? asLong = Read<long>(key);
            if (asLong != null)
                return asLong;

            string? asString = Read<string>(key);
            if (asString != null && long.TryParse(asString, out long asStringLong))
                return asStringLong;

            return null;
        }

        /// <summary>
        /// Read a key as a string, returning null on error
        /// </summary>
        public string? ReadString(string key)
        {
            string? asString = Read<string>(key);
            if (asString != null)
                return asString;

            string[]? asArray = Read<string[]>(key);
            if (asArray != null)
                return string.Join(',', asArray);

            return null;
        }

        /// <summary>
        /// Read a key as a string[], returning null on error
        /// </summary>
        public string[]? ReadStringArray(string key)
        {
            string[]? asArray = Read<string[]>(key);
            if (asArray != null)
                return asArray;

            string? asString = Read<string>(key);
            if (asString != null)
                return new string[] { asString };

            return null;
        }
    }
}