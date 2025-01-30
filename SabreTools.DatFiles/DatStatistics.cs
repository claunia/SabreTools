using System.Collections.Generic;
using SabreTools.Core.Tools;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;
using SabreTools.Hashing;

namespace SabreTools.DatFiles
{
    /// <summary>
    /// Statistics wrapper for outputting
    /// </summary>
    public class DatStatistics
    {
        #region Private instance variables

        /// <summary>
        /// Number of items for each hash type
        /// </summary>
        private readonly Dictionary<HashType, long> _hashCounts = [];

        /// <summary>
        /// Number of items for each item type
        /// </summary>
        private readonly Dictionary<ItemType, long> _itemCounts = [];

        /// <summary>
        /// Number of items for each item status
        /// </summary>
        private readonly Dictionary<ItemStatus, long> _statusCounts = [];

        /// <summary>
        /// Lock for statistics calculation
        /// </summary>
        private readonly object statsLock = new();

        #endregion

        #region Fields

        /// <summary>
        /// Overall item count
        /// </summary>
        public long TotalCount { get; private set; } = 0;

        /// <summary>
        /// Number of machines
        /// </summary>
        /// <remarks>Special count only used by statistics output</remarks>
        public long GameCount { get; set; } = 0;

        /// <summary>
        /// Total uncompressed size
        /// </summary>
        public long TotalSize { get; private set; } = 0;

        /// <summary>
        /// Number of items with the remove flag
        /// </summary>
        public long RemovedCount { get; private set; } = 0;

        /// <summary>
        /// Name to display on output
        /// </summary>
        public string? DisplayName { get; set; }

        /// <summary>
        /// Total machine count to use on output
        /// </summary>
        public long MachineCount { get; set; }

        /// <summary>
        /// Determines if statistics are for a directory or not
        /// </summary>
        public readonly bool IsDirectory;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public DatStatistics()
        {
            DisplayName = null;
            MachineCount = 0;
            IsDirectory = false;
        }

        /// <summary>
        /// Constructor for aggregate data
        /// </summary>
        public DatStatistics(string? displayName, bool isDirectory)
        {
            DisplayName = displayName;
            MachineCount = 0;
            IsDirectory = isDirectory;
        }

        #endregion

        #region Accessors

