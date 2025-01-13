using System;
#if NET40_OR_GREATER || NETCOREAPP
using System.Collections.Concurrent;
#endif
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
#if NET40_OR_GREATER || NETCOREAPP
using System.Threading.Tasks;
#endif
using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core.Filter;
using SabreTools.Core.Tools;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;
using SabreTools.Hashing;
using SabreTools.IO.Logging;
using SabreTools.Matching.Compare;

/*
 * Planning Notes:
 * 
 * In order for this in-memory "database" design to work, there need to be a few things:
 * - Feature parity with all existing item dictionary operations
 * - A way to transition between the two item dictionaries (a flag?)
 * - Helper methods that target the "database" version instead of assuming the standard dictionary
 * 
 * Notable changes include:
 * - Separation of Machine from DatItem, leading to a mapping instead
 *      + Should DatItem include an index reference to the machine? Or should that be all external?
 * - Adding machines to the dictionary distinctly from the items
 * - Having a separate "bucketing" system that only reorders indicies and not full items; quicker?
 * - Non-key-based add/remove of values; use explicit methods instead of dictionary-style accessors
*/

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
        private readonly ConcurrentDictionary<long, DatItem> _items = [];
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
        private readonly ConcurrentDictionary<long, Machine> _machines = [];
#else
        private readonly Dictionary<long, Machine> _machines = [];
#endif

        /// <summary>
        /// Current highest available machine index
        /// </summary>
        [JsonIgnore, XmlIgnore]
        private long _machineIndex = 0;

        /// <summary>
        /// Internal dictionary for all sources
        /// </summary>
        [JsonIgnore, XmlIgnore]
#if NET40_OR_GREATER || NETCOREAPP
        private readonly ConcurrentDictionary<long, Source> _sources = [];
#else
        private readonly Dictionary<long, Source> _sources = [];
#endif

        /// <summary>
        /// Current highest available source index
        /// </summary>
        [JsonIgnore, XmlIgnore]
        private long _sourceIndex = 0;

        /// <summary>
        /// Internal dictionary for item to machine mappings
        /// </summary>
        /// TODO: Make private when access issues are figured out
        [JsonIgnore, XmlIgnore]
#if NET40_OR_GREATER || NETCOREAPP
        internal readonly ConcurrentDictionary<long, long> _itemToMachineMapping = [];
#else
        internal readonly Dictionary<long, long> _itemToMachineMapping = [];
#endif

        /// <summary>
        /// Internal dictionary for item to source mappings
        /// </summary>
        [JsonIgnore, XmlIgnore]
#if NET40_OR_GREATER || NETCOREAPP
        private readonly ConcurrentDictionary<long, long> _itemToSourceMapping = [];
#else
        private readonly Dictionary<long, long> _itemToSourceMapping = [];
#endif

        /// <summary>
        /// Internal dictionary representing the current buckets
        /// </summary>
        /// TODO: Make private when access issues are figured out
        [JsonIgnore, XmlIgnore]
#if NET40_OR_GREATER || NETCOREAPP
        internal readonly ConcurrentDictionary<string, List<long>> _buckets = [];
#else
        internal readonly Dictionary<string, List<long>> _buckets = [];
