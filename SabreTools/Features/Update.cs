using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using SabreTools.Core;
using SabreTools.DatFiles;
using SabreTools.Help;
using SabreTools.IO;
using SabreTools.Logging;

namespace SabreTools.Features
{
    internal class Update : BaseFeature
    {
        public const string Value = "Update";

        public Update()
        {
            Name = Value;
            Flags = new List<string>() { "-ud", "--update" };
            Description = "Update and manipulate DAT(s)";
            _featureType = ParameterType.Flag;
            LongDescription = "This is the multitool part of the program, allowing for almost every manipulation to a DAT, or set of DATs. This is also a combination of many different programs that performed DAT manipulation that work better together.";
            Features = new Dictionary<string, Help.Feature>();

            // Output Formats
            AddFeature(OutputTypeListInput);
            this[OutputTypeListInput].AddFeature(PrefixStringInput);
            this[OutputTypeListInput].AddFeature(PostfixStringInput);
            this[OutputTypeListInput].AddFeature(QuotesFlag);
            this[OutputTypeListInput].AddFeature(RomsFlag);
            this[OutputTypeListInput].AddFeature(GamePrefixFlag);
            this[OutputTypeListInput].AddFeature(AddExtensionStringInput);
            this[OutputTypeListInput].AddFeature(ReplaceExtensionStringInput);
            this[OutputTypeListInput].AddFeature(RemoveExtensionsFlag);
            this[OutputTypeListInput].AddFeature(RombaFlag);
            this[OutputTypeListInput][RombaFlag].AddFeature(RombaDepthInt32Input);
            this[OutputTypeListInput].AddFeature(DeprecatedFlag);

            AddHeaderFeatures();
            AddFeature(KeepEmptyGamesFlag);
            AddFeature(CleanFlag);
            AddFeature(RemoveUnicodeFlag);
            AddFeature(DescriptionAsNameFlag);
            AddInternalSplitFeatures();
            AddFeature(TrimFlag);
            this[TrimFlag].AddFeature(RootDirStringInput);
            AddFeature(SingleSetFlag);
            AddFeature(DedupFlag);
            AddFeature(GameDedupFlag);
            AddFeature(MergeFlag);
            this[MergeFlag].AddFeature(NoAutomaticDateFlag);
            AddFeature(DiffAllFlag);
            this[DiffAllFlag].AddFeature(NoAutomaticDateFlag);
            AddFeature(DiffDuplicatesFlag);
            this[DiffDuplicatesFlag].AddFeature(NoAutomaticDateFlag);
            AddFeature(DiffIndividualsFlag);
            this[DiffIndividualsFlag].AddFeature(NoAutomaticDateFlag);
            AddFeature(DiffNoDuplicatesFlag);
            this[DiffNoDuplicatesFlag].AddFeature(NoAutomaticDateFlag);
            AddFeature(DiffAgainstFlag);
            this[DiffAgainstFlag].AddFeature(BaseDatListInput);
            this[DiffAgainstFlag].AddFeature(ByGameFlag);
            AddFeature(BaseReplaceFlag);
            this[BaseReplaceFlag].AddFeature(BaseDatListInput);
            this[BaseReplaceFlag].AddFeature(UpdateFieldListInput);
            this[BaseReplaceFlag][UpdateFieldListInput].AddFeature(OnlySameFlag);
            this[BaseReplaceFlag].AddFeature(UpdateNamesFlag);
            this[BaseReplaceFlag].AddFeature(UpdateHashesFlag);
            this[BaseReplaceFlag].AddFeature(UpdateDescriptionFlag);
            this[BaseReplaceFlag][UpdateDescriptionFlag].AddFeature(OnlySameFlag);
            this[BaseReplaceFlag].AddFeature(UpdateGameTypeFlag);
            this[BaseReplaceFlag].AddFeature(UpdateYearFlag);
            this[BaseReplaceFlag].AddFeature(UpdateManufacturerFlag);
            this[BaseReplaceFlag].AddFeature(UpdateParentsFlag);
            AddFeature(ReverseBaseReplaceFlag);
            this[ReverseBaseReplaceFlag].AddFeature(BaseDatListInput);
            this[ReverseBaseReplaceFlag].AddFeature(UpdateFieldListInput);
            this[ReverseBaseReplaceFlag][UpdateFieldListInput].AddFeature(OnlySameFlag);
            this[ReverseBaseReplaceFlag].AddFeature(UpdateNamesFlag);
            this[ReverseBaseReplaceFlag].AddFeature(UpdateHashesFlag);
            this[ReverseBaseReplaceFlag].AddFeature(UpdateDescriptionFlag);
            this[ReverseBaseReplaceFlag][UpdateDescriptionFlag].AddFeature(OnlySameFlag);
            this[ReverseBaseReplaceFlag].AddFeature(UpdateGameTypeFlag);
            this[ReverseBaseReplaceFlag].AddFeature(UpdateYearFlag);
            this[ReverseBaseReplaceFlag].AddFeature(UpdateManufacturerFlag);
            this[ReverseBaseReplaceFlag].AddFeature(UpdateParentsFlag);
            AddFeature(DiffCascadeFlag);
            this[DiffCascadeFlag].AddFeature(SkipFirstOutputFlag);
            AddFeature(DiffReverseCascadeFlag);
            this[DiffReverseCascadeFlag].AddFeature(SkipFirstOutputFlag);
            AddFeature(ExtraIniListInput);
            AddFilteringFeatures();
            AddFeature(OutputDirStringInput);
            AddFeature(InplaceFlag);
            AddFeature(ThreadsInt32Input);
        }

