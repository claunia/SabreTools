#if NET40_OR_GREATER || NETCOREAPP
using System.Collections.Concurrent;
#endif
using System.Collections.Generic;
using System.IO;
using System.Linq;
#if NET40_OR_GREATER || NETCOREAPP
using System.Threading.Tasks;
#endif
using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;
using SabreTools.Hashing;
using SabreTools.Matching;

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
        [JsonIgnore, XmlIgnore]
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
        [JsonIgnore, XmlIgnore]
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

        /// <summary>
        /// Internal dictionary representing the current buckets
        /// </summary>
        [JsonIgnore, XmlIgnore]
#if NET40_OR_GREATER || NETCOREAPP
        private readonly ConcurrentDictionary<string, ConcurrentList<long>> _buckets = new ConcurrentDictionary<string, ConcurrentList<long>>();
#else
        private readonly Dictionary<string, ConcurrentList<long>> _buckets = [];
#endif

        /// <summary>
        /// Current bucketed by value
        /// </summary>
        private ItemKey _bucketedBy = ItemKey.NULL;

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

        #region Bucketing

        /// <summary>
        /// Update the bucketing dictionary
        /// </summary>
        /// <param name="bucketBy">ItemKey enum representing how to bucket the individual items</param>
        /// <param name="lower">True if the key should be lowercased (default), false otherwise</param>
        /// <param name="norename">True if games should only be compared on game and file name, false if system and source are counted</param>
        /// <returns></returns>
        public void UpdateBucketBy(ItemKey bucketBy, bool lower = true, bool norename = true)
        {
            // If the bucketing value is the same or null
            if (bucketBy == _bucketedBy || bucketBy == ItemKey.NULL)
                return;

            // Reset the bucketing values
            _bucketedBy = bucketBy;
            _buckets.Clear();

            // Get the current list of item indicies
            long[] itemIndicies = [.. _items.Keys];

#if NET452_OR_GREATER || NETCOREAPP
            Parallel.For(0, itemIndicies.Length, Globals.ParallelOptions, i =>
#elif NET40_OR_GREATER
            Parallel.For(0, itemIndicies.Length, i =>
#else
            for (int i = 0; i < itemIndicies.Length; i++)
#endif
            {
                string? bucketKey = GetBucketKey(i, bucketBy);
                EnsureBucketingKey(bucketKey);
                _buckets[bucketKey].Add(i);
#if NET40_OR_GREATER || NETCOREAPP
            });
#else
            }
#endif

            // Sort the buckets that have been created for consistency
            SortBuckets(norename);
        }

        /// <summary>
        /// Get the bucketing key for a given item index
        /// </summary>
        private string GetBucketKey(long itemIndex, ItemKey bucketBy)
        {
            if (!_items.ContainsKey(itemIndex))
                return string.Empty;

            var datItem = _items[itemIndex];
            if (datItem == null)
                return string.Empty;

            if (!_itemToMachineMapping.ContainsKey(itemIndex))
                return string.Empty;

            long machineIndex = _itemToMachineMapping[itemIndex];
            if (!_machines.ContainsKey(machineIndex))
                return string.Empty;

            var machine = _machines[machineIndex];
            if (machine == null)
                return string.Empty;

            return bucketBy switch
            {
                ItemKey.Machine => machine.GetStringFieldValue(Models.Metadata.Machine.NameKey) ?? string.Empty,
                _ => GetBucketHashValue(datItem, bucketBy),
            };
        }

        /// <summary>
        /// Get the hash value for a given item, if possible
        /// </summary>
        private static string GetBucketHashValue(DatItem datItem, ItemKey bucketBy)
        {
            return datItem switch
            {
                Disk disk => bucketBy switch
                {
                    ItemKey.CRC => Constants.CRCZero,
                    ItemKey.MD5 => disk.GetStringFieldValue(Models.Metadata.Disk.MD5Key) ?? string.Empty,
                    ItemKey.SHA1 => disk.GetStringFieldValue(Models.Metadata.Disk.SHA1Key) ?? string.Empty,
                    ItemKey.SHA256 => Constants.SHA256Zero,
                    ItemKey.SHA384 => Constants.SHA384Zero,
                    ItemKey.SHA512 => Constants.SHA512Zero,
                    ItemKey.SpamSum => Constants.SpamSumZero,
                    _ => string.Empty,
                },
                Media media => bucketBy switch
                {
                    ItemKey.CRC => Constants.CRCZero,
                    ItemKey.MD5 => media.GetStringFieldValue(Models.Metadata.Media.MD5Key) ?? string.Empty,
                    ItemKey.SHA1 => media.GetStringFieldValue(Models.Metadata.Media.SHA1Key) ?? string.Empty,
                    ItemKey.SHA256 => media.GetStringFieldValue(Models.Metadata.Media.SHA256Key) ?? string.Empty,
                    ItemKey.SHA384 => Constants.SHA384Zero,
                    ItemKey.SHA512 => Constants.SHA512Zero,
                    ItemKey.SpamSum => media.GetStringFieldValue(Models.Metadata.Media.SpamSumKey) ?? string.Empty,
                    _ => string.Empty,
                },
                Rom rom => bucketBy switch
                {
                    ItemKey.CRC => rom.GetStringFieldValue(Models.Metadata.Rom.CRCKey) ?? string.Empty,
                    ItemKey.MD5 => rom.GetStringFieldValue(Models.Metadata.Rom.MD5Key) ?? string.Empty,
                    ItemKey.SHA1 => rom.GetStringFieldValue(Models.Metadata.Rom.SHA1Key) ?? string.Empty,
                    ItemKey.SHA256 => rom.GetStringFieldValue(Models.Metadata.Rom.SHA256Key) ?? string.Empty,
                    ItemKey.SHA384 => rom.GetStringFieldValue(Models.Metadata.Rom.SHA384Key) ?? string.Empty,
                    ItemKey.SHA512 => rom.GetStringFieldValue(Models.Metadata.Rom.SHA512Key) ?? string.Empty,
                    ItemKey.SpamSum => rom.GetStringFieldValue(Models.Metadata.Rom.SpamSumKey) ?? string.Empty,
                    _ => string.Empty,
                },
                _ => bucketBy switch
                {
                    ItemKey.CRC => Constants.CRCZero,
                    ItemKey.MD5 => Constants.MD5Zero,
                    ItemKey.SHA1 => Constants.SHA1Zero,
                    ItemKey.SHA256 => Constants.SHA256Zero,
                    ItemKey.SHA384 => Constants.SHA384Zero,
                    ItemKey.SHA512 => Constants.SHA512Zero,
                    ItemKey.SpamSum => Constants.SpamSumZero,
                    _ => string.Empty,
                },
            };
        }

        /// <summary>
        /// Ensure the key exists in the items dictionary
        /// </summary>
        private void EnsureBucketingKey(string key)
        {
            // If the key is missing from the dictionary, add it
            if (!_buckets.ContainsKey(key))
#if NET40_OR_GREATER || NETCOREAPP
                _buckets.TryAdd(key, []);
#else
                _buckets[key] = [];
#endif
        }

        /// <summary>
        /// Sort existing buckets for consistency
        /// </summary>
        private void SortBuckets(bool norename)
        {
            // Get the current list of bucket keys
            string[] bucketKeys = [.. _buckets.Keys];

#if NET452_OR_GREATER || NETCOREAPP
            Parallel.For(0, bucketKeys.Length, Globals.ParallelOptions, i =>
#elif NET40_OR_GREATER
            Parallel.For(0, bucketKeys.Length, i =>
#else
            for (int i = 0; i < bucketKeys.Length; i++)
#endif
            {
                var itemIndices = _buckets[bucketKeys[i]];
                if (itemIndices == null || !itemIndices.Any())
                {
#if NET40_OR_GREATER || NETCOREAPP
                    _buckets.TryRemove(bucketKeys[i], out _);
                    return;
#else
                    _buckets.Remove(bucketKeys[i]);
                    continue;
#endif
                }

                var datItems = itemIndices
                    .Where(i => _items.ContainsKey(i))
                    .Select(i => (i, _items[i]))
                    .ToList();

                Sort(ref datItems, norename);

                _buckets[bucketKeys[i]] = datItems.Select(m => m.Item1).ToConcurrentList();
#if NET40_OR_GREATER || NETCOREAPP
            });
#else
            }
#endif
        }

        /// <summary>
        /// Sort a list of File objects by SourceID, Game, and Name (in order)
        /// </summary>
        /// <param name="itemMappings">List of File objects representing the roms to be sorted</param>
        /// <param name="norename">True if files are not renamed, false otherwise</param>
        /// <returns>True if it sorted correctly, false otherwise</returns>
        private bool Sort(ref List<(long, DatItem)> itemMappings, bool norename)
        {
            itemMappings.Sort(delegate ((long, DatItem) x, (long, DatItem) y)
            {
                try
                {
                    var nc = new NaturalComparer();

                    // If machine names don't match
                    string? xMachineName = _machines[_itemToMachineMapping[x.Item1]].GetStringFieldValue(Models.Metadata.Machine.NameKey);
                    string? yMachineName = _machines[_itemToMachineMapping[y.Item1]].GetStringFieldValue(Models.Metadata.Machine.NameKey);
                    if (xMachineName != yMachineName)
                        return nc.Compare(xMachineName, yMachineName);

                    // If types don't match
                    string? xType = x.Item2.GetStringFieldValue(Models.Metadata.DatItem.TypeKey);
                    string? yType = y.Item2.GetStringFieldValue(Models.Metadata.DatItem.TypeKey);
                    if (xType != yType)
                        return xType.AsEnumValue<ItemType>() - yType.AsEnumValue<ItemType>();

                    // If directory names don't match
                    string? xDirectoryName = Path.GetDirectoryName(TextHelper.RemovePathUnsafeCharacters(x.Item2.GetName() ?? string.Empty));
                    string? yDirectoryName = Path.GetDirectoryName(TextHelper.RemovePathUnsafeCharacters(y.Item2.GetName() ?? string.Empty));
                    if (xDirectoryName != yDirectoryName)
                        return nc.Compare(xDirectoryName, yDirectoryName);

                    // If item names don't match
                    string? xName = Path.GetFileName(TextHelper.RemovePathUnsafeCharacters(x.Item2.GetName() ?? string.Empty));
                    string? yName = Path.GetFileName(TextHelper.RemovePathUnsafeCharacters(y.Item2.GetName() ?? string.Empty));
                    if (xName != yName)
                        return nc.Compare(xName, yName);

                    // Otherwise, compare on machine or source, depending on the flag
                    int? xSourceIndex = x.Item2.GetFieldValue<Source?>(DatItem.SourceKey)?.Index;
                    int? ySourceIndex = y.Item2.GetFieldValue<Source?>(DatItem.SourceKey)?.Index;
                    return (norename ? nc.Compare(xMachineName, yMachineName) : (xSourceIndex - ySourceIndex) ?? 0);
                }
                catch
                {
                    // Absorb the error
                    return 0;
                }
            });

            return true;
        }

        // TODO: Write a method that deduplicates items based on any of the fields selected

        #endregion
    }
}
