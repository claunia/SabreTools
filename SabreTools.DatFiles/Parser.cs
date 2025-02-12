using System;
using System.IO;
using System.Text.RegularExpressions;
using SabreTools.Core.Tools;
using SabreTools.DatItems;
using SabreTools.IO;
using SabreTools.IO.Extensions;
using SabreTools.IO.Logging;

namespace SabreTools.DatFiles
{
    /// <summary>
    /// Helper methods for parsing into DatFiles
    /// </summary>
    public static class Parser
    {
        #region Logging

        /// <summary>
        /// Logging object
        /// </summary>
        private static readonly Logger _staticLogger = new();

        #endregion

        /// <summary>
        /// Parse a DAT and return all found games and roms within
        /// </summary>
        /// <param name="datFile">Current DatFile object to add to</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="keep">True if full pathnames are to be kept, false otherwise (default)</param>
        /// <param name="keepext">True if original extension should be kept, false otherwise (default)</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <param name="throwOnError">True if the error that is thrown should be thrown back to the caller, false otherwise</param>
        public static void ParseInto(
            DatFile datFile,
            string filename,
            int indexId = 0,
            bool keep = false,
            bool keepext = false,
            bool statsOnly = false,
            bool throwOnError = false)
        {
            // Check the file extension first as a safeguard
            if (!Utilities.HasValidDatExtension(filename))
                return;

            // If the output filename isn't set already, get the internal filename
            datFile.Header.SetFieldValue<string?>(DatHeader.FileNameKey, string.IsNullOrEmpty(datFile.Header.GetStringFieldValue(DatHeader.FileNameKey))
                ? (keepext
                    ? Path.GetFileName(filename)
                    : Path.GetFileNameWithoutExtension(filename))
                : datFile.Header.GetStringFieldValue(DatHeader.FileNameKey));

            // If the output type isn't set already, get the internal output type
            DatFormat datFormat = GetDatFormat(filename);
            datFile.Header.SetFieldValue<DatFormat>(DatHeader.DatFormatKey, datFile.Header.GetFieldValue<DatFormat>(DatHeader.DatFormatKey) == 0
                ? datFormat
                : datFile.Header.GetFieldValue<DatFormat>(DatHeader.DatFormatKey));
            datFile.Items.SetBucketedBy(ItemKey.CRC); // Setting this because it can reduce issues later

            InternalStopwatch watch = new($"Parsing '{filename}' into internal DAT");

            // Now parse the correct type of DAT
            try
            {
                DatFile parsingDatFile = DatFileTool.CreateDatFile(datFormat, datFile);
                parsingDatFile.ParseFile(filename, indexId, keep, statsOnly: statsOnly, throwOnError: throwOnError);
            }
            catch (Exception ex) when (!throwOnError)
            {
                _staticLogger.Error(ex, $"Error with file '{filename}'");
            }

            watch.Stop();
        }

        /// <summary>
        /// Create a DatFile and parse statistics into it
        /// </summary>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="throwOnError">True if the error that is thrown should be thrown back to the caller, false otherwise</param>
        public static DatFile ParseStatistics(string? filename, bool throwOnError = false)
        {
            // Null filenames are invalid
            if (filename == null)
                return DatFileTool.CreateDatFile();

            DatFile datFile = DatFileTool.CreateDatFile();
            ParseInto(datFile, filename, statsOnly: true, throwOnError: throwOnError);
            return datFile;
        }

        /// <summary>
        /// Get what type of DAT the input file is
        /// </summary>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <returns>The DatFormat corresponding to the DAT</returns>
        private static DatFormat GetDatFormat(string filename)
        {
            // Limit the output formats based on extension
            if (!Utilities.HasValidDatExtension(filename))
                return 0;

            // Get the extension from the filename
            string? ext = filename.GetNormalizedExtension();

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
                case "md2":
                    return DatFormat.RedumpMD2;
                case "md4":
                    return DatFormat.RedumpMD4;
                case "md5":
                    return DatFormat.RedumpMD5;
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
                using StreamReader sr = File.OpenText(filename);
                first = FindNextLine(sr);
                second = FindNextLine(sr);
            }
            catch { }

