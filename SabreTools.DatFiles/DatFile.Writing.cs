using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using SabreTools.Core;
using SabreTools.DatFiles.Reports;
using SabreTools.DatItems;
using SabreTools.IO;

// This file represents all methods related to writing to a file
namespace SabreTools.DatFiles
{
    public abstract partial class DatFile
    {
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
    }
}