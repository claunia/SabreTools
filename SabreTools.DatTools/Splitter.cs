using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
#if NET40_OR_GREATER || NETCOREAPP
using System.Threading.Tasks;
#endif
using SabreTools.Core.Tools;
using SabreTools.DatFiles;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;
using SabreTools.IO.Extensions;
using SabreTools.IO.Logging;
using SabreTools.Matching.Compare;

namespace SabreTools.DatTools
{
    /// <summary>
    /// Helper methods for splitting DatFiles
    /// </summary>
    /// <remarks>TODO: Implement Level split</remarks>
    public class Splitter
    {
        #region Logging

        /// <summary>
        /// Logging object
        /// </summary>
        private static readonly Logger _staticLogger = new();

        #endregion

        /// <summary>
        /// Split a DAT by input extensions
        /// </summary>
        /// <param name="datFile">Current DatFile object to split</param>
        /// <param name="extA">List of extensions to split on (first DAT)</param>
        /// <param name="extB">List of extensions to split on (second DAT)</param>
        /// <returns>Extension Set A and Extension Set B DatFiles</returns>
        public static (DatFile? extADat, DatFile? extBDat) SplitByExtension(DatFile datFile, List<string> extA, List<string> extB)
        {
            // If roms is empty, return false
            if (datFile.DatStatistics.TotalCount == 0)
                return (null, null);

            InternalStopwatch watch = new($"Splitting DAT by extension");

            // Make sure all of the extensions don't have a dot at the beginning
            var newExtA = extA.ConvertAll(s => s.TrimStart('.').ToLowerInvariant()).ToArray();
            string newExtAString = string.Join(",", newExtA);

            var newExtB = extB.ConvertAll(s => s.TrimStart('.').ToLowerInvariant()).ToArray();
            string newExtBString = string.Join(",", newExtB);

            // Set all of the appropriate outputs for each of the subsets
            DatFile extADat = DatFileTool.CreateDatFile(datFile.Header.CloneStandard());
            extADat.Header.SetFieldValue<string?>(DatHeader.FileNameKey, extADat.Header.GetStringFieldValue(DatHeader.FileNameKey) + $" ({newExtAString})");
            extADat.Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, extADat.Header.GetStringFieldValue(Models.Metadata.Header.NameKey) + $" ({newExtAString})");
            extADat.Header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, extADat.Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey) + $" ({newExtAString})");

            DatFile extBDat = DatFileTool.CreateDatFile(datFile.Header.CloneStandard());
            extBDat.Header.SetFieldValue<string?>(DatHeader.FileNameKey, extBDat.Header.GetStringFieldValue(DatHeader.FileNameKey) + $" ({newExtBString})");
            extBDat.Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, extBDat.Header.GetStringFieldValue(Models.Metadata.Header.NameKey) + $" ({newExtBString})");
            extBDat.Header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, extBDat.Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey) + $" ({newExtBString})");

            // Now separate the roms accordingly
#if NET452_OR_GREATER || NETCOREAPP
            Parallel.ForEach(datFile.Items.Keys, Core.Globals.ParallelOptions, key =>
#elif NET40_OR_GREATER
            Parallel.ForEach(datFile.Items.Keys, key =>
#else
            foreach (var key in datFile.Items.Keys)
#endif
            {
                var items = datFile.GetItemsForBucket(key);
                if (items == null)
#if NET40_OR_GREATER || NETCOREAPP
                    return;
#else
                    continue;
#endif

                foreach (DatItem item in items)
                {
                    if (Array.IndexOf(newExtA, (item.GetName() ?? string.Empty).GetNormalizedExtension()) > -1)
                    {
                        extADat.Add(key, item);
                    }
                    if (Array.IndexOf(newExtB, (item.GetName() ?? string.Empty).GetNormalizedExtension()) > -1)
                    {
                        extBDat.Add(key, item);
                    }
                    else
                    {
                        extADat.Add(key, item);
                        extBDat.Add(key, item);
                    }
                }
#if NET40_OR_GREATER || NETCOREAPP
            });
#else
            }
