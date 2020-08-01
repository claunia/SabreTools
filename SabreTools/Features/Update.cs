using System;
using System.Collections.Generic;

using SabreTools.Library.Data;
using SabreTools.Library.DatFiles;
using SabreTools.Library.Help;

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
            _featureType = FeatureType.Flag;
            LongDescription = "This is the multitool part of the program, allowing for almost every manipulation to a DAT, or set of DATs. This is also a combination of many different programs that performed DAT manipulation that work better together.";
            Features = new Dictionary<string, Feature>();

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
            AddFilteringFeatures();
            AddFeature(OutputDirStringInput);
            AddFeature(InplaceFlag);
            AddFeature(ThreadsInt32Input);
        }

        public override void ProcessFeatures(Dictionary<string, Feature> features)
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
                        + (Header.DedupeRoms != DedupeType.None ? "-deduped" : string.Empty);
                }

                if (string.IsNullOrWhiteSpace(Header.Description))
                {
                    Header.Description = (updateMode != 0 ? "DiffDAT" : "MergeDAT")
                        + (Header.Type == "SuperDAT" ? "-SuperDAT" : string.Empty)
                        + (Header.DedupeRoms != DedupeType.None ? " - deduped" : string.Empty);

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
                updateFields = new List<Field>() { Field.Name };

            // Populate the DatData object
            DatFile userInputDat = DatFile.Create(Header);

            userInputDat.DetermineUpdateType(
                Inputs,
                GetList(features, BaseDatListValue),
                OutputDir,
                updateMode,
                GetBoolean(features, InplaceValue),
                GetBoolean(features, SkipFirstOutputValue),
                Filter,
                updateFields,
                GetBoolean(features, OnlySameValue));
        }
    }
}
