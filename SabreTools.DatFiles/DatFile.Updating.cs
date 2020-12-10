using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using SabreTools.Core;
using SabreTools.DatItems;
using SabreTools.IO;
using SabreTools.Logging;

// This file represents all methods related to converting and updating DatFiles
namespace SabreTools.DatFiles
{
    public abstract partial class DatFile
    {
        /// <summary>
        /// Replace item values from the base set represented by the current DAT
        /// </summary>
        /// <param name="intDat">DatFile to replace the values in</param>
        /// <param name="updateFields">List of Fields representing what should be updated</param>
        /// <param name="onlySame">True if descriptions should only be replaced if the game name is the same, false otherwise</param>
        public void BaseReplace(DatFile intDat, List<Field> updateFields, bool onlySame)
        {
            logger.User($"Replacing items in '{intDat.Header.FileName}' from the base DAT");

            // If we are matching based on DatItem fields of any sort
            if (updateFields.Intersect(DatItem.DatItemFields).Any())
            {
                // For comparison's sake, we want to use CRC as the base bucketing
                Items.BucketBy(Field.DatItem_CRC, DedupeType.Full);
                intDat.Items.BucketBy(Field.DatItem_CRC, DedupeType.None);

                // Then we do a hashwise comparison against the base DAT
                Parallel.ForEach(intDat.Items.Keys, Globals.ParallelOptions, key =>
                {
                    List<DatItem> datItems = intDat.Items[key];
                    List<DatItem> newDatItems = new List<DatItem>();
                    foreach (DatItem datItem in datItems)
                    {
                        List<DatItem> dupes = Items.GetDuplicates(datItem, sorted: true);
                        DatItem newDatItem = datItem.Clone() as DatItem;

                        // Replace fields from the first duplicate, if we have one
                        if (dupes.Count > 0)
                            newDatItem.ReplaceFields(dupes.First(), updateFields);

                        newDatItems.Add(newDatItem);
                    }

                    // Now add the new list to the key
                    intDat.Items.Remove(key);
                    intDat.Items.AddRange(key, newDatItems);
                });
            }

            // If we are matching based on Machine fields of any sort
            if (updateFields.Intersect(DatItem.MachineFields).Any())
            {
                // For comparison's sake, we want to use Machine Name as the base bucketing
                Items.BucketBy(Field.Machine_Name, DedupeType.Full);
                intDat.Items.BucketBy(Field.Machine_Name, DedupeType.None);

                // Then we do a namewise comparison against the base DAT
                Parallel.ForEach(intDat.Items.Keys, Globals.ParallelOptions, key =>
                {
                    List<DatItem> datItems = intDat.Items[key];
                    List<DatItem> newDatItems = new List<DatItem>();
                    foreach (DatItem datItem in datItems)
                    {
                        DatItem newDatItem = datItem.Clone() as DatItem;
                        if (Items.ContainsKey(key) && Items[key].Count() > 0)
                            newDatItem.Machine.ReplaceFields(Items[key][0].Machine, updateFields, onlySame);

                        newDatItems.Add(newDatItem);
                    }

                    // Now add the new list to the key
                    intDat.Items.Remove(key);
                    intDat.Items.AddRange(key, newDatItems);
                });
            }
        }