#endif

            // Then return both DatFiles
            watch.Stop();
            return (extADat, extBDat);
        }

        /// <summary>
        /// Split a DAT by input extensions
        /// </summary>
        /// <param name="datFile">Current DatFile object to split</param>
        /// <param name="extA">List of extensions to split on (first DAT)</param>
        /// <param name="extB">List of extensions to split on (second DAT)</param>
        /// <returns>Extension Set A and Extension Set B DatFiles</returns>
        public static (DatFile? extADat, DatFile? extBDat) SplitByExtensionDB(DatFile datFile, List<string> extA, List<string> extB)
        {
            // If roms is empty, return false
            if (datFile.ItemsDB.DatStatistics.TotalCount == 0)
                return (null, null);

            InternalStopwatch watch = new($"Splitting DAT by extension");

            // Make sure all of the extensions don't have a dot at the beginning
            var newExtA = extA.ConvertAll(s => s.TrimStart('.').ToLowerInvariant()).ToArray();
            string newExtAString = string.Join(",", newExtA);

            var newExtB = extB.ConvertAll(s => s.TrimStart('.').ToLowerInvariant()).ToArray();
            string newExtBString = string.Join(",", newExtB);

            // Set all of the appropriate outputs for each of the subsets
            DatFile extADat = DatFileTool.CreateDatFile(datFile.Header.CloneStandard());
            extADat.Header.SetFieldValue<string?>(DatHeader.FileNameKey, extADat.Header.GetStringFieldValue(DatHeader.FileNameKey) + $" ({newExtAString})");
            extADat.Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, extADat.Header.GetStringFieldValue(Models.Metadata.Header.NameKey) + $" ({newExtAString})");
            extADat.Header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, extADat.Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey) + $" ({newExtAString})");

            DatFile extBDat = DatFileTool.CreateDatFile(datFile.Header.CloneStandard());
            extBDat.Header.SetFieldValue<string?>(DatHeader.FileNameKey, extBDat.Header.GetStringFieldValue(DatHeader.FileNameKey) + $" ({newExtBString})");
            extBDat.Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, extBDat.Header.GetStringFieldValue(Models.Metadata.Header.NameKey) + $" ({newExtBString})");
            extBDat.Header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, extBDat.Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey) + $" ({newExtBString})");

            // Get all current items, machines, and mappings
            var datItems = datFile.ItemsDB.GetItems();
            var machines = datFile.ItemsDB.GetMachines();
            var sources = datFile.ItemsDB.GetSources();
            var itemMachineMappings = datFile.ItemsDB.GetItemMachineMappings();
            var itemSourceMappings = datFile.ItemsDB.GetItemSourceMappings();

            // Create mappings from old index to new index
            var machineRemapping = new Dictionary<long, long>();
            var sourceRemapping = new Dictionary<long, long>();

            // Loop through and add all sources
            foreach (var source in sources)
            {
                long newSourceIndex = extADat.AddSourceDB(source.Value);
                _ = extBDat.AddSourceDB(source.Value);
                sourceRemapping[source.Key] = newSourceIndex;
            }

            // Loop through and add all machines
            foreach (var machine in machines)
            {
                long newMachineIndex = extADat.AddMachineDB(machine.Value);
                _ = extBDat.AddMachineDB(machine.Value);
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
                long machineIndex = itemMachineMappings[item.Key];
                long sourceIndex = itemSourceMappings[item.Key];

                if (newExtA.Contains((item.Value.GetName() ?? string.Empty).GetNormalizedExtension()))
                {
                    extADat.AddItemDB(item.Value, machineRemapping[machineIndex], sourceRemapping[sourceIndex], statsOnly: false);
                }
                else if (newExtB.Contains((item.Value.GetName() ?? string.Empty).GetNormalizedExtension()))
                {
                    extBDat.AddItemDB(item.Value, machineRemapping[machineIndex], sourceRemapping[sourceIndex], statsOnly: false);
                }
                else
                {
                    extADat.AddItemDB(item.Value, machineRemapping[machineIndex], sourceRemapping[sourceIndex], statsOnly: false);
                    extBDat.AddItemDB(item.Value, machineRemapping[machineIndex], sourceRemapping[sourceIndex], statsOnly: false);
                }
#if NET40_OR_GREATER || NETCOREAPP
            });
#else
            }
#endif

            // Then return both DatFiles
            watch.Stop();
            return (extADat, extBDat);
        }

        /// <summary>
        /// Split a DAT by best available hashes
        /// </summary>
        /// <param name="datFile">Current DatFile object to split</param>
        /// <returns>Dictionary of Field to DatFile mappings</returns>
        public static Dictionary<string, DatFile> SplitByHash(DatFile datFile)
        {
            // Create each of the respective output DATs
            var watch = new InternalStopwatch($"Splitting DAT by best available hashes");

            // Create mapping of keys to suffixes
            var mappings = new Dictionary<string, string>
            {
                [Models.Metadata.Rom.StatusKey] = " (Nodump)",
                [Models.Metadata.Rom.SHA512Key] = " (SHA-512)",
                [Models.Metadata.Rom.SHA384Key] = " (SHA-384)",
                [Models.Metadata.Rom.SHA256Key] = " (SHA-256)",
                [Models.Metadata.Rom.SHA1Key] = " (SHA-1)",
                [Models.Metadata.Rom.MD5Key] = " (MD5)",
                [Models.Metadata.Rom.MD4Key] = " (MD4)",
                [Models.Metadata.Rom.MD2Key] = " (MD2)",
                [Models.Metadata.Rom.CRCKey] = " (CRC)",
                ["null"] = " (Other)",
            };

            // Create the set of field-to-dat mappings
            Dictionary<string, DatFile> fieldDats = [];
            foreach (var kvp in mappings)
            {
                fieldDats[kvp.Key] = DatFileTool.CreateDatFile(datFile.Header.CloneStandard());
                fieldDats[kvp.Key].Header.SetFieldValue<string?>(DatHeader.FileNameKey, fieldDats[kvp.Key].Header.GetStringFieldValue(DatHeader.FileNameKey) + kvp.Value);
                fieldDats[kvp.Key].Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, fieldDats[kvp.Key].Header.GetStringFieldValue(Models.Metadata.Header.NameKey) + kvp.Value);
                fieldDats[kvp.Key].Header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, fieldDats[kvp.Key].Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey) + kvp.Value);
            }

            // Now populate each of the DAT objects in turn
