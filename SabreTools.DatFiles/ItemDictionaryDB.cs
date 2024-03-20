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
using SabreTools.Core;
using SabreTools.Core.Filter;
using SabreTools.Core.Tools;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;
using SabreTools.Hashing;
using SabreTools.Logging;
using SabreTools.Matching;

/*
 * Planning Notes:
 * 
 * In order for this in-memory "database" design to work, there need to be a few things:
 * - Feature parity with all existing item dictionary operations
 * - A way to transition between the two item dictionaries (a flag?)
 * - Helper methods that target the "database" version instead of assuming the standard dictionary
 * - Should sources be separated out the same as machines are?
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

        /// <summary>
        /// Logging object
        /// </summary>
        private readonly Logger logger;

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
                return keys.ToArray();
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
            logger = new Logger(this);
        }

        #region Accessors

        /// <summary>
        /// Add a DatItem to the dictionary after validation
        /// </summary>
        /// <param name="item">Item data to validate</param>
        /// <param name="machineIndex">Index of the machine related to the item</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise (default)</param>
        /// <returns>The index for the added item, -1 on error</returns>
        public long AddItem(DatItem item, long machineIndex, bool statsOnly)
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
                    rom.SetFieldValue<string?>(Models.Metadata.Rom.CRCKey, Constants.CRCZero);
                    rom.SetFieldValue<string?>(Models.Metadata.Rom.MD5Key, Constants.MD5Zero);
                    rom.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, Constants.SHA1Zero);
                    rom.SetFieldValue<string?>(Models.Metadata.Rom.SHA256Key, null); // Constants.SHA256Zero;
                    rom.SetFieldValue<string?>(Models.Metadata.Rom.SHA384Key, null); // Constants.SHA384Zero;
                    rom.SetFieldValue<string?>(Models.Metadata.Rom.SHA512Key, null); // Constants.SHA512Zero;
                    rom.SetFieldValue<string?>(Models.Metadata.Rom.SpamSumKey, null); // Constants.SpamSumZero;
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
                return AddItem(item, machineIndex);
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
        /// Remove any keys that have null or empty values
        /// </summary>
        public void ClearEmpty()
        {
            var keys = SortedKeys.Where(k => k != null).ToList();
            foreach (string key in keys)
            {
                // If the key doesn't exist, skip
                if (!_buckets.ContainsKey(key))
                    continue;

                // If the value is null, remove
                else if (_buckets[key] == null)
#if NET40_OR_GREATER || NETCOREAPP
                    _buckets.TryRemove(key, out _);
#else
                    _buckets.Remove(key);
#endif

                // If there are no non-blank items, remove
                else if (!_buckets[key]!.Any(i => GetItem(i) != null && GetItem(i) is not Blank))
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
        public void ClearMarked()
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
        public (long, Machine?) GetMachine(string? name)
        {
            if (string.IsNullOrEmpty(name))
                return (-1, null);

            var machine = _machines.FirstOrDefault(m => m.Value.GetStringFieldValue(Models.Metadata.Machine.NameKey) == name);
            return (machine.Key, machine.Value);
        }

        /// <summary>
        /// Get the index and machine associated with an item index
        /// </summary>
        public (long, Machine?) GetMachineForItem(long itemIndex)
        {
            if (!_itemToMachineMapping.ContainsKey(itemIndex))
                return (-1, null);

            long machineIndex = _itemToMachineMapping[itemIndex];
            if (!_machines.ContainsKey(machineIndex))
                return (-1, null);

            return (machineIndex, _machines[machineIndex]);
        }

        /// <summary>
        /// Get the indices and items associated with a bucket name
        /// </summary>
        public (long, DatItem)[]? GetDatItemsForBucket(string bucketName, bool filter = false)
        {
            if (!_buckets.ContainsKey(bucketName))
                return null;

            var itemIds = _buckets[bucketName];

            var datItems = new List<(long, DatItem)>();
            foreach (long itemId in itemIds)
            {
                if (_items.ContainsKey(itemId) && (!filter || _items[itemId].GetBoolFieldValue(DatItem.RemoveKey) != true))
                    datItems.Add((itemId, _items[itemId]));
            }

            return [.. datItems];
        }

        /// <summary>
        /// Get the indices and items associated with a machine index
        /// </summary>
        public (long, DatItem)[]? GetDatItemsForMachine(long machineIndex, bool filter = false)
        {
            var itemIds = _itemToMachineMapping
                .Where(mapping => mapping.Value == machineIndex)
                .Select(mapping => mapping.Key);

            var datItems = new List<(long, DatItem)>();
            foreach (long itemId in itemIds)
            {
                if (_items.ContainsKey(itemId) && (!filter || _items[itemId].GetBoolFieldValue(DatItem.RemoveKey) != true))
                    datItems.Add((itemId, _items[itemId]));
            }

            return [.. datItems];
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

        /// <summary>
        /// Add an item, returning the insert index
        /// </summary>
        private long AddItem(DatItem item, long machineIndex)
        {
            // Add the item with a new index
            _items[_itemIndex++] = item;

            // Add the machine mapping
            _itemToMachineMapping[_itemIndex - 1] = machineIndex;

            // Add the item statistics
            DatStatistics.AddItemStatistics(item);

            // Add the item to the default bucket
            PerformItemBucketing(_itemIndex - 1, _bucketedBy, lower: true, norename: true);

            // Return the used index
            return _itemIndex - 1;
        }

        #endregion

        #region Bucketing

        /// <summary>
        /// Update the bucketing dictionary
        /// </summary>
        /// <param name="bucketBy">ItemKey enum representing how to bucket the individual items</param>
        /// <param name="dedupeType">Dedupe type that should be used</param>
        /// <param name="lower">True if the key should be lowercased (default), false otherwise</param>
        /// <param name="norename">True if games should only be compared on game and file name, false if system and source are counted</param>
        /// <returns></returns>
        public void BucketBy(ItemKey bucketBy, DedupeType dedupeType, bool lower = true, bool norename = true)
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
        /// Merge an arbitrary set of item pairs based on the supplied information
        /// </summary>
        /// <param name="itemMappings">List of pairs representing the items to be merged</param>
        private List<(long, DatItem)> Deduplicate(List<(long, DatItem)> itemMappings)
        {
            // Check for null or blank roms first
            if (itemMappings == null || itemMappings.Count == 0)
                return [];

            // Create output list
            List<(long, DatItem)> output = [];

            // Then deduplicate them by checking to see if data matches previous saved roms
            int nodumpCount = 0;
            for (int f = 0; f < itemMappings.Count; f++)
            {
                long itemIndex = itemMappings[f].Item1;
                DatItem datItem = itemMappings[f].Item2;

                // If we somehow have a null item, skip
                if (datItem == null)
                    continue;

                // If we don't have a Disk, File, Media, or Rom, we skip checking for duplicates
                if (datItem is not Disk && datItem is not DatItems.Formats.File && datItem is not Media && datItem is not Rom)
                    continue;

                // If it's a nodump, add and skip
                if (datItem is Rom rom && rom.GetStringFieldValue(Models.Metadata.Rom.StatusKey).AsEnumValue<ItemStatus>() == ItemStatus.Nodump)
                {
                    output.Add((itemIndex, datItem));
                    nodumpCount++;
                    continue;
                }
                else if (datItem is Disk disk && disk.GetStringFieldValue(Models.Metadata.Disk.StatusKey).AsEnumValue<ItemStatus>() == ItemStatus.Nodump)
                {
                    output.Add((itemIndex, datItem));
                    nodumpCount++;
                    continue;
                }
                // If it's the first non-nodump rom in the list, don't touch it
                else if (output.Count == 0 || output.Count == nodumpCount)
                {
                    output.Add((itemIndex, datItem));
                    continue;
                }

                // Check if the rom is a duplicate
                DupeType dupetype = 0x00;
                long savedIndex = -1;
                DatItem saveditem = new Blank();
                int pos = -1;
                for (int i = 0; i < output.Count; i++)
                {
                    long lastIndex = output[i].Item1;
                    DatItem lastrom = output[i].Item2;

                    // Get the duplicate status
                    dupetype = datItem.GetDuplicateStatus(lastrom);

                    // If it's a duplicate, skip adding it to the output but add any missing information
                    if (dupetype != 0x00)
                    {
                        savedIndex = lastIndex;
                        saveditem = lastrom;
                        pos = i;

                        // Disks, Media, and Roms have more information to fill
                        if (datItem is Disk disk && saveditem is Disk savedDisk)
                            savedDisk.FillMissingInformation(disk);
                        else if (datItem is DatItems.Formats.File fileItem && saveditem is DatItems.Formats.File savedFile)
                            savedFile.FillMissingInformation(fileItem);
                        else if (datItem is Media media && saveditem is Media savedMedia)
                            savedMedia.FillMissingInformation(media);
                        else if (datItem is Rom romItem && saveditem is Rom savedRom)
                            savedRom.FillMissingInformation(romItem);

                        saveditem.SetFieldValue<DupeType>(DatItem.DupeTypeKey, dupetype);

                        // Get the machines associated with the items
                        var savedMachine = _machines[_itemToMachineMapping[savedIndex]];
                        var itemMachine = _machines[_itemToMachineMapping[itemIndex]];

                        // If the current system has a lower ID than the previous, set the system accordingly
                        if (datItem.GetFieldValue<Source?>(DatItem.SourceKey)?.Index < saveditem.GetFieldValue<Source?>(DatItem.SourceKey)?.Index)
                        {
                            datItem.SetFieldValue<Source?>(DatItem.SourceKey, datItem.GetFieldValue<Source?>(DatItem.SourceKey)!.Clone() as Source);
                            _machines[_itemToMachineMapping[savedIndex]] = (itemMachine.Clone() as Machine)!;
                            saveditem.SetName(datItem.GetName());
                        }

                        // If the current machine is a child of the new machine, use the new machine instead
                        if (savedMachine.GetStringFieldValue(Models.Metadata.Machine.CloneOfKey) == itemMachine.GetStringFieldValue(Models.Metadata.Machine.NameKey)
                            || savedMachine.GetStringFieldValue(Models.Metadata.Machine.RomOfKey) == itemMachine.GetStringFieldValue(Models.Metadata.Machine.NameKey))
                        {
                            _machines[_itemToMachineMapping[savedIndex]] = (itemMachine.Clone() as Machine)!;
                            saveditem.SetName(datItem.GetName());
                        }

                        break;
                    }
                }

                // If no duplicate is found, add it to the list
                if (dupetype == 0x00)
                {
                    output.Add((itemIndex, datItem));
                }
                // Otherwise, if a new rom information is found, add that
                else
                {
                    output.RemoveAt(pos);
                    output.Insert(pos, (savedIndex, saveditem));
                }
            }

            return output;
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

            if (!_itemToMachineMapping.ContainsKey(itemIndex))
                return string.Empty;

            long machineIndex = _itemToMachineMapping[itemIndex];
            if (!_machines.ContainsKey(machineIndex))
                return string.Empty;

            var machine = _machines[machineIndex];
            if (machine == null)
                return string.Empty;

            string sourceKeyPadded = datItem.GetFieldValue<Source?>(DatItem.SourceKey)?.Index.ToString().PadLeft(10, '0') + '-';
            string machineName = machine.GetStringFieldValue(Models.Metadata.Machine.NameKey) ?? "Default";

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
            Parallel.For(0, itemIndicies.Length, Globals.ParallelOptions, i =>
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
            Parallel.For(0, bucketKeys.Length, Globals.ParallelOptions, i =>
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
                    .Where(i => _items.ContainsKey(i))
                    .Select(i => (i, _items[i]))
                    .ToList();

                Sort(ref datItems, false);

                // If we're merging the roms, do so
                if (dedupeType == DedupeType.Full || (dedupeType == DedupeType.Game && bucketBy == ItemKey.Machine))
                    datItems = Deduplicate(datItems);

                _buckets[bucketKeys[i]] = datItems.Select(m => m.Item1).ToConcurrentList();
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
            Parallel.For(0, bucketKeys.Length, Globals.ParallelOptions, i =>
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
        /// Sort a list of item pairs by SourceID, Game, and Name (in order)
        /// </summary>
        /// <param name="itemMappings">List of pairs representing the items to be sorted</param>
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

        #endregion

        #region Filtering

        /// <summary>
        /// Execute all filters in a filter runner on the items in the dictionary
        /// </summary>
        /// <param name="filterRunner">Preconfigured filter runner to use</param>
        public void ExecuteFilters(FilterRunner filterRunner)
        {
            List<string> keys = [.. SortedKeys];
#if NET452_OR_GREATER || NETCOREAPP
            Parallel.ForEach(keys, Globals.ParallelOptions, key =>
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
        public void MachineDescriptionToName(bool throwOnError = false)
        {
            try
            {
                // First we want to get a mapping for all games to description
#if NET40_OR_GREATER || NETCOREAPP
                ConcurrentDictionary<string, string> mapping = CreateMachineToDescriptionMapping();
#else
                Dictionary<string, string> mapping = CreateMachineToDescriptionMapping();
#endif

                // Now we loop through every item and update accordingly
                UpdateMachineNamesFromDescriptions(mapping);
            }
            catch (Exception ex) when (!throwOnError)
            {
                logger.Warning(ex.ToString());
            }
        }

        /// <summary>
        /// Ensure that all roms are in their own game (or at least try to ensure)
        /// </summary>
        public void SetOneRomPerGame()
        {
            // For each rom, we want to update the game to be "<game name>/<rom name>"
#if NET452_OR_GREATER || NETCOREAPP
            Parallel.ForEach(SortedKeys, Globals.ParallelOptions, key =>
#elif NET40_OR_GREATER
            Parallel.ForEach(SortedKeys, key =>
#else
            foreach (var key in SortedKeys)
#endif
            {
                var items = GetDatItemsForBucket(key);
                if (items == null)
#if NET40_OR_GREATER || NETCOREAPP
                    return;
#else
                    continue;
#endif

                for (int i = 0; i < items.Length; i++)
                {
                    SetOneRomPerGame(items[i]);
                }
#if NET40_OR_GREATER || NETCOREAPP
            });
#else
            }
#endif
        }

        /// <summary>
        /// Strip the dates from the beginning of scene-style set names
        /// </summary>
        public void StripSceneDatesFromItems()
        {
            // Set the regex pattern to use
            string pattern = @"([0-9]{2}\.[0-9]{2}\.[0-9]{2}-)(.*?-.*?)";

            // Now process all of the roms
#if NET452_OR_GREATER || NETCOREAPP
            Parallel.ForEach(SortedKeys, Globals.ParallelOptions, key =>
#elif NET40_OR_GREATER
            Parallel.ForEach(SortedKeys, key =>
#else
            foreach (var key in SortedKeys)
#endif
            {
                var items = GetDatItemsForBucket(key);
                if (items == null)
#if NET40_OR_GREATER || NETCOREAPP
                    return;
#else
                    continue;
#endif

                for (int j = 0; j < items.Length; j++)
                {
                    var item = items[j];
                    var machine = GetMachineForItem(item.Item1);
                    if (machine.Item2 == null)
                        continue;

                    if (Regex.IsMatch(machine.Item2.GetStringFieldValue(Models.Metadata.Machine.NameKey)!, pattern))
                        machine.Item2.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, Regex.Replace(machine.Item2.GetStringFieldValue(Models.Metadata.Machine.NameKey)!, pattern, "$2"));

                    if (Regex.IsMatch(machine.Item2.GetStringFieldValue(Models.Metadata.Machine.DescriptionKey)!, pattern))
                        machine.Item2.SetFieldValue<string?>(Models.Metadata.Machine.DescriptionKey, Regex.Replace(machine.Item2.GetStringFieldValue(Models.Metadata.Machine.DescriptionKey)!, pattern, "$2"));

                    items[j] = item;
                }

                _buckets[key] = items.Select(i => i.Item1).ToConcurrentList();
#if NET40_OR_GREATER || NETCOREAPP
            });
#else
            }
#endif
        }

        /// <summary>
        /// Create machine to description mapping dictionary
        /// </summary>
#if NET40_OR_GREATER || NETCOREAPP
        private ConcurrentDictionary<string, string> CreateMachineToDescriptionMapping()
#else
        private Dictionary<string, string> CreateMachineToDescriptionMapping()
#endif
        {
#if NET40_OR_GREATER || NETCOREAPP
            ConcurrentDictionary<string, string> mapping = new();
#else
            Dictionary<string, string> mapping = [];
#endif
#if NET452_OR_GREATER || NETCOREAPP
            Parallel.ForEach(SortedKeys, Globals.ParallelOptions, key =>
#elif NET40_OR_GREATER
            Parallel.ForEach(SortedKeys, key =>
#else
            foreach (var key in SortedKeys)
#endif
            {
                var items = GetDatItemsForBucket(key);
                if (items == null)
#if NET40_OR_GREATER || NETCOREAPP
                    return;
#else
                    continue;
#endif

                foreach ((long, DatItem) item in items)
                {
                    // Get the current machine
                    var machine = GetMachineForItem(item.Item1);
                    if (machine.Item2 == null)
                        continue;

                    // If the key mapping doesn't exist, add it
#if NET40_OR_GREATER || NETCOREAPP
                    mapping.TryAdd(machine.Item2.GetStringFieldValue(Models.Metadata.Machine.NameKey)!,
                        machine.Item2.GetStringFieldValue(Models.Metadata.Machine.DescriptionKey)!.Replace('/', '_').Replace("\"", "''").Replace(":", " -"));
#else
                    mapping[machine.Item2.GetStringFieldValue(Models.Metadata.Machine.NameKey)!]
                        = machine.Item2.GetStringFieldValue(Models.Metadata.Machine.DescriptionKey)!.Replace('/', '_').Replace("\"", "''").Replace(":", " -");
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
            (long, DatItem)[]? items = GetDatItemsForBucket(bucketName);
            if (items == null)
                return;

            // Filter all items in the current key
            var newItems = new ConcurrentList<(long, DatItem)>();
            foreach (var item in items)
            {
                if (item.Item2.PassesFilter(filterRunner))
                    newItems.Add(item);
            }

            // Set the value in the key to the new set
            _buckets[bucketName] = newItems.Select(i => i.Item1).ToConcurrentList();
        }

        /// <summary>
        /// Set internal names to match One Rom Per Game (ORPG) logic
        /// </summary>
        /// <param name="datItem">DatItem to run logic on</param>
        private void SetOneRomPerGame((long, DatItem) datItem)
        {
            if (datItem.Item1 < 0 || datItem.Item2.GetName() == null)
                return;

            // Get the current machine
            var machine = GetMachineForItem(datItem.Item1);
            if (machine.Item2 == null)
                return;

            string[] splitname = datItem.Item2.GetName()!.Split('.');
#if NET20 || NET35
            machine.Item2.SetFieldValue<string?>(Models.Metadata.Machine.NameKey,
                machine.Item2.GetStringFieldValue(Models.Metadata.Machine.NameKey) + $"/{string.Join(".", splitname.Take(splitname.Length > 1 ? splitname.Length - 1 : 1).ToArray())}");
#else
            machine.Item2.SetFieldValue<string?>(Models.Metadata.Machine.NameKey,
                machine.Item2.GetStringFieldValue(Models.Metadata.Machine.NameKey) + $"/{string.Join(".", splitname.Take(splitname.Length > 1 ? splitname.Length - 1 : 1))}");
#endif
            datItem.Item2.SetName(Path.GetFileName(datItem.Item2.GetName()));
        }

        /// <summary>
        /// Update machine names from descriptions according to mappings
        /// </summary>
#if NET40_OR_GREATER || NETCOREAPP
        private void UpdateMachineNamesFromDescriptions(ConcurrentDictionary<string, string> mapping)
#else
        private void UpdateMachineNamesFromDescriptions(Dictionary<string, string> mapping)
#endif
        {
#if NET452_OR_GREATER || NETCOREAPP
            Parallel.ForEach(SortedKeys, Globals.ParallelOptions, key =>
#elif NET40_OR_GREATER
            Parallel.ForEach(SortedKeys, key =>
#else
            foreach (var key in SortedKeys)
#endif
            {
                var items = GetDatItemsForBucket(key);
                if (items == null)
#if NET40_OR_GREATER || NETCOREAPP
                    return;
#else
                    continue;
#endif

                ConcurrentList<(long, DatItem)> newItems = [];
                foreach ((long, DatItem) item in items)
                {
                    // Get the current machine
                    var machine = GetMachineForItem(item.Item1);
                    if (machine.Item2 == null)
                        continue;

                    // Update machine name
                    if (!string.IsNullOrEmpty(machine.Item2.GetStringFieldValue(Models.Metadata.Machine.NameKey)) && mapping.ContainsKey(machine.Item2.GetStringFieldValue(Models.Metadata.Machine.NameKey)!))
                        machine.Item2.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, mapping[machine.Item2.GetStringFieldValue(Models.Metadata.Machine.NameKey)!]);

                    // Update cloneof
                    if (!string.IsNullOrEmpty(machine.Item2.GetStringFieldValue(Models.Metadata.Machine.CloneOfKey)) && mapping.ContainsKey(machine.Item2.GetStringFieldValue(Models.Metadata.Machine.CloneOfKey)!))
                        machine.Item2.SetFieldValue<string?>(Models.Metadata.Machine.CloneOfKey, mapping[machine.Item2.GetStringFieldValue(Models.Metadata.Machine.CloneOfKey)!]);

                    // Update romof
                    if (!string.IsNullOrEmpty(machine.Item2.GetStringFieldValue(Models.Metadata.Machine.RomOfKey)) && mapping.ContainsKey(machine.Item2.GetStringFieldValue(Models.Metadata.Machine.RomOfKey)!))
                        machine.Item2.SetFieldValue<string?>(Models.Metadata.Machine.RomOfKey, mapping[machine.Item2.GetStringFieldValue(Models.Metadata.Machine.RomOfKey)!]);

                    // Update sampleof
                    if (!string.IsNullOrEmpty(machine.Item2.GetStringFieldValue(Models.Metadata.Machine.SampleOfKey)) && mapping.ContainsKey(machine.Item2.GetStringFieldValue(Models.Metadata.Machine.SampleOfKey)!))
                        machine.Item2.SetFieldValue<string?>(Models.Metadata.Machine.SampleOfKey, mapping[machine.Item2.GetStringFieldValue(Models.Metadata.Machine.SampleOfKey)!]);

                    // Add the new item to the output list
                    newItems.Add(item);
                }

                // Replace the old list of roms with the new one
                _buckets[key] = newItems.Select(i => i.Item1).ToConcurrentList();
#if NET40_OR_GREATER || NETCOREAPP
            });
#else
            }
#endif
        }

        #endregion

        #region Splitting

        /// <summary>
        /// Use romof tags to add roms to the children
        /// </summary>
        public void AddRomsFromBios()
        {
            List<string> games = [.. SortedKeys];
            foreach (string game in games)
            {
                var items = GetDatItemsForBucket(game);

                // If the game has no items in it, we want to continue
                if (items == null || items.Length == 0)
                    continue;

                // Get the machine for the first item
                var machine = GetMachineForItem(items[0].Item1);
                if (machine.Item2 == null)
                    continue;

                // Determine if the game has a parent or not
                (long, string?) parent = (-1, null);
                if (!string.IsNullOrEmpty(machine.Item2.GetStringFieldValue(Models.Metadata.Machine.RomOfKey)))
                {
                    string? romOf = machine.Item2.GetStringFieldValue(Models.Metadata.Machine.RomOfKey);
                    var romOfMachine = GetMachine(romOf);
                    parent = (romOfMachine.Item1, romOf);
                }

                // If the parent doesnt exist, we want to continue
                if (string.IsNullOrEmpty(parent.Item2))
                    continue;

                // If the parent doesn't have any items, we want to continue
                var parentItems = GetDatItemsForBucket(parent.Item2!);
                if (parentItems == null || parentItems.Length == 0)
                    continue;

                // If the parent exists and has items, we copy the items from the parent to the current game
                foreach ((long, DatItem) item in parentItems)
                {
                    DatItem datItem = (item.Item2.Clone() as DatItem)!;
                    if (!items.Where(i => i.Item2.GetName() == datItem.GetName()).Any() && !items.Any(i => i.Item2 == datItem))
                        AddItem(datItem, machine.Item1);
                }
            }
        }

        /// <summary>
        /// Use device_ref and optionally slotoption tags to add roms to the children
        /// </summary>
        /// <param name="dev">True if only child device sets are touched, false for non-device sets</param>
        /// <param name="useSlotOptions">True if slotoptions tags are used as well, false otherwise</param>
        public bool AddRomsFromDevices(bool dev, bool useSlotOptions)
        {
            bool foundnew = false;
            List<string> games = [.. SortedKeys];
            foreach (string game in games)
            {
                // Get the items for this game
                var items = GetDatItemsForBucket(game);

                // If the machine doesn't have items, we continue
                if (items == null || items.Length == 0)
                    continue;

                // Get the machine for the first item
                var machine = GetMachineForItem(items[0].Item1);
                if (machine.Item2 == null)
                    continue;

                // If the machine (is/is not) a device, we want to continue
                if (dev ^ (machine.Item2.GetBoolFieldValue(Models.Metadata.Machine.IsDeviceKey) == true))
                    continue;

                // Get all device reference names from the current machine
                List<string?> deviceReferences = items
                    .Where(i => i.Item2 is DeviceRef)
                    .Select(i => i.Item2 as DeviceRef)
                    .Select(dr => dr!.GetName())
                    .Distinct()
                    .ToList();

                // Get all slot option names from the current machine
                List<string?> slotOptions = items
                    .Where(i => i.Item2 is Slot)
                    .Select(i => i.Item2 as Slot)
                    .Where(s => s!.SlotOptionsSpecified)
                    .SelectMany(s => s!.GetFieldValue<SlotOption[]?>(Models.Metadata.Slot.SlotOptionKey)!)
                    .Select(so => so.GetStringFieldValue(Models.Metadata.SlotOption.DevNameKey))
                    .Distinct()
                    .ToList();

                // If we're checking device references
                if (deviceReferences.Any())
                {
                    // Loop through all names and check the corresponding machines
                    List<string> newDeviceReferences = [];
                    foreach (string? deviceReference in deviceReferences)
                    {
                        // If the device reference is invalid
                        if (deviceReference == null)
                            continue;

                        // If the machine doesn't exist then we continue
                        var devItems = GetDatItemsForBucket(deviceReference);
                        if (devItems == null || devItems.Length == 0)
                            continue;

                        // Add to the list of new device reference names
                        newDeviceReferences.AddRange(devItems
                            .Where(i => i.Item2 is DeviceRef)
                            .Select(i => (i.Item2 as DeviceRef)!.GetName()!));

                        // Set new machine information and add to the current machine
                        var copyFrom = GetMachineForItem(GetDatItemsForBucket(game)![0].Item1);
                        if (copyFrom.Item2 == null)
                            continue;

                        foreach ((long, DatItem) item in devItems)
                        {
                            // If the parent machine doesn't already contain this item, add it
                            if (!GetDatItemsForBucket(game)!
                                .Any(i => i.Item2.GetStringFieldValue(Models.Metadata.DatItem.TypeKey) == item.Item2.GetStringFieldValue(Models.Metadata.DatItem.TypeKey)
                                    && i.Item2.GetName() == item.Item2.GetName()))
                            {
                                // Set that we found new items
                                foundnew = true;

                                // Clone the item and then add it
                                DatItem datItem = (item.Item2.Clone() as DatItem)!;
                                AddItem(datItem, machine.Item1);
                            }
                        }
                    }

                    // Now that every device reference is accounted for, add the new list of device references, if they don't already exist
                    foreach (string deviceReference in newDeviceReferences.Distinct())
                    {
                        if (!deviceReferences.Contains(deviceReference))
                        {
                            var deviceRef = new DeviceRef();
                            deviceRef.SetName(deviceReference);
                            AddItem(deviceRef, machine.Item1);
                        }
                    }
                }

                // If we're checking slotoptions
                if (useSlotOptions && slotOptions.Any())
                {
                    // Loop through all names and check the corresponding machines
                    List<string> newSlotOptions = [];
                    foreach (string? slotOption in slotOptions)
                    {
                        // If the slot option is invalid
                        if (slotOption == null)
                            continue;

                        // If the machine doesn't exist then we continue
                        var slotItems = GetDatItemsForBucket(slotOption);
                        if (slotItems == null || slotItems.Length == 0)
                            continue;

                        // Add to the list of new slot option names
                        newSlotOptions.AddRange(slotItems
                            .Where(i => i.Item2 is Slot)
                            .Where(s => (s.Item2 as Slot)!.SlotOptionsSpecified)
                            .SelectMany(s => (s.Item2 as Slot)!.GetFieldValue<SlotOption[]?>(Models.Metadata.Slot.SlotOptionKey)!)
                            .Select(o => o.GetStringFieldValue(Models.Metadata.SlotOption.DevNameKey)!));

                        // Set new machine information and add to the current machine
                        var copyFrom = GetMachineForItem(GetDatItemsForBucket(game)![0].Item1);
                        if (copyFrom.Item2 == null)
                            continue;

                        foreach ((long, DatItem) item in slotItems)
                        {
                            // If the parent machine doesn't already contain this item, add it
                            if (!GetDatItemsForBucket(game)!
                                .Any(i => i.Item2.GetStringFieldValue(Models.Metadata.DatItem.TypeKey) == item.Item2.GetStringFieldValue(Models.Metadata.DatItem.TypeKey)
                                    && i.Item2.GetName() == item.Item2.GetName()))
                            {
                                // Set that we found new items
                                foundnew = true;

                                // Clone the item and then add it
                                DatItem datItem = (item.Item2.Clone() as DatItem)!;
                                AddItem(datItem, machine.Item1);
                            }
                        }
                    }

                    // Now that every device is accounted for, add the new list of slot options, if they don't already exist
                    foreach (string slotOption in newSlotOptions.Distinct())
                    {
                        if (!slotOptions.Contains(slotOption))
                        {
                            var slotOptionItem = new SlotOption();
                            slotOptionItem.SetFieldValue<string?>(Models.Metadata.SlotOption.DevNameKey, slotOption);

                            var slotItem = new Slot();
                            slotItem.SetFieldValue<SlotOption[]?>(Models.Metadata.Slot.SlotOptionKey, [slotOptionItem]);

                            AddItem(slotItem, machine.Item1);
                        }
                    }
                }
            }

            return foundnew;
        }

        /// <summary>
        /// Use cloneof tags to add roms to the children, setting the new romof tag in the process
        /// </summary>
        public void AddRomsFromParent()
        {
            List<string> games = [.. SortedKeys];
            foreach (string game in games)
            {
                // If the game has no items in it, we want to continue
                var items = GetDatItemsForBucket(game);
                if (items == null || items.Length == 0)
                    continue;

                // Get the machine for the first item in the list
                var machine = GetMachineForItem(items[0].Item1);
                if (machine.Item2 == null)
                    continue;

                // Determine if the game has a parent or not
                (long, string?) parent = (-1, null);
                if (!string.IsNullOrEmpty(machine.Item2.GetStringFieldValue(Models.Metadata.Machine.CloneOfKey)))
                {
                    string? cloneOf = machine.Item2.GetStringFieldValue(Models.Metadata.Machine.CloneOfKey);
                    var cloneOfMachine = GetMachine(cloneOf);
                    parent = (cloneOfMachine.Item1, cloneOf);
                }

                // If there is no parent, then we continue
                if (string.IsNullOrEmpty(parent.Item2))
                    continue;

                // If the parent doesn't have any items, we want to continue
                var parentItems = GetDatItemsForBucket(parent.Item2!);
                if (parentItems == null || parentItems.Length == 0)
                    continue;

                // If the parent exists and has items, we copy the items from the parent to the current game
                foreach (var item in parentItems)
                {
                    DatItem datItem = (DatItem)item.Item2.Clone();
                    if (!items.Where(i => i.Item2.GetName()?.ToLowerInvariant() == datItem.GetName()?.ToLowerInvariant()).Any()
                        && !items.Any(i => i.Item2 == datItem))
                    {
                        AddItem(datItem, machine.Item1);
                    }
                }

                // Get the parent machine
                var parentMachine = GetMachineForItem(GetDatItemsForBucket(parent.Item2!)![0].Item1);
                if (parentMachine.Item2 == null)
                    continue;

                // Now we want to get the parent romof tag and put it in each of the items
                items = GetDatItemsForBucket(game);
                string? romof = parentMachine.Item2.GetStringFieldValue(Models.Metadata.Machine.RomOfKey);
                foreach ((long, DatItem) item in items!)
                {
                    var itemMachine = GetMachineForItem(item.Item1);
                    if (itemMachine.Item2 == null)
                        continue;

                    itemMachine.Item2.SetFieldValue<string?>(Models.Metadata.Machine.RomOfKey, romof);
                }
            }
        }

        /// <summary>
        /// Use cloneof tags to add roms to the parents, removing the child sets in the process
        /// </summary>
        /// <param name="subfolder">True to add DatItems to subfolder of parent (not including Disk), false otherwise</param>
        /// <param name="skipDedup">True to skip checking for duplicate ROMs in parent, false otherwise</param>
        public void AddRomsFromChildren(bool subfolder, bool skipDedup)
        {
            List<string> games = [.. SortedKeys];
            foreach (string game in games)
            {
                // If the game has no items in it, we want to continue
                var items = GetDatItemsForBucket(game);
                if (items == null || items.Length == 0)
                    continue;

                // Get the machine for the first item
                var machine = GetMachineForItem(items[0].Item1);
                if (machine.Item2 == null)
                    continue;

                // Determine if the game has a parent or not
                (long, string?) parent = (-1, null);
                if (!string.IsNullOrEmpty(machine.Item2.GetStringFieldValue(Models.Metadata.Machine.CloneOfKey)))
                {
                    string? cloneOf = machine.Item2.GetStringFieldValue(Models.Metadata.Machine.CloneOfKey);
                    var cloneOfMachine = GetMachine(cloneOf);
                    parent = (cloneOfMachine.Item1, cloneOf);
                }

                // If there is no parent, then we continue
                if (string.IsNullOrEmpty(parent.Item2))
                    continue;

                items = GetDatItemsForBucket(game);
                foreach ((long, DatItem) item in items!)
                {
                    // Get the parent items and current machine name
                    var parentItems = GetDatItemsForBucket(parent.Item2!);
                    string? machineName = GetMachineForItem(item.Item1).Item2?.GetStringFieldValue(Models.Metadata.Machine.NameKey);

                    // Special disk handling
                    if (item.Item2 is Disk disk)
                    {
                        string? mergeTag = disk.GetStringFieldValue(Models.Metadata.Disk.MergeKey);

                        // If the merge tag exists and the parent already contains it, skip
                        if (mergeTag != null && parentItems!
                            .Where(i => i.Item2 is Disk)
                            .Select(i => (i.Item2 as Disk)!.GetName())
                            .Contains(mergeTag))
                        {
                            continue;
                        }

                        // If the merge tag exists but the parent doesn't contain it, add to parent
                        else if (mergeTag != null && !parentItems!
                            .Where(i => i.Item2 is Disk)
                            .Select(i => (i.Item2 as Disk)!.GetName())
                            .Contains(mergeTag))
                        {
                            _itemToMachineMapping[item.Item1] = parent.Item1;
                            _buckets[parent.Item2!].Add(disk);
                        }

                        // If there is no merge tag, add to parent
                        else if (mergeTag == null)
                        {
                            _itemToMachineMapping[item.Item1] = parent.Item1;
                            _buckets[parent.Item2!].Add(disk);
                        }
                    }

                    // Special rom handling
                    else if (item.Item2 is Rom rom)
                    {
                        // If the merge tag exists and the parent already contains it, skip
                        if (rom.GetStringFieldValue(Models.Metadata.Rom.MergeKey) != null && parentItems!
                            .Where(i => i.Item2 is Rom)
                            .Select(i => (i.Item2 as Rom)!.GetName())
                            .Contains(rom.GetStringFieldValue(Models.Metadata.Rom.MergeKey)))
                        {
                            continue;
                        }

                        // If the merge tag exists but the parent doesn't contain it, add to subfolder of parent
                        else if (rom.GetStringFieldValue(Models.Metadata.Rom.MergeKey) != null && !parentItems!
                            .Where(i => i.Item2 is Rom)
                            .Select(i => (i.Item2 as Rom)!.GetName())
                            .Contains(rom.GetStringFieldValue(Models.Metadata.Rom.MergeKey)))
                        {
                            if (subfolder)
                                rom.SetName($"{machineName}\\{rom.GetName()}");

                            _itemToMachineMapping[item.Item1] = parent.Item1;
                            _buckets[parent.Item2!].Add(rom);
                        }

                        // If the parent doesn't already contain this item, add to subfolder of parent
                        else if (!parentItems!.Contains(item) || skipDedup)
                        {
                            if (subfolder)
                                rom.SetName($"{machineName}\\{rom.GetName()}");

                            _itemToMachineMapping[item.Item1] = parent.Item1;
                            _buckets[parent.Item2!].Add(rom);
                        }
                    }

                    // All other that would be missing to subfolder of parent
                    else if (!parentItems!.Contains(item))
                    {
                        if (subfolder)
                            item.Item2.SetName($"{machineName}\\{item.Item2.GetName()}");

                        _itemToMachineMapping[item.Item1] = parent.Item1;
                        _buckets[parent.Item2!].Add(item);
                    }
                }

                // Then, remove the old game so it's not picked up by the writer
#if NET40_OR_GREATER || NETCOREAPP
                _buckets.TryRemove(game, out _);
#else
                _buckets.Remove(game);
#endif
            }
        }

        /// <summary>
        /// Remove all BIOS and device sets
        /// </summary>
        public void RemoveBiosAndDeviceSets()
        {
            List<string> games = [.. SortedKeys];
            foreach (string game in games)
            {
                var items = GetDatItemsForBucket(game);
                if (items == null || items.Length == 0)
                    continue;

                var machine = GetMachineForItem(items[0].Item1);
                if (machine.Item2 == null)
                    continue;

                if ((machine.Item2.GetBoolFieldValue(Models.Metadata.Machine.IsBiosKey) == true)
                    || (machine.Item2.GetBoolFieldValue(Models.Metadata.Machine.IsDeviceKey) == true))
                {
                    foreach (var item in items)
                    {
                        RemoveItem(item.Item1);
                    }
                }
            }
        }

        /// <summary>
        /// Use romof tags to remove bios roms from children
        /// </summary>
        /// <param name="bios">True if only child Bios sets are touched, false for non-bios sets</param>
        public void RemoveBiosRomsFromChild(bool bios)
        {
            // Loop through the romof tags
            List<string> games = [.. SortedKeys];
            foreach (string game in games)
            {
                // If the game has no items in it, we want to continue
                var items = GetDatItemsForBucket(game);
                if (items == null || items.Length == 0)
                    continue;

                // Get the machine for the item
                var machine = GetMachineForItem(items[0].Item1);
                if (machine.Item2 == null)
                    continue;

                // If the game (is/is not) a bios, we want to continue
                if (bios ^ (machine.Item2.GetBoolFieldValue(Models.Metadata.Machine.IsBiosKey) == true))
                    continue;

                // Determine if the game has a parent or not
                string? parent = null;
                if (!string.IsNullOrEmpty(machine.Item2.GetStringFieldValue(Models.Metadata.Machine.RomOfKey)))
                    parent = machine.Item2.GetStringFieldValue(Models.Metadata.Machine.RomOfKey);

                // If the parent doesnt exist, we want to continue
                if (string.IsNullOrEmpty(parent))
                    continue;

                // If the parent doesn't have any items, we want to continue
                var parentItems = GetDatItemsForBucket(parent!);
                if (parentItems == null || parentItems.Length == 0)
                    continue;

                // If the parent exists and has items, we remove the items that are in the parent from the current game
                foreach ((long, DatItem) item in parentItems)
                {
                    var matchedIndices = items.Where(i => i.Item2 == item.Item2).Select(i => i.Item1);
                    foreach (long index in matchedIndices)
                    {
                        RemoveItem(index);
                    }
                }
            }
        }

        /// <summary>
        /// Use cloneof tags to remove roms from the children
        /// </summary>
        public void RemoveRomsFromChild()
        {
            List<string> games = [.. SortedKeys];
            foreach (string game in games)
            {
                var items = GetDatItemsForBucket(game);
                if (items == null)
                    continue;

                // If the game has no items in it, we want to continue
                if (items.Length == 0)
                    continue;

                // Get the machine for the first item
                var machine = GetMachineForItem(items[0].Item1);
                if (machine.Item2 == null)
                    continue;

                // Determine if the game has a parent or not
                string? parent = null;
                if (!string.IsNullOrEmpty(machine.Item2.GetStringFieldValue(Models.Metadata.Machine.CloneOfKey)))
                    parent = machine.Item2.GetStringFieldValue(Models.Metadata.Machine.CloneOfKey);

                // If the parent doesnt exist, we want to continue
                if (string.IsNullOrEmpty(parent))
                    continue;

                // If the parent doesn't have any items, we want to continue
                var parentItems = GetDatItemsForBucket(parent!);
                if (parentItems == null || parentItems.Length == 0)
                    continue;

                // If the parent exists and has items, we remove the parent items from the current game
                foreach ((long, DatItem) item in parentItems)
                {
                    var matchedIndices = items.Where(i => i.Item2 == item.Item2).Select(i => i.Item1);
                    foreach (long index in matchedIndices)
                    {
                        RemoveItem(index);
                    }
                }

                // Now we want to get the parent romof tag and put it in each of the remaining items
                items = GetDatItemsForBucket(game);
                machine = GetMachineForItem(GetDatItemsForBucket(parent!)![0].Item1);
                if (machine.Item2 == null)
                    continue;

                string? romof = machine.Item2.GetStringFieldValue(Models.Metadata.Machine.RomOfKey);
                foreach ((long, DatItem) item in items!)
                {
                    machine = GetMachineForItem(item.Item1);
                    if (machine.Item2 == null)
                        continue;

                    machine.Item2.SetFieldValue<string?>(Models.Metadata.Machine.RomOfKey, romof);
                }
            }
        }

        /// <summary>
        /// Remove all romof and cloneof tags from all games
        /// </summary>
        public void RemoveTagsFromChild()
        {
            List<string> games = [.. SortedKeys];
            foreach (string game in games)
            {
                var items = GetDatItemsForBucket(game);
                if (items == null)
                    continue;

                foreach ((long, DatItem) item in items)
                {
                    var machine = GetMachineForItem(item.Item1);
                    if (machine.Item2 == null)
                        continue;

                    machine.Item2.SetFieldValue<string?>(Models.Metadata.Machine.CloneOfKey, null);
                    machine.Item2.SetFieldValue<string?>(Models.Metadata.Machine.RomOfKey, null);
                    machine.Item2.SetFieldValue<string?>(Models.Metadata.Machine.SampleOfKey, null);
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