        /// <summary>
        /// Output diffs against a base set represented by the current DAT
        /// </summary>
        /// <param name="intDat">DatFile to replace the values in</param>
        /// <param name="useGames">True to diff using games, false to use hashes</param>
        public void DiffAgainst(DatFile intDat, bool useGames)
        {
            // For comparison's sake, we want to use a base ordering
            if (useGames)
                Items.BucketBy(Field.Machine_Name, DedupeType.None);
            else
                Items.BucketBy(Field.DatItem_CRC, DedupeType.None);

            logger.User($"Comparing '{intDat.Header.FileName}' to base DAT");

            // For comparison's sake, we want to a the base bucketing
            if (useGames)
                intDat.Items.BucketBy(Field.Machine_Name, DedupeType.None);
            else
                intDat.Items.BucketBy(Field.DatItem_CRC, DedupeType.Full);

            // Then we compare against the base DAT
            List<string> keys = intDat.Items.Keys.ToList();
            Parallel.ForEach(keys, Globals.ParallelOptions, key =>
            {
                // Game Against uses game names
                if (useGames)
                {
                    // If the base DAT doesn't contain the key, keep it
                    if (!Items.ContainsKey(key))
                        return;

                    // If the number of items is different, then keep it
                    if (Items[key].Count != intDat.Items[key].Count)
                        return;

                    // Otherwise, compare by name and hash the remaining files
                    bool exactMatch = true;
                    foreach (DatItem item in intDat.Items[key])
                    {
                        // TODO: Make this granular to name as well
                        if (!Items[key].Contains(item))
                        {
                            exactMatch = false;
                            break;
                        }
                    }

                    // If we have an exact match, remove the game
                    if (exactMatch)
                        intDat.Items.Remove(key);
                }

                // Standard Against uses hashes
                else
                {
                    List<DatItem> datItems = intDat.Items[key];
                    List<DatItem> keepDatItems = new List<DatItem>();
                    foreach (DatItem datItem in datItems)
                    {
                        if (!Items.HasDuplicates(datItem, true))
                            keepDatItems.Add(datItem);
                    }

                    // Now add the new list to the key
                    intDat.Items.Remove(key);
                    intDat.Items.AddRange(key, keepDatItems);
                }
            });
        }

        /// <summary>
        /// Output cascading diffs
        /// </summary>
        /// <param name="datHeaders">Dat headers used optionally</param>
        /// <returns>List of DatFiles representing the individually indexed items</returns>
        public List<DatFile> DiffCascade(List<DatHeader> datHeaders)
        {
            // Create a list of DatData objects representing output files
            List<DatFile> outDats = new List<DatFile>();

            // Ensure the current DatFile is sorted optimally
            Items.BucketBy(Field.DatItem_CRC, DedupeType.None);

            // Loop through each of the inputs and get or create a new DatData object
            InternalStopwatch watch = new InternalStopwatch("Initializing and filling all output DATs");

            // Create the DatFiles from the set of headers
            DatFile[] outDatsArray = new DatFile[datHeaders.Count];
            Parallel.For(0, datHeaders.Count, Globals.ParallelOptions, j =>
            {
                DatFile diffData = Create(datHeaders[j]);
                diffData.Items = new ItemDictionary();
                FillWithSourceIndex(diffData, j);
                outDatsArray[j] = diffData;
            });

            outDats = outDatsArray.ToList();
            watch.Stop();

            return outDats;
        }

        /// <summary>
        /// Output duplicate item diff
        /// </summary>
        /// <param name="inputs">List of inputs to write out from</param>
        public DatFile DiffDuplicates(List<string> inputs)
        {
            List<ParentablePath> paths = inputs.Select(i => new ParentablePath(i)).ToList();
            return DiffDuplicates(paths);
        }

