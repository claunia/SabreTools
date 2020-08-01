using System.Collections.Generic;

using SabreTools.Library.Data;
using SabreTools.Library.Help;

namespace RombaSharp.Features
{
    internal class DisplayHelp : BaseFeature
    {
        public const string Value = "Help";

        public DisplayHelp()
        {
            Name = Value;
            Flags = new List<string>() { "-?", "-h", "--help" };
            Description = "Show this help";
            _featureType = FeatureType.Flag;
            LongDescription = "Built-in to most of the programs is a basic help text.";
            Features = new Dictionary<string, Feature>();
        }

        public override bool ProcessArgs(string[] args, Help help)
        {
            // If we had something else after help
            if (args.Length > 1)
            {
                help.OutputIndividualFeature(args[1]);
                return true;
            }

            // Otherwise, show generic help
            else
            {
                help.OutputGenericHelp();
                return true;
            }
        }
    }
}
