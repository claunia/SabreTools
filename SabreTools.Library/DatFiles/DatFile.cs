using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

using SabreTools.Data;
using SabreTools.Logging;
using SabreTools.Library.DatItems;
using SabreTools.Library.FileTypes;
using SabreTools.Library.Filtering;
using SabreTools.Library.IO;
using SabreTools.Library.Reports;
using SabreTools.Library.Skippers;
using SabreTools.Library.Tools;
using NaturalSort;
using Newtonsoft.Json;

namespace SabreTools.Library.DatFiles
{
    /// <summary>
    /// Represents a format-agnostic DAT
    /// </summary>
    [JsonObject("datfile"), XmlRoot("datfile")]
    public abstract class DatFile
    {
        #region Fields

        /// <summary>
        /// Header values
        /// </summary>
        [JsonProperty("header"), XmlElement("header")]
        public DatHeader Header { get; set; } = new DatHeader();

        /// <summary>
        /// DatItems and related statistics
        /// </summary>
        [JsonProperty("items"), XmlElement("items")]
        public ItemDictionary Items { get; set; } = new ItemDictionary();

        #endregion

        #region Logging

        /// <summary>
        /// Logging object
        /// </summary>
        [JsonIgnore, XmlIgnore]
        protected Logger logger;

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new DatFile from an existing one
        /// </summary>
        /// <param name="datFile">DatFile to get the values from</param>
        public DatFile(DatFile datFile)
        {
            logger = new Logger(this);
            if (datFile != null)
            {
                Header = datFile.Header;
                Items = datFile.Items;
            }
        }

        /// <summary>
        /// Create a specific type of DatFile to be used based on a format and a base DAT
        /// </summary>
        /// <param name="datFormat">Format of the DAT to be created</param>
        /// <param name="baseDat">DatFile containing the information to use in specific operations</param>
        /// <param name="quotes">For relevant types, assume the usage of quotes</param>
        /// <returns>DatFile of the specific internal type that corresponds to the inputs</returns>
        public static DatFile Create(DatFormat? datFormat = null, DatFile baseDat = null, bool quotes = true)
        {
            switch (datFormat)
            {
                case DatFormat.AttractMode:
                    return new AttractMode(baseDat);

                case DatFormat.ClrMamePro:
                    return new ClrMamePro(baseDat, quotes);

                case DatFormat.CSV:
                    return new SeparatedValue(baseDat, ',');

                case DatFormat.DOSCenter:
                    return new DosCenter(baseDat);

                case DatFormat.EverdriveSMDB:
                    return new EverdriveSMDB(baseDat);

                case DatFormat.Listrom:
                    return new Listrom(baseDat);

                case DatFormat.Listxml:
                    return new Listxml(baseDat);

                case DatFormat.Logiqx:
                    return new Logiqx(baseDat, false);

                case DatFormat.LogiqxDeprecated:
                    return new Logiqx(baseDat, true);

                case DatFormat.MissFile:
                    return new Missfile(baseDat);

                case DatFormat.OfflineList:
                    return new OfflineList(baseDat);

                case DatFormat.OpenMSX:
                    return new OpenMSX(baseDat);

                case DatFormat.RedumpMD5:
                    return new Hashfile(baseDat, Hash.MD5);

#if NET_FRAMEWORK
                case DatFormat.RedumpRIPEMD160:
                    return new Hashfile(baseDat, Hash.RIPEMD160);
#endif

                case DatFormat.RedumpSFV:
                    return new Hashfile(baseDat, Hash.CRC);

                case DatFormat.RedumpSHA1:
                    return new Hashfile(baseDat, Hash.SHA1);

                case DatFormat.RedumpSHA256:
                    return new Hashfile(baseDat, Hash.SHA256);

                case DatFormat.RedumpSHA384:
                    return new Hashfile(baseDat, Hash.SHA384);

                case DatFormat.RedumpSHA512:
                    return new Hashfile(baseDat, Hash.SHA512);

                case DatFormat.RedumpSpamSum:
                    return new Hashfile(baseDat, Hash.SpamSum);

                case DatFormat.RomCenter:
                    return new RomCenter(baseDat);

                case DatFormat.SabreJSON:
                    return new SabreJSON(baseDat);

                case DatFormat.SabreXML:
                    return new SabreXML(baseDat);

                case DatFormat.SoftwareList:
                    return new SoftwareList(baseDat);

                case DatFormat.SSV:
                    return new SeparatedValue(baseDat, ';');

                case DatFormat.TSV:
                    return new SeparatedValue(baseDat, '\t');

                // We use new-style Logiqx as a backup for generic DatFile
                case null:
                default:
                    return new Logiqx(baseDat, false);
            }
        }

        /// <summary>
        /// Create a new DatFile from an existing DatHeader
        /// </summary>
        /// <param name="datHeader">DatHeader to get the values from</param>
        public static DatFile Create(DatHeader datHeader)
        {
            DatFile datFile = Create(datHeader.DatFormat);
            datFile.Header = (DatHeader)datHeader.Clone();
            return datFile;
        }

        /// <summary>
        /// Add items from another DatFile to the existing DatFile
        /// </summary>
        /// <param name="datFile">DatFile to add from</param>
        /// <param name="delete">If items should be deleted from the source DatFile</param>
        public void AddFromExisting(DatFile datFile, bool delete = false)
        {
            // Get the list of keys from the DAT
            var keys = datFile.Items.Keys.ToList();
            foreach (string key in keys)
            {
                // Add everything from the key to the internal DAT
                Items.AddRange(key, datFile.Items[key]);

                // Now remove the key from the source DAT
                if (delete)
                    datFile.Items.Remove(key);
            }

            // Now remove the file dictionary from the source DAT
            if (delete)
                datFile.Items = null;
        }

        /// <summary>
        /// Apply a DatHeader to an existing DatFile
        /// </summary>
        /// <param name="datHeader">DatHeader to get the values from</param>
        public void ApplyDatHeader(DatHeader datHeader)
        {
            Header.ConditionalCopy(datHeader);
        }

        /// <summary>
        /// Fill the header values based on existing Header and path
        /// </summary>
        /// <param name="path">Path used for creating a name, if necessary</param>
        /// <param name="bare">True if the date should be omitted from name and description, false otherwise</param>
        public void FillHeaderFromPath(string path, bool bare)
        {
            // If the description is defined but not the name, set the name from the description
            if (string.IsNullOrWhiteSpace(Header.Name) && !string.IsNullOrWhiteSpace(Header.Description))
            {
                Header.Name = Header.Description;
            }

            // If the name is defined but not the description, set the description from the name
            else if (!string.IsNullOrWhiteSpace(Header.Name) && string.IsNullOrWhiteSpace(Header.Description))
            {
                Header.Description = Header.Name + (bare ? string.Empty : $" ({Header.Date})");
            }

            // If neither the name or description are defined, set them from the automatic values
            else if (string.IsNullOrWhiteSpace(Header.Name) && string.IsNullOrWhiteSpace(Header.Description))
            {
                string[] splitpath = path.TrimEnd(Path.DirectorySeparatorChar).Split(Path.DirectorySeparatorChar);
                Header.Name = splitpath.Last();
                Header.Description = Header.Name + (bare ? string.Empty : $" ({Header.Date})");
            }
        }

        #endregion

