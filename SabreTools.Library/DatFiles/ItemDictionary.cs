using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SabreTools.Library.Data;
using SabreTools.Library.DatItems;
using NaturalSort;

namespace SabreTools.Library.DatFiles
{
    /// <summary>
    /// Item dictionary with statistics, bucketing, and sorting
    /// </summary>
    public class ItemDictionary : IDictionary<string, List<DatItem>>
    {
        #region Private instance variables

        /// <summary>
        /// Determine the bucketing key for all items
        /// </summary>
        private BucketedBy bucketedBy;

        /// <summary>
        /// Determine merging type for all items
        /// </summary>
        private DedupeType mergedBy;

        /// <summary>
        /// Internal dictionary for the class
        /// </summary>
        private Dictionary<string, List<DatItem>> items;

        #endregion

        #region Publically available fields

        /// <summary>
        /// Get the keys from the file dictionary
        /// </summary>
        /// <returns>List of the keys</returns>
        public ICollection<string> Keys
        {
            get { return items.Keys; }
        }

        /// <summary>
        /// Get the keys in sorted order from the file dictionary
        /// </summary>
        /// <returns>List of the keys in sorted order</returns>
        public List<string> SortedKeys
        {
            get
            {
                var keys = items.Keys.ToList();
                keys.Sort(new NaturalComparer());
                return keys;
            }
        }

        /// <summary>
        /// DatStats object for reporting
        /// </summary>
        public DatStats Statistics { get; private set; }

        #endregion

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
                Statistics.AddItem(value);
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
                    Statistics.AddItem(item);
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
        /// Remove a key from the file dictionary if it exists
        /// </summary>
        /// <param name="key">Key in the dictionary to remove</param>
        public bool Remove(string key)
        {
            // If the key doesn't exist, return
            if (!ContainsKey(key))
                return false;

            // Remove the statistics first
            foreach (DatItem item in items[key])
            {
                Statistics.RemoveItem(item);
            }

            // Remove the key from the dictionary
            return items.Remove(key);
        }

        /// <summary>
        /// Remove the first instance of a value from the file dictionary if it exists
        /// </summary>
        /// <param name="key">Key in the dictionary to remove from</param>
        /// <param name="value">Value to remove from the dictionary</param>
        public bool Remove(string key, DatItem value)
        {
            // If the key and value doesn't exist, return
            if (!Contains(key, value))
                return false;

            // Remove the statistics first
            Statistics.RemoveItem(value);

            return items[key].Remove(value);
        }

        /// <summary>
        /// Override the internal BucketedBy value
        /// </summary>
        /// <param name="newBucket"></param>
        public void SetBucketedBy(BucketedBy newBucket)
        {
            bucketedBy = newBucket;
        }

