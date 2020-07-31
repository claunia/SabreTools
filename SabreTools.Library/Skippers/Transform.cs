using System;
using System.Collections.Generic;
using System.IO;

using SabreTools.Library.Data;
using SabreTools.Library.FileTypes;
using SabreTools.Library.Tools;

namespace SabreTools.Library.Skippers
{
    /// <summary>
    /// Class for wrapping general file transformations
    /// <summary>
    /// <remarks>
    /// Skippers, in general, are distributed as XML files by some projects
    /// in order to denote a way of transforming a file so that it will match
    /// the hashes included in their DATs. Each skipper file can contain multiple
    /// skipper rules, each of which denote a type of header/transformation. In
    /// turn, each of those rules can contain multiple tests that denote that
    /// a file should be processed using that rule. Transformations can include
    /// simply skipping over a portion of the file all the way to byteswapping
    /// the entire file. For the purposes of this library, Skippers also denote
    /// a way of changing files directly in order to produce a file whose external
    /// hash would match those same DATs.
    /// </remarks>
    public static class Transform
    {
        /// <summary>
        /// Header skippers represented by a list of skipper objects
        /// </summary>
        private static List<SkipperFile> List;

        /// <summary>
        /// Local paths
        /// </summary>
        public static string LocalPath = Path.Combine(Globals.ExeDir, "Skippers") + Path.DirectorySeparatorChar;

        /// <summary>
        /// Initialize static fields
        /// </summary>
        public static void Init()
        {
            PopulateSkippers();
        }

        /// <summary>
        /// Populate the entire list of header Skippers
        /// </summary>
        /// <remarks>
        /// http://mamedev.emulab.it/clrmamepro/docs/xmlheaders.txt
        /// http://www.emulab.it/forum/index.php?topic=127.0
        /// </remarks>
        private static void PopulateSkippers()
        {
            if (List == null)
                List = new List<SkipperFile>();

            foreach (string skipperFile in Directory.EnumerateFiles(LocalPath, "*", SearchOption.AllDirectories))
            {
                List.Add(new SkipperFile(Path.GetFullPath(skipperFile)));
            }
        }

        /// <summary>
        /// Detect header skipper compliance and create an output file
        /// </summary>
        /// <param name="file">Name of the file to be parsed</param>
        /// <param name="outDir">Output directory to write the file to, empty means the same directory as the input file</param>
        /// <param name="nostore">True if headers should not be stored in the database, false otherwise</param>
        /// <returns>True if the output file was created, false otherwise</returns>
        public static bool DetectTransformStore(string file, string outDir, bool nostore)
        {
            // Create the output directory if it doesn't exist
            DirectoryExtensions.Ensure(outDir, create: true);

            Globals.Logger.User($"\nGetting skipper information for '{file}'");

            // Get the skipper rule that matches the file, if any
            SkipperRule rule = GetMatchingRule(file, string.Empty);

            // If we have an empty rule, return false
            if (rule.Tests == null || rule.Tests.Count == 0 || rule.Operation != HeaderSkipOperation.None)
                return false;

            Globals.Logger.User("File has a valid copier header");

            // Get the header bytes from the file first
            string hstr;
            try
            {
                // Extract the header as a string for the database
#if NET_FRAMEWORK
                using (var fs = FileExtensions.TryOpenRead(file))
                {
#else
                using var fs = FileExtensions.TryOpenRead(file);
#endif
                byte[] hbin = new byte[(int)rule.StartOffset];
                fs.Read(hbin, 0, (int)rule.StartOffset);
                hstr = Utilities.ByteArrayToString(hbin);
#if NET_FRAMEWORK
                }
#endif
            }
            catch
            {
                return false;
            }

            // Apply the rule to the file
            string newfile = (string.IsNullOrWhiteSpace(outDir) ? Path.GetFullPath(file) + ".new" : Path.Combine(outDir, Path.GetFileName(file)));
            rule.TransformFile(file, newfile);

            // If the output file doesn't exist, return false
            if (!File.Exists(newfile))
                return false;

            // Now add the information to the database if it's not already there
            if (!nostore)
            {
                BaseFile baseFile = FileExtensions.GetInfo(newfile, chdsAsFiles: true);
                DatabaseTools.AddHeaderToDatabase(hstr, Utilities.ByteArrayToString(baseFile.SHA1), rule.SourceFile);
            }

            return true;
        }

