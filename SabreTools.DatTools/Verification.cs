using System.Collections.Generic;
using System.IO;
using System.Linq;
using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.DatFiles;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;
using SabreTools.FileTypes;
using SabreTools.FileTypes.Archives;
using SabreTools.Hashing;
using SabreTools.Logging;

namespace SabreTools.DatTools
{
    /// <summary>
    /// Helper methods for verifying data from DatFiles
    /// </summary>
    public class Verification
    {
        #region Logging

        /// <summary>
        /// Logging object
        /// </summary>
        private static readonly Logger logger = new();

        #endregion
        
        /// <summary>
        /// Verify a DatFile against a set of depots, leaving only missing files
        /// </summary>
        /// <param name="datFile">Current DatFile object to verify against</param>
        /// <param name="inputs">List of input directories to compare against</param>
        /// <returns>True if verification was a success, false otherwise</returns>
        public static bool VerifyDepot(DatFile datFile, List<string> inputs)
        {
            bool success = true;

            InternalStopwatch watch = new("Verifying all from supplied depots");

            // Now loop through and get only directories from the input paths
            List<string> directories = [];
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
            datFile.Items.BucketBy(ItemKey.SHA1, DedupeType.None);

            // Then we want to loop through each of the hashes and see if we can rebuild
            var keys = datFile.Items.SortedKeys.ToList();
            foreach (string hash in keys)
            {
                // Pre-empt any issues that could arise from string length
                if (hash.Length != Constants.SHA1Length)
                    continue;

                logger.User($"Checking hash '{hash}'");

                // Get the extension path for the hash
                string? subpath = Utilities.GetDepotPath(hash, datFile.Header.InputDepot?.Depth ?? 0);
                if (subpath == null)
                    continue;

                // Find the first depot that includes the hash
                string? foundpath = null;
                foreach (string directory in directories)
                {
                    if (System.IO.File.Exists(Path.Combine(directory, subpath)))
                    {
                        foundpath = Path.Combine(directory, subpath);
                        break;
                    }
                }

                // If we didn't find a path, then we continue
                if (foundpath == null)
                    continue;

                // If we have a path, we want to try to get the rom information
                GZipArchive tgz = new(foundpath);
                BaseFile? fileinfo = tgz.GetTorrentGZFileInfo();

                // If the file information is null, then we continue
                if (fileinfo == null)
                    continue;

                // Now we want to remove all duplicates from the DAT
                datFile.Items.GetDuplicates(new Rom(fileinfo))
                    .AddRange(datFile.Items.GetDuplicates(new Disk(fileinfo)));
            }

            watch.Stop();

            // Set fixdat headers in case of writing out
            datFile.Header.SetFieldValue<string?>(DatHeader.FileNameKey, $"fixDAT_{datFile.Header.GetFieldValue<string?>(DatHeader.FileNameKey)}");
            datFile.Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, $"fixDAT_{datFile.Header.GetFieldValue<string?>(Models.Metadata.Header.NameKey)}");
            datFile.Header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, $"fixDAT_{datFile.Header.GetFieldValue<string?>(Models.Metadata.Header.DescriptionKey)}");
            datFile.Items.ClearMarked();

            return success;
        }

        /// <summary>
        /// Verify a DatFile against a set of inputs, leaving only missing files
        /// </summary>
        /// <param name="datFile">Current DatFile object to verify against</param>
        /// <param name="hashOnly">True if only hashes should be checked, false for full file information</param>
        /// <returns>True if verification was a success, false otherwise</returns>
        public static bool VerifyGeneric(DatFile datFile, bool hashOnly)
        {
            bool success = true;

            InternalStopwatch watch = new("Verifying all from supplied paths");

            // Force bucketing according to the flags
            datFile.Items.SetBucketedBy(ItemKey.NULL);
            if (hashOnly)
                datFile.Items.BucketBy(ItemKey.CRC, DedupeType.Full);
            else
                datFile.Items.BucketBy(ItemKey.Machine, DedupeType.Full);

            // Then mark items for removal
            var keys = datFile.Items.SortedKeys.ToList();
            foreach (string key in keys)
            {
                ConcurrentList<DatItem>? items = datFile.Items[key];
                if (items == null)
                    continue;

                for (int i = 0; i < items.Count; i++)
                {
                    // Unmatched items will have a source ID of int.MaxValue, remove all others
                    if (items[i].GetFieldValue<Source?>(DatItem.SourceKey)?.Index != int.MaxValue)
                        items[i].SetFieldValue<bool>(DatItem.RemoveKey, true);
                }

                // Set the list back, just in case
                datFile.Items[key] = items;
            }

            watch.Stop();

            // Set fixdat headers in case of writing out
            datFile.Header.SetFieldValue<string?>(DatHeader.FileNameKey, $"fixDAT_{datFile.Header.GetFieldValue<string?>(DatHeader.FileNameKey)}");
            datFile.Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, $"fixDAT_{datFile.Header.GetFieldValue<string?>(Models.Metadata.Header.NameKey)}");
            datFile.Header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, $"fixDAT_{datFile.Header.GetFieldValue<string?>(Models.Metadata.Header.DescriptionKey)}");
            datFile.Items.ClearMarked();

            return success;
        }
    }
}