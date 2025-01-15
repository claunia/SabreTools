using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
#if NET40_OR_GREATER || NETCOREAPP
using System.Threading.Tasks;
#endif
using SabreTools.Core.Tools;
using SabreTools.DatFiles.Formats;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;
using SabreTools.IO;
using SabreTools.IO.Logging;
using SabreTools.Matching.Compare;

namespace SabreTools.DatFiles
{
    /// <summary>
    /// Helper methods for updating and converting DatFiles
    /// </summary>
    public static class DatFileTool
    {
        #region Logging

        /// <summary>
        /// Logging object
        /// </summary>
        private static readonly Logger _staticLogger = new();

        #endregion

        #region Creation

        /// <summary>
        /// Create a specific type of DatFile to be used based on a format and a base DAT
        /// </summary>
        /// <param name="datFormat">Format of the DAT to be created, default is <see cref="DatFormat.Logiqx"/> </param>
        /// <param name="baseDat">DatFile containing the information to use in specific operations, default is null</param>
        /// <returns>DatFile of the specific internal type that corresponds to the inputs</returns>
        public static DatFile CreateDatFile(DatFormat datFormat = DatFormat.Logiqx, DatFile? baseDat = null)
        {
            return datFormat switch
            {
                DatFormat.ArchiveDotOrg => new ArchiveDotOrg(baseDat),
                DatFormat.AttractMode => new AttractMode(baseDat),
                DatFormat.ClrMamePro => new ClrMamePro(baseDat),
                DatFormat.CSV => new CommaSeparatedValue(baseDat),
                DatFormat.DOSCenter => new DosCenter(baseDat),
                DatFormat.EverdriveSMDB => new EverdriveSMDB(baseDat),
                DatFormat.Listrom => new Listrom(baseDat),
                DatFormat.Listxml => new Listxml(baseDat),
                DatFormat.Logiqx => new Logiqx(baseDat, false),
                DatFormat.LogiqxDeprecated => new Logiqx(baseDat, true),
                DatFormat.MissFile => new Missfile(baseDat),
                DatFormat.OfflineList => new OfflineList(baseDat),
                DatFormat.OpenMSX => new OpenMSX(baseDat),
                DatFormat.RedumpMD2 => new Md2File(baseDat),
                DatFormat.RedumpMD4 => new Md4File(baseDat),
                DatFormat.RedumpMD5 => new Md5File(baseDat),
                DatFormat.RedumpSFV => new SfvFile(baseDat),
                DatFormat.RedumpSHA1 => new Sha1File(baseDat),
                DatFormat.RedumpSHA256 => new Sha256File(baseDat),
                DatFormat.RedumpSHA384 => new Sha384File(baseDat),
                DatFormat.RedumpSHA512 => new Sha512File(baseDat),
                DatFormat.RedumpSpamSum => new SpamSumFile(baseDat),
                DatFormat.RomCenter => new RomCenter(baseDat),
                DatFormat.SabreJSON => new SabreJSON(baseDat),
                DatFormat.SabreXML => new SabreXML(baseDat),
                DatFormat.SoftwareList => new Formats.SoftwareList(baseDat),
                DatFormat.SSV => new SemicolonSeparatedValue(baseDat),
                DatFormat.TSV => new TabSeparatedValue(baseDat),

                // We use new-style Logiqx as a backup for generic DatFile
                _ => new Logiqx(baseDat, false),
            };
        }

        /// <summary>
        /// Create a new DatFile from an existing DatHeader
        /// </summary>
        /// <param name="datHeader">DatHeader to get the values from</param>
        public static DatFile CreateDatFile(DatHeader datHeader)
        {
            DatFormat format = datHeader.GetFieldValue<DatFormat>(DatHeader.DatFormatKey);
            DatFile datFile = CreateDatFile(format);
            datFile.SetHeader(datHeader);
            return datFile;
        }

        #endregion

        #region Sorting and Merging

        /// <summary>
        /// Merge an arbitrary set of DatItems based on the supplied information
        /// </summary>
        /// <param name="items">List of DatItem objects representing the items to be merged</param>
        /// <returns>A List of DatItem objects representing the merged items</returns>
        public static List<DatItem> Merge(List<DatItem>? items)
        {
            // Check for null or blank inputs first
            if (items == null || items.Count == 0)
                return [];

            // Create output list
            List<DatItem> output = [];

            // Then deduplicate them by checking to see if data matches previous saved roms
            int nodumpCount = 0;
            foreach (DatItem datItem in items)
            {
                // If we don't have a Disk, File, Media, or Rom, we skip checking for duplicates
                if (datItem is not Disk && datItem is not DatItems.Formats.File && datItem is not Media && datItem is not Rom)
                    continue;

                // If it's a nodump, add and skip
                if (datItem is Rom rom && rom.GetStringFieldValue(Models.Metadata.Rom.StatusKey).AsEnumValue<ItemStatus>() == ItemStatus.Nodump)
                {
                    output.Add(datItem);
                    nodumpCount++;
                    continue;
                }
                else if (datItem is Disk disk && disk.GetStringFieldValue(Models.Metadata.Disk.StatusKey).AsEnumValue<ItemStatus>() == ItemStatus.Nodump)
                {
                    output.Add(datItem);
                    nodumpCount++;
                    continue;
                }

                // If it's the first non-nodump item in the list, don't touch it
                if (output.Count == nodumpCount)
                {
                    output.Add(datItem);
                    continue;
                }

                // Find the index of the first duplicate, if one exists
                int pos = output.FindIndex(lastItem => datItem.GetDuplicateStatus(lastItem) != 0x00);
                if (pos < 0)
                {
                    output.Add(datItem);
                    continue;
                }

                // Get the duplicate item
                DatItem savedItem = output[pos];
                DupeType dupetype = datItem.GetDuplicateStatus(savedItem);

                // Disks, File, Media, and Roms have more information to fill
                if (datItem is Disk diskItem && savedItem is Disk savedDisk)
                    savedDisk.FillMissingInformation(diskItem);
                else if (datItem is DatItems.Formats.File fileItem && savedItem is DatItems.Formats.File savedFile)
                    savedFile.FillMissingInformation(fileItem);
                else if (datItem is Media mediaItem && savedItem is Media savedMedia)
                    savedMedia.FillMissingInformation(mediaItem);
                else if (datItem is Rom romItem && savedItem is Rom savedRom)
                    savedRom.FillMissingInformation(romItem);

                // Set the duplicate type on the saved item
                savedItem.SetFieldValue<DupeType>(DatItem.DupeTypeKey, dupetype);

                // Get the sources associated with the items
                var savedSource = savedItem.GetFieldValue<Source?>(DatItem.SourceKey);
                var itemSource = datItem.GetFieldValue<Source?>(DatItem.SourceKey);

                // Get the machines associated with the items
                var savedMachine = savedItem.GetFieldValue<Machine>(DatItem.MachineKey);
                var itemMachine = datItem.GetFieldValue<Machine>(DatItem.MachineKey);

                // If the current source has a lower ID than the saved, use the saved source
                if (itemSource?.Index < savedSource?.Index)
                {
                    datItem.SetFieldValue<Source?>(DatItem.SourceKey, savedSource.Clone() as Source);
                    savedItem.CopyMachineInformation(datItem);
                    savedItem.SetName(datItem.GetName());
                }

                // If the saved machine is a child of the current machine, use the current machine instead
                if (savedMachine?.GetStringFieldValue(Models.Metadata.Machine.CloneOfKey) == itemMachine?.GetStringFieldValue(Models.Metadata.Machine.NameKey)
                    || savedMachine?.GetStringFieldValue(Models.Metadata.Machine.RomOfKey) == itemMachine?.GetStringFieldValue(Models.Metadata.Machine.NameKey))
                {
                    savedItem.CopyMachineInformation(datItem);
                    savedItem.SetName(datItem.GetName());
                }

                // Replace the original item in the list
                output.RemoveAt(pos);
                output.Insert(pos, savedItem);
            }

            // Then return the result
            return output;
        }