        /// <summary>
        /// Detect and replace header(s) to the given file
        /// </summary>
        /// <param name="file">Name of the file to be parsed</param>
        /// <param name="outDir">Output directory to write the file to, empty means the same directory as the input file</param>
        /// <returns>True if a header was found and appended, false otherwise</returns>
        public static bool RestoreHeader(string file, string outDir)
        {
            // Create the output directory if it doesn't exist
            if (!string.IsNullOrWhiteSpace(outDir) && !Directory.Exists(outDir))
                Directory.CreateDirectory(outDir);

            // First, get the SHA-1 hash of the file
            BaseFile baseFile = FileExtensions.GetInfo(file, chdsAsFiles: true);

            // Retrieve a list of all related headers from the database
            List<string> headers = DatabaseTools.RetrieveHeadersFromDatabase(Utilities.ByteArrayToString(baseFile.SHA1));

            // If we have nothing retrieved, we return false
            if (headers.Count == 0)
                return false;

            // Now loop through and create the reheadered files, if possible
            for (int i = 0; i < headers.Count; i++)
            {
                string outputFile = (string.IsNullOrWhiteSpace(outDir) ? $"{Path.GetFullPath(file)}.new" : Path.Combine(outDir, Path.GetFileName(file))) + i;
                Globals.Logger.User($"Creating reheadered file: {outputFile}");
                FileExtensions.AppendBytes(file, outputFile, Utilities.StringToByteArray(headers[i]), null);
                Globals.Logger.User("Reheadered file created!");
            }

            return true;
        }

        /// <summary>
        /// Get the SkipperRule associated with a given file
        /// </summary>
        /// <param name="input">Name of the file to be checked</param>
        /// <param name="skipperName">Name of the skipper to be used, blank to find a matching skipper</param>
        /// <param name="logger">Logger object for file and console output</param>
        /// <returns>The SkipperRule that matched the file</returns>
        public static SkipperRule GetMatchingRule(string input, string skipperName)
        {
            // If the file doesn't exist, return a blank skipper rule
            if (!File.Exists(input))
            {
                Globals.Logger.Error($"The file '{input}' does not exist so it cannot be tested");
                return new SkipperRule();
            }

            return GetMatchingRule(FileExtensions.TryOpenRead(input), skipperName);
        }