        /// <summary>
        /// Ensure the key exists in the items dictionary
        /// </summary>
        /// <param name="key">Key to ensure</param>
        private void EnsureKey(string key)
        {
            // If the key is missing from the dictionary, add it
            if (!items.ContainsKey(key))
                items.Add(key, new List<DatItem>());
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Generic constructor
        /// </summary>
        public ItemDictionary()
        {
            bucketedBy = BucketedBy.Default;
            mergedBy = DedupeType.None;
            items = new Dictionary<string, List<DatItem>>();

            Statistics = new DatStats();
        }

        #endregion

        #region Custom Functionality

        /// <summary>
        /// Take the arbitrarily bucketed Files Dictionary and convert to one bucketed by a user-defined method
        /// </summary>
        /// <param name="bucketBy">BucketedBy enum representing how to bucket the individual items</param>
        /// <param name="dedupeType">Dedupe type that should be used</param>
        /// <param name="lower">True if the key should be lowercased (default), false otherwise</param>
        /// <param name="norename">True if games should only be compared on game and file name, false if system and source are counted</param>
        public void BucketBy(BucketedBy bucketBy, DedupeType dedupeType, bool lower = true, bool norename = true)
        {
            // If we have a situation where there's no dictionary or no keys at all, we skip
            if (items == null || items.Count == 0)
                return;

            // If the sorted type isn't the same, we want to sort the dictionary accordingly
            if (this.bucketedBy != bucketBy)
            {
                Globals.Logger.User($"Organizing roms by {bucketBy}");

                // Set the sorted type
                this.bucketedBy = bucketBy;

                // Reset the merged type since this might change the merge
                this.mergedBy = DedupeType.None;

                // First do the initial sort of all of the roms inplace
                List<string> oldkeys = Keys.ToList();
                for (int k = 0; k < oldkeys.Count; k++)
                {
                    string key = oldkeys[k];

                    // Get the unsorted current list
                    List<DatItem> items = this[key];

                    // Now add each of the roms to their respective keys
                    for (int i = 0; i < items.Count; i++)
                    {
                        DatItem item = items[i];
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
                }
            }

            // If the merge type isn't the same, we want to merge the dictionary accordingly
            if (this.mergedBy != dedupeType)
            {
                Globals.Logger.User($"Deduping roms by {dedupeType}");

                // Set the sorted type
                this.mergedBy = dedupeType;

                Parallel.ForEach(Keys, Globals.ParallelOptions, key =>
                {
                    // Get the possibly unsorted list
                    List<DatItem> sortedlist = this[key];

                    // Sort the list of items to be consistent
                    DatItem.Sort(ref sortedlist, false);

                    // If we're merging the roms, do so
                    if (dedupeType == DedupeType.Full || (dedupeType == DedupeType.Game && bucketBy == BucketedBy.Game))
                        sortedlist = DatItem.Merge(sortedlist);

                    // Add the list back to the dictionary
                    Remove(key);
                    AddRange(key, sortedlist);
                });
            }
            // If the merge type is the same, we want to sort the dictionary to be consistent
            else
            {
                Parallel.ForEach(Keys, Globals.ParallelOptions, key =>
                {
                    // Get the possibly unsorted list
                    List<DatItem> sortedlist = this[key];

                    // Sort the list of items to be consistent
                    DatItem.Sort(ref sortedlist, false);
                });
            }

            // Now clean up all empty keys
            ClearEmpty();
        }

        /// <summary>
        /// Remove all items marked for removal
        /// </summary>
        public void ClearMarked()
        {
            foreach (string key in items.Keys)
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
        /// <param name="remove">True to mark matched roms for removal from the input, false otherwise (default)</param>
        /// <param name="sorted">True if the DAT is already sorted accordingly, false otherwise (default)</param>
        /// <returns>List of matched DatItem objects</returns>
        public List<DatItem> GetDuplicates(DatItem datItem, bool remove = false, bool sorted = false)
        {
            List<DatItem> output = new List<DatItem>();

            // Check for an empty rom list first
            if (Statistics.Count == 0)
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

            // If we're in removal mode, add back all roms with the proper flags
            if (remove)
            {
                Remove(key);
                AddRange(key, output);
                AddRange(key, left);
            }

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
            if (Statistics.Count == 0)
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
            Statistics.Reset();

            // If we have a blank Dat in any way, return
            if (this == null || Statistics.Count == 0)
                return;

            // Loop through and add
            Parallel.ForEach(items.Keys, Globals.ParallelOptions, key =>
            {
                List<DatItem> datItems = items[key];
                foreach (DatItem item in datItems)
                {
                    Statistics.AddItem(item);
                }
            });
        }

        /// <summary>
        /// Remove any keys that have null or empty values
        /// </summary>
        private void ClearEmpty()
        {
            foreach (string key in items.Keys)
            {
                if (items[key] == null || items[key].Count == 0)
                    items.Remove(key);
            }
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
                BucketBy(Statistics.GetBestAvailable(), DedupeType.None);

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
    }
}