        /// <summary>
        /// Sort a list of DatItem objects by SourceID, Game, and Name (in order)
        /// </summary>
        /// <param name="items">List of DatItem objects representing the items to be sorted</param>
        /// <param name="norename">True if files are not renamed, false otherwise</param>
        /// <returns>True if it sorted correctly, false otherwise</returns>
        public static bool Sort(ref List<DatItem> items, bool norename)
        {
            items.Sort(delegate (DatItem x, DatItem y)
            {
                try
                {
                    var nc = new NaturalComparer();

                    // If machine names don't match
                    string? xMachineName = x.GetFieldValue<Machine>(DatItem.MachineKey)?.GetStringFieldValue(Models.Metadata.Machine.NameKey);
                    string? yMachineName = y.GetFieldValue<Machine>(DatItem.MachineKey)?.GetStringFieldValue(Models.Metadata.Machine.NameKey);
                    if (xMachineName != yMachineName)
                        return nc.Compare(xMachineName, yMachineName);

                    // If types don't match
                    string? xType = x.GetStringFieldValue(Models.Metadata.DatItem.TypeKey);
                    string? yType = y.GetStringFieldValue(Models.Metadata.DatItem.TypeKey);
                    if (xType != yType)
                        return xType.AsEnumValue<ItemType>() - yType.AsEnumValue<ItemType>();

                    // If directory names don't match
                    string? xDirectoryName = Path.GetDirectoryName(TextHelper.RemovePathUnsafeCharacters(x.GetName() ?? string.Empty));
                    string? yDirectoryName = Path.GetDirectoryName(TextHelper.RemovePathUnsafeCharacters(y.GetName() ?? string.Empty));
                    if (xDirectoryName != yDirectoryName)
                        return nc.Compare(xDirectoryName, yDirectoryName);

                    // If item names don't match
                    string? xName = Path.GetFileName(TextHelper.RemovePathUnsafeCharacters(x.GetName() ?? string.Empty));
                    string? yName = Path.GetFileName(TextHelper.RemovePathUnsafeCharacters(y.GetName() ?? string.Empty));
                    if (xName != yName)
                        return nc.Compare(xName, yName);

                    // Otherwise, compare on machine or source, depending on the flag
                    int? xSourceIndex = x.GetFieldValue<Source?>(DatItem.SourceKey)?.Index;
                    int? ySourceIndex = y.GetFieldValue<Source?>(DatItem.SourceKey)?.Index;
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
        /// Sort a list of DatItem objects by SourceID, Game, and Name (in order)
        /// </summary>
        /// <param name="mappings">List of item ID to DatItem mappings representing the items to be sorted</param>
        /// <param name="norename">True if files are not renamed, false otherwise</param>
        /// <returns>True if it sorted correctly, false otherwise</returns>
        public static bool SortDB(ref List<KeyValuePair<long, DatItem>> mappings, bool norename)
        {
            mappings.Sort(delegate (KeyValuePair<long, DatItem> x, KeyValuePair<long, DatItem> y)
            {
                try
                {
                    var nc = new NaturalComparer();

                    // TODO: Fix this since DB uses an external map for machines

                    // If machine names don't match
                    string? xMachineName = x.Value.GetFieldValue<Machine>(DatItem.MachineKey)?.GetStringFieldValue(Models.Metadata.Machine.NameKey);
                    string? yMachineName = y.Value.GetFieldValue<Machine>(DatItem.MachineKey)?.GetStringFieldValue(Models.Metadata.Machine.NameKey);
                    if (xMachineName != yMachineName)
                        return nc.Compare(xMachineName, yMachineName);

                    // If types don't match
                    string? xType = x.Value.GetStringFieldValue(Models.Metadata.DatItem.TypeKey);
                    string? yType = y.Value.GetStringFieldValue(Models.Metadata.DatItem.TypeKey);
                    if (xType != yType)
                        return xType.AsEnumValue<ItemType>() - yType.AsEnumValue<ItemType>();

                    // If directory names don't match
                    string? xDirectoryName = Path.GetDirectoryName(TextHelper.RemovePathUnsafeCharacters(x.Value.GetName() ?? string.Empty));
                    string? yDirectoryName = Path.GetDirectoryName(TextHelper.RemovePathUnsafeCharacters(y.Value.GetName() ?? string.Empty));
                    if (xDirectoryName != yDirectoryName)
                        return nc.Compare(xDirectoryName, yDirectoryName);

                    // If item names don't match
                    string? xName = Path.GetFileName(TextHelper.RemovePathUnsafeCharacters(x.Value.GetName() ?? string.Empty));
                    string? yName = Path.GetFileName(TextHelper.RemovePathUnsafeCharacters(y.Value.GetName() ?? string.Empty));
                    if (xName != yName)
                        return nc.Compare(xName, yName);

                    // Otherwise, compare on machine or source, depending on the flag
                    int? xSourceIndex = x.Value.GetFieldValue<Source?>(DatItem.SourceKey)?.Index;
                    int? ySourceIndex = y.Value.GetFieldValue<Source?>(DatItem.SourceKey)?.Index;
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

        #region SuperDAT

        /// <summary>
        /// Apply SuperDAT naming logic to a merged DatFile
        /// </summary>
        /// <param name="datFile">Current DatFile object to run operations on</param>
        /// <param name="inputs">List of inputs to use for renaming</param>
        public static void ApplySuperDAT(DatFile datFile, List<ParentablePath> inputs)
        {
#if NET452_OR_GREATER || NETCOREAPP
            Parallel.ForEach(datFile.Items.SortedKeys, Core.Globals.ParallelOptions, key =>
#elif NET40_OR_GREATER
            Parallel.ForEach(datFile.Items.SortedKeys, key =>
#else
            foreach (var key in datFile.Items.SortedKeys)
#endif
            {
                List<DatItem>? items = datFile.GetItemsForBucket(key);
                if (items == null)
#if NET40_OR_GREATER || NETCOREAPP
                    return;
#else
                    continue;
#endif

                List<DatItem> newItems = [];
                foreach (DatItem item in items)
                {
                    DatItem newItem = item;
                    var source = newItem.GetFieldValue<Source?>(DatItem.SourceKey);
                    if (source == null)
                        continue;

                    string filename = inputs[source.Index].CurrentPath;
                    string rootpath = inputs[source.Index].ParentPath ?? string.Empty;

                    if (rootpath.Length > 0
#if NETFRAMEWORK
                        && !rootpath.EndsWith(Path.DirectorySeparatorChar.ToString())
                        && !rootpath.EndsWith(Path.AltDirectorySeparatorChar.ToString()))
#else
                        && !rootpath.EndsWith(Path.DirectorySeparatorChar)
                        && !rootpath.EndsWith(Path.AltDirectorySeparatorChar))
#endif
                    {
                        rootpath += Path.DirectorySeparatorChar.ToString();
                    }

                    filename = filename.Remove(0, rootpath.Length);

                    var machine = newItem.GetFieldValue<Machine>(DatItem.MachineKey);
                    if (machine == null)
                        continue;

                    machine.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, Path.GetDirectoryName(filename)
                        + Path.DirectorySeparatorChar
                        + Path.GetFileNameWithoutExtension(filename)
                        + Path.DirectorySeparatorChar
                        + machine.GetStringFieldValue(Models.Metadata.Machine.NameKey));

                    newItems.Add(newItem);
                }

                datFile.RemoveBucket(key);
                newItems.ForEach(item => datFile.AddItem(item, statsOnly: false));
#if NET40_OR_GREATER || NETCOREAPP
            });
#else
            }
#endif
        }

        /// <summary>
        /// Apply SuperDAT naming logic to a merged DatFile
        /// </summary>
        /// <param name="datFile">Current DatFile object to run operations on</param>
        /// <param name="inputs">List of inputs to use for renaming</param>
        public static void ApplySuperDATDB(DatFile datFile, List<ParentablePath> inputs)
        {
            List<string> keys = [.. datFile.ItemsDB.SortedKeys];
#if NET452_OR_GREATER || NETCOREAPP
            Parallel.ForEach(keys, Core.Globals.ParallelOptions, key =>
#elif NET40_OR_GREATER
            Parallel.ForEach(keys, key =>
#else
            foreach (var key in keys)
#endif
            {
                var items = datFile.GetItemsForBucketDB(key);
                if (items == null)
#if NET40_OR_GREATER || NETCOREAPP
                    return;
#else
                    continue;
#endif

                foreach (var item in items)
                {
                    var source = datFile.ItemsDB.GetSourceForItem(item.Key);
                    if (source.Value == null)
                        continue;

                    var machine = datFile.ItemsDB.GetMachineForItem(item.Key);
                    if (machine.Value == null)
                        continue;

                    string filename = inputs[source.Value.Index].CurrentPath;
                    string rootpath = inputs[source.Value.Index].ParentPath ?? string.Empty;

                    if (rootpath.Length > 0
#if NETFRAMEWORK
                        && !rootpath!.EndsWith(Path.DirectorySeparatorChar.ToString())
                        && !rootpath!.EndsWith(Path.AltDirectorySeparatorChar.ToString()))
#else
                        && !rootpath.EndsWith(Path.DirectorySeparatorChar)
                        && !rootpath.EndsWith(Path.AltDirectorySeparatorChar))
#endif
                    {
                        rootpath += Path.DirectorySeparatorChar.ToString();
                    }

                    filename = filename.Remove(0, rootpath.Length);

                    machine.Value.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, Path.GetDirectoryName(filename) + Path.DirectorySeparatorChar
                        + Path.GetFileNameWithoutExtension(filename) + Path.DirectorySeparatorChar
                        + machine.Value.GetStringFieldValue(Models.Metadata.Machine.NameKey));
                }
#if NET40_OR_GREATER || NETCOREAPP
            });
#else
            }
#endif
        }

        #endregion

        #region Replacement

        /// <summary>
        /// Replace item values from the base set represented by the current DAT
        /// </summary>
        /// <param name="datFile">Current DatFile object to use for updating</param>
        /// <param name="intDat">DatFile to replace the values in</param>
        /// <param name="machineFieldNames">List of machine field names representing what should be updated</param>
        /// <param name="itemFieldNames">List of item field names representing what should be updated</param>
        /// <param name="onlySame">True if descriptions should only be replaced if the game name is the same, false otherwise</param>
        public static void BaseReplace(
            DatFile datFile,
            DatFile intDat,
            List<string> machineFieldNames,
            Dictionary<string, List<string>> itemFieldNames,
            bool onlySame)
        {
            InternalStopwatch watch = new($"Replacing items in '{intDat.Header.GetStringFieldValue(DatHeader.FileNameKey)}' from the base DAT");

            // If we are matching based on DatItem fields of any sort
            if (itemFieldNames.Count > 0)
            {
                // For comparison's sake, we want to use CRC as the base bucketing
                datFile.BucketBy(ItemKey.CRC);
                datFile.Deduplicate(DedupeType.Full);
                intDat.BucketBy(ItemKey.CRC);

                // Then we do a hashwise comparison against the base DAT
#if NET452_OR_GREATER || NETCOREAPP
                Parallel.ForEach(intDat.Items.SortedKeys, Core.Globals.ParallelOptions, key =>
#elif NET40_OR_GREATER
                Parallel.ForEach(intDat.Items.SortedKeys, key =>
#else
                foreach (var key in intDat.Items.SortedKeys)
#endif
                {
                    List<DatItem>? datItems = intDat.GetItemsForBucket(key);
                    if (datItems == null)
#if NET40_OR_GREATER || NETCOREAPP
                        return;
#else
                        continue;
#endif

                    List<DatItem> newDatItems = [];
                    foreach (DatItem datItem in datItems)
                    {
                        List<DatItem> dupes = datFile.GetDuplicates(datItem, sorted: true);
                        if (datItem.Clone() is not DatItem newDatItem)
                            continue;

                        // Replace fields from the first duplicate, if we have one
                        if (dupes.Count > 0)
                            Replacer.ReplaceFields(newDatItem, dupes[0], itemFieldNames);

                        newDatItems.Add(newDatItem);
                    }

                    // Now add the new list to the key
                    intDat.RemoveBucket(key);
                    newDatItems.ForEach(item => intDat.AddItem(item, statsOnly: false));
#if NET40_OR_GREATER || NETCOREAPP
                });
#else
                }
#endif
            }

            // If we are matching based on Machine fields of any sort
            if (machineFieldNames.Count > 0)
            {
                // For comparison's sake, we want to use Machine Name as the base bucketing
                datFile.BucketBy(ItemKey.Machine);
                datFile.Deduplicate(DedupeType.Full);
                intDat.BucketBy(ItemKey.Machine);

                // Then we do a namewise comparison against the base DAT
#if NET452_OR_GREATER || NETCOREAPP
                Parallel.ForEach(intDat.Items.SortedKeys, Core.Globals.ParallelOptions, key =>
#elif NET40_OR_GREATER
                Parallel.ForEach(intDat.Items.SortedKeys, key =>
#else
                foreach (var key in intDat.Items.SortedKeys)
#endif
                {
                    List<DatItem>? datItems = intDat.GetItemsForBucket(key);
                    if (datItems == null)
#if NET40_OR_GREATER || NETCOREAPP
                        return;
#else
                        continue;
#endif

                    List<DatItem> newDatItems = [];
                    foreach (DatItem datItem in datItems)
                    {
                        if (datItem.Clone() is not DatItem newDatItem)
                            continue;

                        var list = datFile.GetItemsForBucket(key);
                        if (list.Count > 0)
                            Replacer.ReplaceFields(newDatItem.GetFieldValue<Machine>(DatItem.MachineKey)!, list[index: 0].GetFieldValue<Machine>(DatItem.MachineKey)!, machineFieldNames, onlySame);

                        newDatItems.Add(newDatItem);
                    }

                    // Now add the new list to the key
                    intDat.RemoveBucket(key);
                    newDatItems.ForEach(item => intDat.AddItem(item, statsOnly: false));
#if NET40_OR_GREATER || NETCOREAPP
                });
#else
                }
#endif
            }

            watch.Stop();
        }

        /// <summary>
        /// Replace item values from the base set represented by the current DAT
        /// </summary>
        /// <param name="datFile">Current DatFile object to use for updating</param>
        /// <param name="intDat">DatFile to replace the values in</param>
        /// <param name="machineFieldNames">List of machine field names representing what should be updated</param>
        /// <param name="itemFieldNames">List of item field names representing what should be updated</param>
        /// <param name="onlySame">True if descriptions should only be replaced if the game name is the same, false otherwise</param>
        public static void BaseReplaceDB(
            DatFile datFile,
            DatFile intDat,
            List<string> machineFieldNames,
            Dictionary<string, List<string>> itemFieldNames,
            bool onlySame)
        {
            InternalStopwatch watch = new($"Replacing items in '{intDat.Header.GetStringFieldValue(DatHeader.FileNameKey)}' from the base DAT");

            // If we are matching based on DatItem fields of any sort
            if (itemFieldNames.Count > 0)
            {
                // For comparison's sake, we want to use CRC as the base bucketing
                datFile.BucketBy(ItemKey.CRC);
                datFile.Deduplicate(DedupeType.Full);
                intDat.BucketBy(ItemKey.CRC);

                // Then we do a hashwise comparison against the base DAT
#if NET452_OR_GREATER || NETCOREAPP
                Parallel.ForEach(intDat.ItemsDB.SortedKeys, Core.Globals.ParallelOptions, key =>
#elif NET40_OR_GREATER
                Parallel.ForEach(intDat.ItemsDB.SortedKeys, key =>
#else
                foreach (var key in intDat.ItemsDB.SortedKeys)
#endif
                {
                    var datItems = intDat.GetItemsForBucketDB(key);
                    if (datItems == null)
#if NET40_OR_GREATER || NETCOREAPP
                        return;
#else
                        continue;
#endif

                    foreach (var datItem in datItems)
                    {
                        var dupes = datFile.GetDuplicatesDB(datItem, sorted: true);
                        if (datItem.Value.Clone() is not DatItem newDatItem)
                            continue;

                        // Replace fields from the first duplicate, if we have one
                        if (dupes.Count > 0)
                            Replacer.ReplaceFields(datItem.Value, dupes.First().Value, itemFieldNames);
                    }
#if NET40_OR_GREATER || NETCOREAPP
                });
#else
                }
#endif
            }

            // If we are matching based on Machine fields of any sort
            if (machineFieldNames.Count > 0)
            {
                // For comparison's sake, we want to use Machine Name as the base bucketing
                datFile.BucketBy(ItemKey.Machine);
                datFile.Deduplicate(DedupeType.Full);
                intDat.BucketBy(ItemKey.Machine);

                // Then we do a namewise comparison against the base DAT
#if NET452_OR_GREATER || NETCOREAPP
                Parallel.ForEach(intDat.ItemsDB.SortedKeys, Core.Globals.ParallelOptions, key =>
#elif NET40_OR_GREATER
                Parallel.ForEach(intDat.ItemsDB.SortedKeys, key =>
#else
                foreach (var key in intDat.ItemsDB.SortedKeys)
#endif
                {
                    var datItems = intDat.GetItemsForBucketDB(key);
                    if (datItems == null)
#if NET40_OR_GREATER || NETCOREAPP
                        return;
#else
                        continue;
#endif

                    foreach (var datItem in datItems)
                    {
                        var datMachine = datFile.ItemsDB.GetMachineForItem(datFile.GetItemsForBucketDB(key)!.First().Key);
                        var intMachine = intDat.ItemsDB.GetMachineForItem(datItem.Key);
                        if (datMachine.Value != null && intMachine.Value != null)
                            Replacer.ReplaceFields(intMachine.Value, datMachine.Value, machineFieldNames, onlySame);
                    }
#if NET40_OR_GREATER || NETCOREAPP
                });
#else
                }
#endif
            }

            watch.Stop();
        }

        #endregion

        #region Diffing

        /// <summary>
        /// Output diffs against a base set represented by the current DAT
        /// </summary>
        /// <param name="datFile">Current DatFile object to use for updating</param>
        /// <param name="intDat">DatFile to replace the values in</param>
        /// <param name="useGames">True to diff using games, false to use hashes</param>
        public static void DiffAgainst(DatFile datFile, DatFile intDat, bool useGames)
        {
            InternalStopwatch watch = new($"Comparing '{intDat.Header.GetStringFieldValue(DatHeader.FileNameKey)}' to base DAT");

            // For comparison's sake, we want to a the base bucketing
            if (useGames)
            {
                intDat.BucketBy(ItemKey.Machine);
            }
            else
            {
                intDat.BucketBy(ItemKey.CRC);
                intDat.Deduplicate(DedupeType.Full);
            }

            // Then we compare against the base DAT
#if NET452_OR_GREATER || NETCOREAPP
            Parallel.ForEach(intDat.Items.SortedKeys, Core.Globals.ParallelOptions, key =>
#elif NET40_OR_GREATER
            Parallel.ForEach(intDat.Items.SortedKeys, key =>
#else
            foreach (var key in intDat.Items.SortedKeys)
#endif
            {
                // Game Against uses game names
                if (useGames)
                {
                    // If the key is null, keep it
                    var intList = intDat.GetItemsForBucket(key);
                    if (intList.Count == 0)
#if NET40_OR_GREATER || NETCOREAPP
                        return;
#else
                        continue;
#endif

                    // If the base DAT doesn't contain the key, keep it
                    var list = datFile.GetItemsForBucket(key);
                    if (list.Count == 0)
#if NET40_OR_GREATER || NETCOREAPP
                        return;
#else
                        continue;
#endif

                    // If the number of items is different, then keep it
                    if (list.Count != intList.Count)
#if NET40_OR_GREATER || NETCOREAPP
                        return;
#else
                        continue;
#endif

                    // Otherwise, compare by name and hash the remaining files
                    bool exactMatch = true;
                    foreach (DatItem item in intList)
                    {
                        // TODO: Make this granular to name as well
                        if (!list.Contains(item))
                        {
                            exactMatch = false;
                            break;
                        }
                    }

                    // If we have an exact match, remove the game
                    if (exactMatch)
                        intDat.RemoveBucket(key);
                }

                // Standard Against uses hashes
                else
                {
                    List<DatItem>? datItems = intDat.GetItemsForBucket(key);
                    if (datItems == null)
#if NET40_OR_GREATER || NETCOREAPP
                        return;
#else
                        continue;
#endif

                    List<DatItem> keepDatItems = [];
                    foreach (DatItem datItem in datItems)
                    {
                        if (!datFile.HasDuplicates(datItem, true))
                            keepDatItems.Add(datItem);
                    }

                    // Now add the new list to the key
                    intDat.RemoveBucket(key);
                    keepDatItems.ForEach(item => intDat.AddItem(item, statsOnly: false));
                }
#if NET40_OR_GREATER || NETCOREAPP
            });
#else
            }
#endif

            watch.Stop();
        }

        /// <summary>
        /// Output cascading diffs
        /// </summary>
        /// <param name="datFile">Current DatFile object to use for updating</param>
        /// <param name="datHeaders">Dat headers used optionally</param>
        /// <returns>List of DatFiles representing the individually indexed items</returns>
        public static List<DatFile> DiffCascade(DatFile datFile, List<DatHeader> datHeaders)
        {
            // Create a list of DatData objects representing output files
            List<DatFile> outDats = [];

            // Ensure the current DatFile is sorted optimally
            datFile.BucketBy(ItemKey.CRC);

            // Loop through each of the inputs and get or create a new DatData object
            InternalStopwatch watch = new("Initializing and filling all output DATs");

            // Create the DatFiles from the set of headers
            DatFile[] outDatsArray = new DatFile[datHeaders.Count];
#if NET452_OR_GREATER || NETCOREAPP
            Parallel.For(0, datHeaders.Count, Core.Globals.ParallelOptions, j =>
#elif NET40_OR_GREATER
            Parallel.For(0, datHeaders.Count, j =>
#else
            for (int j = 0; j < datHeaders.Count; j++)
#endif
            {
                DatFile diffData = CreateDatFile(datHeaders[j]);
                diffData.ResetDictionary();
                FillWithSourceIndex(datFile, diffData, j);
                FillWithSourceIndexDB(datFile, diffData, j);
                outDatsArray[j] = diffData;
#if NET40_OR_GREATER || NETCOREAPP
            });
#else
            }
#endif

            outDats = [.. outDatsArray];
            watch.Stop();

            return outDats;
        }

        /// <summary>
        /// Output duplicate item diff
        /// </summary>
        /// <param name="datFile">Current DatFile object to use for updating</param>
        /// <param name="inputs">List of inputs to write out from</param>
        public static DatFile DiffDuplicates(DatFile datFile, List<string> inputs)
        {
            List<ParentablePath> paths = inputs.ConvertAll(i => new ParentablePath(i));
            return DiffDuplicates(datFile, paths);
            //return DiffDuplicatesDB(datFile, paths);
        }

        /// <summary>
        /// Output duplicate item diff
        /// </summary>
        /// <param name="datFile">Current DatFile object to use for updating</param>
        /// <param name="inputs">List of inputs to write out from</param>
        public static DatFile DiffDuplicates(DatFile datFile, List<ParentablePath> inputs)
        {
            InternalStopwatch watch = new("Initializing duplicate DAT");

            // Fill in any information not in the base DAT
            if (string.IsNullOrEmpty(datFile.Header.GetStringFieldValue(DatHeader.FileNameKey)))
                datFile.Header.SetFieldValue<string?>(DatHeader.FileNameKey, "All DATs");

            if (string.IsNullOrEmpty(datFile.Header.GetStringFieldValue(Models.Metadata.Header.NameKey)))
                datFile.Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, "datFile.All DATs");

            if (string.IsNullOrEmpty(datFile.Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey)))
                datFile.Header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, "datFile.All DATs");

            string post = " (Duplicates)";
            DatFile dupeData = CreateDatFile(datFile.Header);
            dupeData.Header.SetFieldValue<string?>(DatHeader.FileNameKey, dupeData.Header.GetStringFieldValue(DatHeader.FileNameKey) + post);
            dupeData.Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, dupeData.Header.GetStringFieldValue(Models.Metadata.Header.NameKey) + post);
            dupeData.Header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, dupeData.Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey) + post);
            dupeData.ResetDictionary();

