using System.Collections.Generic;
using System.IO;

using SabreTools.Library.Data;
using SabreTools.Library.DatFiles;
using SabreTools.Library.Filtering;
using SabreTools.Library.Help;
using SabreTools.Library.Tools;

namespace RombaSharp.Features
{
    internal class Diffdat : BaseFeature
    {
        public const string Value = "Diffdat";

        public Diffdat()
        {
            Name = Value;
            Flags = new List<string>() { "diffdat" };
            Description = "Creates a DAT file with those entries that are in -new DAT.";
            _featureType = FeatureType.Flag;
            LongDescription = @"Creates a DAT file with those entries that are in -new DAT file and not
in -old DAT file. Ignores those entries in -old that are not in -new.";
            this.Features = new Dictionary<string, Feature>();

            AddFeature(OutStringInput);
            AddFeature(OldStringInput);
            AddFeature(NewStringInput);
            AddFeature(NameStringInput);
            AddFeature(DescriptionStringInput);
        }

        public override void ProcessFeatures(Dictionary<string, Feature> features)
        {
            base.ProcessFeatures(features);

            // Get feature flags
            string name = GetString(features, NameStringValue);
            string description = GetString(features, DescriptionStringValue);
            string newdat = GetString(features, NewStringValue);
            string olddat = GetString(features, OldStringValue);
            string outdat = GetString(features, OutStringValue);

            // Ensure the output directory
            DirectoryExtensions.Ensure(outdat, create: true);

            // Check that all required files exist
            if (!File.Exists(olddat))
            {
                Globals.Logger.Error($"File '{olddat}' does not exist!");
                return;
            }

            if (!File.Exists(newdat))
            {
                Globals.Logger.Error($"File '{newdat}' does not exist!");
                return;
            }

            // Create the encapsulating datfile
            DatFile datfile = DatFile.Create();
            datfile.Header.Name = name;
            datfile.Header.Description = description;

            // Create the inputs
            List<string> dats = new List<string> { newdat };
            List<string> basedats = new List<string> { olddat };

            // Now run the diff on the inputs
            datfile.DetermineUpdateType(dats, basedats, outdat, UpdateMode.DiffAgainst, false /* inplace */, false /* skip */,
                new Filter(), new List<Field>(), false /* onlySame */);
        }
    }
}
