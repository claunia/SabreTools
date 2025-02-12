using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SabreTools.Core;
using SabreTools.DatFiles;
using SabreTools.DatTools;
using SabreTools.Help;
using SabreTools.IO;
using SabreTools.IO.Extensions;
using SabreTools.IO.Logging;

namespace SabreTools.Features
{
    internal class Update : BaseFeature
    {
        public const string Value = "Update";

        public Update()
        {
            Name = Value;
            Flags.AddRange(["ud", "update"]);
            Description = "Update and manipulate DAT(s)";
            _featureType = ParameterType.Flag;
            LongDescription = "This is the multitool part of the program, allowing for almost every manipulation to a DAT, or set of DATs. This is also a combination of many different programs that performed DAT manipulation that work better together.";

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
            Modifiers!.AddExtension = string.IsNullOrEmpty(Modifiers.AddExtension) || Modifiers.AddExtension!.StartsWith(".")
                ? Modifiers.AddExtension
                : $".{Modifiers.AddExtension}";
            Modifiers.ReplaceExtension = string.IsNullOrEmpty(Modifiers.ReplaceExtension) || Modifiers.ReplaceExtension!.StartsWith(".")
                ? Modifiers.ReplaceExtension
                : $".{Modifiers.ReplaceExtension}";

            // If we're in a non-replacement special update mode and the names aren't set, set defaults
            SetDefaultHeaderValues(updateMode, GetBoolean(features, NoAutomaticDateValue));

            // If no update fields are set, default to Names
            if (updateItemFieldNames == null || updateItemFieldNames.Count == 0)
            {
                updateItemFieldNames = [];
                updateItemFieldNames["item"] = [Models.Metadata.Rom.NameKey];
            }

            // Ensure we only have files in the inputs
            List<ParentablePath> inputPaths = PathTool.GetFilesOnly(Inputs, appendParent: true);
            List<ParentablePath> basePaths = PathTool.GetFilesOnly(GetList(features, BaseDatListValue));

            // Ensure the output directory
            OutputDir = OutputDir.Ensure();

            // If we're in standard update mode, run through all of the inputs
            if (updateMode == UpdateMode.None)
            {
                StandardUpdate(inputPaths, GetBoolean(features, InplaceValue));
                return true;
            }

            // Reverse inputs if we're in a required mode
#if NET20 || NET35
            if ((updateMode & UpdateMode.DiffReverseCascade) != 0)
#else
            if (updateMode.HasFlag(UpdateMode.DiffReverseCascade))
#endif
            {
                updateMode |= UpdateMode.DiffCascade;
                inputPaths.Reverse();
            }
#if NET20 || NET35
            if ((updateMode & UpdateMode.ReverseBaseReplace) != 0)
#else
            if (updateMode.HasFlag(UpdateMode.ReverseBaseReplace))
#endif
            {
                updateMode |= UpdateMode.BaseReplace;
                basePaths.Reverse();
            }

            // Create a DAT to capture inputs
            DatFile userInputDat = Parser.CreateDatFile(Header!, Modifiers);

            // Populate using the correct set
            List<DatHeader> datHeaders = GetDatHeaders(updateMode, inputPaths, basePaths, userInputDat);

            // Perform additional processing steps
            AdditionalProcessing(userInputDat);

            // Output only DatItems that are duplicated across inputs
#if NET20 || NET35
            if ((updateMode & UpdateMode.DiffDupesOnly) != 0)
#else
            if (updateMode.HasFlag(UpdateMode.DiffDupesOnly))
#endif
            {
                DatFile dupeData = Diffing.Duplicates(userInputDat, inputPaths);
                //DatFile dupeData = Diffing.DuplicatesDB(userInputDat, inputPaths);

                InternalStopwatch watch = new("Outputting duplicate DAT");
                Writer.Write(dupeData, OutputDir, overwrite: false);
                watch.Stop();
            }

            // Output only DatItems that are not duplicated across inputs
#if NET20 || NET35
            if ((updateMode & UpdateMode.DiffNoDupesOnly) != 0)
#else
            if (updateMode.HasFlag(UpdateMode.DiffNoDupesOnly))
#endif
            {
                DatFile outerDiffData = Diffing.NoDuplicates(userInputDat, inputPaths);
                //DatFile outerDiffData = Diffing.NoDuplicatesDB(userInputDat, inputPaths);

                InternalStopwatch watch = new("Outputting no duplicate DAT");
                Writer.Write(outerDiffData, OutputDir, overwrite: false);
                watch.Stop();
            }

            // Output only DatItems that are unique to each input
#if NET20 || NET35
            if ((updateMode & UpdateMode.DiffIndividualsOnly) != 0)
#else
            if (updateMode.HasFlag(UpdateMode.DiffIndividualsOnly))
#endif
            {
                // Get all of the output DatFiles
                List<DatFile> datFiles = Diffing.Individuals(userInputDat, inputPaths);
                //List<DatFile> datFiles = Diffing.IndividualsDB(userInputDat, inputPaths);

                // Loop through and output the new DatFiles
                InternalStopwatch watch = new("Outputting all individual DATs");

#if NET452_OR_GREATER || NETCOREAPP
                Parallel.For(0, inputPaths.Count, Core.Globals.ParallelOptions, j =>
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
#if NET20 || NET35
            if ((updateMode & UpdateMode.DiffCascade) != 0)
#else
            if (updateMode.HasFlag(UpdateMode.DiffCascade))
#endif
            {
                // Preprocess the DatHeaders
#if NET452_OR_GREATER || NETCOREAPP
                Parallel.For(0, datHeaders.Count, Core.Globals.ParallelOptions, j =>
#elif NET40_OR_GREATER
                Parallel.For(0, datHeaders.Count, j =>
#else
                for (int j = 0; j < datHeaders.Count; j++)
#endif
                {
                    // Skip renaming if not outputting to the runtime folder
                    if (GetBoolean(features, InplaceValue) || OutputDir != Environment.CurrentDirectory)
#if NET40_OR_GREATER || NETCOREAPP
                        return;
#else
                        continue;
#endif

                    // Update the naming for the header
                    string innerpost = $" ({j} - {inputPaths[j].GetNormalizedFileName(true)} Only)";
                    datHeaders[j] = userInputDat.Header;
                    datHeaders[j].SetFieldValue<string?>(DatHeader.FileNameKey, datHeaders[j].GetStringFieldValue(DatHeader.FileNameKey) + innerpost);
                    datHeaders[j].SetFieldValue<string?>(Models.Metadata.Header.NameKey, datHeaders[j].GetStringFieldValue(Models.Metadata.Header.NameKey) + innerpost);
                    datHeaders[j].SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, datHeaders[j].GetStringFieldValue(Models.Metadata.Header.DescriptionKey) + innerpost);
#if NET40_OR_GREATER || NETCOREAPP
                });
#else
                }
#endif

                // Get all of the output DatFiles
                List<DatFile> datFiles = Diffing.Cascade(userInputDat, datHeaders);

                // Loop through and output the new DatFiles
                InternalStopwatch watch = new("Outputting all created DATs");

                int startIndex = GetBoolean(features, SkipFirstOutputValue) ? 1 : 0;
#if NET452_OR_GREATER || NETCOREAPP
                Parallel.For(startIndex, inputPaths.Count, Core.Globals.ParallelOptions, j =>
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
#if NET20 || NET35
            if ((updateMode & UpdateMode.DiffAgainst) != 0)
#else
            if (updateMode.HasFlag(UpdateMode.DiffAgainst))
#endif
            {
                // Loop through each input and diff against the base
#if NET452_OR_GREATER || NETCOREAPP
                Parallel.ForEach(inputPaths, Core.Globals.ParallelOptions, inputPath =>
#elif NET40_OR_GREATER
                Parallel.ForEach(inputPaths, inputPath =>
#else
                foreach (var inputPath in inputPaths)
#endif
                {
                    // Parse the path to a new DatFile
                    DatFile repDat = Parser.CreateDatFile(Header!, Modifiers);
                    Parser.ParseInto(repDat, inputPath.CurrentPath, indexId: 1, keep: true);

                    // Perform additional processing steps
                    AdditionalProcessing(repDat);

                    // Now replace the fields from the base DatFile
                    Diffing.Against(userInputDat, repDat, GetBoolean(Features, ByGameValue));

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
#if NET20 || NET35
            if ((updateMode & UpdateMode.BaseReplace) != 0)
#else
            if (updateMode.HasFlag(UpdateMode.BaseReplace))
#endif
            {
                // Loop through each input and apply the base DatFile
#if NET452_OR_GREATER || NETCOREAPP
                Parallel.ForEach(inputPaths, Core.Globals.ParallelOptions, inputPath =>
#elif NET40_OR_GREATER
                Parallel.ForEach(inputPaths, inputPath =>
#else
                foreach (var inputPath in inputPaths)
#endif
                {
                    // Parse the path to a new DatFile
                    DatFile repDat = Parser.CreateDatFile(Header!, Modifiers);
                    Parser.ParseInto(repDat, inputPath.CurrentPath, indexId: 1, keep: true);

                    // Perform additional processing steps
                    AdditionalProcessing(repDat);

                    // Now replace the fields from the base DatFile
                    Replacer.BaseReplace(
                    //Replacer.BaseReplaceDB(
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
#if NET20 || NET35
            if ((updateMode & UpdateMode.Merge) != 0)
#else
            if (updateMode.HasFlag(UpdateMode.Merge))
#endif
            {
                // If we're in SuperDAT mode, prefix all games with their respective DATs
                if (string.Equals(userInputDat.Header.GetStringFieldValue(Models.Metadata.Header.TypeKey), "SuperDAT", StringComparison.OrdinalIgnoreCase))
                {
                    MergeSplit.ApplySuperDAT(userInputDat, inputPaths);
                    //MergeSplit.ApplySuperDATDB(userInputDat, inputPaths);
                }

                Writer.Write(userInputDat, OutputDir);
            }

            return true;
        }

        /// <summary>
        /// Set default header values for non-specialized update types
        /// </summary>
        /// <param name="updateMode">Update mode that is currently being run</param>
        /// <param name="noAutomaticDate">True if date should be omitted from the description, false otherwise</param>
        private void SetDefaultHeaderValues(UpdateMode updateMode, bool noAutomaticDate)
        {
            // Skip running if a required object is null
            if (Header == null || Cleaner == null)
                return;

            // Skip running for diff against and base replacement
#if NET20 || NET35
            if ((updateMode & UpdateMode.DiffAgainst) != 0)
                return;
            if ((updateMode & UpdateMode.BaseReplace) != 0)
                return;
#else
            if (updateMode.HasFlag(UpdateMode.DiffAgainst))
                return;
            if (updateMode.HasFlag(UpdateMode.BaseReplace))
                return;
#endif

            // Date
            if (string.IsNullOrEmpty(Header.GetStringFieldValue(Models.Metadata.Header.DateKey)))
                Header.SetFieldValue<string?>(Models.Metadata.Header.DateKey, DateTime.Now.ToString("yyyy-MM-dd"));

            // Name
            if (string.IsNullOrEmpty(Header.GetStringFieldValue(Models.Metadata.Header.NameKey)))
            {
                Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, (updateMode != 0 ? "DiffDAT" : "MergeDAT")
                    + (Header.GetStringFieldValue(Models.Metadata.Header.TypeKey) == "SuperDAT" ? "-SuperDAT" : string.Empty)
                    + (Cleaner.DedupeRoms != DedupeType.None ? "-deduped" : string.Empty));
            }

            // Description
            if (string.IsNullOrEmpty(Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey)))
            {
                Header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, (updateMode != 0 ? "DiffDAT" : "MergeDAT")
                    + (Header.GetStringFieldValue(Models.Metadata.Header.TypeKey) == "SuperDAT" ? "-SuperDAT" : string.Empty)
                    + (Cleaner!.DedupeRoms != DedupeType.None ? " - deduped" : string.Empty));

                if (!noAutomaticDate)
                    Header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, $"{Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey)} ({Header.GetStringFieldValue(Models.Metadata.Header.DateKey)})");
            }

            // Category
            if (string.IsNullOrEmpty(Header.GetStringFieldValue(Models.Metadata.Header.CategoryKey)) && updateMode != 0)
                Header.SetFieldValue<string?>(Models.Metadata.Header.CategoryKey, "DiffDAT");

            // Author
            if (string.IsNullOrEmpty(Header.GetStringFieldValue(Models.Metadata.Header.AuthorKey)))
                Header.SetFieldValue<string?>(Models.Metadata.Header.AuthorKey, $"SabreTools {Globals.Version}");

            // Comment
            if (string.IsNullOrEmpty(Header.GetStringFieldValue(Models.Metadata.Header.CommentKey)))
                Header.SetFieldValue<string?>(Models.Metadata.Header.CommentKey, $"Generated by SabreTools {Globals.Version}");
        }

        /// <summary>
        /// Perform standard processing and cleaning
        /// </summary>
        /// <param name="inputPaths">Set of input paths to process</param>
        /// <param name="inplace">True to output to the input folder, false otherwise</param>
        private void StandardUpdate(List<ParentablePath> inputPaths, bool inplace)
        {
            // Loop through each input and update
#if NET452_OR_GREATER || NETCOREAPP
            Parallel.ForEach(inputPaths, Core.Globals.ParallelOptions, inputPath =>
#elif NET40_OR_GREATER
            Parallel.ForEach(inputPaths, inputPath =>
#else
            foreach (var inputPath in inputPaths)
#endif
            {
                // Create a new base DatFile
                DatFile datFile = Parser.CreateDatFile(Header!, Modifiers!);
                _logger.User($"Processing '{Path.GetFileName(inputPath.CurrentPath)}'");

                // Check the current format
                DatFormat currentFormat = datFile.Header.GetFieldValue<DatFormat>(DatHeader.DatFormatKey);
#if NET20 || NET35
                    bool isSeparatedFile = (currentFormat & DatFormat.CSV) != 0
                        || (currentFormat & DatFormat.SSV) != 0
                        || (currentFormat & DatFormat.TSV) != 0;
#else
                bool isSeparatedFile = currentFormat.HasFlag(DatFormat.CSV)
                    || currentFormat.HasFlag(DatFormat.SSV)
                    || currentFormat.HasFlag(DatFormat.TSV);
#endif

                Parser.ParseInto(datFile, inputPath.CurrentPath, keep: true, keepext: isSeparatedFile);

                // Perform additional processing steps
                AdditionalProcessing(datFile);

                // Get the correct output path
                string realOutDir = inputPath.GetOutputPath(OutputDir, inplace)!;

                // Try to output the file, overwriting only if it's not in the current directory
                Writer.Write(datFile, realOutDir, overwrite: inplace);
#if NET40_OR_GREATER || NETCOREAPP
            });
#else
            }
#endif
        }

        /// <summary>
        /// Get the DatHeader values appopriate for the update mode
        /// </summary>
        /// <param name="updateMode">Update mode that is currently being run</param>
        /// <param name="inputPaths">Set of input paths</param>
        /// <param name="basePaths">Set of base paths</param>
        /// <param name="userInputDat">DatFile to parse into</param>
        /// <returns>List of DatHeader values representing the parsed files</returns>
        private static List<DatHeader> GetDatHeaders(UpdateMode updateMode, List<ParentablePath> inputPaths, List<ParentablePath> basePaths, DatFile userInputDat)
        {
#if NET20 || NET35
            if ((updateMode & UpdateMode.DiffAgainst) != 0 || (updateMode & UpdateMode.BaseReplace) != 0)
#else
            if (updateMode.HasFlag(UpdateMode.DiffAgainst) || updateMode.HasFlag(UpdateMode.BaseReplace))
#endif
                return Parser.PopulateUserData(userInputDat, basePaths);
            else
                return Parser.PopulateUserData(userInputDat, inputPaths);
        }

        /// <summary>
        /// Perform additional processing on a given DatFile
        /// </summary>
        /// <param name="datFile">DatFile to process</param>
        private void AdditionalProcessing(DatFile datFile)
        {
            Extras!.ApplyExtras(datFile);
            Extras!.ApplyExtrasDB(datFile);
            Splitter!.ApplySplitting(datFile, useTags: false);
            datFile.ExecuteFilters(FilterRunner!);
            Cleaner!.ApplyCleaning(datFile);
            Remover!.ApplyRemovals(datFile);
        }
    }
}
