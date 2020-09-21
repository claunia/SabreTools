using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Serialization;

using SabreTools.Library.Data;
using SabreTools.Library.DatItems;
using SabreTools.Library.IO;
using SabreTools.Library.Reports;
using NaturalSort;
using Newtonsoft.Json;

namespace SabreTools.Library.DatFiles
{
    /// <summary>
    /// Item dictionary with statistics, bucketing, and sorting
    /// </summary>
    [JsonObject("items"), XmlRoot("items")]
    public class ItemDictionary : IDictionary<string, List<DatItem>>
    {
        #region Private instance variables

        /// <summary>
        /// Determine the bucketing key for all items
        /// </summary>
        private Field bucketedBy;

        /// <summary>
        /// Determine merging type for all items
        /// </summary>
        private DedupeType mergedBy;

        /// <summary>
        /// Internal dictionary for the class
        /// </summary>
        private ConcurrentDictionary<string, List<DatItem>> items;

        /// <summary>
        /// Lock for statistics calculation
        /// </summary>
        private object statsLock = new object();

        #endregion

        #region Publically available fields

        #region Keys

        /// <summary>
        /// Get the keys from the file dictionary
        /// </summary>
        /// <returns>List of the keys</returns>
        [JsonIgnore, XmlIgnore]
        public ICollection<string> Keys
        {
            get { return items.Keys; }
        }

        /// <summary>
        /// Get the keys in sorted order from the file dictionary
        /// </summary>
        /// <returns>List of the keys in sorted order</returns>
        [JsonIgnore, XmlIgnore]
        public List<string> SortedKeys
        {
            get
            {
                var keys = items.Keys.ToList();
                keys.Sort(new NaturalComparer());
                return keys;
            }
        }

        #endregion

        #region Statistics

        /// <summary>
        /// Overall item count
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public long TotalCount { get; private set; } = 0;

        /// <summary>
        /// Number of Adjuster items
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public long AdjusterCount { get; private set; } = 0;

        /// <summary>
        /// Number of Analog items
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public long AnalogCount { get; private set; } = 0;

        /// <summary>
        /// Number of Archive items
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public long ArchiveCount { get; private set; } = 0;

        /// <summary>
        /// Number of BiosSet items
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public long BiosSetCount { get; private set; } = 0;

        /// <summary>
        /// Number of Chip items
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public long ChipCount { get; private set; } = 0;

        /// <summary>
        /// Number of top-level Condition items
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public long ConditionCount { get; private set; } = 0;

        /// <summary>
        /// Number of Configuration items
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public long ConfigurationCount { get; private set; } = 0;

        /// <summary>
        /// Number of DataArea items
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public long DataAreaCount { get; private set; } = 0;

        /// <summary>
        /// Number of Device items
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public long DeviceCount { get; private set; } = 0;

        /// <summary>
        /// Number of Device Reference items
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public long DeviceReferenceCount { get; private set; } = 0;

        /// <summary>
        /// Number of DIP Switch items
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public long DipSwitchCount { get; private set; } = 0;

        /// <summary>
        /// Number of Disk items
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public long DiskCount { get; private set; } = 0;

        /// <summary>
        /// Number of DiskArea items
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public long DiskAreaCount { get; private set; } = 0;

        /// <summary>
        /// Number of Display items
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public long DisplayCount { get; private set; } = 0;

        /// <summary>
        /// Number of Driver items
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public long DriverCount { get; private set; } = 0;

        /// <summary>
        /// Number of Feature items
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public long FeatureCount { get; private set; } = 0;

        /// <summary>
        /// Number of Info items
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public long InfoCount { get; private set; } = 0;

        /// <summary>
        /// Number of Input items
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public long InputCount { get; private set; } = 0;

        /// <summary>
        /// Number of Media items
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public long MediaCount { get; private set; } = 0;

        /// <summary>
        /// Number of Part items
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public long PartCount { get; private set; } = 0;

        /// <summary>
        /// Number of PartFeature items
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public long PartFeatureCount { get; private set; } = 0;

        /// <summary>
        /// Number of Port items
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public long PortCount { get; private set; } = 0;

        /// <summary>
        /// Number of RamOption items
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public long RamOptionCount { get; private set; } = 0;

        /// <summary>
        /// Number of Release items
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public long ReleaseCount { get; private set; } = 0;

        /// <summary>
        /// Number of Rom items
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public long RomCount { get; private set; } = 0;

        /// <summary>
        /// Number of Sample items
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public long SampleCount { get; private set; } = 0;

        /// <summary>
        /// Number of SharedFeature items
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public long SharedFeatureCount { get; private set; } = 0;

        /// <summary>
        /// Number of Slot items
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public long SlotCount { get; private set; } = 0;

        /// <summary>
        /// Number of SoftwareList items
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public long SoftwareListCount { get; private set; } = 0;

        /// <summary>
        /// Number of Sound items
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public long SoundCount { get; private set; } = 0;

        /// <summary>
        /// Number of machines
        /// </summary>
        /// <remarks>Special count only used by statistics output</remarks>
        [JsonIgnore, XmlIgnore]
        public long GameCount { get; private set; } = 0;

        /// <summary>
        /// Total uncompressed size
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public long TotalSize { get; private set; } = 0;

        /// <summary>
        /// Number of items with a CRC hash
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public long CRCCount { get; private set; } = 0;

