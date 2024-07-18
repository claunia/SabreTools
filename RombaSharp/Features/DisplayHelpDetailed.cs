using SabreTools.Help;

namespace RombaSharp.Features
{
    internal class DisplayHelpDetailed : BaseFeature
    {
        public const string Value = "Help (Detailed)";

        public DisplayHelpDetailed()
        {
            Name = Value;
            Flags.AddRange(["-??", "-hd", "--help-detailed"]);
            Description = "Show this detailed help";
            _featureType = ParameterType.Flag;
            LongDescription = "Display a detailed help text to the screen.";
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
