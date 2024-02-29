using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using SabreTools.IO;
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
        /// Header detectors represented by a list of detector objects
        /// </summary>
        private static List<Detector>? Skippers = null;

        /// <summary>
        /// Local paths
        /// </summary>
        private static readonly string LocalPath = Path.Combine(PathTool.GetRuntimeDirectory(), "Skippers") + Path.DirectorySeparatorChar;

        #region Logging

        /// <summary>
        /// Logging object
        /// </summary>
        private static readonly Logger logger = new();

        #endregion

        /// <summary>
        /// Initialize static fields
        /// </summary>
        /// <param name="experimental">True to enable internal header skipper generation, false to use file-based generation (default)</param>
        public static void Init(bool experimental = false)
        {
            // If the list is populated, don't add to it
            if (Skippers != null)
                return;

            // If we're using internal skipper generation
            if (experimental)
                PopulateSkippersInternal();

            // If we're using file-based skipper generation
            else
                PopulateSkippers();
        }

        /// <summary>
        /// Populate the entire list of header skippers from physical files
        /// </summary>
        /// <remarks>
        /// http://mamedev.emulab.it/clrmamepro/docs/xmlheaders.txt
        /// http://www.emulab.it/forum/index.php?topic=127.0
        /// </remarks>
        private static void PopulateSkippers()
        {
            // Ensure the list exists
            Skippers ??= [];

            // Create the XML serializer
            var xts = new XmlSerializer(typeof(Detector));

            // Get skippers for each known header type
#if NET20 || NET35
            foreach (string skipperPath in Directory.GetFiles(LocalPath, "*"))
#else
            foreach (string skipperPath in Directory.EnumerateFiles(LocalPath, "*", SearchOption.AllDirectories))
#endif
            {
                try
                {
                    // Create the XML reader
                    var xtr = XmlReader.Create(skipperPath, new XmlReaderSettings
                    {
                        CheckCharacters = false,
#if NET40_OR_GREATER
                        DtdProcessing = DtdProcessing.Ignore,
#endif
                        IgnoreComments = true,
                        IgnoreWhitespace = true,
                        ValidationFlags = XmlSchemaValidationFlags.None,
                        ValidationType = ValidationType.None,
                    });

                    // Deserialize the detector, if possible
                    if (xts.Deserialize(xtr) is not Detector detector || detector == null)
                        continue;

                    // Set the source file on the detector
                    string sourceFile = Path.GetFileNameWithoutExtension(skipperPath);
                    detector.SourceFile = sourceFile;

                    // Set the source file on the rules
                    if (detector.Rules != null)
                    {
                        for (int i = 0; i < detector.Rules.Length; i++)
                        {
                            if (detector.Rules[i] == null)
                                continue;

                            detector.Rules[i].SourceFile = sourceFile;
                        }
                    }

                    // Add the skipper to the set
                    Skippers.Add(detector);
                }
                catch { }
            }
        }

        /// <summary>
        /// Populate the entire list of header skippers from generated objects
        /// </summary>
        /// <remarks>
        /// http://mamedev.emulab.it/clrmamepro/docs/xmlheaders.txt
        /// http://www.emulab.it/forum/index.php?topic=127.0
        /// </remarks>
        private static void PopulateSkippersInternal()
        {
            // Ensure the list exists
            Skippers ??= [];

            // Get skippers for each known header type
            Skippers.Add(new Detectors.Atari7800());
            Skippers.Add(new Detectors.AtariLynx());
            Skippers.Add(new Detectors.CommodorePSID());
            Skippers.Add(new Detectors.NECPCEngine());
            Skippers.Add(new Detectors.Nintendo64());
            Skippers.Add(new Detectors.NintendoEntertainmentSystem());
            Skippers.Add(new Detectors.NintendoFamicomDiskSystem());
            Skippers.Add(new Detectors.SuperNintendoEntertainmentSystem());
            Skippers.Add(new Detectors.SuperFamicomSPC());
        }

        /// <summary>
        /// Get the Rule associated with a given file
        /// </summary>
        /// <param name="input">Name of the file to be checked</param>
        /// <param name="skipperName">Name of the skipper to be used, blank to find a matching skipper</param>
        /// <param name="logger">Logger object for file and console output</param>
        /// <returns>The Rule that matched the file</returns>
        public static Rule GetMatchingRule(string input, string skipperName)
        {
            // If the file doesn't exist, return a blank skipper rule
            if (!File.Exists(input))
            {
                logger.Error($"The file '{input}' does not exist so it cannot be tested");
                return new Rule();
            }

            return GetMatchingRule(File.OpenRead(input), skipperName);
        }

        /// <summary>
        /// Get the Rule associated with a given stream
        /// </summary>
        /// <param name="input">Name of the file to be checked</param>
        /// <param name="skipperName">Name of the skipper to be used, blank to find a matching skipper</param>
        /// <param name="keepOpen">True if the underlying stream should be kept open, false otherwise</param>
        /// <returns>The Rule that matched the file</returns>
        public static Rule GetMatchingRule(Stream? input, string skipperName, bool keepOpen = false)
        {
            var skipperRule = new Rule();

            // If we have an invalid input
            if (input == null || !input.CanRead)
                return skipperRule;

            // If we have an invalid set of skippers or skipper name
            if (Skippers == null || skipperName == null)
                return skipperRule;

            // Loop through and find a Skipper that has the right name
            logger.Verbose("Beginning search for matching header skip rules");

            // Loop through all known Detectors
            foreach (Detector? skipper in Skippers)
            {
                // This should not happen
                if (skipper == null)
                    continue;

                skipperRule = skipper.GetMatchingRule(input, skipperName);
                if (skipperRule != null)
                    break;
            }

            // If we're not keeping the stream open, dispose of the binary reader
            if (!keepOpen)
                input?.Dispose();

            // If the Rule is null, make it empty
            skipperRule ??= new Rule();

            // If we have a blank rule, inform the user
            if (skipperRule.Tests == null)
                logger.Verbose("No matching rule found!");
            else
                logger.User("Matching rule found!");

            return skipperRule;
        }
    }
}