            watch.Stop();

            // Now, loop through the dictionary and populate the correct DATs
            watch.Start("Populating duplicate DAT");

#if NET452_OR_GREATER || NETCOREAPP
            Parallel.ForEach(datFile.Items.SortedKeys, Core.Globals.ParallelOptions, key =>
#elif NET40_OR_GREATER
            Parallel.ForEach(datFile.Items.SortedKeys, key =>
#else
            foreach (var key in datFile.Items.SortedKeys)
#endif
            {
                List<DatItem> items = Merge(datFile.GetItemsForBucket(key));

                // If the rom list is empty or null, just skip it
                if (items == null || items.Count == 0)
#if NET40_OR_GREATER || NETCOREAPP
                    return;
#else
                    continue;
#endif

                // Loop through and add the items correctly
                foreach (DatItem item in items)
                {
#if NET20 || NET35
                    if ((item.GetFieldValue<DupeType>(DatItem.DupeTypeKey) & DupeType.External) != 0)
#else
                    if (item.GetFieldValue<DupeType>(DatItem.DupeTypeKey).HasFlag(DupeType.External))
#endif
                    {
                        if (item.Clone() is not DatItem newrom)
                            continue;

                        if (item.GetFieldValue<Source?>(DatItem.SourceKey) != null)
                            newrom.GetFieldValue<Machine>(DatItem.MachineKey)!.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, newrom.GetFieldValue<Machine>(DatItem.MachineKey)!.GetStringFieldValue(Models.Metadata.Machine.NameKey) + $" ({Path.GetFileNameWithoutExtension(inputs[item.GetFieldValue<Source?>(DatItem.SourceKey)!.Index].CurrentPath)})");

                        dupeData.AddItem(newrom, statsOnly: false);
                    }
                }
#if NET40_OR_GREATER || NETCOREAPP
            });
#else
            }
