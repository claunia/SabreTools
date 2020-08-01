using System.Collections.Generic;

using SabreTools.Library.Data;
using SabreTools.Library.Help;

namespace RombaSharp.Features
{
    internal class Shutdown : BaseFeature
    {
        public const string Value = "Shutdown";

        public Shutdown()
        {
            Name = Value;
            Flags = new List<string>() { "shutdown" };
            Description = "Gracefully shuts down server.";
            _featureType = FeatureType.Flag;
            LongDescription = "Gracefully shuts down server saving all the cached data.";
            Features = new Dictionary<string, Feature>();
        }

        public override void ProcessFeatures(Dictionary<string, Feature> features)
        {
            base.ProcessFeatures(features);
            Globals.Logger.User("This feature is not yet implemented: shutdown");
        }
    }
}