#if NET452_OR_GREATER || NETCOREAPP
            Parallel.ForEach(datFile.Items.Keys, Core.Globals.ParallelOptions, key =>
#elif NET40_OR_GREATER
            Parallel.ForEach(datFile.Items.Keys, key =>
#else
            foreach (var key in datFile.Items.Keys)
#endif
            {
                var items = datFile.GetItemsForBucket(key);
                if (items == null)
#if NET40_OR_GREATER || NETCOREAPP
                    return;
#else
                    continue;
#endif
                foreach (DatItem item in items)
                {
                    // If the file is not a Disk, Media, or Rom, continue
                    switch (item)
                    {
                        case Disk disk:
                            if (disk.GetStringFieldValue(Models.Metadata.Disk.StatusKey).AsEnumValue<ItemStatus>() == ItemStatus.Nodump)
                                fieldDats[Models.Metadata.Disk.StatusKey].Add(key, item);
                            else if (!string.IsNullOrEmpty(disk.GetStringFieldValue(Models.Metadata.Disk.SHA1Key)))
                                fieldDats[Models.Metadata.Disk.SHA1Key].Add(key, item);
                            else if (!string.IsNullOrEmpty(disk.GetStringFieldValue(Models.Metadata.Disk.MD5Key)))
                                fieldDats[Models.Metadata.Disk.MD5Key].Add(key, item);
                            else if (!string.IsNullOrEmpty(disk.GetStringFieldValue(Models.Metadata.Disk.MD5Key)))
                                fieldDats[Models.Metadata.Disk.MD5Key].Add(key, item);
                            else
                                fieldDats["null"].Add(key, item);
                            break;

                        case Media media:
                            if (!string.IsNullOrEmpty(media.GetStringFieldValue(Models.Metadata.Media.SHA256Key)))
                                fieldDats[Models.Metadata.Media.SHA256Key].Add(key, item);
                            else if (!string.IsNullOrEmpty(media.GetStringFieldValue(Models.Metadata.Media.SHA1Key)))
                                fieldDats[Models.Metadata.Media.SHA1Key].Add(key, item);
                            else if (!string.IsNullOrEmpty(media.GetStringFieldValue(Models.Metadata.Media.MD5Key)))
                                fieldDats[Models.Metadata.Media.MD5Key].Add(key, item);
                            else
                                fieldDats["null"].Add(key, item);
                            break;

                        case Rom rom:
                            if (rom.GetStringFieldValue(Models.Metadata.Rom.StatusKey).AsEnumValue<ItemStatus>() == ItemStatus.Nodump)
                                fieldDats[Models.Metadata.Rom.StatusKey].Add(key, item);
                            else if (!string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.SHA512Key)))
                                fieldDats[Models.Metadata.Rom.SHA512Key].Add(key, item);
                            else if (!string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.SHA384Key)))
                                fieldDats[Models.Metadata.Rom.SHA384Key].Add(key, item);
                            else if (!string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.SHA256Key)))
                                fieldDats[Models.Metadata.Rom.SHA256Key].Add(key, item);
                            else if (!string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.SHA1Key)))
                                fieldDats[Models.Metadata.Rom.SHA1Key].Add(key, item);
                            else if (!string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.MD5Key)))
                                fieldDats[Models.Metadata.Rom.MD5Key].Add(key, item);
                            else if (!string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.MD4Key)))
                                fieldDats[Models.Metadata.Rom.MD4Key].Add(key, item);
                            else if (!string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.MD2Key)))
                                fieldDats[Models.Metadata.Rom.MD2Key].Add(key, item);
                            else if (!string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.CRCKey)))
                                fieldDats[Models.Metadata.Rom.CRCKey].Add(key, item);
                            else
                                fieldDats["null"].Add(key, item);
                            break;

                        default:
                            continue;
                    }
                }
#if NET40_OR_GREATER || NETCOREAPP
            });
#else
            }