        /// <summary>
        /// Get the SkipperRule associated with a given stream
        /// </summary>
        /// <param name="input">Name of the file to be checked</param>
        /// <param name="skipperName">Name of the skipper to be used, blank to find a matching skipper</param>
        /// <param name="logger">Logger object for file and console output</param>
        /// <param name="keepOpen">True if the underlying stream should be kept open, false otherwise</param>
        /// <returns>The SkipperRule that matched the file</returns>
        public static SkipperRule GetMatchingRule(Stream input, string skipperName, bool keepOpen = false)
        {
            SkipperRule skipperRule = new SkipperRule();

            // If we have a null skipper name, we return since we're not matching skippers
            if (skipperName == null)
                return skipperRule;

            // Loop through and find a Skipper that has the right name
            Globals.Logger.Verbose("Beginning search for matching header skip rules");
            List<SkipperFile> tempList = new List<SkipperFile>();
            tempList.AddRange(List);

            foreach (SkipperFile skipper in tempList)
            {
                // If we're searching for the skipper OR we have a match to an inputted one
                if (string.IsNullOrWhiteSpace(skipperName)
                    || (!string.IsNullOrWhiteSpace(skipper.Name) && skipperName.ToLowerInvariant() == skipper.Name.ToLowerInvariant())
                    || (!string.IsNullOrWhiteSpace(skipper.Name) && skipperName.ToLowerInvariant() == skipper.SourceFile.ToLowerInvariant()))
                {
                    // Loop through the rules until one is found that works
                    BinaryReader br = new BinaryReader(input);

                    foreach (SkipperRule rule in skipper.Rules)
                    {
                        // Always reset the stream back to the original place
                        input.Seek(0, SeekOrigin.Begin);

                        // For each rule, make sure it passes each test
                        bool success = true;
                        foreach (SkipperTest test in rule.Tests)
                        {
                            bool result = true;
                            switch (test.Type)
                            {
                                case HeaderSkipTest.Data:
                                    // First seek to the correct position
                                    if (test.Offset == null)
                                        input.Seek(0, SeekOrigin.End);
                                    else if (test.Offset > 0 && test.Offset <= input.Length)
                                        input.Seek((long)test.Offset, SeekOrigin.Begin);
                                    else if (test.Offset < 0 && Math.Abs((long)test.Offset) <= input.Length)
                                        input.Seek((long)test.Offset, SeekOrigin.End);

                                    // Then read and compare bytewise
                                    result = true;
                                    for (int i = 0; i < test.Value.Length; i++)
                                    {
                                        try
                                        {
                                            if (br.ReadByte() != test.Value[i])
                                            {
                                                result = false;
                                                break;
                                            }
                                        }
                                        catch
                                        {
                                            result = false;
                                            break;
                                        }
                                    }

                                    // Return if the expected and actual results match
                                    success &= (result == test.Result);
                                    break;

                                case HeaderSkipTest.Or:
                                case HeaderSkipTest.Xor:
                                case HeaderSkipTest.And:
                                    // First seek to the correct position
                                    if (test.Offset == null)
                                        input.Seek(0, SeekOrigin.End);
                                    else if (test.Offset > 0 && test.Offset <= input.Length)
                                        input.Seek((long)test.Offset, SeekOrigin.Begin);
                                    else if (test.Offset < 0 && Math.Abs((long)test.Offset) <= input.Length)
                                        input.Seek((long)test.Offset, SeekOrigin.End);

                                    result = true;
                                    try
                                    {
                                        // Then apply the mask if it exists
                                        byte[] read = br.ReadBytes(test.Mask.Length);
                                        byte[] masked = new byte[test.Mask.Length];
                                        for (int i = 0; i < read.Length; i++)
                                        {
                                            masked[i] = (byte)(test.Type == HeaderSkipTest.And ? read[i] & test.Mask[i] :
                                                (test.Type == HeaderSkipTest.Or ? read[i] | test.Mask[i] : read[i] ^ test.Mask[i])
                                            );
                                        }

                                        // Finally, compare it against the value
                                        for (int i = 0; i < test.Value.Length; i++)
                                        {
                                            if (masked[i] != test.Value[i])
                                            {
                                                result = false;
                                                break;
                                            }
                                        }
                                    }
                                    catch
                                    {
                                        result = false;
                                    }

                                    // Return if the expected and actual results match
                                    success &= (result == test.Result);
                                    break;

                                case HeaderSkipTest.File:
                                    // First get the file size from stream
                                    long size = input.Length;

                                    // If we have a null size, check that the size is a power of 2
                                    result = true;
                                    if (test.Size == null)
                                    {
                                        // http://stackoverflow.com/questions/600293/how-to-check-if-a-number-is-a-power-of-2
                                        result = (((ulong)size & ((ulong)size - 1)) == 0);
                                    }
                                    else if (test.Operator == HeaderSkipTestFileOperator.Less)
                                    {
                                        result = (size < test.Size);
                                    }
                                    else if (test.Operator == HeaderSkipTestFileOperator.Greater)
                                    {
                                        result = (size > test.Size);
                                    }
                                    else if (test.Operator == HeaderSkipTestFileOperator.Equal)
                                    {
                                        result = (size == test.Size);
                                    }

                                    // Return if the expected and actual results match
                                    success &= (result == test.Result);
                                    break;
                            }
                        }

                        // If we still have a success, then return this rule
                        if (success)
                        {
                            // If we're not keeping the stream open, dispose of the binary reader
                            if (!keepOpen)
                                input.Dispose();

                            Globals.Logger.User(" Matching rule found!");
                            return rule;
                        }
                    }
                }
            }

            // If we're not keeping the stream open, dispose of the binary reader
            if (!keepOpen)
                input.Dispose();

            // If we have a blank rule, inform the user
            if (skipperRule.Tests == null)
                Globals.Logger.Verbose("No matching rule found!");

            return skipperRule;
        }
    }
}