#endif

            watch.Stop();

            return dupeData;
        }

        /// <summary>
        /// Output duplicate item diff
        /// </summary>
        /// <param name="datFile">Current DatFile object to use for updating</param>
        /// <param name="inputs">List of inputs to write out from</param>
        public static DatFile DiffDuplicatesDB(DatFile datFile, List<ParentablePath> inputs)
        {
            var watch = new InternalStopwatch("Initializing duplicate DAT");

            // Fill in any information not in the base DAT
            if (string.IsNullOrEmpty(datFile.Header.GetStringFieldValue(DatHeader.FileNameKey)))
                datFile.Header.SetFieldValue<string?>(DatHeader.FileNameKey, "All DATs");

            if (string.IsNullOrEmpty(datFile.Header.GetStringFieldValue(Models.Metadata.Header.NameKey)))
                datFile.Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, "datFile.All DATs");

            if (string.IsNullOrEmpty(datFile.Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey)))
                datFile.Header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, "datFile.All DATs");

            string post = " (Duplicates)";
            DatFile dupeData = CreateDatFile(datFile.Header);
            dupeData.Header.SetFieldValue<string?>(DatHeader.FileNameKey, dupeData.Header.GetStringFieldValue(DatHeader.FileNameKey) + post);
            dupeData.Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, dupeData.Header.GetStringFieldValue(Models.Metadata.Header.NameKey) + post);
            dupeData.Header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, dupeData.Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey) + post);
            dupeData.ResetDictionary();

            watch.Stop();

            // Now, loop through the dictionary and populate the correct DATs
            watch.Start("Populating duplicate DAT");

            // Get all current items, machines, and mappings
            var datItems = datFile.ItemsDB.GetItems();
            var machines = datFile.GetMachinesDB();
            var sources = datFile.ItemsDB.GetSources();

            // Create mappings from old index to new index
            var machineRemapping = new Dictionary<long, long>();
            var sourceRemapping = new Dictionary<long, long>();

            // Loop through and add all sources
            foreach (var source in sources)
            {
                long newSourceIndex = dupeData.AddSourceDB(source.Value);
                sourceRemapping[source.Key] = newSourceIndex;
            }

            // Loop through and add all machines
            foreach (var machine in machines)
            {
                long newMachineIndex = dupeData.AddMachineDB(machine.Value);
                machineRemapping[machine.Key] = newMachineIndex;
            }

            // Loop through and add the items
