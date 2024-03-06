using System;
using System.Collections.Generic;
using System.IO;
using SabreTools.Core;
using SabreTools.Features;
using SabreTools.Help;
using SabreTools.IO;
using SabreTools.Logging;

namespace SabreTools
{
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
        /// Entry point for the SabreTools application
        /// </summary>
        /// <param name="args">String array representing command line parameters</param>
        public static void Main(string[] args)
        {
            // Perform initial setup and verification
            LoggerImpl.SetFilename(Path.Combine(PathTool.GetRuntimeDirectory(), "logs", "sabretools.log"), true);
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

            // TODO: Remove this block once trimming is no longer needed
            // TODO: Update wiki documentation ONLY after this reaches stable
            // TODO: Re-evaluate feature flags with this change in mind
            featureName = featureName.TrimStart('-');
            if (args[0].StartsWith("-"))
                logger.User($"Feature flags no longer require leading '-' characters");

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
            BaseFeature feature = (_help[featureName] as BaseFeature)!;

            // If we had the help feature first
            if (featureName == DisplayHelp.Value || featureName == DisplayHelpDetailed.Value)
            {
                feature.ProcessArgs(args, _help);
                LoggerImpl.Close();
                return;
            }

            // Now verify that all other flags are valid
            if (!feature.ProcessArgs(args, _help))
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
                Prepare.SetConsoleHeader("SabreTools");
            }

            // Now process the current feature
            Dictionary<string, Feature?> features = _help.GetEnabledFeatures();
            bool success = false;
            switch (featureName)
            {
                // No-op as these should be caught
                case DisplayHelp.Value:
                case DisplayHelpDetailed.Value:
                    break;

                // Require input verification
                case Batch.Value:
                case DatFromDir.Value:
                case Extract.Value:
                case Restore.Value:
                case Split.Value:
                case Stats.Value:
                case Update.Value:
                case Verify.Value:
                    VerifyInputs(feature.Inputs, feature);
                    success = feature.ProcessFeatures(features);
                    break;

                // Requires no input verification
                case Sort.Value:
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
            List<string> helpHeader =
            [
                "SabreTools - Manipulate, convert, and use DAT files",
                barrier,
                "Usage: SabreTools [option] [flags] [filename|dirname] ...",
                string.Empty
            ];

            // Create the base help object with header
            var help = new FeatureSet(helpHeader);

            // Add all of the features
            help.Add(new DisplayHelp());
            help.Add(new DisplayHelpDetailed());
            help.Add(new Batch());
            help.Add(new DatFromDir());
            help.Add(new Extract());
            help.Add(new Restore());
            help.Add(new Sort());
            help.Add(new Split());
            help.Add(new Stats());
            help.Add(new Update());
            help.Add(new Verify());

            return help;
        }

        /// <summary>
        /// Verify that there are inputs, show help otherwise
        /// </summary>
        /// <param name="inputs">List of inputs</param>
        /// <param name="feature">Name of the current feature</param>
        private static void VerifyInputs(List<string> inputs, BaseFeature feature)
        {
            if (inputs.Count == 0)
            {
                logger.Error("This feature requires at least one input");
                _help?.OutputIndividualFeature(feature.Name);
                Environment.Exit(0);
            }
        }
    }
}
