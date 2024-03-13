using System.Collections.Generic;
using SabreTools.Core;
using SabreTools.Help;

namespace RombaSharp.Features
{
    internal class Version : BaseFeature
    {
        public const string Value = "Version";

        public Version()
        {
            Name = Value;
            Flags = ["version"];
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

            logger.User($"RombaSharp version: {Globals.Version}");
            return true;
        }
    }
}
