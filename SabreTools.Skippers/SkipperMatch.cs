using System.Collections.Generic;
using System.IO;

using SabreTools.Core;
using SabreTools.Logging;

namespace SabreTools.Skippers
{
    /// <summary>
    /// Class for matching existing Skippers
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
    public static class SkipperMatch
    {
        /// <summary>
        /// Header skippers represented by a list of skipper objects
        /// </summary>
        private static List<SkipperFile> List;

        /// <summary>
        /// Local paths
        /// </summary>
        private static readonly string LocalPath = Path.Combine(Globals.ExeDir, "Skippers") + Path.DirectorySeparatorChar;

        #region Logging

        /// <summary>
        /// Logging object
        /// </summary>
        private static readonly Logger logger = new Logger();

        #endregion

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
                logger.Error($"The file '{input}' does not exist so it cannot be tested");
                return new SkipperRule();
            }

            return GetMatchingRule(File.OpenRead(input), skipperName);
        }

        /// <summary>
        /// Get the SkipperRule associated with a given stream
        /// </summary>
        /// <param name="input">Name of the file to be checked</param>
        /// <param name="skipperName">Name of the skipper to be used, blank to find a matching skipper</param>
        /// <param name="keepOpen">True if the underlying stream should be kept open, false otherwise</param>
        /// <returns>The SkipperRule that matched the file</returns>
        public static SkipperRule GetMatchingRule(Stream input, string skipperName, bool keepOpen = false)
        {
            SkipperRule skipperRule = new SkipperRule();

            // If we have a null skipper name, we return since we're not matching skippers
            if (skipperName == null)
                return skipperRule;

            // Loop through and find a Skipper that has the right name
            logger.Verbose("Beginning search for matching header skip rules");
            List<SkipperFile> tempList = new List<SkipperFile>();
            tempList.AddRange(List);

            // Loop through all known SkipperFiles
            foreach (SkipperFile skipper in tempList)
            {
                skipperRule = skipper.GetMatchingRule(input, skipperName);
                if (skipperRule != null)
                    break;
            }

            // If we're not keeping the stream open, dispose of the binary reader
            if (!keepOpen)
                input.Dispose();

            // If the SkipperRule is null, make it empty
            if (skipperRule == null)
                skipperRule = new SkipperRule();

            // If we have a blank rule, inform the user
            if (skipperRule.Tests == null)
                logger.Verbose("No matching rule found!");
            else
                logger.User("Matching rule found!");

            return skipperRule;
        }
    }
}