using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
#if NET40_OR_GREATER || NETCOREAPP
using System.Threading.Tasks;
#endif
using SabreTools.Core;
using SabreTools.Core.Tools;
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
            extADat.Header.SetFieldValue<string?>(DatHeader.FileNameKey, extADat.Header.GetFieldValue<string?>(DatHeader.FileNameKey) + $" ({newExtAString})");
            extADat.Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, extADat.Header.GetFieldValue<string?>(Models.Metadata.Header.NameKey) + $" ({newExtAString})");
            extADat.Header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, extADat.Header.GetFieldValue<string?>(Models.Metadata.Header.DescriptionKey) + $" ({newExtAString})");

            DatFile extBDat = DatFile.Create(datFile.Header.CloneStandard());
            extBDat.Header.SetFieldValue<string?>(DatHeader.FileNameKey, extBDat.Header.GetFieldValue<string?>(DatHeader.FileNameKey) + $" ({newExtBString})");
            extBDat.Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, extBDat.Header.GetFieldValue<string?>(Models.Metadata.Header.NameKey) + $" ({newExtBString})");
            extBDat.Header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, extBDat.Header.GetFieldValue<string?>(Models.Metadata.Header.DescriptionKey) + $" ({newExtBString})");

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
        public static Dictionary<string, DatFile> SplitByHash(DatFile datFile)
        {
            // Create each of the respective output DATs
            InternalStopwatch watch = new($"Splitting DAT by best available hashes");

            // Create the set of field-to-dat mappings
            Dictionary<string, DatFile> fieldDats = [];

            // TODO: Can this be made into a loop?
            fieldDats[Models.Metadata.Rom.StatusKey] = DatFile.Create(datFile.Header.CloneStandard());
            fieldDats[Models.Metadata.Rom.StatusKey].Header.SetFieldValue<string?>(DatHeader.FileNameKey, fieldDats[Models.Metadata.Rom.StatusKey].Header.GetFieldValue<string?>(DatHeader.FileNameKey) + " (Nodump)");
            fieldDats[Models.Metadata.Rom.StatusKey].Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, fieldDats[Models.Metadata.Rom.StatusKey].Header.GetFieldValue<string?>(Models.Metadata.Header.NameKey) + " (Nodump)");
            fieldDats[Models.Metadata.Rom.StatusKey].Header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, fieldDats[Models.Metadata.Rom.StatusKey].Header.GetFieldValue<string?>(Models.Metadata.Header.DescriptionKey) + " (Nodump)");

            fieldDats[Models.Metadata.Rom.SHA512Key] = DatFile.Create(datFile.Header.CloneStandard());
            fieldDats[Models.Metadata.Rom.SHA512Key].Header.SetFieldValue<string?>(DatHeader.FileNameKey, fieldDats[Models.Metadata.Rom.SHA512Key].Header.GetFieldValue<string?>(DatHeader.FileNameKey) + " (SHA-512)");
            fieldDats[Models.Metadata.Rom.SHA512Key].Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, fieldDats[Models.Metadata.Rom.SHA512Key].Header.GetFieldValue<string?>(Models.Metadata.Header.NameKey) + " (SHA-512)");
            fieldDats[Models.Metadata.Rom.SHA512Key].Header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, fieldDats[Models.Metadata.Rom.SHA512Key].Header.GetFieldValue<string?>(Models.Metadata.Header.DescriptionKey) + " (SHA-512)");

            fieldDats[Models.Metadata.Rom.SHA384Key] = DatFile.Create(datFile.Header.CloneStandard());
            fieldDats[Models.Metadata.Rom.SHA384Key].Header.SetFieldValue<string?>(DatHeader.FileNameKey, fieldDats[Models.Metadata.Rom.SHA384Key].Header.GetFieldValue<string?>(DatHeader.FileNameKey) + " (SHA-384)");
            fieldDats[Models.Metadata.Rom.SHA384Key].Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, fieldDats[Models.Metadata.Rom.SHA384Key].Header.GetFieldValue<string?>(Models.Metadata.Header.NameKey) + " (SHA-384)");
            fieldDats[Models.Metadata.Rom.SHA384Key].Header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, fieldDats[Models.Metadata.Rom.SHA384Key].Header.GetFieldValue<string?>(Models.Metadata.Header.DescriptionKey) + " (SHA-384)");

            fieldDats[Models.Metadata.Rom.SHA256Key] = DatFile.Create(datFile.Header.CloneStandard());
            fieldDats[Models.Metadata.Rom.SHA256Key].Header.SetFieldValue<string?>(DatHeader.FileNameKey, fieldDats[Models.Metadata.Rom.SHA256Key].Header.GetFieldValue<string?>(DatHeader.FileNameKey) + " (SHA-256)");
            fieldDats[Models.Metadata.Rom.SHA256Key].Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, fieldDats[Models.Metadata.Rom.SHA256Key].Header.GetFieldValue<string?>(Models.Metadata.Header.NameKey) + " (SHA-256)");
            fieldDats[Models.Metadata.Rom.SHA256Key].Header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, fieldDats[Models.Metadata.Rom.SHA256Key].Header.GetFieldValue<string?>(Models.Metadata.Header.DescriptionKey) + " (SHA-256)");

            fieldDats[Models.Metadata.Rom.SHA1Key] = DatFile.Create(datFile.Header.CloneStandard());
            fieldDats[Models.Metadata.Rom.SHA1Key].Header.SetFieldValue<string?>(DatHeader.FileNameKey, fieldDats[Models.Metadata.Rom.SHA1Key].Header.GetFieldValue<string?>(DatHeader.FileNameKey) + " (SHA-1)");
            fieldDats[Models.Metadata.Rom.SHA1Key].Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, fieldDats[Models.Metadata.Rom.SHA1Key].Header.GetFieldValue<string?>(Models.Metadata.Header.NameKey) + " (SHA-1)");
            fieldDats[Models.Metadata.Rom.SHA1Key].Header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, fieldDats[Models.Metadata.Rom.SHA1Key].Header.GetFieldValue<string?>(Models.Metadata.Header.DescriptionKey) + " (SHA-1)");

            fieldDats[Models.Metadata.Rom.MD5Key] = DatFile.Create(datFile.Header.CloneStandard());
            fieldDats[Models.Metadata.Rom.MD5Key].Header.SetFieldValue<string?>(DatHeader.FileNameKey, fieldDats[Models.Metadata.Rom.MD5Key].Header.GetFieldValue<string?>(DatHeader.FileNameKey) + " (MD5)");
            fieldDats[Models.Metadata.Rom.MD5Key].Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, fieldDats[Models.Metadata.Rom.MD5Key].Header.GetFieldValue<string?>(Models.Metadata.Header.NameKey) + " (MD5)");
            fieldDats[Models.Metadata.Rom.MD5Key].Header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, fieldDats[Models.Metadata.Rom.MD5Key].Header.GetFieldValue<string?>(Models.Metadata.Header.DescriptionKey) + " (MD5)");

            fieldDats[Models.Metadata.Rom.CRCKey] = DatFile.Create(datFile.Header.CloneStandard());
            fieldDats[Models.Metadata.Rom.CRCKey].Header.SetFieldValue<string?>(DatHeader.FileNameKey, fieldDats[Models.Metadata.Rom.CRCKey].Header.GetFieldValue<string?>(DatHeader.FileNameKey) + " (CRC)");
            fieldDats[Models.Metadata.Rom.CRCKey].Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, fieldDats[Models.Metadata.Rom.CRCKey].Header.GetFieldValue<string?>(Models.Metadata.Header.NameKey) + " (CRC)");
            fieldDats[Models.Metadata.Rom.CRCKey].Header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, fieldDats[Models.Metadata.Rom.CRCKey].Header.GetFieldValue<string?>(Models.Metadata.Header.DescriptionKey) + " (CRC)");

            fieldDats["null"] = DatFile.Create(datFile.Header.CloneStandard());
            fieldDats["null"].Header.SetFieldValue<string?>(DatHeader.FileNameKey, fieldDats["null"].Header.GetFieldValue<string?>(DatHeader.FileNameKey) + " (Other)");
            fieldDats["null"].Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, fieldDats["null"].Header.GetFieldValue<string?>(Models.Metadata.Header.NameKey) + " (Other)");
            fieldDats["null"].Header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, fieldDats["null"].Header.GetFieldValue<string?>(Models.Metadata.Header.DescriptionKey) + " (Other)");

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
                    switch (item)
                    {
                        case Disk disk:
                            if (disk.GetFieldValue<ItemStatus>(Models.Metadata.Disk.StatusKey) == ItemStatus.Nodump)
                                fieldDats[Models.Metadata.Disk.StatusKey].Items.Add(key, item);
                            else if (!string.IsNullOrEmpty(disk.GetFieldValue<string?>(Models.Metadata.Disk.SHA1Key)))
                                fieldDats[Models.Metadata.Disk.SHA1Key].Items.Add(key, item);
                            else if (!string.IsNullOrEmpty(disk.GetFieldValue<string?>(Models.Metadata.Disk.MD5Key)))
                                fieldDats[Models.Metadata.Disk.MD5Key].Items.Add(key, item);
                            else if (!string.IsNullOrEmpty(disk.GetFieldValue<string?>(Models.Metadata.Disk.MD5Key)))
                                fieldDats[Models.Metadata.Disk.MD5Key].Items.Add(key, item);
                            else
                                fieldDats["null"].Items.Add(key, item);
                            break;

                        case Media media:
                            if (!string.IsNullOrEmpty(media.GetFieldValue<string?>(Models.Metadata.Media.SHA256Key)))
                                fieldDats[Models.Metadata.Media.SHA256Key].Items.Add(key, item);
                            else if (!string.IsNullOrEmpty(media.GetFieldValue<string?>(Models.Metadata.Media.SHA1Key)))
                                fieldDats[Models.Metadata.Media.SHA1Key].Items.Add(key, item);
                            else if (!string.IsNullOrEmpty(media.GetFieldValue<string?>(Models.Metadata.Media.MD5Key)))
                                fieldDats[Models.Metadata.Media.MD5Key].Items.Add(key, item);
                            else
                                fieldDats["null"].Items.Add(key, item);
                            break;

                        case Rom rom:
                            if (rom.GetFieldValue<ItemStatus>(Models.Metadata.Rom.StatusKey) == ItemStatus.Nodump)
                                fieldDats[Models.Metadata.Rom.StatusKey].Items.Add(key, item);
                            else if (!string.IsNullOrEmpty(rom.GetFieldValue<string?>(Models.Metadata.Rom.SHA512Key)))
                                fieldDats[Models.Metadata.Rom.SHA512Key].Items.Add(key, item);
                            else if (!string.IsNullOrEmpty(rom.GetFieldValue<string?>(Models.Metadata.Rom.SHA384Key)))
                                fieldDats[Models.Metadata.Rom.SHA384Key].Items.Add(key, item);
                            else if (!string.IsNullOrEmpty(rom.GetFieldValue<string?>(Models.Metadata.Rom.SHA256Key)))
                                fieldDats[Models.Metadata.Rom.SHA256Key].Items.Add(key, item);
                            else if (!string.IsNullOrEmpty(rom.GetFieldValue<string?>(Models.Metadata.Rom.SHA1Key)))
                                fieldDats[Models.Metadata.Rom.SHA1Key].Items.Add(key, item);
                            else if (!string.IsNullOrEmpty(rom.GetFieldValue<string?>(Models.Metadata.Rom.MD5Key)))
                                fieldDats[Models.Metadata.Rom.MD5Key].Items.Add(key, item);
                            else if (!string.IsNullOrEmpty(rom.GetFieldValue<string?>(Models.Metadata.Rom.CRCKey)))
                                fieldDats[Models.Metadata.Rom.CRCKey].Items.Add(key, item);
                            else
                                fieldDats["null"].Items.Add(key, item);
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
            tempDat.Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, null);

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
                if (tempDat.Header.GetFieldValue<string?>(Models.Metadata.Header.NameKey) != null && tempDat.Header.GetFieldValue<string?>(Models.Metadata.Header.NameKey) != Path.GetDirectoryName(key))
                {
                    // Reset the DAT for the next items
                    tempDat = DatFile.Create(datFile.Header);
                    tempDat.Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, null);
                }

                // Clean the input list and set all games to be pathless
                ConcurrentList<DatItem>? items = datFile.Items[key];
                if (items == null)
