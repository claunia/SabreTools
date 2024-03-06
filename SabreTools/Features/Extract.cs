using System.IO;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using SabreTools.Core.Tools;
using SabreTools.FileTypes;
using SabreTools.Hashing;
using SabreTools.Help;
using SabreTools.IO;
using SabreTools.Skippers;

namespace SabreTools.Features
{
    internal class Extract : BaseFeature
    {
        public const string Value = "Extract";

        public Extract()
        {
            Name = Value;
            Flags = ["ex", "extract"];
            Description = "Extract and remove copier headers";
            _featureType = ParameterType.Flag;
            LongDescription = @"This will detect, store, and remove copier headers from a file or folder of files. The headers are backed up and collated by the hash of the unheadered file. Files are then output without the detected copier header alongside the originals with the suffix .new. No input files are altered in the process. Only uncompressed files will be processed.

The following systems have headers that this program can work with:
  - Atari 7800
  - Atari Lynx
  - Commodore PSID Music
  - NEC PC - Engine / TurboGrafx 16
  - Nintendo Famicom / Nintendo Entertainment System
  - Nintendo Famicom Disk System
  - Nintendo Super Famicom / Super Nintendo Entertainment System
  - Nintendo Super Famicom / Super Nintendo Entertainment System SPC";
            Features = [];

            // Common Features
            AddCommonFeatures();

            AddFeature(OutputDirStringInput);
            AddFeature(NoStoreHeaderFlag);
        }

        public override bool ProcessFeatures(Dictionary<string, Feature?> features)
        {
            // If the base fails, just fail out
            if (!base.ProcessFeatures(features))
                return false;

            // Get feature flags
            bool nostore = GetBoolean(features, NoStoreHeaderValue);

            // Get only files from the inputs
            List<ParentablePath> files = PathTool.GetFilesOnly(Inputs);
            foreach (ParentablePath file in files)
            {
                DetectTransformStore(file.CurrentPath, OutputDir, nostore);
            }

            return true;
        }

        /// <summary>
        /// Detect header skipper compliance and create an output file
        /// </summary>
        /// <param name="file">Name of the file to be parsed</param>
        /// <param name="outDir">Output directory to write the file to, empty means the same directory as the input file</param>
        /// <param name="nostore">True if headers should not be stored in the database, false otherwise</param>
        /// <returns>True if the output file was created, false otherwise</returns>
        private bool DetectTransformStore(string file, string? outDir, bool nostore)
        {
            // Create the output directory if it doesn't exist
            if (!string.IsNullOrWhiteSpace(outDir) && !Directory.Exists(outDir))
                Directory.CreateDirectory(outDir);

            logger.User($"\nGetting skipper information for '{file}'");

            // Get the skipper rule that matches the file, if any
            SkipperMatch.Init();
            Rule rule = SkipperMatch.GetMatchingRule(file, string.Empty);

            // If we have an empty rule, return false
            if (rule.Tests == null || rule.Tests.Length == 0 || rule.Operation != HeaderSkipOperation.None)
                return false;

            logger.User("File has a valid copier header");

            // Get the header bytes from the file first
            string hstr;
            try
            {
                // Extract the header as a string for the database
                using var fs = File.OpenRead(file);
                int startOffset = int.Parse(rule.StartOffset ?? "0");
                byte[] hbin = new byte[startOffset];
                fs.Read(hbin, 0, startOffset);
                hstr = TextHelper.ByteArrayToString(hbin)!;
            }
            catch
            {
                return false;
            }

            // Apply the rule to the file
            string newfile = string.IsNullOrWhiteSpace(outDir) ? Path.GetFullPath(file) + ".new" : Path.Combine(outDir, Path.GetFileName(file));
            rule.TransformFile(file, newfile);

            // If the output file doesn't exist, return false
            if (!File.Exists(newfile))
                return false;

            // Now add the information to the database if it's not already there
            if (!nostore)
            {
                BaseFile? baseFile = BaseFile.GetInfo(newfile, hashes: [HashType.SHA1], asFiles: TreatAsFile.NonArchive);
                AddHeaderToDatabase(hstr, TextHelper.ByteArrayToString(baseFile!.SHA1)!, rule.SourceFile!);
            }

            return true;
        }

        /// <summary>
        /// Add a header to the database
        /// </summary>
        /// <param name="header">String representing the header bytes</param>
        /// <param name="SHA1">SHA-1 of the deheadered file</param>
        /// <param name="type">Name of the source skipper file</param>
        private void AddHeaderToDatabase(string header, string SHA1, string source)
        {
            // Ensure the database exists
            EnsureDatabase();

            // Open the database connection
            SqliteConnection dbc = new(HeadererConnectionString);
            dbc.Open();

            string query = $"SELECT * FROM data WHERE sha1='{SHA1}' AND header='{header}'";
            SqliteCommand slc = new(query, dbc);
            SqliteDataReader sldr = slc.ExecuteReader();
            bool exists = sldr.HasRows;

            if (!exists)
            {
                query = $"INSERT INTO data (sha1, header, type) VALUES ('{SHA1}', '{header}', '{source}')";
                slc = new SqliteCommand(query, dbc);
                logger.Verbose($"Result of inserting header: {slc.ExecuteNonQuery()}");
            }

            // Dispose of database objects
            slc.Dispose();
            sldr.Dispose();
            dbc.Dispose();
        }
    }
}
