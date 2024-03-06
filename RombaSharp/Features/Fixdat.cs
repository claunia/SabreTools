using System.Collections.Generic;
using SabreTools.Help;

namespace RombaSharp.Features
{
    internal class Fixdat : BaseFeature
    {
        public const string Value = "Fixdat";

        public Fixdat()
        {
            Name = Value;
            Flags = ["fixdat"];
            Description = "For each specified DAT file it creates a fix DAT.";
            _featureType = ParameterType.Flag;
            LongDescription = @"For each specified DAT file it creates a fix DAT with the missing entries for that DAT. If nothing is missing it doesn't create a fix DAT for that particular DAT.";
            Features = [];

            // Common Features
            AddCommonFeatures();

            AddFeature(OutStringInput);
            AddFeature(FixdatOnlyFlag); // Enabled by default
            AddFeature(WorkersInt32Input);
            AddFeature(SubworkersInt32Input);
        }

        public override bool ProcessFeatures(Dictionary<string, Feature?> features)
        {
            // If the base fails, just fail out
            if (!base.ProcessFeatures(features))
                return false;

            // Get feature flags
            // Inputs
            bool fixdatOnly = GetBoolean(features, FixdatOnlyValue);
            int subworkers = GetInt32(features, SubworkersInt32Value);
            int workers = GetInt32(features, WorkersInt32Value);
            string? outdat = GetString(features, OutStringValue);

            logger.Error("This feature is not yet implemented: fixdat");
            return true;
        }
    }
}