        /// <summary>
        /// Number of items with an MD5 hash
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public long MD5Count { get; private set; } = 0;

#if NET_FRAMEWORK
        /// <summary>
        /// Number of items with a RIPEMD160 hash
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public long RIPEMD160Count { get; private set; } = 0;
#endif

        /// <summary>
        /// Number of items with a SHA-1 hash
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public long SHA1Count { get; private set; } = 0;

        /// <summary>
        /// Number of items with a SHA-256 hash
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public long SHA256Count { get; private set; } = 0;

        /// <summary>
        /// Number of items with a SHA-384 hash
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public long SHA384Count { get; private set; } = 0;

        /// <summary>
        /// Number of items with a SHA-512 hash
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public long SHA512Count { get; private set; } = 0;

        /// <summary>
        /// Number of items with a SpamSum fuzzy hash
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public long SpamSumCount { get; private set; } = 0;

        /// <summary>
        /// Number of items with the baddump status
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public long BaddumpCount { get; private set; } = 0;

        /// <summary>
        /// Number of items with the good status
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public long GoodCount { get; private set; } = 0;

        /// <summary>
        /// Number of items with the nodump status
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public long NodumpCount { get; private set; } = 0;

        /// <summary>
        /// Number of items with the remove flag
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public long RemovedCount { get; private set; } = 0;

        /// <summary>
        /// Number of items with the verified status
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public long VerifiedCount { get; private set; } = 0;

        #endregion

        #endregion

        #region Instance methods

        #region Accessors

        /// <summary>
        /// Passthrough to access the file dictionary
        /// </summary>
        /// <param name="key">Key in the dictionary to reference</param>
        public List<DatItem> this[string key]
        {
            get
            {
                // Explicit lock for some weird corner cases
                lock (key)
                {
                    // Ensure the key exists
                    EnsureKey(key);

                    // Now return the value
                    return items[key];
                }
            }
            set
            {
                Remove(key);
                AddRange(key, value);
            }
        }

        /// <summary>
        /// Add a value to the file dictionary
        /// </summary>
        /// <param name="key">Key in the dictionary to add to</param>
        /// <param name="value">Value to add to the dictionary</param>
        public void Add(string key, DatItem value)
        {
            // Explicit lock for some weird corner cases
            lock (key)
            {
                // Ensure the key exists
                EnsureKey(key);

                // If item is null, don't add it
                if (value == null)
                    return;

                // Now add the value
                items[key].Add(value);

                // Now update the statistics
                AddItemStatistics(value);
            }
        }

        /// <summary>
        /// Add a range of values to the file dictionary
        /// </summary>
        /// <param name="key">Key in the dictionary to add to</param>
        /// <param name="value">Value to add to the dictionary</param>
        public void Add(string key, List<DatItem> value)
        {
            AddRange(key, value);
        }

        /// <summary>
        /// Add a range of values to the file dictionary
        /// </summary>
        /// <param name="key">Key in the dictionary to add to</param>
        /// <param name="value">Value to add to the dictionary</param>
        public void AddRange(string key, List<DatItem> value)
        {
            // Explicit lock for some weird corner cases
            lock (key)
            {
                // Ensure the key exists
                EnsureKey(key);

                // Now add the value
                items[key].AddRange(value);

                // Now update the statistics
                foreach (DatItem item in value)
                {
                    AddItemStatistics(item);
                }
            }
        }

        /// <summary>
        /// Get if the file dictionary contains the key
        /// </summary>
        /// <param name="key">Key in the dictionary to check</param>
        /// <returns>True if the key exists, false otherwise</returns>
        public bool ContainsKey(string key)
        {
            // If the key is null, we return false since keys can't be null
            if (key == null)
                return false;

            // Explicit lock for some weird corner cases
            lock (key)
            {
                return items.ContainsKey(key);
            }
        }

        /// <summary>
        /// Get if the file dictionary contains the key and value
        /// </summary>
        /// <param name="key">Key in the dictionary to check</param>
        /// <param name="value">Value in the dictionary to check</param>
        /// <returns>True if the key exists, false otherwise</returns>
        public bool Contains(string key, DatItem value)
        {
            // If the key is null, we return false since keys can't be null
            if (key == null)
                return false;

            // Explicit lock for some weird corner cases
            lock (key)
            {
                if (items.ContainsKey(key))
                    return items[key].Contains(value);
            }

            return false;
        }

        /// <summary>
        /// Get a list of filtered items for a given key
        /// </summary>
        /// <param name="key">Key in the dictionary to retrieve</param>
        public List<DatItem> FilteredItems(string key)
        {
            lock (key)
            {
                // Get the list, if possible
                List<DatItem> fi = items[key];
                if (fi == null)
                    return new List<DatItem>();

                // Filter the list
                fi = fi.Where(i => i != null)
                    .Where(i => !i.Remove)
                    .Where(i => i.Machine?.Name != null)
                    .ToList();

                // Return the list
                return fi;
            }
        }

        /// <summary>
        /// Remove a key from the file dictionary if it exists
        /// </summary>
        /// <param name="key">Key in the dictionary to remove</param>
        public bool Remove(string key)
        {
            // Explicit lock for some weird corner cases
            lock (key)
            {
                // If the key doesn't exist, return
                if (!ContainsKey(key))
                    return false;

                // Remove the statistics first
                foreach (DatItem item in items[key])
                {
                    RemoveItemStatistics(item);
                }

                // Remove the key from the dictionary
                return items.TryRemove(key, out _);
            }
        }

