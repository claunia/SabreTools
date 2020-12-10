using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using SabreTools.Core;
using SabreTools.DatItems;
using SabreTools.IO;
using NaturalSort;

// This file represents all methods related to splitting a DatFile into multiple
namespace SabreTools.DatFiles
{
    // TODO: Implement Level split
    public partial class DatTool
    {
        /// <summary>
        /// Split a DAT by input extensions
        /// </summary>
        /// <param name="datFile">Current DatFile object to split</param>
        /// <param name="extA">List of extensions to split on (first DAT)</param>
        /// <param name="extB">List of extensions to split on (second DAT)</param>
        /// <returns>Extension Set A and Extension Set B DatFiles</returns>
        public static (DatFile extADat, DatFile extBDat) SplitByExtension(DatFile datFile, List<string> extA, List<string> extB)
        {
            // If roms is empty, return false
            if (datFile.Items.TotalCount == 0)
                return (null, null);

            // Make sure all of the extensions don't have a dot at the beginning
            var newExtA = extA.Select(s => s.TrimStart('.').ToLowerInvariant());
            string newExtAString = string.Join(",", newExtA);

            var newExtB = extB.Select(s => s.TrimStart('.').ToLowerInvariant());
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
            Parallel.ForEach(datFile.Items.Keys, Globals.ParallelOptions, key =>
            {
                List<DatItem> items = datFile.Items[key];
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
        /// <param name="datFile">Current DatFile object to split</param>
        /// <returns>Dictionary of Field to DatFile mappings</returns>
        public static Dictionary<Field, DatFile> SplitByHash(DatFile datFile)
        {
            // Create each of the respective output DATs
            logger.User("Creating and populating new DATs");

            // Create the set of field-to-dat mappings
            Dictionary<Field, DatFile> fieldDats = new Dictionary<Field, DatFile>();

            // TODO: Can this be made into a loop?
            fieldDats[Field.DatItem_Status] = DatFile.Create(datFile.Header.CloneStandard());
            fieldDats[Field.DatItem_Status].Header.FileName += " (Nodump)";
            fieldDats[Field.DatItem_Status].Header.Name += " (Nodump)";
            fieldDats[Field.DatItem_Status].Header.Description += " (Nodump)";

            fieldDats[Field.DatItem_SHA512] = DatFile.Create(datFile.Header.CloneStandard());
            fieldDats[Field.DatItem_SHA512].Header.FileName += " (SHA-512)";
            fieldDats[Field.DatItem_SHA512].Header.Name += " (SHA-512)";
            fieldDats[Field.DatItem_SHA512].Header.Description += " (SHA-512)";

            fieldDats[Field.DatItem_SHA384] = DatFile.Create(datFile.Header.CloneStandard());
            fieldDats[Field.DatItem_SHA384].Header.FileName += " (SHA-384)";
            fieldDats[Field.DatItem_SHA384].Header.Name += " (SHA-384)";
            fieldDats[Field.DatItem_SHA384].Header.Description += " (SHA-384)";

            fieldDats[Field.DatItem_SHA256] = DatFile.Create(datFile.Header.CloneStandard());
            fieldDats[Field.DatItem_SHA256].Header.FileName += " (SHA-256)";
            fieldDats[Field.DatItem_SHA256].Header.Name += " (SHA-256)";
            fieldDats[Field.DatItem_SHA256].Header.Description += " (SHA-256)";

            fieldDats[Field.DatItem_SHA1] = DatFile.Create(datFile.Header.CloneStandard());
            fieldDats[Field.DatItem_SHA1].Header.FileName += " (SHA-1)";
            fieldDats[Field.DatItem_SHA1].Header.Name += " (SHA-1)";
            fieldDats[Field.DatItem_SHA1].Header.Description += " (SHA-1)";

#if NET_FRAMEWORK
            fieldDats[Field.DatItem_RIPEMD160] = DatFile.Create(datFile.Header.CloneStandard());
            fieldDats[Field.DatItem_RIPEMD160].Header.FileName += " (RIPEMD160)";
            fieldDats[Field.DatItem_RIPEMD160].Header.Name += " (RIPEMD160)";
            fieldDats[Field.DatItem_RIPEMD160].Header.Description += " (RIPEMD160)";
#endif

            fieldDats[Field.DatItem_MD5] = DatFile.Create(datFile.Header.CloneStandard());
            fieldDats[Field.DatItem_MD5].Header.FileName += " (MD5)";
            fieldDats[Field.DatItem_MD5].Header.Name += " (MD5)";
            fieldDats[Field.DatItem_MD5].Header.Description += " (MD5)";

            fieldDats[Field.DatItem_CRC] = DatFile.Create(datFile.Header.CloneStandard());
            fieldDats[Field.DatItem_CRC].Header.FileName += " (CRC)";
            fieldDats[Field.DatItem_CRC].Header.Name += " (CRC)";
            fieldDats[Field.DatItem_CRC].Header.Description += " (CRC)";

            fieldDats[Field.NULL] = DatFile.Create(datFile.Header.CloneStandard());
            fieldDats[Field.NULL].Header.FileName += " (Other)";
            fieldDats[Field.NULL].Header.Name += " (Other)";
            fieldDats[Field.NULL].Header.Description += " (Other)";

            // Now populate each of the DAT objects in turn
            Parallel.ForEach(datFile.Items.Keys, Globals.ParallelOptions, key =>
            {
                List<DatItem> items = datFile.Items[key];
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
        /// <param name="datFile">Current DatFile object to split</param>
        /// <param name="outDir">Name of the directory to write the DATs out to</param>
        /// <param name="shortname">True if short names should be used, false otherwise</param>
        /// <param name="basedat">True if original filenames should be used as the base for output filename, false otherwise</param>
        /// <returns>True if split succeeded, false otherwise</returns>
        public static bool SplitByLevel(DatFile datFile, string outDir, bool shortname, bool basedat)
        {
            // First, bucket by games so that we can do the right thing
            datFile.Items.BucketBy(Field.Machine_Name, DedupeType.None, lower: false, norename: true);

            // Create a temporary DAT to add things to
            DatFile tempDat = DatFile.Create(datFile.Header);
            tempDat.Header.Name = null;

            // Sort the input keys
            List<string> keys = datFile.Items.Keys.ToList();
            keys.Sort(SplitByLevelSort);

            // Then, we loop over the games
            Parallel.ForEach(keys, Globals.ParallelOptions, key =>
            {
                // Here, the key is the name of the game to be used for comparison
                if (tempDat.Header.Name != null && tempDat.Header.Name != Path.GetDirectoryName(key))
                {
                    // Reset the DAT for the next items
                    tempDat = DatFile.Create(datFile.Header);
                    tempDat.Header.Name = null;
                }

                // Clean the input list and set all games to be pathless
                List<DatItem> items = datFile.Items[key];
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
        private static int SplitByLevelSort(string a, string b)
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
        /// <param name="datFile">Current DatFile object to split</param>
        /// <param name="newDatFile">DAT to clean and write out</param>
        /// <param name="outDir">Directory to write out to</param>
        /// <param name="shortname">True if short naming scheme should be used, false otherwise</param>
        /// <param name="restore">True if original filenames should be used as the base for output filename, false otherwise</param>
        private static void SplitByLevelHelper(DatFile datFile, DatFile newDatFile, string outDir, bool shortname, bool restore)
        {
            // Get the name from the DAT to use separately
            string name = newDatFile.Header.Name;
            string expName = name.Replace("/", " - ").Replace("\\", " - ");

            // Now set the new output values
            newDatFile.Header.FileName = WebUtility.HtmlDecode(string.IsNullOrWhiteSpace(name)
                ? datFile.Header.FileName
                : (shortname
                    ? Path.GetFileName(name)
                    : expName
                    )
                );
            newDatFile.Header.FileName = restore ? $"{datFile.Header.FileName} ({newDatFile.Header.FileName})" : newDatFile.Header.FileName;
            newDatFile.Header.Name = $"{datFile.Header.Name} ({expName})";
            newDatFile.Header.Description = string.IsNullOrWhiteSpace(datFile.Header.Description) ? newDatFile.Header.Name : $"{datFile.Header.Description} ({expName})";
            newDatFile.Header.Type = null;

            // Write out the temporary DAT to the proper directory
            Write(newDatFile, outDir);
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
            logger.User("Creating and populating new DATs");

            DatFile lessThan = DatFile.Create(datFile.Header.CloneStandard());
            lessThan.Header.FileName += $" (less than {radix})";
            lessThan.Header.Name += $" (less than {radix})";
            lessThan.Header.Description += $" (less than {radix})";

            DatFile greaterThan = DatFile.Create(datFile.Header.CloneStandard());
            greaterThan.Header.FileName += $" (equal-greater than {radix})";
            greaterThan.Header.Name += $" (equal-greater than {radix})";
            greaterThan.Header.Description += $" (equal-greater than {radix})";

            // Now populate each of the DAT objects in turn
            Parallel.ForEach(datFile.Items.Keys, Globals.ParallelOptions, key =>
            {
                List<DatItem> items = datFile.Items[key];
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
        /// <param name="datFile">Current DatFile object to split</param>
        /// <returns>Dictionary of ItemType to DatFile mappings</returns>
        public static Dictionary<ItemType, DatFile> SplitByType(DatFile datFile)
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
                typeDats[itemType] = DatFile.Create(datFile.Header.CloneStandard());
                typeDats[itemType].Header.FileName += $" ({itemType})";
                typeDats[itemType].Header.Name += $" ({itemType})";
                typeDats[itemType].Header.Description += $" ({itemType})";
            }

            // Now populate each of the DAT objects in turn
            Parallel.ForEach(outputTypes, Globals.ParallelOptions, itemType =>
            {
                FillWithItemType(datFile, typeDats[itemType], itemType);
            });

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
            Parallel.ForEach(datFile.Items.Keys, Globals.ParallelOptions, key =>
            {
                List<DatItem> items = DatItem.Merge(datFile.Items[key]);

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
    }
}