#endif

            watch.Stop();
            return fieldDats;
        }

        /// <summary>
        /// Split a DAT by best available hashes
        /// </summary>
        /// <param name="datFile">Current DatFile object to split</param>
        /// <returns>Dictionary of Field to DatFile mappings</returns>
        public static Dictionary<string, DatFile> SplitByHashDB(DatFile datFile)
        {
            // Create each of the respective output DATs
            var watch = new InternalStopwatch($"Splitting DAT by best available hashes");

            // Create mapping of keys to suffixes
            var mappings = new Dictionary<string, string>
            {
                [Models.Metadata.Rom.StatusKey] = " (Nodump)",
                [Models.Metadata.Rom.SHA512Key] = " (SHA-512)",
                [Models.Metadata.Rom.SHA384Key] = " (SHA-384)",
                [Models.Metadata.Rom.SHA256Key] = " (SHA-256)",
                [Models.Metadata.Rom.SHA1Key] = " (SHA-1)",
                [Models.Metadata.Rom.MD5Key] = " (MD5)",
                [Models.Metadata.Rom.MD4Key] = " (MD4)",
                [Models.Metadata.Rom.MD2Key] = " (MD2)",
                [Models.Metadata.Rom.CRCKey] = " (CRC)",
                ["null"] = " (Other)",
            };

            // Create the set of field-to-dat mappings
            Dictionary<string, DatFile> fieldDats = [];
            foreach (var kvp in mappings)
            {
                fieldDats[kvp.Key] = DatFileTool.CreateDatFile(datFile.Header.CloneStandard());
                fieldDats[kvp.Key].Header.SetFieldValue<string?>(DatHeader.FileNameKey, fieldDats[kvp.Key].Header.GetStringFieldValue(DatHeader.FileNameKey) + kvp.Value);
                fieldDats[kvp.Key].Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, fieldDats[kvp.Key].Header.GetStringFieldValue(Models.Metadata.Header.NameKey) + kvp.Value);
                fieldDats[kvp.Key].Header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, fieldDats[kvp.Key].Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey) + kvp.Value);
            }

            // Get all current items, machines, and mappings
            var datItems = datFile.ItemsDB.GetItems();
            var machines = datFile.ItemsDB.GetMachines();
            var sources = datFile.ItemsDB.GetSources();
            var itemMachineMappings = datFile.ItemsDB.GetItemMachineMappings();
            var itemSourceMappings = datFile.ItemsDB.GetItemSourceMappings();

            // Create mappings from old index to new index
            var machineRemapping = new Dictionary<long, long>();
            var sourceRemapping = new Dictionary<long, long>();

            // Loop through and add all sources
            foreach (var source in sources)
            {
                long newSourceIndex = fieldDats[Models.Metadata.Rom.StatusKey].AddSourceDB(source.Value);
                sourceRemapping[source.Key] = newSourceIndex;
                _ = fieldDats[Models.Metadata.Rom.SHA512Key].AddSourceDB(source.Value);
                _ = fieldDats[Models.Metadata.Rom.SHA384Key].AddSourceDB(source.Value);
                _ = fieldDats[Models.Metadata.Rom.SHA256Key].AddSourceDB(source.Value);
                _ = fieldDats[Models.Metadata.Rom.SHA1Key].AddSourceDB(source.Value);
                _ = fieldDats[Models.Metadata.Rom.MD5Key].AddSourceDB(source.Value);
                _ = fieldDats[Models.Metadata.Rom.MD4Key].AddSourceDB(source.Value);
                _ = fieldDats[Models.Metadata.Rom.MD2Key].AddSourceDB(source.Value);
                _ = fieldDats[Models.Metadata.Rom.CRCKey].AddSourceDB(source.Value);
                _ = fieldDats["null"].AddSourceDB(source.Value);
            }

            // Loop through and add all machines
            foreach (var machine in machines)
            {
                long newMachineIndex = fieldDats[Models.Metadata.Rom.StatusKey].AddMachineDB(machine.Value);
                _ = fieldDats[Models.Metadata.Rom.SHA512Key].AddMachineDB(machine.Value);
                _ = fieldDats[Models.Metadata.Rom.SHA384Key].AddMachineDB(machine.Value);
                _ = fieldDats[Models.Metadata.Rom.SHA256Key].AddMachineDB(machine.Value);
                _ = fieldDats[Models.Metadata.Rom.SHA1Key].AddMachineDB(machine.Value);
                _ = fieldDats[Models.Metadata.Rom.MD5Key].AddMachineDB(machine.Value);
                _ = fieldDats[Models.Metadata.Rom.MD4Key].AddMachineDB(machine.Value);
                _ = fieldDats[Models.Metadata.Rom.MD2Key].AddMachineDB(machine.Value);
                _ = fieldDats[Models.Metadata.Rom.CRCKey].AddMachineDB(machine.Value);
                _ = fieldDats["null"].AddMachineDB(machine.Value);
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
                long machineIndex = itemMachineMappings[item.Key];
                long sourceIndex = itemSourceMappings[item.Key];

                // Only process Disk, Media, and Rom
                switch (item.Value)
                {
                    case Disk disk:
                        if (disk.GetStringFieldValue(Models.Metadata.Disk.StatusKey).AsEnumValue<ItemStatus>() == ItemStatus.Nodump)
                            fieldDats[Models.Metadata.Disk.StatusKey].AddItemDB(item.Value, machineRemapping[machineIndex], sourceRemapping[sourceIndex], statsOnly: false);
                        else if (!string.IsNullOrEmpty(disk.GetStringFieldValue(Models.Metadata.Disk.SHA1Key)))
                            fieldDats[Models.Metadata.Disk.SHA1Key].AddItemDB(item.Value, machineRemapping[machineIndex], sourceRemapping[sourceIndex], statsOnly: false);
                        else if (!string.IsNullOrEmpty(disk.GetStringFieldValue(Models.Metadata.Disk.MD5Key)))
                            fieldDats[Models.Metadata.Disk.MD5Key].AddItemDB(item.Value, machineRemapping[machineIndex], sourceRemapping[sourceIndex], statsOnly: false);
                        else if (!string.IsNullOrEmpty(disk.GetStringFieldValue(Models.Metadata.Disk.MD5Key)))
                            fieldDats[Models.Metadata.Disk.MD5Key].AddItemDB(item.Value, machineRemapping[machineIndex], sourceRemapping[sourceIndex], statsOnly: false);
                        else
                            fieldDats["null"].AddItemDB(item.Value, machineRemapping[machineIndex], sourceRemapping[sourceIndex], statsOnly: false);
                        break;

                    case Media media:
                        if (!string.IsNullOrEmpty(media.GetStringFieldValue(Models.Metadata.Media.SHA256Key)))
                            fieldDats[Models.Metadata.Media.SHA256Key].AddItemDB(item.Value, machineRemapping[machineIndex], sourceRemapping[sourceIndex], statsOnly: false);
                        else if (!string.IsNullOrEmpty(media.GetStringFieldValue(Models.Metadata.Media.SHA1Key)))
                            fieldDats[Models.Metadata.Media.SHA1Key].AddItemDB(item.Value, machineRemapping[machineIndex], sourceRemapping[sourceIndex], statsOnly: false);
                        else if (!string.IsNullOrEmpty(media.GetStringFieldValue(Models.Metadata.Media.MD5Key)))
                            fieldDats[Models.Metadata.Media.MD5Key].AddItemDB(item.Value, machineRemapping[machineIndex], sourceRemapping[sourceIndex], statsOnly: false);
                        else
                            fieldDats["null"].AddItemDB(item.Value, machineRemapping[machineIndex], sourceRemapping[sourceIndex], statsOnly: false);
                        break;

                    case Rom rom:
                        if (rom.GetStringFieldValue(Models.Metadata.Rom.StatusKey).AsEnumValue<ItemStatus>() == ItemStatus.Nodump)
                            fieldDats[Models.Metadata.Rom.StatusKey].AddItemDB(item.Value, machineRemapping[machineIndex], sourceRemapping[sourceIndex], statsOnly: false);
                        else if (!string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.SHA512Key)))
                            fieldDats[Models.Metadata.Rom.SHA512Key].AddItemDB(item.Value, machineRemapping[machineIndex], sourceRemapping[sourceIndex], statsOnly: false);
                        else if (!string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.SHA384Key)))
                            fieldDats[Models.Metadata.Rom.SHA384Key].AddItemDB(item.Value, machineRemapping[machineIndex], sourceRemapping[sourceIndex], statsOnly: false);
                        else if (!string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.SHA256Key)))
                            fieldDats[Models.Metadata.Rom.SHA256Key].AddItemDB(item.Value, machineRemapping[machineIndex], sourceRemapping[sourceIndex], statsOnly: false);
                        else if (!string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.SHA1Key)))
                            fieldDats[Models.Metadata.Rom.SHA1Key].AddItemDB(item.Value, machineRemapping[machineIndex], sourceRemapping[sourceIndex], statsOnly: false);
                        else if (!string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.MD5Key)))
                            fieldDats[Models.Metadata.Rom.MD5Key].AddItemDB(item.Value, machineRemapping[machineIndex], sourceRemapping[sourceIndex], statsOnly: false);
                        else if (!string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.MD4Key)))
                            fieldDats[Models.Metadata.Rom.MD4Key].AddItemDB(item.Value, machineRemapping[machineIndex], sourceRemapping[sourceIndex], statsOnly: false);
                        else if (!string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.MD2Key)))
                            fieldDats[Models.Metadata.Rom.MD2Key].AddItemDB(item.Value, machineRemapping[machineIndex], sourceRemapping[sourceIndex], statsOnly: false);
                        else if (!string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.CRCKey)))
                            fieldDats[Models.Metadata.Rom.CRCKey].AddItemDB(item.Value, machineRemapping[machineIndex], sourceRemapping[sourceIndex], statsOnly: false);
                        else
                            fieldDats["null"].AddItemDB(item.Value, machineRemapping[machineIndex], sourceRemapping[sourceIndex], statsOnly: false);
                        break;

                    default:
