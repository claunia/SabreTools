using System.Collections.Generic;

using SabreTools.Library.Data;
using SabreTools.Library.Help;

namespace RombaSharp.Features
{
    internal class Fixdat : BaseFeature
    {
        public const string Value = "Fixdat";

        public Fixdat()
        {
            Name = Value;
            Flags = new List<string>() { "fixdat" };
            Description = "For each specified DAT file it creates a fix DAT.";
            _featureType = FeatureType.Flag;
            LongDescription = @"For each specified DAT file it creates a fix DAT with the missing entries for that DAT. If nothing is missing it doesn't create a fix DAT for that particular DAT.";
            Features = new Dictionary<string, Feature>();

            AddFeature(OutStringInput);
            AddFeature(FixdatOnlyFlag); // Enabled by default
            AddFeature(WorkersInt32Input);
            AddFeature(SubworkersInt32Input);
        }

        public override void ProcessFeatures(Dictionary<string, Feature> features)
        {
            base.ProcessFeatures(features);

            // Get feature flags
            // Inputs
            bool fixdatOnly = GetBoolean(features, FixdatOnlyValue);
            int subworkers = GetInt32(features, SubworkersInt32Value);
            int workers = GetInt32(features, WorkersInt32Value);
            string outdat = GetString(features, OutStringValue);

            Globals.Logger.Error("This feature is not yet implemented: fixdat");
        }
    }
}