        /// <summary>
        /// Remove the first instance of a value from the file dictionary if it exists
        /// </summary>
        /// <param name="key">Key in the dictionary to remove from</param>
        /// <param name="value">Value to remove from the dictionary</param>
        public bool Remove(string key, DatItem value)
        {
            // Explicit lock for some weird corner cases
            lock (key)
            {
                // If the key and value doesn't exist, return
                if (!Contains(key, value))
                    return false;

                // Remove the statistics first
                RemoveItemStatistics(value);

                return items[key].Remove(value);
            }
        }

        /// <summary>
        /// Reset a key from the file dictionary if it exists
        /// </summary>
        /// <param name="key">Key in the dictionary to reset</param>
        public bool Reset(string key)
        {
            // If the key doesn't exist, return
            if (!ContainsKey(key))
                return false;

            // Remove the statistics first
            foreach (DatItem item in items[key])
            {
                RemoveItemStatistics(item);
            }

            // Remove the key from the dictionary
            items[key] = new List<DatItem>();
            return true;
        }

        /// <summary>
        /// Override the internal Field value
        /// </summary>
        /// <param name="newBucket"></param>
        public void SetBucketedBy(Field newBucket)
        {
            bucketedBy = newBucket;
        }

        /// <summary>
        /// Add to the statistics given a DatItem
        /// </summary>
        /// <param name="item">Item to add info from</param>
        private void AddItemStatistics(DatItem item)
        {
            lock (statsLock)
            {
                // No matter what the item is, we increment the count
                TotalCount++;

                // Increment removal count
                if (item.Remove)
                    RemovedCount++;

                // Now we do different things for each item type
                switch (item.ItemType)
                {
                    case ItemType.Adjuster:
                        AdjusterCount++;
                        break;
                    case ItemType.Analog:
                        AnalogCount++;
                        break;
                    case ItemType.Archive:
                        ArchiveCount++;
                        break;
                    case ItemType.BiosSet:
                        BiosSetCount++;
                        break;
                    case ItemType.Chip:
                        ChipCount++;
                        break;
                    case ItemType.Condition:
                        ConditionCount++;
                        break;
                    case ItemType.Configuration:
                        ConfigurationCount++;
                        break;
                    case ItemType.DataArea:
                        DataAreaCount++;
                        break;
                    case ItemType.Device:
                        DeviceCount++;
                        break;
                    case ItemType.DeviceReference:
                        DeviceReferenceCount++;
                        break;
                    case ItemType.DipSwitch:
                        DipSwitchCount++;
                        break;
                    case ItemType.Disk:
                        DiskCount++;
                        if ((item as Disk).ItemStatus != ItemStatus.Nodump)
                        {
                            MD5Count += (string.IsNullOrWhiteSpace((item as Disk).MD5) ? 0 : 1);
                            SHA1Count += (string.IsNullOrWhiteSpace((item as Disk).SHA1) ? 0 : 1);
                        }

                        BaddumpCount += ((item as Disk).ItemStatus == ItemStatus.BadDump ? 1 : 0);
                        GoodCount += ((item as Disk).ItemStatus == ItemStatus.Good ? 1 : 0);
                        NodumpCount += ((item as Disk).ItemStatus == ItemStatus.Nodump ? 1 : 0);
                        VerifiedCount += ((item as Disk).ItemStatus == ItemStatus.Verified ? 1 : 0);
                        break;
                    case ItemType.DiskArea:
                        DiskAreaCount++;
                        break;
                    case ItemType.Display:
                        DisplayCount++;
                        break;
                    case ItemType.Driver:
                        DriverCount++;
                        break;
                    case ItemType.Feature:
                        FeatureCount++;
                        break;
                    case ItemType.Info:
                        InfoCount++;
                        break;
                    case ItemType.Input:
                        InputCount++;
                        break;
                    case ItemType.Media:
                        MediaCount++;
                        MD5Count += (string.IsNullOrWhiteSpace((item as Media).MD5) ? 0 : 1);
                        SHA1Count += (string.IsNullOrWhiteSpace((item as Media).SHA1) ? 0 : 1);
                        SHA256Count += (string.IsNullOrWhiteSpace((item as Media).SHA256) ? 0 : 1);
                        SpamSumCount += (string.IsNullOrWhiteSpace((item as Media).SpamSum) ? 0 : 1);
                        break;
                    case ItemType.Part:
                        PartCount++;
                        break;
                    case ItemType.PartFeature:
                        PartFeatureCount++;
                        break;
                    case ItemType.Port:
                        PortCount++;
                        break;
                    case ItemType.RamOption:
                        RamOptionCount++;
                        break;
                    case ItemType.Release:
                        ReleaseCount++;
                        break;
                    case ItemType.Rom:
                        RomCount++;
                        if ((item as Rom).ItemStatus != ItemStatus.Nodump)
                        {
                            TotalSize += (item as Rom).Size ?? 0;
                            CRCCount += (string.IsNullOrWhiteSpace((item as Rom).CRC) ? 0 : 1);
                            MD5Count += (string.IsNullOrWhiteSpace((item as Rom).MD5) ? 0 : 1);
#if NET_FRAMEWORK
                            RIPEMD160Count += (string.IsNullOrWhiteSpace((item as Rom).RIPEMD160) ? 0 : 1);
#endif
                            SHA1Count += (string.IsNullOrWhiteSpace((item as Rom).SHA1) ? 0 : 1);
                            SHA256Count += (string.IsNullOrWhiteSpace((item as Rom).SHA256) ? 0 : 1);
                            SHA384Count += (string.IsNullOrWhiteSpace((item as Rom).SHA384) ? 0 : 1);
                            SHA512Count += (string.IsNullOrWhiteSpace((item as Rom).SHA512) ? 0 : 1);
                            SpamSumCount += (string.IsNullOrWhiteSpace((item as Rom).SpamSum) ? 0 : 1);
                        }

                        BaddumpCount += ((item as Rom).ItemStatus == ItemStatus.BadDump ? 1 : 0);
                        GoodCount += ((item as Rom).ItemStatus == ItemStatus.Good ? 1 : 0);
                        NodumpCount += ((item as Rom).ItemStatus == ItemStatus.Nodump ? 1 : 0);
                        VerifiedCount += ((item as Rom).ItemStatus == ItemStatus.Verified ? 1 : 0);
                        break;
                    case ItemType.Sample:
                        SampleCount++;
                        break;
                    case ItemType.SharedFeature:
                        SharedFeatureCount++;
                        break;
                    case ItemType.Slot:
                        SlotCount++;
                        break;
                    case ItemType.SoftwareList:
                        SoftwareListCount++;
                        break;
                    case ItemType.Sound:
                        SoundCount++;
                        break;
                }
            }
        }

