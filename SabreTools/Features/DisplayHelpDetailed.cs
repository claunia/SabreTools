using System.Collections.Generic;

using SabreTools.Help;

namespace SabreTools.Features
{
    internal class DisplayHelpDetailed : BaseFeature
    {
        public const string Value = "Help (Detailed)";

        public DisplayHelpDetailed()
        {
            Name = Value;
            Flags = new List<string>() { "??", "hd", "help-detailed" };
            Description = "Show this detailed help";
            _featureType = ParameterType.Flag;
            LongDescription = "Display a detailed help text to the screen.";
            Features = new Dictionary<string, Feature>();
        }

        public override bool ProcessArgs(string[] args, FeatureSet help)
        {
            // If we had something else after help
            if (args.Length > 1)
            {
                help.OutputIndividualFeature(args[1], includeLongDescription: true);
                return true;
            }

            // Otherwise, show generic help
            else
            {
                help.OutputAllHelp();
                return true;
            }
        }
    }
}
