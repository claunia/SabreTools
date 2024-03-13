#if NET40_OR_GREATER || NETCOREAPP
using System.Collections.Concurrent;
#endif
using System.Collections.Generic;
using System.Linq;
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
        [JsonIgnore, XmlIgnore]
#if NET40_OR_GREATER || NETCOREAPP
        private readonly ConcurrentDictionary<long, DatItem> _items = new ConcurrentDictionary<long, DatItem>();
#else
        private readonly Dictionary<long, DatItem> _items = [];
#endif

        /// <summary>
        /// Current highest available item index
        /// </summary>
        private long _itemIndex = 0;

        /// <summary>
        /// Internal dictionary for all machines
        /// </summary>
        [JsonIgnore, XmlIgnore]
#if NET40_OR_GREATER || NETCOREAPP
        private readonly ConcurrentDictionary<long, Machine> _machines = new ConcurrentDictionary<long, Machine>();
#else
        private readonly Dictionary<long, Machine> _machines = [];
#endif

        /// <summary>
        /// Current highest available machine index
        /// </summary>
        private long _machineIndex = 0;

        /// <summary>
        /// Internal dictionary for item to machine mappings
        /// </summary>
        [JsonIgnore, XmlIgnore]
#if NET40_OR_GREATER || NETCOREAPP
        private readonly ConcurrentDictionary<long, long> _itemToMachineMapping = new ConcurrentDictionary<long, long>();
#else
        private readonly Dictionary<long, long> _itemToMachineMapping = [];
#endif

        // TODO: Add another dictionary of string => ConcurrentList<long> representing a bucketed key to a set of item IDs

        #endregion

        #region Fields

        /// <summary>
        /// DAT statistics
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public DatStatistics DatStatistics { get; } = new DatStatistics();

        #endregion

        #region Accessors

        /// <summary>
        /// Add an item, returning the insert index
        /// </summary>
        public long AddItem(DatItem item)
        {
            _items[_itemIndex++] = item;
            DatStatistics.AddItemStatistics(item);
            return _itemIndex - 1;
        }

        /// <summary>
        /// Add a machine, returning the insert index
        /// </summary>
        public long AddMachine(Machine machine)
        {
            _machines[_machineIndex++] = machine;
            return _machineIndex - 1;
        }

        /// <summary>
        /// Get an item based on the index
        /// </summary>
        public DatItem? GetItemByIndex(long index)
        {
            if (!_items.ContainsKey(index))
                return null;

            return _items[index];
        }

        /// <summary>
        /// Get a machine based on the index
        /// </summary>
        public Machine? GetMachineByIndex(long index)
        {
            if (!_machines.ContainsKey(index))
                return null;

            return _machines[index];
        }

        /// <summary>
        /// Get the machine associated with an item index
        /// </summary>
        public Machine? GetMachineForItemByIndex(long itemIndex)
        {
            if (!_itemToMachineMapping.ContainsKey(itemIndex))
                return null;

            long machineIndex = _itemToMachineMapping[itemIndex];
            if (!_machines.ContainsKey(machineIndex))
                return null;

            return _machines[machineIndex];
        }

        /// <summary>
        /// Get the items associated with a machine index
        /// </summary>
        public DatItem[]? GetDatItemsForMachineByIndex(long machineIndex)
        {
            var itemIds = _itemToMachineMapping
                .Where(mapping => mapping.Value == machineIndex)
                .Select(mapping => mapping.Key);

            var datItems = new List<DatItem>();
            foreach (long itemId in itemIds)
            {
                if (_items.ContainsKey(itemId))
                    datItems.Add(_items[itemId]);
            }

            return datItems.ToArray();
        }

        /// <summary>
        /// Remove an item, returning if it could be removed
        /// </summary>
        public bool RemoveItem(long itemIndex)
        {
            if (!_items.ContainsKey(itemIndex))
                return false;

#if NET40_OR_GREATER || NETCOREAPP
            _items.TryRemove(itemIndex, out _);
#else
            _items.Remove(itemIndex);
#endif

            if (_itemToMachineMapping.ContainsKey(itemIndex))
#if NET40_OR_GREATER || NETCOREAPP
                _itemToMachineMapping.TryRemove(itemIndex, out _);
#else
                _itemToMachineMapping.Remove(itemIndex);
#endif

            return true;
        }

        /// <summary>
        /// Remove a machine, returning if it could be removed
        /// </summary>
        public bool RemoveMachine(long machineIndex)
        {
            if (!_machines.ContainsKey(machineIndex))
                return false;

#if NET40_OR_GREATER || NETCOREAPP
            _machines.TryRemove(machineIndex, out _);
#else
            _machines.Remove(machineIndex);
#endif

            var itemIds = _itemToMachineMapping
                .Where(mapping => mapping.Value == machineIndex)
                .Select(mapping => mapping.Key);

            foreach (long itemId in itemIds)
            {
#if NET40_OR_GREATER || NETCOREAPP
                _itemToMachineMapping.TryRemove(itemId, out _);
#else
                _itemToMachineMapping.Remove(itemId);
#endif
            }

            return true;
        }

#endregion
    }
}
