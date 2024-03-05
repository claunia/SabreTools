using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using SabreTools.Core;
using SabreTools.DatFiles;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;
using SabreTools.IO;
using SabreTools.Logging;
using SabreTools.Matching;

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
        private static readonly Logger logger = new();

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
            if (datFile.Items.TotalCount == 0)
                return (null, null);

            InternalStopwatch watch = new($"Splitting DAT by extension");

            // Make sure all of the extensions don't have a dot at the beginning
            var newExtA = extA.Select(s => s.TrimStart('.').ToLowerInvariant()).ToArray();
            string newExtAString = string.Join(",", newExtA);

            var newExtB = extB.Select(s => s.TrimStart('.').ToLowerInvariant()).ToArray();
            string newExtBString = string.Join(",", newExtB);

            // Set all of the appropriate outputs for each of the subsets
            DatFile extADat = DatFile.Create(datFile.Header.CloneStandard());
            extADat.Header.FileName += $" ({newExtAString})";
            extADat.Header.Name += $" ({newExtAString})";
            extADat.Header.Description += $" ({newExtAString})";

            DatFile extBDat = DatFile.Create(datFile.Header.CloneStandard());
            extBDat.Header.FileName += $" ({newExtBString})";
            extBDat.Header.Name += $" ({newExtBString})";
            extBDat.Header.Description += $" ({newExtBString})";

            // Now separate the roms accordingly