        #region Converting and Updating

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
        /// Fill a DatFile with all items with a particular ItemType
        /// </summary>
        /// <param name="indexDat">DatFile to add found items to</param>
        /// <param name="itemType">ItemType to retrieve items for</param>
        /// <returns>DatFile containing all items with the ItemType/returns>
        public void FillWithItemType(DatFile indexDat, ItemType itemType)
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
                    if (item.ItemType == itemType)
                        indexDat.Items.Add(key, item);
                }
            });
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
                datFiles[i].Parse(input, i, keep: true);
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

        #endregion

        #region Filtering

        /// <summary>
        /// Apply cleaning methods to the DatFile
        /// </summary>
        /// <param name="cleaner">Cleaner to use</param>
        /// <param name="throwOnError">True if the error that is thrown should be thrown back to the caller, false otherwise</param>
        /// <returns>True if cleaning was successful, false on error</returns>
        public bool ApplyCleaning(Cleaner cleaner, bool throwOnError = false)
        {
            try
            {
                // Perform item-level cleaning
                CleanDatItems(cleaner);

                // Bucket and dedupe according to the flag
                if (cleaner?.DedupeRoms == DedupeType.Full)
                    Items.BucketBy(Field.DatItem_CRC, cleaner.DedupeRoms);
                else if (cleaner?.DedupeRoms == DedupeType.Game)
                    Items.BucketBy(Field.Machine_Name, cleaner.DedupeRoms);

                // Process description to machine name
                if (cleaner?.DescriptionAsName == true)
                    MachineDescriptionToName();

                // If we are removing scene dates, do that now
                if (cleaner?.SceneDateStrip == true)
                    StripSceneDatesFromItems();

                // Run the one rom per game logic, if required
                if (cleaner?.OneGamePerRegion == true)
                    OneGamePerRegion(cleaner.RegionList);

                // Run the one rom per game logic, if required
                if (cleaner?.OneRomPerGame == true)
                    OneRomPerGame();

                // If we are removing fields, do that now
                if (cleaner.ExcludeFields != null && cleaner.ExcludeFields.Any())
                    RemoveFieldsFromItems(cleaner.ExcludeFields);

                // Remove all marked items
                Items.ClearMarked();

                // We remove any blanks, if we aren't supposed to have any
                if (cleaner?.KeepEmptyGames == false)
                    Items.ClearEmpty();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                if (throwOnError) throw ex;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Apply a set of Extra INIs on the DatFile
        /// </summary>
        /// <param name="extras">ExtrasIni to use</param>
        /// <param name="throwOnError">True if the error that is thrown should be thrown back to the caller, false otherwise</param>
        /// <returns>True if the extras were applied, false on error</returns>
        public bool ApplyExtras(ExtraIni extras, bool throwOnError = false)
        {
            try
            {
                // Bucket by game first
                Items.BucketBy(Field.Machine_Name, DedupeType.None);

                // Create a new set of mappings based on the items
                var map = new Dictionary<string, Dictionary<Field, string>>();

                // Loop through each of the extras
                foreach (ExtraIniItem item in extras.Items)
                {
                    foreach (var mapping in item.Mappings)
                    {
                        string key = mapping.Key;
                        List<string> machineNames = mapping.Value;

                        // Loop through the machines and add the new mappings
                        foreach (string machine in machineNames)
                        {
                            if (!map.ContainsKey(machine))
                                map[machine] = new Dictionary<Field, string>();

                            map[machine][item.Field] = key;
                        }
                    }
                }

                // Now apply the new set of mappings
                foreach (string key in map.Keys)
                {
                    // If the key doesn't exist, continue
                    if (!Items.ContainsKey(key))
                        continue;

                    List<DatItem> datItems = Items[key];
                    var mappings = map[key];

                    foreach (var datItem in datItems)
                    {
                        datItem.SetFields(mappings);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                if (throwOnError) throw ex;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Apply a Filter on the DatFile
        /// </summary>
        /// <param name="filter">Filter to use</param>
        /// <param name="perMachine">True if entire machines are considered, false otherwise (default)</param>
        /// <param name="throwOnError">True if the error that is thrown should be thrown back to the caller, false otherwise</param>
        /// <returns>True if the DatFile was filtered, false on error</returns>
        public bool ApplyFilter(Filter filter, bool perMachine = false, bool throwOnError = false)
        {
            // If we have a null filter, return false
            if (filter == null)
                return false;

            // If we're filtering per machine, bucket by machine first
            if (perMachine)
                Items.BucketBy(Field.Machine_Name, DedupeType.None);

            try
            {
                // Loop over every key in the dictionary
                List<string> keys = Items.Keys.ToList();
                foreach (string key in keys)
                {
                    // For every item in the current key
                    bool machinePass = true;
                    List<DatItem> items = Items[key];
                    foreach (DatItem item in items)
                    {
                        // If we have a null item, we can't pass it
                        if (item == null)
                            continue;

                        // If the item is already filtered out, we skip
                        if (item.Remove)
                            continue;

                        // If the rom doesn't pass the filter, mark for removal
                        if (!item.PassesFilter(filter))
                        {
                            item.Remove = true;

                            // If we're in machine mode, set and break
                            if (perMachine)
                            {
                                machinePass = false;
                                break;
                            }
                        }
                    }

                    // If we didn't pass and we're in machine mode, set all items as remove
                    if (perMachine && !machinePass)
                    {
                        foreach (DatItem item in items)
                        {
                            item.Remove = true;
                        }
                    }

                    // Assign back for caution
                    Items[key] = items;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                if (throwOnError) throw ex;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Apply splitting on the DatFile
        /// </summary>
        /// <param name="splitType">Split type to try</param>
        /// <param name="useTags">True if DatFile tags override splitting, false otherwise</param>
        /// <param name="throwOnError">True if the error that is thrown should be thrown back to the caller, false otherwise</param>
        /// <returns>True if the DatFile was split, false on error</returns>
        public bool ApplySplitting(MergingFlag splitType, bool useTags, bool throwOnError = false)
        {
            try
            {
                // If we are using tags from the DAT, set the proper input for split type unless overridden
                if (useTags && splitType == MergingFlag.None)
                    splitType = Header.ForceMerging;

                // Run internal splitting
                switch (splitType)
                {
                    case MergingFlag.None:
                        // No-op
                        break;
                    case MergingFlag.Device:
                        CreateDeviceNonMergedSets(DedupeType.None);
                        break;
                    case MergingFlag.Full:
                        CreateFullyNonMergedSets(DedupeType.None);
                        break;
                    case MergingFlag.NonMerged:
                        CreateNonMergedSets(DedupeType.None);
                        break;
                    case MergingFlag.Merged:
                        CreateMergedSets(DedupeType.None);
                        break;
                    case MergingFlag.Split:
                        CreateSplitSets(DedupeType.None);
                        break;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                if (throwOnError) throw ex;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Apply SuperDAT naming logic to a merged DatFile
        /// </summary>
        /// <param name="inputs">List of inputs to use for renaming</param>
        public void ApplySuperDAT(List<ParentablePath> inputs)
        {
            List<string> keys = Items.Keys.ToList();
            Parallel.ForEach(keys, Globals.ParallelOptions, key =>
            {
                List<DatItem> items = Items[key].ToList();
                List<DatItem> newItems = new List<DatItem>();
                foreach (DatItem item in items)
                {
                    DatItem newItem = item;
                    string filename = inputs[newItem.Source.Index].CurrentPath;
                    string rootpath = inputs[newItem.Source.Index].ParentPath;

                    if (!string.IsNullOrWhiteSpace(rootpath))
                        rootpath += Path.DirectorySeparatorChar.ToString();

                    filename = filename.Remove(0, rootpath.Length);
                    newItem.Machine.Name = Path.GetDirectoryName(filename) + Path.DirectorySeparatorChar
                        + Path.GetFileNameWithoutExtension(filename) + Path.DirectorySeparatorChar
                        + newItem.Machine.Name;

                    newItems.Add(newItem);
                }

                Items.Remove(key);
                Items.AddRange(key, newItems);
            });
        }

        /// <summary>
        /// Clean individual items based on the current filter
        /// </summary>
        /// <param name="cleaner">Cleaner to use</param>
        public void CleanDatItems(Cleaner cleaner)
        {
            List<string> keys = Items.Keys.ToList();
            foreach (string key in keys)
            {
                // For every item in the current key
                List<DatItem> items = Items[key];
                foreach (DatItem item in items)
                {
                    // If we have a null item, we can't clean it it
                    if (item == null)
                        continue;

                    // Run cleaning per item
                    item.Clean(cleaner);
                }

                // Assign back for caution
                Items[key] = items;
            }
        }

        /// <summary>
        /// Use game descriptions as names in the DAT, updating cloneof/romof/sampleof
        /// </summary>
        /// <param name="throwOnError">True if the error that is thrown should be thrown back to the caller, false otherwise</param>
        public void MachineDescriptionToName(bool throwOnError = false)
        {
            try
            {
                // First we want to get a mapping for all games to description
                ConcurrentDictionary<string, string> mapping = new ConcurrentDictionary<string, string>();
                Parallel.ForEach(Items.Keys, Globals.ParallelOptions, key =>
                {
                    List<DatItem> items = Items[key];
                    foreach (DatItem item in items)
                    {
                        // If the key mapping doesn't exist, add it
                        mapping.TryAdd(item.Machine.Name, item.Machine.Description.Replace('/', '_').Replace("\"", "''").Replace(":", " -"));
                    }
                });

                // Now we loop through every item and update accordingly
                Parallel.ForEach(Items.Keys, Globals.ParallelOptions, key =>
                {
                    List<DatItem> items = Items[key];
                    List<DatItem> newItems = new List<DatItem>();
                    foreach (DatItem item in items)
                    {
                        // Update machine name
                        if (!string.IsNullOrWhiteSpace(item.Machine.Name) && mapping.ContainsKey(item.Machine.Name))
                            item.Machine.Name = mapping[item.Machine.Name];

                        // Update cloneof
                        if (!string.IsNullOrWhiteSpace(item.Machine.CloneOf) && mapping.ContainsKey(item.Machine.CloneOf))
                            item.Machine.CloneOf = mapping[item.Machine.CloneOf];

                        // Update romof
                        if (!string.IsNullOrWhiteSpace(item.Machine.RomOf) && mapping.ContainsKey(item.Machine.RomOf))
                            item.Machine.RomOf = mapping[item.Machine.RomOf];

                        // Update sampleof
                        if (!string.IsNullOrWhiteSpace(item.Machine.SampleOf) && mapping.ContainsKey(item.Machine.SampleOf))
                            item.Machine.SampleOf = mapping[item.Machine.SampleOf];

                        // Add the new item to the output list
                        newItems.Add(item);
                    }

                    // Replace the old list of roms with the new one
                    Items.Remove(key);
                    Items.AddRange(key, newItems);
                });
            }
            catch (Exception ex)
            {
                logger.Warning(ex.ToString());
                if (throwOnError) throw ex;
            }
        }

        /// <summary>
        /// Filter a DAT using 1G1R logic given an ordered set of regions
        /// </summary>
        /// <param name="regions">Ordered list of regions to use</param>
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
        public void OneGamePerRegion(List<string> regions)
        {
            // If we have null region list, make it empty
            if (regions == null)
                regions = new List<string>();

            // For sake of ease, the first thing we want to do is bucket by game
            Items.BucketBy(Field.Machine_Name, DedupeType.None, norename: true);

            // Then we want to get a mapping of all machines to parents
            Dictionary<string, List<string>> parents = new Dictionary<string, List<string>>();
            foreach (string key in Items.Keys)
            {
                DatItem item = Items[key][0];

                // Match on CloneOf first
                if (!string.IsNullOrEmpty(item.Machine.CloneOf))
                {
                    if (!parents.ContainsKey(item.Machine.CloneOf.ToLowerInvariant()))
                        parents.Add(item.Machine.CloneOf.ToLowerInvariant(), new List<string>());

                    parents[item.Machine.CloneOf.ToLowerInvariant()].Add(item.Machine.Name.ToLowerInvariant());
                }

                // Then by RomOf
                else if (!string.IsNullOrEmpty(item.Machine.RomOf))
                {
                    if (!parents.ContainsKey(item.Machine.RomOf.ToLowerInvariant()))
                        parents.Add(item.Machine.RomOf.ToLowerInvariant(), new List<string>());

                    parents[item.Machine.RomOf.ToLowerInvariant()].Add(item.Machine.Name.ToLowerInvariant());
                }

                // Otherwise, treat it as a parent
                else
                {
                    if (!parents.ContainsKey(item.Machine.Name.ToLowerInvariant()))
                        parents.Add(item.Machine.Name.ToLowerInvariant(), new List<string>());

                    parents[item.Machine.Name.ToLowerInvariant()].Add(item.Machine.Name.ToLowerInvariant());
                }
            }

            // Once we have the full list of mappings, filter out games to keep
            foreach (string key in parents.Keys)
            {
                // Find the first machine that matches the regions in order, if possible
                string machine = default;
                foreach (string region in regions)
                {
                    machine = parents[key].FirstOrDefault(m => Regex.IsMatch(m, @"\(.*" + region + @".*\)", RegexOptions.IgnoreCase));
                    if (machine != default)
                        break;
                }

                // If we didn't get a match, use the parent
                if (machine == default)
                    machine = key;

                // Remove the key from the list
                parents[key].Remove(machine);

                // Remove the rest of the items from this key
                parents[key].ForEach(k => Items.Remove(k));
            }

            // Finally, strip out the parent tags
            RemoveTagsFromChild();
        }

        /// <summary>
        /// Ensure that all roms are in their own game (or at least try to ensure)
        /// </summary>
        public void OneRomPerGame()
        {
            // Because this introduces subfolders, we need to set the SuperDAT type
            Header.Type = "SuperDAT";

            // For each rom, we want to update the game to be "<game name>/<rom name>"
            Parallel.ForEach(Items.Keys, Globals.ParallelOptions, key =>
            {
                List<DatItem> items = Items[key];
                for (int i = 0; i < items.Count; i++)
                {
                    items[i].SetOneRomPerGame();
                }
            });
        }

        /// <summary>
        /// Remove fields as per the header
        /// </summary>
        /// <param name="fields">List of fields to use</param>
        public void RemoveFieldsFromItems(List<Field> fields)
        {
            // If we have null field list, make it empty
            if (fields == null)
                fields = new List<Field>();

            // Output the logging statement
            logger.User("Removing filtered fields");

            // Now process all of the roms
            Parallel.ForEach(Items.Keys, Globals.ParallelOptions, key =>
            {
                List<DatItem> items = Items[key];
                for (int j = 0; j < items.Count; j++)
                {
                    items[j].RemoveFields(fields);
                }

                Items.Remove(key);
                Items.AddRange(key, items);
            });
        }

        /// <summary>
        /// Strip the dates from the beginning of scene-style set names
        /// </summary>
        public void StripSceneDatesFromItems()
        {
            // Output the logging statement
            logger.User("Stripping scene-style dates");

            // Set the regex pattern to use
            string pattern = @"([0-9]{2}\.[0-9]{2}\.[0-9]{2}-)(.*?-.*?)";

            // Now process all of the roms
            Parallel.ForEach(Items.Keys, Globals.ParallelOptions, key =>
            {
                List<DatItem> items = Items[key];
                for (int j = 0; j < items.Count; j++)
                {
                    DatItem item = items[j];
                    if (Regex.IsMatch(item.Machine.Name, pattern))
                        item.Machine.Name = Regex.Replace(item.Machine.Name, pattern, "$2");

                    if (Regex.IsMatch(item.Machine.Description, pattern))
                        item.Machine.Description = Regex.Replace(item.Machine.Description, pattern, "$2");

                    items[j] = item;
                }

                Items.Remove(key);
                Items.AddRange(key, items);
            });
        }

        #endregion

        // TODO: Should any of these create a new DatFile in the process?
        // The reason this comes up is that doing any of the splits or merges
        // is an inherently destructive process. Making it output a new DatFile
        // might make it easier to deal with multiple internal steps. On the other
        // hand, this will increase memory usage significantly and would force the
        // existing paths to behave entirely differently
        #region Internal Splitting/Merging

        /// <summary>
        /// Use cdevice_ref tags to get full non-merged sets and remove parenting tags
        /// </summary>
        /// <param name="mergeroms">Dedupe type to be used</param>
        private void CreateDeviceNonMergedSets(DedupeType mergeroms)
        {
            logger.User("Creating device non-merged sets from the DAT");

            // For sake of ease, the first thing we want to do is bucket by game
            Items.BucketBy(Field.Machine_Name, mergeroms, norename: true);

            // Now we want to loop through all of the games and set the correct information
            while (AddRomsFromDevices(false, false)) ;
            while (AddRomsFromDevices(true, false)) ;

            // Then, remove the romof and cloneof tags so it's not picked up by the manager
            RemoveTagsFromChild();
        }

        /// <summary>
        /// Use cloneof tags to create non-merged sets and remove the tags plus using the device_ref tags to get full sets
        /// </summary>
        /// <param name="mergeroms">Dedupe type to be used</param>
        private void CreateFullyNonMergedSets(DedupeType mergeroms)
        {
            logger.User("Creating fully non-merged sets from the DAT");

            // For sake of ease, the first thing we want to do is bucket by game
            Items.BucketBy(Field.Machine_Name, mergeroms, norename: true);

            // Now we want to loop through all of the games and set the correct information
            while (AddRomsFromDevices(true, true)) ;
            AddRomsFromDevices(false, true);
            AddRomsFromParent();

            // Now that we have looped through the cloneof tags, we loop through the romof tags
            AddRomsFromBios();

            // Then, remove the romof and cloneof tags so it's not picked up by the manager
            RemoveTagsFromChild();
        }

        /// <summary>
        /// Use cloneof tags to create merged sets and remove the tags
        /// </summary>
        /// <param name="mergeroms">Dedupe type to be used</param>
        private void CreateMergedSets(DedupeType mergeroms)
        {
            logger.User("Creating merged sets from the DAT");

            // For sake of ease, the first thing we want to do is bucket by game
            Items.BucketBy(Field.Machine_Name, mergeroms, norename: true);

            // Now we want to loop through all of the games and set the correct information
            AddRomsFromChildren();

            // Now that we have looped through the cloneof tags, we loop through the romof tags
            RemoveBiosRomsFromChild(false);
            RemoveBiosRomsFromChild(true);

            // Finally, remove the romof and cloneof tags so it's not picked up by the manager
            RemoveTagsFromChild();
        }

        /// <summary>
        /// Use cloneof tags to create non-merged sets and remove the tags
        /// </summary>
        /// <param name="mergeroms">Dedupe type to be used</param>
        private void CreateNonMergedSets(DedupeType mergeroms)
        {
            logger.User("Creating non-merged sets from the DAT");

            // For sake of ease, the first thing we want to do is bucket by game
            Items.BucketBy(Field.Machine_Name, mergeroms, norename: true);

            // Now we want to loop through all of the games and set the correct information
            AddRomsFromParent();

            // Now that we have looped through the cloneof tags, we loop through the romof tags
            RemoveBiosRomsFromChild(false);
            RemoveBiosRomsFromChild(true);

            // Finally, remove the romof and cloneof tags so it's not picked up by the manager
            RemoveTagsFromChild();
        }

        /// <summary>
        /// Use cloneof and romof tags to create split sets and remove the tags
        /// </summary>
        /// <param name="mergeroms">Dedupe type to be used</param>
        private void CreateSplitSets(DedupeType mergeroms)
        {
            logger.User("Creating split sets from the DAT");

            // For sake of ease, the first thing we want to do is bucket by game
            Items.BucketBy(Field.Machine_Name, mergeroms, norename: true);

            // Now we want to loop through all of the games and set the correct information
            RemoveRomsFromChild();

            // Now that we have looped through the cloneof tags, we loop through the romof tags
            RemoveBiosRomsFromChild(false);
            RemoveBiosRomsFromChild(true);

            // Finally, remove the romof and cloneof tags so it's not picked up by the manager
            RemoveTagsFromChild();
        }

        /// <summary>
        /// Use romof tags to add roms to the children
        /// </summary>
        private void AddRomsFromBios()
        {
            List<string> games = Items.Keys.OrderBy(g => g).ToList();
            foreach (string game in games)
            {
                // If the game has no items in it, we want to continue
                if (Items[game].Count == 0)
                    continue;

                // Determine if the game has a parent or not
                string parent = null;
                if (!string.IsNullOrWhiteSpace(Items[game][0].Machine.RomOf))
                    parent = Items[game][0].Machine.RomOf;

                // If the parent doesnt exist, we want to continue
                if (string.IsNullOrWhiteSpace(parent))
                    continue;

                // If the parent doesn't have any items, we want to continue
                if (Items[parent].Count == 0)
                    continue;

                // If the parent exists and has items, we copy the items from the parent to the current game
                DatItem copyFrom = Items[game][0];
                List<DatItem> parentItems = Items[parent];
                foreach (DatItem item in parentItems)
                {
                    DatItem datItem = (DatItem)item.Clone();
                    datItem.CopyMachineInformation(copyFrom);
                    if (Items[game].Where(i => i.GetName() == datItem.GetName()).Count() == 0 && !Items[game].Contains(datItem))
                        Items.Add(game, datItem);
                }
            }
        }

        /// <summary>
        /// Use device_ref and optionally slotoption tags to add roms to the children
        /// </summary>
        /// <param name="dev">True if only child device sets are touched, false for non-device sets (default)</param>
        /// <param name="useSlotOptions">True if slotoptions tags are used as well, false otherwise</param>
        private bool AddRomsFromDevices(bool dev = false, bool useSlotOptions = false)
        {
            bool foundnew = false;
            List<string> machines = Items.Keys.OrderBy(g => g).ToList();
            foreach (string machine in machines)
            {
                // If the machine doesn't have items, we continue
                if (Items[machine] == null || Items[machine].Count == 0)
                    continue;

                // If the machine (is/is not) a device, we want to continue
                if (dev ^ (Items[machine][0].Machine.MachineType.HasFlag(MachineType.Device)))
                    continue;

                // Get all device reference names from the current machine
                List<string> deviceReferences = Items[machine]
                    .Where(i => i.ItemType == ItemType.DeviceReference)
                    .Select(i => i as DeviceReference)
                    .Select(dr => dr.Name)
                    .Distinct()
                    .ToList();

                // Get all slot option names from the current machine
                List<string> slotOptions = Items[machine]
                    .Where(i => i.ItemType == ItemType.Slot)
                    .Select(i => i as Slot)
                    .Where(s => s.SlotOptionsSpecified)
                    .SelectMany(s => s.SlotOptions)
                    .Select(so => so.DeviceName)
                    .Distinct()
                    .ToList();

                // If we're checking device references
                if (deviceReferences.Any())
                {
                    // Loop through all names and check the corresponding machines
                    List<string> newDeviceReferences = new List<string>();
                    foreach (string deviceReference in deviceReferences)
                    {
                        // If the machine doesn't exist then we continue
                        if (Items[deviceReference] == null || Items[deviceReference].Count == 0)
                            continue;

                        // Add to the list of new device reference names
                        List<DatItem> devItems = Items[deviceReference];
                        newDeviceReferences.AddRange(devItems
                            .Where(i => i.ItemType == ItemType.DeviceReference)
                            .Select(i => (i as DeviceReference).Name));

                        // Set new machine information and add to the current machine
                        DatItem copyFrom = Items[machine][0];
                        foreach (DatItem item in devItems)
                        {
                            // If the parent machine doesn't already contain this item, add it
                            if (!Items[machine].Any(i => i.ItemType == item.ItemType && i.GetName() == item.GetName()))
                            {
                                // Set that we found new items
                                foundnew = true;

                                // Clone the item and then add it
                                DatItem datItem = (DatItem)item.Clone();
                                datItem.CopyMachineInformation(copyFrom);
                                Items.Add(machine, datItem);
                            }
                        }
                    }

                    // Now that every device reference is accounted for, add the new list of device references, if they don't already exist
                    foreach (string deviceReference in newDeviceReferences.Distinct())
                    {
                        if (!deviceReferences.Contains(deviceReference))
                            Items[machine].Add(new DeviceReference() { Name = deviceReference });
                    }
                }

                // If we're checking slotoptions
                if (useSlotOptions && slotOptions.Any())
                {
                    // Loop through all names and check the corresponding machines
                    List<string> newSlotOptions = new List<string>();
                    foreach (string slotOption in slotOptions)
                    {
                        // If the machine doesn't exist then we continue
                        if (Items[slotOption] == null || Items[slotOption].Count == 0)
                            continue;

                        // Add to the list of new slot option names
                        List<DatItem> slotItems = Items[slotOption];
                        newSlotOptions.AddRange(slotItems
                            .Where(i => i.ItemType == ItemType.Slot)
                            .Where(s => (s as Slot).SlotOptionsSpecified)
                            .SelectMany(s => (s as Slot).SlotOptions)
                            .Select(o => o.DeviceName));

                        // Set new machine information and add to the current machine
                        DatItem copyFrom = Items[machine][0];
                        foreach (DatItem item in slotItems)
                        {
                            // If the parent machine doesn't already contain this item, add it
                            if (!Items[machine].Any(i => i.ItemType == item.ItemType && i.GetName() == item.GetName()))
                            {
                                // Set that we found new items
                                foundnew = true;

                                // Clone the item and then add it
                                DatItem datItem = (DatItem)item.Clone();
                                datItem.CopyMachineInformation(copyFrom);
                                Items.Add(machine, datItem);
                            }
                        }
                    }

                    // Now that every device is accounted for, add the new list of slot options, if they don't already exist
                    foreach (string slotOption in newSlotOptions.Distinct())
                    {
                        if (!slotOptions.Contains(slotOption))
                            Items[machine].Add(new Slot() { SlotOptions = new List<SlotOption> { new SlotOption { DeviceName = slotOption } } });
                    }
                }
            }

            return foundnew;
        }

        /// <summary>
        /// Use cloneof tags to add roms to the children, setting the new romof tag in the process
        /// </summary>
        private void AddRomsFromParent()
        {
            List<string> games = Items.Keys.OrderBy(g => g).ToList();
            foreach (string game in games)
            {
                // If the game has no items in it, we want to continue
                if (Items[game].Count == 0)
                    continue;

                // Determine if the game has a parent or not
                string parent = null;
                if (!string.IsNullOrWhiteSpace(Items[game][0].Machine.CloneOf))
                    parent = Items[game][0].Machine.CloneOf;

                // If the parent doesnt exist, we want to continue
                if (string.IsNullOrWhiteSpace(parent))
                    continue;

                // If the parent doesn't have any items, we want to continue
                if (Items[parent].Count == 0)
                    continue;

                // If the parent exists and has items, we copy the items from the parent to the current game
                DatItem copyFrom = Items[game][0];
                List<DatItem> parentItems = Items[parent];
                foreach (DatItem item in parentItems)
                {
                    DatItem datItem = (DatItem)item.Clone();
                    datItem.CopyMachineInformation(copyFrom);
                    if (Items[game].Where(i => i.GetName()?.ToLowerInvariant() == datItem.GetName()?.ToLowerInvariant()).Count() == 0
                        && !Items[game].Contains(datItem))
                    {
                        Items.Add(game, datItem);
                    }
                }

                // Now we want to get the parent romof tag and put it in each of the items
                List<DatItem> items = Items[game];
                string romof = Items[parent][0].Machine.RomOf;
                foreach (DatItem item in items)
                {
                    item.Machine.RomOf = romof;
                }
            }
        }

        /// <summary>
        /// Use cloneof tags to add roms to the parents, removing the child sets in the process
        /// </summary>
        /// <param name="subfolder">True to add DatItems to subfolder of parent (not including Disk), false otherwise</param>
        private void AddRomsFromChildren(bool subfolder = true)
        {
            List<string> games = Items.Keys.OrderBy(g => g).ToList();
            foreach (string game in games)
            {
                // If the game has no items in it, we want to continue
                if (Items[game].Count == 0)
                    continue;

                // Determine if the game has a parent or not
                string parent = null;
                if (!string.IsNullOrWhiteSpace(Items[game][0].Machine.CloneOf))
                    parent = Items[game][0].Machine.CloneOf;

                // If there is no parent, then we continue
                if (string.IsNullOrWhiteSpace(parent))
                    continue;

                // Otherwise, move the items from the current game to a subfolder of the parent game
                DatItem copyFrom;
                if (Items[parent].Count == 0)
                {
                    copyFrom = new Rom();
                    copyFrom.Machine.Name = parent;
                    copyFrom.Machine.Description = parent;
                }
                else
                {
                    copyFrom = Items[parent][0];
                }

                List<DatItem> items = Items[game];
                foreach (DatItem item in items)
                {
                    // Special disk handling
                    if (item.ItemType == ItemType.Disk)
                    {
                        Disk disk = item as Disk;

                        // If the merge tag exists and the parent already contains it, skip
                        if (disk.MergeTag != null && Items[parent].Where(i => i.ItemType == ItemType.Disk).Select(i => (i as Disk).Name).Contains(disk.MergeTag))
                        {
                            continue;
                        }

                        // If the merge tag exists but the parent doesn't contain it, add to parent
                        else if (disk.MergeTag != null && !Items[parent].Where(i => i.ItemType == ItemType.Disk).Select(i => (i as Disk).Name).Contains(disk.MergeTag))
                        {
                            disk.CopyMachineInformation(copyFrom);
                            Items.Add(parent, disk);
                        }

                        // If there is no merge tag, add to parent
                        else if (disk.MergeTag == null)
                        {
                            disk.CopyMachineInformation(copyFrom);
                            Items.Add(parent, disk);
                        }
                    }

                    // Special rom handling
                    else if (item.ItemType == ItemType.Rom)
                    {
                        Rom rom = item as Rom;

                        // If the merge tag exists and the parent already contains it, skip
                        if (rom.MergeTag != null && Items[parent].Where(i => i.ItemType == ItemType.Rom).Select(i => (i as Rom).Name).Contains(rom.MergeTag))
                        {
                            continue;
                        }

                        // If the merge tag exists but the parent doesn't contain it, add to subfolder of parent
                        else if (rom.MergeTag != null && !Items[parent].Where(i => i.ItemType == ItemType.Rom).Select(i => (i as Rom).Name).Contains(rom.MergeTag))
                        {
                            if (subfolder)
                                rom.Name = $"{rom.Machine.Name}\\{rom.Name}";

                            rom.CopyMachineInformation(copyFrom);
                            Items.Add(parent, rom);
                        }

                        // If the parent doesn't already contain this item, add to subfolder of parent
                        else if (!Items[parent].Contains(item))
                        {
                            if (subfolder)
                                rom.Name = $"{item.Machine.Name}\\{rom.Name}";

                            rom.CopyMachineInformation(copyFrom);
                            Items.Add(parent, rom);
                        }
                    }

                    // All other that would be missing to subfolder of parent
                    else if (!Items[parent].Contains(item))
                    {
                        if (subfolder)
                            item.SetFields(new Dictionary<Field, string> { [Field.DatItem_Name] = $"{item.Machine.Name}\\{item.GetName()}" });

                        item.CopyMachineInformation(copyFrom);
                        Items.Add(parent, item);
                    }
                }

                // Then, remove the old game so it's not picked up by the writer
                Items.Remove(game);
            }
        }

        /// <summary>
        /// Remove all BIOS and device sets
        /// </summary>
        private void RemoveBiosAndDeviceSets()
        {
            List<string> games = Items.Keys.OrderBy(g => g).ToList();
            foreach (string game in games)
            {
                if (Items[game].Count > 0
                    && (Items[game][0].Machine.MachineType.HasFlag(MachineType.Bios)
                        || Items[game][0].Machine.MachineType.HasFlag(MachineType.Device)))
                {
                    Items.Remove(game);
                }
            }
        }

        /// <summary>
        /// Use romof tags to remove bios roms from children
        /// </summary>
        /// <param name="bios">True if only child Bios sets are touched, false for non-bios sets (default)</param>
        private void RemoveBiosRomsFromChild(bool bios = false)
        {
            // Loop through the romof tags
            List<string> games = Items.Keys.OrderBy(g => g).ToList();
            foreach (string game in games)
            {
                // If the game has no items in it, we want to continue
                if (Items[game].Count == 0)
                    continue;

                // If the game (is/is not) a bios, we want to continue
                if (bios ^ Items[game][0].Machine.MachineType.HasFlag(MachineType.Bios))
                    continue;

                // Determine if the game has a parent or not
                string parent = null;
                if (!string.IsNullOrWhiteSpace(Items[game][0].Machine.RomOf))
                    parent = Items[game][0].Machine.RomOf;

                // If the parent doesnt exist, we want to continue
                if (string.IsNullOrWhiteSpace(parent))
                    continue;

                // If the parent doesn't have any items, we want to continue
                if (Items[parent].Count == 0)
                    continue;

                // If the parent exists and has items, we remove the items that are in the parent from the current game
                List<DatItem> parentItems = Items[parent];
                foreach (DatItem item in parentItems)
                {
                    DatItem datItem = (DatItem)item.Clone();
                    while (Items[game].Contains(datItem))
                    {
                        Items.Remove(game, datItem);
                    }
                }
            }
        }

        /// <summary>
        /// Use cloneof tags to remove roms from the children
        /// </summary>
        private void RemoveRomsFromChild()
        {
            List<string> games = Items.Keys.OrderBy(g => g).ToList();
            foreach (string game in games)
            {
                // If the game has no items in it, we want to continue
                if (Items[game].Count == 0)
                    continue;

                // Determine if the game has a parent or not
                string parent = null;
                if (!string.IsNullOrWhiteSpace(Items[game][0].Machine.CloneOf))
                    parent = Items[game][0].Machine.CloneOf;

                // If the parent doesnt exist, we want to continue
                if (string.IsNullOrWhiteSpace(parent))
                    continue;

                // If the parent doesn't have any items, we want to continue
                if (Items[parent].Count == 0)
                    continue;

                // If the parent exists and has items, we remove the parent items from the current game
                List<DatItem> parentItems = Items[parent];
                foreach (DatItem item in parentItems)
                {
                    DatItem datItem = (DatItem)item.Clone();
                    while (Items[game].Contains(datItem))
                    {
                        Items.Remove(game, datItem);
                    }
                }

                // Now we want to get the parent romof tag and put it in each of the remaining items
                List<DatItem> items = Items[game];
                string romof = Items[parent][0].Machine.RomOf;
                foreach (DatItem item in items)
                {
                    item.Machine.RomOf = romof;
                }
            }
        }

        /// <summary>
        /// Remove all romof and cloneof tags from all games
        /// </summary>
        private void RemoveTagsFromChild()
        {
            List<string> games = Items.Keys.OrderBy(g => g).ToList();
            foreach (string game in games)
            {
                List<DatItem> items = Items[game];
                foreach (DatItem item in items)
                {
                    item.Machine.CloneOf = null;
                    item.Machine.RomOf = null;
                    item.Machine.SampleOf = null;
                }
            }
        }

        #endregion

        #region Parsing

        /// <summary>
        /// Create a DatFile and parse a file into it
        /// </summary>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="throwOnError">True if the error that is thrown should be thrown back to the caller, false otherwise</param>
        public static DatFile CreateAndParse(string filename, bool throwOnError = false)
        {
            DatFile datFile = Create();
            datFile.Parse(new ParentablePath(filename), throwOnError: throwOnError);
            return datFile;
        }

        /// <summary>
        /// Parse a DAT and return all found games and roms within
        /// </summary>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="keep">True if full pathnames are to be kept, false otherwise (default)</param>
        /// <param name="keepext">True if original extension should be kept, false otherwise (default)</param>
        /// <param name="quotes">True if quotes are assumed in supported types (default), false otherwise</param>
        /// <param name="throwOnError">True if the error that is thrown should be thrown back to the caller, false otherwise</param>
        public void Parse(
            string filename,
            int indexId = 0,
            bool keep = false,
            bool keepext = false,
            bool quotes = true,
            bool throwOnError = false)
        {
            ParentablePath path = new ParentablePath(filename.Trim('"'));
            Parse(path, indexId, keep, keepext, quotes, throwOnError);
        }

        /// <summary>
        /// Parse a DAT and return all found games and roms within
        /// </summary>
        /// <param name="input">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="keep">True if full pathnames are to be kept, false otherwise (default)</param>
        /// <param name="keepext">True if original extension should be kept, false otherwise (default)</param>
        /// <param name="quotes">True if quotes are assumed in supported types (default), false otherwise</param>
        /// <param name="throwOnError">True if the error that is thrown should be thrown back to the caller, false otherwise</param>
        public void Parse(
            ParentablePath input,
            int indexId = 0,
            bool keep = false,
            bool keepext = false,
            bool quotes = true,
            bool throwOnError = true)
        {
            // Get the current path from the filename
            string currentPath = input.CurrentPath;

            // Check the file extension first as a safeguard
            if (!PathExtensions.HasValidDatExtension(currentPath))
                return;

            // If the output filename isn't set already, get the internal filename
            Header.FileName = (string.IsNullOrWhiteSpace(Header.FileName) ? (keepext ? Path.GetFileName(currentPath) : Path.GetFileNameWithoutExtension(currentPath)) : Header.FileName);

            // If the output type isn't set already, get the internal output type
            Header.DatFormat = (Header.DatFormat == 0 ? currentPath.GetDatFormat() : Header.DatFormat);
            Items.SetBucketedBy(Field.DatItem_CRC); // Setting this because it can reduce issues later

            // Now parse the correct type of DAT
            try
            {
                Create(currentPath.GetDatFormat(), this, quotes)?.ParseFile(currentPath, indexId, keep, throwOnError);
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Error with file '{currentPath}'");
                if (throwOnError) throw ex;
            }
        }

        /// <summary>
        /// Add a rom to the Dat after checking
        /// </summary>
        /// <param name="item">Item data to check against</param>
        /// <returns>The key for the item</returns>
        protected string ParseAddHelper(DatItem item)
        {
            string key = string.Empty;

            // If we have a Disk, Media, or Rom, clean the hash data
            if (item.ItemType == ItemType.Disk)
            {
                Disk disk = item as Disk;

                // If the file has aboslutely no hashes, skip and log
                if (disk.ItemStatus != ItemStatus.Nodump
                    && string.IsNullOrWhiteSpace(disk.MD5)
                    && string.IsNullOrWhiteSpace(disk.SHA1))
                {
                    logger.Verbose($"Incomplete entry for '{disk.Name}' will be output as nodump");
                    disk.ItemStatus = ItemStatus.Nodump;
                }

                item = disk;
            }
            else if (item.ItemType == ItemType.Rom)
            {
                Rom rom = item as Rom;

                // If we have the case where there is SHA-1 and nothing else, we don't fill in any other part of the data
                if (rom.Size == null && !rom.HasHashes())
                {
                    // No-op, just catch it so it doesn't go further
                    logger.Verbose($"{Header.FileName}: Entry with only SHA-1 found - '{rom.Name}'");
                }

                // If we have a rom and it's missing size AND the hashes match a 0-byte file, fill in the rest of the info
                else if ((rom.Size == 0 || rom.Size == null)
                    && (string.IsNullOrWhiteSpace(rom.CRC) || rom.HasZeroHash()))
                {
                    // TODO: All instances of Hash.DeepHashes should be made into 0x0 eventually
                    rom.Size = Constants.SizeZero;
                    rom.CRC = Constants.CRCZero;
                    rom.MD5 = Constants.MD5Zero;
#if NET_FRAMEWORK
                    rom.RIPEMD160 = null; // Constants.RIPEMD160Zero;
#endif
                    rom.SHA1 = Constants.SHA1Zero;
                    rom.SHA256 = null; // Constants.SHA256Zero;
                    rom.SHA384 = null; // Constants.SHA384Zero;
                    rom.SHA512 = null; // Constants.SHA512Zero;
                    rom.SpamSum = null; // Constants.SpamSumZero;
                }

                // If the file has no size and it's not the above case, skip and log
                else if (rom.ItemStatus != ItemStatus.Nodump && (rom.Size == 0 || rom.Size == null))
                {
                    logger.Verbose($"{Header.FileName}: Incomplete entry for '{rom.Name}' will be output as nodump");
                    rom.ItemStatus = ItemStatus.Nodump;
                }

                // If the file has a size but aboslutely no hashes, skip and log
                else if (rom.ItemStatus != ItemStatus.Nodump
                    && rom.Size != null && rom.Size > 0
                    && !rom.HasHashes())
                {
                    logger.Verbose($"{Header.FileName}: Incomplete entry for '{rom.Name}' will be output as nodump");
                    rom.ItemStatus = ItemStatus.Nodump;
                }

                item = rom;
            }

            // Get the key and add the file
            key = item.GetKey(Field.Machine_Name);
            Items.Add(key, item);

            return key;
        }

        /// <summary>
        /// Parse DatFile and return all found games and roms within
        /// </summary>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="keep">True if full pathnames are to be kept, false otherwise (default)</param>
        /// <param name="throwOnError">True if the error that is thrown should be thrown back to the caller, false otherwise</param>
        protected abstract void ParseFile(string filename, int indexId, bool keep, bool throwOnError = false);

        #endregion

        // TODO: See if any of the methods can be broken up a bit more neatly
        #region Populate DAT from Directory

        /// <summary>
        /// Create a new Dat from a directory
        /// </summary>
        /// <param name="basePath">Base folder to be used in creating the DAT</param>
        /// <param name="asFiles">TreatAsFiles representing CHD and Archive scanning</param>
        /// <param name="skipFileType">Type of files that should be skipped</param>
        /// <param name="addBlanks">True if blank items should be created for empty folders, false otherwise</param>
        /// <param name="hashes">Hashes to include in the information</param>
        public bool PopulateFromDir(
            string basePath,
            TreatAsFile asFiles = 0x00,
            SkipFileType skipFileType = SkipFileType.None,
            bool addBlanks = false,
            Hash hashes = Hash.Standard)
        {
            // Clean the temp directory path
            Globals.TempDir = DirectoryExtensions.Ensure(Globals.TempDir, temp: true);

            // Set the progress variables
            long totalSize = 0;
            long currentSize = 0;

            // Process the input
            if (Directory.Exists(basePath))
            {
                logger.Verbose($"Folder found: {basePath}");

                // Get a list of all files to process
                List<string> files = Directory.EnumerateFiles(basePath, "*", SearchOption.AllDirectories).ToList();

                // Loop through and add the file sizes
                Parallel.ForEach(files, Globals.ParallelOptions, item =>
                {
                    Interlocked.Add(ref totalSize, new FileInfo(item).Length);
                });

                // Process the files in the main folder or any subfolder
                logger.User(totalSize, currentSize);
                foreach (string item in files)
                {
                    CheckFileForHashes(item, basePath, asFiles, skipFileType, addBlanks, hashes);
                    currentSize += new FileInfo(item).Length;
                    logger.User(totalSize, currentSize, item);
                }

                // Now find all folders that are empty, if we are supposed to
                if (addBlanks)
                    ProcessDirectoryBlanks(basePath);
            }
            else if (File.Exists(basePath))
            {
                logger.Verbose($"File found: {basePath}");

                totalSize = new FileInfo(basePath).Length;
                logger.User(totalSize, currentSize);

                string parentPath = Path.GetDirectoryName(Path.GetDirectoryName(basePath));
                CheckFileForHashes(basePath, parentPath, asFiles, skipFileType, addBlanks, hashes);
                logger.User(totalSize, totalSize, basePath);
            }

            // Now that we're done, delete the temp folder (if it's not the default)
            logger.User("Cleaning temp folder");
            if (Globals.TempDir != Path.GetTempPath())
                DirectoryExtensions.TryDelete(Globals.TempDir);

            return true;
        }

        /// <summary>
        /// Check a given file for hashes, based on current settings
        /// </summary>
        /// <param name="item">Filename of the item to be checked</param>
        /// <param name="basePath">Base folder to be used in creating the DAT</param>
        /// <param name="asFiles">TreatAsFiles representing CHD and Archive scanning</param>
        /// <param name="skipFileType">Type of files that should be skipped</param>
        /// <param name="addBlanks">True if blank items should be created for empty folders, false otherwise</param>
        /// <param name="hashes">Hashes to include in the information</param>
        private void CheckFileForHashes(string item, string basePath, TreatAsFile asFiles, SkipFileType skipFileType, bool addBlanks, Hash hashes)
        {
            // If we're in depot mode, process it separately
            if (CheckDepotFile(item))
                return;

            // Initialize possible archive variables
            BaseArchive archive = BaseArchive.Create(item);

            // Process archives according to flags
            if (archive != null)
            {
                // Set the archive flags
                archive.AvailableHashes = hashes;

                // Skip if we're treating archives as files and skipping files
                if (asFiles.HasFlag(TreatAsFile.Archive) && skipFileType == SkipFileType.File)
                {
                    return;
                }

                // Skip if we're skipping archives
                else if (skipFileType == SkipFileType.Archive)
                {
                    return;
                }

                // Process as archive if we're not treating archives as files
                else if (!asFiles.HasFlag(TreatAsFile.Archive))
                {
                    var extracted = archive.GetChildren();

                    // If we have internal items to process, do so
                    if (extracted != null)
                        ProcessArchive(item, basePath, extracted);

                    // Now find all folders that are empty, if we are supposed to
                    if (addBlanks)
                        ProcessArchiveBlanks(item, basePath, archive);
                }

                // Process as file if we're treating archives as files
                else
                {
                    ProcessFile(item, basePath, hashes, asFiles);
                }
            }

            // Process non-archives according to flags
            else
            {
                // Skip if we're skipping files
                if (skipFileType == SkipFileType.File)
                    return;

                // Process as file
                else
                    ProcessFile(item, basePath, hashes, asFiles);
            }
        }

        /// <summary>
        /// Check an item as if it's supposed to be in a depot
        /// </summary>
        /// <param name="item">Filename of the item to be checked</param>
        /// <returns>True if we checked a depot file, false otherwise</returns>
        private bool CheckDepotFile(string item)
        {
            // If we're not in Depot mode, return false
            if (Header.OutputDepot?.IsActive != true)
                return false;

            // Check the file as if it were in a depot
            GZipArchive gzarc = new GZipArchive(item);
            BaseFile baseFile = gzarc.GetTorrentGZFileInfo();

            // If the rom is valid, add it
            if (baseFile != null && baseFile.Filename != null)
            {
                // Add the list if it doesn't exist already
                Rom rom = new Rom(baseFile);
                Items.Add(rom.GetKey(Field.DatItem_CRC), rom);
                logger.Verbose($"File added: {Path.GetFileNameWithoutExtension(item)}");
            }
            else
            {
                logger.Verbose($"File not added: {Path.GetFileNameWithoutExtension(item)}");
                return true;
            }

            return true;
        }

        /// <summary>
        /// Process a single file as an archive
        /// </summary>
        /// <param name="item">File to be added</param>
        /// <param name="basePath">Path the represents the parent directory</param>
        /// <param name="extracted">List of BaseFiles representing the internal files</param>
        private void ProcessArchive(string item, string basePath, List<BaseFile> extracted)
        {
            // Get the parent path for all items
            string parent = (Path.GetDirectoryName(Path.GetFullPath(item)) + Path.DirectorySeparatorChar).Remove(0, basePath.Length) + Path.GetFileNameWithoutExtension(item);

            // First take care of the found items
            Parallel.ForEach(extracted, Globals.ParallelOptions, baseFile =>
            {
                DatItem datItem = DatItem.Create(baseFile);
                ProcessFileHelper(item, datItem, basePath, parent);
            });
        }

        /// <summary>
        /// Process blank folders in an archive
        /// </summary>
        /// <param name="item">File containing the blanks</param>
        /// <param name="basePath">Path the represents the parent directory</param>
        /// <param name="archive">BaseArchive to get blanks from</param>
        private void ProcessArchiveBlanks(string item, string basePath, BaseArchive archive)
        {
            List<string> empties = new List<string>();

            // Get the parent path for all items
            string parent = (Path.GetDirectoryName(Path.GetFullPath(item)) + Path.DirectorySeparatorChar).Remove(0, basePath.Length) + Path.GetFileNameWithoutExtension(item);

            // Now get all blank folders from the archive
            if (archive != null)
                empties = archive.GetEmptyFolders();

            // Add add all of the found empties to the DAT
            Parallel.ForEach(empties, Globals.ParallelOptions, empty =>
            {
                Rom emptyRom = new Rom(Path.Combine(empty, "_"), item);
                ProcessFileHelper(item, emptyRom, basePath, parent);
            });
        }

        /// <summary>
        /// Process blank folders in a directory
        /// </summary>
        /// <param name="basePath">Path the represents the parent directory</param>
        private void ProcessDirectoryBlanks(string basePath)
        {
            // If we're in depot mode, we don't process blanks
            if (Header.OutputDepot?.IsActive == true)
                return;

            List<string> empties = DirectoryExtensions.ListEmpty(basePath);
            Parallel.ForEach(empties, Globals.ParallelOptions, dir =>
            {
                // Get the full path for the directory
                string fulldir = Path.GetFullPath(dir);

                // Set the temporary variables
                string gamename = string.Empty;
                string romname = string.Empty;

                // If we have a SuperDAT, we want anything that's not the base path as the game, and the file as the rom
                if (Header.Type == "SuperDAT")
                {
                    gamename = fulldir.Remove(0, basePath.Length + 1);
                    romname = "_";
                }

                // Otherwise, we want just the top level folder as the game, and the file as everything else
                else
                {
                    gamename = fulldir.Remove(0, basePath.Length + 1).Split(Path.DirectorySeparatorChar)[0];
                    romname = Path.Combine(fulldir.Remove(0, basePath.Length + 1 + gamename.Length), "_");
                }

                // Sanitize the names
                gamename = gamename.Trim(Path.DirectorySeparatorChar);
                romname = romname.Trim(Path.DirectorySeparatorChar);

                logger.Verbose($"Adding blank empty folder: {gamename}");
                Items["null"].Add(new Rom(romname, gamename));
            });
        }

        /// <summary>
        /// Process a single file as a file
        /// </summary>
        /// <param name="item">File to be added</param>
        /// <param name="basePath">Path the represents the parent directory</param>
        /// <param name="hashes">Hashes to include in the information</param>
        /// <param name="asFiles">TreatAsFiles representing CHD and Archive scanning</param>
        private void ProcessFile(string item, string basePath, Hash hashes, TreatAsFile asFiles)
        {
            logger.Verbose($"'{Path.GetFileName(item)}' treated like a file");
            BaseFile baseFile = FileExtensions.GetInfo(item, header: Header.HeaderSkipper, hashes: hashes, asFiles: asFiles);
            DatItem datItem = DatItem.Create(baseFile);
            ProcessFileHelper(item, datItem, basePath, string.Empty);
        }

        /// <summary>
        /// Process a single file as a file (with found Rom data)
        /// </summary>
        /// <param name="item">File to be added</param>
        /// <param name="item">Rom data to be used to write to file</param>
        /// <param name="basepath">Path the represents the parent directory</param>
        /// <param name="parent">Parent game to be used</param>
        private void ProcessFileHelper(string item, DatItem datItem, string basepath, string parent)
        {
            // If we didn't get an accepted parsed type somehow, cancel out
            List<ItemType> parsed = new List<ItemType> { ItemType.Disk, ItemType.Media, ItemType.Rom };
            if (!parsed.Contains(datItem.ItemType))
                return;

            try
            {
                // If the basepath doesn't end with a directory separator, add it
                if (!basepath.EndsWith(Path.DirectorySeparatorChar.ToString()))
                    basepath += Path.DirectorySeparatorChar.ToString();

                // Make sure we have the full item path
                item = Path.GetFullPath(item);

                // Process the item to sanitize names based on input
                SetDatItemInfo(datItem, item, parent, basepath);

                // Add the file information to the DAT
                string key = datItem.GetKey(Field.DatItem_CRC);
                Items.Add(key, datItem);

                logger.Verbose($"File added: {datItem.GetName() ?? string.Empty}");
            }
            catch (IOException ex)
            {
                logger.Error(ex);
                return;
            }
        }

        /// <summary>
        /// Set proper Game and Rom names from user inputs
        /// </summary>
        /// <param name="datItem">DatItem representing the input file</param>
        /// <param name="item">Item name to use</param>
        /// <param name="parent">Parent name to use</param>
        /// <param name="basepath">Base path to use</param>
        private void SetDatItemInfo(DatItem datItem, string item, string parent, string basepath)
        {
            // Get the data to be added as game and item names
            string machineName, itemName;

            // If the parent is blank, then we have a non-archive file
            if (string.IsNullOrWhiteSpace(parent))
            {
                // If we have a SuperDAT, we want anything that's not the base path as the game, and the file as the rom
                if (Header.Type == "SuperDAT")
                {
                    machineName = Path.GetDirectoryName(item.Remove(0, basepath.Length));
                    itemName = Path.GetFileName(item);
                }

                // Otherwise, we want just the top level folder as the game, and the file as everything else
                else
                {
                    machineName = item.Remove(0, basepath.Length).Split(Path.DirectorySeparatorChar)[0];
                    itemName = item.Remove(0, (Path.Combine(basepath, machineName).Length));
                }
            }

            // Otherwise, we assume that we have an archive
            else
            {
                // If we have a SuperDAT, we want the archive name as the game, and the file as everything else (?)
                if (Header.Type == "SuperDAT")
                {
                    machineName = parent;
                    itemName = datItem.GetName();
                }

                // Otherwise, we want the archive name as the game, and the file as everything else
                else
                {
                    machineName = parent;
                    itemName = datItem.GetName();
                }
            }

            // Sanitize the names
            machineName = machineName.Trim(Path.DirectorySeparatorChar);
            itemName = itemName?.Trim(Path.DirectorySeparatorChar) ?? string.Empty;

            if (!string.IsNullOrWhiteSpace(machineName) && string.IsNullOrWhiteSpace(itemName))
            {
                itemName = machineName;
                machineName = "Default";
            }

            // Update machine information
            datItem.Machine.Name = machineName;
            datItem.Machine.Description = machineName;

            // If we have a Disk, then the ".chd" extension needs to be removed
            if (datItem.ItemType == ItemType.Disk && itemName.EndsWith(".chd"))
            {
                itemName = itemName.Substring(0, itemName.Length - 4);
            }

            // If we have a Media, then the extension needs to be removed
            else if (datItem.ItemType == ItemType.Media)
            {
                if (itemName.EndsWith(".dicf"))
                    itemName = itemName.Substring(0, itemName.Length - 5);
                else if (itemName.EndsWith(".aaru"))
                    itemName = itemName.Substring(0, itemName.Length - 5);
                else if (itemName.EndsWith(".aaruformat"))
                    itemName = itemName.Substring(0, itemName.Length - 11);
                else if (itemName.EndsWith(".aaruf"))
                    itemName = itemName.Substring(0, itemName.Length - 6);
                else if (itemName.EndsWith(".aif"))
                    itemName = itemName.Substring(0, itemName.Length - 4);
            }

            // Set the item name back
            datItem.SetFields(new Dictionary<Field, string> { [Field.DatItem_Name] = itemName });
        }

        #endregion

        #region Rebuilding and Verifying

        /// <summary>
        /// Process the DAT and find all matches in input files and folders assuming they're a depot
        /// </summary>
        /// <param name="inputs">List of input files/folders to check</param>
        /// <param name="outDir">Output directory to use to build to</param>
        /// <param name="date">True if the date from the DAT should be used if available, false otherwise</param>
        /// <param name="delete">True if input files should be deleted, false otherwise</param>
        /// <param name="inverse">True if the DAT should be used as a filter instead of a template, false otherwise</param>
        /// <param name="outputFormat">Output format that files should be written to</param>
        /// <returns>True if rebuilding was a success, false otherwise</returns>
        public bool RebuildDepot(
            List<string> inputs,
            string outDir,
            bool date = false,
            bool delete = false,
            bool inverse = false,
            OutputFormat outputFormat = OutputFormat.Folder)
        {
            #region Perform setup

            // If the DAT is not populated and inverse is not set, inform the user and quit
            if (Items.TotalCount == 0 && !inverse)
            {
                logger.User("No entries were found to rebuild, exiting...");
                return false;
            }

            // Check that the output directory exists
            outDir = DirectoryExtensions.Ensure(outDir, create: true);

            // Now we want to get forcepack flag if it's not overridden
            if (outputFormat == OutputFormat.Folder && Header.ForcePacking != PackingFlag.None)
                outputFormat = Header.ForcePacking.AsOutputFormat();

            // Preload the Skipper list
            Transform.Init();

            #endregion

            bool success = true;

            #region Rebuild from depots in order

            string format = outputFormat.FromOutputFormat() ?? string.Empty;
            InternalStopwatch watch = new InternalStopwatch($"Rebuilding all files to {format}");

            // Now loop through and get only directories from the input paths
            List<string> directories = new List<string>();
            Parallel.ForEach(inputs, Globals.ParallelOptions, input =>
            {
                // Add to the list if the input is a directory
                if (Directory.Exists(input))
                {
                    logger.Verbose($"Adding depot: {input}");
                    lock (directories)
                    {
                        directories.Add(input);
                    }
                }
            });

            // If we don't have any directories, we want to exit
            if (directories.Count == 0)
                return success;

            // Now that we have a list of depots, we want to bucket the input DAT by SHA-1
            Items.BucketBy(Field.DatItem_SHA1, DedupeType.None);

            // Then we want to loop through each of the hashes and see if we can rebuild
            var keys = Items.SortedKeys.ToList();
            foreach (string hash in keys)
            {
                // Pre-empt any issues that could arise from string length
                if (hash.Length != Constants.SHA1Length)
                    continue;

                logger.User($"Checking hash '{hash}'");

                // Get the extension path for the hash
                string subpath = PathExtensions.GetDepotPath(hash, Header.InputDepot.Depth);

                // Find the first depot that includes the hash
                string foundpath = null;
                foreach (string directory in directories)
                {
                    if (File.Exists(Path.Combine(directory, subpath)))
                    {
                        foundpath = Path.Combine(directory, subpath);
                        break;
                    }
                }

                // If we didn't find a path, then we continue
                if (foundpath == null)
                    continue;

                // If we have a path, we want to try to get the rom information
                GZipArchive archive = new GZipArchive(foundpath);
                BaseFile fileinfo = archive.GetTorrentGZFileInfo();

                // If the file information is null, then we continue
                if (fileinfo == null)
                    continue;

                // Ensure we are sorted correctly (some other calls can change this)
                Items.BucketBy(Field.DatItem_SHA1, DedupeType.None);

                // If there are no items in the hash, we continue
                if (Items[hash] == null || Items[hash].Count == 0)
                    continue;

                // Otherwise, we rebuild that file to all locations that we need to
                bool usedInternally;
                if (Items[hash][0].ItemType == ItemType.Disk)
                    usedInternally = RebuildIndividualFile(new Disk(fileinfo), foundpath, outDir, date, inverse, outputFormat, false /* isZip */);
                else if (Items[hash][0].ItemType == ItemType.Media)
                    usedInternally = RebuildIndividualFile(new Media(fileinfo), foundpath, outDir, date, inverse, outputFormat, false /* isZip */);
                else
                    usedInternally = RebuildIndividualFile(new Rom(fileinfo), foundpath, outDir, date, inverse, outputFormat, false /* isZip */);

                // If we are supposed to delete the depot file, do so
                if (delete && usedInternally)
                    FileExtensions.TryDelete(foundpath);
            }

            watch.Stop();

            #endregion

            return success;
        }

        /// <summary>
        /// Process the DAT and find all matches in input files and folders
        /// </summary>
        /// <param name="inputs">List of input files/folders to check</param>
        /// <param name="outDir">Output directory to use to build to</param>
        /// <param name="quickScan">True to enable external scanning of archives, false otherwise</param>
        /// <param name="date">True if the date from the DAT should be used if available, false otherwise</param>
        /// <param name="delete">True if input files should be deleted, false otherwise</param>
        /// <param name="inverse">True if the DAT should be used as a filter instead of a template, false otherwise</param>
        /// <param name="outputFormat">Output format that files should be written to</param>
        /// <param name="asFiles">TreatAsFiles representing special format scanning</param>
        /// <returns>True if rebuilding was a success, false otherwise</returns>
        public bool RebuildGeneric(
            List<string> inputs,
            string outDir,
            bool quickScan = false,
            bool date = false,
            bool delete = false,
            bool inverse = false,
            OutputFormat outputFormat = OutputFormat.Folder,
            TreatAsFile asFiles = 0x00)
        {
            #region Perform setup

            // If the DAT is not populated and inverse is not set, inform the user and quit
            if (Items.TotalCount == 0 && !inverse)
            {
                logger.User("No entries were found to rebuild, exiting...");
                return false;
            }

            // Check that the output directory exists
            if (!Directory.Exists(outDir))
            {
                Directory.CreateDirectory(outDir);
                outDir = Path.GetFullPath(outDir);
            }

            // Now we want to get forcepack flag if it's not overridden
            if (outputFormat == OutputFormat.Folder && Header.ForcePacking != PackingFlag.None)
                outputFormat = Header.ForcePacking.AsOutputFormat();

            // Preload the Skipper list
            Transform.Init();

            #endregion

            bool success = true;

            #region Rebuild from sources in order

            string format = outputFormat.FromOutputFormat() ?? string.Empty;
            InternalStopwatch watch = new InternalStopwatch($"Rebuilding all files to {format}");

            // Now loop through all of the files in all of the inputs
            foreach (string input in inputs)
            {
                // If the input is a file
                if (File.Exists(input))
                {
                    logger.User($"Checking file: {input}");
                    bool rebuilt = RebuildGenericHelper(input, outDir, quickScan, date, inverse, outputFormat, asFiles);

                    // If we are supposed to delete the file, do so
                    if (delete && rebuilt)
                        FileExtensions.TryDelete(input);
                }

                // If the input is a directory
                else if (Directory.Exists(input))
                {
                    logger.Verbose($"Checking directory: {input}");
                    foreach (string file in Directory.EnumerateFiles(input, "*", SearchOption.AllDirectories))
                    {
                        logger.User($"Checking file: {file}");
                        bool rebuilt = RebuildGenericHelper(file, outDir, quickScan, date, inverse, outputFormat, asFiles);

                        // If we are supposed to delete the file, do so
                        if (delete && rebuilt)
                            FileExtensions.TryDelete(input);
                    }
                }
            }

            watch.Stop();

            #endregion

            return success;
        }

        /// <summary>
        /// Attempt to add a file to the output if it matches
        /// </summary>
        /// <param name="file">Name of the file to process</param>
        /// <param name="outDir">Output directory to use to build to</param>
        /// <param name="quickScan">True to enable external scanning of archives, false otherwise</param>
        /// <param name="date">True if the date from the DAT should be used if available, false otherwise</param>
        /// <param name="inverse">True if the DAT should be used as a filter instead of a template, false otherwise</param>
        /// <param name="outputFormat">Output format that files should be written to</param>
        /// <param name="asFiles">TreatAsFiles representing special format scanning</param>
        /// <returns>True if the file was used to rebuild, false otherwise</returns>
        private bool RebuildGenericHelper(
            string file,
            string outDir,
            bool quickScan,
            bool date,
            bool inverse,
            OutputFormat outputFormat,
            TreatAsFile asFiles)
        {
            // If we somehow have a null filename, return
            if (file == null)
                return false;

            // Set the deletion variables
            bool usedExternally = false, usedInternally = false;

            // Create an empty list of BaseFile for archive entries
            List<BaseFile> entries = null;

            // Get the TGZ and TXZ status for later
            GZipArchive tgz = new GZipArchive(file);
            XZArchive txz = new XZArchive(file);
            bool isSingleTorrent = tgz.IsTorrent() || txz.IsTorrent();

            // Get the base archive first
            BaseArchive archive = BaseArchive.Create(file);

            // Now get all extracted items from the archive
            if (archive != null)
            {
                archive.AvailableHashes = quickScan ? Hash.CRC : Hash.Standard;
                entries = archive.GetChildren();
            }

            // If the entries list is null, we encountered an error or have a file and should scan externally
            if (entries == null && File.Exists(file))
            {
                BaseFile internalFileInfo = FileExtensions.GetInfo(file, asFiles: asFiles);

                // Create the correct DatItem
                DatItem internalDatItem;
                if (internalFileInfo.Type == FileType.AaruFormat && !asFiles.HasFlag(TreatAsFile.AaruFormat))
                    internalDatItem = new Media(internalFileInfo);
                else if (internalFileInfo.Type == FileType.CHD && !asFiles.HasFlag(TreatAsFile.CHD))
                    internalDatItem = new Disk(internalFileInfo);
                else
                    internalDatItem = new Rom(internalFileInfo);

                usedExternally = RebuildIndividualFile(internalDatItem, file, outDir, date, inverse, outputFormat);
            }
            // Otherwise, loop through the entries and try to match
            else
            {
                foreach (BaseFile entry in entries)
                {
                    DatItem internalDatItem = DatItem.Create(entry);
                    usedInternally |= RebuildIndividualFile(internalDatItem, file, outDir, date, inverse, outputFormat, !isSingleTorrent /* isZip */);
                }
            }

            return usedExternally || usedInternally;
        }

        /// <summary>
        /// Find duplicates and rebuild individual files to output
        /// </summary>
        /// <param name="datItem">Information for the current file to rebuild from</param>
        /// <param name="file">Name of the file to process</param>
        /// <param name="outDir">Output directory to use to build to</param>
        /// <param name="date">True if the date from the DAT should be used if available, false otherwise</param>
        /// <param name="inverse">True if the DAT should be used as a filter instead of a template, false otherwise</param>
        /// <param name="outputFormat">Output format that files should be written to</param>
        /// <param name="isZip">True if the input file is an archive, false if the file is TGZ/TXZ, null otherwise</param>
        /// <returns>True if the file was able to be rebuilt, false otherwise</returns>
        private bool RebuildIndividualFile(
            DatItem datItem,
            string file,
            string outDir,
            bool date,
            bool inverse,
            OutputFormat outputFormat,
            bool? isZip = null)
        {
            // Set the initial output value
            bool rebuilt = false;

            // If the DatItem is a Disk or Media, force rebuilding to a folder except if TGZ or TXZ
            if ((datItem.ItemType == ItemType.Disk || datItem.ItemType == ItemType.Media)
                && !(outputFormat == OutputFormat.TorrentGzip || outputFormat == OutputFormat.TorrentGzipRomba)
                && !(outputFormat == OutputFormat.TorrentXZ || outputFormat == OutputFormat.TorrentXZRomba))
            {
                outputFormat = OutputFormat.Folder;
            }

            // If we have a Disk or Media, change it into a Rom for later use
            if (datItem.ItemType == ItemType.Disk)
                datItem = (datItem as Disk).ConvertToRom();
            else if (datItem.ItemType == ItemType.Media)
                datItem = (datItem as Media).ConvertToRom();

            // Prepopluate a key string
            string crc = (datItem as Rom).CRC ?? string.Empty;

            // Try to get the stream for the file
            if (!GetFileStream(datItem, file, isZip, out Stream fileStream))
                return false;

            // If either we have duplicates or we're filtering
            if (ShouldRebuild(datItem, fileStream, inverse, out List<DatItem> dupes))
            {
                // If we have a very specific TGZ->TGZ case, just copy it accordingly
                if (RebuildTorrentGzip(datItem, file, outDir, outputFormat, isZip))
                    return true;

                // If we have a very specific TXZ->TXZ case, just copy it accordingly
                if (RebuildTorrentXz(datItem, file, outDir, outputFormat, isZip))
                    return true;

                logger.User($"{(inverse ? "No matches" : "Matches")} found for '{Path.GetFileName(datItem.GetName() ?? datItem.ItemType.ToString())}', rebuilding accordingly...");
                rebuilt = true;

                // Special case for partial packing mode
                bool shouldCheck = false;
                if (outputFormat == OutputFormat.Folder && Header.ForcePacking == PackingFlag.Partial)
                {
                    shouldCheck = true;
                    Items.BucketBy(Field.Machine_Name, DedupeType.None, lower: false);
                }

                // Now loop through the list and rebuild accordingly
                foreach (DatItem item in dupes)
                {
                    // If we should check for the items in the machine
                    if (shouldCheck && Items[item.Machine.Name].Count > 1)
                        outputFormat = OutputFormat.Folder;
                    else if (shouldCheck && Items[item.Machine.Name].Count == 1)
                        outputFormat = OutputFormat.ParentFolder;

                    // Get the output archive, if possible
                    Folder outputArchive = GetPreconfiguredFolder(date, outputFormat);

                    // Now rebuild to the output file
                    outputArchive.Write(fileStream, outDir, item as Rom);
                }

                // Close the input stream
                fileStream?.Dispose();
            }

            // Now we want to take care of headers, if applicable
            if (Header.HeaderSkipper != null)
            {
                // Check to see if we have a matching header first
                SkipperRule rule = Transform.GetMatchingRule(fileStream, Path.GetFileNameWithoutExtension(Header.HeaderSkipper));

                // If there's a match, create the new file to write
                if (rule.Tests != null && rule.Tests.Count != 0)
                {
                    // If the file could be transformed correctly
                    MemoryStream transformStream = new MemoryStream();
                    if (rule.TransformStream(fileStream, transformStream, keepReadOpen: true, keepWriteOpen: true))
                    {
                        // Get the file informations that we will be using
                        Rom headerless = new Rom(transformStream.GetInfo(keepReadOpen: true));

                        // If we have duplicates and we're not filtering
                        if (ShouldRebuild(headerless, transformStream, false, out dupes))
                        {
                            logger.User($"Headerless matches found for '{Path.GetFileName(datItem.GetName() ?? datItem.ItemType.ToString())}', rebuilding accordingly...");
                            rebuilt = true;

                            // Now loop through the list and rebuild accordingly
                            foreach (DatItem item in dupes)
                            {
                                // Create a headered item to use as well
                                datItem.CopyMachineInformation(item);
                                datItem.SetFields(new Dictionary<Field, string> { [Field.DatItem_Name] = $"{datItem.GetName()}_{crc}" });

                                // Get the output archive, if possible
                                Folder outputArchive = GetPreconfiguredFolder(date, outputFormat);

                                // Now rebuild to the output file
                                bool eitherSuccess = false;
                                eitherSuccess |= outputArchive.Write(transformStream, outDir, item as Rom);
                                eitherSuccess |= outputArchive.Write(fileStream, outDir, datItem as Rom);

                                // Now add the success of either rebuild
                                rebuilt &= eitherSuccess;
                            }
                        }
                    }

                    // Dispose of the stream
                    transformStream?.Dispose();
                }

                // Dispose of the stream
                fileStream?.Dispose();
            }

            return rebuilt;
        }

        /// <summary>
        /// Get the rebuild state for a given item
        /// </summary>
        /// <param name="datItem">Information for the current file to rebuild from</param>
        /// <param name="stream">Stream representing the input file</param>
        /// <param name="inverse">True if the DAT should be used as a filter instead of a template, false otherwise</param>
        /// <param name="dupes">Output list of duplicate items to rebuild to</param>
        /// <returns>True if the item should be rebuilt, false otherwise</returns>
        private bool ShouldRebuild(DatItem datItem, Stream stream, bool inverse, out List<DatItem> dupes)
        {
            // Find if the file has duplicates in the DAT
            dupes = Items.GetDuplicates(datItem);
            bool hasDuplicates = dupes.Count > 0;

            // If we have duplicates but we're filtering
            if (hasDuplicates && inverse)
            {
                return false;
            }

            // If we have duplicates without filtering
            else if (hasDuplicates && !inverse)
            {
                return true;
            }

            // If we have no duplicates and we're filtering
            else if (!hasDuplicates && inverse)
            {
                string machinename = null;

                // Get the item from the current file
                Rom item = new Rom(stream.GetInfo(keepReadOpen: true));
                item.Machine.Name = Path.GetFileNameWithoutExtension(item.Name);
                item.Machine.Description = Path.GetFileNameWithoutExtension(item.Name);

                // If we are coming from an archive, set the correct machine name
                if (machinename != null)
                {
                    item.Machine.Name = machinename;
                    item.Machine.Description = machinename;
                }

                dupes.Add(item);
                return true;
            }

            // If we have no duplicates and we're not filtering
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Rebuild from TorrentGzip to TorrentGzip
        /// </summary>
        /// <param name="datItem">Information for the current file to rebuild from</param>
        /// <param name="file">Name of the file to process</param>
        /// <param name="outDir">Output directory to use to build to</param>
        /// <param name="outputFormat">Output format that files should be written to</param>
        /// <param name="isZip">True if the input file is an archive, false if the file is TGZ, null otherwise</param>
        /// <returns>True if rebuilt properly, false otherwise</returns>
        private bool RebuildTorrentGzip(DatItem datItem, string file, string outDir, OutputFormat outputFormat, bool? isZip)
        {
            // If we have a very specific TGZ->TGZ case, just copy it accordingly
            GZipArchive tgz = new GZipArchive(file);
            BaseFile tgzRom = tgz.GetTorrentGZFileInfo();
            if (isZip == false && tgzRom != null && (outputFormat == OutputFormat.TorrentGzip || outputFormat == OutputFormat.TorrentGzipRomba))
            {
                logger.User($"Matches found for '{Path.GetFileName(datItem.GetName() ?? string.Empty)}', rebuilding accordingly...");

                // Get the proper output path
                string sha1 = (datItem as Rom).SHA1 ?? string.Empty;
                if (outputFormat == OutputFormat.TorrentGzipRomba)
                    outDir = Path.Combine(outDir, PathExtensions.GetDepotPath(sha1, Header.OutputDepot.Depth));
                else
                    outDir = Path.Combine(outDir, sha1 + ".gz");

                // Make sure the output folder is created
                Directory.CreateDirectory(Path.GetDirectoryName(outDir));

                // Now copy the file over
                try
                {
                    File.Copy(file, outDir);
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// Rebuild from TorrentXz to TorrentXz
        /// </summary>
        /// <param name="datItem">Information for the current file to rebuild from</param>
        /// <param name="file">Name of the file to process</param>
        /// <param name="outDir">Output directory to use to build to</param>
        /// <param name="outputFormat">Output format that files should be written to</param>
        /// <param name="isZip">True if the input file is an archive, false if the file is TXZ, null otherwise</param>
        /// <returns>True if rebuilt properly, false otherwise</returns>
        private bool RebuildTorrentXz(DatItem datItem, string file, string outDir, OutputFormat outputFormat, bool? isZip)
        {
            // If we have a very specific TGZ->TGZ case, just copy it accordingly
            XZArchive txz = new XZArchive(file);
            BaseFile txzRom = txz.GetTorrentXZFileInfo();
            if (isZip == false && txzRom != null && (outputFormat == OutputFormat.TorrentXZ || outputFormat == OutputFormat.TorrentXZRomba))
            {
                logger.User($"Matches found for '{Path.GetFileName(datItem.GetName() ?? string.Empty)}', rebuilding accordingly...");

                // Get the proper output path
                string sha1 = (datItem as Rom).SHA1 ?? string.Empty;
                if (outputFormat == OutputFormat.TorrentXZRomba)
                    outDir = Path.Combine(outDir, PathExtensions.GetDepotPath(sha1, Header.OutputDepot.Depth)).Replace(".gz", ".xz");
                else
                    outDir = Path.Combine(outDir, sha1 + ".xz");

                // Make sure the output folder is created
                Directory.CreateDirectory(Path.GetDirectoryName(outDir));

                // Now copy the file over
                try
                {
                    File.Copy(file, outDir);
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// Get the Stream related to a file
        /// </summary>
        /// <param name="datItem">Information for the current file to rebuild from</param>
        /// <param name="file">Name of the file to process</param>
        /// <param name="isZip">Non-null if the input file is an archive</param>
        /// <param name="stream">Output stream representing the opened file</param>
        /// <returns>True if the stream opening succeeded, false otherwise</returns>
        private bool GetFileStream(DatItem datItem, string file, bool? isZip, out Stream stream)
        {
            // Get a generic stream for the file
            stream = null;

            // If we have a zipfile, extract the stream to memory
            if (isZip != null)
            {
                BaseArchive archive = BaseArchive.Create(file);
                if (archive != null)
                    (stream, _) = archive.CopyToStream(datItem.GetName() ?? datItem.ItemType.ToString());
            }
            // Otherwise, just open the filestream
            else
            {
                stream = FileExtensions.TryOpenRead(file);
            }

            // If the stream is null, then continue
            if (stream == null)
                return false;

            // Seek to the beginning of the stream
            if (stream.CanSeek)
                stream.Seek(0, SeekOrigin.Begin);

            return true;
        }

        /// <summary>
        /// Get preconfigured Folder for rebuilding
        /// </summary>
        /// <param name="date">True if the date from the DAT should be used if available, false otherwise</param>
        /// <param name="outputFormat">Output format that files should be written to</param>
        /// <returns>Folder configured with proper flags</returns>
        private Folder GetPreconfiguredFolder(bool date, OutputFormat outputFormat)
        {
            Folder outputArchive = Folder.Create(outputFormat);
            if (outputArchive is BaseArchive baseArchive && date)
                baseArchive.UseDates = date;

            // Set the depth fields where appropriate
            if (outputArchive is GZipArchive gzipArchive)
                gzipArchive.Depth = Header.OutputDepot.Depth;
            else if (outputArchive is XZArchive xzArchive)
                xzArchive.Depth = Header.OutputDepot.Depth;

            return outputArchive;
        }

        /// <summary>
        /// Verify a DatFile against a set of depots, leaving only missing files
        /// </summary>
        /// <param name="inputs">List of input directories to compare against</param>
        /// <returns>True if verification was a success, false otherwise</returns>
        public bool VerifyDepot(List<string> inputs)
        {
            bool success = true;

            InternalStopwatch watch = new InternalStopwatch("Verifying all from supplied depots");

            // Now loop through and get only directories from the input paths
            List<string> directories = new List<string>();
            foreach (string input in inputs)
            {
                // Add to the list if the input is a directory
                if (Directory.Exists(input))
                {
                    logger.Verbose($"Adding depot: {input}");
                    directories.Add(input);
                }
            }

            // If we don't have any directories, we want to exit
            if (directories.Count == 0)
                return success;

            // Now that we have a list of depots, we want to bucket the input DAT by SHA-1
            Items.BucketBy(Field.DatItem_SHA1, DedupeType.None);

            // Then we want to loop through each of the hashes and see if we can rebuild
            var keys = Items.SortedKeys.ToList();
            foreach (string hash in keys)
            {
                // Pre-empt any issues that could arise from string length
                if (hash.Length != Constants.SHA1Length)
                    continue;

                logger.User($"Checking hash '{hash}'");

                // Get the extension path for the hash
                string subpath = PathExtensions.GetDepotPath(hash, Header.InputDepot.Depth);

                // Find the first depot that includes the hash
                string foundpath = null;
                foreach (string directory in directories)
                {
                    if (File.Exists(Path.Combine(directory, subpath)))
                    {
                        foundpath = Path.Combine(directory, subpath);
                        break;
                    }
                }

                // If we didn't find a path, then we continue
                if (foundpath == null)
                    continue;

                // If we have a path, we want to try to get the rom information
                GZipArchive tgz = new GZipArchive(foundpath);
                BaseFile fileinfo = tgz.GetTorrentGZFileInfo();

                // If the file information is null, then we continue
                if (fileinfo == null)
                    continue;

                // Now we want to remove all duplicates from the DAT
                Items.GetDuplicates(new Rom(fileinfo))
                    .AddRange(Items.GetDuplicates(new Disk(fileinfo)));
            }

            watch.Stop();

            // Set fixdat headers in case of writing out
            Header.FileName = $"fixDAT_{Header.FileName}";
            Header.Name = $"fixDAT_{Header.Name}";
            Header.Description = $"fixDAT_{Header.Description}";
            Items.ClearMarked();

            return success;
        }

        /// <summary>
        /// Verify a DatFile against a set of inputs, leaving only missing files
        /// </summary>
        /// <param name="hashOnly">True if only hashes should be checked, false for full file information</param>
        /// <returns>True if verification was a success, false otherwise</returns>
        public bool VerifyGeneric(bool hashOnly)
        {
            bool success = true;

            // Force bucketing according to the flags
            Items.SetBucketedBy(Field.NULL);
            if (hashOnly)
                Items.BucketBy(Field.DatItem_CRC, DedupeType.Full);
            else
                Items.BucketBy(Field.Machine_Name, DedupeType.Full);

            // Then mark items for removal
            var keys = Items.SortedKeys.ToList();
            foreach (string key in keys)
            {
                List<DatItem> items = Items[key];
                for (int i = 0; i < items.Count; i++)
                {
                    // Unmatched items will have a source ID of int.MaxValue, remove all others
                    if (items[i].Source.Index != int.MaxValue)
                        items[i].Remove = true;
                }

                // Set the list back, just in case
                Items[key] = items;
            }

            // Set fixdat headers in case of writing out
            Header.FileName = $"fixDAT_{Header.FileName}";
            Header.Name = $"fixDAT_{Header.Name}";
            Header.Description = $"fixDAT_{Header.Description}";
            Items.ClearMarked();

            return success;
        }

        #endregion

        // TODO: Implement Level split
        #region Splitting

        /// <summary>
        /// Split a DAT by input extensions
        /// </summary>
        /// <param name="extA">List of extensions to split on (first DAT)</param>
        /// <param name="extB">List of extensions to split on (second DAT)</param>
        /// <returns>Extension Set A and Extension Set B DatFiles</returns>
        public (DatFile extADat, DatFile extBDat) SplitByExtension(List<string> extA, List<string> extB)
        {
            // If roms is empty, return false
            if (Items.TotalCount == 0)
                return (null, null);

            // Make sure all of the extensions don't have a dot at the beginning
            var newExtA = extA.Select(s => s.TrimStart('.').ToLowerInvariant());
            string newExtAString = string.Join(",", newExtA);

            var newExtB = extB.Select(s => s.TrimStart('.').ToLowerInvariant());
            string newExtBString = string.Join(",", newExtB);

            // Set all of the appropriate outputs for each of the subsets
            DatFile extADat = Create(Header.CloneStandard());
            extADat.Header.FileName += $" ({newExtAString})";
            extADat.Header.Name += $" ({newExtAString})";
            extADat.Header.Description += $" ({newExtAString})";

            DatFile extBDat = Create(Header.CloneStandard());
            extBDat.Header.FileName += $" ({newExtBString})";
            extBDat.Header.Name += $" ({newExtBString})";
            extBDat.Header.Description += $" ({newExtBString})";

            // Now separate the roms accordingly
            Parallel.ForEach(Items.Keys, Globals.ParallelOptions, key =>
            {
                List<DatItem> items = Items[key];
                foreach (DatItem item in items)
                {
                    if (newExtA.Contains(PathExtensions.GetNormalizedExtension(item.GetName() ?? string.Empty)))
                    {
                        extADat.Items.Add(key, item);
                    }
                    else if (newExtB.Contains(PathExtensions.GetNormalizedExtension(item.GetName() ?? string.Empty)))
                    {
                        extBDat.Items.Add(key, item);
                    }
                    else
                    {
                        extADat.Items.Add(key, item);
                        extBDat.Items.Add(key, item);
                    }
                }
            });

            // Then return both DatFiles
            return (extADat, extBDat);
        }

        /// <summary>
        /// Split a DAT by best available hashes
        /// </summary>
        /// <returns>Dictionary of Field to DatFile mappings</returns>
        public Dictionary<Field, DatFile> SplitByHash()
        {
            // Create each of the respective output DATs
            logger.User("Creating and populating new DATs");

            // Create the set of field-to-dat mappings
            Dictionary<Field, DatFile> fieldDats = new Dictionary<Field, DatFile>();

            // TODO: Can this be made into a loop?
            fieldDats[Field.DatItem_Status] = Create(Header.CloneStandard());
            fieldDats[Field.DatItem_Status].Header.FileName += " (Nodump)";
            fieldDats[Field.DatItem_Status].Header.Name += " (Nodump)";
            fieldDats[Field.DatItem_Status].Header.Description += " (Nodump)";

            fieldDats[Field.DatItem_SHA512] = Create(Header.CloneStandard());
            fieldDats[Field.DatItem_SHA512].Header.FileName += " (SHA-512)";
            fieldDats[Field.DatItem_SHA512].Header.Name += " (SHA-512)";
            fieldDats[Field.DatItem_SHA512].Header.Description += " (SHA-512)";

            fieldDats[Field.DatItem_SHA384] = Create(Header.CloneStandard());
            fieldDats[Field.DatItem_SHA384].Header.FileName += " (SHA-384)";
            fieldDats[Field.DatItem_SHA384].Header.Name += " (SHA-384)";
            fieldDats[Field.DatItem_SHA384].Header.Description += " (SHA-384)";

            fieldDats[Field.DatItem_SHA256] = Create(Header.CloneStandard());
            fieldDats[Field.DatItem_SHA256].Header.FileName += " (SHA-256)";
            fieldDats[Field.DatItem_SHA256].Header.Name += " (SHA-256)";
            fieldDats[Field.DatItem_SHA256].Header.Description += " (SHA-256)";

            fieldDats[Field.DatItem_SHA1] = Create(Header.CloneStandard());
            fieldDats[Field.DatItem_SHA1].Header.FileName += " (SHA-1)";
            fieldDats[Field.DatItem_SHA1].Header.Name += " (SHA-1)";
            fieldDats[Field.DatItem_SHA1].Header.Description += " (SHA-1)";

#if NET_FRAMEWORK
            fieldDats[Field.DatItem_RIPEMD160] = Create(Header.CloneStandard());
            fieldDats[Field.DatItem_RIPEMD160].Header.FileName += " (RIPEMD160)";
            fieldDats[Field.DatItem_RIPEMD160].Header.Name += " (RIPEMD160)";
            fieldDats[Field.DatItem_RIPEMD160].Header.Description += " (RIPEMD160)";
#endif

            fieldDats[Field.DatItem_MD5] = Create(Header.CloneStandard());
            fieldDats[Field.DatItem_MD5].Header.FileName += " (MD5)";
            fieldDats[Field.DatItem_MD5].Header.Name += " (MD5)";
            fieldDats[Field.DatItem_MD5].Header.Description += " (MD5)";

            fieldDats[Field.DatItem_CRC] = Create(Header.CloneStandard());
            fieldDats[Field.DatItem_CRC].Header.FileName += " (CRC)";
            fieldDats[Field.DatItem_CRC].Header.Name += " (CRC)";
            fieldDats[Field.DatItem_CRC].Header.Description += " (CRC)";

            fieldDats[Field.NULL] = Create(Header.CloneStandard());
            fieldDats[Field.NULL].Header.FileName += " (Other)";
            fieldDats[Field.NULL].Header.Name += " (Other)";
            fieldDats[Field.NULL].Header.Description += " (Other)";

            // Now populate each of the DAT objects in turn
            Parallel.ForEach(Items.Keys, Globals.ParallelOptions, key =>
            {
                List<DatItem> items = Items[key];
                foreach (DatItem item in items)
                {
                    // If the file is not a Disk, Media, or Rom, continue
                    if (item.ItemType != ItemType.Disk && item.ItemType != ItemType.Media && item.ItemType != ItemType.Rom)
                        return;

                    // If the file is a nodump
                    if ((item.ItemType == ItemType.Rom && (item as Rom).ItemStatus == ItemStatus.Nodump)
                        || (item.ItemType == ItemType.Disk && (item as Disk).ItemStatus == ItemStatus.Nodump))
                    {
                        fieldDats[Field.DatItem_Status].Items.Add(key, item);
                    }

                    // If the file has a SHA-512
                    else if ((item.ItemType == ItemType.Rom && !string.IsNullOrWhiteSpace((item as Rom).SHA512)))
                    {
                        fieldDats[Field.DatItem_SHA512].Items.Add(key, item);
                    }

                    // If the file has a SHA-384
                    else if ((item.ItemType == ItemType.Rom && !string.IsNullOrWhiteSpace((item as Rom).SHA384)))
                    {
                        fieldDats[Field.DatItem_SHA384].Items.Add(key, item);
                    }

                    // If the file has a SHA-256
                    else if ((item.ItemType == ItemType.Media && !string.IsNullOrWhiteSpace((item as Media).SHA256))
                        || (item.ItemType == ItemType.Rom && !string.IsNullOrWhiteSpace((item as Rom).SHA256)))
                    {
                        fieldDats[Field.DatItem_SHA256].Items.Add(key, item);
                    }

                    // If the file has a SHA-1
                    else if ((item.ItemType == ItemType.Disk && !string.IsNullOrWhiteSpace((item as Disk).SHA1))
                        || (item.ItemType == ItemType.Media && !string.IsNullOrWhiteSpace((item as Media).SHA1))
                        || (item.ItemType == ItemType.Rom && !string.IsNullOrWhiteSpace((item as Rom).SHA1)))
                    {
                        fieldDats[Field.DatItem_SHA1].Items.Add(key, item);
                    }

#if NET_FRAMEWORK
                    // If the file has a RIPEMD160
                    else if ((item.ItemType == ItemType.Rom && !string.IsNullOrWhiteSpace((item as Rom).RIPEMD160)))
                    {
                        fieldDats[Field.DatItem_RIPEMD160].Items.Add(key, item);
                    }
#endif

                    // If the file has an MD5
                    else if ((item.ItemType == ItemType.Disk && !string.IsNullOrWhiteSpace((item as Disk).MD5))
                        || (item.ItemType == ItemType.Media && !string.IsNullOrWhiteSpace((item as Media).MD5))
                        || (item.ItemType == ItemType.Rom && !string.IsNullOrWhiteSpace((item as Rom).MD5)))
                    {
                        fieldDats[Field.DatItem_MD5].Items.Add(key, item);
                    }

                    // If the file has a CRC
                    else if ((item.ItemType == ItemType.Rom && !string.IsNullOrWhiteSpace((item as Rom).CRC)))
                    {
                        fieldDats[Field.DatItem_CRC].Items.Add(key, item);
                    }

                    else
                    {
                        fieldDats[Field.NULL].Items.Add(key, item);
                    }
                }
            });

            return fieldDats;
        }

        /// <summary>
        /// Split a SuperDAT by lowest available directory level
        /// </summary>
        /// <param name="outDir">Name of the directory to write the DATs out to</param>
        /// <param name="shortname">True if short names should be used, false otherwise</param>
        /// <param name="basedat">True if original filenames should be used as the base for output filename, false otherwise</param>
        /// <returns>True if split succeeded, false otherwise</returns>
        public bool SplitByLevel(string outDir, bool shortname, bool basedat)
        {
            // First, bucket by games so that we can do the right thing
            Items.BucketBy(Field.Machine_Name, DedupeType.None, lower: false, norename: true);

            // Create a temporary DAT to add things to
            DatFile tempDat = Create(Header);
            tempDat.Header.Name = null;

            // Sort the input keys
            List<string> keys = Items.Keys.ToList();
            keys.Sort(SplitByLevelSort);

            // Then, we loop over the games
            Parallel.ForEach(keys, Globals.ParallelOptions, key =>
            {
                // Here, the key is the name of the game to be used for comparison
                if (tempDat.Header.Name != null && tempDat.Header.Name != Path.GetDirectoryName(key))
                {
                    // Reset the DAT for the next items
                    tempDat = Create(Header);
                    tempDat.Header.Name = null;
                }

                // Clean the input list and set all games to be pathless
                List<DatItem> items = Items[key];
                items.ForEach(item => item.Machine.Name = Path.GetFileName(item.Machine.Name));
                items.ForEach(item => item.Machine.Description = Path.GetFileName(item.Machine.Description));

                // Now add the game to the output DAT
                tempDat.Items.AddRange(key, items);

                // Then set the DAT name to be the parent directory name
                tempDat.Header.Name = Path.GetDirectoryName(key);
            });

            return true;
        }

        /// <summary>
        /// Helper function for SplitByLevel to sort the input game names
        /// </summary>
        /// <param name="a">First string to compare</param>
        /// <param name="b">Second string to compare</param>
        /// <returns>-1 for a coming before b, 0 for a == b, 1 for a coming after b</returns>
        private int SplitByLevelSort(string a, string b)
        {
            NaturalComparer nc = new NaturalComparer();
            int adeep = a.Count(c => c == '/' || c == '\\');
            int bdeep = b.Count(c => c == '/' || c == '\\');

            if (adeep == bdeep)
                return nc.Compare(a, b);

            return adeep - bdeep;
        }

        /// <summary>
        /// Helper function for SplitByLevel to clean and write out a DAT
        /// </summary>
        /// <param name="newDatFile">DAT to clean and write out</param>
        /// <param name="outDir">Directory to write out to</param>
        /// <param name="shortname">True if short naming scheme should be used, false otherwise</param>
        /// <param name="restore">True if original filenames should be used as the base for output filename, false otherwise</param>
        private void SplitByLevelHelper(DatFile newDatFile, string outDir, bool shortname, bool restore)
        {
            // Get the name from the DAT to use separately
            string name = newDatFile.Header.Name;
            string expName = name.Replace("/", " - ").Replace("\\", " - ");

            // Now set the new output values
            newDatFile.Header.FileName = WebUtility.HtmlDecode(string.IsNullOrWhiteSpace(name)
                ? Header.FileName
                : (shortname
                    ? Path.GetFileName(name)
                    : expName
                    )
                );
            newDatFile.Header.FileName = (restore ? $"{Header.FileName} ({newDatFile.Header.FileName})" : newDatFile.Header.FileName);
            newDatFile.Header.Name = $"{Header.Name} ({expName})";
            newDatFile.Header.Description = (string.IsNullOrWhiteSpace(Header.Description) ? newDatFile.Header.Name : $"{Header.Description} ({expName})");
            newDatFile.Header.Type = null;

            // Write out the temporary DAT to the proper directory
            newDatFile.Write(outDir);
        }

        /// <summary>
        /// Split a DAT by size of Rom
        /// </summary>
        /// <param name="radix">Long value representing the split point</param>
        /// <returns>Less Than and Greater Than DatFiles</returns>
        public (DatFile lessThan, DatFile greaterThan) SplitBySize(long radix)
        {
            // Create each of the respective output DATs
            logger.User("Creating and populating new DATs");

            DatFile lessThan = Create(Header.CloneStandard());
            lessThan.Header.FileName += $" (less than {radix})";
            lessThan.Header.Name += $" (less than {radix})";
            lessThan.Header.Description += $" (less than {radix})";

            DatFile greaterThan = Create(Header.CloneStandard());
            greaterThan.Header.FileName += $" (equal-greater than {radix})";
            greaterThan.Header.Name += $" (equal-greater than {radix})";
            greaterThan.Header.Description += $" (equal-greater than {radix})";

            // Now populate each of the DAT objects in turn
            Parallel.ForEach(Items.Keys, Globals.ParallelOptions, key =>
            {
                List<DatItem> items = Items[key];
                foreach (DatItem item in items)
                {
                    // If the file is not a Rom, it automatically goes in the "lesser" dat
                    if (item.ItemType != ItemType.Rom)
                        lessThan.Items.Add(key, item);

                    // If the file is a Rom and has no size, put it in the "lesser" dat
                    else if (item.ItemType == ItemType.Rom && (item as Rom).Size == null)
                        lessThan.Items.Add(key, item);

                    // If the file is a Rom and less than the radix, put it in the "lesser" dat
                    else if (item.ItemType == ItemType.Rom && (item as Rom).Size < radix)
                        lessThan.Items.Add(key, item);

                    // If the file is a Rom and greater than or equal to the radix, put it in the "greater" dat
                    else if (item.ItemType == ItemType.Rom && (item as Rom).Size >= radix)
                        greaterThan.Items.Add(key, item);
                }
            });

            // Then return both DatFiles
            return (lessThan, greaterThan);
        }

        /// <summary>
        /// Split a DAT by type of DatItem
        /// </summary>
        /// <returns>Dictionary of ItemType to DatFile mappings</returns>
        public Dictionary<ItemType, DatFile> SplitByType()
        {
            // Create each of the respective output DATs
            logger.User("Creating and populating new DATs");

            // Create the set of type-to-dat mappings
            Dictionary<ItemType, DatFile> typeDats = new Dictionary<ItemType, DatFile>();

            // We only care about a subset of types
            List<ItemType> outputTypes = new List<ItemType>
            {
                ItemType.Disk,
                ItemType.Media,
                ItemType.Rom,
                ItemType.Sample,
            };

            // Setup all of the DatFiles
            foreach (ItemType itemType in outputTypes)
            {
                typeDats[itemType] = Create(Header.CloneStandard());
                typeDats[itemType].Header.FileName += $" ({itemType})";
                typeDats[itemType].Header.Name += $" ({itemType})";
                typeDats[itemType].Header.Description += $" ({itemType})";
            }

            // Now populate each of the DAT objects in turn
            Parallel.ForEach(outputTypes, Globals.ParallelOptions, itemType =>
            {
                FillWithItemType(typeDats[itemType], itemType);
            });

            return typeDats;
        }

        #endregion

        #region Writing

        /// <summary>
        /// Create and open an output file for writing direct from a dictionary
        /// </summary>
        /// <param name="outDir">Set the output directory (current directory on null)</param>
        /// <param name="overwrite">True if files should be overwritten (default), false if they should be renamed instead</param>
        /// <param name="ignoreblanks">True if blank roms should be skipped on output, false otherwise (default)</param>
        /// <param name="quotes">True if quotes are assumed in supported types (default), false otherwise</param>
        /// <param name="throwOnError">True if the error that is thrown should be thrown back to the caller, false otherwise</param>
        /// <returns>True if the DAT was written correctly, false otherwise</returns>
        public bool Write(
            string outDir,
            bool overwrite = true,
            bool ignoreblanks = false,
            bool quotes = true,
            bool throwOnError = false)
        {
            // If we have nothing writable, abort
            if (!HasWritable())
            {
                logger.User("There were no items to write out!");
                return false;
            }

            // Ensure the output directory is set and created
            outDir = DirectoryExtensions.Ensure(outDir, create: true);

            // If the DAT has no output format, default to XML
            if (Header.DatFormat == 0)
            {
                logger.Verbose("No DAT format defined, defaulting to XML");
                Header.DatFormat = DatFormat.Logiqx;
            }

            // Make sure that the three essential fields are filled in
            EnsureHeaderFields();

            // Bucket roms by game name, if not already
            Items.BucketBy(Field.Machine_Name, DedupeType.None);

            // Output the number of items we're going to be writing
            logger.User($"A total of {Items.TotalCount - Items.RemovedCount} items will be written out to '{Header.FileName}'");

            // Get the outfile names
            Dictionary<DatFormat, string> outfiles = Header.CreateOutFileNames(outDir, overwrite);

            try
            {
                // Write out all required formats
                Parallel.ForEach(outfiles.Keys, Globals.ParallelOptions, datFormat =>
                {
                    string outfile = outfiles[datFormat];
                    try
                    {
                        Create(datFormat, this, quotes)?.WriteToFile(outfile, ignoreblanks, throwOnError);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex, $"Datfile {outfile} could not be written out");
                        if (throwOnError) throw ex;
                    }

                });
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                if (throwOnError) throw ex;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Write the stats out to console for the current DatFile
        /// </summary>
        public void WriteStatsToConsole()
        {
            if (Items.RomCount + Items.DiskCount == 0)
                Items.RecalculateStats();

            Items.BucketBy(Field.Machine_Name, DedupeType.None, norename: true);

            var consoleOutput = BaseReport.Create(StatReportFormat.None, null, true, true);
            consoleOutput.ReplaceStatistics(Header.FileName, Items.Keys.Count(), Items);
        }

        /// <summary>
        /// Create and open an output file for writing direct from a dictionary
        /// </summary>
        /// <param name="outfile">Name of the file to write to</param>
        /// <param name="ignoreblanks">True if blank roms should be skipped on output, false otherwise (default)</param>
        /// <param name="throwOnError">True if the error that is thrown should be thrown back to the caller, false otherwise</param>
        /// <returns>True if the DAT was written correctly, false otherwise</returns>
        public abstract bool WriteToFile(string outfile, bool ignoreblanks = false, bool throwOnError = false);

        /// <summary>
        /// Create a prefix or postfix from inputs
        /// </summary>
        /// <param name="item">DatItem to create a prefix/postfix for</param>
        /// <param name="prefix">True for prefix, false for postfix</param>
        /// <returns>Sanitized string representing the postfix or prefix</returns>
        protected string CreatePrefixPostfix(DatItem item, bool prefix)
        {
            // Initialize strings
            string fix = string.Empty,
                game = item.Machine.Name,
                name = item.GetName() ?? item.ItemType.ToString(),
                crc = string.Empty,
                md5 = string.Empty,
                ripemd160 = string.Empty,
                sha1 = string.Empty,
                sha256 = string.Empty,
                sha384 = string.Empty,
                sha512 = string.Empty,
                size = string.Empty,
                spamsum = string.Empty;

            // If we have a prefix
            if (prefix)
                fix = Header.Prefix + (Header.Quotes ? "\"" : string.Empty);

            // If we have a postfix
            else
                fix = (Header.Quotes ? "\"" : string.Empty) + Header.Postfix;

            // Ensure we have the proper values for replacement
            if (item.ItemType == ItemType.Disk)
            {
                md5 = (item as Disk).MD5 ?? string.Empty;
                sha1 = (item as Disk).SHA1 ?? string.Empty;
            }
            else if (item.ItemType == ItemType.Media)
            {
                md5 = (item as Media).MD5 ?? string.Empty;
                sha1 = (item as Media).SHA1 ?? string.Empty;
                sha256 = (item as Media).SHA256 ?? string.Empty;
                spamsum = (item as Media).SpamSum ?? string.Empty;
            }
            else if (item.ItemType == ItemType.Rom)
            {
                crc = (item as Rom).CRC ?? string.Empty;
                md5 = (item as Rom).MD5 ?? string.Empty;
#if NET_FRAMEWORK
                ripemd160 = (item as Rom).RIPEMD160 ?? string.Empty;
#endif
                sha1 = (item as Rom).SHA1 ?? string.Empty;
                sha256 = (item as Rom).SHA256 ?? string.Empty;
                sha384 = (item as Rom).SHA384 ?? string.Empty;
                sha512 = (item as Rom).SHA512 ?? string.Empty;
                size = (item as Rom).Size?.ToString() ?? string.Empty;
                spamsum = (item as Rom).SpamSum ?? string.Empty;
            }

            // Now do bulk replacement where possible
            fix = fix
                .Replace("%game%", game)
                .Replace("%machine%", game)
                .Replace("%name%", name)
                .Replace("%manufacturer%", item.Machine.Manufacturer ?? string.Empty)
                .Replace("%publisher%", item.Machine.Publisher ?? string.Empty)
                .Replace("%category%", item.Machine.Category ?? string.Empty)
                .Replace("%crc%", crc)
                .Replace("%md5%", md5)
                .Replace("%ripemd160%", ripemd160)
                .Replace("%sha1%", sha1)
                .Replace("%sha256%", sha256)
                .Replace("%sha384%", sha384)
                .Replace("%sha512%", sha512)
                .Replace("%size%", size)
                .Replace("%spamsum%", spamsum);

            // TODO: Add GameName logic here too?
            // TODO: Figure out what I meant by the above ^

            return fix;
        }

        /// <summary>
        /// Process an item and correctly set the item name
        /// </summary>
        /// <param name="item">DatItem to update</param>
        /// <param name="forceRemoveQuotes">True if the Quotes flag should be ignored, false otherwise</param>
        /// <param name="forceRomName">True if the UseRomName should be always on (default), false otherwise</param>
        protected void ProcessItemName(DatItem item, bool forceRemoveQuotes, bool forceRomName = true)
        {
            string name = item.GetName() ?? string.Empty;

            // Backup relevant values and set new ones accordingly
            bool quotesBackup = Header.Quotes;
            bool useRomNameBackup = Header.UseRomName;
            if (forceRemoveQuotes)
                Header.Quotes = false;

            if (forceRomName)
                Header.UseRomName = true;

            // Create the proper Prefix and Postfix
            string pre = CreatePrefixPostfix(item, true);
            string post = CreatePrefixPostfix(item, false);

            // If we're in Depot mode, take care of that instead
            if (Header.OutputDepot?.IsActive == true)
            {
                if (item.ItemType == ItemType.Disk)
                {
                    Disk disk = item as Disk;

                    // We can only write out if there's a SHA-1
                    if (!string.IsNullOrWhiteSpace(disk.SHA1))
                    {
                        name = PathExtensions.GetDepotPath(disk.SHA1, Header.OutputDepot.Depth).Replace('\\', '/');
                        item.SetFields(new Dictionary<Field, string> { [Field.DatItem_Name] = $"{pre}{name}{post}" } );
                    }
                }
                else if (item.ItemType == ItemType.Media)
                {
                    Media media = item as Media;

                    // We can only write out if there's a SHA-1
                    if (!string.IsNullOrWhiteSpace(media.SHA1))
                    {
                        name = PathExtensions.GetDepotPath(media.SHA1, Header.OutputDepot.Depth).Replace('\\', '/');
                        item.SetFields(new Dictionary<Field, string> { [Field.DatItem_Name] = $"{pre}{name}{post}" });
                    }
                }
                else if (item.ItemType == ItemType.Rom)
                {
                    Rom rom = item as Rom;

                    // We can only write out if there's a SHA-1
                    if (!string.IsNullOrWhiteSpace(rom.SHA1))
                    {
                        name = PathExtensions.GetDepotPath(rom.SHA1, Header.OutputDepot.Depth).Replace('\\', '/');
                        item.SetFields(new Dictionary<Field, string> { [Field.DatItem_Name] = $"{pre}{name}{post}" });
                    }
                }

                return;
            }

            if (!string.IsNullOrWhiteSpace(Header.ReplaceExtension) || Header.RemoveExtension)
            {
                if (Header.RemoveExtension)
                    Header.ReplaceExtension = string.Empty;

                string dir = Path.GetDirectoryName(name);
                dir = dir.TrimStart(Path.DirectorySeparatorChar);
                name = Path.Combine(dir, Path.GetFileNameWithoutExtension(name) + Header.ReplaceExtension);
            }

            if (!string.IsNullOrWhiteSpace(Header.AddExtension))
                name += Header.AddExtension;

            if (Header.UseRomName && Header.GameName)
                name = Path.Combine(item.Machine.Name, name);

            // Now assign back the item name
            item.SetFields(new Dictionary<Field, string> { [Field.DatItem_Name] = pre + name + post });

            // Restore all relevant values
            if (forceRemoveQuotes)
                Header.Quotes = quotesBackup;

            if (forceRomName)
                Header.UseRomName = useRomNameBackup;
        }

        /// <summary>
        /// Process any DatItems that are "null", usually created from directory population
        /// </summary>
        /// <param name="datItem">DatItem to check for "null" status</param>
        /// <returns>Cleaned DatItem</returns>
        protected DatItem ProcessNullifiedItem(DatItem datItem)
        {
            // If we don't have a Rom, we can ignore it
            if (datItem.ItemType != ItemType.Rom)
                return datItem;

            // Cast for easier parsing
            Rom rom = datItem as Rom;

            // If the Rom has "null" characteristics, ensure all fields
            if (rom.Size == null && rom.CRC == "null")
            {
                logger.Verbose($"Empty folder found: {datItem.Machine.Name}");

                rom.Name = (rom.Name == "null" ? "-" : rom.Name);
                rom.Size = Constants.SizeZero;
                rom.CRC = rom.CRC == "null" ? Constants.CRCZero : null;
                rom.MD5 = rom.MD5 == "null" ? Constants.MD5Zero : null;
#if NET_FRAMEWORK
                rom.RIPEMD160 = rom.RIPEMD160 == "null" ? Constants.RIPEMD160Zero : null;
#endif
                rom.SHA1 = rom.SHA1 == "null" ? Constants.SHA1Zero : null;
                rom.SHA256 = rom.SHA256 == "null" ? Constants.SHA256Zero : null;
                rom.SHA384 = rom.SHA384 == "null" ? Constants.SHA384Zero : null;
                rom.SHA512 = rom.SHA512 == "null" ? Constants.SHA512Zero : null;
                rom.SpamSum = rom.SpamSum == "null" ? Constants.SpamSumZero : null;
            }

            return rom;
        }

        /// <summary>
        /// Get supported types for write
        /// </summary>
        /// <returns>List of supported types for writing</returns>
        protected virtual ItemType[] GetSupportedTypes()
        {
            return Enum.GetValues(typeof(ItemType)) as ItemType[];
        }

        /// <summary>
        /// Get if a machine contains any writable items
        /// </summary>
        /// <param name="datItems">DatItems to check</param>
        /// <returns>True if the machine contains at least one writable item, false otherwise</returns>
        /// <remarks>Empty machines are kept with this</remarks>
        protected bool ContainsWritable(List<DatItem> datItems)
        {
            // Empty machines are considered writable
            if (datItems == null || datItems.Count == 0)
                return true;

            foreach (DatItem datItem in datItems)
            {
                if (GetSupportedTypes().Contains(datItem.ItemType))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Get if an item should be ignored on write
        /// </summary>
        /// <param name="datItem">DatItem to check</param>
        /// <param name="ignoreblanks">True if blank roms should be skipped on output, false otherwise</param>
        /// <returns>True if the item should be skipped on write, false otherwise</returns>
        protected bool ShouldIgnore(DatItem datItem, bool ignoreBlanks)
        {
            // If the item is supposed to be removed, we ignore
            if (datItem.Remove)
                return true;

            // If we have the Blank dat item, we ignore
            if (datItem.ItemType == ItemType.Blank)
                return true;

            // If we're ignoring blanks and we have a Rom
            if (ignoreBlanks && datItem.ItemType == ItemType.Rom)
            {
                Rom rom = datItem as Rom;

                // If we have a 0-size or blank rom, then we ignore
                if (rom.Size == 0 || rom.Size == null)
                    return true;
            }

            // If we have an item type not in the list of supported values
            if (!GetSupportedTypes().Contains(datItem.ItemType))
                return true;

            return false;
        }

        /// <summary>
        /// Ensure that FileName, Name, and Description are filled with some value
        /// </summary>
        private void EnsureHeaderFields()
        {
            // Empty FileName
            if (string.IsNullOrWhiteSpace(Header.FileName))
            {
                if (string.IsNullOrWhiteSpace(Header.Name) && string.IsNullOrWhiteSpace(Header.Description))
                    Header.FileName = Header.Name = Header.Description = "Default";

                else if (string.IsNullOrWhiteSpace(Header.Name) && !string.IsNullOrWhiteSpace(Header.Description))
                    Header.FileName = Header.Name = Header.Description;

                else if (!string.IsNullOrWhiteSpace(Header.Name) && string.IsNullOrWhiteSpace(Header.Description))
                    Header.FileName = Header.Description = Header.Name;

                else if (!string.IsNullOrWhiteSpace(Header.Name) && !string.IsNullOrWhiteSpace(Header.Description))
                    Header.FileName = Header.Description;
            }

            // Filled FileName
            else
            {
                if (string.IsNullOrWhiteSpace(Header.Name) && string.IsNullOrWhiteSpace(Header.Description))
                    Header.Name = Header.Description = Header.FileName;

                else if (string.IsNullOrWhiteSpace(Header.Name) && !string.IsNullOrWhiteSpace(Header.Description))
                    Header.Name = Header.Description;

                else if (!string.IsNullOrWhiteSpace(Header.Name) && string.IsNullOrWhiteSpace(Header.Description))
                    Header.Description = Header.Name;
            }
        }

        /// <summary>
        /// Get if the DatFile has any writable items
        /// </summary>
        /// <returns>True if there are any writable items, false otherwise</returns>
        private bool HasWritable()
        {
            // Force a statistics recheck, just in case
            Items.RecalculateStats();

            // If there's nothing there, abort
            if (Items.TotalCount == 0)
                return false;

            // If every item is removed, abort
            if (Items.TotalCount == Items.RemovedCount)
                return false;

            return true;
        }

        #endregion
    }
}
