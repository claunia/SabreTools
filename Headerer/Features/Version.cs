using System.Collections.Generic;
using SabreTools.Core;
using SabreTools.Help;

namespace Headerer.Features
{
    internal class Version : BaseFeature
    {
        public const string Value = "Version";

        public Version()
        {
            Name = Value;
            Flags.AddRange(["v", "version"]);
            Description = "Prints version";
            _featureType = ParameterType.Flag;
            LongDescription = "Prints current program version.";

            // Common Features
            AddCommonFeatures();
        }

        public override bool ProcessFeatures(Dictionary<string, Feature?> features)
        {
            // If the base fails, just fail out
            if (!base.ProcessFeatures(features))
                return false;

            logger.User($"Headerer version: {Globals.Version}");
            return true;
        }
    }
}
