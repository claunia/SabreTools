using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SabreTools.Core;
using SabreTools.DatFiles;
using SabreTools.DatTools;
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
            Flags = ["ud", "update"];
            Description = "Update and manipulate DAT(s)";
            _featureType = ParameterType.Flag;
            LongDescription = "This is the multitool part of the program, allowing for almost every manipulation to a DAT, or set of DATs. This is also a combination of many different programs that performed DAT manipulation that work better together.";
            Features = [];

            // Common Features
            AddCommonFeatures();

            // Output Formats
            AddFeature(OutputTypeListInput);
            this[OutputTypeListInput]!.AddFeature(PrefixStringInput);
            this[OutputTypeListInput]!.AddFeature(PostfixStringInput);
            this[OutputTypeListInput]!.AddFeature(QuotesFlag);
            this[OutputTypeListInput]!.AddFeature(RomsFlag);
            this[OutputTypeListInput]!.AddFeature(GamePrefixFlag);
            this[OutputTypeListInput]!.AddFeature(AddExtensionStringInput);
            this[OutputTypeListInput]!.AddFeature(ReplaceExtensionStringInput);
            this[OutputTypeListInput]!.AddFeature(RemoveExtensionsFlag);
            this[OutputTypeListInput]!.AddFeature(RombaFlag);
            this[OutputTypeListInput]![RombaFlag]!.AddFeature(RombaDepthInt32Input);
            this[OutputTypeListInput]!.AddFeature(DeprecatedFlag);

            AddHeaderFeatures();
            AddFeature(KeepEmptyGamesFlag);
            AddFeature(CleanFlag);
            AddFeature(RemoveUnicodeFlag);
            AddFeature(DescriptionAsNameFlag);
            AddInternalSplitFeatures();
            AddFeature(TrimFlag);
            this[TrimFlag]!.AddFeature(RootDirStringInput);
            AddFeature(SingleSetFlag);
            AddFeature(DedupFlag);
            AddFeature(GameDedupFlag);
            AddFeature(MergeFlag);
            this[MergeFlag]!.AddFeature(NoAutomaticDateFlag);
            AddFeature(DiffAllFlag);
            this[DiffAllFlag]!.AddFeature(NoAutomaticDateFlag);
            AddFeature(DiffDuplicatesFlag);
            this[DiffDuplicatesFlag]!.AddFeature(NoAutomaticDateFlag);
            AddFeature(DiffIndividualsFlag);
            this[DiffIndividualsFlag]!.AddFeature(NoAutomaticDateFlag);
            AddFeature(DiffNoDuplicatesFlag);
            this[DiffNoDuplicatesFlag]!.AddFeature(NoAutomaticDateFlag);
            AddFeature(DiffAgainstFlag);
            this[DiffAgainstFlag]!.AddFeature(BaseDatListInput);
            this[DiffAgainstFlag]!.AddFeature(ByGameFlag);
            AddFeature(BaseReplaceFlag);
            this[BaseReplaceFlag]!.AddFeature(BaseDatListInput);
            this[BaseReplaceFlag]!.AddFeature(UpdateFieldListInput);
            this[BaseReplaceFlag]![UpdateFieldListInput]!.AddFeature(OnlySameFlag);
            AddFeature(ReverseBaseReplaceFlag);
            this[ReverseBaseReplaceFlag]!.AddFeature(BaseDatListInput);
            this[ReverseBaseReplaceFlag]!.AddFeature(UpdateFieldListInput);
            this[ReverseBaseReplaceFlag]![UpdateFieldListInput]!.AddFeature(OnlySameFlag);
            AddFeature(DiffCascadeFlag);
            this[DiffCascadeFlag]!.AddFeature(SkipFirstOutputFlag);
            AddFeature(DiffReverseCascadeFlag);
            this[DiffReverseCascadeFlag]!.AddFeature(SkipFirstOutputFlag);
            AddFeature(ExtraIniListInput);
            AddFilteringFeatures();
            AddFeature(OutputDirStringInput);
            AddFeature(InplaceFlag);
        }

        public override bool ProcessFeatures(Dictionary<string, Feature?> features)
        {
            // If the base fails, just fail out
            if (!base.ProcessFeatures(features))
                return false;

            // Get feature flags
            var updateMachineFieldNames = GetUpdateMachineFields(features);
            var updateItemFieldNames = GetUpdateDatItemFields(features);
            var updateMode = GetUpdateMode(features);

            // Normalize the extensions
            Header!.SetFieldValue<string?>(DatHeader.AddExtensionKey, (string.IsNullOrWhiteSpace(Header.GetStringFieldValue(DatHeader.AddExtensionKey)) || Header.GetStringFieldValue(DatHeader.AddExtensionKey)!.StartsWith(".")
                ? Header.GetStringFieldValue(DatHeader.AddExtensionKey)
                : $".{Header.GetStringFieldValue(DatHeader.AddExtensionKey)}"));
            Header.SetFieldValue<string?>(DatHeader.ReplaceExtensionKey, (string.IsNullOrWhiteSpace(Header.GetStringFieldValue(DatHeader.ReplaceExtensionKey)) || Header.GetStringFieldValue(DatHeader.ReplaceExtensionKey)!.StartsWith(".")
                ? Header.GetStringFieldValue(DatHeader.ReplaceExtensionKey)
                : $".{Header.GetStringFieldValue(DatHeader.ReplaceExtensionKey)}"));

            // If we're in a non-replacement special update mode and the names aren't set, set defaults
            if (updateMode != 0
                && !(updateMode.HasFlag(UpdateMode.DiffAgainst) || updateMode.HasFlag(UpdateMode.BaseReplace)))
            {
                // Get the values that will be used
                if (string.IsNullOrWhiteSpace(Header.GetStringFieldValue(Models.Metadata.Header.DateKey)))
                    Header.SetFieldValue<string?>(Models.Metadata.Header.DateKey, DateTime.Now.ToString("yyyy-MM-dd"));

                if (string.IsNullOrWhiteSpace(Header.GetStringFieldValue(Models.Metadata.Header.NameKey)))
                {
                    Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, (updateMode != 0 ? "DiffDAT" : "MergeDAT")
                        + (Header.GetStringFieldValue(Models.Metadata.Header.TypeKey) == "SuperDAT" ? "-SuperDAT" : string.Empty)
                        + (Cleaner!.DedupeRoms != DedupeType.None ? "-deduped" : string.Empty));
                }

                if (string.IsNullOrWhiteSpace(Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey)))
                {
                    Header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, (updateMode != 0 ? "DiffDAT" : "MergeDAT")
                        + (Header.GetStringFieldValue(Models.Metadata.Header.TypeKey) == "SuperDAT" ? "-SuperDAT" : string.Empty)
                        + (Cleaner!.DedupeRoms != DedupeType.None ? " - deduped" : string.Empty));

                    if (!GetBoolean(features, NoAutomaticDateValue))
                        Header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, $"{Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey)} ({Header.GetStringFieldValue(Models.Metadata.Header.DateKey)})");
                }

                if (string.IsNullOrWhiteSpace(Header.GetStringFieldValue(Models.Metadata.Header.CategoryKey)) && updateMode != 0)
                    Header.SetFieldValue<string?>(Models.Metadata.Header.CategoryKey, "DiffDAT");

                if (string.IsNullOrWhiteSpace(Header.GetStringFieldValue(Models.Metadata.Header.AuthorKey)))
                    Header.SetFieldValue<string?>(Models.Metadata.Header.AuthorKey, $"SabreTools {Globals.Version}");

                if (string.IsNullOrWhiteSpace(Header.GetStringFieldValue(Models.Metadata.Header.CommentKey)))
                    Header.SetFieldValue<string?>(Models.Metadata.Header.CommentKey, $"Generated by SabreTools {Globals.Version}");
            }

            // If no update fields are set, default to Names
            if (updateItemFieldNames == null || updateItemFieldNames.Count == 0)
            {
                updateItemFieldNames = [];
                updateItemFieldNames["item"] = [Models.Metadata.Rom.NameKey];
            }

            // Ensure we only have files in the inputs
            List<ParentablePath> inputPaths = PathTool.GetFilesOnly(Inputs, appendparent: true);
            List<ParentablePath> basePaths = PathTool.GetFilesOnly(GetList(features, BaseDatListValue));

            // Ensure the output directory
            OutputDir = OutputDir.Ensure();

            // If we're in standard update mode, run through all of the inputs
            if (updateMode == UpdateMode.None)
            {
                // Loop through each input and update
#if NET452_OR_GREATER || NETCOREAPP
                Parallel.ForEach(inputPaths, Globals.ParallelOptions, inputPath =>
#elif NET40_OR_GREATER
                Parallel.ForEach(inputPaths, inputPath =>
#else
                foreach (var inputPath in inputPaths)
#endif
                {
                    // Create a new base DatFile
                    DatFile datFile = DatFile.Create(Header);
                    logger.User($"Processing '{Path.GetFileName(inputPath.CurrentPath)}'");
                    Parser.ParseInto(datFile, inputPath, keep: true,
                        keepext: datFile.Header.GetFieldValue<DatFormat>(DatHeader.DatFormatKey).HasFlag(DatFormat.TSV)
                            || datFile.Header.GetFieldValue<DatFormat>(DatHeader.DatFormatKey).HasFlag(DatFormat.CSV)
                            || datFile.Header.GetFieldValue<DatFormat>(DatHeader.DatFormatKey).HasFlag(DatFormat.SSV));

                    // Perform additional processing steps
                    Extras!.ApplyExtras(datFile);
                    Splitter!.ApplySplitting(datFile, useTags: false);
                    datFile.ExecuteFilters(FilterRunner!);
                    Cleaner!.ApplyCleaning(datFile);
                    Remover!.ApplyRemovals(datFile);

                    // Get the correct output path
                    string realOutDir = inputPath.GetOutputPath(OutputDir, GetBoolean(features, InplaceValue))!;

                    // Try to output the file, overwriting only if it's not in the current directory
                    Writer.Write(datFile, realOutDir, overwrite: GetBoolean(features, InplaceValue));
#if NET40_OR_GREATER || NETCOREAPP
                });
#else
                }
#endif

                return true;
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
                datHeaders = DatFileTool.PopulateUserData(userInputDat, basePaths);
            else
                datHeaders = DatFileTool.PopulateUserData(userInputDat, inputPaths);

            // Perform additional processing steps
            Extras!.ApplyExtras(userInputDat);
            Splitter!.ApplySplitting(userInputDat, useTags: false);
            userInputDat.ExecuteFilters(FilterRunner!);
            Cleaner!.ApplyCleaning(userInputDat);
            Remover!.ApplyRemovals(userInputDat);

            // Output only DatItems that are duplicated across inputs
            if (updateMode.HasFlag(UpdateMode.DiffDupesOnly))
            {
                DatFile dupeData = DatFileTool.DiffDuplicates(userInputDat, inputPaths);

                InternalStopwatch watch = new("Outputting duplicate DAT");
                Writer.Write(dupeData, OutputDir, overwrite: false);
                watch.Stop();
            }

            // Output only DatItems that are not duplicated across inputs
            if (updateMode.HasFlag(UpdateMode.DiffNoDupesOnly))
            {
                DatFile outerDiffData = DatFileTool.DiffNoDuplicates(userInputDat, inputPaths);

                InternalStopwatch watch = new("Outputting no duplicate DAT");
                Writer.Write(outerDiffData, OutputDir, overwrite: false);
                watch.Stop();
            }

            // Output only DatItems that are unique to each input
            if (updateMode.HasFlag(UpdateMode.DiffIndividualsOnly))
            {
                // Get all of the output DatFiles
                List<DatFile> datFiles = DatFileTool.DiffIndividuals(userInputDat, inputPaths);

                // Loop through and output the new DatFiles
                InternalStopwatch watch = new("Outputting all individual DATs");

#if NET452_OR_GREATER || NETCOREAPP
                Parallel.For(0, inputPaths.Count, Globals.ParallelOptions, j =>
#elif NET40_OR_GREATER
                Parallel.For(0, inputPaths.Count, j =>
#else
                for (int j = 0; j < inputPaths.Count; j++)
#endif
                {
                    string path = inputPaths[j].GetOutputPath(OutputDir, GetBoolean(features, InplaceValue))!;

                    // Try to output the file
                    Writer.Write(datFiles[j], path, overwrite: GetBoolean(features, InplaceValue));
#if NET40_OR_GREATER || NETCOREAPP
                });
#else
                }
#endif

                watch.Stop();
            }

            // Output cascaded diffs
            if (updateMode.HasFlag(UpdateMode.DiffCascade))
            {
                // Preprocess the DatHeaders
#if NET452_OR_GREATER || NETCOREAPP
                Parallel.For(0, datHeaders.Count, Globals.ParallelOptions, j =>
#elif NET40_OR_GREATER
                Parallel.For(0, datHeaders.Count, j =>
#else
                for (int j = 0; j < datHeaders.Count; j++)
#endif
                {
                    // If we're outputting to the runtime folder, rename
                    if (!GetBoolean(features, InplaceValue) && OutputDir == Environment.CurrentDirectory)
                    {
                        string innerpost = $" ({j} - {inputPaths[j].GetNormalizedFileName(true)} Only)";

                        datHeaders[j] = userInputDat.Header;
                        datHeaders[j].SetFieldValue<string?>(DatHeader.FileNameKey, datHeaders[j].GetStringFieldValue(DatHeader.FileNameKey) + innerpost);
                        datHeaders[j].SetFieldValue<string?>(Models.Metadata.Header.NameKey, datHeaders[j].GetStringFieldValue(Models.Metadata.Header.NameKey) + innerpost);
                        datHeaders[j].SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, datHeaders[j].GetStringFieldValue(Models.Metadata.Header.DescriptionKey) + innerpost);
                    }
#if NET40_OR_GREATER || NETCOREAPP
                });
#else
                }
#endif

                // Get all of the output DatFiles
                List<DatFile> datFiles = DatFileTool.DiffCascade(userInputDat, datHeaders);

                // Loop through and output the new DatFiles
                InternalStopwatch watch = new("Outputting all created DATs");

                int startIndex = GetBoolean(features, SkipFirstOutputValue) ? 1 : 0;
#if NET452_OR_GREATER || NETCOREAPP
                Parallel.For(startIndex, inputPaths.Count, Globals.ParallelOptions, j =>
#elif NET40_OR_GREATER
                Parallel.For(startIndex, inputPaths.Count, j =>
#else
                for (int j = startIndex; j < inputPaths.Count; j++)
#endif
                {
                    string path = inputPaths[j].GetOutputPath(OutputDir, GetBoolean(features, InplaceValue))!;

                    // Try to output the file
                    Writer.Write(datFiles[j], path, overwrite: GetBoolean(features, InplaceValue));
#if NET40_OR_GREATER || NETCOREAPP
                });
#else
                }
#endif

                watch.Stop();
            }

            // Output differences against a base DAT
            if (updateMode.HasFlag(UpdateMode.DiffAgainst))
            {
                // Loop through each input and diff against the base
#if NET452_OR_GREATER || NETCOREAPP
                Parallel.ForEach(inputPaths, Globals.ParallelOptions, inputPath =>
#elif NET40_OR_GREATER
                Parallel.ForEach(inputPaths, inputPath =>
#else
                foreach (var inputPath in inputPaths)
#endif
                {
                    // Parse the path to a new DatFile
                    DatFile repDat = DatFile.Create(Header);
                    Parser.ParseInto(repDat, inputPath, indexId: 1, keep: true);

                    // Perform additional processing steps
                    Extras.ApplyExtras(repDat);
                    Splitter.ApplySplitting(repDat, useTags: false);
                    repDat.ExecuteFilters(FilterRunner!);
                    Cleaner.ApplyCleaning(repDat);
                    Remover.ApplyRemovals(repDat);

                    // Now replace the fields from the base DatFile
                    DatFileTool.DiffAgainst(userInputDat, repDat, GetBoolean(Features, ByGameValue));

                    // Finally output the diffed DatFile
                    string interOutDir = inputPath.GetOutputPath(OutputDir, GetBoolean(features, InplaceValue))!;
                    Writer.Write(repDat, interOutDir, overwrite: GetBoolean(features, InplaceValue));
#if NET40_OR_GREATER || NETCOREAPP
                });
#else
                }
#endif
            }

            // Output DATs after replacing fields from a base DatFile
            if (updateMode.HasFlag(UpdateMode.BaseReplace))
            {
                // Loop through each input and apply the base DatFile
#if NET452_OR_GREATER || NETCOREAPP
                Parallel.ForEach(inputPaths, Globals.ParallelOptions, inputPath =>
#elif NET40_OR_GREATER
                Parallel.ForEach(inputPaths, inputPath =>
#else
                foreach (var inputPath in inputPaths)
#endif
                {
                    // Parse the path to a new DatFile
                    DatFile repDat = DatFile.Create(Header);
                    Parser.ParseInto(repDat, inputPath, indexId: 1, keep: true);

                    // Perform additional processing steps
                    Extras.ApplyExtras(repDat);
                    Splitter.ApplySplitting(repDat, useTags: false);
                    repDat.ExecuteFilters(FilterRunner!);
                    Cleaner.ApplyCleaning(repDat);
                    Remover.ApplyRemovals(repDat);

                    // Now replace the fields from the base DatFile
                    DatFileTool.BaseReplace(
                        userInputDat,
                        repDat,
                        updateMachineFieldNames,
                        updateItemFieldNames,
                        GetBoolean(features, OnlySameValue));

                    // Finally output the replaced DatFile
                    string interOutDir = inputPath.GetOutputPath(OutputDir, GetBoolean(features, InplaceValue))!;
                    Writer.Write(repDat, interOutDir, overwrite: GetBoolean(features, InplaceValue));
#if NET40_OR_GREATER || NETCOREAPP
                });
#else
                }
#endif
            }

            // Merge all input files and write
            // This has to be last due to the SuperDAT handling
            if (updateMode.HasFlag(UpdateMode.Merge))
            {
                // If we're in SuperDAT mode, prefix all games with their respective DATs
                if (string.Equals(userInputDat.Header.GetStringFieldValue(Models.Metadata.Header.TypeKey), "SuperDAT", StringComparison.OrdinalIgnoreCase))
                    DatFileTool.ApplySuperDAT(userInputDat, inputPaths);

                Writer.Write(userInputDat, OutputDir);
            }

            return true;
        }
    }
}
