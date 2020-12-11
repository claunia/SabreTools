using System.IO;
using System.Collections.Generic;

using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.FileTypes;
using SabreTools.Help;
using SabreTools.IO;
using Microsoft.Data.Sqlite;

namespace SabreTools.Features
{
    internal class Restore : BaseFeature
    {
        public const string Value = "Restore";

        public Restore()
        {
            Name = Value;
            Flags = new List<string>() { "-re", "--restore" };
            Description = "Restore header to file based on SHA-1";
            _featureType = ParameterType.Flag;
            LongDescription = @"This will make use of stored copier headers and reapply them to files if they match the included hash. More than one header can be applied to a file, so they will be output to new files, suffixed with .newX, where X is a number. No input files are altered in the process. Only uncompressed files will be processed.

The following systems have headers that this program can work with:
  - Atari 7800
  - Atari Lynx
  - Commodore PSID Music
  - NEC PC - Engine / TurboGrafx 16
  - Nintendo Famicom / Nintendo Entertainment System
  - Nintendo Famicom Disk System
  - Nintendo Super Famicom / Super Nintendo Entertainment System
  - Nintendo Super Famicom / Super Nintendo Entertainment System SPC";
            Features = new Dictionary<string, Feature>();

            AddFeature(OutputDirStringInput);
        }

        public override void ProcessFeatures(Dictionary<string, Feature> features)
        {
            base.ProcessFeatures(features);

            // Get only files from the inputs
            List<ParentablePath> files = PathTool.GetFilesOnly(Inputs);
            foreach (ParentablePath file in files)
            {
                RestoreHeader(file.CurrentPath, OutputDir);
            }
        }
    
        /// <summary>
        /// Detect and replace header(s) to the given file
        /// </summary>
        /// <param name="file">Name of the file to be parsed</param>
        /// <param name="outDir">Output directory to write the file to, empty means the same directory as the input file</param>
        /// <returns>True if a header was found and appended, false otherwise</returns>
        public bool RestoreHeader(string file, string outDir)
        {
            // Create the output directory if it doesn't exist
            if (!string.IsNullOrWhiteSpace(outDir) && !Directory.Exists(outDir))
                Directory.CreateDirectory(outDir);

            // First, get the SHA-1 hash of the file
            BaseFile baseFile = BaseFile.GetInfo(file, hashes: Hash.SHA1, asFiles: TreatAsFile.NonArchive);

            // Retrieve a list of all related headers from the database
            List<string> headers = RetrieveHeadersFromDatabase(Utilities.ByteArrayToString(baseFile.SHA1));

            // If we have nothing retrieved, we return false
            if (headers.Count == 0)
                return false;

            // Now loop through and create the reheadered files, if possible
            for (int i = 0; i < headers.Count; i++)
            {
                string outputFile = (string.IsNullOrWhiteSpace(outDir) ? $"{Path.GetFullPath(file)}.new" : Path.Combine(outDir, Path.GetFileName(file))) + i;
                logger.User($"Creating reheadered file: {outputFile}");
                AppendBytes(file, outputFile, Utilities.StringToByteArray(headers[i]), null);
                logger.User("Reheadered file created!");
            }

            return true;
        }

        /// <summary>
        /// Retrieve headers from the database
        /// </summary>
        /// <param name="SHA1">SHA-1 of the deheadered file</param>
        /// <returns>List of strings representing the headers to add</returns>
        private List<string> RetrieveHeadersFromDatabase(string SHA1)
        {
            // Ensure the database exists
            EnsureDatabase();

            // Open the database connection
            SqliteConnection dbc = new SqliteConnection(HeadererConnectionString);
            dbc.Open();

            // Create the output list of headers
            List<string> headers = new List<string>();

            string query = $"SELECT header, type FROM data WHERE sha1='{SHA1}'";
            SqliteCommand slc = new SqliteCommand(query, dbc);
            SqliteDataReader sldr = slc.ExecuteReader();

            if (sldr.HasRows)
            {
                while (sldr.Read())
                {
                    logger.Verbose($"Found match with rom type '{sldr.GetString(1)}'");
                    headers.Add(sldr.GetString(0));
                }
            }
            else
            {
                logger.Warning("No matching header could be found!");
            }

            // Dispose of database objects
            slc.Dispose();
            sldr.Dispose();
            dbc.Dispose();

            return headers;
        }
    
        /// <summary>
        /// Add an aribtrary number of bytes to the inputted file
        /// </summary>
        /// <param name="input">File to be appended to</param>
        /// <param name="output">Outputted file</param>
        /// <param name="bytesToAddToHead">Bytes to be added to head of file</param>
        /// <param name="bytesToAddToTail">Bytes to be added to tail of file</param>
        private void AppendBytes(string input, string output, byte[] bytesToAddToHead, byte[] bytesToAddToTail)
        {
            // If any of the inputs are invalid, skip
            if (!File.Exists(input))
                return;

#if NET_FRAMEWORK
            using (FileStream fsr = File.OpenRead(input))
            using (FileStream fsw = File.OpenWrite(output))
            {
#else
            using FileStream fsr = File.OpenRead(input);
            using FileStream fsw = File.OpenWrite(output);
#endif
                AppendBytes(fsr, fsw, bytesToAddToHead, bytesToAddToTail);
#if NET_FRAMEWORK
            }
#endif
        }

        /// <summary>
        /// Add an aribtrary number of bytes to the inputted stream
        /// </summary>
        /// <param name="input">Stream to be appended to</param>
        /// <param name="output">Outputted stream</param>
        /// <param name="bytesToAddToHead">Bytes to be added to head of stream</param>
        /// <param name="bytesToAddToTail">Bytes to be added to tail of stream</param>
        private void AppendBytes(Stream input, Stream output, byte[] bytesToAddToHead, byte[] bytesToAddToTail)
        {
            // Write out prepended bytes
            if (bytesToAddToHead != null && bytesToAddToHead.Length > 0)
                output.Write(bytesToAddToHead, 0, bytesToAddToHead.Length);

            // Now copy the existing file over
            input.CopyTo(output);

            // Write out appended bytes
            if (bytesToAddToTail != null && bytesToAddToTail.Length > 0)
                output.Write(bytesToAddToTail, 0, bytesToAddToTail.Length);
        }
    }
}
