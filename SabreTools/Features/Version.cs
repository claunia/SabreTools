using System.Collections.Generic;
using SabreTools.Core;
using SabreTools.Help;

namespace SabreTools.Features
{
    internal class Version : BaseFeature
    {
        public const string Value = "Version";

        public Version()
        {
            Name = Value;
            Flags = ["v", "version"];
            Description = "Prints version";
            _featureType = ParameterType.Flag;
            LongDescription = "Prints current program version.";
            Features = [];

            // Common Features
            AddCommonFeatures();
        }

        public override bool ProcessFeatures(Dictionary<string, Feature?> features)
        {
            // If the base fails, just fail out
            if (!base.ProcessFeatures(features))
                return false;

            logger.User($"SabreTools version: {Prepare.Version}");
            return true;
        }
    }
}
