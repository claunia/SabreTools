using System.Collections.Generic;

using SabreTools.Help;

namespace RombaSharp.Features
{
    internal class Progress : BaseFeature
    {
        public const string Value = "Progress";

        public Progress()
        {
            Name = Value;
            Flags = new List<string>() { "progress" };
            Description = "Shows progress of the currently running command.";
            _featureType = ParameterType.Flag;
            LongDescription = "Shows progress of the currently running command.";
            Features = new Dictionary<string, Feature>();

            // Common Features
            AddCommonFeatures();
        }

        public override bool ProcessFeatures(Dictionary<string, Feature> features)
        {
            // If the base fails, just fail out
            if (!base.ProcessFeatures(features))
                return false;

            logger.User("This feature is not yet implemented: progress");
            return true;
        }
    }
}