#if NET40_OR_GREATER || NETCOREAPP
                    return;
#else
                    continue;
#endif
                items.ForEach(item => item.GetFieldValue<Machine>(DatItem.MachineKey)!.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, Path.GetFileName(item.GetFieldValue<Machine>(DatItem.MachineKey)!.GetFieldValue<string?>(Models.Metadata.Machine.NameKey))));
                items.ForEach(item => item.GetFieldValue<Machine>(DatItem.MachineKey)!.SetFieldValue<string?>(Models.Metadata.Machine.DescriptionKey, Path.GetFileName(item.GetFieldValue<Machine>(DatItem.MachineKey)!.GetFieldValue<string?>(Models.Metadata.Machine.DescriptionKey))));

                // Now add the game to the output DAT
                tempDat.Items.AddRange(key, items);

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
            string? name = newDatFile.Header.GetFieldValue<string?>(Models.Metadata.Header.NameKey);
            string? expName = name?.Replace("/", " - ")?.Replace("\\", " - ");

            // Now set the new output values
#if NET20 || NET35
            newDatFile.Header.SetFieldValue<string?>(DatHeader.FileNameKey, string.IsNullOrEmpty(name)
                ? datFile.Header.GetFieldValue<string?>(DatHeader.FileNameKey)
                : (shortname
                    ? Path.GetFileName(name)
                    : expName
                ));
