using System.Collections.Generic;

using SabreTools.Library.Help;
using SabreTools.Library.IO;
using SabreTools.Library.Skippers;

namespace SabreTools.Features
{
    internal class Extract : BaseFeature
    {
        public const string Value = "Extract";

        public Extract()
        {
            Name = Value;
            Flags = new List<string>() { "-ex", "--extract" };
            Description = "Extract and remove copier headers";
            _featureType = FeatureType.Flag;
            LongDescription = @"This will detect, store, and remove copier headers from a file or folder of files. The headers are backed up and collated by the hash of the unheadered file. Files are then output without the detected copier header alongside the originals with the suffix .new. No input files are altered in the process. Only uncompressed files will be processed.

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
            AddFeature(NoStoreHeaderFlag);
        }

        public override void ProcessFeatures(Dictionary<string, Feature> features)
        {
            base.ProcessFeatures(features);

            // Get feature flags
            bool nostore = GetBoolean(features, NoStoreHeaderValue);

            // Get only files from the inputs
            List<ParentablePath> files = DirectoryExtensions.GetFilesOnly(Inputs);
            foreach (ParentablePath file in files)
            {
                Transform.DetectTransformStore(file.CurrentPath, OutputDir, nostore);
            }
        }
    }
}