#if NET452_OR_GREATER || NETCOREAPP
            Parallel.ForEach(datItems, Core.Globals.ParallelOptions, item =>
#elif NET40_OR_GREATER
            Parallel.ForEach(datItems, item =>
#else
            foreach (var item in datItems)
#endif
            {
                // Get the machine and source index for this item
                long machineIndex = datFile.ItemsDB.GetMachineForItem(item.Key).Key;
                long sourceIndex = datFile.ItemsDB.GetSourceForItem(item.Key).Key;

                // If the current item isn't an external duplicate
#if NET20 || NET35
                if ((item.Value.GetFieldValue<DupeType>(DatItem.DupeTypeKey) & DupeType.External) == 0)
#else
                if (!item.Value.GetFieldValue<DupeType>(DatItem.DupeTypeKey).HasFlag(DupeType.External))
#endif
#if NET40_OR_GREATER || NETCOREAPP
                    return;
#else
                    continue;
#endif

                // Get the current source and machine
                var currentSource = sources[sourceIndex];
                string? currentMachineName = machines[machineIndex].GetStringFieldValue(Models.Metadata.Machine.NameKey);
                var currentMachine = datFile.ItemsDB.GetMachine(currentMachineName);
                if (currentMachine.Value == null)
#if NET40_OR_GREATER || NETCOREAPP
                    return;
#else
                    continue;
#endif

                // Get the source-specific machine
                string? renamedMachineName = $"{currentMachineName} ({Path.GetFileNameWithoutExtension(inputs[currentSource!.Index].CurrentPath)})";
                var renamedMachine = datFile.ItemsDB.GetMachine(renamedMachineName);
                if (renamedMachine.Value == null)
                {
                    var newMachine = currentMachine.Value.Clone() as Machine;
                    newMachine!.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, renamedMachineName);
                    long newMachineIndex = dupeData.AddMachineDB(newMachine!);
                    renamedMachine = new KeyValuePair<long, Machine?>(newMachineIndex, newMachine);
                }

                dupeData.AddItemDB(item.Value, renamedMachine.Key, sourceRemapping[sourceIndex], statsOnly: false);
#if NET40_OR_GREATER || NETCOREAPP
            });
#else
            }
#endif

            watch.Stop();

            return dupeData;
        }

        /// <summary>
        /// Output non-cascading diffs
        /// </summary>
        /// <param name="datFile">Current DatFile object to use for updating</param>
        /// <param name="inputs">List of inputs to write out from</param>
        public static List<DatFile> DiffIndividuals(DatFile datFile, List<string> inputs)
        {
            List<ParentablePath> paths = inputs.ConvertAll(i => new ParentablePath(i));
            return DiffIndividuals(datFile, paths);
            //return DiffIndividualsDB(datFile, paths);
        }

        /// <summary>
        /// Output non-cascading diffs
        /// </summary>
        /// <param name="datFile">Current DatFile object to use for updating</param>
        /// <param name="inputs">List of inputs to write out from</param>
        public static List<DatFile> DiffIndividuals(DatFile datFile, List<ParentablePath> inputs)
        {
            InternalStopwatch watch = new("Initializing all individual DATs");

            // Fill in any information not in the base DAT
            if (string.IsNullOrEmpty(datFile.Header.GetStringFieldValue(DatHeader.FileNameKey)))
                datFile.Header.SetFieldValue<string?>(DatHeader.FileNameKey, "All DATs");

            if (string.IsNullOrEmpty(datFile.Header.GetStringFieldValue(Models.Metadata.Header.NameKey)))
                datFile.Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, "All DATs");

            if (string.IsNullOrEmpty(datFile.Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey)))
                datFile.Header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, "All DATs");

            // Loop through each of the inputs and get or create a new DatData object
            DatFile[] outDatsArray = new DatFile[inputs.Count];

#if NET452_OR_GREATER || NETCOREAPP
            Parallel.For(0, inputs.Count, Core.Globals.ParallelOptions, j =>
#elif NET40_OR_GREATER
            Parallel.For(0, inputs.Count, j =>
#else
            for (int j = 0; j < inputs.Count; j++)
#endif
            {
                string innerpost = $" ({j} - {inputs[j].GetNormalizedFileName(true)} Only)";
                DatFile diffData = CreateDatFile(datFile.Header);
                diffData.Header.SetFieldValue<string?>(DatHeader.FileNameKey, diffData.Header.GetStringFieldValue(DatHeader.FileNameKey) + innerpost);
                diffData.Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, diffData.Header.GetStringFieldValue(Models.Metadata.Header.NameKey) + innerpost);
                diffData.Header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, diffData.Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey) + innerpost);
                diffData.ResetDictionary();
                outDatsArray[j] = diffData;