        /// <summary>
        /// Output duplicate item diff
        /// </summary>
        /// <param name="inputs">List of inputs to write out from</param>
        public DatFile DiffDuplicates(List<ParentablePath> inputs)
        {
            InternalStopwatch watch = new InternalStopwatch("Initializing duplicate DAT");

            // Fill in any information not in the base DAT
            if (string.IsNullOrWhiteSpace(Header.FileName))
                Header.FileName = "All DATs";

            if (string.IsNullOrWhiteSpace(Header.Name))
                Header.Name = "All DATs";

            if (string.IsNullOrWhiteSpace(Header.Description))
                Header.Description = "All DATs";

            string post = " (Duplicates)";
            DatFile dupeData = Create(Header);
            dupeData.Header.FileName += post;
            dupeData.Header.Name += post;
            dupeData.Header.Description += post;
            dupeData.Items = new ItemDictionary();

            watch.Stop();

            // Now, loop through the dictionary and populate the correct DATs
            watch.Start("Populating duplicate DAT");

            Parallel.ForEach(Items.Keys, Globals.ParallelOptions, key =>
            {
                List<DatItem> items = DatItem.Merge(Items[key]);

                // If the rom list is empty or null, just skip it
                if (items == null || items.Count == 0)
                    return;

                // Loop through and add the items correctly
                foreach (DatItem item in items)
                {
                    if (item.DupeType.HasFlag(DupeType.External))
                    {
                        DatItem newrom = item.Clone() as DatItem;
                        newrom.Machine.Name += $" ({Path.GetFileNameWithoutExtension(inputs[item.Source.Index].CurrentPath)})";

                        dupeData.Items.Add(key, newrom);
                    }
                }
            });

            watch.Stop();

            return dupeData;
        }

        /// <summary>
        /// Output non-cascading diffs
        /// </summary>
        /// <param name="inputs">List of inputs to write out from</param>
        public List<DatFile> DiffIndividuals(List<string> inputs)
        {
            List<ParentablePath> paths = inputs.Select(i => new ParentablePath(i)).ToList();
            return DiffIndividuals(paths);
        }

        /// <summary>
        /// Output non-cascading diffs
        /// </summary>
        /// <param name="inputs">List of inputs to write out from</param>
        public List<DatFile> DiffIndividuals(List<ParentablePath> inputs)
        {
            InternalStopwatch watch = new InternalStopwatch("Initializing all individual DATs");

            // Fill in any information not in the base DAT
            if (string.IsNullOrWhiteSpace(Header.FileName))
                Header.FileName = "All DATs";

            if (string.IsNullOrWhiteSpace(Header.Name))
                Header.Name = "All DATs";

            if (string.IsNullOrWhiteSpace(Header.Description))
                Header.Description = "All DATs";

            // Loop through each of the inputs and get or create a new DatData object
            DatFile[] outDatsArray = new DatFile[inputs.Count];

            Parallel.For(0, inputs.Count, Globals.ParallelOptions, j =>
            {
                string innerpost = $" ({j} - {inputs[j].GetNormalizedFileName(true)} Only)";
                DatFile diffData = Create(Header);
                diffData.Header.FileName += innerpost;
                diffData.Header.Name += innerpost;
                diffData.Header.Description += innerpost;
                diffData.Items = new ItemDictionary();
                outDatsArray[j] = diffData;
            });

            // Create a list of DatData objects representing individual output files
            List<DatFile> outDats = outDatsArray.ToList();

            watch.Stop();

            // Now, loop through the dictionary and populate the correct DATs
            watch.Start("Populating all individual DATs");

            Parallel.ForEach(Items.Keys, Globals.ParallelOptions, key =>
            {
                List<DatItem> items = DatItem.Merge(Items[key]);

                // If the rom list is empty or null, just skip it
                if (items == null || items.Count == 0)
                    return;

                // Loop through and add the items correctly
                foreach (DatItem item in items)
                {
                    if (item.DupeType.HasFlag(DupeType.Internal) || item.DupeType == 0x00)
                        outDats[item.Source.Index].Items.Add(key, item);
                }
            });

            watch.Stop();

            return outDats.ToList();
        }

        /// <summary>
        /// Output non-duplicate item diff
        /// </summary>
        /// <param name="inputs">List of inputs to write out from</param>
        public DatFile DiffNoDuplicates(List<string> inputs)
        {
            List<ParentablePath> paths = inputs.Select(i => new ParentablePath(i)).ToList();
            return DiffNoDuplicates(paths);
        }

