using System;
using System.Collections.Generic;
using System.IO;

using SabreTools.Core;
using SabreTools.DatFiles;
using SabreTools.DatTools;
using SabreTools.FileTypes;
using SabreTools.Help;

namespace SabreTools.Features
{
    internal class DatFromDir : BaseFeature
    {
        public const string Value = "DATFromDir";

        public DatFromDir()
        {
            Name = Value;
            Flags = new List<string>() { "-d", "--d2d", "--dfd" };
            Description = "Create DAT(s) from an input directory";
            _featureType = ParameterType.Flag;
            LongDescription = "Create a DAT file from an input directory or set of files. By default, this will output a DAT named based on the input directory and the current date. It will also treat all archives as possible games and add all three hashes (CRC, MD5, SHA-1) for each file.";
            Features = new Dictionary<string, Help.Feature>();

            // Hash Features
            AddFeature(SkipMd5Flag);
#if NET_FRAMEWORK
            AddFeature(SkipRipeMd160Flag);
#endif
            AddFeature(SkipSha1Flag);
            AddFeature(SkipSha256Flag);
            AddFeature(SkipSha384Flag);
            AddFeature(SkipSha512Flag);
            AddFeature(SkipSpamSumFlag);

            AddFeature(NoAutomaticDateFlag);
            AddFeature(AaruFormatsAsFilesFlag);
            AddFeature(ArchivesAsFilesFlag);
            AddFeature(ChdsAsFilesFlag);
            AddFeature(OutputTypeListInput);
            this[OutputTypeListInput].AddFeature(DeprecatedFlag);
            AddFeature(RombaFlag);
            this[RombaFlag].AddFeature(RombaDepthInt32Input);
            AddFeature(SkipArchivesFlag);
            AddFeature(SkipFilesFlag);
            AddHeaderFeatures();
            AddFeature(AddBlankFilesFlag);
            AddFeature(AddDateFlag);
            AddFeature(HeaderStringInput);
            AddFeature(ExtraIniListInput);
            AddFilteringFeatures();
            AddFeature(OutputDirStringInput);
            AddFeature(ThreadsInt32Input);
        }

        public override void ProcessFeatures(Dictionary<string, Help.Feature> features)
        {
            base.ProcessFeatures(features);

            // Get feature flags
            bool addBlankFiles = GetBoolean(features, AddBlankFilesValue);
            bool addFileDates = GetBoolean(features, AddDateValue);
            TreatAsFile asFiles = GetTreatAsFiles(features);
            bool noAutomaticDate = GetBoolean(features, NoAutomaticDateValue);
            var includeInScan = GetIncludeInScan(features);
            var skipFileType = GetSkipFileType(features);
            var splitType = GetSplitType(features);

            // Apply the specialized field removals to the cleaner
            if (Cleaner.ExcludeDatItemFields == null)
                Cleaner.ExcludeDatItemFields = new List<DatItemField>();

            if (!addFileDates)
                Cleaner.ExcludeDatItemFields.Add(DatItemField.Date);

            // Create a new DATFromDir object and process the inputs
            DatFile basedat = DatFile.Create(Header);
            basedat.Header.Date = DateTime.Now.ToString("yyyy-MM-dd");

            // For each input directory, create a DAT
            foreach (string path in Inputs)
            {
                if (Directory.Exists(path) || File.Exists(path))
                {
                    // Clone the base Dat for information
                    DatFile datdata = DatFile.Create(basedat.Header);

                    // Get the base path and fill the header, if needed
                    string basePath = Path.GetFullPath(path);
                    datdata.FillHeaderFromPath(basePath, noAutomaticDate);

                    // Now populate from the path
                    bool success = DatTools.DatFromDir.PopulateFromDir(
                        datdata,
                        basePath,
                        asFiles,
                        skipFileType,
                        addBlankFiles,
                        hashes: includeInScan);

                    if (success)
                    {
                        // Perform additional processing steps
                        Modification.ApplyExtras(datdata, Extras);
                        Modification.ApplySplitting(datdata, splitType, false);
                        Modification.ApplyFilters(datdata, Cleaner);
                        Modification.ApplyCleaning(datdata, Cleaner);

                        // Write out the file
                        Writer.Write(datdata, OutputDir);
                    }
                    else
                    {
                        Console.WriteLine();
                        OutputRecursive(0);
                    }
                }
            }
        }
    }
}