        /// <summary>
        /// Add statistics from another DatStats object
        /// </summary>
        /// <param name="stats">DatStats object to add from</param>
        private void AddStatistics(ItemDictionary stats)
        {
            TotalCount += stats.Count;

            ArchiveCount += stats.ArchiveCount;
            BiosSetCount += stats.BiosSetCount;
            ChipCount += stats.ChipCount;
            DiskCount += stats.DiskCount;
            MediaCount += stats.MediaCount;
            ReleaseCount += stats.ReleaseCount;
            RomCount += stats.RomCount;
            SampleCount += stats.SampleCount;

            GameCount += stats.GameCount;

            TotalSize += stats.TotalSize;

            // Individual hash counts
            CRCCount += stats.CRCCount;
            MD5Count += stats.MD5Count;
#if NET_FRAMEWORK
            RIPEMD160Count += stats.RIPEMD160Count;
#endif
            SHA1Count += stats.SHA1Count;
            SHA256Count += stats.SHA256Count;
            SHA384Count += stats.SHA384Count;
            SHA512Count += stats.SHA512Count;
            SpamSumCount += stats.SpamSumCount;

            // Individual status counts
            BaddumpCount += stats.BaddumpCount;
            GoodCount += stats.GoodCount;
            NodumpCount += stats.NodumpCount;
            RemovedCount += stats.RemovedCount;
            VerifiedCount += stats.VerifiedCount;
        }