        /// <summary>
        /// Output non-duplicate item diff
        /// </summary>
        /// <param name="inputs">List of inputs to write out from</param>
        public DatFile DiffNoDuplicates(List<ParentablePath> inputs)
        {
            InternalStopwatch watch = new InternalStopwatch("Initializing no duplicate DAT");

            // Fill in any information not in the base DAT
            if (string.IsNullOrWhiteSpace(Header.FileName))
                Header.FileName = "All DATs";

            if (string.IsNullOrWhiteSpace(Header.Name))
                Header.Name = "All DATs";

            if (string.IsNullOrWhiteSpace(Header.Description))
                Header.Description = "All DATs";

            string post = " (No Duplicates)";
            DatFile outerDiffData = Create(Header);
            outerDiffData.Header.FileName += post;
            outerDiffData.Header.Name += post;
            outerDiffData.Header.Description += post;
            outerDiffData.Items = new ItemDictionary();

            watch.Stop();

            // Now, loop through the dictionary and populate the correct DATs
            watch.Start("Populating no duplicate DAT");

            Parallel.ForEach(Items.Keys, Globals.ParallelOptions, key =>
            {
                List<DatItem> items = DatItem.Merge(Items[key]);

                // If the rom list is empty or null, just skip it
                if (items == null || items.Count == 0)
                    return;

                // Loop through and add the items correctly
                foreach (DatItem item in items)
                {
                    if (item.DupeType.HasFlag(DupeType.Internal) || item.DupeType == 0x00)
                    {
                        DatItem newrom = item.Clone() as DatItem;
                        newrom.Machine.Name += $" ({Path.GetFileNameWithoutExtension(inputs[item.Source.Index].CurrentPath)})";
                        outerDiffData.Items.Add(key, newrom);
                    }
                }
            });

            watch.Stop();

            return outerDiffData;
        }

        /// <summary>
        /// Fill a DatFile with all items with a particular source index ID
        /// </summary>
        /// <param name="indexDat">DatFile to add found items to</param>
        /// <param name="index">Source index ID to retrieve items for</param>
        /// <returns>DatFile containing all items with the source index ID/returns>
        public void FillWithSourceIndex(DatFile indexDat, int index)
        {
            // Loop through and add the items for this index to the output
            Parallel.ForEach(Items.Keys, Globals.ParallelOptions, key =>
            {
                List<DatItem> items = DatItem.Merge(Items[key]);

                // If the rom list is empty or null, just skip it
                if (items == null || items.Count == 0)
                    return;

                foreach (DatItem item in items)
                {
                    if (item.Source.Index == index)
                        indexDat.Items.Add(key, item);
                }
            });
        }

        /// <summary>
        /// Populate from multiple paths while returning the invividual headers
        /// </summary>
        /// <param name="inputs">Paths to DATs to parse</param>
        /// <returns>List of DatHeader objects representing headers</returns>
        public List<DatHeader> PopulateUserData(List<string> inputs)
        {
            List<ParentablePath> paths = inputs.Select(i => new ParentablePath(i)).ToList();
            return PopulateUserData(paths);
        }

        /// <summary>
        /// Populate from multiple paths while returning the invividual headers
        /// </summary>
        /// <param name="inputs">Paths to DATs to parse</param>
        /// <returns>List of DatHeader objects representing headers</returns>
        public List<DatHeader> PopulateUserData(List<ParentablePath> inputs)
        {
            DatFile[] datFiles = new DatFile[inputs.Count];
            InternalStopwatch watch = new InternalStopwatch("Processing individual DATs");

            // Parse all of the DATs into their own DatFiles in the array
            Parallel.For(0, inputs.Count, Globals.ParallelOptions, i =>
            {
                var input = inputs[i];
                logger.User($"Adding DAT: {input.CurrentPath}");
                datFiles[i] = Create(Header.CloneFiltering());
                DatTool.ParseInto(datFiles[i], input, i, keep: true);
            });

            watch.Stop();

            watch.Start("Populating internal DAT");
            for (int i = 0; i < inputs.Count; i++)
            {
                AddFromExisting(datFiles[i], true);
            }

            watch.Stop();

            return datFiles.Select(d => d.Header).ToList();
        }
    }
}