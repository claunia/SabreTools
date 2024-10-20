using System.Collections.Generic;
using System.IO;
using System.Linq;
using SabreTools.Help;
using SabreTools.IO;

namespace RombaSharp.Features
{
    internal class Merge : BaseFeature
    {
        public const string Value = "Merge";

        public Merge()
        {
            Name = Value;
            Flags.AddRange(["merge"]);
            Description = "Merges depot";
            _featureType = ParameterType.Flag;
            LongDescription = "Merges specified depot into current depot.";

            // Common Features
            AddCommonFeatures();

            AddFeature(OnlyNeededFlag);
            AddFeature(ResumeStringInput);
            AddFeature(WorkersInt32Input);
            AddFeature(SkipInitialScanFlag);
        }

        // TODO: Add way of specifying "current depot" since that's what Romba relies on
        public override bool ProcessFeatures(Dictionary<string, Feature?> features)
        {
            // If the base fails, just fail out
            if (!base.ProcessFeatures(features))
                return false;

            // Get feature flags
            bool onlyNeeded = GetBoolean(features, OnlyNeededValue);
            bool skipInitialscan = GetBoolean(features, SkipInitialScanValue);
            int workers = GetInt32(features, WorkersInt32Value);
            string? resume = GetString(features, ResumeStringValue);

            logger.Error("This feature is not yet implemented: merge");

            // Verify that the inputs are valid directories
            var dirs = PathTool.GetDirectoriesOnly(Inputs).Select(p => p.CurrentPath);
            Inputs.Clear();
            Inputs.AddRange(dirs);

            // Loop over all input directories
            foreach (string input in Inputs)
            {
#if NET20 || NET35
                List<string> depotFiles = [.. Directory.GetFiles(input, "*.gz")];
#else
                List<string> depotFiles = [.. Directory.EnumerateFiles(input, "*.gz", SearchOption.AllDirectories)];
#endif

                // If we are copying all that is possible but we want to scan first
                if (!onlyNeeded && !skipInitialscan)
                {

                }

                // If we are copying all that is possible but we don't care to scan first
                else if (!onlyNeeded && skipInitialscan)
                {

                }

                // If we are copying only what is needed but we want to scan first
                else if (onlyNeeded && !skipInitialscan)
                {

                }

                // If we are copying only what is needed but we don't care to scan first
                else if (onlyNeeded && skipInitialscan)
                {

                }
            }

            return true;
        }
    }
}