        /// <summary>
        /// Ensure the key exists in the items dictionary
        /// </summary>
        /// <param name="key">Key to ensure</param>
        private void EnsureKey(string key)
        {
            // If the key is missing from the dictionary, add it
            if (!items.ContainsKey(key))
                items.TryAdd(key, new List<DatItem>());
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
                if (item.Remove)
                    RemovedCount--;

                // Now we do different things for each item type
                switch (item.ItemType)
                {
                    case ItemType.Adjuster:
                        AdjusterCount--;
                        break;
                    case ItemType.Analog:
                        AnalogCount--;
                        break;
                    case ItemType.Archive:
                        ArchiveCount--;
                        break;
                    case ItemType.BiosSet:
                        BiosSetCount--;
                        break;
                    case ItemType.Chip:
                        ChipCount--;
                        break;
                    case ItemType.Condition:
                        ConditionCount--;
                        break;
                    case ItemType.Configuration:
                        ConfigurationCount--;
                        break;
                    case ItemType.DataArea:
                        DataAreaCount--;
                        break;
                    case ItemType.Device:
                        DeviceCount--;
                        break;
                    case ItemType.DeviceReference:
                        DeviceReferenceCount--;
                        break;
                    case ItemType.DipSwitch:
                        DipSwitchCount--;
                        break;
                    case ItemType.Disk:
                        DiskCount--;
                        if ((item as Disk).ItemStatus != ItemStatus.Nodump)
                        {
                            MD5Count -= (string.IsNullOrWhiteSpace((item as Disk).MD5) ? 0 : 1);
                            SHA1Count -= (string.IsNullOrWhiteSpace((item as Disk).SHA1) ? 0 : 1);
                        }

                        BaddumpCount -= ((item as Disk).ItemStatus == ItemStatus.BadDump ? 1 : 0);
                        GoodCount -= ((item as Disk).ItemStatus == ItemStatus.Good ? 1 : 0);
                        NodumpCount -= ((item as Disk).ItemStatus == ItemStatus.Nodump ? 1 : 0);
                        VerifiedCount -= ((item as Disk).ItemStatus == ItemStatus.Verified ? 1 : 0);
                        break;
                    case ItemType.DiskArea:
                        DiskAreaCount--;
                        break;
                    case ItemType.Display:
                        DisplayCount--;
                        break;
                    case ItemType.Driver:
                        DriverCount--;
                        break;
                    case ItemType.Feature:
                        FeatureCount--;
                        break;
                    case ItemType.Info:
                        InfoCount--;
                        break;
                    case ItemType.Input:
                        InputCount--;
                        break;
                    case ItemType.Media:
                        MediaCount--;
                        MD5Count -= (string.IsNullOrWhiteSpace((item as Media).MD5) ? 0 : 1);
                        SHA1Count -= (string.IsNullOrWhiteSpace((item as Media).SHA1) ? 0 : 1);
                        SHA256Count -= (string.IsNullOrWhiteSpace((item as Media).SHA256) ? 0 : 1);
                        break;
                    case ItemType.Part:
                        PartCount--;
                        break;
                    case ItemType.PartFeature:
                        PartFeatureCount--;
                        break;
                    case ItemType.Port:
                        PortCount--;
                        break;
                    case ItemType.RamOption:
                        RamOptionCount--;
                        break;
                    case ItemType.Release:
                        ReleaseCount--;
                        break;
                    case ItemType.Rom:
                        RomCount--;
                        if ((item as Rom).ItemStatus != ItemStatus.Nodump)
                        {
                            TotalSize -= (item as Rom).Size ?? 0;
                            CRCCount -= (string.IsNullOrWhiteSpace((item as Rom).CRC) ? 0 : 1);
                            MD5Count -= (string.IsNullOrWhiteSpace((item as Rom).MD5) ? 0 : 1);
#if NET_FRAMEWORK
                        RIPEMD160Count -= (string.IsNullOrWhiteSpace((item as Rom).RIPEMD160) ? 0 : 1);
#endif
                            SHA1Count -= (string.IsNullOrWhiteSpace((item as Rom).SHA1) ? 0 : 1);
                            SHA256Count -= (string.IsNullOrWhiteSpace((item as Rom).SHA256) ? 0 : 1);
                            SHA384Count -= (string.IsNullOrWhiteSpace((item as Rom).SHA384) ? 0 : 1);
                            SHA512Count -= (string.IsNullOrWhiteSpace((item as Rom).SHA512) ? 0 : 1);
                        }

                        BaddumpCount -= ((item as Rom).ItemStatus == ItemStatus.BadDump ? 1 : 0);
                        GoodCount -= ((item as Rom).ItemStatus == ItemStatus.Good ? 1 : 0);
                        NodumpCount -= ((item as Rom).ItemStatus == ItemStatus.Nodump ? 1 : 0);
                        VerifiedCount -= ((item as Rom).ItemStatus == ItemStatus.Verified ? 1 : 0);
                        break;
                    case ItemType.Sample:
                        SampleCount--;
                        break;
                    case ItemType.SharedFeature:
                        SharedFeatureCount--;
                        break;
                    case ItemType.Slot:
                        SlotCount--;
                        break;
                    case ItemType.SoftwareList:
                        SoftwareListCount--;
                        break;
                    case ItemType.Sound:
                        SoundCount--;
                        break;
                }
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Generic constructor
        /// </summary>
        public ItemDictionary()
        {
            bucketedBy = Field.NULL;
            mergedBy = DedupeType.None;
            items = new ConcurrentDictionary<string, List<DatItem>>();
        }

        #endregion

        #region Custom Functionality

        /// <summary>
        /// Take the arbitrarily bucketed Files Dictionary and convert to one bucketed by a user-defined method
        /// </summary>
        /// <param name="bucketBy">Field enum representing how to bucket the individual items</param>
        /// <param name="dedupeType">Dedupe type that should be used</param>
        /// <param name="lower">True if the key should be lowercased (default), false otherwise</param>
        /// <param name="norename">True if games should only be compared on game and file name, false if system and source are counted</param>
        public void BucketBy(Field bucketBy, DedupeType dedupeType, bool lower = true, bool norename = true)
        {
            // If we have a situation where there's no dictionary or no keys at all, we skip
            if (items == null || items.Count == 0)
                return;

            // If the sorted type isn't the same, we want to sort the dictionary accordingly
            if (bucketedBy != bucketBy)
            {
                Globals.Logger.User($"Organizing roms by {bucketBy}");

                // Set the sorted type
                bucketedBy = bucketBy;

                // Reset the merged type since this might change the merge
                mergedBy = DedupeType.None;

                // First do the initial sort of all of the roms inplace
                List<string> oldkeys = Keys.ToList();
                Parallel.For(0, oldkeys.Count, Globals.ParallelOptions, k =>
                {
                    string key = oldkeys[k];

                    // Now add each of the roms to their respective keys
                    for (int i = 0; i < this[key].Count; i++)
                    {
                        DatItem item = this[key][i];
                        if (item == null)
                            continue;

                        // We want to get the key most appropriate for the given sorting type
                        string newkey = item.GetKey(bucketBy, lower, norename);

                        // If the key is different, move the item to the new key
                        if (newkey != key)
                        {
                            Add(newkey, item);
                            Remove(key, item);
                            i--; // This make sure that the pointer stays on the correct since one was removed
                        }
                    }

                    // If the key is now empty, remove it
                    if (this[key].Count == 0)
                        Remove(key);
                });
            }

            // If the merge type isn't the same, we want to merge the dictionary accordingly
            if (mergedBy != dedupeType)
            {
                Globals.Logger.User($"Deduping roms by {dedupeType}");

                // Set the sorted type
                mergedBy = dedupeType;

                List<string> keys = Keys.ToList();
                Parallel.ForEach(keys, Globals.ParallelOptions, key =>
                {
                    // Get the possibly unsorted list
                    List<DatItem> sortedlist = this[key].ToList();

                    // Sort the list of items to be consistent
                    DatItem.Sort(ref sortedlist, false);

                    // If we're merging the roms, do so
                    if (dedupeType == DedupeType.Full || (dedupeType == DedupeType.Game && bucketBy == Field.Machine_Name))
                        sortedlist = DatItem.Merge(sortedlist);

                    // Add the list back to the dictionary
                    Reset(key);
                    AddRange(key, sortedlist);
                });
            }
            // If the merge type is the same, we want to sort the dictionary to be consistent
            else
            {
                List<string> keys = Keys.ToList();
                Parallel.ForEach(keys, Globals.ParallelOptions, key =>
                {
                    // Get the possibly unsorted list
                    List<DatItem> sortedlist = this[key];

                    // Sort the list of items to be consistent
                    DatItem.Sort(ref sortedlist, false);
                });
            }
        }

        /// <summary>
        /// Remove any keys that have null or empty values
        /// </summary>
        public void ClearEmpty()
        {
            var keys = items.Keys.Where(k => k != null).ToList();
            foreach (string key in keys)
            {
                // If the key doesn't exist, skip
                if (!items.ContainsKey(key))
                    continue;

                // If the value is null, remove
                else if (items[key] == null)
                    items.TryRemove(key, out _);

                // If there are no non-blank items, remove
                else if (items[key].Count(i => i != null && i.ItemType != ItemType.Blank) == 0)
                    items.TryRemove(key, out _);
            }
        }

        /// <summary>
        /// Remove all items marked for removal
        /// </summary>
        public void ClearMarked()
        {
            var keys = items.Keys.ToList();
            foreach (string key in keys)
            {
                List<DatItem> oldItemList = items[key];
                List<DatItem> newItemList = oldItemList.Where(i => !i.Remove).ToList();

                Remove(key);
                AddRange(key, newItemList);
            }
        }

        /// <summary>
        /// List all duplicates found in a DAT based on a DatItem
        /// </summary>
        /// <param name="datItem">Item to try to match</param>
        /// <param name="sorted">True if the DAT is already sorted accordingly, false otherwise (default)</param>
        /// <returns>List of matched DatItem objects</returns>
        public List<DatItem> GetDuplicates(DatItem datItem, bool sorted = false)
        {
            List<DatItem> output = new List<DatItem>();

            // Check for an empty rom list first
            if (TotalCount == 0)
                return output;

            // We want to get the proper key for the DatItem
            string key = SortAndGetKey(datItem, sorted);

            // If the key doesn't exist, return the empty list
            if (!ContainsKey(key))
                return output;

            // Try to find duplicates
            List<DatItem> roms = this[key];
            List<DatItem> left = new List<DatItem>();
            for (int i = 0; i < roms.Count; i++)
            {
                DatItem other = roms[i];
                if (other.Remove)
                    continue;

                if (datItem.Equals(other))
                {
                    other.Remove = true;
                    output.Add(other);
                }
                else
                {
                    left.Add(other);
                }
            }

            // Add back all roms with the proper flags
            Remove(key);
            AddRange(key, output);
            AddRange(key, left);

            return output;
        }

        /// <summary>
        /// Check if a DAT contains the given DatItem
        /// </summary>
        /// <param name="datItem">Item to try to match</param>
        /// <param name="sorted">True if the DAT is already sorted accordingly, false otherwise (default)</param>
        /// <returns>True if it contains the rom, false otherwise</returns>
        public bool HasDuplicates(DatItem datItem, bool sorted = false)
        {
            // Check for an empty rom list first
            if (TotalCount == 0)
                return false;

            // We want to get the proper key for the DatItem
            string key = SortAndGetKey(datItem, sorted);

            // If the key doesn't exist, return the empty list
            if (!ContainsKey(key))
                return false;

            // Try to find duplicates
            List<DatItem> roms = this[key];
            return roms.Any(r => datItem.Equals(r));
        }

        /// <summary>
        /// Recalculate the statistics for the Dat
        /// </summary>
        public void RecalculateStats()
        {
            // Wipe out any stats already there
            ResetStatistics();

            // If we have a blank Dat in any way, return
            if (items == null)
                return;

            // Loop through and add
            foreach (string key in items.Keys)
            {
                List<DatItem> datItems = items[key];
                foreach (DatItem item in datItems)
                {
                    AddItemStatistics(item);
                }
            }
        }

        /// <summary>
        /// Get the highest-order Field value that represents the statistics
        /// </summary>
        private Field GetBestAvailable()
        {
            // If all items are supposed to have a SHA-512, we bucket by that
            if (DiskCount + MediaCount + RomCount - NodumpCount == SHA512Count)
                return Field.DatItem_SHA512;

            // If all items are supposed to have a SHA-384, we bucket by that
            else if (DiskCount + MediaCount + RomCount - NodumpCount == SHA384Count)
                return Field.DatItem_SHA384;

            // If all items are supposed to have a SHA-256, we bucket by that
            else if (DiskCount + MediaCount + RomCount - NodumpCount == SHA256Count)
                return Field.DatItem_SHA256;

            // If all items are supposed to have a SHA-1, we bucket by that
            else if (DiskCount + MediaCount + RomCount - NodumpCount == SHA1Count)
                return Field.DatItem_SHA1;

#if NET_FRAMEWORK
            // If all items are supposed to have a RIPEMD160, we bucket by that
            else if (DiskCount + MediaCount + RomCount - NodumpCount == RIPEMD160Count)
                return Field.DatItem_RIPEMD160;
#endif

            // If all items are supposed to have a MD5, we bucket by that
            else if (DiskCount + MediaCount + RomCount - NodumpCount == MD5Count)
                return Field.DatItem_MD5;

            // Otherwise, we bucket by CRC
            else
                return Field.DatItem_CRC;
        }

        /// <summary>
        /// Reset all statistics
        /// </summary>
        private void ResetStatistics()
        {
            TotalCount = 0;

            ArchiveCount = 0;
            BiosSetCount = 0;
            ChipCount = 0;
            DiskCount = 0;
            MediaCount = 0;
            ReleaseCount = 0;
            RomCount = 0;
            SampleCount = 0;

            GameCount = 0;

            TotalSize = 0;

            CRCCount = 0;
            MD5Count = 0;
#if NET_FRAMEWORK
            RIPEMD160Count = 0;
#endif
            SHA1Count = 0;
            SHA256Count = 0;
            SHA384Count = 0;
            SHA512Count = 0;
            SpamSumCount = 0;

            BaddumpCount = 0;
            GoodCount = 0;
            NodumpCount = 0;
            RemovedCount = 0;
            VerifiedCount = 0;
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
            return datItem.GetKey(bucketedBy);
        }

        #endregion

        #region IDictionary Implementations

        public ICollection<List<DatItem>> Values => ((IDictionary<string, List<DatItem>>)items).Values;

        public int Count => ((ICollection<KeyValuePair<string, List<DatItem>>>)items).Count;

        public bool IsReadOnly => ((ICollection<KeyValuePair<string, List<DatItem>>>)items).IsReadOnly;

        public bool TryGetValue(string key, out List<DatItem> value)
        {
            return ((IDictionary<string, List<DatItem>>)items).TryGetValue(key, out value);
        }

        public void Add(KeyValuePair<string, List<DatItem>> item)
        {
            ((ICollection<KeyValuePair<string, List<DatItem>>>)items).Add(item);
        }

        public void Clear()
        {
            ((ICollection<KeyValuePair<string, List<DatItem>>>)items).Clear();
        }

        public bool Contains(KeyValuePair<string, List<DatItem>> item)
        {
            return ((ICollection<KeyValuePair<string, List<DatItem>>>)items).Contains(item);
        }

        public void CopyTo(KeyValuePair<string, List<DatItem>>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<string, List<DatItem>>>)items).CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<string, List<DatItem>> item)
        {
            return ((ICollection<KeyValuePair<string, List<DatItem>>>)items).Remove(item);
        }

