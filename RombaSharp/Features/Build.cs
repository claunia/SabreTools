using System.Collections.Generic;
using System.IO;
using System.Linq;
using SabreTools.DatFiles;
using SabreTools.DatTools;
using SabreTools.FileTypes;
using SabreTools.Help;
using SabreTools.IO.Extensions;

namespace RombaSharp.Features
{
    internal class Build : BaseFeature
    {
        public const string Value = "Build";

        public Build()
        {
            Name = Value;
            Flags = ["build"];
            Description = "For each specified DAT file it creates the torrentzip files.";
            _featureType = ParameterType.Flag;
            LongDescription = @"For each specified DAT file it creates the torrentzip files in the specified
output dir. The files will be placed in the specified location using a folder
structure according to the original DAT master directory tree structure.";
            Features = [];

            // Common Features
            AddCommonFeatures();

            AddFeature(OutStringInput);
            AddFeature(FixdatOnlyFlag);
            AddFeature(CopyFlag);
            AddFeature(WorkersInt32Input);
            AddFeature(SubworkersInt32Input);
        }

        public override bool ProcessFeatures(Dictionary<string, Feature?> features)
        {
            // If the base fails, just fail out
            if (!base.ProcessFeatures(features))
                return false;

            // Get feature flags
            bool copy = GetBoolean(features, CopyValue);
            string? outdat = GetString(features, OutStringValue);

            // Verify the filenames
            Dictionary<string, string> foundDats = GetValidDats(Inputs);

            // Ensure the output directory is set
            if (string.IsNullOrWhiteSpace(outdat))
                outdat = "out";

            // Now that we have the dictionary, we can loop through and output to a new folder for each
            foreach (string key in foundDats.Keys)
            {
                // Get the DAT file associated with the key
                DatFile datFile = Parser.CreateAndParse(Path.Combine(_dats!, foundDats[key]));

                // Set the depot values
                datFile.Header.SetFieldValue<DepotInformation?>(DatHeader.InputDepotKey, new DepotInformation(true, 4));
                datFile.Header.SetFieldValue<DepotInformation?>(DatHeader.OutputDepotKey, new DepotInformation(true, 4));

                // Create the new output directory if it doesn't exist
                string outputFolder = Path.Combine(outdat, Path.GetFileNameWithoutExtension(foundDats[key]));
                outputFolder.Ensure(create: true);

                // Get all online depots
                List<string> onlineDepots = _depots!.Where(d => d.Value.Item2).Select(d => d.Key).ToList();

                // Now scan all of those depots and rebuild
                Rebuilder.RebuildDepot(
                    datFile,
                    onlineDepots,
                    outDir: outputFolder,
                    outputFormat: copy ? OutputFormat.TorrentGzipRomba : OutputFormat.TorrentZip);
            }

            return true;
        }
    }
}
