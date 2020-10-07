using System;
using System.Collections.Generic;

using RombaSharp.Features;
using SabreTools.Library.Data;
using SabreTools.Library.Help;
using SabreTools.Library.Logging;

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
        private static Help _help;

        /// <summary>
        /// Entry class for the RombaSharp application
        /// </summary>
        public static void Main(string[] args)
        {
            // Perform initial setup and verification
            Globals.Logger = new Logger("romba.log")
            {
                AppendPrefix = true,
                LowestLogLevel = LogLevel.VERBOSE,
                ThrowOnError = false,
            };

            // Create a new Help object for this program
            _help = RetrieveHelp();

            // Get the location of the script tag, if it exists
            int scriptLocation = (new List<string>(args)).IndexOf("--script");

            // If output is being redirected or we are in script mode, don't allow clear screens
            if (!Console.IsOutputRedirected && scriptLocation == -1)
            {
                Console.Clear();
                Prepare.SetConsoleHeader("RombaSharp");
            }

            // Now we remove the script tag because it messes things up
            if (scriptLocation > -1)
            {
                List<string> newargs = new List<string>(args);
                newargs.RemoveAt(scriptLocation);
                args = newargs.ToArray();
            }

            // Credits take precidence over all
            if ((new List<string>(args)).Contains("--credits"))
            {
                _help.OutputCredits();
                Globals.Logger.Close();
                return;
            }

            // If there's no arguments, show help
            if (args.Length == 0)
            {
                _help.OutputGenericHelp();
                Globals.Logger.Close();
                return;
            }

            // Get the first argument as a feature flag
            string featureName = args[0];

            // Verify that the flag is valid
            if (!_help.TopLevelFlag(featureName))
            {
                Globals.Logger.User($"'{featureName}' is not valid feature flag");
                _help.OutputIndividualFeature(featureName);
                Globals.Logger.Close();
                return;
            }

            // Get the proper name for the feature
            featureName = _help.GetFeatureName(featureName);

            // Get the associated feature
            BaseFeature feature = _help[featureName] as BaseFeature;

            // If we had the help feature first
            if (featureName == DisplayHelp.Value || featureName == DisplayHelpDetailed.Value)
            {
                feature.ProcessArgs(args, _help);
                Globals.Logger.Close();
                return;
            }

            // Now verify that all other flags are valid
            if (!feature.ProcessArgs(args, _help))
            {
                Globals.Logger.Close();
                return;
            }

            // Now process the current feature
            Dictionary<string, Feature> features = _help.GetEnabledFeatures();
            switch (featureName)
            {
                case DisplayHelpDetailed.Value:
                case DisplayHelp.Value:
                case Script.Value:
                    // No-op as this should be caught
                    break;

                // Require input verification
                case Archive.Value:
                case Features.Build.Value:
                case DatStats.Value:
                case Fixdat.Value:
                case Import.Value:
                case Lookup.Value:
                case Merge.Value:
                case Miss.Value:
                case RescanDepots.Value:
                    VerifyInputs(feature.Inputs, featureName);
                    feature.ProcessFeatures(features);
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
                    feature.ProcessFeatures(features);
                    break;

                // If nothing is set, show the help
                default:
                    _help.OutputGenericHelp();
                    break;
            }

            Globals.Logger.Close();
            return;
        }

        /// <summary>
        /// Generate a Help object for this program
        /// </summary>
        /// <returns>Populated Help object</returns>
        private static Help RetrieveHelp()
        {
            // Create and add the header to the Help object
            string barrier = "-----------------------------------------";
            List<string> helpHeader = new List<string>()
            {
                "RombaSharp - C# port of the Romba rom management tool",
                barrier,
                "Usage: RombaSharp [option] [filename|dirname] ...",
                string.Empty
            };

            // Create the base help object with header
            Help help = new Help(helpHeader);

            // Add all of the features
            help.Add(new DisplayHelp());
            help.Add(new DisplayHelpDetailed());
            help.Add(new Script());
            help.Add(new Archive());
            help.Add(new Features.Build());
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
                Globals.Logger.Error("This feature requires at least one input");
                _help.OutputIndividualFeature(feature);
                Environment.Exit(0);
            }
        }
    }
}