#if NET40_OR_GREATER || NETCOREAPP
            });
#else
            }
#endif

            // Create a list of DatData objects representing individual output files
            List<DatFile> outDats = [.. outDatsArray];

            watch.Stop();

            // Now, loop through the dictionary and populate the correct DATs
            watch.Start("Populating all individual DATs");

#if NET452_OR_GREATER || NETCOREAPP
            Parallel.ForEach(datFile.Items.SortedKeys, Core.Globals.ParallelOptions, key =>
#elif NET40_OR_GREATER
            Parallel.ForEach(datFile.Items.SortedKeys, key =>
#else
            foreach (var key in datFile.Items.SortedKeys)
#endif
            {
                List<DatItem> items = Merge(datFile.GetItemsForBucket(key));

                // If the rom list is empty or null, just skip it
                if (items == null || items.Count == 0)
#if NET40_OR_GREATER || NETCOREAPP
                    return;
#else
                    continue;
#endif

                // Loop through and add the items correctly
                foreach (DatItem item in items)
                {
                    if (item.GetFieldValue<Source?>(DatItem.SourceKey) == null)
                        continue;

#if NET20 || NET35
                    if ((item.GetFieldValue<DupeType>(DatItem.DupeTypeKey) & DupeType.Internal) != 0 || item.GetFieldValue<DupeType>(DatItem.DupeTypeKey) == 0x00)
#else
                    if (item.GetFieldValue<DupeType>(DatItem.DupeTypeKey).HasFlag(DupeType.Internal) || item.GetFieldValue<DupeType>(DatItem.DupeTypeKey) == 0x00)
#endif
                        outDats[item.GetFieldValue<Source?>(DatItem.SourceKey)!.Index].AddItem(item, statsOnly: false);
                }
#if NET40_OR_GREATER || NETCOREAPP
            });
#else
            }
#endif

            watch.Stop();

            return [.. outDats];
        }

        /// <summary>
        /// Output non-cascading diffs
        /// </summary>
        /// <param name="datFile">Current DatFile object to use for updating</param>
        /// <param name="inputs">List of inputs to write out from</param>
        public static List<DatFile> DiffIndividualsDB(DatFile datFile, List<ParentablePath> inputs)
        {
            InternalStopwatch watch = new("Initializing all individual DATs");

            // Fill in any information not in the base DAT
            if (string.IsNullOrEmpty(datFile.Header.GetStringFieldValue(DatHeader.FileNameKey)))
                datFile.Header.SetFieldValue<string?>(DatHeader.FileNameKey, "All DATs");

            if (string.IsNullOrEmpty(datFile.Header.GetStringFieldValue(Models.Metadata.Header.NameKey)))
                datFile.Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, "All DATs");

            if (string.IsNullOrEmpty(datFile.Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey)))
                datFile.Header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, "All DATs");

            // Loop through each of the inputs and get or create a new DatData object
            DatFile[] outDatsArray = new DatFile[inputs.Count];

#if NET452_OR_GREATER || NETCOREAPP
            Parallel.For(0, inputs.Count, Core.Globals.ParallelOptions, j =>
#elif NET40_OR_GREATER
            Parallel.For(0, inputs.Count, j =>
#else
            for (int j = 0; j < inputs.Count; j++)
#endif
            {
                string innerpost = $" ({j} - {inputs[j].GetNormalizedFileName(true)} Only)";
                DatFile diffData = CreateDatFile(datFile.Header);
                diffData.Header.SetFieldValue<string?>(DatHeader.FileNameKey, diffData.Header.GetStringFieldValue(DatHeader.FileNameKey) + innerpost);
                diffData.Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, diffData.Header.GetStringFieldValue(Models.Metadata.Header.NameKey) + innerpost);
                diffData.Header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, diffData.Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey) + innerpost);
                diffData.ResetDictionary();
                outDatsArray[j] = diffData;
#if NET40_OR_GREATER || NETCOREAPP
            });
#else
            }
#endif

            // Create a list of DatData objects representing individual output files
            List<DatFile> outDats = [.. outDatsArray];

            watch.Stop();

            // Now, loop through the dictionary and populate the correct DATs
            watch.Start("Populating all individual DATs");

            // Get all current items, machines, and mappings
            var datItems = datFile.ItemsDB.GetItems();
            var machines = datFile.GetMachinesDB();
            var sources = datFile.ItemsDB.GetSources();

            // Create mappings from old index to new index
            var machineRemapping = new Dictionary<long, long>();
            var sourceRemapping = new Dictionary<long, long>();

            // Loop through and add all sources
            foreach (var source in sources)
            {
                long newSourceIndex = outDats[0].AddSourceDB(source.Value);
                sourceRemapping[source.Key] = newSourceIndex;

                for (int i = 1; i < outDats.Count; i++)
                {
                    _ = outDats[i].AddSourceDB(source.Value);
                }
            }

            // Loop through and add all machines
            foreach (var machine in machines)
            {
                long newMachineIndex = outDats[0].AddMachineDB(machine.Value);
                machineRemapping[machine.Key] = newMachineIndex;

                for (int i = 1; i < outDats.Count; i++)
                {
                    _ = outDats[i].AddMachineDB(machine.Value);
                }
            }

            // Loop through and add the items
#if NET452_OR_GREATER || NETCOREAPP
            Parallel.ForEach(datItems, Core.Globals.ParallelOptions, item =>
#elif NET40_OR_GREATER
            Parallel.ForEach(datItems, item =>
#else
            foreach (var item in datItems)
#endif
            {
                // Get the machine and source index for this item
                long machineIndex = datFile.ItemsDB.GetMachineForItem(item.Key).Key;
                long sourceIndex = datFile.ItemsDB.GetSourceForItem(item.Key).Key;

                // Get the source associated with the item
                var source = datFile.ItemsDB.GetSource(sourceIndex);
                if (source == null)
#if NET40_OR_GREATER || NETCOREAPP
                    return;
#else
                    continue;
#endif

#if NET20 || NET35
                if ((item.Value.GetFieldValue<DupeType>(DatItem.DupeTypeKey) & DupeType.Internal) != 0 || item.Value.GetFieldValue<DupeType>(DatItem.DupeTypeKey) == 0x00)
#else
                if (item.Value.GetFieldValue<DupeType>(DatItem.DupeTypeKey).HasFlag(DupeType.Internal) || item.Value.GetFieldValue<DupeType>(DatItem.DupeTypeKey) == 0x00)
#endif
                    outDats[source.Index].AddItemDB(item.Value, machineRemapping[machineIndex], sourceRemapping[sourceIndex], statsOnly: false);
#if NET40_OR_GREATER || NETCOREAPP
            });
#else
            }
#endif

            watch.Stop();

            return [.. outDats];
        }

        /// <summary>
        /// Output non-duplicate item diff
        /// </summary>
        /// <param name="datFile">Current DatFile object to use for updating</param>
        /// <param name="inputs">List of inputs to write out from</param>
        public static DatFile DiffNoDuplicates(DatFile datFile, List<string> inputs)
        {
            List<ParentablePath> paths = inputs.ConvertAll(i => new ParentablePath(i));
            return DiffNoDuplicates(datFile, paths);
            //return DiffNoDuplicatesDB(datFile, paths);
        }

        /// <summary>
        /// Output non-duplicate item diff
        /// </summary>
        /// <param name="datFile">Current DatFile object to use for updating</param>
        /// <param name="inputs">List of inputs to write out from</param>
        public static DatFile DiffNoDuplicates(DatFile datFile, List<ParentablePath> inputs)
        {
            InternalStopwatch watch = new("Initializing no duplicate DAT");

            // Fill in any information not in the base DAT
            if (string.IsNullOrEmpty(datFile.Header.GetStringFieldValue(DatHeader.FileNameKey)))
                datFile.Header.SetFieldValue<string?>(DatHeader.FileNameKey, "All DATs");

            if (string.IsNullOrEmpty(datFile.Header.GetStringFieldValue(Models.Metadata.Header.NameKey)))
                datFile.Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, "All DATs");

            if (string.IsNullOrEmpty(datFile.Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey)))
                datFile.Header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, "All DATs");

            string post = " (No Duplicates)";
            DatFile outerDiffData = CreateDatFile(datFile.Header);
            outerDiffData.Header.SetFieldValue<string?>(DatHeader.FileNameKey, outerDiffData.Header.GetStringFieldValue(DatHeader.FileNameKey) + post);
            outerDiffData.Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, outerDiffData.Header.GetStringFieldValue(Models.Metadata.Header.NameKey) + post);
            outerDiffData.Header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, outerDiffData.Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey) + post);
            outerDiffData.ResetDictionary();

            watch.Stop();

            // Now, loop through the dictionary and populate the correct DATs
            watch.Start("Populating no duplicate DAT");

