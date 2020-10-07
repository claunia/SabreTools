using System.Collections.Generic;

using SabreTools.Library.Help;

namespace RombaSharp.Features
{
    internal class Memstats : BaseFeature
    {
        public const string Value = "Memstats";

        public Memstats()
        {
            Name = Value;
            Flags = new List<string>() { "memstats" };
            Description = "Prints memory stats.";
            _featureType = FeatureType.Flag;
            LongDescription = "Print memory stats.";
            Features = new Dictionary<string, Feature>();
        }

        public override void ProcessFeatures(Dictionary<string, Feature> features)
        {
            base.ProcessFeatures(features);
            logger.User("This feature is not yet implemented: memstats");
        }
    }
}
