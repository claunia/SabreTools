using System.Collections.Generic;

using SabreTools.Help;

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
            _featureType = ParameterType.Flag;
            LongDescription = "Print memory stats.";
            Features = new Dictionary<string, Feature>();

            // Common Features
            AddCommonFeatures();
        }

        public override bool ProcessFeatures(Dictionary<string, Feature> features)
        {
            // If the base fails, just fail out
            if (!base.ProcessFeatures(features))
                return false;

            logger.User("This feature is not yet implemented: memstats");
            return true;
        }
    }
}