#if NET452_OR_GREATER || NETCOREAPP
            Parallel.ForEach(datFile.Items.Keys, Globals.ParallelOptions, key =>
#elif NET40_OR_GREATER
            Parallel.ForEach(datFile.Items.Keys, key =>
#else
            foreach (var key in datFile.Items.Keys)
#endif
            {
                var items = datFile.Items[key];
                if (items == null)
#if NET40_OR_GREATER || NETCOREAPP
                    return;
#else
                    continue;
#endif

                foreach (DatItem item in items)
                {
                    if (newExtA.Contains((item.GetName() ?? string.Empty).GetNormalizedExtension()))
                    {
                        extADat.Items.Add(key, item);
                    }
                    else if (newExtB.Contains((item.GetName() ?? string.Empty).GetNormalizedExtension()))
                    {
                        extBDat.Items.Add(key, item);
                    }
                    else
                    {
                        extADat.Items.Add(key, item);
                        extBDat.Items.Add(key, item);
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
        /// Split a DAT by best available hashes
        /// </summary>
        /// <param name="datFile">Current DatFile object to split</param>
        /// <returns>Dictionary of Field to DatFile mappings</returns>
        public static Dictionary<DatItemField, DatFile> SplitByHash(DatFile datFile)
        {
            // Create each of the respective output DATs
            InternalStopwatch watch = new($"Splitting DAT by best available hashes");

            // Create the set of field-to-dat mappings
            Dictionary<DatItemField, DatFile> fieldDats = [];

            // TODO: Can this be made into a loop?
            fieldDats[DatItemField.Status] = DatFile.Create(datFile.Header.CloneStandard());
            fieldDats[DatItemField.Status].Header.FileName += " (Nodump)";
            fieldDats[DatItemField.Status].Header.Name += " (Nodump)";
            fieldDats[DatItemField.Status].Header.Description += " (Nodump)";

            fieldDats[DatItemField.SHA512] = DatFile.Create(datFile.Header.CloneStandard());
            fieldDats[DatItemField.SHA512].Header.FileName += " (SHA-512)";
            fieldDats[DatItemField.SHA512].Header.Name += " (SHA-512)";
            fieldDats[DatItemField.SHA512].Header.Description += " (SHA-512)";

            fieldDats[DatItemField.SHA384] = DatFile.Create(datFile.Header.CloneStandard());
            fieldDats[DatItemField.SHA384].Header.FileName += " (SHA-384)";
            fieldDats[DatItemField.SHA384].Header.Name += " (SHA-384)";
            fieldDats[DatItemField.SHA384].Header.Description += " (SHA-384)";

            fieldDats[DatItemField.SHA256] = DatFile.Create(datFile.Header.CloneStandard());
            fieldDats[DatItemField.SHA256].Header.FileName += " (SHA-256)";
            fieldDats[DatItemField.SHA256].Header.Name += " (SHA-256)";
            fieldDats[DatItemField.SHA256].Header.Description += " (SHA-256)";

            fieldDats[DatItemField.SHA1] = DatFile.Create(datFile.Header.CloneStandard());
            fieldDats[DatItemField.SHA1].Header.FileName += " (SHA-1)";
            fieldDats[DatItemField.SHA1].Header.Name += " (SHA-1)";
            fieldDats[DatItemField.SHA1].Header.Description += " (SHA-1)";

            fieldDats[DatItemField.MD5] = DatFile.Create(datFile.Header.CloneStandard());
            fieldDats[DatItemField.MD5].Header.FileName += " (MD5)";
            fieldDats[DatItemField.MD5].Header.Name += " (MD5)";
            fieldDats[DatItemField.MD5].Header.Description += " (MD5)";

            fieldDats[DatItemField.CRC] = DatFile.Create(datFile.Header.CloneStandard());
            fieldDats[DatItemField.CRC].Header.FileName += " (CRC)";
            fieldDats[DatItemField.CRC].Header.Name += " (CRC)";
            fieldDats[DatItemField.CRC].Header.Description += " (CRC)";

            fieldDats[DatItemField.NULL] = DatFile.Create(datFile.Header.CloneStandard());
            fieldDats[DatItemField.NULL].Header.FileName += " (Other)";
            fieldDats[DatItemField.NULL].Header.Name += " (Other)";
            fieldDats[DatItemField.NULL].Header.Description += " (Other)";

            // Now populate each of the DAT objects in turn
#if NET452_OR_GREATER || NETCOREAPP
            Parallel.ForEach(datFile.Items.Keys, Globals.ParallelOptions, key =>
#elif NET40_OR_GREATER
            Parallel.ForEach(datFile.Items.Keys, key =>
#else
            foreach (var key in datFile.Items.Keys)
#endif
            {
                var items = datFile.Items[key];
                if (items == null)
#if NET40_OR_GREATER || NETCOREAPP
                    return;
#else
                    continue;
#endif
                foreach (DatItem item in items)
                {
                    // If the file is not a Disk, Media, or Rom, continue
                    if (item.ItemType != ItemType.Disk && item.ItemType != ItemType.Media && item.ItemType != ItemType.Rom)
                        continue;

                    // If the file is a nodump
                    if ((item.ItemType == ItemType.Rom && (item as Rom)!.ItemStatus == ItemStatus.Nodump)
                        || (item.ItemType == ItemType.Disk && (item as Disk)!.ItemStatus == ItemStatus.Nodump))
                    {
                        fieldDats[DatItemField.Status].Items.Add(key, item);
                    }

                    // If the file has a SHA-512
                    else if ((item.ItemType == ItemType.Rom && !string.IsNullOrEmpty((item as Rom)!.SHA512)))
                    {
                        fieldDats[DatItemField.SHA512].Items.Add(key, item);
                    }

                    // If the file has a SHA-384
                    else if ((item.ItemType == ItemType.Rom && !string.IsNullOrEmpty((item as Rom)!.SHA384)))
                    {
                        fieldDats[DatItemField.SHA384].Items.Add(key, item);
                    }

                    // If the file has a SHA-256
                    else if ((item.ItemType == ItemType.Media && !string.IsNullOrEmpty((item as Media)!.SHA256))
                        || (item.ItemType == ItemType.Rom && !string.IsNullOrEmpty((item as Rom)!.SHA256)))
                    {
                        fieldDats[DatItemField.SHA256].Items.Add(key, item);
                    }

                    // If the file has a SHA-1
                    else if ((item.ItemType == ItemType.Disk && !string.IsNullOrEmpty((item as Disk)!.SHA1))
                        || (item.ItemType == ItemType.Media && !string.IsNullOrEmpty((item as Media)!.SHA1))
                        || (item.ItemType == ItemType.Rom && !string.IsNullOrEmpty((item as Rom)!.SHA1)))
                    {
                        fieldDats[DatItemField.SHA1].Items.Add(key, item);
                    }

                    // If the file has an MD5
                    else if ((item.ItemType == ItemType.Disk && !string.IsNullOrEmpty((item as Disk)!.MD5))
                        || (item.ItemType == ItemType.Media && !string.IsNullOrEmpty((item as Media)!.MD5))
                        || (item.ItemType == ItemType.Rom && !string.IsNullOrEmpty((item as Rom)!.MD5)))
                    {
                        fieldDats[DatItemField.MD5].Items.Add(key, item);
                    }

                    // If the file has a CRC
                    else if ((item.ItemType == ItemType.Rom && !string.IsNullOrEmpty((item as Rom)!.CRC)))
                    {
                        fieldDats[DatItemField.CRC].Items.Add(key, item);
                    }

                    else
                    {
                        fieldDats[DatItemField.NULL].Items.Add(key, item);
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
            datFile.Items.BucketBy(ItemKey.Machine, DedupeType.None, lower: false, norename: true);

            // Create a temporary DAT to add things to
            DatFile tempDat = DatFile.Create(datFile.Header);
            tempDat.Header.Name = null;

            // Sort the input keys
            List<string> keys = [.. datFile.Items.Keys];
            keys.Sort(SplitByLevelSort);

            // Then, we loop over the games
#if NET452_OR_GREATER || NETCOREAPP
            Parallel.ForEach(keys, Globals.ParallelOptions, key =>
#elif NET40_OR_GREATER
            Parallel.ForEach(keys, key =>
#else
            foreach (var key in keys)
#endif
            {
                // Here, the key is the name of the game to be used for comparison
                if (tempDat.Header.Name != null && tempDat.Header.Name != Path.GetDirectoryName(key))
                {
                    // Reset the DAT for the next items
                    tempDat = DatFile.Create(datFile.Header);
                    tempDat.Header.Name = null;
                }

                // Clean the input list and set all games to be pathless
                ConcurrentList<DatItem>? items = datFile.Items[key];
                if (items == null)
#if NET40_OR_GREATER || NETCOREAPP
                    return;
#else
                    continue;
#endif
                items.ForEach(item => item.Machine.Name = Path.GetFileName(item.Machine.Name));
                items.ForEach(item => item.Machine.Description = Path.GetFileName(item.Machine.Description));

                // Now add the game to the output DAT
                tempDat.Items.AddRange(key, items);

                // Then set the DAT name to be the parent directory name
                tempDat.Header.Name = Path.GetDirectoryName(key);
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
            string? name = newDatFile.Header.Name;
            string? expName = name?.Replace("/", " - ")?.Replace("\\", " - ");

            // Now set the new output values
#if NET20 || NET35
            newDatFile.Header.FileName = string.IsNullOrEmpty(name)
                ? datFile.Header.FileName
                : (shortname
                    ? Path.GetFileName(name)
                    : expName
                    );
#else
            newDatFile.Header.FileName = WebUtility.HtmlDecode(string.IsNullOrEmpty(name)
                ? datFile.Header.FileName
                : (shortname
                    ? Path.GetFileName(name)
                    : expName
                    )
                );
#endif
            newDatFile.Header.FileName = restore ? $"{datFile.Header.FileName} ({newDatFile.Header.FileName})" : newDatFile.Header.FileName;
            newDatFile.Header.Name = $"{datFile.Header.Name} ({expName})";
            newDatFile.Header.Description = string.IsNullOrEmpty(datFile.Header.Description) ? newDatFile.Header.Name : $"{datFile.Header.Description} ({expName})";
            newDatFile.Header.Type = null;

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

            DatFile lessThan = DatFile.Create(datFile.Header.CloneStandard());
            lessThan.Header.FileName += $" (less than {radix})";
            lessThan.Header.Name += $" (less than {radix})";
            lessThan.Header.Description += $" (less than {radix})";

            DatFile greaterThan = DatFile.Create(datFile.Header.CloneStandard());
            greaterThan.Header.FileName += $" (equal-greater than {radix})";
            greaterThan.Header.Name += $" (equal-greater than {radix})";
            greaterThan.Header.Description += $" (equal-greater than {radix})";

            // Now populate each of the DAT objects in turn
#if NET452_OR_GREATER || NETCOREAPP
            Parallel.ForEach(datFile.Items.Keys, Globals.ParallelOptions, key =>
#elif NET40_OR_GREATER
            Parallel.ForEach(datFile.Items.Keys, key =>
#else
            foreach (var key in datFile.Items.Keys)
#endif
            {
                ConcurrentList<DatItem>? items = datFile.Items[key];
                if (items == null)
#if NET40_OR_GREATER || NETCOREAPP
                    return;
#else
                    continue;
#endif
                foreach (DatItem item in items)
                {
                    // If the file is not a Rom, it automatically goes in the "lesser" dat
                    if (item.ItemType != ItemType.Rom)
                        lessThan.Items.Add(key, item);

                    // If the file is a Rom and has no size, put it in the "lesser" dat
                    else if (item.ItemType == ItemType.Rom && (item as Rom)!.Size == null)
                        lessThan.Items.Add(key, item);

                    // If the file is a Rom and less than the radix, put it in the "lesser" dat
                    else if (item.ItemType == ItemType.Rom && (item as Rom)!.Size < radix)
                        lessThan.Items.Add(key, item);

                    // If the file is a Rom and greater than or equal to the radix, put it in the "greater" dat
                    else if (item.ItemType == ItemType.Rom && (item as Rom)!.Size >= radix)
                        greaterThan.Items.Add(key, item);
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
            datFile.Items.BucketBy(ItemKey.Machine, DedupeType.None);

            // Get the keys in a known order for easier sorting
            var keys = datFile.Items.SortedKeys;

            // Get the output list
            List<DatFile> datFiles = [];

            // Initialize everything
            long currentSize = 0;
            long currentIndex = 0;
            DatFile currentDat = DatFile.Create(datFile.Header.CloneStandard());
            currentDat.Header.FileName += $"_{currentIndex}";
            currentDat.Header.Name += $"_{currentIndex}";
            currentDat.Header.Description += $"_{currentIndex}";

            // Loop through each machine 
            foreach (string machine in keys)
            {
                // Get the current machine
                var items = datFile.Items[machine];
                if (items == null || !items.Any())
                {
                    logger.Error($"{machine} contains no items and will be skipped");
                    continue;
                }

                // Get the total size of the current machine
                long machineSize = 0;
                foreach (var item in items)
                {
                    if (item is Rom rom)
                    {
                        // TODO: Should there be more than just a log if a single item is larger than the chunksize?
                        machineSize += rom.Size ?? 0;
                        if ((rom.Size ?? 0) > chunkSize)
                            logger.Error($"{rom.GetName() ?? string.Empty} in {machine} is larger than {chunkSize}");
                    }
                }

                // If the current machine size is greater than the chunk size by itself, we want to log and skip
                // TODO: Should this eventually try to split the machine here?
                if (machineSize > chunkSize)
                {
                    logger.Error($"{machine} is larger than {chunkSize} and will be skipped");
                    continue;
                }

                // If the current machine size makes the current DatFile too big, split
                else if (currentSize + machineSize > chunkSize)
                {
                    datFiles.Add(currentDat);
                    currentSize = 0;
                    currentIndex++;
                    currentDat = DatFile.Create(datFile.Header.CloneStandard());
                    currentDat.Header.FileName += $"_{currentIndex}";
                    currentDat.Header.Name += $"_{currentIndex}";
                    currentDat.Header.Description += $"_{currentIndex}";
                }

                // Add the current machine to the current DatFile
                currentDat.Items[machine] = items;
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
                typeDats[itemType] = DatFile.Create(datFile.Header.CloneStandard());
                typeDats[itemType].Header.FileName += $" ({itemType})";
                typeDats[itemType].Header.Name += $" ({itemType})";
                typeDats[itemType].Header.Description += $" ({itemType})";
            }

            // Now populate each of the DAT objects in turn
#if NET452_OR_GREATER || NETCOREAPP
            Parallel.ForEach(outputTypes, Globals.ParallelOptions, itemType =>
#elif NET40_OR_GREATER
            Parallel.ForEach(outputTypes, itemType =>
#else
            foreach (var itemType in outputTypes)
#endif
            {
                FillWithItemType(datFile, typeDats[itemType], itemType);
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
            Parallel.ForEach(datFile.Items.Keys, Globals.ParallelOptions, key =>
#elif NET40_OR_GREATER
            Parallel.ForEach(datFile.Items.Keys, key =>
#else
            foreach (var key in datFile.Items.Keys)
#endif
            {
                ConcurrentList<DatItem> items = DatItem.Merge(datFile.Items[key]);

                // If the rom list is empty or null, just skip it
                if (items == null || items.Count == 0)
#if NET40_OR_GREATER || NETCOREAPP
                    return;
#else
                    continue;
#endif

                foreach (DatItem item in items)
                {
                    if (item.ItemType == itemType)
                        indexDat.Items.Add(key, item);
                }
#if NET40_OR_GREATER || NETCOREAPP
            });
#else
            }
#endif
        }
    }
}