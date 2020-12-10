using System;
using System.IO;
using System.Text.RegularExpressions;

using SabreTools.Core;
using SabreTools.DatItems;
using SabreTools.IO;

// This file represents all methods related to parsing from a file
namespace SabreTools.DatFiles
{
    public abstract partial class DatFile
    {
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
            DatFormat currentPathFormat = GetDatFormat(currentPath);
            Header.DatFormat = (Header.DatFormat == 0 ? currentPathFormat : Header.DatFormat);
            Items.SetBucketedBy(Field.DatItem_CRC); // Setting this because it can reduce issues later

            // Now parse the correct type of DAT
            try
            {
                Create(currentPathFormat, this, quotes)?.ParseFile(currentPath, indexId, keep, throwOnError);
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Error with file '{currentPath}'");
                if (throwOnError) throw ex;
            }
        }

        /// <summary>
        /// Get what type of DAT the input file is
        /// </summary>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <returns>The DatFormat corresponding to the DAT</returns>
        protected DatFormat GetDatFormat(string filename)
        {
            // Limit the output formats based on extension
            if (!PathExtensions.HasValidDatExtension(filename))
                return 0;

            // Get the extension from the filename
            string ext = PathExtensions.GetNormalizedExtension(filename);

            // Check if file exists
            if (!File.Exists(filename))
                return 0;
            
            // Some formats should only require the extension to know
            switch (ext)
            {
                case "csv":
                    return DatFormat.CSV;
                case "json":
                    return DatFormat.SabreJSON;
                case "md5":
                    return DatFormat.RedumpMD5;
#if NET_FRAMEWORK
                case "ripemd160":
                    return DatFormat.RedumpRIPEMD160;
#endif
                case "sfv":
                    return DatFormat.RedumpSFV;
                case "sha1":
                    return DatFormat.RedumpSHA1;
                case "sha256":
                    return DatFormat.RedumpSHA256;
                case "sha384":
                    return DatFormat.RedumpSHA384;
                case "sha512":
                    return DatFormat.RedumpSHA512;
                case "spamsum":
                    return DatFormat.RedumpSpamSum;
                case "ssv":
                    return DatFormat.SSV;
                case "tsv":
                    return DatFormat.TSV;
            }

            // For everything else, we need to read it
            // Get the first two non-whitespace, non-comment lines to check, if possible
            string first = string.Empty, second = string.Empty;

            try
            {
                using (StreamReader sr = File.OpenText(filename))
                {
                    first = sr.ReadLine().ToLowerInvariant();
                    while ((string.IsNullOrWhiteSpace(first) || first.StartsWith("<!--"))
                        && !sr.EndOfStream)
                    {
                        first = sr.ReadLine().ToLowerInvariant();
                    }

                    if (!sr.EndOfStream)
                    {
                        second = sr.ReadLine().ToLowerInvariant();
                        while (string.IsNullOrWhiteSpace(second) || second.StartsWith("<!--")
                            && !sr.EndOfStream)
                        {
                            second = sr.ReadLine().ToLowerInvariant();
                        }
                    }
                }
            }
            catch { }

            // If we have an XML-based DAT
            if (first.Contains("<?xml") && first.Contains("?>"))
            {
                if (second.StartsWith("<!doctype datafile"))
                    return DatFormat.Logiqx;

                else if (second.StartsWith("<!doctype mame")
                    || second.StartsWith("<!doctype m1")
                    || second.StartsWith("<mame")
                    || second.StartsWith("<m1"))
                    return DatFormat.Listxml;

                else if (second.StartsWith("<!doctype softwaredb"))
                    return DatFormat.OpenMSX;

                else if (second.StartsWith("<!doctype softwarelist"))
                    return DatFormat.SoftwareList;

                else if (second.StartsWith("<!doctype sabredat"))
                    return DatFormat.SabreXML;

                else if ((second.StartsWith("<dat") && !second.StartsWith("<datafile"))
                    || second.StartsWith("<?xml-stylesheet"))
                    return DatFormat.OfflineList;

                // Older and non-compliant DATs
                else
                    return DatFormat.Logiqx;
            }

            // If we have an SMDB (SHA-256, Filename, SHA-1, MD5, CRC32)
            else if (Regex.IsMatch(first, @"[0-9a-f]{64}\t.*?\t[0-9a-f]{40}\t[0-9a-f]{32}\t[0-9a-f]{8}"))
                return DatFormat.EverdriveSMDB;

            // If we have an INI-based DAT
            else if (first.Contains("[") && first.Contains("]"))
                return DatFormat.RomCenter;

            // If we have a listroms DAT
            else if (first.StartsWith("roms required for driver"))
                return DatFormat.Listrom;

            // If we have a CMP-based DAT
            else if (first.Contains("clrmamepro"))
                return DatFormat.ClrMamePro;

            else if (first.Contains("romvault"))
                return DatFormat.ClrMamePro;

            else if (first.Contains("doscenter"))
                return DatFormat.DOSCenter;

            else if (first.Contains("#Name;Title;Emulator;CloneOf;Year;Manufacturer;Category;Players;Rotation;Control;Status;DisplayCount;DisplayType;AltRomname;AltTitle;Extra"))
                return DatFormat.AttractMode;

            else
                return DatFormat.ClrMamePro;
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
    }
}