#if NET40_OR_GREATER || NETCOREAPP
                        return;
#else
                        continue;
#endif
                }
#if NET40_OR_GREATER || NETCOREAPP
            });
#else
            }
#endif

            watch.Stop();
            return fieldDats;
        }

        /// <summary>
        /// Split a SuperDAT by lowest available directory level
        /// </summary>
        /// <param name="datFile">Current DatFile object to split</param>
        /// <param name="outDir">Name of the directory to write the DATs out to</param>
        /// <param name="shortname">True if short names should be used, false otherwise</param>
        /// <param name="basedat">True if original filenames should be used as the base for output filename, false otherwise</param>
        /// <returns>True if split succeeded, false otherwise</returns>
        public static bool SplitByLevel(DatFile datFile, string outDir, bool shortname, bool basedat)
        {
            InternalStopwatch watch = new($"Splitting DAT by level");

            // First, bucket by games so that we can do the right thing
            datFile.BucketBy(ItemKey.Machine, DedupeType.None, lower: false, norename: true);

            // Create a temporary DAT to add things to
            DatFile tempDat = DatFileTool.CreateDatFile(datFile.Header);
            tempDat.Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, null);

            // Sort the input keys
            List<string> keys = [.. datFile.Items.Keys];
            keys.Sort(SplitByLevelSort);

            // Then, we loop over the games
#if NET452_OR_GREATER || NETCOREAPP
            Parallel.ForEach(keys, Core.Globals.ParallelOptions, key =>
#elif NET40_OR_GREATER
            Parallel.ForEach(keys, key =>
#else
            foreach (var key in keys)
#endif
            {
                // Here, the key is the name of the game to be used for comparison
                if (tempDat.Header.GetStringFieldValue(Models.Metadata.Header.NameKey) != null && tempDat.Header.GetStringFieldValue(Models.Metadata.Header.NameKey) != Path.GetDirectoryName(key))
                {
                    // Reset the DAT for the next items
                    tempDat = DatFileTool.CreateDatFile(datFile.Header);
                    tempDat.Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, null);
                }

                // Clean the input list and set all games to be pathless
                List<DatItem>? items = datFile.GetItemsForBucket(key);
                if (items == null)
#if NET40_OR_GREATER || NETCOREAPP
                    return;
#else
                    continue;
#endif
                items.ForEach(item => item.GetFieldValue<Machine>(DatItem.MachineKey)!.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, Path.GetFileName(item.GetFieldValue<Machine>(DatItem.MachineKey)!.GetStringFieldValue(Models.Metadata.Machine.NameKey))));
                items.ForEach(item => item.GetFieldValue<Machine>(DatItem.MachineKey)!.SetFieldValue<string?>(Models.Metadata.Machine.DescriptionKey, Path.GetFileName(item.GetFieldValue<Machine>(DatItem.MachineKey)!.GetStringFieldValue(Models.Metadata.Machine.DescriptionKey))));

                // Now add the game to the output DAT
                tempDat.Add(key, items);

                // Then set the DAT name to be the parent directory name
                tempDat.Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, Path.GetDirectoryName(key));
#if NET40_OR_GREATER || NETCOREAPP
            });
#else
            }