#endif

        /// <summary>
        /// Current bucketed by value
        /// </summary>
        private ItemKey _bucketedBy = ItemKey.NULL;

        /// <summary>
        /// Logging object
        /// </summary>
        private readonly Logger _logger;

        #endregion

        #region Fields

        /// <summary>
        /// Get the keys in sorted order from the file dictionary
        /// </summary>
        /// <returns>List of the keys in sorted order</returns>
        [JsonIgnore, XmlIgnore]
        public string[] SortedKeys
        {
            get
            {
                List<string> keys = [.. _buckets.Keys];
                keys.Sort(new NaturalComparer());
                return [.. keys];
            }
        }

        /// <summary>
        /// DAT statistics
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public DatStatistics DatStatistics { get; } = new DatStatistics();

        #endregion

        /// <summary>
        /// Generic constructor
        /// </summary>
        public ItemDictionaryDB()
        {
            _logger = new Logger(this);
        }

        #region Accessors

        /// <summary>
        /// Add a DatItem to the dictionary after validation
        /// </summary>
        /// <param name="item">Item data to validate</param>
        /// <param name="machineIndex">Index of the machine related to the item</param>
        /// <param name="sourceIndex">Index of the source related to the item</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <returns>The index for the added item, -1 on error</returns>
        public long AddItem(DatItem item, long machineIndex, long sourceIndex, bool statsOnly)
        {
            // If we have a Disk, Media, or Rom, clean the hash data
            if (item is Disk disk)
            {
                // If the file has aboslutely no hashes, skip and log
                if (disk.GetStringFieldValue(Models.Metadata.Disk.StatusKey).AsEnumValue<ItemStatus>() != ItemStatus.Nodump
                    && string.IsNullOrEmpty(disk.GetStringFieldValue(Models.Metadata.Disk.MD5Key))
                    && string.IsNullOrEmpty(disk.GetStringFieldValue(Models.Metadata.Disk.SHA1Key)))
                {
                    disk.SetFieldValue<string?>(Models.Metadata.Disk.StatusKey, ItemStatus.Nodump.AsStringValue());
                }

                item = disk;
            }
            else if (item is Media media)
            {
                // If the file has aboslutely no hashes, skip and log
                if (string.IsNullOrEmpty(media.GetStringFieldValue(Models.Metadata.Media.MD5Key))
                    && string.IsNullOrEmpty(media.GetStringFieldValue(Models.Metadata.Media.SHA1Key))
                    && string.IsNullOrEmpty(media.GetStringFieldValue(Models.Metadata.Media.SHA256Key))
                    && string.IsNullOrEmpty(media.GetStringFieldValue(Models.Metadata.Media.SpamSumKey)))
                {
                    // No-op as there is no status key for Media
                }

                item = media;
            }
            else if (item is Rom rom)
            {
                long? size = rom.GetInt64FieldValue(Models.Metadata.Rom.SizeKey);

                // If we have the case where there is SHA-1 and nothing else, we don't fill in any other part of the data
                if (size == null && !rom.HasHashes())
                {
                    // No-op, just catch it so it doesn't go further
                }

                // If we have a rom and it's missing size AND the hashes match a 0-byte file, fill in the rest of the info
                else if ((size == 0 || size == null)
                    && (string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.CRCKey)) || rom.HasZeroHash()))
                {
                    rom.SetFieldValue<string?>(Models.Metadata.Rom.SizeKey, Constants.SizeZero.ToString());
                    rom.SetFieldValue<string?>(Models.Metadata.Rom.CRCKey, ZeroHash.CRC32Str);
                    rom.SetFieldValue<string?>(Models.Metadata.Rom.MD2Key, null); // ZeroHash.GetString(HashType.MD2)
                    rom.SetFieldValue<string?>(Models.Metadata.Rom.MD4Key, null); // ZeroHash.GetString(HashType.MD4)
                    rom.SetFieldValue<string?>(Models.Metadata.Rom.MD5Key, ZeroHash.MD5Str);
                    rom.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, ZeroHash.SHA1Str);
                    rom.SetFieldValue<string?>(Models.Metadata.Rom.SHA256Key, null); // ZeroHash.SHA256Str;
                    rom.SetFieldValue<string?>(Models.Metadata.Rom.SHA384Key, null); // ZeroHash.SHA384Str;
                    rom.SetFieldValue<string?>(Models.Metadata.Rom.SHA512Key, null); // ZeroHash.SHA512Str;
                    rom.SetFieldValue<string?>(Models.Metadata.Rom.SpamSumKey, null); // ZeroHash.SpamSumStr;
                }

                // If the file has no size and it's not the above case, skip and log
                else if (rom.GetStringFieldValue(Models.Metadata.Rom.StatusKey).AsEnumValue<ItemStatus>() != ItemStatus.Nodump && (size == 0 || size == null))
                {
                    rom.SetFieldValue<string?>(Models.Metadata.Rom.StatusKey, ItemStatus.Nodump.AsStringValue());
                }

                // If the file has a size but aboslutely no hashes, skip and log
                else if (rom.GetStringFieldValue(Models.Metadata.Rom.StatusKey).AsEnumValue<ItemStatus>() != ItemStatus.Nodump
                    && size != null && size > 0
                    && !rom.HasHashes())
                {
                    rom.SetFieldValue<string?>(Models.Metadata.Rom.StatusKey, ItemStatus.Nodump.AsStringValue());
                }

                item = rom;
            }

            // Get the key and add the file
            string key = item.GetKey(ItemKey.Machine);

            // If only adding statistics, we add an empty key for games and then just item stats
            if (statsOnly)
            {
                EnsureBucketingKey(key);
                DatStatistics.AddItemStatistics(item);
                return -1;
            }
            else
            {
                return AddItem(item, machineIndex, sourceIndex);
            }
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
        /// Add a source, returning the insert index
        /// </summary>
        public long AddSource(Source source)
        {
            _sources[_sourceIndex++] = source;
            return _sourceIndex - 1;
        }

        /// <summary>
        /// Remove any keys that have null or empty values
        /// </summary>
        internal void ClearEmpty()
        {
            var keys = Array.FindAll(SortedKeys, k => k != null);
            foreach (string key in keys)
            {
                // If the key doesn't exist, skip
                if (!_buckets.ContainsKey(key))
                    continue;

                // If there are no non-blank items, remove
                else if (!_buckets[key].Exists(i => GetItem(i) != null && GetItem(i) is not Blank))
#if NET40_OR_GREATER || NETCOREAPP
                    _buckets.TryRemove(key, out _);
#else
                    _buckets.Remove(key);
#endif
            }
        }

        /// <summary>
        /// Remove all items marked for removal
        /// </summary>
        internal void ClearMarked()
        {
            var itemIndices = _items.Keys;
            foreach (long itemIndex in itemIndices)
            {
                var datItem = _items[itemIndex];
                if (datItem == null || datItem.GetBoolFieldValue(DatItem.RemoveKey) != true)
                    continue;

                RemoveItem(itemIndex);
            }
        }

        /// <summary>
        /// Get an item based on the index
        /// </summary>
        public DatItem? GetItem(long index)
        {
            if (!_items.ContainsKey(index))
                return null;

            return _items[index];
        }

        /// <summary>
        /// Get all item to machine mappings
        /// </summary>
        public IDictionary<long, long> GetItemMachineMappings() => _itemToMachineMapping;

        /// <summary>
        /// Get all item to source mappings
        /// </summary>
        public IDictionary<long, long> GetItemSourceMappings() => _itemToSourceMapping;

        /// <summary>
        /// Get all items and their indicies
        /// </summary>
        public IDictionary<long, DatItem> GetItems() => _items;

        /// <summary>
        /// Get the indices and items associated with a bucket name
        /// </summary>
        public Dictionary<long, DatItem> GetItemsForBucket(string bucketName, bool filter = false)
        {
            if (!_buckets.ContainsKey(bucketName))
                return [];

            var itemIds = _buckets[bucketName];

            var datItems = new Dictionary<long, DatItem>();
            foreach (long itemId in itemIds)
            {
                // Ignore missing IDs
                if (!_items.ContainsKey(itemId))
                    continue;

                if (!filter || _items[itemId].GetBoolFieldValue(DatItem.RemoveKey) != true)
                    datItems[itemId] = _items[itemId];
            }

            return datItems;
        }

        /// <summary>
        /// Get the indices and items associated with a machine index
        /// </summary>
        public IDictionary<long, DatItem>? GetItemsForMachine(long machineIndex, bool filter = false)
        {
            var itemIds = _itemToMachineMapping
                .Where(mapping => mapping.Value == machineIndex)
                .Select(mapping => mapping.Key);

            var datItems = new Dictionary<long, DatItem>();
            foreach (long itemId in itemIds)
            {
                // Ignore missing IDs
                if (!_items.ContainsKey(itemId))
                    continue;

                if (!filter || _items[itemId].GetBoolFieldValue(DatItem.RemoveKey) != true)
                    datItems[itemId] = _items[itemId];
            }

            return datItems;
        }

        /// <summary>
        /// Get the indices and items associated with a source index
        /// </summary>
        public IDictionary<long, DatItem>? GetItemsForSource(long sourceIndex, bool filter = false)
        {
            var itemIds = _itemToSourceMapping
                .Where(mapping => mapping.Value == sourceIndex)
                .Select(mapping => mapping.Key);

            var datItems = new Dictionary<long, DatItem>();
            foreach (long itemId in itemIds)
            {
                // Ignore missing IDs
                if (!_items.ContainsKey(itemId))
                    continue;

                if (!filter || _items[itemId].GetBoolFieldValue(DatItem.RemoveKey) != true)
                    datItems[itemId] = _items[itemId];
            }

            return datItems;
        }

        /// <summary>
        /// Get a machine based on the index
        /// </summary>
        public Machine? GetMachine(long index)
        {
            if (!_machines.ContainsKey(index))
                return null;

            return _machines[index];
        }

        /// <summary>
        /// Get a machine based on the name
        /// </summary>
        /// <remarks>This assume that all machines have unique names</remarks>
        public KeyValuePair<long, Machine?> GetMachine(string? name)
        {
            if (string.IsNullOrEmpty(name))
                return new KeyValuePair<long, Machine?>(-1, null);

            var machine = _machines.FirstOrDefault(m => m.Value.GetStringFieldValue(Models.Metadata.Machine.NameKey) == name);
            return new KeyValuePair<long, Machine?>(machine.Key, machine.Value);
        }

        /// <summary>
        /// Get the index and machine associated with an item index
        /// </summary>
        public KeyValuePair<long, Machine?> GetMachineForItem(long itemIndex)
        {
            if (!_itemToMachineMapping.ContainsKey(itemIndex))
                return new KeyValuePair<long, Machine?>(-1, null);

            long machineIndex = _itemToMachineMapping[itemIndex];
            if (!_machines.ContainsKey(machineIndex))
                return new KeyValuePair<long, Machine?>(-1, null);

            return new KeyValuePair<long, Machine?>(machineIndex, _machines[machineIndex]);
        }

        /// <summary>
        /// Get all machines and their indicies
        /// </summary>
        public IDictionary<long, Machine> GetMachines() => _machines;

        /// <summary>
        /// Get a source based on the index
        /// </summary>
        public Source? GetSource(long index)
        {
            if (!_sources.ContainsKey(index))
                return null;

            return _sources[index];
        }

        /// <summary>
        /// Get the index and source associated with an item index
        /// </summary>
        public KeyValuePair<long, Source?> GetSourceForItem(long itemIndex)
        {
            if (!_itemToSourceMapping.ContainsKey(itemIndex))
                return new KeyValuePair<long, Source?>(-1, null);

            long sourceIndex = _itemToSourceMapping[itemIndex];
            if (!_sources.ContainsKey(sourceIndex))
                return new KeyValuePair<long, Source?>(-1, null);

            return new KeyValuePair<long, Source?>(sourceIndex, _sources[sourceIndex]);
        }

        /// <summary>
        /// Get all sources and their indicies
        /// </summary>
        public IDictionary<long, Source> GetSources() => _sources;

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

        /// <summary>
        /// Remove a machine, returning if it could be removed
        /// </summary>
        public bool RemoveMachine(string machineName)
        {
            if (string.IsNullOrEmpty(machineName))
                return false;

            var machine = _machines.FirstOrDefault(m => m.Value.GetStringFieldValue(Models.Metadata.Machine.NameKey) == machineName);
            return RemoveMachine(machine.Key);
        }

        /// <summary>
        /// Add an item, returning the insert index
        /// </summary>
        internal long AddItem(DatItem item, long machineIndex, long sourceIndex)
        {
            // Add the item with a new index
            _items[_itemIndex++] = item;

            // Add the machine mapping
            _itemToMachineMapping[_itemIndex - 1] = machineIndex;

            // Add the source mapping
            _itemToSourceMapping[_itemIndex - 1] = sourceIndex;

            // Add the item statistics
            DatStatistics.AddItemStatistics(item);

            // Add the item to the default bucket
            PerformItemBucketing(_itemIndex - 1, _bucketedBy, lower: true, norename: true);

            // Return the used index
            return _itemIndex - 1;
        }

        #endregion

        // TODO: All internal, can this be put into a better location?
        #region Bucketing

        /// <summary>
        /// Update the bucketing dictionary
        /// </summary>
        /// <param name="bucketBy">ItemKey enum representing how to bucket the individual items</param>
        /// <param name="dedupeType">Dedupe type that should be used</param>
        /// <param name="lower">True if the key should be lowercased (default), false otherwise</param>
        /// <param name="norename">True if games should only be compared on game and file name, false if system and source are counted</param>
        /// <returns></returns>
        internal void BucketBy(ItemKey bucketBy, DedupeType dedupeType, bool lower = true, bool norename = true)
        {
            // If the sorted type isn't the same, we want to sort the dictionary accordingly
            if (_bucketedBy != bucketBy && bucketBy != ItemKey.NULL)
                PerformBucketing(bucketBy, lower, norename);

            // If the merge type isn't the same, we want to merge the dictionary accordingly
            if (dedupeType != DedupeType.None)
            {
                PerformDeduplication(bucketBy, dedupeType);
            }
            // If the merge type is the same, we want to sort the dictionary to be consistent
            else
            {
                PerformSorting(norename);
            }
        }

        /// <summary>
        /// List all duplicates found in a DAT based on a DatItem
        /// </summary>
        /// <param name="datItem">Item to try to match</param>
        /// <param name="sorted">True if the DAT is already sorted accordingly, false otherwise (default)</param>
        /// <returns>List of matched DatItem objects</returns>
        internal Dictionary<long, DatItem> GetDuplicates(DatItem datItem, bool sorted = false)
        {
            Dictionary<long, DatItem> output = [];

            // Check for an empty rom list first
            if (DatStatistics.TotalCount == 0)
                return output;

            // We want to get the proper key for the DatItem
            string key = SortAndGetKey(datItem, sorted);

            // If the key doesn't exist, return the empty list
            var roms = GetItemsForBucket(key);
            if (roms == null || roms.Count == 0)
                return output;

            // Try to find duplicates
            Dictionary<long, DatItem> left = [];
            foreach (var rom in roms)
            {
                if (rom.Value.GetBoolFieldValue(DatItem.RemoveKey) == true)
                    continue;

                if (datItem.Equals(rom.Value))
                {
                    rom.Value.SetFieldValue<bool?>(DatItem.RemoveKey, true);
                    output[rom.Key] = rom.Value;
                }
                else
                {
                    left[rom.Key] = rom.Value;
                }
            }

            // Add back all roms with the proper flags
            _buckets[key] = [.. output.Keys, .. left.Keys];
            return output;
        }

        /// <summary>
        /// List all duplicates found in a DAT based on a DatItem
        /// </summary>
        /// <param name="datItem">Item to try to match</param>
        /// <param name="sorted">True if the DAT is already sorted accordingly, false otherwise (default)</param>
        /// <returns>List of matched DatItem objects</returns>
        /// <remarks>This also sets the remove flag on any duplicates found</remarks>
        internal Dictionary<long, DatItem> GetDuplicates(KeyValuePair<long, DatItem> datItem, bool sorted = false)
        {
            Dictionary<long, DatItem> output = [];

            // Check for an empty rom list first
            if (DatStatistics.TotalCount == 0)
                return output;

            // We want to get the proper key for the DatItem
            string key = SortAndGetKey(datItem, sorted);

            // If the key doesn't exist, return the empty list
            var roms = GetItemsForBucket(key);
            if (roms == null || roms.Count == 0)
                return output;

            // Try to find duplicates
            Dictionary<long, DatItem> left = [];
            foreach (var rom in roms)
            {
                if (rom.Value.GetBoolFieldValue(DatItem.RemoveKey) == true)
                {
                    left[rom.Key] = rom.Value;
                    continue;
                }

                if (datItem.Value.Equals(rom.Value))
                {
                    rom.Value.SetFieldValue<bool?>(DatItem.RemoveKey, true);
                    output[rom.Key] = rom.Value;
                }
                else
                {
                    left[rom.Key] = rom.Value;
                }
            }

            // Add back all roms with the proper flags
            _buckets[key] = [.. output.Keys, .. left.Keys];
            return output;
        }

        /// <summary>
        /// Check if a DAT contains the given DatItem
        /// </summary>
        /// <param name="datItem">Item to try to match</param>
        /// <param name="sorted">True if the DAT is already sorted accordingly, false otherwise (default)</param>
        /// <returns>True if it contains the rom, false otherwise</returns>
        internal bool HasDuplicates(KeyValuePair<long, DatItem> datItem, bool sorted = false)
        {
            // Check for an empty rom list first
            if (DatStatistics.TotalCount == 0)
                return false;

            // We want to get the proper key for the DatItem
            string key = SortAndGetKey(datItem, sorted);

            // If the key doesn't exist
            var roms = GetItemsForBucket(key);
            if (roms == null || roms.Count == 0)
                return false;

            // Try to find duplicates
            return roms.Values.Any(r => datItem.Equals(r));
        }

        /// <summary>
        /// Merge an arbitrary set of item pairs based on the supplied information
        /// </summary>
        /// <param name="itemMappings">List of pairs representing the items to be merged</param>
        private List<KeyValuePair<long, DatItem>> Merge(List<KeyValuePair<long, DatItem>> itemMappings)
        {
            // Check for null or blank roms first
            if (itemMappings == null || itemMappings.Count == 0)
                return [];

            // Create output list
            List<KeyValuePair<long, DatItem>> output = [];

            // Then deduplicate them by checking to see if data matches previous saved roms
            int nodumpCount = 0;
            foreach (var kvp in itemMappings)
            {
                long itemIndex = kvp.Key;
                DatItem datItem = kvp.Value;

                // If we don't have a Disk, File, Media, or Rom, we skip checking for duplicates
                if (datItem is not Disk && datItem is not DatItems.Formats.File && datItem is not Media && datItem is not Rom)
                    continue;

                // If it's a nodump, add and skip
                if (datItem is Rom rom && rom.GetStringFieldValue(Models.Metadata.Rom.StatusKey).AsEnumValue<ItemStatus>() == ItemStatus.Nodump)
                {
                    output.Add(new KeyValuePair<long, DatItem>(itemIndex, datItem));
                    nodumpCount++;
                    continue;
                }
                else if (datItem is Disk disk && disk.GetStringFieldValue(Models.Metadata.Disk.StatusKey).AsEnumValue<ItemStatus>() == ItemStatus.Nodump)
                {
                    output.Add(new KeyValuePair<long, DatItem>(itemIndex, datItem));
                    nodumpCount++;
                    continue;
                }

                // If it's the first non-nodump rom in the list, don't touch it
                if (output.Count == nodumpCount)
                {
                    output.Add(new KeyValuePair<long, DatItem>(itemIndex, datItem));
                    continue;
                }

                // Find the index of the first duplicate, if one exists
                int pos = output.FindIndex(lastItem => datItem.GetDuplicateStatus(lastItem.Value) != 0x00);
                if (pos < 0)
                {
                    output.Add(new KeyValuePair<long, DatItem>(itemIndex, datItem));
                    continue;
                }

                // Get the duplicate item
                long savedIndex = output[pos].Key;
                DatItem savedItem = output[pos].Value;
                DupeType dupetype = datItem.GetDuplicateStatus(savedItem);

                // Disks, Media, and Roms have more information to fill
                if (datItem is Disk diskItem && savedItem is Disk savedDisk)
                    savedDisk.FillMissingInformation(diskItem);
                else if (datItem is DatItems.Formats.File fileItem && savedItem is DatItems.Formats.File savedFile)
                    savedFile.FillMissingInformation(fileItem);
                else if (datItem is Media mediaItem && savedItem is Media savedMedia)
                    savedMedia.FillMissingInformation(mediaItem);
                else if (datItem is Rom romItem && savedItem is Rom savedRom)
                    savedRom.FillMissingInformation(romItem);

                savedItem.SetFieldValue<DupeType>(DatItem.DupeTypeKey, dupetype);

                // Get the sources associated with the items
                var savedSource = _sources[_itemToSourceMapping[savedIndex]];
                var itemSource = _sources[_itemToSourceMapping[itemIndex]];

                // Get the machines associated with the items
                var savedMachine = _machines[_itemToMachineMapping[savedIndex]];
                var itemMachine = _machines[_itemToMachineMapping[itemIndex]];

                // If the current source has a lower ID than the saved, use the saved source
                if (itemSource?.Index < savedSource?.Index)
                {
                    _itemToSourceMapping[itemIndex] = _itemToSourceMapping[savedIndex];
                    _machines[_itemToMachineMapping[savedIndex]] = (itemMachine.Clone() as Machine)!;
                    savedItem.SetName(datItem.GetName());
                }

                // If the saved machine is a child of the current machine, use the current machine instead
                if (savedMachine.GetStringFieldValue(Models.Metadata.Machine.CloneOfKey) == itemMachine.GetStringFieldValue(Models.Metadata.Machine.NameKey)
                    || savedMachine.GetStringFieldValue(Models.Metadata.Machine.RomOfKey) == itemMachine.GetStringFieldValue(Models.Metadata.Machine.NameKey))
                {
                    _machines[_itemToMachineMapping[savedIndex]] = (itemMachine.Clone() as Machine)!;
                    savedItem.SetName(datItem.GetName());
                }

                // Replace the original item in the list
                output.RemoveAt(pos);
                output.Insert(pos, new KeyValuePair<long, DatItem>(savedIndex, savedItem));
            }

            return output;
        }

        /// <summary>
        /// Get the highest-order Field value that represents the statistics
        /// </summary>
        private ItemKey GetBestAvailable()
        {
            // Get the required counts
            long diskCount = DatStatistics.GetItemCount(ItemType.Disk);
            long mediaCount = DatStatistics.GetItemCount(ItemType.Media);
            long romCount = DatStatistics.GetItemCount(ItemType.Rom);
            long nodumpCount = DatStatistics.GetStatusCount(ItemStatus.Nodump);

            // If all items are supposed to have a SHA-512, we bucket by that
            if (diskCount + mediaCount + romCount - nodumpCount == DatStatistics.GetHashCount(HashType.SHA512))
                return ItemKey.SHA512;

            // If all items are supposed to have a SHA-384, we bucket by that
            else if (diskCount + mediaCount + romCount - nodumpCount == DatStatistics.GetHashCount(HashType.SHA384))
                return ItemKey.SHA384;

            // If all items are supposed to have a SHA-256, we bucket by that
            else if (diskCount + mediaCount + romCount - nodumpCount == DatStatistics.GetHashCount(HashType.SHA256))
                return ItemKey.SHA256;

            // If all items are supposed to have a SHA-1, we bucket by that
            else if (diskCount + mediaCount + romCount - nodumpCount == DatStatistics.GetHashCount(HashType.SHA1))
                return ItemKey.SHA1;

            // If all items are supposed to have a MD5, we bucket by that
            else if (diskCount + mediaCount + romCount - nodumpCount == DatStatistics.GetHashCount(HashType.MD5))
                return ItemKey.MD5;

            // If all items are supposed to have a MD4, we bucket by that
            else if (diskCount + mediaCount + romCount - nodumpCount == DatStatistics.GetHashCount(HashType.MD4))
                return ItemKey.MD4;

            // If all items are supposed to have a MD2, we bucket by that
            else if (diskCount + mediaCount + romCount - nodumpCount == DatStatistics.GetHashCount(HashType.MD2))
                return ItemKey.MD2;

            // Otherwise, we bucket by CRC
            else
                return ItemKey.CRC;
        }

        /// <summary>
        /// Get the bucketing key for a given item index
        /// <param name="itemIndex">Index of the current item</param>
        /// <param name="bucketBy">ItemKey value representing what key to get</param>
        /// <param name="lower">True if the key should be lowercased, false otherwise</param>
        /// <param name="norename">True if games should only be compared on game and file name, false if system and source are counted</param>
        /// </summary>
        private string GetBucketKey(long itemIndex, ItemKey bucketBy, bool lower, bool norename)
        {
            if (!_items.ContainsKey(itemIndex))
                return string.Empty;

            var datItem = _items[itemIndex];
            if (datItem == null)
                return string.Empty;

            var machine = GetMachineForItem(itemIndex);
            if (machine.Value == null)
                return string.Empty;

            var source = GetSourceForItem(itemIndex);

            string sourceKeyPadded = source.Value?.Index.ToString().PadLeft(10, '0') + '-';
            string machineName = machine.Value.GetStringFieldValue(Models.Metadata.Machine.NameKey) ?? "Default";

            string bucketKey = bucketBy switch
            {
                ItemKey.Machine => (norename ? string.Empty : sourceKeyPadded) + machineName,
                _ => GetBucketHashValue(datItem, bucketBy),
            };

            if (lower)
                bucketKey = bucketKey.ToLowerInvariant();

            return bucketKey;
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
                    ItemKey.CRC => ZeroHash.CRC32Str,
                    ItemKey.MD2 => ZeroHash.GetString(HashType.MD2),
                    ItemKey.MD4 => ZeroHash.GetString(HashType.MD4),
                    ItemKey.MD5 => disk.GetStringFieldValue(Models.Metadata.Disk.MD5Key) ?? string.Empty,
                    ItemKey.SHA1 => disk.GetStringFieldValue(Models.Metadata.Disk.SHA1Key) ?? string.Empty,
                    ItemKey.SHA256 => ZeroHash.SHA256Str,
                    ItemKey.SHA384 => ZeroHash.SHA384Str,
                    ItemKey.SHA512 => ZeroHash.SHA512Str,
                    ItemKey.SpamSum => ZeroHash.SpamSumStr,
                    _ => string.Empty,
                },
                Media media => bucketBy switch
                {
                    ItemKey.CRC => ZeroHash.CRC32Str,
                    ItemKey.MD2 => ZeroHash.GetString(HashType.MD2),
                    ItemKey.MD4 => ZeroHash.GetString(HashType.MD4),
                    ItemKey.MD5 => media.GetStringFieldValue(Models.Metadata.Media.MD5Key) ?? string.Empty,
                    ItemKey.SHA1 => media.GetStringFieldValue(Models.Metadata.Media.SHA1Key) ?? string.Empty,
                    ItemKey.SHA256 => media.GetStringFieldValue(Models.Metadata.Media.SHA256Key) ?? string.Empty,
                    ItemKey.SHA384 => ZeroHash.SHA384Str,
                    ItemKey.SHA512 => ZeroHash.SHA512Str,
                    ItemKey.SpamSum => media.GetStringFieldValue(Models.Metadata.Media.SpamSumKey) ?? string.Empty,
                    _ => string.Empty,
                },
                Rom rom => bucketBy switch
                {
                    ItemKey.CRC => rom.GetStringFieldValue(Models.Metadata.Rom.CRCKey) ?? string.Empty,
                    ItemKey.MD2 => rom.GetStringFieldValue(Models.Metadata.Rom.MD2Key) ?? string.Empty,
                    ItemKey.MD4 => rom.GetStringFieldValue(Models.Metadata.Rom.MD4Key) ?? string.Empty,
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
                    ItemKey.CRC => ZeroHash.CRC32Str,
                    ItemKey.MD2 => ZeroHash.GetString(HashType.MD2),
                    ItemKey.MD4 => ZeroHash.GetString(HashType.MD4),
                    ItemKey.MD5 => ZeroHash.MD5Str,
                    ItemKey.SHA1 => ZeroHash.SHA1Str,
                    ItemKey.SHA256 => ZeroHash.SHA256Str,
                    ItemKey.SHA384 => ZeroHash.SHA384Str,
                    ItemKey.SHA512 => ZeroHash.SHA512Str,
                    ItemKey.SpamSum => ZeroHash.SpamSumStr,
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
        /// Perform bucketing based on the item key provided
        /// </summary>
        /// <param name="bucketBy">ItemKey enum representing how to bucket the individual items</param>
        /// <param name="lower">True if the key should be lowercased, false otherwise</param>
        /// <param name="norename">True if games should only be compared on game and file name, false if system and source are counted</param>
        private void PerformBucketing(ItemKey bucketBy, bool lower, bool norename)
        {
            // Reset the bucketing values
            _bucketedBy = bucketBy;
            _buckets.Clear();

            // Get the current list of item indicies
            long[] itemIndicies = [.. _items.Keys];

#if NET452_OR_GREATER || NETCOREAPP
            Parallel.For(0, itemIndicies.Length, Core.Globals.ParallelOptions, i =>
#elif NET40_OR_GREATER
            Parallel.For(0, itemIndicies.Length, i =>
#else
            for (int i = 0; i < itemIndicies.Length; i++)
#endif
            {
                PerformItemBucketing(i, bucketBy, lower, norename);
#if NET40_OR_GREATER || NETCOREAPP
            });
#else
            }
#endif
        }

        /// <summary>
        /// Bucket a single DatItem
        /// </summary>
        /// <param name="itemIndex">Index of the item to bucket</param>
        /// <param name="bucketBy">ItemKey enum representing how to bucket the individual items</param>
        /// <param name="lower">True if the key should be lowercased, false otherwise</param>
        /// <param name="norename">True if games should only be compared on game and file name, false if system and source are counted</param>
        private void PerformItemBucketing(long itemIndex, ItemKey bucketBy, bool lower, bool norename)
        {
            string? bucketKey = GetBucketKey(itemIndex, bucketBy, lower, norename);
            EnsureBucketingKey(bucketKey);
            _buckets[bucketKey].Add(itemIndex);
        }

        /// <summary>
        /// Perform deduplication based on the deduplication type provided
        /// </summary>
        /// <param name="bucketBy">ItemKey enum representing how to bucket the individual items</param>
        /// <param name="dedupeType">Dedupe type that should be used</param>
        private void PerformDeduplication(ItemKey bucketBy, DedupeType dedupeType)
        {
            // Get the current list of bucket keys
            string[] bucketKeys = [.. _buckets.Keys];

#if NET452_OR_GREATER || NETCOREAPP
            Parallel.For(0, bucketKeys.Length, Core.Globals.ParallelOptions, i =>
#elif NET40_OR_GREATER
            Parallel.For(0, bucketKeys.Length, i =>
#else
            for (int i = 0; i < bucketKeys.Length; i++)
#endif
            {
                var itemIndices = _buckets[bucketKeys[i]];
                if (itemIndices == null || itemIndices.Count == 0)
#if NET40_OR_GREATER || NETCOREAPP
                    return;
#else
                    continue;
#endif

                var datItems = itemIndices
                    .FindAll(i => _items.ContainsKey(i))
                    .Select(i => new KeyValuePair<long, DatItem>(i, _items[i]))
                    .ToList();

                Sort(ref datItems, false);

                // If we're merging the roms, do so
                if (dedupeType == DedupeType.Full || (dedupeType == DedupeType.Game && bucketBy == ItemKey.Machine))
                    datItems = Merge(datItems);

                _buckets[bucketKeys[i]] = [.. datItems.Select(kvp => kvp.Key)];
#if NET40_OR_GREATER || NETCOREAPP
            });
#else
            }
#endif
        }

        /// <summary>
        /// Sort existing buckets for consistency
        /// </summary>
        private void PerformSorting(bool norename)
        {
            // Get the current list of bucket keys
            string[] bucketKeys = [.. _buckets.Keys];

#if NET452_OR_GREATER || NETCOREAPP
            Parallel.For(0, bucketKeys.Length, Core.Globals.ParallelOptions, i =>
#elif NET40_OR_GREATER
            Parallel.For(0, bucketKeys.Length, i =>
#else
            for (int i = 0; i < bucketKeys.Length; i++)
#endif
            {
                var itemIndices = _buckets[bucketKeys[i]];
                if (itemIndices == null || itemIndices.Count == 0)
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
                    .FindAll(i => _items.ContainsKey(i))
                    .Select(i => new KeyValuePair<long, DatItem>(i, _items[i]))
                    .ToList();

                Sort(ref datItems, norename);

                _buckets[bucketKeys[i]] = [.. datItems.Select(kvp => kvp.Key)];
#if NET40_OR_GREATER || NETCOREAPP
            });
#else
            }
#endif
        }

        /// <summary>
        /// Sort a list of item pairs by SourceID, Game, and Name (in order)
        /// </summary>
        /// <param name="itemMappings">List of pairs representing the items to be sorted</param>
        /// <param name="norename">True if files are not renamed, false otherwise</param>
        /// <returns>True if it sorted correctly, false otherwise</returns>
        private bool Sort(ref List<KeyValuePair<long, DatItem>> itemMappings, bool norename)
        {
            itemMappings.Sort(delegate (KeyValuePair<long, DatItem> x, KeyValuePair<long, DatItem> y)
            {
                try
                {
                    var nc = new NaturalComparer();

                    // If machine names don't match
                    string? xMachineName = _machines[_itemToMachineMapping[x.Key]].GetStringFieldValue(Models.Metadata.Machine.NameKey);
                    string? yMachineName = _machines[_itemToMachineMapping[y.Key]].GetStringFieldValue(Models.Metadata.Machine.NameKey);
                    if (xMachineName != yMachineName)
                        return nc.Compare(xMachineName, yMachineName);

                    // If types don't match
                    string? xType = x.Value.GetStringFieldValue(Models.Metadata.DatItem.TypeKey);
                    string? yType = y.Value.GetStringFieldValue(Models.Metadata.DatItem.TypeKey);
                    if (xType != yType)
                        return xType.AsEnumValue<ItemType>() - yType.AsEnumValue<ItemType>();

                    // If directory names don't match
                    string? xDirectoryName = Path.GetDirectoryName(TextHelper.RemovePathUnsafeCharacters(x.Value.GetName()));
                    string? yDirectoryName = Path.GetDirectoryName(TextHelper.RemovePathUnsafeCharacters(y.Value.GetName()));
                    if (xDirectoryName != yDirectoryName)
                        return nc.Compare(xDirectoryName, yDirectoryName);

                    // If item names don't match
                    string? xName = Path.GetFileName(TextHelper.RemovePathUnsafeCharacters(x.Value.GetName()));
                    string? yName = Path.GetFileName(TextHelper.RemovePathUnsafeCharacters(y.Value.GetName()));
                    if (xName != yName)
                        return nc.Compare(xName, yName);

                    // Otherwise, compare on machine or source, depending on the flag
                    int? xSourceIndex = GetSourceForItem(x.Key).Value?.Index;
                    int? ySourceIndex = GetSourceForItem(y.Key).Value?.Index;
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

        /// <summary>
        /// Sort the input DAT and get the key to be used by the item
        /// </summary>
        /// <param name="datItem">Item to try to match</param>
        /// <param name="sorted">True if the DAT is already sorted accordingly, false otherwise (default)</param>
        /// <returns>Key to try to use</returns>
        private string SortAndGetKey(DatItem datItem, bool sorted = false)
        {
            // If we're not already sorted, take care of it
            if (!sorted)
                BucketBy(GetBestAvailable(), DedupeType.None);

            // Now that we have the sorted type, we get the proper key
            return datItem.GetKeyDB(_bucketedBy, null);
        }

        /// <summary>
        /// Sort the input DAT and get the key to be used by the item
        /// </summary>
        /// <param name="datItem">Item to try to match</param>
        /// <param name="sorted">True if the DAT is already sorted accordingly, false otherwise (default)</param>
        /// <returns>Key to try to use</returns>
        private string SortAndGetKey(KeyValuePair<long, DatItem> datItem, bool sorted = false)
        {
            // If we're not already sorted, take care of it
            if (!sorted)
                BucketBy(GetBestAvailable(), DedupeType.None);

            // Now that we have the sorted type, we get the proper key
            var source = GetSourceForItem(datItem.Key);
            return datItem.Value.GetKeyDB(_bucketedBy, source.Value);
        }

        #endregion

        // TODO: All internal, can this be put into a better location?
        #region Filtering

        /// <summary>
        /// Execute all filters in a filter runner on the items in the dictionary
        /// </summary>
        /// <param name="filterRunner">Preconfigured filter runner to use</param>
        internal void ExecuteFilters(FilterRunner filterRunner)
        {
            List<string> keys = [.. SortedKeys];
#if NET452_OR_GREATER || NETCOREAPP
            Parallel.ForEach(keys, Core.Globals.ParallelOptions, key =>
#elif NET40_OR_GREATER
            Parallel.ForEach(keys, key =>
#else
            foreach (var key in keys)
#endif
            {
                ExecuteFilterOnBucket(filterRunner, key);
#if NET40_OR_GREATER || NETCOREAPP
            });
#else
            }
#endif
        }

        /// <summary>
        /// Use game descriptions as names, updating cloneof/romof/sampleof
        /// </summary>
        /// <param name="throwOnError">True if the error that is thrown should be thrown back to the caller, false otherwise</param>
        internal void MachineDescriptionToName(bool throwOnError = false)
        {
            try
            {
                // First we want to get a mapping for all games to description
                var mapping = CreateMachineToDescriptionMapping();

                // Now we loop through every item and update accordingly
                UpdateMachineNamesFromDescriptions(mapping);
            }
            catch (Exception ex) when (!throwOnError)
            {
                _logger.Warning(ex.ToString());
            }
        }

        /// <summary>
        /// Ensure that all roms are in their own game (or at least try to ensure)
        /// </summary>
        internal void SetOneRomPerGame()
        {
            // For each rom, we want to update the game to be "<game name>/<rom name>"
#if NET452_OR_GREATER || NETCOREAPP
            Parallel.ForEach(SortedKeys, Core.Globals.ParallelOptions, key =>
#elif NET40_OR_GREATER
            Parallel.ForEach(SortedKeys, key =>
#else
            foreach (var key in SortedKeys)
#endif
            {
                var items = GetItemsForBucket(key);
                if (items == null)
#if NET40_OR_GREATER || NETCOREAPP
                    return;
#else
                    continue;
#endif

                foreach (var item in items)
                {
                    SetOneRomPerGame(item);
                }
#if NET40_OR_GREATER || NETCOREAPP
            });
#else
            }
#endif
        }

        /// <summary>
        /// Filter a DAT using 1G1R logic given an ordered set of regions
        /// </summary>
        /// <param name="regionList">List of regions in order of priority</param>
        /// <remarks>
        /// In the most technical sense, the way that the region list is being used does not
        /// confine its values to be just regions. Since it's essentially acting like a
        /// specialized version of the machine name filter, anything that is usually encapsulated
        /// in parenthesis would be matched on, including disc numbers, languages, editions,
        /// and anything else commonly used. Please note that, unlike other existing 1G1R 
        /// solutions, this does not have the ability to contain custom mappings of parent
        /// to clone sets based on name, nor does it have the ability to match on the 
        /// Release DatItem type.
        /// </remarks>
        internal void SetOneGamePerRegion(List<string> regionList)
        {
            // If we have null region list, make it empty
            regionList ??= [];

            // For sake of ease, the first thing we want to do is bucket by game
            BucketBy(ItemKey.Machine, DedupeType.None, norename: true);

            // Then we want to get a mapping of all machines to parents
            Dictionary<string, List<string>> parents = [];
            foreach (string key in SortedKeys)
            {
                var items = GetItemsForBucket(key);
                if (items == null || items.Count == 0)
                    continue;

                var item = items.First();
                var machine = GetMachineForItem(item.Key);
                if (machine.Value == null)
                    continue;

                // Get machine information
                Machine? machineObj = machine.Value.GetFieldValue<Machine>(DatItem.MachineKey);
                string? machineName = machineObj?.GetStringFieldValue(Models.Metadata.Machine.NameKey)?.ToLowerInvariant();
                if (machineObj == null || machineName == null)
                    continue;

                // Get the string values
                string? cloneOf = machineObj.GetStringFieldValue(Models.Metadata.Machine.CloneOfKey)?.ToLowerInvariant();
                string? romOf = machineObj.GetStringFieldValue(Models.Metadata.Machine.RomOfKey)?.ToLowerInvariant();

                // Match on CloneOf first
                if (!string.IsNullOrEmpty(cloneOf))
                {
                    if (!parents.ContainsKey(cloneOf!))
                        parents.Add(cloneOf!, []);

                    parents[cloneOf!].Add(machineName);
                }

                // Then by RomOf
                else if (!string.IsNullOrEmpty(romOf))
                {
                    if (!parents.ContainsKey(romOf!))
                        parents.Add(romOf!, []);

                    parents[romOf!].Add(machineName);
                }

                // Otherwise, treat it as a parent
                else
                {
                    if (!parents.ContainsKey(machineName))
                        parents.Add(machineName, []);

                    parents[machineName].Add(machineName);
                }
            }

            // Once we have the full list of mappings, filter out games to keep
            foreach (string key in parents.Keys)
            {
                // Find the first machine that matches the regions in order, if possible
                string? machine = default;
                foreach (string region in regionList)
                {
                    machine = parents[key].Find(m => Regex.IsMatch(m, @"\(.*" + region + @".*\)", RegexOptions.IgnoreCase));
                    if (machine != default)
                        break;
                }

                // If we didn't get a match, use the parent
                if (machine == default)
                    machine = key;

                // Remove the key from the list
                parents[key].Remove(machine);

                // Remove the rest of the items from this key
                parents[key].ForEach(k => RemoveMachine(k));
            }

            // Finally, strip out the parent tags
            RemoveTagsFromChild();
        }

        /// <summary>
        /// Strip the dates from the beginning of scene-style set names
        /// </summary>
        internal void StripSceneDatesFromItems()
        {
            // Set the regex pattern to use
            const string pattern = @"([0-9]{2}\.[0-9]{2}\.[0-9]{2}-)(.*?-.*?)";

            // Now process all of the machines
#if NET452_OR_GREATER || NETCOREAPP
            Parallel.ForEach(GetMachines(), Core.Globals.ParallelOptions, machine =>
#elif NET40_OR_GREATER
            Parallel.ForEach(GetMachines(), machine =>
#else
            foreach (var machine in GetMachines())
#endif
            {
                if (machine.Value == null)
#if NET40_OR_GREATER || NETCOREAPP
                    return;
#else
                    continue;
#endif

                if (Regex.IsMatch(machine.Value.GetStringFieldValue(Models.Metadata.Machine.NameKey)!, pattern))
                    machine.Value.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, Regex.Replace(machine.Value.GetStringFieldValue(Models.Metadata.Machine.NameKey)!, pattern, "$2"));

                if (Regex.IsMatch(machine.Value.GetStringFieldValue(Models.Metadata.Machine.DescriptionKey)!, pattern))
                    machine.Value.SetFieldValue<string?>(Models.Metadata.Machine.DescriptionKey, Regex.Replace(machine.Value.GetStringFieldValue(Models.Metadata.Machine.DescriptionKey)!, pattern, "$2"));
#if NET40_OR_GREATER || NETCOREAPP
            });
#else
            }
#endif
        }

        /// <summary>
        /// Create machine to description mapping dictionary
        /// </summary>
        private IDictionary<string, string> CreateMachineToDescriptionMapping()
        {
#if NET40_OR_GREATER || NETCOREAPP
            ConcurrentDictionary<string, string> mapping = new();
#else
            Dictionary<string, string> mapping = [];
#endif
#if NET452_OR_GREATER || NETCOREAPP
            Parallel.ForEach(SortedKeys, Core.Globals.ParallelOptions, key =>
#elif NET40_OR_GREATER
            Parallel.ForEach(SortedKeys, key =>
#else
            foreach (var key in SortedKeys)
#endif
            {
                var items = GetItemsForBucket(key);
                if (items == null)
#if NET40_OR_GREATER || NETCOREAPP
                    return;
#else
                    continue;
#endif

                foreach (var item in items)
                {
                    // Get the current machine
                    var machine = GetMachineForItem(item.Key);
                    if (machine.Value == null)
                        continue;

                    // If the key mapping doesn't exist, add it
#if NET40_OR_GREATER || NETCOREAPP
                    mapping.TryAdd(machine.Value.GetStringFieldValue(Models.Metadata.Machine.NameKey)!,
                        machine.Value.GetStringFieldValue(Models.Metadata.Machine.DescriptionKey)!.Replace('/', '_').Replace("\"", "''").Replace(":", " -"));
#else
                    mapping[machine.Value.GetStringFieldValue(Models.Metadata.Machine.NameKey)!]
                        = machine.Value.GetStringFieldValue(Models.Metadata.Machine.DescriptionKey)!.Replace('/', '_').Replace("\"", "''").Replace(":", " -");
#endif
                }
#if NET40_OR_GREATER || NETCOREAPP
            });
#else
            }
#endif

            return mapping;
        }

        /// <summary>
        /// Execute all filters in a filter runner on a single bucket
        /// </summary>
        /// <param name="filterRunner">Preconfigured filter runner to use</param>
        /// <param name="bucketName">Name of the bucket to filter on</param>
        private void ExecuteFilterOnBucket(FilterRunner filterRunner, string bucketName)
        {
            var items = GetItemsForBucket(bucketName);
            if (items == null)
                return;

            // Filter all items in the current key
            List<long> newItems = [];
            foreach (var item in items)
            {
                if (item.Value.PassesFilterDB(filterRunner))
                    newItems.Add(item.Key);
            }

            // Set the value in the key to the new set
            _buckets[bucketName] = newItems;
        }

        /// <summary>
        /// Set internal names to match One Rom Per Game (ORPG) logic
        /// </summary>
        /// <param name="datItem">DatItem to run logic on</param>
        private void SetOneRomPerGame(KeyValuePair<long, DatItem> datItem)
        {
            // If the item name is null
            string? machineName = datItem.Value.GetName();
            if (datItem.Key < 0 || machineName == null)
                return;

            // Get the current machine
            var machine = GetMachineForItem(datItem.Key);
            if (machine.Value == null)
                return;

            // Remove extensions from Rom items
            if (datItem.Value is Rom)
            {
                string[] splitname = machineName.Split('.');
                machineName = machine.Value.GetStringFieldValue(Models.Metadata.Machine.NameKey)
                    + $"/{string.Join(".", splitname, 0, splitname.Length > 1 ? splitname.Length - 1 : 1)}";
            }

            // Strip off "Default" prefix only for ORPG
            if (machineName.StartsWith("Default"))
                machineName = machineName.Substring("Default".Length + 1);

            machine.Value.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, machineName);
            datItem.Value.SetName(Path.GetFileName(datItem.Value.GetName()));
        }

        /// <summary>
        /// Update machine names from descriptions according to mappings
        /// </summary>
        private void UpdateMachineNamesFromDescriptions(IDictionary<string, string> mapping)
        {
#if NET452_OR_GREATER || NETCOREAPP
            Parallel.ForEach(SortedKeys, Core.Globals.ParallelOptions, key =>
#elif NET40_OR_GREATER
            Parallel.ForEach(SortedKeys, key =>
#else
            foreach (var key in SortedKeys)
#endif
            {
                var items = GetItemsForBucket(key);
                if (items == null)
#if NET40_OR_GREATER || NETCOREAPP
                    return;
#else
                    continue;
#endif

                List<long> newItems = [];
                foreach (var item in items)
                {
                    // Get the current machine
                    var machine = GetMachineForItem(item.Key);
                    if (machine.Value == null)
                        continue;

                    // Update machine name
                    if (!string.IsNullOrEmpty(machine.Value.GetStringFieldValue(Models.Metadata.Machine.NameKey)) && mapping.ContainsKey(machine.Value.GetStringFieldValue(Models.Metadata.Machine.NameKey)!))
                        machine.Value.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, mapping[machine.Value.GetStringFieldValue(Models.Metadata.Machine.NameKey)!]);

                    // Update cloneof
                    if (!string.IsNullOrEmpty(machine.Value.GetStringFieldValue(Models.Metadata.Machine.CloneOfKey)) && mapping.ContainsKey(machine.Value.GetStringFieldValue(Models.Metadata.Machine.CloneOfKey)!))
                        machine.Value.SetFieldValue<string?>(Models.Metadata.Machine.CloneOfKey, mapping[machine.Value.GetStringFieldValue(Models.Metadata.Machine.CloneOfKey)!]);

                    // Update romof
                    if (!string.IsNullOrEmpty(machine.Value.GetStringFieldValue(Models.Metadata.Machine.RomOfKey)) && mapping.ContainsKey(machine.Value.GetStringFieldValue(Models.Metadata.Machine.RomOfKey)!))
                        machine.Value.SetFieldValue<string?>(Models.Metadata.Machine.RomOfKey, mapping[machine.Value.GetStringFieldValue(Models.Metadata.Machine.RomOfKey)!]);

                    // Update sampleof
                    if (!string.IsNullOrEmpty(machine.Value.GetStringFieldValue(Models.Metadata.Machine.SampleOfKey)) && mapping.ContainsKey(machine.Value.GetStringFieldValue(Models.Metadata.Machine.SampleOfKey)!))
                        machine.Value.SetFieldValue<string?>(Models.Metadata.Machine.SampleOfKey, mapping[machine.Value.GetStringFieldValue(Models.Metadata.Machine.SampleOfKey)!]);

                    // Add the new item to the output list
                    newItems.Add(item.Key);
                }

                // Replace the old list of roms with the new one
                _buckets[key] = newItems;
#if NET40_OR_GREATER || NETCOREAPP
            });
#else
            }
#endif
        }

        #endregion

        // TODO: All internal, can this be put into a better location?
        #region Splitting

        /// <summary>
        /// Remove all romof and cloneof tags from all games
        /// </summary>
        internal void RemoveTagsFromChild()
        {
            List<string> games = [.. SortedKeys];
            foreach (string game in games)
            {
                // If the game has no items in it, we want to continue
                var items = GetItemsForBucket(game);
                if (items == null || items.Count == 0)
                    continue;

                foreach (long id in items.Keys)
                {
                    var machine = GetMachineForItem(id);
                    if (machine.Value == null)
                        continue;

                    machine.Value.SetFieldValue<string?>(Models.Metadata.Machine.CloneOfKey, null);
                    machine.Value.SetFieldValue<string?>(Models.Metadata.Machine.RomOfKey, null);
                    machine.Value.SetFieldValue<string?>(Models.Metadata.Machine.SampleOfKey, null);
                }
            }
        }

        #endregion

        #region Statistics

        /// <summary>
        /// Recalculate the statistics for the Dat
        /// </summary>
        public void RecalculateStats()
        {
            // Wipe out any stats already there
            DatStatistics.ResetStatistics();

            // If there are no items
            if (_items == null || _items.Count == 0)
                return;

            // Loop through and add
            foreach (var item in _items.Values)
            {
                if (item == null)
                    continue;

                DatStatistics.AddItemStatistics(item);
            }
        }

        #endregion
    }
}
