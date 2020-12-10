using System.Collections.Generic;
using System.IO;
using System.Linq;

using SabreTools.Core;
using SabreTools.DatFiles;
using SabreTools.Help;
using SabreTools.IO;

namespace RombaSharp.Features
{
    internal class Build : BaseFeature
    {
        public const string Value = "Build";

        public Build()
        {
            Name = Value;
            Flags = new List<string>() { "build" };
            Description = "For each specified DAT file it creates the torrentzip files.";
            _featureType = ParameterType.Flag;
            LongDescription = @"For each specified DAT file it creates the torrentzip files in the specified
output dir. The files will be placed in the specified location using a folder
structure according to the original DAT master directory tree structure.";
            Features = new Dictionary<string, Feature>();

            AddFeature(OutStringInput);
            AddFeature(FixdatOnlyFlag);
            AddFeature(CopyFlag);
            AddFeature(WorkersInt32Input);
            AddFeature(SubworkersInt32Input);
        }

        public override void ProcessFeatures(Dictionary<string, Feature> features)
        {
            base.ProcessFeatures(features);

            // Get feature flags
            bool copy = GetBoolean(features, CopyValue);
            string outdat = GetString(features, OutStringValue);

            // Verify the filenames
            Dictionary<string, string> foundDats = GetValidDats(Inputs);

            // Ensure the output directory is set
            if (string.IsNullOrWhiteSpace(outdat))
                outdat = "out";

            // Get the DatTool for parsing
            DatTool dt = new DatTool();

            // Now that we have the dictionary, we can loop through and output to a new folder for each
            foreach (string key in foundDats.Keys)
            {
                // Get the DAT file associated with the key
                DatFile datFile = dt.CreateAndParse(Path.Combine(_dats, foundDats[key]));

                // Set the depot values
                datFile.Header.InputDepot = new DepotInformation(true, 4);
                datFile.Header.OutputDepot = new DepotInformation(true, 4);

                // Create the new output directory if it doesn't exist
                string outputFolder = Path.Combine(outdat, Path.GetFileNameWithoutExtension(foundDats[key]));
                DirectoryExtensions.Ensure(outputFolder, create: true);

                // Get all online depots
                List<string> onlineDepots = _depots.Where(d => d.Value.Item2).Select(d => d.Key).ToList();

                // Now scan all of those depots and rebuild
                datFile.RebuildDepot(
                    onlineDepots,
                    outDir: outputFolder,
                    outputFormat: (copy ? OutputFormat.TorrentGzipRomba : OutputFormat.TorrentZip));
            }
        }
    }
}