#endif

            watch.Stop();
            return true;
        }

        /// <summary>
        /// Helper function for SplitByLevel to sort the input game names
        /// </summary>
        /// <param name="a">First string to compare</param>
        /// <param name="b">Second string to compare</param>
        /// <returns>-1 for a coming before b, 0 for a == b, 1 for a coming after b</returns>
        private static int SplitByLevelSort(string a, string b)
        {
            NaturalComparer nc = new();
            int adeep = a.Count(c => c == '/' || c == '\\');
            int bdeep = b.Count(c => c == '/' || c == '\\');

            if (adeep == bdeep)
                return nc.Compare(a, b);

            return adeep - bdeep;
        }

        /// <summary>
        /// Helper function for SplitByLevel to clean and write out a DAT
        /// </summary>
        /// <param name="datFile">Current DatFile object to split</param>
        /// <param name="newDatFile">DAT to clean and write out</param>
        /// <param name="outDir">Directory to write out to</param>
        /// <param name="shortname">True if short naming scheme should be used, false otherwise</param>
        /// <param name="restore">True if original filenames should be used as the base for output filename, false otherwise</param>
        private static void SplitByLevelHelper(DatFile datFile, DatFile newDatFile, string outDir, bool shortname, bool restore)
        {
            // Get the name from the DAT to use separately
            string? name = newDatFile.Header.GetStringFieldValue(Models.Metadata.Header.NameKey);
            string? expName = name?.Replace("/", " - ")?.Replace("\\", " - ");

            // Now set the new output values
#if NET20 || NET35
            newDatFile.Header.SetFieldValue<string?>(DatHeader.FileNameKey, string.IsNullOrEmpty(name)
                ? datFile.Header.GetStringFieldValue(DatHeader.FileNameKey)
                : (shortname
                    ? Path.GetFileName(name)
                    : expName
                ));
#else
            newDatFile.Header.SetFieldValue<string?>(DatHeader.FileNameKey, WebUtility.HtmlDecode(string.IsNullOrEmpty(name)
                ? datFile.Header.GetStringFieldValue(DatHeader.FileNameKey)
                : (shortname
                    ? Path.GetFileName(name)
                    : expName
                    )
                ));
#endif
            newDatFile.Header.SetFieldValue<string?>(DatHeader.FileNameKey, restore
                ? $"{datFile.Header.GetStringFieldValue(DatHeader.FileNameKey)} ({newDatFile.Header.GetStringFieldValue(DatHeader.FileNameKey)})"
                : newDatFile.Header.GetStringFieldValue(DatHeader.FileNameKey));
            newDatFile.Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, $"{datFile.Header.GetStringFieldValue(Models.Metadata.Header.NameKey)} ({expName})");
            newDatFile.Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, string.IsNullOrEmpty(datFile.Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey))
                ? newDatFile.Header.GetStringFieldValue(Models.Metadata.Header.NameKey)
                : $"{datFile.Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey)} ({expName})");
            newDatFile.Header.SetFieldValue<string?>(Models.Metadata.Header.TypeKey, null);

            // Write out the temporary DAT to the proper directory
            Writer.Write(newDatFile, outDir);
        }

        /// <summary>
        /// Split a DAT by size of Rom
        /// </summary>
        /// <param name="datFile">Current DatFile object to split</param>
        /// <param name="radix">Long value representing the split point</param>
        /// <returns>Less Than and Greater Than DatFiles</returns>
        public static (DatFile lessThan, DatFile greaterThan) SplitBySize(DatFile datFile, long radix)
        {
            // Create each of the respective output DATs
            InternalStopwatch watch = new($"Splitting DAT by size");

            DatFile lessThan = DatFileTool.CreateDatFile(datFile.Header.CloneStandard());
            lessThan.Header.SetFieldValue<string?>(DatHeader.FileNameKey, lessThan.Header.GetStringFieldValue(DatHeader.FileNameKey) + $" (less than {radix})");
            lessThan.Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, lessThan.Header.GetStringFieldValue(Models.Metadata.Header.NameKey) + $" (less than {radix})");
            lessThan.Header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, lessThan.Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey) + $" (less than {radix})");

            DatFile greaterThan = DatFileTool.CreateDatFile(datFile.Header.CloneStandard());
            greaterThan.Header.SetFieldValue<string?>(DatHeader.FileNameKey, greaterThan.Header.GetStringFieldValue(DatHeader.FileNameKey) + $" (equal-greater than {radix})");
            greaterThan.Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, greaterThan.Header.GetStringFieldValue(Models.Metadata.Header.NameKey) + $" (equal-greater than {radix})");
            greaterThan.Header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, greaterThan.Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey) + $" (equal-greater than {radix})");

            // Now populate each of the DAT objects in turn
#if NET452_OR_GREATER || NETCOREAPP
            Parallel.ForEach(datFile.Items.Keys, Core.Globals.ParallelOptions, key =>
#elif NET40_OR_GREATER
            Parallel.ForEach(datFile.Items.Keys, key =>
#else
            foreach (var key in datFile.Items.Keys)
#endif
            {
                List<DatItem>? items = datFile.GetItemsForBucket(key);
                if (items == null)
#if NET40_OR_GREATER || NETCOREAPP
                    return;
#else
                    continue;
#endif
                foreach (DatItem item in items)
                {
                    // If the file is not a Rom, it automatically goes in the "lesser" dat
                    if (item is not Rom rom)
                        lessThan.Add(key, item);

                    // If the file is a Rom and has no size, put it in the "lesser" dat
                    else if (rom.GetInt64FieldValue(Models.Metadata.Rom.SizeKey) == null)
                        lessThan.Add(key, item);

                    // If the file is a Rom and less than the radix, put it in the "lesser" dat
                    else if (rom.GetInt64FieldValue(Models.Metadata.Rom.SizeKey) < radix)
                        lessThan.Add(key, item);

                    // If the file is a Rom and greater than or equal to the radix, put it in the "greater" dat
                    else if (rom.GetInt64FieldValue(Models.Metadata.Rom.SizeKey) >= radix)
                        greaterThan.Add(key, item);
                }
#if NET40_OR_GREATER || NETCOREAPP
            });
#else
            }
