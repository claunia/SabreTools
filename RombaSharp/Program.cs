using System;
using System.Collections.Generic;
using System.IO;
using RombaSharp.Features;
using SabreTools.Core;
using SabreTools.Help;
using SabreTools.IO;
using SabreTools.IO.Logging;

namespace RombaSharp
{
    /// <summary>
    /// Entry class for the RombaSharp application
    /// </summary>
    /// <remarks>
    /// In the database, we want to enable "offline mode". That is, when a user does an operation
    /// that needs to read from the depot themselves, if the depot folder cannot be found, the
    /// user is prompted to reconnect the depot OR skip that depot entirely.
    /// </remarks>
    public class Program
    {
        #region Static Variables

        /// <summary>
        /// Help object that determines available functionality
        /// </summary>
        private static FeatureSet? _help;

        /// <summary>
        /// Logging object
        /// </summary>
        private static readonly Logger logger = new();

        #endregion

        /// <summary>
        /// Entry class for the RombaSharp application
        /// </summary>
        public static void Main(string[] args)
        {
            // Perform initial setup and verification
            LoggerImpl.SetFilename(Path.Combine(PathTool.GetRuntimeDirectory(), "logs", "romba.log"), true);
            LoggerImpl.AppendPrefix = true;
            LoggerImpl.LowestLogLevel = LogLevel.VERBOSE;
            LoggerImpl.ThrowOnError = false;
            LoggerImpl.Start();

            // Create a new Help object for this program
            _help = RetrieveHelp();

            // Credits take precidence over all
            if (new List<string>(args).Contains("--credits"))
            {
                FeatureSet.OutputCredits();
                LoggerImpl.Close();
                return;
            }

            // If there's no arguments, show help
            if (args.Length == 0)
            {
                _help.OutputGenericHelp();
                LoggerImpl.Close();
                return;
            }

            // Get the first argument as a feature flag
            string featureName = args[0];

            // Verify that the flag is valid
            if (!_help.TopLevelFlag(featureName))
            {
                logger.User($"'{featureName}' is not valid feature flag");
                _help.OutputIndividualFeature(featureName);
                LoggerImpl.Close();
                return;
            }

            // Get the proper name for the feature
            featureName = _help.GetFeatureName(featureName);

            // Get the associated feature
            BaseFeature? feature = _help[featureName] as BaseFeature;

            // If we had the help feature first
            if (featureName == DisplayHelp.Value || featureName == DisplayHelpDetailed.Value)
            {
                feature!.ProcessArgs(args, _help);
                LoggerImpl.Close();
                return;
            }

            // Now verify that all other flags are valid
            if (!feature!.ProcessArgs(args, _help))
            {
                LoggerImpl.Close();
                return;
            }

            // Set the new log level based on settings
            LoggerImpl.LowestLogLevel = feature.LogLevel;

            // If output is being redirected or we are in script mode, don't allow clear screens
            if (!Console.IsOutputRedirected && feature.ScriptMode)
            {
                Console.Clear();
                Globals.SetConsoleHeader("RombaSharp [Deprecated]");
            }

            // Now process the current feature
            Dictionary<string, Feature?> features = _help.GetEnabledFeatures();
            bool success = false;
            switch (featureName)
            {
                case DisplayHelpDetailed.Value:
                case DisplayHelp.Value:
                    // No-op as this should be caught
                    break;

                // Require input verification
                case Archive.Value:
                case Build.Value:
                case DatStats.Value:
                case Fixdat.Value:
                case Import.Value:
                case Lookup.Value:
                case Merge.Value:
                case Miss.Value:
                case RescanDepots.Value:
                    VerifyInputs(feature.Inputs, featureName);
                    success = feature.ProcessFeatures(features);
                    break;

                // Requires no input verification
                case Cancel.Value:
                case DbStats.Value:
                case Diffdat.Value:
                case Dir2Dat.Value:
                case EDiffdat.Value:
                case Export.Value:
                case Memstats.Value:
                case Progress.Value:
                case PurgeBackup.Value:
                case PurgeDelete.Value:
                case RefreshDats.Value:
                case Shutdown.Value:
                case Features.Version.Value:
                    success = feature.ProcessFeatures(features);
                    break;

                // If nothing is set, show the help
                default:
                    _help.OutputGenericHelp();
                    break;
            }

            // If the feature failed, output help
            if (!success)
            {
                logger.Error("An error occurred during processing!");
                _help.OutputIndividualFeature(featureName);
            }

            LoggerImpl.Close();
            return;
        }

        /// <summary>
        /// Generate a Help object for this program
        /// </summary>
        /// <returns>Populated Help object</returns>
        private static FeatureSet RetrieveHelp()
        {
            // Create and add the header to the Help object
            string barrier = "-----------------------------------------";
            List<string> helpHeader = new()
            {
                "RombaSharp - C# port of the Romba rom management tool",
                barrier,
                "Usage: RombaSharp [option] [filename|dirname] ...",
                string.Empty
            };

            // Create the base help object with header
            FeatureSet help = new(helpHeader);

            // Add all of the features
            help.Add(new DisplayHelp());
            help.Add(new DisplayHelpDetailed());
            help.Add(new Archive());
            help.Add(new Build());
            help.Add(new Cancel());
            help.Add(new DatStats());
            help.Add(new DbStats());
            help.Add(new Diffdat());
            help.Add(new Dir2Dat());
            help.Add(new EDiffdat());
            help.Add(new Export());
            help.Add(new Fixdat());
            help.Add(new Import());
            help.Add(new Lookup());
            help.Add(new Memstats());
            help.Add(new Merge());
            help.Add(new Miss());
            help.Add(new PurgeBackup());
            help.Add(new PurgeDelete());
            help.Add(new RefreshDats());
            help.Add(new RescanDepots());
            help.Add(new Progress());
            help.Add(new Shutdown());
            help.Add(new Features.Version());

            return help;
        }

        /// <summary>
        /// Verify that there are inputs, show help otherwise
        /// </summary>
        /// <param name="inputs">List of inputs</param>
        /// <param name="feature">Name of the current feature</param>
        private static void VerifyInputs(List<string> inputs, string feature)
        {
            if (inputs.Count == 0)
            {
                logger.Error("This feature requires at least one input");
                _help?.OutputIndividualFeature(feature);
                Environment.Exit(0);
            }
        }
    }
}
