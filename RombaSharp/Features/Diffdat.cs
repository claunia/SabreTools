using System.Collections.Generic;
using System.IO;
using SabreTools.DatFiles;
using SabreTools.DatTools;
using SabreTools.Help;
using SabreTools.IO;

namespace RombaSharp.Features
{
    internal class Diffdat : BaseFeature
    {
        public const string Value = "Diffdat";

        public Diffdat()
        {
            Name = Value;
            Flags = ["diffdat"];
            Description = "Creates a DAT file with those entries that are in -new DAT.";
            _featureType = ParameterType.Flag;
            LongDescription = @"Creates a DAT file with those entries that are in -new DAT file and not
in -old DAT file. Ignores those entries in -old that are not in -new.";
            this.Features = [];

            // Common Features
            AddCommonFeatures();

            AddFeature(OutStringInput);
            AddFeature(OldStringInput);
            AddFeature(NewStringInput);
            AddFeature(NameStringInput);
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
            string? newdat = GetString(features, NewStringValue);
            string? olddat = GetString(features, OldStringValue);
            string? outdat = GetString(features, OutStringValue);

            // Ensure the output directory
            outdat = outdat.Ensure(create: true);

            // Check that all required files exist
            if (!File.Exists(olddat))
            {
                logger.Error($"File '{olddat}' does not exist!");
                return false;
            }

            if (!File.Exists(newdat))
            {
                logger.Error($"File '{newdat}' does not exist!");
                return false;
            }

            // Create the encapsulating datfile
            DatFile datfile = DatFile.Create();
            datfile.Header.SetFieldValue<string?>(SabreTools.Models.Metadata.Header.NameKey, name);
            datfile.Header.SetFieldValue<string?>(SabreTools.Models.Metadata.Header.DescriptionKey, description);
            Parser.ParseInto(datfile, olddat);

            // Diff against the new datfile
            DatFile intDat = Parser.CreateAndParse(newdat);
            DatFileTool.DiffAgainst(datfile, intDat, false);
            Writer.Write(intDat, outdat!);
            return true;
        }
    }
}