        public IEnumerator<KeyValuePair<string, List<DatItem>>> GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<string, List<DatItem>>>)items).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)items).GetEnumerator();
        }

        #endregion

        #endregion // Instance methods

        #region Static methods

        #region Writing

        /// <summary>
        /// Output the stats for a list of input dats as files in a human-readable format
        /// </summary>
        /// <param name="inputs">List of input files and folders</param>
        /// <param name="reportName">Name of the output file</param>
        /// <param name="single">True if single DAT stats are output, false otherwise</param>
        /// <param name="baddumpCol">True if baddumps should be included in output, false otherwise</param>
        /// <param name="nodumpCol">True if nodumps should be included in output, false otherwise</param>
        /// <param name="statDatFormat" > Set the statistics output format to use</param>
        public static void OutputStats(
            List<string> inputs,
            string reportName,
            string outDir,
            bool single,
            bool baddumpCol,
            bool nodumpCol,
            StatReportFormat statDatFormat)
        {
            // If there's no output format, set the default
            if (statDatFormat == StatReportFormat.None)
                statDatFormat = StatReportFormat.Textfile;

            // Get the proper output file name
            if (string.IsNullOrWhiteSpace(reportName))
                reportName = "report";

            // Get the proper output directory name
            outDir = DirectoryExtensions.Ensure(outDir);

            // Get the dictionary of desired output report names
            Dictionary<StatReportFormat, string> outputs = CreateOutStatsNames(outDir, statDatFormat, reportName);

            // Make sure we have all files and then order them
            List<ParentablePath> files = DirectoryExtensions.GetFilesOnly(inputs);
            files = files
                .OrderBy(i => Path.GetDirectoryName(i.CurrentPath))
                .ThenBy(i => Path.GetFileName(i.CurrentPath))
                .ToList();

            // Get all of the writers that we need
            List<BaseReport> reports = outputs.Select(kvp => BaseReport.Create(kvp.Key, kvp.Value, baddumpCol, nodumpCol)).ToList();

            // Write the header, if any
            reports.ForEach(report => report.WriteHeader());

            // Init all total variables
            ItemDictionary totalStats = new ItemDictionary();

            // Init directory-level variables
            string lastdir = null;
            string basepath = null;
            ItemDictionary dirStats = new ItemDictionary();

            // Now process each of the input files
            foreach (ParentablePath file in files)
            {
                // Get the directory for the current file
                string thisdir = Path.GetDirectoryName(file.CurrentPath);
                basepath = Path.GetDirectoryName(Path.GetDirectoryName(file.CurrentPath));

                // If we don't have the first file and the directory has changed, show the previous directory stats and reset
                if (lastdir != null && thisdir != lastdir)
                {
                    // Output separator if needed
                    reports.ForEach(report => report.WriteMidSeparator());

                    DatFile lastdirdat = DatFile.Create();

                    reports.ForEach(report => report.ReplaceStatistics($"DIR: {WebUtility.HtmlEncode(lastdir)}", dirStats.GameCount, dirStats));
                    reports.ForEach(report => report.Write());

                    // Write the mid-footer, if any
                    reports.ForEach(report => report.WriteFooterSeparator());

                    // Write the header, if any
                    reports.ForEach(report => report.WriteMidHeader());

                    // Reset the directory stats
                    dirStats.ResetStatistics();
                }

                Globals.Logger.Verbose($"Beginning stat collection for '{file.CurrentPath}'", false);
                List<string> games = new List<string>();
                DatFile datdata = DatFile.CreateAndParse(file.CurrentPath);
                datdata.Items.BucketBy(Field.Machine_Name, DedupeType.None, norename: true);

                // Output single DAT stats (if asked)
                Globals.Logger.User($"Adding stats for file '{file.CurrentPath}'\n", false);
                if (single)
                {
                    reports.ForEach(report => report.ReplaceStatistics(datdata.Header.FileName, datdata.Items.Keys.Count, datdata.Items));
                    reports.ForEach(report => report.Write());
                }

                // Add single DAT stats to dir
                dirStats.AddStatistics(datdata.Items);
                dirStats.GameCount += datdata.Items.Keys.Count();

                // Add single DAT stats to totals
                totalStats.AddStatistics(datdata.Items);
                totalStats.GameCount += datdata.Items.Keys.Count();

                // Make sure to assign the new directory
                lastdir = thisdir;
            }

            // Output the directory stats one last time
            reports.ForEach(report => report.WriteMidSeparator());

            if (single)
            {
                reports.ForEach(report => report.ReplaceStatistics($"DIR: {WebUtility.HtmlEncode(lastdir)}", dirStats.GameCount, dirStats));
                reports.ForEach(report => report.Write());
            }

            // Write the mid-footer, if any
            reports.ForEach(report => report.WriteFooterSeparator());

            // Write the header, if any
            reports.ForEach(report => report.WriteMidHeader());

            // Reset the directory stats
            dirStats.ResetStatistics();

            // Output total DAT stats
            reports.ForEach(report => report.ReplaceStatistics("DIR: All DATs", totalStats.GameCount, totalStats));
            reports.ForEach(report => report.Write());

            // Output footer if needed
            reports.ForEach(report => report.WriteFooter());

            Globals.Logger.User(@"
Please check the log folder if the stats scrolled offscreen", false);
        }

        /// <summary>
        /// Get the proper extension for the stat output format
        /// </summary>
        /// <param name="outDir">Output path to use</param>
        /// <param name="statDatFormat">StatDatFormat to get the extension for</param>
        /// <param name="reportName">Name of the input file to use</param>
        /// <returns>Dictionary of output formats mapped to file names</returns>
        private static Dictionary<StatReportFormat, string> CreateOutStatsNames(string outDir, StatReportFormat statDatFormat, string reportName, bool overwrite = true)
        {
            Dictionary<StatReportFormat, string> output = new Dictionary<StatReportFormat, string>();

            // First try to create the output directory if we need to
            if (!Directory.Exists(outDir))
                Directory.CreateDirectory(outDir);

            // Double check the outDir for the end delim
            if (!outDir.EndsWith(Path.DirectorySeparatorChar.ToString()))
                outDir += Path.DirectorySeparatorChar;

            // For each output format, get the appropriate stream writer
            output.Add(StatReportFormat.None, CreateOutStatsNamesHelper(outDir, ".null", reportName, overwrite));

            if (statDatFormat.HasFlag(StatReportFormat.Textfile))
                output.Add(StatReportFormat.Textfile, CreateOutStatsNamesHelper(outDir, ".txt", reportName, overwrite));

            if (statDatFormat.HasFlag(StatReportFormat.CSV))
                output.Add(StatReportFormat.CSV, CreateOutStatsNamesHelper(outDir, ".csv", reportName, overwrite));

            if (statDatFormat.HasFlag(StatReportFormat.HTML))
                output.Add(StatReportFormat.HTML, CreateOutStatsNamesHelper(outDir, ".html", reportName, overwrite));

            if (statDatFormat.HasFlag(StatReportFormat.SSV))
                output.Add(StatReportFormat.SSV, CreateOutStatsNamesHelper(outDir, ".ssv", reportName, overwrite));

            if (statDatFormat.HasFlag(StatReportFormat.TSV))
                output.Add(StatReportFormat.TSV, CreateOutStatsNamesHelper(outDir, ".tsv", reportName, overwrite));

            return output;
        }

        /// <summary>
        /// Help generating the outstats name
        /// </summary>
        /// <param name="outDir">Output directory</param>
        /// <param name="extension">Extension to use for the file</param>
        /// <param name="reportName">Name of the input file to use</param>
        /// <param name="overwrite">True if we ignore existing files, false otherwise</param>
        /// <returns>String containing the new filename</returns>
        private static string CreateOutStatsNamesHelper(string outDir, string extension, string reportName, bool overwrite)
        {
            string outfile = outDir + reportName + extension;
            outfile = outfile.Replace($"{Path.DirectorySeparatorChar}{Path.DirectorySeparatorChar}", Path.DirectorySeparatorChar.ToString());

            if (!overwrite)
            {
                int i = 1;
                while (File.Exists(outfile))
                {
                    outfile = $"{outDir}{reportName}_{i}{extension}";
                    outfile = outfile.Replace($"{Path.DirectorySeparatorChar}{Path.DirectorySeparatorChar}", Path.DirectorySeparatorChar.ToString());
                    i++;
                }
            }

            return outfile;
        }

        #endregion

        #endregion // Static methods
    }
}
