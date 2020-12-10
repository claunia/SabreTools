using System.Collections.Generic;
using System.IO;
using System.Linq;

using SabreTools.Core;
using SabreTools.DatItems;
using SabreTools.FileTypes;
using SabreTools.IO;
using SabreTools.Logging;

// This file represents all methods related to verifying with a DatFile
namespace SabreTools.DatFiles
{
    public abstract partial class DatFile
    {
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
    }
}