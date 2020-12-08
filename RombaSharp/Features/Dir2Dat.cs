using System.Collections.Generic;
using System.IO;

using SabreTools.Core;
using SabreTools.Filtering;
using SabreTools.Help;
using SabreTools.IO;
using SabreTools.Library.DatFiles;

namespace RombaSharp.Features
{
    internal class Dir2Dat : BaseFeature
    {
        public const string Value = "Dir2Dat";

        public Dir2Dat()
        {
            Name = Value;
            Flags = new List<string>() { "dir2dat" };
            Description = "Creates a DAT file for the specified input directory and saves it to the -out filename.";
            _featureType = ParameterType.Flag;
            LongDescription = "Creates a DAT file for the specified input directory and saves it to the -out filename.";
            Features = new Dictionary<string, Feature>();

            AddFeature(OutStringInput);
            AddFeature(SourceStringInput);
            AddFeature(NameStringInput); // Defaults to "untitled"
            AddFeature(DescriptionStringInput);
        }

        public override void ProcessFeatures(Dictionary<string, Feature> features)
        {
            base.ProcessFeatures(features);

            // Get feature flags
            string name = GetString(features, NameStringValue);
            string description = GetString(features, DescriptionStringValue);
            string source = GetString(features, SourceStringValue);
            string outdat = GetString(features, OutStringValue);

            // Ensure the output directory
            DirectoryExtensions.Ensure(outdat, create: true);

            // Check that all required directories exist
            if (!Directory.Exists(source))
            {
                logger.Error($"File '{source}' does not exist!");
                return;
            }

            // Create and write the encapsulating datfile
            DatFile datfile = DatFile.Create();
            datfile.Header.Name = string.IsNullOrWhiteSpace(name) ? "untitled" : name;
            datfile.Header.Description = description;
            datfile.PopulateFromDir(source, asFiles: TreatAsFile.NonArchive);
            datfile.ApplyCleaning(new Cleaner() { ExcludeFields = Hash.DeepHashes.AsFields() });
            datfile.Write(outdat);
        }
    }
}
