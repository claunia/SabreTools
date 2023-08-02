using System.Collections.Generic;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Models.Internal
{
    /// <summary>
    /// Format-agnostic representation of item data
    /// </summary>
    public class DatItem : Dictionary<string, object?>
    {
        #region Common Keys

        public const string TypeKey = "_type";

        #endregion

        /// <summary>
        /// Quick accessor to item type, if it exists
        /// </summary>
        [JsonProperty("itemtype", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("itemtype")]
        public ItemType? Type
        {
            get => ContainsKey(TypeKey) ? this[TypeKey] as ItemType? : null;
            set => this[TypeKey] = value;
        }

        #region Reading Helpers

        /// <summary>
        /// Read a key as a bool, returning null on error
        /// </summary>
        public bool? ReadBool(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return null;
            if (!ContainsKey(key))
                return null;
            return this[key] as bool?;
        }

        /// <summary>
        /// Read a key as a double, returning null on error
        /// </summary>
        public double? ReadDouble(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return null;
            if (!ContainsKey(key))
                return null;
            return this[key] as double?;
        }

        /// <summary>
        /// Read a key as a long, returning null on error
        /// </summary>
        public long? ReadLong(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return null;
            if (!ContainsKey(key))
                return null;
            return this[key] as long?;
        }

        /// <summary>
        /// Read a key as a string, returning null on error
        /// </summary>
        public string? ReadString(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return null;
            if (!ContainsKey(key))
                return null;
            return this[key] as string;
        }
    
        #endregion
    }
}