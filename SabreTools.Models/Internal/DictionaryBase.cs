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
        public bool? ReadBool(string key) => Read<bool>(key);

        /// <summary>
        /// Read a key as a double, returning null on error
        /// </summary>
        public double? ReadDouble(string key) => Read<double>(key);

        /// <summary>
        /// Read a key as a long, returning null on error
        /// </summary>
        public long? ReadLong(string key) => Read<long>(key);

        /// <summary>
        /// Read a key as a string, returning null on error
        /// </summary>
        public string? ReadString(string key) => Read<string>(key);

        /// <summary>
        /// Read a key as a string[], returning null on error
        /// </summary>
        public string[]? ReadStringArray(string key) => Read<string[]>(key);
    }
}