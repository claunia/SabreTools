using System.Collections.Generic;
using System.IO;

using SabreTools.Core;
using SabreTools.DatFiles;
using SabreTools.Help;
using SabreTools.IO;

namespace RombaSharp.Features
{
    internal class Miss : BaseFeature
    {
        public const string Value = "Miss";

        // Unique to RombaSharp
        public Miss()
        {
            Name = Value;
            Flags = new List<string>() { "miss" };
            Description = "Create miss and have file";
            _featureType = ParameterType.Flag;
            LongDescription = "For each specified DAT file, create miss and have file";
            Features = new Dictionary<string, Feature>();
        }

        public override void ProcessFeatures(Dictionary<string, Feature> features)
        {
            base.ProcessFeatures(features);

            // Verify the filenames
            Dictionary<string, string> foundDats = GetValidDats(Inputs);

            // Create the new output directory if it doesn't exist
            DirectoryExtensions.Ensure(Path.Combine(Globals.ExeDir, "out"), create: true);

            // Now that we have the dictionary, we can loop through and output to a new folder for each
            foreach (string key in foundDats.Keys)
            {
                // Get the DAT file associated with the key
                DatFile datFile = DatFile.CreateAndParse(Path.Combine(_dats, foundDats[key]));

                // Now loop through and see if all of the hash combinations exist in the database
                /* ended here */
            }

            logger.Error("This feature is not yet implemented: miss");
        }
    }
}
