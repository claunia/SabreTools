using System.Collections.Generic;
using SabreTools.Help;

namespace RombaSharp.Features
{
    internal class Cancel : BaseFeature
    {
        public const string Value = "Cancel";

        public Cancel()
        {
            Name = Value;
            Flags = ["cancel"];
            Description = "Cancels current long-running job";
            _featureType = ParameterType.Flag;
            LongDescription = "Cancels current long-running job.";
            Features = [];

            // Common Features
            AddCommonFeatures();
        }

        public override bool ProcessFeatures(Dictionary<string, Feature?> features)
        {
            // If the base fails, just fail out
            if (!base.ProcessFeatures(features))
                return false;

            logger.User("This feature is not yet implemented: cancel");
            return true;
        }
    }
}
