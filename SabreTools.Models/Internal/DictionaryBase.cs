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
            if (!ValidateKey(key))
                return default;
            return (T?)this[key];
        }

        /// <summary>
        /// Read a key as a bool, returning null on error
        /// </summary>
        public bool? ReadBool(string key)
        {
            if (!ValidateKey(key))
                return null;

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
            if (!ValidateKey(key))
                return null;

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
            if (!ValidateKey(key))
                return null;

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
            if (!ValidateKey(key))
                return null;

            string? asString = Read<string>(key);
            if (asString != null)
                return asString;

            string[]? asArray = Read<string[]>(key);
            if (asArray != null)
                return string.Join(',', asArray);

            return this[key]!.ToString();
        }

        /// <summary>
        /// Read a key as a string[], returning null on error
        /// </summary>
        public string[]? ReadStringArray(string key)
        {
            if (!ValidateKey(key))
                return null;

            string[]? asArray = Read<string[]>(key);
            if (asArray != null)
                return asArray;

            string? asString = Read<string>(key);
            if (asString != null)
                return new string[] { asString };

            asString = this[key]!.ToString();
            if (asString != null)
                return new string[] { asString };

            return null;
        }

        /// <summary>
        /// Check if a key is valid
        /// </summary>
        private bool ValidateKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return false;
            else if (!ContainsKey(key))
                return false;
            else if (this[key] == null)
                return false;

            return true;
        }
    }
}