#else
            newDatFile.Header.SetFieldValue<string?>(DatHeader.FileNameKey, WebUtility.HtmlDecode(string.IsNullOrEmpty(name)
                ? datFile.Header.GetFieldValue<string?>(DatHeader.FileNameKey)
                : (shortname
                    ? Path.GetFileName(name)
                    : expName
                    )
                ));
#endif
            newDatFile.Header.SetFieldValue<string?>(DatHeader.FileNameKey, restore
                ? $"{datFile.Header.GetFieldValue<string?>(DatHeader.FileNameKey)} ({newDatFile.Header.GetFieldValue<string?>(DatHeader.FileNameKey)})"
                : newDatFile.Header.GetFieldValue<string?>(DatHeader.FileNameKey));
            newDatFile.Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, $"{datFile.Header.GetFieldValue<string?>(Models.Metadata.Header.NameKey)} ({expName})");
            newDatFile.Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, string.IsNullOrEmpty(datFile.Header.GetFieldValue<string?>(Models.Metadata.Header.DescriptionKey))
                ? newDatFile.Header.GetFieldValue<string?>(Models.Metadata.Header.NameKey)
                : $"{datFile.Header.GetFieldValue<string?>(Models.Metadata.Header.DescriptionKey)} ({expName})");
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

            DatFile lessThan = DatFile.Create(datFile.Header.CloneStandard());
            lessThan.Header.SetFieldValue<string?>(DatHeader.FileNameKey, lessThan.Header.GetFieldValue<string?>(DatHeader.FileNameKey) + $" (less than {radix})");
            lessThan.Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, lessThan.Header.GetFieldValue<string?>(Models.Metadata.Header.NameKey) + $" (less than {radix})");
            lessThan.Header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, lessThan.Header.GetFieldValue<string?>(Models.Metadata.Header.DescriptionKey) + $" (less than {radix})");

            DatFile greaterThan = DatFile.Create(datFile.Header.CloneStandard());
            greaterThan.Header.SetFieldValue<string?>(DatHeader.FileNameKey, greaterThan.Header.GetFieldValue<string?>(DatHeader.FileNameKey) + $" (equal-greater than {radix})");
            greaterThan.Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, greaterThan.Header.GetFieldValue<string?>(Models.Metadata.Header.NameKey) + $" (equal-greater than {radix})");
            greaterThan.Header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, greaterThan.Header.GetFieldValue<string?>(Models.Metadata.Header.DescriptionKey) + $" (equal-greater than {radix})");

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
                    if (item is not Rom rom)
                        lessThan.Items.Add(key, item);

                    // If the file is a Rom and has no size, put it in the "lesser" dat
                    else if (rom.GetFieldValue<string?>(Models.Metadata.Rom.SizeKey) == null)
                        lessThan.Items.Add(key, item);

                    // If the file is a Rom and less than the radix, put it in the "lesser" dat
                    else if (NumberHelper.ConvertToInt64(rom.GetFieldValue<string?>(Models.Metadata.Rom.SizeKey)) < radix)
                        lessThan.Items.Add(key, item);

                    // If the file is a Rom and greater than or equal to the radix, put it in the "greater" dat
                    else if (NumberHelper.ConvertToInt64(rom.GetFieldValue<string?>(Models.Metadata.Rom.SizeKey)) >= radix)
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
            currentDat.Header.SetFieldValue<string?>(DatHeader.FileNameKey, currentDat.Header.GetFieldValue<string?>(DatHeader.FileNameKey) + $"_{currentIndex}");
            currentDat.Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, currentDat.Header.GetFieldValue<string?>(Models.Metadata.Header.NameKey) + $"_{currentIndex}");
            currentDat.Header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, currentDat.Header.GetFieldValue<string?>(Models.Metadata.Header.DescriptionKey) + $"_{currentIndex}");

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
                        machineSize += NumberHelper.ConvertToInt64(rom.GetFieldValue<string?>(Models.Metadata.Rom.SizeKey)) ?? 0;
                        if ((NumberHelper.ConvertToInt64(rom.GetFieldValue<string?>(Models.Metadata.Rom.SizeKey)) ?? 0) > chunkSize)
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
                    currentDat.Header.SetFieldValue<string?>(DatHeader.FileNameKey, currentDat.Header.GetFieldValue<string?>(DatHeader.FileNameKey) + $"_{currentIndex}");
                    currentDat.Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, currentDat.Header.GetFieldValue<string?>(Models.Metadata.Header.NameKey) + $"_{currentIndex}");
                    currentDat.Header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, currentDat.Header.GetFieldValue<string?>(Models.Metadata.Header.DescriptionKey) + $"_{currentIndex}");
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
                typeDats[itemType].Header.SetFieldValue<string?>(DatHeader.FileNameKey, typeDats[itemType].Header.GetFieldValue<string?>(DatHeader.FileNameKey) + $" ({itemType})");
                typeDats[itemType].Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, typeDats[itemType].Header.GetFieldValue<string?>(Models.Metadata.Header.NameKey) + $" ({itemType})");
                typeDats[itemType].Header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, typeDats[itemType].Header.GetFieldValue<string?>(Models.Metadata.Header.DescriptionKey) + $" ({itemType})");
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
                    if (item.GetFieldValue<ItemType>(Models.Metadata.DatItem.TypeKey) == itemType)
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