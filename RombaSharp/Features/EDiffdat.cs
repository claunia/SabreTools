using System.Collections.Generic;
using System.IO;

using SabreTools.Help;
using SabreTools.Library.DatFiles;
using SabreTools.Library.IO;

namespace RombaSharp.Features
{
    internal class EDiffdat : BaseFeature
    {
        public const string Value = "EDiffdat";

        public EDiffdat()
        {
            Name = Value;
            Flags = new List<string>() { "ediffdat" };
            Description = "Creates a DAT file with those entries that are in -new DAT.";
            _featureType = ParameterType.Flag;
            LongDescription = @"Creates a DAT file with those entries that are in -new DAT files and not in -old DAT files. Ignores those entries in -old that are not in -new.";
            Features = new Dictionary<string, Feature>();

            AddFeature(OutStringInput);
            AddFeature(OldStringInput);
            AddFeature(NewStringInput);
        }

        public override void ProcessFeatures(Dictionary<string, Feature> features)
        {
            base.ProcessFeatures(features);

            // Get feature flags
            string olddat = GetString(features, OldStringValue);
            string outdat = GetString(features, OutStringValue);
            string newdat = GetString(features, NewStringValue);

            // Ensure the output directory
            DirectoryExtensions.Ensure(outdat, create: true);

            // Check that all required files exist
            if (!File.Exists(olddat))
            {
                logger.Error($"File '{olddat}' does not exist!");
                return;
            }

            if (!File.Exists(newdat))
            {
                logger.Error($"File '{newdat}' does not exist!");
                return;
            }

            // Create the encapsulating datfile
            DatFile datfile = DatFile.CreateAndParse(olddat);

            // Diff against the new datfile
            DatFile intDat = DatFile.CreateAndParse(newdat);
            datfile.DiffAgainst(intDat, false);
            intDat.Write(outdat);
        }
    }
}
