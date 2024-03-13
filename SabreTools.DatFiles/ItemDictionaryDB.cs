#if NET40_OR_GREATER || NETCOREAPP
using System.Collections.Concurrent;
#else
using System.Collections.Generic;
#endif
using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.DatItems;

namespace SabreTools.DatFiles
{
    /// <summary>
    /// Item dictionary with statistics, bucketing, and sorting
    /// </summary>
    [JsonObject("items"), XmlRoot("items")]
    public class ItemDictionaryDB
    {
        #region Private instance variables

        /// <summary>
        /// Internal dictionary for all items
        /// </summary>
#if NET40_OR_GREATER || NETCOREAPP
        private readonly ConcurrentDictionary<long, DatItem> items = new ConcurrentDictionary<long, DatItem>();
#else
        private readonly Dictionary<long, DatItem> items = [];
#endif

        /// <summary>
        /// Internal dictionary for all machines
        /// </summary>
#if NET40_OR_GREATER || NETCOREAPP
        private readonly ConcurrentDictionary<long, Machine> machines = new ConcurrentDictionary<long, Machine>();
#else
        private readonly Dictionary<long, Machine> machines = [];
#endif

        /// <summary>
        /// Internal dictionary for item to machine mappings
        /// </summary>
#if NET40_OR_GREATER || NETCOREAPP
        private readonly ConcurrentDictionary<long, long> itemToMachineMapping = new ConcurrentDictionary<long, long>();
#else
        private readonly Dictionary<long, long> itemToMachineMapping = [];
#endif

        #endregion
    }
}