#endif

            // Then return both DatFiles
            watch.Stop();
            return (lessThan, greaterThan);
        }

        /// <summary>
        /// Split a DAT by size of Rom
        /// </summary>
        /// <param name="datFile">Current DatFile object to split</param>
        /// <param name="radix">Long value representing the split point</param>
        /// <returns>Less Than and Greater Than DatFiles</returns>
        public static (DatFile lessThan, DatFile greaterThan) SplitBySizeDB(DatFile datFile, long radix)
        {
            // Create each of the respective output DATs
            var watch = new InternalStopwatch($"Splitting DAT by size");

            DatFile lessThan = DatFileTool.CreateDatFile(datFile.Header.CloneStandard());
            lessThan.Header.SetFieldValue<string?>(DatHeader.FileNameKey, lessThan.Header.GetStringFieldValue(DatHeader.FileNameKey) + $" (less than {radix})");
            lessThan.Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, lessThan.Header.GetStringFieldValue(Models.Metadata.Header.NameKey) + $" (less than {radix})");
            lessThan.Header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, lessThan.Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey) + $" (less than {radix})");

            DatFile greaterThan = DatFileTool.CreateDatFile(datFile.Header.CloneStandard());
            greaterThan.Header.SetFieldValue<string?>(DatHeader.FileNameKey, greaterThan.Header.GetStringFieldValue(DatHeader.FileNameKey) + $" (equal-greater than {radix})");
            greaterThan.Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, greaterThan.Header.GetStringFieldValue(Models.Metadata.Header.NameKey) + $" (equal-greater than {radix})");
            greaterThan.Header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, greaterThan.Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey) + $" (equal-greater than {radix})");

            // Get all current items, machines, and mappings
            var datItems = datFile.ItemsDB.GetItems();
            var machines = datFile.ItemsDB.GetMachines();
            var sources = datFile.ItemsDB.GetSources();
            var itemMachineMappings = datFile.ItemsDB.GetItemMachineMappings();
            var itemSourceMappings = datFile.ItemsDB.GetItemSourceMappings();

            // Create mappings from old index to new index
            var machineRemapping = new Dictionary<long, long>();
            var sourceRemapping = new Dictionary<long, long>();

            // Loop through and add all sources
            foreach (var source in sources)
            {
                long newSourceIndex = lessThan.AddSourceDB(source.Value);
                _ = greaterThan.AddSourceDB(source.Value);
                sourceRemapping[source.Key] = newSourceIndex;
            }

            // Loop through and add all machines
            foreach (var machine in machines)
            {
                long newMachineIndex = lessThan.AddMachineDB(machine.Value);
                _ = greaterThan.AddMachineDB(machine.Value);
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
                long machineIndex = itemMachineMappings[item.Key];
                long sourceIndex = itemSourceMappings[item.Key];

                // If the file is not a Rom, it automatically goes in the "lesser" dat
                if (item.Value is not Rom rom)
                    lessThan.AddItemDB(item.Value, machineRemapping[machineIndex], sourceRemapping[sourceIndex], statsOnly: false);

                // If the file is a Rom and has no size, put it in the "lesser" dat
                else if (rom.GetInt64FieldValue(Models.Metadata.Rom.SizeKey) == null)
                    lessThan.AddItemDB(item.Value, machineRemapping[machineIndex], sourceRemapping[sourceIndex], statsOnly: false);

                // If the file is a Rom and less than the radix, put it in the "lesser" dat
                else if (rom.GetInt64FieldValue(Models.Metadata.Rom.SizeKey) < radix)
                    lessThan.AddItemDB(item.Value, machineRemapping[machineIndex], sourceRemapping[sourceIndex], statsOnly: false);

                // If the file is a Rom and greater than or equal to the radix, put it in the "greater" dat
                else if (rom.GetInt64FieldValue(Models.Metadata.Rom.SizeKey) >= radix)
                    greaterThan.AddItemDB(item.Value, machineRemapping[machineIndex], sourceRemapping[sourceIndex], statsOnly: false);
#if NET40_OR_GREATER || NETCOREAPP
            });
#else
            }
#endif

            // Then return both DatFiles
            watch.Stop();
            return (lessThan, greaterThan);
        }

        /// <summary>
        /// Split a DAT by size of Rom
        /// </summary>
        /// <param name="datFile">Current DatFile object to split</param>
        /// <param name="chunkSize">Long value representing the total size to split at</param>
        /// <returns>Less Than and Greater Than DatFiles</returns>
        public static List<DatFile> SplitByTotalSize(DatFile datFile, long chunkSize)
        {
            // If the size is invalid, just return
            if (chunkSize <= 0)
                return [];

            // Create each of the respective output DATs
            InternalStopwatch watch = new($"Splitting DAT by total size");

            // Sort the DatFile by machine name
            datFile.BucketBy(ItemKey.Machine, DedupeType.None);

            // Get the keys in a known order for easier sorting
            var keys = datFile.Items.SortedKeys;

            // Get the output list
            List<DatFile> datFiles = [];

            // Initialize everything
            long currentSize = 0;
            long currentIndex = 0;
            DatFile currentDat = DatFileTool.CreateDatFile(datFile.Header.CloneStandard());
            currentDat.Header.SetFieldValue<string?>(DatHeader.FileNameKey, currentDat.Header.GetStringFieldValue(DatHeader.FileNameKey) + $"_{currentIndex}");
            currentDat.Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, currentDat.Header.GetStringFieldValue(Models.Metadata.Header.NameKey) + $"_{currentIndex}");
            currentDat.Header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, currentDat.Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey) + $"_{currentIndex}");

            // Loop through each machine 
            foreach (string machine in keys)
            {
                // Get the current machine
                var items = datFile.GetItemsForBucket(machine);
                if (items == null || items.Count == 0)
                {
                    _staticLogger.Error($"{machine} contains no items and will be skipped");
                    continue;
                }

                // Get the total size of the current machine
                long machineSize = 0;
                foreach (var item in items)
                {
                    if (item is Rom rom)
                    {
                        // TODO: Should there be more than just a log if a single item is larger than the chunksize?
                        machineSize += rom.GetInt64FieldValue(Models.Metadata.Rom.SizeKey) ?? 0;
                        if ((rom.GetInt64FieldValue(Models.Metadata.Rom.SizeKey) ?? 0) > chunkSize)
                            _staticLogger.Error($"{rom.GetName() ?? string.Empty} in {machine} is larger than {chunkSize}");
                    }
                }

                // If the current machine size is greater than the chunk size by itself, we want to log and skip
                // TODO: Should this eventually try to split the machine here?
                if (machineSize > chunkSize)
                {
                    _staticLogger.Error($"{machine} is larger than {chunkSize} and will be skipped");
                    continue;
                }

                // If the current machine size makes the current DatFile too big, split
                else if (currentSize + machineSize > chunkSize)
                {
                    datFiles.Add(currentDat);
                    currentSize = 0;
                    currentIndex++;
                    currentDat = DatFileTool.CreateDatFile(datFile.Header.CloneStandard());
                    currentDat.Header.SetFieldValue<string?>(DatHeader.FileNameKey, currentDat.Header.GetStringFieldValue(DatHeader.FileNameKey) + $"_{currentIndex}");
                    currentDat.Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, currentDat.Header.GetStringFieldValue(Models.Metadata.Header.NameKey) + $"_{currentIndex}");
                    currentDat.Header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, currentDat.Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey) + $"_{currentIndex}");
                }

                // Add the current machine to the current DatFile
                currentDat.Add(machine, items);
                currentSize += machineSize;
            }

            // Add the final DatFile to the list
            datFiles.Add(currentDat);

            // Then return the list
            watch.Stop();
            return datFiles;
        }

        /// <summary>
        /// Split a DAT by type of DatItem
        /// </summary>
        /// <param name="datFile">Current DatFile object to split</param>
        /// <returns>Dictionary of ItemType to DatFile mappings</returns>
        public static Dictionary<ItemType, DatFile> SplitByType(DatFile datFile)
        {
            // Create each of the respective output DATs
            InternalStopwatch watch = new($"Splitting DAT by item type");

            // Create the set of type-to-dat mappings
            Dictionary<ItemType, DatFile> typeDats = [];

            // We only care about a subset of types
            List<ItemType> outputTypes =
            [
                ItemType.Disk,
                ItemType.Media,
                ItemType.Rom,
                ItemType.Sample,
            ];

            // Setup all of the DatFiles
            foreach (ItemType itemType in outputTypes)
            {
                typeDats[itemType] = DatFileTool.CreateDatFile(datFile.Header.CloneStandard());
                typeDats[itemType].Header.SetFieldValue<string?>(DatHeader.FileNameKey, typeDats[itemType].Header.GetStringFieldValue(DatHeader.FileNameKey) + $" ({itemType})");
                typeDats[itemType].Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, typeDats[itemType].Header.GetStringFieldValue(Models.Metadata.Header.NameKey) + $" ({itemType})");
                typeDats[itemType].Header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, typeDats[itemType].Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey) + $" ({itemType})");
            }

            // Now populate each of the DAT objects in turn