        public override void ProcessFeatures(Dictionary<string, Help.Feature> features)
        {
            base.ProcessFeatures(features);

            // Get feature flags
            var updateFields = GetUpdateFields(features);
            var updateMode = GetUpdateMode(features);

            // Normalize the extensions
            Header.AddExtension = (string.IsNullOrWhiteSpace(Header.AddExtension) || Header.AddExtension.StartsWith(".")
                ? Header.AddExtension
                : $".{Header.AddExtension}");
            Header.ReplaceExtension = (string.IsNullOrWhiteSpace(Header.ReplaceExtension) || Header.ReplaceExtension.StartsWith(".")
                ? Header.ReplaceExtension
                : $".{Header.ReplaceExtension}");

            // If we're in a special update mode and the names aren't set, set defaults
            if (updateMode != 0)
            {
                // Get the values that will be used
                if (string.IsNullOrWhiteSpace(Header.Date))
                    Header.Date = DateTime.Now.ToString("yyyy-MM-dd");

                if (string.IsNullOrWhiteSpace(Header.Name))
                {
                    Header.Name = (updateMode != 0 ? "DiffDAT" : "MergeDAT")
                        + (Header.Type == "SuperDAT" ? "-SuperDAT" : string.Empty)
                        + (Cleaner.DedupeRoms != DedupeType.None ? "-deduped" : string.Empty);
                }

                if (string.IsNullOrWhiteSpace(Header.Description))
                {
                    Header.Description = (updateMode != 0 ? "DiffDAT" : "MergeDAT")
                        + (Header.Type == "SuperDAT" ? "-SuperDAT" : string.Empty)
                        + (Cleaner.DedupeRoms != DedupeType.None ? " - deduped" : string.Empty);

                    if (!GetBoolean(features, NoAutomaticDateValue))
                        Header.Description += $" ({Header.Date})";
                }

                if (string.IsNullOrWhiteSpace(Header.Category) && updateMode != 0)
                    Header.Category = "DiffDAT";

                if (string.IsNullOrWhiteSpace(Header.Author))
                    Header.Author = "SabreTools";
            }

            // If no update fields are set, default to Names
            if (updateFields == null || updateFields.Count == 0)
                updateFields = new List<Field>() { Field.DatItem_Name };

            // Ensure we only have files in the inputs
            List<ParentablePath> inputPaths = DirectoryExtensions.GetFilesOnly(Inputs, appendparent: true);
            List<ParentablePath> basePaths = DirectoryExtensions.GetFilesOnly(GetList(features, BaseDatListValue));

            // Get the DatTool for parsing
            DatTool dt = new DatTool();

            // If we're in standard update mode, run through all of the inputs
            if (updateMode == UpdateMode.None)
            {
                // Loop through each input and update
                Parallel.ForEach(inputPaths, Globals.ParallelOptions, inputPath =>
                {
                    // Create a new base DatFile
                    DatFile datFile = DatFile.Create(Header);
                    logger.User($"Processing '{Path.GetFileName(inputPath.CurrentPath)}'");
                    dt.ParseInto(datFile, inputPath, keep: true,
                        keepext: datFile.Header.DatFormat.HasFlag(DatFormat.TSV)
                            || datFile.Header.DatFormat.HasFlag(DatFormat.CSV)
                            || datFile.Header.DatFormat.HasFlag(DatFormat.SSV));

                    // Perform additional processing steps
                    datFile.ApplyExtras(Extras);
                    datFile.ApplySplitting(GetSplitType(features), false);
                    datFile.ApplyFilter(Filter);
                    datFile.ApplyCleaning(Cleaner);

                    // Get the correct output path
                    string realOutDir = inputPath.GetOutputPath(OutputDir, GetBoolean(features, InplaceValue));

                    // Try to output the file, overwriting only if it's not in the current directory
                    dt.Write(datFile, realOutDir, overwrite: GetBoolean(features, InplaceValue));
                });

                return;
            }

            // Reverse inputs if we're in a required mode
            if (updateMode.HasFlag(UpdateMode.DiffReverseCascade))
            {
                updateMode |= UpdateMode.DiffCascade;
                inputPaths.Reverse();
            }
            if (updateMode.HasFlag(UpdateMode.ReverseBaseReplace))
            {
                updateMode |= UpdateMode.BaseReplace;
                basePaths.Reverse();
            }

            // Create a DAT to capture inputs
            DatFile userInputDat = DatFile.Create(Header);

            // Populate using the correct set
            List<DatHeader> datHeaders;
            if (updateMode.HasFlag(UpdateMode.DiffAgainst) || updateMode.HasFlag(UpdateMode.BaseReplace))
                datHeaders = userInputDat.PopulateUserData(basePaths);
            else
                datHeaders = userInputDat.PopulateUserData(inputPaths);

            // Perform additional processing steps
            userInputDat.ApplyExtras(Extras);
            userInputDat.ApplySplitting(GetSplitType(features), false);
            userInputDat.ApplyFilter(Filter);
            userInputDat.ApplyCleaning(Cleaner);

            // Output only DatItems that are duplicated across inputs
            if (updateMode.HasFlag(UpdateMode.DiffDupesOnly))
            {
                DatFile dupeData = userInputDat.DiffDuplicates(inputPaths);

                InternalStopwatch watch = new InternalStopwatch("Outputting duplicate DAT");
                dt.Write(dupeData, OutputDir, overwrite: false);
                watch.Stop();
            }

            // Output only DatItems that are not duplicated across inputs
            if (updateMode.HasFlag(UpdateMode.DiffNoDupesOnly))
            {
                DatFile outerDiffData = userInputDat.DiffNoDuplicates(inputPaths);

                InternalStopwatch watch = new InternalStopwatch("Outputting no duplicate DAT");
                dt.Write(outerDiffData, OutputDir, overwrite: false);
                watch.Stop();
            }

            // Output only DatItems that are unique to each input
            if (updateMode.HasFlag(UpdateMode.DiffIndividualsOnly))
            {
                // Get all of the output DatFiles
                List<DatFile> datFiles = userInputDat.DiffIndividuals(inputPaths);

                // Loop through and output the new DatFiles
                InternalStopwatch watch = new InternalStopwatch("Outputting all individual DATs");

                Parallel.For(0, inputPaths.Count, Globals.ParallelOptions, j =>
                {
                    string path = inputPaths[j].GetOutputPath(OutputDir, GetBoolean(features, InplaceValue));

                    // Try to output the file
                    dt.Write(datFiles[j], path, overwrite: GetBoolean(features, InplaceValue));
                });

                watch.Stop();
            }

            // Output cascaded diffs
            if (updateMode.HasFlag(UpdateMode.DiffCascade))
            {
                // Preprocess the DatHeaders
                Parallel.For(0, datHeaders.Count, Globals.ParallelOptions, j =>
                {
                    // If we're outputting to the runtime folder, rename
                    if (!GetBoolean(features, InplaceValue) && OutputDir == Environment.CurrentDirectory)
                    {
                        string innerpost = $" ({j} - {inputPaths[j].GetNormalizedFileName(true)} Only)";

                        datHeaders[j] = userInputDat.Header;
                        datHeaders[j].FileName += innerpost;
                        datHeaders[j].Name += innerpost;
                        datHeaders[j].Description += innerpost;
                    }
                });

                // Get all of the output DatFiles
                List<DatFile> datFiles = userInputDat.DiffCascade(datHeaders);

                // Loop through and output the new DatFiles
                InternalStopwatch watch = new InternalStopwatch("Outputting all created DATs");

                int startIndex = GetBoolean(features, SkipFirstOutputValue) ? 1 : 0;
                Parallel.For(startIndex, inputPaths.Count, Globals.ParallelOptions, j =>
                {
                    string path = inputPaths[j].GetOutputPath(OutputDir, GetBoolean(features, InplaceValue));

                    // Try to output the file
                    dt.Write(datFiles[j], path, overwrite: GetBoolean(features, InplaceValue));
                });

                watch.Stop();
            }

            // Output differences against a base DAT
            if (updateMode.HasFlag(UpdateMode.DiffAgainst))
            {
                // Loop through each input and diff against the base
                Parallel.ForEach(inputPaths, Globals.ParallelOptions, inputPath =>
                {
                    // Parse the path to a new DatFile
                    DatFile repDat = DatFile.Create(userInputDat.Header.CloneFiltering());
                    dt.ParseInto(repDat, inputPath, indexId: 1, keep: true);

                    // Perform additional processing steps
                    repDat.ApplyExtras(Extras);
                    repDat.ApplySplitting(GetSplitType(features), false);
                    repDat.ApplyFilter(Filter);
                    repDat.ApplyCleaning(Cleaner);

                    // Now replace the fields from the base DatFile
                    userInputDat.DiffAgainst(repDat, GetBoolean(Features, ByGameValue));

                    // Finally output the diffed DatFile
                    string interOutDir = inputPath.GetOutputPath(OutputDir, GetBoolean(features, InplaceValue));
                    dt.Write(repDat, interOutDir, overwrite: GetBoolean(features, InplaceValue));
                });
            }

            // Output DATs after replacing fields from a base DatFile
            if (updateMode.HasFlag(UpdateMode.BaseReplace))
            {
                // Loop through each input and apply the base DatFile
                Parallel.ForEach(inputPaths, Globals.ParallelOptions, inputPath =>
                {
                    // Parse the path to a new DatFile
                    DatFile repDat = DatFile.Create(userInputDat.Header.CloneFiltering());
                    dt.ParseInto(repDat, inputPath, indexId: 1, keep: true);

                    // Perform additional processing steps
                    repDat.ApplyExtras(Extras);
                    repDat.ApplySplitting(GetSplitType(features), false);
                    repDat.ApplyFilter(Filter);
                    repDat.ApplyCleaning(Cleaner);

                    // Now replace the fields from the base DatFile
                    userInputDat.BaseReplace(repDat, updateFields, GetBoolean(features, OnlySameValue));

                    // Finally output the replaced DatFile
                    string interOutDir = inputPath.GetOutputPath(OutputDir, GetBoolean(features, InplaceValue));
                    dt.Write(repDat, interOutDir, overwrite: GetBoolean(features, InplaceValue));
                });
            }

            // Merge all input files and write
            // This has to be last due to the SuperDAT handling
            if (updateMode.HasFlag(UpdateMode.Merge))
            {
                // If we're in SuperDAT mode, prefix all games with their respective DATs
                if (string.Equals(userInputDat.Header.Type, "SuperDAT", StringComparison.OrdinalIgnoreCase))
                    userInputDat.ApplySuperDAT(inputPaths);

                dt.Write(userInputDat, OutputDir);
            }
        }
    }
}
