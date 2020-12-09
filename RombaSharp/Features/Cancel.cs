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
            Flags = new List<string>() { "cancel" };
            Description = "Cancels current long-running job";
            _featureType = ParameterType.Flag;
            LongDescription = "Cancels current long-running job.";
            Features = new Dictionary<string, Feature>();
        }

        public override void ProcessFeatures(Dictionary<string, Feature> features)
        {
            base.ProcessFeatures(features);
            logger.User("This feature is not yet implemented: cancel");
        }
    }
}