#if NET452_OR_GREATER || NETCOREAPP
            Parallel.ForEach(outputTypes, Core.Globals.ParallelOptions, itemType =>
#elif NET40_OR_GREATER
            Parallel.ForEach(outputTypes, itemType =>
#else
            foreach (var itemType in outputTypes)
#endif
            {
                FillWithItemType(datFile, typeDats[itemType], itemType);
                FillWithItemTypeDB(datFile, typeDats[itemType], itemType);
#if NET40_OR_GREATER || NETCOREAPP
            });
#else
            }
#endif

            watch.Stop();
            return typeDats;
        }

        /// <summary>
        /// Fill a DatFile with all items with a particular ItemType
        /// </summary>
        /// <param name="datFile">Current DatFile object to split</param>
        /// <param name="indexDat">DatFile to add found items to</param>
        /// <param name="itemType">ItemType to retrieve items for</param>
        /// <returns>DatFile containing all items with the ItemType/returns>
        private static void FillWithItemType(DatFile datFile, DatFile indexDat, ItemType itemType)
        {
            // Loop through and add the items for this index to the output
#if NET452_OR_GREATER || NETCOREAPP
            Parallel.ForEach(datFile.Items.Keys, Core.Globals.ParallelOptions, key =>
#elif NET40_OR_GREATER
            Parallel.ForEach(datFile.Items.Keys, key =>
#else
            foreach (var key in datFile.Items.Keys)
#endif
            {
                List<DatItem> items = DatFileTool.Merge(datFile.GetItemsForBucket(key));

                // If the rom list is empty or null, just skip it
                if (items == null || items.Count == 0)
#if NET40_OR_GREATER || NETCOREAPP
                    return;
#else
                    continue;
#endif

                foreach (DatItem item in items)
                {
                    if (item.GetStringFieldValue(Models.Metadata.DatItem.TypeKey).AsEnumValue<ItemType>() == itemType)
                        indexDat.Add(key, item);
                }
#if NET40_OR_GREATER || NETCOREAPP
            });
#else
            }
#endif
        }

        /// <summary>
        /// Fill a DatFile with all items with a particular ItemType
        /// </summary>
        /// <param name="datFile">Current DatFile object to split</param>
        /// <param name="indexDat">DatFile to add found items to</param>
        /// <param name="itemType">ItemType to retrieve items for</param>
        /// <returns>DatFile containing all items with the ItemType/returns>
        private static void FillWithItemTypeDB(DatFile datFile, DatFile indexDat, ItemType itemType)
        {
            // Get all current items, machines, and mappings
            var datItems = datFile.ItemsDB.GetItems();
            var machines = datFile.ItemsDB.GetMachines();
            var sources = datFile.ItemsDB.GetSources();
            var itemMachineMappings = datFile.ItemsDB.GetItemMachineMappings();
            var itemSourceMappings = datFile.ItemsDB.GetItemSourceMappings();

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
                long machineIndex = itemMachineMappings[item.Key];
                long sourceIndex = itemSourceMappings[item.Key];

                if (item.Value.GetStringFieldValue(Models.Metadata.DatItem.TypeKey).AsEnumValue<ItemType>() == itemType)
                    indexDat.AddItemDB(item.Value, machineRemapping[machineIndex], sourceRemapping[sourceIndex], statsOnly: false);
#if NET40_OR_GREATER || NETCOREAPP
            });
#else
            }
#endif
        }
    }
}