        /// <summary>
        /// Add to the statistics given a DatItem
        /// </summary>
        /// <param name="item">Item to add info from</param>
        public void AddItemStatistics(DatItem item)
        {
            lock (statsLock)
            {
                // No matter what the item is, we increment the count
                TotalCount++;

                // Increment removal count
                if (item.GetBoolFieldValue(DatItem.RemoveKey) == true)
                    RemovedCount++;

                // Increment the item count for the type
                AddItemCount(item.GetStringFieldValue(Models.Metadata.DatItem.TypeKey).AsEnumValue<ItemType>());

                // Some item types require special processing
                switch (item)
                {
                    case Disk disk:
                        if (disk.GetStringFieldValue(Models.Metadata.Disk.StatusKey).AsEnumValue<ItemStatus>() != ItemStatus.Nodump)
                        {
                            AddHashCount(HashType.MD5, string.IsNullOrEmpty(disk.GetStringFieldValue(Models.Metadata.Disk.MD5Key)) ? 0 : 1);
                            AddHashCount(HashType.SHA1, string.IsNullOrEmpty(disk.GetStringFieldValue(Models.Metadata.Disk.SHA1Key)) ? 0 : 1);
                        }

                        AddStatusCount(ItemStatus.BadDump, disk.GetStringFieldValue(Models.Metadata.Disk.StatusKey).AsEnumValue<ItemStatus>() == ItemStatus.BadDump ? 1 : 0);
                        AddStatusCount(ItemStatus.Good, disk.GetStringFieldValue(Models.Metadata.Disk.StatusKey).AsEnumValue<ItemStatus>() == ItemStatus.Good ? 1 : 0);
                        AddStatusCount(ItemStatus.Nodump, disk.GetStringFieldValue(Models.Metadata.Disk.StatusKey).AsEnumValue<ItemStatus>() == ItemStatus.Nodump ? 1 : 0);
                        AddStatusCount(ItemStatus.Verified, disk.GetStringFieldValue(Models.Metadata.Disk.StatusKey).AsEnumValue<ItemStatus>() == ItemStatus.Verified ? 1 : 0);
                        break;
                    case Media media:
                        AddHashCount(HashType.MD5, string.IsNullOrEmpty(media.GetStringFieldValue(Models.Metadata.Media.MD5Key)) ? 0 : 1);
                        AddHashCount(HashType.SHA1, string.IsNullOrEmpty(media.GetStringFieldValue(Models.Metadata.Media.SHA1Key)) ? 0 : 1);
                        AddHashCount(HashType.SHA256, string.IsNullOrEmpty(media.GetStringFieldValue(Models.Metadata.Media.SHA256Key)) ? 0 : 1);
                        AddHashCount(HashType.SpamSum, string.IsNullOrEmpty(media.GetStringFieldValue(Models.Metadata.Media.SpamSumKey)) ? 0 : 1);
                        break;
                    case Rom rom:
                        if (rom.GetStringFieldValue(Models.Metadata.Rom.StatusKey).AsEnumValue<ItemStatus>() != ItemStatus.Nodump)
                        {
                            TotalSize += rom.GetInt64FieldValue(Models.Metadata.Rom.SizeKey) ?? 0;
                            AddHashCount(HashType.CRC32, string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.CRCKey)) ? 0 : 1);
                            AddHashCount(HashType.MD2, string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.MD2Key)) ? 0 : 1);
                            AddHashCount(HashType.MD4, string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.MD4Key)) ? 0 : 1);
                            AddHashCount(HashType.MD5, string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.MD5Key)) ? 0 : 1);
                            AddHashCount(HashType.SHA1, string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.SHA1Key)) ? 0 : 1);
                            AddHashCount(HashType.SHA256, string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.SHA256Key)) ? 0 : 1);
                            AddHashCount(HashType.SHA384, string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.SHA384Key)) ? 0 : 1);
                            AddHashCount(HashType.SHA512, string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.SHA512Key)) ? 0 : 1);
                            AddHashCount(HashType.SpamSum, string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.SpamSumKey)) ? 0 : 1);
                        }

                        AddStatusCount(ItemStatus.BadDump, rom.GetStringFieldValue(Models.Metadata.Rom.StatusKey).AsEnumValue<ItemStatus>() == ItemStatus.BadDump ? 1 : 0);
                        AddStatusCount(ItemStatus.Good, rom.GetStringFieldValue(Models.Metadata.Rom.StatusKey).AsEnumValue<ItemStatus>() == ItemStatus.Good ? 1 : 0);
                        AddStatusCount(ItemStatus.Nodump, rom.GetStringFieldValue(Models.Metadata.Rom.StatusKey).AsEnumValue<ItemStatus>() == ItemStatus.Nodump ? 1 : 0);
                        AddStatusCount(ItemStatus.Verified, rom.GetStringFieldValue(Models.Metadata.Rom.StatusKey).AsEnumValue<ItemStatus>() == ItemStatus.Verified ? 1 : 0);
                        break;
                }
            }
        }

        /// <summary>
        /// Add statistics from another DatStatistics object
        /// </summary>
        /// <param name="stats">DatStatistics object to add from</param>
        public void AddStatistics(DatStatistics stats)
        {
            TotalCount += stats.TotalCount;

            // Loop through and add stats for all items
            foreach (var itemCountKvp in stats._itemCounts)
            {
                AddItemCount(itemCountKvp.Key, itemCountKvp.Value);
            }

            GameCount += stats.GameCount;

            TotalSize += stats.TotalSize;

            // Individual hash counts
            foreach (var hashCountKvp in stats._hashCounts)
            {
                AddHashCount(hashCountKvp.Key, hashCountKvp.Value);
            }

            // Individual status counts
            foreach (var statusCountKvp in stats._statusCounts)
            {
                AddStatusCount(statusCountKvp.Key, statusCountKvp.Value);
            }

            RemovedCount += stats.RemovedCount;
        }

        /// <summary>
        /// Get the item count for a given hash type, defaulting to 0 if it does not exist
        /// </summary>
        /// <param name="hashType">Hash type to retrieve</param>
        /// <returns>The number of items with that hash, if it exists</returns>
        public long GetHashCount(HashType hashType)
        {
            lock (_hashCounts)
            {
                if (!_hashCounts.ContainsKey(hashType))
                    return 0;

                return _hashCounts[hashType];
            }
        }

        /// <summary>
        /// Get the item count for a given item type, defaulting to 0 if it does not exist
        /// </summary>
        /// <param name="itemType">Item type to retrieve</param>
        /// <returns>The number of items of that type, if it exists</returns>
        public long GetItemCount(ItemType itemType)
        {
            lock (_itemCounts)
            {
                if (!_itemCounts.ContainsKey(itemType))
                    return 0;

                return _itemCounts[itemType];
            }
        }

        /// <summary>
        /// Get the item count for a given item status, defaulting to 0 if it does not exist
        /// </summary>
        /// <param name="itemStatus">Item status to retrieve</param>
        /// <returns>The number of items of that type, if it exists</returns>
        public long GetStatusCount(ItemStatus itemStatus)
        {
            lock (_statusCounts)
            {
                if (!_statusCounts.ContainsKey(itemStatus))
                    return 0;

                return _statusCounts[itemStatus];
            }
        }

        /// <summary>
        /// Increment the hash count for a given hash type
        /// </summary>
        /// <param name="hashType">Hash type to increment</param>
        /// <param name="interval">Amount to increment by, defaults to 1</param>
        private void AddHashCount(HashType hashType, long interval = 1)
        {
            lock (_hashCounts)
            {
                if (!_hashCounts.ContainsKey(hashType))
                    _hashCounts[hashType] = 0;

                _hashCounts[hashType] += interval;
                if (_hashCounts[hashType] < 0)
                    _hashCounts[hashType] = 0;
            }
        }

        /// <summary>
        /// Increment the item count for a given item type
        /// </summary>
        /// <param name="itemType">Item type to increment</param>
        /// <param name="interval">Amount to increment by, defaults to 1</param>
        private void AddItemCount(ItemType itemType, long interval = 1)
        {
            lock (_itemCounts)
            {
                if (!_itemCounts.ContainsKey(itemType))
                    _itemCounts[itemType] = 0;

                _itemCounts[itemType] += interval;
                if (_itemCounts[itemType] < 0)
                    _itemCounts[itemType] = 0;
            }
        }

        /// <summary>
        /// Increment the item count for a given item status
        /// </summary>
        /// <param name="itemStatus">Item type to increment</param>
        /// <param name="interval">Amount to increment by, defaults to 1</param>
        private void AddStatusCount(ItemStatus itemStatus, long interval = 1)
        {
            lock (_statusCounts)
            {
                if (!_statusCounts.ContainsKey(itemStatus))
                    _statusCounts[itemStatus] = 0;

                _statusCounts[itemStatus] += interval;
                if (_statusCounts[itemStatus] < 0)
                    _statusCounts[itemStatus] = 0;
            }
        }

        /// <summary>
        /// Remove from the statistics given a DatItem
        /// </summary>
        /// <param name="item">Item to remove info for</param>
        public void RemoveItemStatistics(DatItem item)
        {
            // If we have a null item, we can't do anything
            if (item == null)
                return;

            lock (statsLock)
            {
                // No matter what the item is, we decrease the count
                TotalCount--;

                // Decrement removal count
                if (item.GetBoolFieldValue(DatItem.RemoveKey) == true)
                    RemovedCount--;

                // Decrement the item count for the type
                RemoveItemCount(item.GetStringFieldValue(Models.Metadata.DatItem.TypeKey).AsEnumValue<ItemType>());

                // Some item types require special processing
                switch (item)
                {
                    case Disk disk:
                        if (disk.GetStringFieldValue(Models.Metadata.Disk.StatusKey).AsEnumValue<ItemStatus>() != ItemStatus.Nodump)
                        {
                            RemoveHashCount(HashType.MD5, string.IsNullOrEmpty(disk.GetStringFieldValue(Models.Metadata.Disk.MD5Key)) ? 0 : 1);
                            RemoveHashCount(HashType.SHA1, string.IsNullOrEmpty(disk.GetStringFieldValue(Models.Metadata.Disk.SHA1Key)) ? 0 : 1);
                        }

                        RemoveStatusCount(ItemStatus.BadDump, disk.GetStringFieldValue(Models.Metadata.Disk.StatusKey).AsEnumValue<ItemStatus>() == ItemStatus.BadDump ? 1 : 0);
                        RemoveStatusCount(ItemStatus.Good, disk.GetStringFieldValue(Models.Metadata.Disk.StatusKey).AsEnumValue<ItemStatus>() == ItemStatus.Good ? 1 : 0);
                        RemoveStatusCount(ItemStatus.Nodump, disk.GetStringFieldValue(Models.Metadata.Disk.StatusKey).AsEnumValue<ItemStatus>() == ItemStatus.Nodump ? 1 : 0);
                        RemoveStatusCount(ItemStatus.Verified, disk.GetStringFieldValue(Models.Metadata.Disk.StatusKey).AsEnumValue<ItemStatus>() == ItemStatus.Verified ? 1 : 0);
                        break;
                    case Media media:
                        RemoveHashCount(HashType.MD5, string.IsNullOrEmpty(media.GetStringFieldValue(Models.Metadata.Media.MD5Key)) ? 0 : 1);
                        RemoveHashCount(HashType.SHA1, string.IsNullOrEmpty(media.GetStringFieldValue(Models.Metadata.Media.SHA1Key)) ? 0 : 1);
                        RemoveHashCount(HashType.SHA256, string.IsNullOrEmpty(media.GetStringFieldValue(Models.Metadata.Media.SHA256Key)) ? 0 : 1);
                        RemoveHashCount(HashType.SpamSum, string.IsNullOrEmpty(media.GetStringFieldValue(Models.Metadata.Media.SpamSumKey)) ? 0 : 1);
                        break;
                    case Rom rom:
                        if (rom.GetStringFieldValue(Models.Metadata.Rom.StatusKey).AsEnumValue<ItemStatus>() != ItemStatus.Nodump)
                        {
                            TotalSize -= rom.GetInt64FieldValue(Models.Metadata.Rom.SizeKey) ?? 0;
                            RemoveHashCount(HashType.CRC32, string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.CRCKey)) ? 0 : 1);
                            RemoveHashCount(HashType.MD2, string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.MD2Key)) ? 0 : 1);
                            RemoveHashCount(HashType.MD4, string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.MD4Key)) ? 0 : 1);
                            RemoveHashCount(HashType.MD5, string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.MD5Key)) ? 0 : 1);
                            RemoveHashCount(HashType.SHA1, string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.SHA1Key)) ? 0 : 1);
                            RemoveHashCount(HashType.SHA256, string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.SHA256Key)) ? 0 : 1);
                            RemoveHashCount(HashType.SHA384, string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.SHA384Key)) ? 0 : 1);
                            RemoveHashCount(HashType.SHA512, string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.SHA512Key)) ? 0 : 1);
                            RemoveHashCount(HashType.SpamSum, string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.SpamSumKey)) ? 0 : 1);
                        }

                        RemoveStatusCount(ItemStatus.BadDump, rom.GetStringFieldValue(Models.Metadata.Rom.StatusKey).AsEnumValue<ItemStatus>() == ItemStatus.BadDump ? 1 : 0);
                        RemoveStatusCount(ItemStatus.Good, rom.GetStringFieldValue(Models.Metadata.Rom.StatusKey).AsEnumValue<ItemStatus>() == ItemStatus.Good ? 1 : 0);
                        RemoveStatusCount(ItemStatus.Nodump, rom.GetStringFieldValue(Models.Metadata.Rom.StatusKey).AsEnumValue<ItemStatus>() == ItemStatus.Nodump ? 1 : 0);
                        RemoveStatusCount(ItemStatus.Verified, rom.GetStringFieldValue(Models.Metadata.Rom.StatusKey).AsEnumValue<ItemStatus>() == ItemStatus.Verified ? 1 : 0);
                        break;
                }
            }
        }

        /// <summary>
        /// Reset all statistics
        /// </summary>
        public void ResetStatistics()
        {
            TotalCount = 0;
            _itemCounts.Clear();
            GameCount = 0;
            TotalSize = 0;
            _hashCounts.Clear();
            _statusCounts.Clear();
            RemovedCount = 0;
        }

        /// <summary>
        /// Decrement the hash count for a given hash type
        /// </summary>
        /// <param name="hashType">Hash type to increment</param>
        /// <param name="interval">Amount to increment by, defaults to 1</param>
        private void RemoveHashCount(HashType hashType, long interval = 1)
        {
            lock (_hashCounts)
            {
                if (!_hashCounts.ContainsKey(hashType))
                    return;

                _hashCounts[hashType] -= interval;
                if (_hashCounts[hashType] < 0)
                    _hashCounts[hashType] = 0;
            }
        }

        /// <summary>
        /// Decrement the item count for a given item type
        /// </summary>
        /// <param name="itemType">Item type to decrement</param>
        /// <param name="interval">Amount to increment by, defaults to 1</param>
        private void RemoveItemCount(ItemType itemType, long interval = 1)
        {
            lock (_itemCounts)
            {
                if (!_itemCounts.ContainsKey(itemType))
                    return;

                _itemCounts[itemType] -= interval;
                if (_itemCounts[itemType] < 0)
                    _itemCounts[itemType] = 0;
            }
        }

        /// <summary>
        /// Decrement the item count for a given item status
        /// </summary>
        /// <param name="itemStatus">Item type to decrement</param>
        /// <param name="interval">Amount to increment by, defaults to 1</param>
        private void RemoveStatusCount(ItemStatus itemStatus, long interval = 1)
        {
            lock (_statusCounts)
            {
                if (!_statusCounts.ContainsKey(itemStatus))
                    return;

                _statusCounts[itemStatus] -= interval;
                if (_statusCounts[itemStatus] < 0)
                    _statusCounts[itemStatus] = 0;
            }
        }

        #endregion
    }
}