#if NET452_OR_GREATER || NETCOREAPP
            Parallel.ForEach(datFile.Items.SortedKeys, Core.Globals.ParallelOptions, key =>
#elif NET40_OR_GREATER
            Parallel.ForEach(datFile.Items.SortedKeys, key =>
#else
            foreach (var key in datFile.Items.SortedKeys)
#endif
            {
                List<DatItem> items = Merge(datFile.GetItemsForBucket(key));

                // If the rom list is empty or null, just skip it
                if (items == null || items.Count == 0)
#if NET40_OR_GREATER || NETCOREAPP
                    return;
#else
                    continue;
#endif

                // Loop through and add the items correctly
                foreach (DatItem item in items)
                {
#if NET20 || NET35
                    if ((item.GetFieldValue<DupeType>(DatItem.DupeTypeKey) & DupeType.Internal) != 0 || item.GetFieldValue<DupeType>(DatItem.DupeTypeKey) == 0x00)
#else
                    if (item.GetFieldValue<DupeType>(DatItem.DupeTypeKey).HasFlag(DupeType.Internal) || item.GetFieldValue<DupeType>(DatItem.DupeTypeKey) == 0x00)
#endif
                    {
                        if (item.Clone() is not DatItem newrom || newrom.GetFieldValue<Source?>(DatItem.SourceKey) == null)
                            continue;

                        newrom.GetFieldValue<Machine>(DatItem.MachineKey)!.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, newrom.GetFieldValue<Machine>(DatItem.MachineKey)!.GetStringFieldValue(Models.Metadata.Machine.NameKey) + $" ({Path.GetFileNameWithoutExtension(inputs[newrom.GetFieldValue<Source?>(DatItem.SourceKey)!.Index].CurrentPath)})");
                        outerDiffData.AddItem(newrom, statsOnly: false);
                    }
                }
#if NET40_OR_GREATER || NETCOREAPP
            });
#else
            }
#endif

            watch.Stop();

            return outerDiffData;
        }

        /// <summary>
        /// Output non-duplicate item diff
        /// </summary>
        /// <param name="datFile">Current DatFile object to use for updating</param>
        /// <param name="inputs">List of inputs to write out from</param>
        public static DatFile DiffNoDuplicatesDB(DatFile datFile, List<ParentablePath> inputs)
        {
            var watch = new InternalStopwatch("Initializing no duplicate DAT");

            // Fill in any information not in the base DAT
            if (string.IsNullOrEmpty(datFile.Header.GetStringFieldValue(DatHeader.FileNameKey)))
                datFile.Header.SetFieldValue<string?>(DatHeader.FileNameKey, "All DATs");

            if (string.IsNullOrEmpty(datFile.Header.GetStringFieldValue(Models.Metadata.Header.NameKey)))
                datFile.Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, "All DATs");

            if (string.IsNullOrEmpty(datFile.Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey)))
                datFile.Header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, "All DATs");

            string post = " (No Duplicates)";
            DatFile outerDiffData = CreateDatFile(datFile.Header);
            outerDiffData.Header.SetFieldValue<string?>(DatHeader.FileNameKey, outerDiffData.Header.GetStringFieldValue(DatHeader.FileNameKey) + post);
            outerDiffData.Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, outerDiffData.Header.GetStringFieldValue(Models.Metadata.Header.NameKey) + post);
            outerDiffData.Header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, outerDiffData.Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey) + post);
            outerDiffData.ResetDictionary();

            watch.Stop();

            // Now, loop through the dictionary and populate the correct DATs
            watch.Start("Populating no duplicate DAT");

            // Get all current items, machines, and mappings
            var datItems = datFile.ItemsDB.GetItems();
            var machines = datFile.GetMachinesDB();
            var sources = datFile.ItemsDB.GetSources();

            // Create mappings from old index to new index
            var machineRemapping = new Dictionary<long, long>();
            var sourceRemapping = new Dictionary<long, long>();

            // Loop through and add all sources
            foreach (var source in sources)
            {
                long newSourceIndex = outerDiffData.AddSourceDB(source.Value);
                sourceRemapping[source.Key] = newSourceIndex;
            }

            // Loop through and add all machines
            foreach (var machine in machines)
            {
                long newMachineIndex = outerDiffData.AddMachineDB(machine.Value);
                machineRemapping[machine.Key] = newMachineIndex;
            }

            // Loop through and add the items
#if NET452_OR_GREATER || NETCOREAPP
            Parallel.ForEach(datItems, Core.Globals.ParallelOptions, item =>
#elif NET40_OR_GREATER
            Parallel.ForEach(datItems, item =>
#else
            foreach (var item in datItems)
#endif
            {
                // Get the machine and source index for this item
                long machineIndex = datFile.ItemsDB.GetMachineForItem(item.Key).Key;
                long sourceIndex = datFile.ItemsDB.GetSourceForItem(item.Key).Key;

                // If the current item isn't a duplicate
#if NET20 || NET35
                if ((item.Value.GetFieldValue<DupeType>(DatItem.DupeTypeKey) & DupeType.Internal) == 0 && item.Value.GetFieldValue<DupeType>(DatItem.DupeTypeKey) != 0x00)
#else
                if (!item.Value.GetFieldValue<DupeType>(DatItem.DupeTypeKey).HasFlag(DupeType.Internal) && item.Value.GetFieldValue<DupeType>(DatItem.DupeTypeKey) != 0x00)
#endif
#if NET40_OR_GREATER || NETCOREAPP
                    return;
#else
                    continue;
#endif

                // Get the current source and machine
                var currentSource = sources[sourceIndex];
                string? currentMachineName = machines[machineIndex].GetStringFieldValue(Models.Metadata.Machine.NameKey);
                var currentMachine = datFile.ItemsDB.GetMachine(currentMachineName);
                if (currentMachine.Value == null)
#if NET40_OR_GREATER || NETCOREAPP
                    return;
#else
                    continue;
#endif

                // Get the source-specific machine
                string? renamedMachineName = $"{currentMachineName} ({Path.GetFileNameWithoutExtension(inputs[currentSource!.Index].CurrentPath)})";
                var renamedMachine = datFile.ItemsDB.GetMachine(renamedMachineName);
                if (renamedMachine.Value == null)
                {
                    var newMachine = currentMachine.Value.Clone() as Machine;
                    newMachine!.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, renamedMachineName);
                    long newMachineIndex = outerDiffData.AddMachineDB(newMachine);
                    renamedMachine = new KeyValuePair<long, Machine?>(newMachineIndex, newMachine);
                }

                outerDiffData.AddItemDB(item.Value, renamedMachine.Key, sourceRemapping[sourceIndex], statsOnly: false);
#if NET40_OR_GREATER || NETCOREAPP
            });
#else
            }
