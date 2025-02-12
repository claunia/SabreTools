using System;
using System.Collections.Generic;
using System.IO;
#if NET40_OR_GREATER || NETCOREAPP
using System.Threading.Tasks;
#endif
using SabreTools.Core.Tools;
using SabreTools.DatFiles.Formats;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;
using SabreTools.IO;
using SabreTools.IO.Logging;

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
        /// <param name="datModifiers">DatModifiers to get the values from</param>
        public static DatFile CreateDatFile(DatHeader datHeader, DatModifiers datModifiers)
        {
            DatFormat format = datHeader.GetFieldValue<DatFormat>(DatHeader.DatFormatKey);
            DatFile datFile = CreateDatFile(format);
            datFile.SetHeader(datHeader);
            datFile.SetModifiers(datModifiers);
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
                datFiles[i] = CreateDatFile(datFile.Header.CloneFormat(), datFile.Modifiers);
                Parser.ParseInto(datFiles[i], input.CurrentPath, i, keep: true);
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

        #endregion
    }
}
