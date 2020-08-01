using System.Collections.Generic;

using SabreTools.Library.Data;
using SabreTools.Library.Help;
using SabreTools.Library.Skippers;
using SabreTools.Library.Tools;

namespace SabreTools.Features
{
    internal class Restore : BaseFeature
    {
        public const string Value = "Restore";

        public Restore()
        {
            Name = Value;
            Flags = new List<string>() { "-re", "--restore" };
            Description = "Restore header to file based on SHA-1";
            _featureType = FeatureType.Flag;
            LongDescription = @"This will make use of stored copier headers and reapply them to files if they match the included hash. More than one header can be applied to a file, so they will be output to new files, suffixed with .newX, where X is a number. No input files are altered in the process.

The following systems have headers that this program can work with:
  - Atari 7800
  - Atari Lynx
  - Commodore PSID Music
  - NEC PC - Engine / TurboGrafx 16
  - Nintendo Famicom / Nintendo Entertainment System
  - Nintendo Famicom Disk System
  - Nintendo Super Famicom / Super Nintendo Entertainment System
  - Nintendo Super Famicom / Super Nintendo Entertainment System SPC";
            Features = new Dictionary<string, Feature>();

            AddFeature(OutputDirStringInput);
        }

        public override void ProcessFeatures(Dictionary<string, Feature> features)
        {
            base.ProcessFeatures(features);

            // Get only files from the inputs
            List<ParentablePath> files = DirectoryExtensions.GetFilesOnly(Inputs);
            foreach (ParentablePath file in files)
            {
                Transform.RestoreHeader(file.CurrentPath, OutputDir);
            }
        }
    }
}
