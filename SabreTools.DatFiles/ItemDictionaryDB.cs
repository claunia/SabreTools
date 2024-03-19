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

/*
 * Planning Notes:
 * 
 * In order for this in-memory "database" design to work, there need to be a few things:
 * - Feature parity with all existing item dictionary operations
 * - A way to transition between the two item dictionaries (a flag?)
 * - Helper methods that target the "database" version instead of assuming the standard dictionary
 * - Automatically add to default buckets based on... [Machine name? Item type?]
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
        /// Add a DatItem to the dictionary after validation
        /// </summary>
        /// <param name="item">Item data to validate</param>
        /// <param name="machineIndex">Index of the machine related to the item</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <returns>The index for the added item, -1 on error</returns>
        public long AddItemAndValidate(DatItem item, long machineIndex, bool statsOnly)
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
        /// Add an item, returning the insert index
        /// </summary>
        public long AddItem(DatItem item, long machineIndex)
        {
            // TODO: Add to buckets based on current sorting

            _items[_itemIndex++] = item;
            _itemToMachineMapping[_itemIndex - 1] = machineIndex;
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
        /// Get all bucket keys
        /// </summary>
        public string[] GetBucketKeys() => [.. _buckets.Keys];

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
        /// Get the machine associated with an item index
        /// </summary>
        public Machine? GetMachineForItem(long itemIndex)
        {
            if (!_itemToMachineMapping.ContainsKey(itemIndex))
                return null;

            long machineIndex = _itemToMachineMapping[itemIndex];
            if (!_machines.ContainsKey(machineIndex))
                return null;

            return _machines[machineIndex];
        }

        /// <summary>
        /// Get the items associated with a bucket name
        /// </summary>
        public DatItem[]? GetDatItemsForBucket(string bucketName, bool filter = false)
        {
            if (!_buckets.ContainsKey(bucketName))
                return null;

            var itemIds = _buckets[bucketName];

            var datItems = new List<DatItem>();
            foreach (long itemId in itemIds)
            {
                if (_items.ContainsKey(itemId) && (!filter || _items[itemId].GetBoolFieldValue(DatItem.RemoveKey) != true))
                    datItems.Add(_items[itemId]);
            }

            return [.. datItems];
        }

        /// <summary>
        /// Get the items associated with a machine index
        /// </summary>
        public DatItem[]? GetDatItemsForMachine(long machineIndex, bool filter = false)
        {
            var itemIds = _itemToMachineMapping
                .Where(mapping => mapping.Value == machineIndex)
                .Select(mapping => mapping.Key);

            var datItems = new List<DatItem>();
            foreach (long itemId in itemIds)
            {
                if (_items.ContainsKey(itemId) && (!filter || _items[itemId].GetBoolFieldValue(DatItem.RemoveKey) != true))
                    datItems.Add(_items[itemId]);
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
            if (itemMappings == null || !itemMappings.Any())
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
                string? bucketKey = GetBucketKey(i, bucketBy, lower, norename);
                EnsureBucketingKey(bucketKey);
                _buckets[bucketKey].Add(i);
#if NET40_OR_GREATER || NETCOREAPP
            });
#else
            }
#endif
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
                if (itemIndices == null || !itemIndices.Any())
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

        #region Statistics

        /// <summary>
        /// Recalculate the statistics for the Dat
        /// </summary>
        public void RecalculateStats()
        {
            // Wipe out any stats already there
            DatStatistics.ResetStatistics();

            // If there are no items
            if (_items == null || !_items.Any())
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
