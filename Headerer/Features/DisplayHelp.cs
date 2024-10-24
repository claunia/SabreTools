using SabreTools.Help;

namespace Headerer.Features
{
    internal class DisplayHelp : BaseFeature
    {
        public const string Value = "Help";

        public DisplayHelp()
        {
            Name = Value;
            Flags.AddRange(["?", "h", "help"]);
            Description = "Show this help";
            _featureType = ParameterType.Flag;
            LongDescription = "Built-in to most of the programs is a basic help text.";
        }

        public override bool ProcessArgs(string[] args, FeatureSet help)
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