            // If we have an XML-based DAT
            if (first.Contains("<?xml") && first.Contains("?>"))
            {
                if (second.StartsWith("<!doctype datafile"))
                    return DatFormat.Logiqx;
                else if (second.StartsWith("<datafile xmlns:xsi"))
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

                else if (second.StartsWith("<files"))
                    return DatFormat.ArchiveDotOrg;

                // Older and non-compliant DATs
                else
                    return DatFormat.Logiqx;
            }

            // If we have an SMDB (SHA-256, Filename, SHA-1, MD5, CRC32)
            else if (Regex.IsMatch(first, @"[0-9a-f]{64}\t.*?\t[0-9a-f]{40}\t[0-9a-f]{32}\t[0-9a-f]{8}"))
                return DatFormat.EverdriveSMDB;

            // If we have an INI-based DAT
#if NETFRAMEWORK
            else if (first.Contains("[") && first.Contains("]"))
#else
            else if (first.Contains('[') && first.Contains(']'))
#endif
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

#if NETFRAMEWORK
            else if (first.ToLowerInvariant().Contains("#name;title;emulator;cloneof;year;manufacturer;category;players;rotation;control;status;displaycount;displaytype;altromname;alttitle;extra"))
#else
            else if (first.Contains("#Name;Title;Emulator;CloneOf;Year;Manufacturer;Category;Players;Rotation;Control;Status;DisplayCount;DisplayType;AltRomname;AltTitle;Extra", StringComparison.InvariantCultureIgnoreCase))
#endif
                return DatFormat.AttractMode;

#if NETFRAMEWORK
            else if (first.ToLowerInvariant().Contains("#romname;title;emulator;cloneof;year;manufacturer;category;players;rotation;control;status;displaycount;displaytype;altromname;alttitle;extra"))
#else
            else if (first.Contains("#RomName;Title;Emulator;CloneOf;Year;Manufacturer;Category;Players;Rotation;Control;Status;DisplayCount;DisplayType;AltRomname;AltTitle;Extra", StringComparison.InvariantCultureIgnoreCase))
#endif
                return DatFormat.AttractMode;

            else
                return DatFormat.ClrMamePro;
        }

        /// <summary>
        /// Find the next non-whitespace, non-comment line from an input
        /// </summary>
        /// <param name="sr">StreamReader representing the input</param>
        /// <returns>The next complete line, if possible</returns>
        private static string FindNextLine(StreamReader sr)
        {
            // If we're at the end of the stream, we can't do anything
            if (sr.EndOfStream)
                return string.Empty;

            // Find the first line that's not whitespace or an XML comment
            string? line = sr.ReadLine()?.ToLowerInvariant()?.Trim();
            bool inComment = line?.StartsWith("<!--") ?? false;
            while ((string.IsNullOrEmpty(line) || inComment) && !sr.EndOfStream)
            {
                // Null lines should not happen
                if (line == null)
                {
                    line = sr.ReadLine()?.ToLowerInvariant()?.Trim();
                    continue;
                }

                // Self-contained comment lines
                if (line.StartsWith("<!--") && line.EndsWith("-->"))
                {
                    inComment = false;
                    line = sr.ReadLine()?.ToLowerInvariant()?.Trim();
                }

                // Start of block comments
                else if (line.StartsWith("<!--"))
                {
                    inComment = true;
                    line = sr.ReadLine()?.ToLowerInvariant()?.Trim();
                }

                // End of block comments
                else if (inComment && line.EndsWith("-->"))
                {
                    line = sr.ReadLine()?.ToLowerInvariant()?.Trim();
                    inComment = line?.StartsWith("<!--") ?? false;
                }

                // Empty lines are just skipped
                else if (string.IsNullOrEmpty(line))
                {
                    line = sr.ReadLine()?.ToLowerInvariant()?.Trim();
                    inComment |= line?.StartsWith("<!--") ?? false;
                }

                // In-comment lines
                else if (inComment)
                {
                    line = sr.ReadLine()?.ToLowerInvariant()?.Trim();
                }
            }

            // If we ended in a comment, return an empty string
            if (inComment)
                return string.Empty;

            return line ?? string.Empty;
        }
    }
}