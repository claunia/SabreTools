using System.Collections.Generic;

using SabreTools.Help;

namespace SabreTools.Features
{
    // TODO: With the introduction of the `--log-level` input, can we create a better way
    // to handle "universal" flags? Having script as its own feature is not ideal.
    internal class Script : BaseFeature
    {
        public const string Value = "Script";

        public Script()
        {
            Name = Value;
            Flags = new List<string>() { "--script" };
            Description = "Enable script mode (no clear screen)";
            _featureType = ParameterType.Flag;
            LongDescription = "For times when SabreTools is being used in a scripted environment, the user may not want the screen to be cleared every time that it is called. This flag allows the user to skip clearing the screen on run just like if the console was being redirected.";
            Features = new Dictionary<string, Feature>();
        }
    }
}