#endif

            watch.Stop();

            return outerDiffData;
        }

        #endregion

        #region Population

        /// <summary>
        /// Populate from multiple paths while returning the invividual headers
        /// </summary>
        /// <param name="datFile">Current DatFile object to use for updating</param>
        /// <param name="inputs">Paths to DATs to parse</param>
        /// <returns>List of DatHeader objects representing headers</returns>
        public static List<DatHeader> PopulateUserData(DatFile datFile, List<string> inputs)
        {
            List<ParentablePath> paths = inputs.ConvertAll(i => new ParentablePath(i));
            return PopulateUserData(datFile, paths);
        }

        /// <summary>
        /// Populate from multiple paths while returning the invividual headers
        /// </summary>
        /// <param name="datFile">Current DatFile object to use for updating</param>
        /// <param name="inputs">Paths to DATs to parse</param>
        /// <returns>List of DatHeader objects representing headers</returns>
        public static List<DatHeader> PopulateUserData(DatFile datFile, List<ParentablePath> inputs)
        {
            DatFile[] datFiles = new DatFile[inputs.Count];
            InternalStopwatch watch = new("Processing individual DATs");

            // Parse all of the DATs into their own DatFiles in the array
#if NET452_OR_GREATER || NETCOREAPP
            Parallel.For(0, inputs.Count, Core.Globals.ParallelOptions, i =>
#elif NET40_OR_GREATER
            Parallel.For(0, inputs.Count, i =>
#else
            for (int i = 0; i < inputs.Count; i++)
#endif
            {
                var input = inputs[i];
                _staticLogger.User($"Adding DAT: {input.CurrentPath}");
                datFiles[i] = CreateDatFile(datFile.Header.CloneFiltering());
                Parser.ParseInto(datFiles[i], input, i, keep: true);
#if NET40_OR_GREATER || NETCOREAPP
            });
#else
            }
#endif

            watch.Stop();

            watch.Start("Populating internal DAT");
            for (int i = 0; i < inputs.Count; i++)
            {
                AddFromExisting(datFile, datFiles[i], true);
                //AddFromExistingDB(datFile, datFiles[i], true);
            }

            watch.Stop();

            return [.. Array.ConvertAll(datFiles, d => d.Header)];
        }

        /// <summary>
        /// Add items from another DatFile to the existing DatFile
        /// </summary>
        /// <param name="addTo">DatFile to add to</param>
        /// <param name="addFrom">DatFile to add from</param>
        /// <param name="delete">If items should be deleted from the source DatFile</param>
        private static void AddFromExisting(DatFile addTo, DatFile addFrom, bool delete = false)
        {
            // Get the list of keys from the DAT
            foreach (string key in addFrom.Items.SortedKeys)
            {
                // Add everything from the key to the internal DAT
                addFrom.GetItemsForBucket(key).ForEach(item => addTo.AddItem(item, statsOnly: false));

                // Now remove the key from the source DAT
                if (delete)
                    addFrom.RemoveBucket(key);
            }

            // Now remove the file dictionary from the source DAT
            if (delete)
                addFrom.ResetDictionary();
        }

        /// <summary>
        /// Add items from another DatFile to the existing DatFile
        /// </summary>
        /// <param name="addTo">DatFile to add to</param>
        /// <param name="addFrom">DatFile to add from</param>
        /// <param name="delete">If items should be deleted from the source DatFile</param>
        private static void AddFromExistingDB(DatFile addTo, DatFile addFrom, bool delete = false)
        {
            // Get all current items, machines, and mappings
            var datItems = addFrom.ItemsDB.GetItems();
            var machines = addFrom.GetMachinesDB();
            var sources = addFrom.ItemsDB.GetSources();

            // Create mappings from old index to new index
            var machineRemapping = new Dictionary<long, long>();
            var sourceRemapping = new Dictionary<long, long>();

            // Loop through and add all sources
            foreach (var source in sources)
            {
                long newSourceIndex = addTo.AddSourceDB(source.Value);
                sourceRemapping[source.Key] = newSourceIndex;
            }

            // Loop through and add all machines
            foreach (var machine in machines)
            {
                long newMachineIndex = addTo.AddMachineDB(machine.Value);
                machineRemapping[machine.Key] = newMachineIndex;
            }

            // Loop through and add the items
#if NET452_OR_GREATER || NETCOREAPP
            Parallel.ForEach(datItems, Core.Globals.ParallelOptions, item =>
#elif NET40_OR_GREATER
            Parallel.ForEach(datItems, item =>
#else
            foreach (var item in datItems)
#endif
            {
                // Get the machine and source index for this item
                long machineIndex = addFrom.ItemsDB.GetMachineForItem(item.Key).Key;
                long sourceIndex = addFrom.ItemsDB.GetSourceForItem(item.Key).Key;

                addTo.AddItemDB(item.Value, machineRemapping[machineIndex], sourceRemapping[sourceIndex], statsOnly: false);

                // Now remove the key from the source DAT
                if (delete)
                    addFrom.RemoveItemDB(item.Key);

#if NET40_OR_GREATER || NETCOREAPP
            });
#else
            }
#endif

            // Now remove the file dictionary from the source DAT
            if (delete)
                addFrom.ResetDictionary();
        }

        /// <summary>
        /// Fill a DatFile with all items with a particular source index ID
        /// </summary>
        /// <param name="datFile">Current DatFile object to use for updating</param>
        /// <param name="indexDat">DatFile to add found items to</param>
        /// <param name="index">Source index ID to retrieve items for</param>
        /// <returns>DatFile containing all items with the source index ID/returns>
        private static void FillWithSourceIndex(DatFile datFile, DatFile indexDat, int index)
        {
            // Loop through and add the items for this index to the output
#if NET452_OR_GREATER || NETCOREAPP
            Parallel.ForEach(datFile.Items.SortedKeys, Core.Globals.ParallelOptions, key =>
#elif NET40_OR_GREATER
            Parallel.ForEach(datFile.Items.SortedKeys, key =>
#else
            foreach (var key in datFile.Items.SortedKeys)
#endif
            {
                List<DatItem> items = Merge(datFile.GetItemsForBucket(key));

                // If the rom list is empty or null, just skip it
                if (items == null || items.Count == 0)
#if NET40_OR_GREATER || NETCOREAPP
                    return;
#else
                    continue;
#endif

                foreach (DatItem item in items)
                {
                    var source = item.GetFieldValue<Source?>(DatItem.SourceKey);
                    if (source != null && source.Index == index)
                        indexDat.AddItem(item, statsOnly: false);
                }
#if NET40_OR_GREATER || NETCOREAPP
            });
#else
            }
#endif
        }

        /// <summary>
        /// Fill a DatFile with all items with a particular source index ID
        /// </summary>
        /// <param name="datFile">Current DatFile object to use for updating</param>
        /// <param name="indexDat">DatFile to add found items to</param>
        /// <param name="index">Source index ID to retrieve items for</param>
        /// <returns>DatFile containing all items with the source index ID/returns>
        private static void FillWithSourceIndexDB(DatFile datFile, DatFile indexDat, int index)
        {
            // Get all current items, machines, and mappings
            var datItems = datFile.ItemsDB.GetItems();
            var machines = datFile.GetMachinesDB();
            var sources = datFile.ItemsDB.GetSources();

            // Create mappings from old index to new index
            var machineRemapping = new Dictionary<long, long>();
            var sourceRemapping = new Dictionary<long, long>();

            // Loop through and add all sources
            foreach (var source in sources)
            {
                long newSourceIndex = indexDat.AddSourceDB(source.Value);
                sourceRemapping[source.Key] = newSourceIndex;
            }

            // Loop through and add all machines
            foreach (var machine in machines)
            {
                long newMachineIndex = indexDat.AddMachineDB(machine.Value);
                machineRemapping[machine.Key] = newMachineIndex;
            }

            // Loop through and add the items
#if NET452_OR_GREATER || NETCOREAPP
            Parallel.ForEach(datItems, Core.Globals.ParallelOptions, item =>
#elif NET40_OR_GREATER
            Parallel.ForEach(datItems, item =>
#else
            foreach (var item in datItems)
#endif
            {
                // Get the machine and source index for this item
                long machineIndex = datFile.ItemsDB.GetMachineForItem(item.Key).Key;
                long sourceIndex = datFile.ItemsDB.GetSourceForItem(item.Key).Key;

                // Get the source associated with the item
                var source = datFile.ItemsDB.GetSource(sourceIndex);

                if (source != null && source.Index == index)
                    indexDat.AddItemDB(item.Value, machineRemapping[machineIndex], sourceRemapping[sourceIndex], statsOnly: false);
#if NET40_OR_GREATER || NETCOREAPP
            });
#else
            }
#endif
        }

        #endregion
    }
}
