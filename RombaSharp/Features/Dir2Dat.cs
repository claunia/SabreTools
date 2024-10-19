using System.Collections.Generic;
using System.IO;
using SabreTools.DatFiles;
using SabreTools.DatTools;
using SabreTools.FileTypes;
using SabreTools.Hashing;
using SabreTools.Help;
using SabreTools.IO.Extensions;

namespace RombaSharp.Features
{
    internal class Dir2Dat : BaseFeature
    {
        public const string Value = "Dir2Dat";

        public Dir2Dat()
        {
            Name = Value;
            Flags.AddRange(["dir2dat"]);
            Description = "Creates a DAT file for the specified input directory and saves it to the -out filename.";
            _featureType = ParameterType.Flag;
            LongDescription = "Creates a DAT file for the specified input directory and saves it to the -out filename.";

            // Common Features
            AddCommonFeatures();

            AddFeature(OutStringInput);
            AddFeature(SourceStringInput);
            AddFeature(NameStringInput); // Defaults to "untitled"
            AddFeature(DescriptionStringInput);
        }

        public override bool ProcessFeatures(Dictionary<string, Feature?> features)
        {
            // If the base fails, just fail out
            if (!base.ProcessFeatures(features))
                return false;

            // Get feature flags
            string? name = GetString(features, NameStringValue);
            string? description = GetString(features, DescriptionStringValue);
            string? source = GetString(features, SourceStringValue);
            string? outdat = GetString(features, OutStringValue);

            // Ensure the output directory
            outdat = outdat.Ensure(create: true);

            // Check that all required directories exist
            if (source == null || !Directory.Exists(source))
            {
                logger.Error($"File '{source}' does not exist!");
                return false;
            }

            // Create and write the encapsulating datfile
            DatFile datfile = DatFile.Create();
            datfile.Header.SetFieldValue<string?>(SabreTools.Models.Metadata.Header.NameKey, string.IsNullOrWhiteSpace(name) ? "untitled" : name);
            datfile.Header.SetFieldValue<string?>(SabreTools.Models.Metadata.Header.DescriptionKey, description);
            DatFromDir.PopulateFromDir(datfile, source, asFiles: TreatAsFile.NonArchive, hashes: [HashType.CRC32, HashType.MD5, HashType.SHA1]);
            Writer.Write(datfile, outdat!);
            return true;
        }
    }
}
