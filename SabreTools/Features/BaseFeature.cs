using System;
using System.Collections.Generic;

using SabreTools.Library.Data;
using SabreTools.Library.DatFiles;
using SabreTools.Library.DatItems;
using SabreTools.Library.FileTypes;
using SabreTools.Library.Filtering;
using SabreTools.Library.Help;
using SabreTools.Library.Reports;
using SabreTools.Library.Tools;

namespace SabreTools.Features
{
    internal class BaseFeature : TopLevel
    {
        #region Enums

        /// <summary>
        /// Determines how the DAT will be split on output
        /// </summary>
        [Flags]
        public enum SplittingMode
        {
            None = 0x00,

            Extension = 1 << 0,
            Hash = 1 << 2,
            Level = 1 << 3,
            Type = 1 << 4,
            Size = 1 << 5,
        }

        /// <summary>
        /// Determines special update modes
        /// </summary>
        [Flags]
        public enum UpdateMode
        {
            None = 0x00,

            // Standard diffs
            DiffDupesOnly = 1 << 0,
            DiffNoDupesOnly = 1 << 1,
            DiffIndividualsOnly = 1 << 2,

            // Cascaded diffs
            DiffCascade = 1 << 3,
            DiffReverseCascade = 1 << 4,

            // Base diffs
            DiffAgainst = 1 << 5,

            // Special update modes
            Merge = 1 << 6,
            BaseReplace = 1 << 7,
            ReverseBaseReplace = 1 << 8,

            // Combinations
            AllDiffs = DiffDupesOnly | DiffNoDupesOnly | DiffIndividualsOnly,
        }

        #endregion

        #region Features

        #region Flag features

        internal const string AaruFormatsAsFilesValue = "aaruformats-as-files";
        internal static Library.Help.Feature AaruFormatsAsFilesFlag
        {
            get
            {
                return new Library.Help.Feature(
                    AaruFormatsAsFilesValue,
                    new List<string>() { "-caf", "--aaruformats-as-files" },
                    "Treat AaruFormats as files",
                    Library.Help.FeatureType.Flag,
                    longDescription: "Normally, AaruFormats would be processed using their internal hash to compare against the input DATs. This flag forces all AaruFormats to be treated like regular files.");
            }
        }

        internal const string AddBlankFilesValue = "add-blank-files";
        internal static Library.Help.Feature AddBlankFilesFlag
        {
            get
            {
                return new Library.Help.Feature(
                    AddBlankFilesValue,
                    new List<string>() { "-ab", "--add-blank-files" },
                    "Output blank files for folders",
                    Library.Help.FeatureType.Flag,
                    longDescription: "If this flag is set, then blank entries will be created for each of the empty directories in the source. This is useful for tools that require all folders be accounted for in the output DAT.");
            }
        }

        internal const string AddDateValue = "add-date";
        internal static Library.Help.Feature AddDateFlag
        {
            get
            {
                return new Library.Help.Feature(
                    AddDateValue,
                    new List<string>() { "-ad", "--add-date" },
                    "Add dates to items, where possible",
                    Library.Help.FeatureType.Flag,
                    longDescription: "If this flag is set, then the Date will be appended to each file information in the output DAT. The output format is standardized as \"yyyy/MM/dd HH:mm:ss\".");
            }
        }

        internal const string ArchivesAsFilesValue = "archives-as-files";
        internal static Library.Help.Feature ArchivesAsFilesFlag
        {
            get
            {
                return new Library.Help.Feature(
                    ArchivesAsFilesValue,
                    new List<string>() { "-aaf", "--archives-as-files" },
                    "Treat archives as files",
                    Library.Help.FeatureType.Flag,
                    longDescription: "Instead of trying to enumerate the files within archives, treat the archives as files themselves. This is good for uncompressed sets that include archives that should be read as-is.");
            }
        }

        internal const string BaddumpColumnValue = "baddump-column";
        internal static Library.Help.Feature BaddumpColumnFlag
        {
            get
            {
                return new Library.Help.Feature(
                    BaddumpColumnValue,
                    new List<string>() { "-bc", "--baddump-column" },
                    "Add baddump stats to output",
                    Library.Help.FeatureType.Flag,
                    longDescription: "Add a new column or field for counting the number of baddumps in the DAT.");
            }
        }

        internal const string BaseValue = "base";
        internal static Library.Help.Feature BaseFlag
        {
            get
            {
                return new Library.Help.Feature(
                    BaseValue,
                    new List<string>() { "-ba", "--base" },
                    "Use source DAT as base name for outputs",
                    Library.Help.FeatureType.Flag,
                    longDescription: "If splitting an entire folder of DATs, some output files may be normally overwritten since the names would be the same. With this flag, the original DAT name is used in the output name, in the format of \"Original Name(Dir - Name)\". This can be used in conjunction with --short to output in the format of \"Original Name (Name)\" instead.");
            }
        }

        internal const string BaseReplaceValue = "base-replace";
        internal static Library.Help.Feature BaseReplaceFlag
        {
            get
            {
                return new Library.Help.Feature(
                    BaseReplaceValue,
                    new List<string>() { "-br", "--base-replace" },
                    "Replace from base DATs in order",
                    Library.Help.FeatureType.Flag,
                    longDescription: "By default, no item names are changed except when there is a merge occurring. This flag enables users to define a DAT or set of base DATs to use as \"replacements\" for all input DATs. Note that the first found instance of an item in the base DAT(s) will be used and all others will be discarded. If no additional flag is given, it will default to updating names.");
            }
        }

        internal const string ByGameValue = "by-game";
        internal static Library.Help.Feature ByGameFlag
        {
            get
            {
                return new Library.Help.Feature(
                    ByGameValue,
                    new List<string>() { "-bg", "--by-game" },
                    "Diff against by game instead of hashes",
                    Library.Help.FeatureType.Flag,
                    longDescription: "By default, diffing against uses hashes to determine similar files. This flag enables using using each game as a comparision point instead.");
            }
        }

        internal const string ChdsAsFilesValue = "chds-as-files";
        internal static Library.Help.Feature ChdsAsFilesFlag
        {
            get
            {
                return new Library.Help.Feature(
                    ChdsAsFilesValue,
                    new List<string>() { "-ic", "--chds-as-files" },
                    "Treat CHDs as regular files",
                    Library.Help.FeatureType.Flag,
                    longDescription: "Normally, CHDs would be processed using their internal hash to compare against the input DATs. This flag forces all CHDs to be treated like regular files.");
            }
        }

        internal const string CleanValue = "clean";
        internal static Library.Help.Feature CleanFlag
        {
            get
            {
                return new Library.Help.Feature(
                    CleanValue,
                    new List<string>() { "-clean", "--clean" },
                    "Clean game names according to WoD standards",
                    Library.Help.FeatureType.Flag,
                    longDescription: "Game names will be sanitized to remove what the original WoD standards deemed as unneeded information, such as parenthesized or bracketed strings.");
            }
        }

        internal const string CopyFilesValue = "copy-files";
        internal static Library.Help.Feature CopyFilesFlag
        {
            get
            {
                return new Library.Help.Feature(
                    CopyFilesValue,
                    new List<string>() { "-cf", "--copy-files" },
                    "Copy files to the temp directory before parsing",
                    Library.Help.FeatureType.Flag,
                    longDescription: "If this flag is set, then all files that are going to be parsed are moved to the temporary directory before being hashed. This can be helpful in cases where the temp folder is located on an SSD and the user wants to take advantage of this.");
            }
        }

        internal const string DatDeviceNonMergedValue = "dat-device-non-merged";
        internal static Library.Help.Feature DatDeviceNonMergedFlag
        {
            get
            {
                return new Library.Help.Feature(
                    DatDeviceNonMergedValue,
                    new List<string>() { "-dnd", "--dat-device-non-merged" },
                    "Create device non-merged sets",
                    Library.Help.FeatureType.Flag,
                    longDescription: "Preprocess the DAT to have child sets contain all items from the device references. This is incompatible with the other --dat-X flags.");
            }
        }

        internal const string DatFullNonMergedValue = "dat-full-non-merged";
        internal static Library.Help.Feature DatFullNonMergedFlag
        {
            get
            {
                return new Library.Help.Feature(
                    DatFullNonMergedValue,
                    new List<string>() { "-df", "--dat-full-non-merged" },
                    "Create fully non-merged sets",
                    Library.Help.FeatureType.Flag,
                    longDescription: "Preprocess the DAT to have child sets contain all items from the parent sets based on the cloneof and romof tags as well as device references. This is incompatible with the other --dat-X flags.");
            }
        }

        internal const string DatMergedValue = "dat-merged";
        internal static Library.Help.Feature DatMergedFlag
        {
            get
            {
                return new Library.Help.Feature(
                    DatMergedValue,
                    new List<string>() { "-dm", "--dat-merged" },
                    "Force creating merged sets",
                    Library.Help.FeatureType.Flag,
                    longDescription: "Preprocess the DAT to have parent sets contain all items from the children based on the cloneof tag. This is incompatible with the other --dat-X flags.");
            }
        }

        internal const string DatNonMergedValue = "dat-non-merged";
        internal static Library.Help.Feature DatNonMergedFlag
        {
            get
            {
                return new Library.Help.Feature(
                    DatNonMergedValue,
                    new List<string>() { "-dnm", "--dat-non-merged" },
                    "Force creating non-merged sets",
                    Library.Help.FeatureType.Flag,
                    longDescription: "Preprocess the DAT to have child sets contain all items from the parent set based on the romof and cloneof tags. This is incompatible with the other --dat-X flags.");
            }
        }

        internal const string DatSplitValue = "dat-split";
        internal static Library.Help.Feature DatSplitFlag
        {
            get
            {
                return new Library.Help.Feature(
                    DatSplitValue,
                    new List<string>() { "-ds", "--dat-split" },
                    "Force creating split sets",
                    Library.Help.FeatureType.Flag,
                    longDescription: "Preprocess the DAT to remove redundant files between parents and children based on the romof and cloneof tags. This is incompatible with the other --dat-X flags.");
            }
        }

        internal const string DedupValue = "dedup";
        internal static Library.Help.Feature DedupFlag
        {
            get
            {
                return new Library.Help.Feature(
                    DedupValue,
                    new List<string>() { "-dd", "--dedup" },
                    "Enable deduping in the created DAT",
                    Library.Help.FeatureType.Flag,
                    longDescription: "For all outputted DATs, allow for hash deduping. This makes sure that there are effectively no duplicates in the output files. Cannot be used with game dedup.");
            }
        }

        internal const string DeleteValue = "delete";
        internal static Library.Help.Feature DeleteFlag
        {
            get
            {
                return new Library.Help.Feature(
                    DeleteValue,
                    new List<string>() { "-del", "--delete" },
                    "Delete fully rebuilt input files",
                    Library.Help.FeatureType.Flag,
                    longDescription: "Optionally, the input files, once processed and fully matched, can be deleted. This can be useful when the original file structure is no longer needed or if there is limited space on the source drive.");
            }
        }

        internal const string DepotValue = "depot";
        internal static Library.Help.Feature DepotFlag
        {
            get
            {
                return new Library.Help.Feature(
                    DepotValue,
                    new List<string>() { "-dep", "--depot" },
                    "Assume directories are Romba depots",
                    Library.Help.FeatureType.Flag,
                    longDescription: "Normally, input directories will be treated with no special format. If this flag is used, all input directories will be assumed to be Romba-style depots.");
            }
        }

        internal const string DeprecatedValue = "deprecated";
        internal static Library.Help.Feature DeprecatedFlag
        {
            get
            {
                return new Library.Help.Feature(
                    DeprecatedValue,
                    new List<string>() { "-dpc", "--deprecated" },
                    "Output 'game' instead of 'machine'",
                    Library.Help.FeatureType.Flag,
                    longDescription: "By default, Logiqx XML DATs output with the more modern \"machine\" tag for each set. This flag allows users to output the older \"game\" tag instead, for compatibility reasons. [Logiqx only]");
            }
        }

        internal const string DescriptionAsNameValue = "description-as-name";
        internal static Library.Help.Feature DescriptionAsNameFlag
        {
            get
            {
                return new Library.Help.Feature(
                    DescriptionAsNameValue,
                    new List<string>() { "-dan", "--description-as-name" },
                    "Use description instead of machine name",
                    Library.Help.FeatureType.Flag,
                    longDescription: "By default, all DATs are converted exactly as they are input. Enabling this flag allows for the machine names in the DAT to be replaced by the machine description instead. In most cases, this will result in no change in the output DAT, but a notable example would be a software list DAT where the machine names are generally DOS-friendly while the description is more complete.");
            }
        }

        internal const string DiffAgainstValue = "diff-against";
        internal static Library.Help.Feature DiffAgainstFlag
        {
            get
            {
                return new Library.Help.Feature(
                    DiffAgainstValue,
                    new List<string>() { "-dag", "--diff-against" },
                    "Diff all inputs against a set of base DATs",
                    Library.Help.FeatureType.Flag,
                    "This flag will enable a special type of diffing in which a set of base DATs are used as a comparison point for each of the input DATs. This allows users to get a slightly different output to cascaded diffing, which may be more useful in some cases. This is heavily influenced by the diffing model used by Romba.");
            }
        }

        internal const string DiffAllValue = "diff-all";
        internal static Library.Help.Feature DiffAllFlag
        {
            get
            {
                return new Library.Help.Feature(
                    DiffAllValue,
                    new List<string>() { "-di", "--diff-all" },
                    "Create diffdats from inputs (all standard outputs)",
                    Library.Help.FeatureType.Flag,
                    longDescription: "By default, all DATs are processed individually with the user-specified flags. With this flag enabled, input DATs are diffed against each other to find duplicates, no duplicates, and only in individuals.");
            }
        }

        internal const string DiffCascadeValue = "diff-cascade";
        internal static Library.Help.Feature DiffCascadeFlag
        {
            get
            {
                return new Library.Help.Feature(
                    DiffCascadeValue,
                    new List<string>() { "-dc", "--diff-cascade" },
                    "Enable cascaded diffing",
                    Library.Help.FeatureType.Flag,
                    longDescription: "This flag allows for a special type of diffing in which the first DAT is considered a base, and for each additional input DAT, it only leaves the files that are not in one of the previous DATs. This can allow for the creation of rollback sets or even just reduce the amount of duplicates across multiple sets.");
            }
        }

        internal const string DiffDuplicatesValue = "diff-duplicates";
        internal static Library.Help.Feature DiffDuplicatesFlag
        {
            get
            {
                return new Library.Help.Feature(
                    DiffDuplicatesValue,
                    new List<string>() { "-did", "--diff-duplicates" },
                    "Create diffdat containing just duplicates",
                    Library.Help.FeatureType.Flag,
                    longDescription: "All files that have duplicates outside of the original DAT are included.");
            }
        }

        internal const string DiffIndividualsValue = "diff-individuals";
        internal static Library.Help.Feature DiffIndividualsFlag
        {
            get
            {
                return new Library.Help.Feature(
                    DiffIndividualsValue,
                    new List<string>() { "-dii", "--diff-individuals" },
                    "Create diffdats for individual DATs",
                    Library.Help.FeatureType.Flag,
                    longDescription: "All files that have no duplicates outside of the original DATs are put into DATs that are named after the source DAT.");
            }
        }

        internal const string DiffNoDuplicatesValue = "diff-no-duplicates";
        internal static Library.Help.Feature DiffNoDuplicatesFlag
        {
            get
            {
                return new Library.Help.Feature(
                    DiffNoDuplicatesValue,
                    new List<string>() { "-din", "--diff-no-duplicates" },
                    "Create diffdat containing no duplicates",
                    Library.Help.FeatureType.Flag,
                    longDescription: "All files that have no duplicates outside of the original DATs are included.");
            }
        }

        internal const string DiffReverseCascadeValue = "diff-reverse-cascade";
        internal static Library.Help.Feature DiffReverseCascadeFlag
        {
            get
            {
                return new Library.Help.Feature(
                    DiffReverseCascadeValue,
                    new List<string>() { "-drc", "--diff-reverse-cascade" },
                    "Enable reverse cascaded diffing",
                    Library.Help.FeatureType.Flag,
                    longDescription: "This flag allows for a special type of diffing in which the last DAT is considered a base, and for each additional input DAT, it only leaves the files that are not in one of the previous DATs. This can allow for the creation of rollback sets or even just reduce the amount of duplicates across multiple sets.");
            }
        }

        internal const string ExtensionValue = "extension";
        internal static Library.Help.Feature ExtensionFlag
        {
            get
            {
                return new Library.Help.Feature(
                    ExtensionValue,
                    new List<string>() { "-es", "--extension" },
                    "Split DAT(s) by two file extensions",
                    Library.Help.FeatureType.Flag,
                    longDescription: "For a DAT, or set of DATs, allow for splitting based on a list of input extensions. This can allow for combined DAT files, such as those combining two separate systems, to be split. Files with any extensions not listed in the input lists will be included in both outputted DAT files.");
            }
        }

        internal const string GameDedupValue = "game-dedup";
        internal static Library.Help.Feature GameDedupFlag
        {
            get
            {
                return new Library.Help.Feature(
                    GameDedupValue,
                    new List<string>() { "-gdd", "--game-dedup" },
                    "Enable deduping within games in the created DAT",
                    Library.Help.FeatureType.Flag,
                    longDescription: "For all outputted DATs, allow for hash deduping but only within the games, and not across the entire DAT. This makes sure that there are effectively no duplicates within each of the output sets. Cannot be used with standard dedup.");
            }
        }

        internal const string GamePrefixValue = "game-prefix";
        internal static Library.Help.Feature GamePrefixFlag
        {
            get
            {
                return new Library.Help.Feature(
                    GamePrefixValue,
                    new List<string>() { "-gp", "--game-prefix" },
                    "Add game name as a prefix",
                    Library.Help.FeatureType.Flag,
                    longDescription: "This flag allows for the name of the game to be used as a prefix to each file.");
            }
        }

        internal const string HashValue = "hash";
        internal static Library.Help.Feature HashFlag
        {
            get
            {
                return new Library.Help.Feature(
                    HashValue,
                    new List<string>() { "-hs", "--hash" },
                    "Split DAT(s) or folder by best-available hashes",
                    Library.Help.FeatureType.Flag,
                    longDescription: "For a DAT, or set of DATs, allow for splitting based on the best available hash for each file within. The order of preference for the outputted DATs is as follows: Nodump, SHA-512, SHA-384, SHA-256, SHA-1, MD5, CRC (or worse).");
            }
        }

        internal const string HashOnlyValue = "hash-only";
        internal static Library.Help.Feature HashOnlyFlag
        {
            get
            {
                return new Library.Help.Feature(
                    HashOnlyValue,
                    new List<string>() { "-ho", "--hash-only" },
                    "Check files by hash only",
                    Library.Help.FeatureType.Flag,
                    longDescription: "This sets a mode where files are not checked based on name but rather hash alone. This allows verification of (possibly) incorrectly named folders and sets to be verified without worrying about the proper set structure to be there.");
            }
        }

        internal const string IndividualValue = "individual";
        internal static Library.Help.Feature IndividualFlag
        {
            get
            {
                return new Library.Help.Feature(
                    IndividualValue,
                    new List<string>() { "-ind", "--individual" },
                    "Process input DATs individually",
                    Library.Help.FeatureType.Flag,
                    longDescription: "In cases where DATs would be processed in bulk, this flag allows them to be processed on their own instead.");
            }
        }

        internal const string InplaceValue = "inplace";
        internal static Library.Help.Feature InplaceFlag
        {
            get
            {
                return new Library.Help.Feature(
                    InplaceValue,
                    new List<string>() { "-ip", "--inplace" },
                    "Write to the input directories, where possible",
                    Library.Help.FeatureType.Flag,
                    longDescription: "By default, files are written to the runtime directory (or the output directory, if set). This flag enables users to write out to the directory that the DATs originated from.");
            }
        }

        internal const string InverseValue = "inverse";
        internal static Library.Help.Feature InverseFlag
        {
            get
            {
                return new Library.Help.Feature(
                    InverseValue,
                    new List<string>() { "-in", "--inverse" },
                    "Rebuild only files not in DAT",
                    Library.Help.FeatureType.Flag,
                    longDescription: "Instead of the normal behavior of rebuilding using a DAT, this flag allows the user to use the DAT as a filter instead. All files that are found in the DAT will be skipped and everything else will be output in the selected format.");
            }
        }

        internal const string KeepEmptyGamesValue = "keep-empty-games";
        internal static Library.Help.Feature KeepEmptyGamesFlag
        {
            get
            {
                return new Library.Help.Feature(
                    KeepEmptyGamesValue,
                    new List<string>() { "-keg", "--keep-empty-games" },
                    "Keep originally empty sets from the input(s)",
                    Library.Help.FeatureType.Flag,
                    longDescription: "Normally, any sets that are considered empty will not be included in the output, this flag allows these empty sets to be added to the output.");
            }
        }

        internal const string LevelValue = "level";
        internal static Library.Help.Feature LevelFlag
        {
            get
            {
                return new Library.Help.Feature(
                    LevelValue,
                    new List<string>() { "-ls", "--level" },
                    "Split a SuperDAT or folder by lowest available level",
                    Library.Help.FeatureType.Flag,
                    longDescription: "For a DAT, or set of DATs, allow for splitting based on the lowest available level of game name. That is, if a game name is top/mid/last, then it will create an output DAT for the parent directory \"mid\" in a folder called \"top\" with a game called \"last\".");
            }
        }

        internal const string MatchOfTagsValue = "match-of-tags";
        internal static Library.Help.Feature MatchOfTagsFlag
        {
            get
            {
                return new Library.Help.Feature(
                    MatchOfTagsValue,
                    new List<string>() { "-ofg", "--match-of-tags" },
                    "Allow cloneof and romof tags to match game name filters",
                    Library.Help.FeatureType.Flag,
                    longDescription: "If filter or exclude by game name is used, this flag will allow those filters to be checked against the romof and cloneof tags as well. This can allow for more advanced set-building, especially in arcade-based sets.");
            }
        }

        internal const string MergeValue = "merge";
        internal static Library.Help.Feature MergeFlag
        {
            get
            {
                return new Library.Help.Feature(
                    MergeValue,
                    new List<string>() { "-m", "--merge" },
                    "Merge the input DATs",
                    Library.Help.FeatureType.Flag,
                    longDescription: "By default, all DATs are processed individually with the user-specified flags. With this flag enabled, all of the input DATs are merged into a single output. This is best used with the dedup flag.");
            }
        }

        internal const string NoAutomaticDateValue = "no-automatic-date";
        internal static Library.Help.Feature NoAutomaticDateFlag
        {
            get
            {
                return new Library.Help.Feature(
                    NoAutomaticDateValue,
                    new List<string>() { "-b", "--no-automatic-date" },
                    "Don't include date in file name",
                    Library.Help.FeatureType.Flag,
                    longDescription: "Normally, the DAT will be created with the date in the file name in brackets. This flag removes that instead of the default.");
            }
        }

        internal const string NodumpColumnValue = "nodump-column";
        internal static Library.Help.Feature NodumpColumnFlag
        {
            get
            {
                return new Library.Help.Feature(
                    NodumpColumnValue,
                    new List<string>() { "-nc", "--nodump-column" },
                    "Add statistics for nodumps to output",
                    Library.Help.FeatureType.Flag,
                    longDescription: "Add a new column or field for counting the number of nodumps in the DAT.");
            }
        }

        internal const string NoStoreHeaderValue = "no-store-header";
        internal static Library.Help.Feature NoStoreHeaderFlag
        {
            get
            {
                return new Library.Help.Feature(
                    NoStoreHeaderValue,
                    new List<string>() { "-nsh", "--no-store-header" },
                    "Don't store the extracted header",
                    Library.Help.FeatureType.Flag,
                    longDescription: "By default, all headers that are removed from files are backed up in the database. This flag allows users to skip that step entirely, avoiding caching the headers at all.");
            }
        }

        internal const string NotRunnableValue = "not-runnable";
        internal static Library.Help.Feature NotRunnableFlag
        {
            get
            {
                return new Library.Help.Feature(
                    NotRunnableValue,
                    new List<string>() { "-nrun", "--not-runnable" },
                    "Include only items that are not marked runnable",
                    Library.Help.FeatureType.Flag,
                    longDescription: "This allows users to include only unrunnable games.");
            }
        }

        internal const string OneGamePerRegionValue = "one-game-per-region";
        internal static Library.Help.Feature OneGamePerRegionFlag
        {
            get
            {
                return new Library.Help.Feature(
                    OneGamePerRegionValue,
                    new List<string>() { "-1g1r", "--one-game-per-region" },
                    "Try to ensure one game per user-defined region",
                    Library.Help.FeatureType.Flag,
                    longDescription: "This allows users to input a list of regions to use to filter on in order so only one game from each set of parent and clones will be included. This requires either cloneof or romof tags to function properly.");
            }
        }

        internal const string OneRomPerGameValue = "one-rom-per-game";
        internal static Library.Help.Feature OneRomPerGameFlag
        {
            get
            {
                return new Library.Help.Feature(
                    OneRomPerGameValue,
                    new List<string>() { "-orpg", "--one-rom-per-game" },
                    "Try to ensure each rom has its own game",
                    Library.Help.FeatureType.Flag,
                    longDescription: "In some cases, it is beneficial to have every rom put into its own output set as a subfolder of the original parent. This flag enables outputting each rom to its own game for this purpose.");
            }
        }

        internal const string OnlySameValue = "only-same";
        internal static Library.Help.Feature OnlySameFlag
        {
            get
            {
                return new Library.Help.Feature(
                    OnlySameValue,
                    new List<string>() { "-ons", "--only-same" },
                    "Only update description if machine name matches description",
                    Library.Help.FeatureType.Flag,
                    longDescription: "Normally, updating the description will always overwrite if the machine names are the same. With this flag, descriptions will only be overwritten if they are the same as the machine names.");
            }
        }

        internal const string QuickValue = "quick";
        internal static Library.Help.Feature QuickFlag
        {
            get
            {
                return new Library.Help.Feature(
                    QuickValue,
                    new List<string>() { "-qs", "--quick" },
                    "Enable quick scanning of archives",
                    Library.Help.FeatureType.Flag,
                    longDescription: "For all archives, if this flag is enabled, it will only use the header information to get the archive entries' file information. The upside to this is that it is the fastest option. On the downside, it can only get the CRC and size from most archive formats, leading to possible issues.");
            }
        }

        internal const string QuotesValue = "quotes";
        internal static Library.Help.Feature QuotesFlag
        {
            get
            {
                return new Library.Help.Feature(
                    QuotesValue,
                    new List<string>() { "-q", "--quotes" },
                    "Double-quote each item",
                    Library.Help.FeatureType.Flag,
                    longDescription: "This flag surrounds the item by double-quotes, not including the prefix or postfix.");
            }
        }

        internal const string RemoveExtensionsValue = "remove-extensions";
        internal static Library.Help.Feature RemoveExtensionsFlag
        {
            get
            {
                return new Library.Help.Feature(
                    RemoveExtensionsValue,
                    new List<string>() { "-rme", "--remove-extensions" },
                    "Remove all extensions from all items",
                    Library.Help.FeatureType.Flag,
                    longDescription: "For each item, remove the extension.");
            }
        }

        internal const string RemoveUnicodeValue = "remove-unicode";
        internal static Library.Help.Feature RemoveUnicodeFlag
        {
            get
            {
                return new Library.Help.Feature(
                    RemoveUnicodeValue,
                    new List<string>() { "-ru", "--remove-unicode" },
                    "Remove unicode characters from names",
                    Library.Help.FeatureType.Flag,
                    longDescription: "By default, the character set from the original file(s) will be used for item naming. This flag removes all Unicode characters from the item names, machine names, and machine descriptions.");
            }
        }

        internal const string ReverseBaseReplaceValue = "reverse-base-replace";
        internal static Library.Help.Feature ReverseBaseReplaceFlag
        {
            get
            {
                return new Library.Help.Feature(
                    ReverseBaseReplaceValue,
                    new List<string>() { "-rbr", "--reverse-base-replace" },
                    "Replace item names from base DATs in reverse",
                    Library.Help.FeatureType.Flag,
                    longDescription: "By default, no item names are changed except when there is a merge occurring. This flag enables users to define a DAT or set of base DATs to use as \"replacements\" for all input DATs. Note that the first found instance of an item in the last base DAT(s) will be used and all others will be discarded. If no additional flag is given, it will default to updating names.");
            }
        }

        internal const string RombaValue = "romba";
        internal static Library.Help.Feature RombaFlag
        {
            get
            {
                return new Library.Help.Feature(
                    RombaValue,
                    new List<string>() { "-ro", "--romba" },
                    "Treat like a Romba depot (requires SHA-1)",
                    Library.Help.FeatureType.Flag,
                    longDescription: "This flag allows reading and writing of DATs and output files to and from a Romba-style depot. This also implies TorrentGZ input and output for physical files. Where appropriate, Romba depot files will be created as well.");
            }
        }

        internal const string RomsValue = "roms";
        internal static Library.Help.Feature RomsFlag
        {
            get
            {
                return new Library.Help.Feature(
                    RomsValue,
                    new List<string>() { "-r", "--roms" },
                    "Output roms to miss instead of sets",
                    Library.Help.FeatureType.Flag,
                    longDescription: "By default, the outputted file will include the name of the game so this flag allows for the name of the rom to be output instead. [Missfile only]");
            }
        }

        internal const string RunnableValue = "runnable";
        internal static Library.Help.Feature RunnableFlag
        {
            get
            {
                return new Library.Help.Feature(
                    RunnableValue,
                    new List<string>() { "-run", "--runnable" },
                    "Include only items that are marked runnable",
                    Library.Help.FeatureType.Flag,
                    longDescription: "This allows users to include only verified runnable games.");
            }
        }

        internal const string SceneDateStripValue = "scene-date-strip";
        internal static Library.Help.Feature SceneDateStripFlag
        {
            get
            {
                return new Library.Help.Feature(
                    SceneDateStripValue,
                    new List<string>() { "-sds", "--scene-date-strip" },
                    "Remove date from scene-named sets",
                    Library.Help.FeatureType.Flag,
                    longDescription: "If this flag is enabled, sets with \"scene\" names will have the date removed from the beginning. For example \"01.01.01-Game_Name-GROUP\" would become \"Game_Name-Group\".");
            }
        }

        internal const string ShortValue = "short";
        internal static Library.Help.Feature ShortFlag
        {
            get
            {
                return new Library.Help.Feature(
                    ShortValue,
                    new List<string>() { "-s", "--short" },
                    "Use short output names",
                    Library.Help.FeatureType.Flag,
                    longDescription: "Instead of using ClrMamePro-style long names for DATs, use just the name of the folder as the name of the DAT. This can be used in conjunction with --base to output in the format of \"Original Name (Name)\" instead.");
            }
        }

        internal const string SingleSetValue = "single-set";
        internal static Library.Help.Feature SingleSetFlag
        {
            get
            {
                return new Library.Help.Feature(
                    SingleSetValue,
                    new List<string>() { "-si", "--single-set" },
                    "All game names replaced by '!'",
                    Library.Help.FeatureType.Flag,
                    longDescription: "This is useful for keeping all roms in a DAT in the same archive or folder.");
            }
        }

        internal const string SizeValue = "size";
        internal static Library.Help.Feature SizeFlag
        {
            get
            {
                return new Library.Help.Feature(
                    SizeValue,
                    new List<string>() { "-szs", "--size" },
                    "Split DAT(s) or folder by file sizes",
                    Library.Help.FeatureType.Flag,
                    longDescription: "For a DAT, or set of DATs, allow for splitting based on the sizes of the files, specifically if the type is a Rom (most item types don't have sizes).");
            }
        }

        internal const string SkipArchivesValue = "skip-archives";
        internal static Library.Help.Feature SkipArchivesFlag
        {
            get
            {
                return new Library.Help.Feature(
                    SkipArchivesValue,
                    new List<string>() { "-ska", "--skip-archives" },
                    "Skip all archives",
                    Library.Help.FeatureType.Flag,
                    longDescription: "Skip any files that are treated like archives");
            }
        }

        internal const string SkipFilesValue = "skip-files";
        internal static Library.Help.Feature SkipFilesFlag
        {
            get
            {
                return new Library.Help.Feature(
                    SkipFilesValue,
                    new List<string>() { "-skf", "--skip-files" },
                    "Skip all non-archives",
                    Library.Help.FeatureType.Flag,
                    longDescription: "Skip any files that are not treated like archives");
            }
        }

        internal const string SkipFirstOutputValue = "skip-first-output";
        internal static Library.Help.Feature SkipFirstOutputFlag
        {
            get
            {
                return new Library.Help.Feature(
                    SkipFirstOutputValue,
                    new List<string>() { "-sf", "--skip-first-output" },
                    "Skip output of first DAT",
                    Library.Help.FeatureType.Flag,
                    longDescription: "In times where the first DAT does not need to be written out a second time, this will skip writing it. This can often speed up the output process.");
            }
        }

        internal const string SkipMd5Value = "skip-md5";
        internal static Library.Help.Feature SkipMd5Flag
        {
            get
            {
                return new Library.Help.Feature(
                    SkipMd5Value,
                    new List<string>() { "-nm", "--skip-md5" },
                    "Don't include MD5 in output",
                    Library.Help.FeatureType.Flag,
                    longDescription: "This allows the user to skip calculating the MD5 for each of the files which will speed up the creation of the DAT.");
            }
        }

#if NET_FRAMEWORK
        internal const string SkipRipeMd160Value = "skip-ripemd160";
        internal static Library.Help.Feature SkipRipeMd160Flag
        {
            get
            {
                return new Library.Help.Feature(
                    SkipRipeMd160Value,
                    new List<string>() { "-nr160", "--skip-ripemd160" },
                    "Include RIPEMD160 in output", // TODO: This needs to be inverted later
                    Library.Help.FeatureType.Flag,
                    longDescription: "This allows the user to include calculating the RIPEMD160 for each of the files.");
            }
        }
#endif

        internal const string SkipSha1Value = "skip-sha1";
        internal static Library.Help.Feature SkipSha1Flag
        {
            get
            {
                return new Library.Help.Feature(
                    SkipSha1Value,
                    new List<string>() { "-ns", "--skip-sha1" },
                    "Don't include SHA-1 in output",
                    Library.Help.FeatureType.Flag,
                    longDescription: "This allows the user to skip calculating the SHA-1 for each of the files which will speed up the creation of the DAT.");
            }
        }

        internal const string SkipSha256Value = "skip-sha256";
        internal static Library.Help.Feature SkipSha256Flag
        {
            get
            {
                return new Library.Help.Feature(
                    SkipSha256Value,
                    new List<string>() { "-ns256", "--skip-sha256" },
                    "Include SHA-256 in output", // TODO: This needs to be inverted later
                    Library.Help.FeatureType.Flag,
                    longDescription: "This allows the user to include calculating the SHA-256 for each of the files.");
            }
        }

        internal const string SkipSha384Value = "skip-sha384";
        internal static Library.Help.Feature SkipSha384Flag
        {
            get
            {
                return new Library.Help.Feature(
                    SkipSha384Value,
                    new List<string>() { "-ns384", "--skip-sha384" },
                    "Include SHA-384 in output", // TODO: This needs to be inverted later
                    Library.Help.FeatureType.Flag,
                    longDescription: "This allows the user to include calculating the SHA-384 for each of the files.");
            }
        }

        internal const string SkipSha512Value = "skip-sha512";
        internal static Library.Help.Feature SkipSha512Flag
        {
            get
            {
                return new Library.Help.Feature(
                    SkipSha512Value,
                    new List<string>() { "-ns512", "--skip-sha512" },
                    "Include SHA-512 in output", // TODO: This needs to be inverted later
                    Library.Help.FeatureType.Flag,
                    longDescription: "This allows the user to include calculating the SHA-512 for each of the files.");
            }
        }

        internal const string SkipSpamSumValue = "skip-spamsum";
        internal static Library.Help.Feature SkipSpamSumFlag
        {
            get
            {
                return new Library.Help.Feature(
                    SkipSpamSumValue,
                    new List<string>() { "-nss", "--skip-spamsum" },
                    "Include SpamSum in output", // TODO: This needs to be inverted later
                    Library.Help.FeatureType.Flag,
                    longDescription: "This allows the user to include calculating the SpamSum for each of the files.");
            }
        }

        internal const string SuperdatValue = "superdat";
        internal static Library.Help.Feature SuperdatFlag
        {
            get
            {
                return new Library.Help.Feature(
                    SuperdatValue,
                    new List<string>() { "-sd", "--superdat" },
                    "Enable SuperDAT creation",
                    Library.Help.FeatureType.Flag,
                    longDescription: "Set the type flag to \"SuperDAT\" for the output DAT as well as preserving the directory structure of the inputted folder, if applicable.");
            }
        }

        internal const string TarValue = "tar";
        internal static Library.Help.Feature TarFlag
        {
            get
            {
                return new Library.Help.Feature(
                    TarValue,
                    new List<string>() { "-tar", "--tar" },
                    "Enable Tape ARchive output",
                    Library.Help.FeatureType.Flag,
                    longDescription: "Instead of outputting the files to folder, files will be rebuilt to Tape ARchive (TAR) files. This format is a standardized storage archive without any compression, usually used with other compression formats around it. It is widely used in backup applications and source code archives.");
            }
        }

        internal const string Torrent7zipValue = "torrent-7zip";
        internal static Library.Help.Feature Torrent7zipFlag
        {
            get
            {
                return new Library.Help.Feature(
                    Torrent7zipValue,
                    new List<string>() { "-t7z", "--torrent-7zip" },
                    "Enable Torrent7Zip output",
                    Library.Help.FeatureType.Flag,
                    longDescription: "Instead of outputting the files to folder, files will be rebuilt to Torrent7Zip (T7Z) files. This format is based on the LZMA container format 7Zip, but with custom header information. This is currently unused by any major application. Currently does not produce proper Torrent-compatible outputs.");
            }
        }

        internal const string TorrentGzipValue = "torrent-gzip";
        internal static Library.Help.Feature TorrentGzipFlag
        {
            get
            {
                return new Library.Help.Feature(
                    TorrentGzipValue,
                    new List<string>() { "-tgz", "--torrent-gzip" },
                    "Enable Torrent GZip output",
                    Library.Help.FeatureType.Flag,
                    longDescription: "Instead of outputting the files to folder, files will be rebuilt to TorrentGZ (TGZ) files. This format is based on the GZip archive format, but with custom header information and a file name replaced by the SHA-1 of the file inside. This is primarily used by external tool Romba (https://github.com/uwedeportivo/romba), but may be used more widely in the future.");
            }
        }

        internal const string TorrentZipValue = "torrent-zip";
        internal static Library.Help.Feature TorrentZipFlag
        {
            get
            {
                return new Library.Help.Feature(
                    TorrentZipValue,
                    new List<string>() { "-tzip", "--torrent-zip" },
                    "Enable Torrent Zip output",
                    Library.Help.FeatureType.Flag,
                    longDescription: "Instead of outputting files to folder, files will be rebuilt to TorrentZip (TZip) files. This format is based on the ZIP archive format, but with custom header information. This is primarily used by external tool RomVault (http://www.romvault.com/) and is already widely used.");
            }
        }

        internal const string TrimValue = "trim";
        internal static Library.Help.Feature TrimFlag
        {
            get
            {
                return new Library.Help.Feature(
                    TrimValue,
                    new List<string>() { "-trim", "--trim" },
                    "Trim file names to fit NTFS length",
                    Library.Help.FeatureType.Flag,
                    longDescription: "In the cases where files will have too long a name, this allows for trimming the name of the files to the NTFS maximum length at most.");
            }
        }

        internal const string TypeValue = "type";
        internal static Library.Help.Feature TypeFlag
        {
            get
            {
                return new Library.Help.Feature(
                    TypeValue,
                    new List<string>() { "-ts", "--type" },
                    "Split DAT(s) or folder by file types (rom/disk)",
                    Library.Help.FeatureType.Flag,
                    longDescription: "For a DAT, or set of DATs, allow for splitting based on the types of the files, specifically if the type is a rom or a disk.");
            }
        }

        internal const string UpdateDatValue = "update-dat";
        internal static Library.Help.Feature UpdateDatFlag
        {
            get
            {
                return new Library.Help.Feature(
                    UpdateDatValue,
                    new List<string>() { "-ud", "--update-dat" },
                    "Output updated DAT to output directory",
                    Library.Help.FeatureType.Flag,
                    longDescription: "Once the files that were able to rebuilt are taken care of, a DAT of the files that could not be matched will be output to the output directory.");
            }
        }

        internal const string UpdateDescriptionValue = "update-description";
        internal static Library.Help.Feature UpdateDescriptionFlag
        {
            get
            {
                return new Library.Help.Feature(
                    UpdateDescriptionValue,
                    new List<string>() { "-udd", "--update-description" },
                    "Update machine descriptions from base DATs",
                    Library.Help.FeatureType.Flag,
                    longDescription: "This flag enables updating of machine descriptions from base DATs.");
            }
        }

        internal const string UpdateGameTypeValue = "update-game-type";
        internal static Library.Help.Feature UpdateGameTypeFlag
        {
            get
            {
                return new Library.Help.Feature(
                    UpdateGameTypeValue,
                    new List<string>() { "-ugt", "--update-game-type" },
                    "Update machine type from base DATs",
                    Library.Help.FeatureType.Flag,
                    longDescription: "This flag enables updating of machine type from base DATs.");
            }
        }

        internal const string UpdateHashesValue = "update-hashes";
        internal static Library.Help.Feature UpdateHashesFlag
        {
            get
            {
                return new Library.Help.Feature(
                    UpdateHashesValue,
                    new List<string>() { "-uh", "--update-hashes" },
                    "Update hashes from base DATs",
                    Library.Help.FeatureType.Flag,
                    longDescription: "This flag enables updating of hashes from base DATs.");
            }
        }

        internal const string UpdateManufacturerValue = "update-manufacturer";
        internal static Library.Help.Feature UpdateManufacturerFlag
        {
            get
            {
                return new Library.Help.Feature(
                    UpdateManufacturerValue,
                    new List<string>() { "-um", "--update-manufacturer" },
                    "Update machine manufacturers from base DATs",
                    Library.Help.FeatureType.Flag,
                    longDescription: "This flag enables updating of machine manufacturers from base DATs.");
            }
        }

        internal const string UpdateNamesValue = "update-names";
        internal static Library.Help.Feature UpdateNamesFlag
        {
            get
            {
                return new Library.Help.Feature(
                    UpdateNamesValue,
                    new List<string>() { "-un", "--update-names" },
                    "Update item names from base DATs",
                    Library.Help.FeatureType.Flag,
                    longDescription: "This flag enables updating of item names from base DATs.");
            }
        }

        internal const string UpdateParentsValue = "update-parents";
        internal static Library.Help.Feature UpdateParentsFlag
        {
            get
            {
                return new Library.Help.Feature(
                    UpdateParentsValue,
                    new List<string>() { "-up", "--update-parents" },
                    "Update machine parents from base DATs",
                    Library.Help.FeatureType.Flag,
                    longDescription: "This flag enables updating of machine parents (romof, cloneof, sampleof) from base DATs.");
            }
        }

        internal const string UpdateYearValue = "update-year";
        internal static Library.Help.Feature UpdateYearFlag
        {
            get
            {
                return new Library.Help.Feature(
                    UpdateYearValue,
                    new List<string>() { "-uy", "--update-year" },
                    "Update machine years from base DATs",
                    Library.Help.FeatureType.Flag,
                    longDescription: "This flag enables updating of machine years from base DATs.");
            }
        }

        #endregion

        #region Int32 features

        internal const string DepotDepthInt32Value = "depot-depth";
        internal static Library.Help.Feature DepotDepthInt32Input
        {
            get
            {
                return new Library.Help.Feature(
                    DepotDepthInt32Value,
                    new List<string>() { "-depd", "--depot-depth" },
                    "Set depth of depot for inputs",
                    Library.Help.FeatureType.Int32,
                    longDescription: "Optionally, set the depth of input depots. Defaults to 4 deep otherwise.");
            }
        }

        internal const string RombaDepthInt32Value = "romba-depth";
        internal static Library.Help.Feature RombaDepthInt32Input
        {
            get
            {
                return new Library.Help.Feature(
                    RombaDepthInt32Value,
                    new List<string>() { "-depr", "--romba-depth" },
                    "Set depth of depot for outputs",
                    Library.Help.FeatureType.Int32,
                    longDescription: "Optionally, set the depth of output depots. Defaults to 4 deep otherwise.");
            }
        }

        internal const string ThreadsInt32Value = "threads";
        internal static Library.Help.Feature ThreadsInt32Input
        {
            get
            {
                return new Library.Help.Feature(
                    ThreadsInt32Value,
                    new List<string>() { "-mt", "--threads" },
                    "Amount of threads to use (default = # cores)",
                    Library.Help.FeatureType.Int32,
                    longDescription: "Optionally, set the number of threads to use for the multithreaded operations. The default is the number of available machine threads; -1 means unlimited threads created.");
            }
        }

        #endregion

        #region Int64 features

        internal const string RadixInt64Value = "radix";
        internal static Library.Help.Feature RadixInt64Input
        {
            get
            {
                return new Library.Help.Feature(
                    RadixInt64Value,
                    new List<string>() { "-rad", "--radix" },
                    "Set the midpoint to split at",
                    Library.Help.FeatureType.Int64,
                    longDescription: "Set the size at which all roms less than the size are put in the first DAT, and everything greater than or equal goes in the second.");
            }
        }

        #endregion

        #region List<string> features

        internal const string BaseDatListValue = "base-dat";
        internal static Library.Help.Feature BaseDatListInput
        {
            get
            {
                return new Library.Help.Feature(
                    BaseDatListValue,
                    new List<string>() { "-bd", "--base-dat" },
                    "Add a base DAT for processing",
                    Library.Help.FeatureType.List,
                    longDescription: "Add a DAT or folder of DATs to the base set to be used for all operations. Multiple instances of this flag are allowed.");
            }
        }

        internal const string CategoryListValue = "category-filter";
        internal static Library.Help.Feature CategoryListInput
        {
            get
            {
                return new Library.Help.Feature(
                    CategoryListValue,
                    new List<string>() { "-cat", "--category-filter" },
                    "Filter by Category",
                    Library.Help.FeatureType.List,
                    longDescription: "Include only items with this Category in the output. Additionally, the user can specify an exact match or full C#-style regex for pattern matching. Multiple instances of this flag are allowed.");
            }
        }

        internal const string CrcListValue = "crc";
        internal static Library.Help.Feature CrcListInput
        {
            get
            {
                return new Library.Help.Feature(
                    CrcListValue,
                    new List<string>() { "-crc", "--crc" },
                    "Filter by CRC hash",
                    Library.Help.FeatureType.List,
                    longDescription: "Include only items with this CRC hash in the output. Additionally, the user can specify an exact match or full C#-style regex for pattern matching. Multiple instances of this flag are allowed.");
            }
        }

        internal const string DatListValue = "dat";
        internal static Library.Help.Feature DatListInput
        {
            get
            {
                return new Library.Help.Feature(
                    DatListValue,
                    new List<string>() { "-dat", "--dat" },
                    "Input DAT to be used",
                    Library.Help.FeatureType.List,
                    longDescription: "User-supplied DAT for use in all operations. Multiple instances of this flag are allowed.");
            }
        }

        internal const string ExcludeFieldListValue = "exclude-field";
        internal static Library.Help.Feature ExcludeFieldListInput
        {
            get
            {
                return new Library.Help.Feature(
                    ExcludeFieldListValue,
                    new List<string>() { "-ef", "--exclude-field" },
                    "Exclude a game/rom field from outputs",
                    Library.Help.FeatureType.List,
                    longDescription: "Exclude any valid item or machine field from outputs. Examples include: romof, publisher, and offset.");
            }
        }

        internal const string ExtAListValue = "exta";
        internal static Library.Help.Feature ExtaListInput
        {
            get
            {
                return new Library.Help.Feature(
                    ExtAListValue,
                    new List<string>() { "-exta", "--exta" },
                    "Set extension to be included in first DAT",
                    Library.Help.FeatureType.List,
                    longDescription: "Set the extension to be used to populate the first DAT. Multiple instances of this flag are allowed.");
            }
        }

        internal const string ExtBListValue = "extb";
        internal static Library.Help.Feature ExtbListInput
        {
            get
            {
                return new Library.Help.Feature(
                    ExtBListValue,
                    new List<string>() { "-extb", "--extb" },
                    "Set extension to be included in second DAT",
                    Library.Help.FeatureType.List,
                    longDescription: "Set the extension to be used to populate the second DAT. Multiple instances of this flag are allowed.");
            }
        }

        internal const string ExtraIniListValue = "extra-ini";
        internal static Library.Help.Feature ExtraIniListInput
        {
            get
            {
                return new Library.Help.Feature(
                    ExtraIniListValue,
                    new List<string>() { "-ini", "--extra-ini" },
                    "Apply a MAME INI for given field(s)",
                    Library.Help.FeatureType.List,
                    longDescription: "Apply any valid MAME INI for any valid field in the DatFile. Inputs are of the form 'Field:path\\to\\ini'. Multiple instances of this flag are allowed.");
            }
        }

        internal const string FilterListValue = "filter";
        internal static Library.Help.Feature FilterListInput
        {
            get
            {
                return new Library.Help.Feature(
                    FilterListValue,
                    new List<string>() { "-fi", "--filter" },
                    "Filter a game/rom field with the given value(s)",
                    Library.Help.FeatureType.List,
                    longDescription: "Filter any valid item or machine field from inputs. Filters are input in the form 'key:value' or '!key:value', where the '!' signifies 'not matching'. Numeric values may also prefix the 'value' with '>', '<', or '=' accordingly. Key examples include: romof, category, and game. Additionally, the user can specify an exact match or full C#-style regex for pattern matching. Multiple instances of this flag are allowed.");
            }
        }

        internal const string GameDescriptionListValue = "game-description";
        internal static Library.Help.Feature GameDescriptionListInput
        {
            get
            {
                return new Library.Help.Feature(
                    GameDescriptionListValue,
                    new List<string>() { "-gd", "--game-description" },
                    "Filter by game description",
                    Library.Help.FeatureType.List,
                    longDescription: "Include only items with this game description in the output. Additionally, the user can specify an exact match or full C#-style regex for pattern matching. Multiple instances of this flag are allowed.");
            }
        }

        internal const string GameNameListValue = "game-name";
        internal static Library.Help.Feature GameNameListInput
        {
            get
            {
                return new Library.Help.Feature(
                    GameNameListValue,
                    new List<string>() { "-gn", "--game-name" },
                    "Filter by game name",
                    Library.Help.FeatureType.List,
                    longDescription: "Include only items with this game name in the output. Additionally, the user can specify an exact match or full C#-style regex for pattern matching. Multiple instances of this flag are allowed.");
            }
        }

        internal const string GameTypeListValue = "game-type";
        internal static Library.Help.Feature GameTypeListInput
        {
            get
            {
                return new Library.Help.Feature(
                    GameTypeListValue,
                    new List<string>() { "-gt", "--game-type" },
                    "Include only games with a given type",
                    Library.Help.FeatureType.List,
                    longDescription: @"Include only items with this game type in the output. Multiple instances of this flag are allowed.
Possible values are: None, Bios, Device, Mechanical");
            }
        }

        internal const string ItemNameListValue = "item-name";
        internal static Library.Help.Feature ItemNameListInput
        {
            get
            {
                return new Library.Help.Feature(
                    ItemNameListValue,
                    new List<string>() { "-rn", "--item-name" },
                    "Filter by item name",
                    Library.Help.FeatureType.List,
                    longDescription: "Include only items with this item name in the output. Additionally, the user can specify an exact match or full C#-style regex for pattern matching. Multiple instances of this flag are allowed.");
            }
        }

        internal const string ItemTypeListValue = "item-type";
        internal static Library.Help.Feature ItemTypeListInput
        {
            get
            {
                return new Library.Help.Feature(
                    ItemTypeListValue,
                    new List<string>() { "-rt", "--item-type" },
                    "Filter by item type",
                    Library.Help.FeatureType.List,
                    longDescription: "Include only items with this item type in the output. Additionally, the user can specify an exact match or full C#-style regex for pattern matching. Multiple instances of this flag are allowed.");
            }
        }

        internal const string Md5ListValue = "md5";
        internal static Library.Help.Feature Md5ListInput
        {
            get
            {
                return new Library.Help.Feature(
                    Md5ListValue,
                    new List<string>() { "-md5", "--md5" },
                    "Filter by MD5 hash",
                    Library.Help.FeatureType.List,
                    longDescription: "Include only items with this MD5 hash in the output. Additionally, the user can specify an exact match or full C#-style regex for pattern matching. Multiple instances of this flag are allowed.");
            }
        }

        internal const string NotCategoryListValue = "not-category";
        internal static Library.Help.Feature NotCategoryListInput
        {
            get
            {
                return new Library.Help.Feature(
                    NotCategoryListValue,
                    new List<string>() { "-ncat", "--not-category" },
                    "Filter by not Category",
                    Library.Help.FeatureType.List,
                    longDescription: "Include only items without this Category in the output. Additionally, the user can specify an exact match or full C#-style regex for pattern matching. Multiple instances of this flag are allowed.");
            }
        }

        internal const string NotCrcListValue = "not-crc";
        internal static Library.Help.Feature NotCrcListInput
        {
            get
            {
                return new Library.Help.Feature(
                    NotCrcListValue,
                    new List<string>() { "-ncrc", "--not-crc" },
                    "Filter by not CRC hash",
                    Library.Help.FeatureType.List,
                    longDescription: "Include only items without this CRC hash in the output. Additionally, the user can specify an exact match or full C#-style regex for pattern matching. Multiple instances of this flag are allowed.");
            }
        }

        internal const string NotGameDescriptionListValue = "not-game-description";
        internal static Library.Help.Feature NotGameDescriptionListInput
        {
            get
            {
                return new Library.Help.Feature(
                    NotGameDescriptionListValue,
                    new List<string>() { "-ngd", "--not-game-description" },
                    "Filter by not game description",
                    Library.Help.FeatureType.List,
                    longDescription: "Include only items without this game description in the output. Additionally, the user can specify an exact match or full C#-style regex for pattern matching. Multiple instances of this flag are allowed.");
            }
        }

        internal const string NotGameNameListValue = "not-game-name";
        internal static Library.Help.Feature NotGameNameListInput
        {
            get
            {
                return new Library.Help.Feature(
                    NotGameNameListValue,
                    new List<string>() { "-ngn", "--not-game-name" },
                    "Filter by not game name",
                    Library.Help.FeatureType.List,
                    longDescription: "Include only items without this game name in the output. Additionally, the user can specify an exact match or full C#-style regex for pattern matching. Multiple instances of this flag are allowed.");
            }
        }

        internal const string NotGameTypeListValue = "not-game-type";
        internal static Library.Help.Feature NotGameTypeListInput
        {
            get
            {
                return new Library.Help.Feature(
                    NotGameTypeListValue,
                    new List<string>() { "-ngt", "--not-game-type" },
                    "Exclude only games with a given type",
                    Library.Help.FeatureType.List,
                    longDescription: @"Include only items without this game type in the output. Multiple instances of this flag are allowed.
Possible values are: None, Bios, Device, Mechanical");
            }
        }

        internal const string NotItemNameListValue = "not-item-name";
        internal static Library.Help.Feature NotItemNameListInput
        {
            get
            {
                return new Library.Help.Feature(
                    NotItemNameListValue,
                    new List<string>() { "-nrn", "--not-item-name" },
                    "Filter by not item name",
                    Library.Help.FeatureType.List,
                    longDescription: "Include only items without this item name in the output. Additionally, the user can specify an exact match or full C#-style regex for pattern matching. Multiple instances of this flag are allowed.");
            }
        }

        internal const string NotItemTypeListValue = "not-item-type";
        internal static Library.Help.Feature NotItemTypeListInput
        {
            get
            {
                return new Library.Help.Feature(
                    NotItemTypeListValue,
                    new List<string>() { "-nrt", "--not-item-type" },
                    "Filter by not item type",
                    Library.Help.FeatureType.List,
                    longDescription: "Include only items without this item type in the output. Additionally, the user can specify an exact match or full C#-style regex for pattern matching. Multiple instances of this flag are allowed.");
            }
        }

        internal const string NotMd5ListValue = "not-md5";
        internal static Library.Help.Feature NotMd5ListInput
        {
            get
            {
                return new Library.Help.Feature(
                    NotMd5ListValue,
                    new List<string>() { "-nmd5", "--not-md5" },
                    "Filter by not MD5 hash",
                    Library.Help.FeatureType.List,
                    longDescription: "Include only items without this MD5 hash in the output. Additionally, the user can specify an exact match or full C#-style regex for pattern matching. Multiple instances of this flag are allowed.");
            }
        }

#if NET_FRAMEWORK
        internal const string NotRipeMd160ListValue = "not-ripemd160";
        internal static Library.Help.Feature NotRipeMd160ListInput
        {
            get
            {
                return new Library.Help.Feature(
                    NotRipeMd160ListValue,
                    new List<string>() { "-nripemd160", "--not-ripemd160" },
                    "Filter by not RIPEMD160 hash",
                    Library.Help.FeatureType.List,
                    longDescription: "Include only items without this RIPEMD160 hash in the output. Additionally, the user can specify an exact match or full C#-style regex for pattern matching. Multiple instances of this flag are allowed.");
            }
        }
#endif

        internal const string NotSha1ListValue = "not-sha1";
        internal static Library.Help.Feature NotSha1ListInput
        {
            get
            {
                return new Library.Help.Feature(
                    NotSha1ListValue,
                    new List<string>() { "-nsha1", "--not-sha1" },
                    "Filter by not SHA-1 hash",
                    Library.Help.FeatureType.List,
                    longDescription: "Include only items without this SHA-1 hash in the output. Additionally, the user can specify an exact match or full C#-style regex for pattern matching. Multiple instances of this flag are allowed.");
            }
        }

        internal const string NotSha256ListValue = "not-sha256";
        internal static Library.Help.Feature NotSha256ListInput
        {
            get
            {
                return new Library.Help.Feature(
                    NotSha256ListValue,
                    new List<string>() { "-nsha256", "--not-sha256" },
                    "Filter by not SHA-256 hash",
                    Library.Help.FeatureType.List,
                    longDescription: "Include only items without this SHA-256 hash in the output. Additionally, the user can specify an exact match or full C#-style regex for pattern matching. Multiple instances of this flag are allowed.");
            }
        }

        internal const string NotSha384ListValue = "not-sha384";
        internal static Library.Help.Feature NotSha384ListInput
        {
            get
            {
                return new Library.Help.Feature(
                    NotSha384ListValue,
                    new List<string>() { "-nsha384", "--not-sha384" },
                    "Filter by not SHA-384 hash",
                    Library.Help.FeatureType.List,
                    longDescription: "Include only items without this SHA-384 hash in the output. Additionally, the user can specify an exact match or full C#-style regex for pattern matching. Multiple instances of this flag are allowed.");
            }
        }

        internal const string NotSha512ListValue = "not-sha512";
        internal static Library.Help.Feature NotSha512ListInput
        {
            get
            {
                return new Library.Help.Feature(
                    NotSha512ListValue,
                    new List<string>() { "-nsha512", "--not-sha512" },
                    "Filter by not SHA-512 hash",
                    Library.Help.FeatureType.List,
                    longDescription: "Include only items without this SHA-512 hash in the output. Additionally, the user can specify an exact match or full C#-style regex for pattern matching. Multiple instances of this flag are allowed.");
            }
        }

        internal const string NotStatusListValue = "not-status";
        internal static Library.Help.Feature NotStatusListInput
        {
            get
            {
                return new Library.Help.Feature(
                    NotStatusListValue,
                    new List<string>() { "-nis", "--not-status" },
                    "Exclude only items with a given status",
                    Library.Help.FeatureType.List,
                    longDescription: @"Include only items without this item status in the output. Multiple instances of this flag are allowed.
Possible values are: None, Good, BadDump, Nodump, Verified");
            }
        }

        internal const string OutputTypeListValue = "output-type";
        internal static Library.Help.Feature OutputTypeListInput
        {
            get
            {
                return new Library.Help.Feature(
                    OutputTypeListValue,
                    new List<string>() { "-ot", "--output-type" },
                    "Output DATs to a specified format",
                    Library.Help.FeatureType.List,
                    longDescription: @"Add outputting the created DAT to known format. Multiple instances of this flag are allowed.

Possible values are:
    all              - All available DAT types
    am, attractmode  - AttractMode XML
    cmp, clrmamepro  - ClrMamePro
    csv              - Standardized Comma-Separated Value
    dc, doscenter    - DOSCenter
    json             - SabreJSON
    lr, listrom      - MAME Listrom
    lx, listxml      - MAME Listxml
    miss, missfile   - GoodTools Missfile
    md5              - MD5
    msx, openmsx     - openMSX Software List
    ol, offlinelist  - OfflineList XML
    rc, romcenter    - RomCenter"
#if NET_FRAMEWORK
+ @"
    ripemd160        - RIPEMD160"
#endif
+ @"
    sd, sabredat     - SabreDAT XML
    sfv              - SFV
    sha1             - SHA1
    sha256           - SHA256
    sha384           - SHA384
    sha512           - SHA512
    smdb, everdrive  - Everdrive SMDB
    sl, softwarelist - MAME Software List XML
    ssv              - Standardized Semicolon-Separated Value
    tsv              - Standardized Tab-Separated Value
    xml, logiqx      - Logiqx XML");
            }
        }

        internal const string RegionListValue = "region";
        internal static Library.Help.Feature RegionListInput
        {
            get
            {
                return new Library.Help.Feature(
                    RegionListValue,
                    new List<string>() { "-reg", "--region" },
                    "Add a region for 1G1R",
                    Library.Help.FeatureType.List,
                    longDescription: "Add a region (in order) for use with 1G1R filtering. If this is not supplied, then by default, only parent sets will be included in the output. Multiple instances of this flag are allowed.");
            }
        }

        internal const string ReportTypeListValue = "report-type";
        internal static Library.Help.Feature ReportTypeListInput
        {
            get
            {
                return new Library.Help.Feature(
                    ReportTypeListValue,
                    new List<string>() { "-srt", "--report-type" },
                    "Output statistics to a specified format",
                    Library.Help.FeatureType.List,
                    longDescription: @"Add outputting the created DAT to known format. Multiple instances of this flag are allowed.

Possible values are:
    all              - All available DAT types
    csv              - Standardized Comma-Separated Value
    html             - HTML webpage
    ssv              - Standardized Semicolon-Separated Value
    text             - Generic textfile
    tsv              - Standardized Tab-Separated Value");
            }
        }

#if NET_FRAMEWORK
        internal const string RipeMd160ListValue = "ripemd160";
        internal static Library.Help.Feature RipeMd160ListInput
        {
            get
            {
                return new Library.Help.Feature(
                    RipeMd160ListValue,
                    new List<string>() { "-ripemd160", "--ripemd160" },
                    "Filter by RIPEMD160 hash",
                    Library.Help.FeatureType.List,
                    longDescription: "Include only items with this RIPEMD160 hash in the output. Additionally, the user can specify an exact match or full C#-style regex for pattern matching. Multiple instances of this flag are allowed.");
            }
        }
#endif

        internal const string Sha1ListValue = "sha1";
        internal static Library.Help.Feature Sha1ListInput
        {
            get
            {
                return new Library.Help.Feature(
                    Sha1ListValue,
                    new List<string>() { "-sha1", "--sha1" },
                    "Filter by SHA-1 hash",
                    Library.Help.FeatureType.List,
                    longDescription: "Include only items with this SHA-1 hash in the output. Additionally, the user can specify an exact match or full C#-style regex for pattern matching. Multiple instances of this flag are allowed.");
            }
        }

        internal const string Sha256ListValue = "sha256";
        internal static Library.Help.Feature Sha256ListInput
        {
            get
            {
                return new Library.Help.Feature(
                    Sha256ListValue,
                    new List<string>() { "-sha256", "--sha256" },
                    "Filter by SHA-256 hash",
                    Library.Help.FeatureType.List,
                    longDescription: "Include only items with this SHA-256 hash in the output. Additionally, the user can specify an exact match or full C#-style regex for pattern matching. Multiple instances of this flag are allowed.");
            }
        }

        internal const string Sha384ListValue = "sha384";
        internal static Library.Help.Feature Sha384ListInput
        {
            get
            {
                return new Library.Help.Feature(
                    Sha384ListValue,
                    new List<string>() { "-sha384", "--sha384" },
                    "Filter by SHA-384 hash",
                    Library.Help.FeatureType.List,
                    longDescription: "Include only items with this SHA-384 hash in the output. Additionally, the user can specify an exact match or full C#-style regex for pattern matching. Multiple instances of this flag are allowed.");
            }
        }

        internal const string Sha512ListValue = "sha512";
        internal static Library.Help.Feature Sha512ListInput
        {
            get
            {
                return new Library.Help.Feature(
                    Sha512ListValue,
                    new List<string>() { "-sha512", "--sha512" },
                    "Filter by SHA-512 hash",
                    Library.Help.FeatureType.List,
                    longDescription: "Include only items with this SHA-512 hash in the output. Additionally, the user can specify an exact match or full C#-style regex for pattern matching. Multiple instances of this flag are allowed.");
            }
        }

        internal const string StatusListValue = "status";
        internal static Library.Help.Feature StatusListInput
        {
            get
            {
                return new Library.Help.Feature(
                    StatusListValue,
                    new List<string>() { "-is", "--status" },
                    "Include only items with a given status",
                    Library.Help.FeatureType.List,
                    longDescription: @"Include only items with this item status in the output. Multiple instances of this flag are allowed.
Possible values are: None, Good, BadDump, Nodump, Verified");
            }
        }

        internal const string UpdateFieldListValue = "update-field";
        internal static Library.Help.Feature UpdateFieldListInput
        {
            get
            {
                return new Library.Help.Feature(
                    UpdateFieldListValue,
                    new List<string>() { "-uf", "--update-field" },
                    "Update a game/rom field from base DATs",
                    Library.Help.FeatureType.List,
                    longDescription: "Update any valid item or machine field from base DAT(s). Examples include: romof, publisher, and offset.");
            }
        }

        #endregion

        #region String features

        internal const string AddExtensionStringValue = "add-extension";
        internal static Library.Help.Feature AddExtensionStringInput
        {
            get
            {
                return new Library.Help.Feature(
                    AddExtensionStringValue,
                    new List<string>() { "-ae", "--add-extension" },
                    "Add an extension to each item",
                    Library.Help.FeatureType.String,
                    longDescription: "Add a postfix extension to each full item name.");
            }
        }

        internal const string AuthorStringValue = "author";
        internal static Library.Help.Feature AuthorStringInput
        {
            get
            {
                return new Library.Help.Feature(
                    AuthorStringValue,
                    new List<string>() { "-au", "--author" },
                    "Set the author of the DAT",
                    Library.Help.FeatureType.String,
                    longDescription: "Set the author header field for the output DAT(s)");
            }
        }

        internal const string CategoryStringValue = "category";
        internal static Library.Help.Feature CategoryStringInput
        {
            get
            {
                return new Library.Help.Feature(
                    CategoryStringValue,
                    new List<string>() { "-c", "--category" },
                    "Set the category of the DAT",
                    Library.Help.FeatureType.String,
                    longDescription: "Set the category header field for the output DAT(s)");
            }
        }

        internal const string CommentStringValue = "comment";
        internal static Library.Help.Feature CommentStringInput
        {
            get
            {
                return new Library.Help.Feature(
                    CommentStringValue,
                    new List<string>() { "-co", "--comment" },
                    "Set a new comment of the DAT",
                    Library.Help.FeatureType.String,
                    longDescription: "Set the comment header field for the output DAT(s)");
            }
        }

        internal const string DateStringValue = "date";
        internal static Library.Help.Feature DateStringInput
        {
            get
            {
                return new Library.Help.Feature(
                    DateStringValue,
                    new List<string>() { "-da", "--date" },
                    "Set a new date",
                    Library.Help.FeatureType.String,
                    longDescription: "Set the date header field for the output DAT(s)");
            }
        }

        internal const string DescriptionStringValue = "description";
        internal static Library.Help.Feature DescriptionStringInput
        {
            get
            {
                return new Library.Help.Feature(
                    DescriptionStringValue,
                    new List<string>() { "-de", "--description" },
                    "Set the description of the DAT",
                    Library.Help.FeatureType.String,
                    longDescription: "Set the description header field for the output DAT(s)");
            }
        }

        internal const string EmailStringValue = "email";
        internal static Library.Help.Feature EmailStringInput
        {
            get
            {
                return new Library.Help.Feature(
                    EmailStringValue,
                    new List<string>() { "-em", "--email" },
                    "Set a new email of the DAT",
                    Library.Help.FeatureType.String,
                    longDescription: "Set the email header field for the output DAT(s)");
            }
        }

        internal const string EqualStringValue = "equal";
        internal static Library.Help.Feature EqualStringInput
        {
            get
            {
                return new Library.Help.Feature(
                    EqualStringValue,
                    new List<string>() { "-seq", "--equal" },
                    "Filter by size ==",
                    Library.Help.FeatureType.String,
                    longDescription: "Only include items of this exact size in the output DAT. Users can specify either a regular integer number or a number with a standard postfix. e.g. 8kb => 8000 or 8kib => 8192");
            }
        }

        internal const string FilenameStringValue = "filename";
        internal static Library.Help.Feature FilenameStringInput
        {
            get
            {
                return new Library.Help.Feature(
                    FilenameStringValue,
                    new List<string>() { "-f", "--filename" },
                    "Set the external name of the DAT",
                    Library.Help.FeatureType.String,
                    longDescription: "Set the external filename for the output DAT(s)");
            }
        }

        internal const string ForceMergingStringValue = "forcemerging";
        internal static Library.Help.Feature ForceMergingStringInput
        {
            get
            {
                return new Library.Help.Feature(
                    ForceMergingStringValue,
                    new List<string>() { "-fm", "--forcemerging" },
                    "Set force merging",
                    Library.Help.FeatureType.String,
                    longDescription: @"Set the forcemerging tag to the given value.
Possible values are: None, Split, Device, Merged, Nonmerged, Full");
            }
        }

        internal const string ForceNodumpStringValue = "forcenodump";
        internal static Library.Help.Feature ForceNodumpStringInput
        {
            get
            {
                return new Library.Help.Feature(
                    ForceNodumpStringValue,
                    new List<string>() { "-fn", "--forcenodump" },
                    "Set force nodump",
                    Library.Help.FeatureType.String,
                    longDescription: @"Set the forcenodump tag to the given value.
Possible values are: None, Obsolete, Required, Ignore");
            }
        }

        internal const string ForcePackingStringValue = "forcepacking";
        internal static Library.Help.Feature ForcePackingStringInput
        {
            get
            {
                return new Library.Help.Feature(
                    ForcePackingStringValue,
                    new List<string>() { "-fp", "--forcepacking" },
                    "Set force packing",
                    Library.Help.FeatureType.String,
                    longDescription: @"Set the forcepacking tag to the given value.
Possible values are: None, Zip, Unzip, Partial, Flat");
            }
        }

        internal const string GreaterStringValue = "greater";
        internal static Library.Help.Feature GreaterStringInput
        {
            get
            {
                return new Library.Help.Feature(
                    GreaterStringValue,
                    new List<string>() { "-sgt", "--greater" },
                    "Filter by size >=",
                    Library.Help.FeatureType.String,
                    longDescription: "Only include items whose size is greater than or equal to this value in the output DAT. Users can specify either a regular integer number or a number with a standard postfix. e.g. 8kb => 8000 or 8kib => 8192");
            }
        }

        internal const string HeaderStringValue = "header";
        internal static Library.Help.Feature HeaderStringInput
        {
            get
            {
                return new Library.Help.Feature(
                    HeaderStringValue,
                    new List<string>() { "-h", "--header" },
                    "Set a header skipper to use, blank means all",
                    Library.Help.FeatureType.String,
                    longDescription: "Set the header special field for the output DAT(s). In file rebuilding, this flag allows for either all copier headers (using \"\") or specific copier headers by name (such as \"fds.xml\") to determine if a file matches or not.");

            }
        }

        internal const string HomepageStringValue = "homepage";
        internal static Library.Help.Feature HomepageStringInput
        {
            get
            {
                return new Library.Help.Feature(
                    HomepageStringValue,
                    new List<string>() { "-hp", "--homepage" },
                    "Set a new homepage of the DAT",
                    Library.Help.FeatureType.String,
                    longDescription: "Set the homepage header field for the output DAT(s)");
            }
        }

        internal const string LessStringValue = "less";
        internal static Library.Help.Feature LessStringInput
        {
            get
            {
                return new Library.Help.Feature(
                    LessStringValue,
                    new List<string>() { "-slt", "--less" },
                    "Filter by size =<",
                    Library.Help.FeatureType.String,
                    longDescription: "Only include items whose size is less than or equal to this value in the output DAT. Users can specify either a regular integer number or a number with a standard postfix. e.g. 8kb => 8000 or 8kib => 8192");
            }
        }

        internal const string NameStringValue = "name";
        internal static Library.Help.Feature NameStringInput
        {
            get
            {
                return new Library.Help.Feature(
                    NameStringValue,
                    new List<string>() { "-n", "--name" },
                    "Set the internal name of the DAT",
                    Library.Help.FeatureType.String,
                    longDescription: "Set the name header field for the output DAT(s)");
            }
        }

        internal const string OutputDirStringValue = "output-dir";
        internal static Library.Help.Feature OutputDirStringInput
        {
            get
            {
                return new Library.Help.Feature(
                    OutputDirStringValue,
                    new List<string>() { "-out", "--output-dir" },
                    "Set output directory",
                    Library.Help.FeatureType.String,
                    longDescription: "This sets an output folder to be used when the files are created. If a path is not defined, the runtime directory is used instead.");
            }
        }

        internal const string PostfixStringValue = "postfix";
        internal static Library.Help.Feature PostfixStringInput
        {
            get
            {
                return new Library.Help.Feature(
                    PostfixStringValue,
                    new List<string>() { "-post", "--postfix" },
                    "Set postfix for all lines",
                    Library.Help.FeatureType.String,
                    longDescription: @"Set a generic postfix to be appended to all outputted lines.

Some special strings that can be used:
- %game% / %machine% - Replaced with the Game/Machine name
- %name% - Replaced with the Rom name
- %manufacturer% - Replaced with game Manufacturer
- %publisher% - Replaced with game Publisher
- %category% - Replaced with game Category
- %crc% - Replaced with the CRC
- %md5% - Replaced with the MD5"
#if NET_FRAMEWORK
+ @"
- %ripemd160% - Replaced with the RIPEMD160"
#endif
+ @"
- %sha1% - Replaced with the SHA-1
- %sha256% - Replaced with the SHA-256
- %sha384% - Replaced with the SHA-384
- %sha512% - Replaced with the SHA-512
- %size% - Replaced with the size");
            }
        }

        internal const string PrefixStringValue = "prefix";
        internal static Library.Help.Feature PrefixStringInput
        {
            get
            {
                return new Library.Help.Feature(
                    PrefixStringValue,
                    new List<string>() { "-pre", "--prefix" },
                    "Set prefix for all lines",
                    Library.Help.FeatureType.String,
                    longDescription: @"Set a generic prefix to be prepended to all outputted lines.

Some special strings that can be used:
- %game% / %machine% - Replaced with the Game/Machine name
- %name% - Replaced with the Rom name
- %manufacturer% - Replaced with game Manufacturer
- %publisher% - Replaced with game Publisher
- %category% - Replaced with game Category
- %crc% - Replaced with the CRC
- %md5% - Replaced with the MD5
- %sha1% - Replaced with the SHA-1
- %sha256% - Replaced with the SHA-256
- %sha384% - Replaced with the SHA-384
- %sha512% - Replaced with the SHA-512
- %size% - Replaced with the size");
            }
        }

        internal const string ReplaceExtensionStringValue = "replace-extension";
        internal static Library.Help.Feature ReplaceExtensionStringInput
        {
            get
            {
                return new Library.Help.Feature(
                    ReplaceExtensionStringValue,
                    new List<string>() { "-rep", "--replace-extension" },
                    "Replace all extensions with specified",
                    Library.Help.FeatureType.String,
                    longDescription: "When an extension exists, replace it with the provided instead.");
            }
        }

        internal const string RootStringValue = "root";
        internal static Library.Help.Feature RootStringInput
        {
            get
            {
                return new Library.Help.Feature(
                    RootStringValue,
                    new List<string>() { "-r", "--root" },
                    "Set a new rootdir",
                    Library.Help.FeatureType.String,
                    longDescription: "Set the rootdir (as used by SuperDAT mode) for the output DAT(s).");
            }
        }

        internal const string RootDirStringValue = "root-dir";
        internal static Library.Help.Feature RootDirStringInput
        {
            get
            {
                return new Library.Help.Feature(
                    RootDirStringValue,
                    new List<string>() { "-rd", "--root-dir" },
                    "Set the root directory for calc",
                    Library.Help.FeatureType.String,
                    longDescription: "In the case that the files will not be stored from the root directory, a new root can be set for path length calculations.");
            }
        }

        internal const string TempStringValue = "temp";
        internal static Library.Help.Feature TempStringInput
        {
            get
            {
                return new Library.Help.Feature(
                    TempStringValue,
                    new List<string>() { "-t", "--temp" },
                    "Set the temporary directory to use",
                    Library.Help.FeatureType.String,
                    longDescription: "Optionally, a temp folder can be supplied in the case the default temp directory is not preferred.");
            }
        }

        internal const string UrlStringValue = "url";
        internal static Library.Help.Feature UrlStringInput
        {
            get
            {
                return new Library.Help.Feature(
                    UrlStringValue,
                    new List<string>() { "-u", "--url" },
                    "Set a new URL of the DAT",
                    Library.Help.FeatureType.String,
                    longDescription: "Set the URL header field for the output DAT(s)");
            }
        }

        internal const string VersionStringValue = "version";
        internal static Library.Help.Feature VersionStringInput
        {
            get
            {
                return new Library.Help.Feature(
                    VersionStringValue,
                    new List<string>() { "-v", "--version" },
                    "Set the version of the DAT",
                    Library.Help.FeatureType.String,
                    longDescription: "Set the version header field for the output DAT(s)");
            }
        }

        #endregion

        #endregion

        #region Fields

        /// <summary>
        /// Preconfigured Cleaner
        /// </summary>
        protected Cleaner Cleaner { get; set; }

        /// <summary>
        /// Preconfigured ExtraIni set
        /// </summary>
        protected ExtraIni Extras { get; set; }

        /// <summary>
        /// Pre-configured Filter
        /// </summary>
        protected Filter Filter { get; set; }

        /// <summary>
        /// Pre-configured DatHeader
        /// </summary>
        protected DatHeader Header { get; set; }

        /// <summary>
        /// Output directory
        /// </summary>
        protected string OutputDir { get; set; }

        #endregion

        #region Add Feature Groups

        /// <summary>
        /// Add Filter-specific features
        /// </summary>
        protected void AddFilteringFeatures()
        {
            AddFeature(FilterListInput);
            AddFeature(CategoryListInput);
            AddFeature(NotCategoryListInput);
            AddFeature(GameNameListInput);
            AddFeature(NotGameNameListInput);
            AddFeature(GameDescriptionListInput);
            AddFeature(NotGameDescriptionListInput);
            AddFeature(MatchOfTagsFlag);
            AddFeature(ItemNameListInput);
            AddFeature(NotItemNameListInput);
            AddFeature(ItemTypeListInput);
            AddFeature(NotItemTypeListInput);
            AddFeature(GreaterStringInput);
            AddFeature(LessStringInput);
            AddFeature(EqualStringInput);
            AddFeature(CrcListInput);
            AddFeature(NotCrcListInput);
            AddFeature(Md5ListInput);
            AddFeature(NotMd5ListInput);
#if NET_FRAMEWORK
            AddFeature(RipeMd160ListInput);
            AddFeature(NotRipeMd160ListInput);
#endif
            AddFeature(Sha1ListInput);
            AddFeature(NotSha1ListInput);
            AddFeature(Sha256ListInput);
            AddFeature(NotSha256ListInput);
            AddFeature(Sha384ListInput);
            AddFeature(NotSha384ListInput);
            AddFeature(Sha512ListInput);
            AddFeature(NotSha512ListInput);
            AddFeature(StatusListInput);
            AddFeature(NotStatusListInput);
            AddFeature(GameTypeListInput);
            AddFeature(NotGameTypeListInput);
            AddFeature(RunnableFlag);
            AddFeature(NotRunnableFlag);
        }

        /// <summary>
        /// Add Header-specific features
        /// </summary>
        protected void AddHeaderFeatures()
        {
            // Header Values
            AddFeature(FilenameStringInput);
            AddFeature(NameStringInput);
            AddFeature(DescriptionStringInput);
            AddFeature(RootStringInput);
            AddFeature(CategoryStringInput);
            AddFeature(VersionStringInput);
            AddFeature(DateStringInput);
            AddFeature(AuthorStringInput);
            AddFeature(EmailStringInput);
            AddFeature(HomepageStringInput);
            AddFeature(UrlStringInput);
            AddFeature(CommentStringInput);
            AddFeature(HeaderStringInput);
            AddFeature(SuperdatFlag);
            AddFeature(ForceMergingStringInput);
            AddFeature(ForceNodumpStringInput);
            AddFeature(ForcePackingStringInput);

            // Header Filters
            AddFeature(ExcludeFieldListInput);
            AddFeature(OneGamePerRegionFlag);
            this[OneGamePerRegionFlag].AddFeature(RegionListInput);
            AddFeature(OneRomPerGameFlag);
            AddFeature(SceneDateStripFlag);
        }

        /// <summary>
        /// Add internal split/merge features
        /// </summary>
        protected void AddInternalSplitFeatures()
        {
            AddFeature(DatMergedFlag);
            AddFeature(DatSplitFlag);
            AddFeature(DatNonMergedFlag);
            AddFeature(DatDeviceNonMergedFlag);
            AddFeature(DatFullNonMergedFlag);
        }

        #endregion

        public override void ProcessFeatures(Dictionary<string, Library.Help.Feature> features)
        {
            // Generic feature flags
            Cleaner = GetCleaner(features);
            Extras = GetExtras(features);
            Filter = GetFilter(features);
            Header = GetDatHeader(features);
            OutputDir = GetString(features, OutputDirStringValue);

            // Set threading flag, if necessary
            if (features.ContainsKey(ThreadsInt32Value))
                Globals.MaxThreads = GetInt32(features, ThreadsInt32Value);

            // Set temp path, if necessary
            if (features.ContainsKey(TempStringValue))
                Globals.TempDir = GetString(features, TempStringValue);
        }

        #region Protected Specific Extraction

        /// <summary>
        /// Get omit from scan from feature list
        /// </summary>
        protected Hash GetOmitFromScan(Dictionary<string, Library.Help.Feature> features)
        {
            Hash omitFromScan = Hash.DeepHashes; // TODO: All instances of Hash.DeepHashes should be made into 0x0 eventually

            if (GetBoolean(features, SkipMd5Value))
                omitFromScan |= Hash.MD5;
#if NET_FRAMEWORK
            if (GetBoolean(features, SkipRipeMd160Value))
                omitFromScan &= ~Hash.RIPEMD160; // TODO: This needs to be inverted later
#endif
            if (GetBoolean(features, SkipSha1Value))
                omitFromScan |= Hash.SHA1;
            if (GetBoolean(features, SkipSha256Value))
                omitFromScan &= ~Hash.SHA256; // TODO: This needs to be inverted later
            if (GetBoolean(features, SkipSha384Value))
                omitFromScan &= ~Hash.SHA384; // TODO: This needs to be inverted later
            if (GetBoolean(features, SkipSha512Value))
                omitFromScan &= ~Hash.SHA512; // TODO: This needs to be inverted later
            if (GetBoolean(features, SkipSpamSumValue))
                omitFromScan &= ~Hash.SpamSum; // TODO: This needs to be inverted later

            return omitFromScan;
        }

        /// <summary>
        /// Get OutputFormat from feature list
        /// </summary>
        protected OutputFormat GetOutputFormat(Dictionary<string, Library.Help.Feature> features)
        {
            if (GetBoolean(features, TarValue))
                return OutputFormat.TapeArchive;
            else if (GetBoolean(features, Torrent7zipValue))
                return OutputFormat.Torrent7Zip;
            else if (GetBoolean(features, TorrentGzipValue))
                return OutputFormat.TorrentGzip;
            //else if (GetBoolean(features, SharedTorrentLrzipValue))
            //    return OutputFormat.TorrentLRZip;
            //else if (GetBoolean(features, SharedTorrentLz4Value))
            //    return OutputFormat.TorrentLZ4;
            //else if (GetBoolean(features, SharedTorrentRarValue))
            //    return OutputFormat.TorrentRar;
            //else if (GetBoolean(features, SharedTorrentXzValue))
            //    return OutputFormat.TorrentXZ;
            else if (GetBoolean(features, TorrentZipValue))
                return OutputFormat.TorrentZip;
            //else if (GetBoolean(features, SharedTorrentZpaqValue))
            //    return OutputFormat.TorrentZPAQ;
            //else if (GetBoolean(features, SharedTorrentZstdValue))
            //    return OutputFormat.TorrentZstd;
            else
                return OutputFormat.Folder;
        }

        /// <summary>
        /// Get SkipFileType from feature list
        /// </summary>
        protected SkipFileType GetSkipFileType(Dictionary<string, Library.Help.Feature> features)
        {
            if (GetBoolean(features, SkipArchivesValue))
                return SkipFileType.Archive;
            else if (GetBoolean(features, SkipFilesValue))
                return SkipFileType.File;
            else
                return SkipFileType.None;
        }

        /// <summary>
        /// Get SplittingMode from feature list
        /// </summary>
        protected SplittingMode GetSplittingMode(Dictionary<string, Library.Help.Feature> features)
        {
            SplittingMode splittingMode = SplittingMode.None;

            if (GetBoolean(features, ExtensionValue))
                splittingMode |= SplittingMode.Extension;
            if (GetBoolean(features, HashValue))
                splittingMode |= SplittingMode.Hash;
            if (GetBoolean(features, LevelValue))
                splittingMode |= SplittingMode.Level;
            if (GetBoolean(features, SizeValue))
                splittingMode |= SplittingMode.Size;
            if (GetBoolean(features, TypeValue))
                splittingMode |= SplittingMode.Type;

            return splittingMode;
        }

        /// <summary>
        /// Get SplitType from feature list
        /// </summary>
        protected MergingFlag GetSplitType(Dictionary<string, Library.Help.Feature> features)
        {
            MergingFlag splitType = MergingFlag.None;
            if (GetBoolean(features, DatDeviceNonMergedValue))
                splitType = MergingFlag.Device;
            else if (GetBoolean(features, DatFullNonMergedValue))
                splitType = MergingFlag.Full;
            else if (GetBoolean(features, DatMergedValue))
                splitType = MergingFlag.Merged;
            else if (GetBoolean(features, DatNonMergedValue))
                splitType = MergingFlag.NonMerged;
            else if (GetBoolean(features, DatSplitValue))
                splitType = MergingFlag.Split;

            return splitType;
        }

        /// <summary>
        /// Get StatReportFormat from feature list
        /// </summary>
        protected StatReportFormat GetStatReportFormat(Dictionary<string, Library.Help.Feature> features)
        {
            StatReportFormat statDatFormat = StatReportFormat.None;

            foreach (string rt in GetList(features, ReportTypeListValue))
            {
                statDatFormat |= rt.AsStatReportFormat();
            }

            return statDatFormat;
        }

        /// <summary>
        /// Get TreatAsFiles from feature list
        /// </summary>
        protected TreatAsFiles GetTreatAsFiles(Dictionary<string, Library.Help.Feature> features)
        {
            TreatAsFiles asFiles = 0x00;
            if (GetBoolean(features, AaruFormatsAsFilesValue))
                asFiles |= TreatAsFiles.AaruFormats;
            if (GetBoolean(features, ArchivesAsFilesValue))
                asFiles |= TreatAsFiles.Archives;
            if (GetBoolean(features, ChdsAsFilesValue))
                asFiles |= TreatAsFiles.CHDs;

            return asFiles;
        }

        /// <summary>
        /// Get update fields from feature list
        /// </summary>
        protected List<Field> GetUpdateFields(Dictionary<string, Library.Help.Feature> features)
        {
            List<Field> updateFields = new List<Field>();

            if (GetBoolean(features, UpdateDescriptionValue))
            {
                Globals.Logger.User($"This flag '{(UpdateDescriptionValue)}' is deprecated, please use {(string.Join(", ", UpdateFieldListInput.Flags))} instead. Please refer to README.1ST or the help feature for more details.");
                updateFields.Add(Field.Machine_Description);
            }

            if (GetBoolean(features, UpdateGameTypeValue))
            {
                Globals.Logger.User($"This flag '{(UpdateGameTypeValue)}' is deprecated, please use {(string.Join(", ", UpdateFieldListInput.Flags))} instead. Please refer to README.1ST or the help feature for more details.");
                updateFields.Add(Field.Machine_Type);
            }

            if (GetBoolean(features, UpdateHashesValue))
            {
                Globals.Logger.User($"This flag '{(UpdateHashesValue)}' is deprecated, please use {(string.Join(", ", UpdateFieldListInput.Flags))} instead. Please refer to README.1ST or the help feature for more details.");
                updateFields.Add(Field.DatItem_CRC);
                updateFields.Add(Field.DatItem_MD5);
#if NET_FRAMEWORK
                updateFields.Add(Field.DatItem_RIPEMD160);
#endif
                updateFields.Add(Field.DatItem_SHA1);
                updateFields.Add(Field.DatItem_SHA256);
                updateFields.Add(Field.DatItem_SHA384);
                updateFields.Add(Field.DatItem_SHA512);
                updateFields.Add(Field.DatItem_SpamSum);
            }

            if (GetBoolean(features, UpdateManufacturerValue))
            {
                Globals.Logger.User($"This flag '{(UpdateManufacturerValue)}' is deprecated, please use {(string.Join(", ", UpdateFieldListInput.Flags))} instead. Please refer to README.1ST or the help feature for more details.");
                updateFields.Add(Field.Machine_Manufacturer);
            }

            if (GetBoolean(features, UpdateNamesValue))
            {
                Globals.Logger.User($"This flag '{(UpdateNamesValue)}' is deprecated, please use {(string.Join(", ", UpdateFieldListInput.Flags))} instead. Please refer to README.1ST or the help feature for more details.");
                updateFields.Add(Field.DatItem_Name);
            }

            if (GetBoolean(features, UpdateParentsValue))
            {
                Globals.Logger.User($"This flag '{(UpdateParentsValue)}' is deprecated, please use {(string.Join(", ", UpdateFieldListInput.Flags))} instead. Please refer to README.1ST or the help feature for more details.");
                updateFields.Add(Field.Machine_CloneOf);
                updateFields.Add(Field.Machine_RomOf);
                updateFields.Add(Field.Machine_SampleOf);
            }

            if (GetBoolean(features, UpdateYearValue))
            {
                Globals.Logger.User($"This flag '{(UpdateYearValue)}' is deprecated, please use {(string.Join(", ", UpdateFieldListInput.Flags))} instead. Please refer to README.1ST or the help feature for more details.");
                updateFields.Add(Field.Machine_Year);
            }

            foreach (string fieldName in GetList(features, UpdateFieldListValue))
            {
                updateFields.Add(fieldName.AsField());
            }

            return updateFields;
        }

        /// <summary>
        /// Get UpdateMode from feature list
        /// </summary>
        protected UpdateMode GetUpdateMode(Dictionary<string, Library.Help.Feature> features)
        {
            UpdateMode updateMode = UpdateMode.None;

            if (GetBoolean(features, DiffAllValue))
                updateMode |= UpdateMode.AllDiffs;

            if (GetBoolean(features, BaseReplaceValue))
                updateMode |= UpdateMode.BaseReplace;

            if (GetBoolean(features, DiffAgainstValue))
                updateMode |= UpdateMode.DiffAgainst;

            if (GetBoolean(features, DiffCascadeValue))
                updateMode |= UpdateMode.DiffCascade;

            if (GetBoolean(features, DiffDuplicatesValue))
                updateMode |= UpdateMode.DiffDupesOnly;

            if (GetBoolean(features, DiffIndividualsValue))
                updateMode |= UpdateMode.DiffIndividualsOnly;

            if (GetBoolean(features, DiffNoDuplicatesValue))
                updateMode |= UpdateMode.DiffNoDupesOnly;

            if (GetBoolean(features, DiffReverseCascadeValue))
                updateMode |= UpdateMode.DiffReverseCascade;

            if (GetBoolean(features, MergeValue))
                updateMode |= UpdateMode.Merge;

            if (GetBoolean(features, ReverseBaseReplaceValue))
                updateMode |= UpdateMode.ReverseBaseReplace;

            return updateMode;
        }

        #endregion

        #region Private Specific Extraction

        /// <summary>
        /// Get Cleaner from feature list
        /// </summary>
        private Cleaner GetCleaner(Dictionary<string, Library.Help.Feature> features)
        {
            Cleaner cleaner = new Cleaner()
            {
                Clean = GetBoolean(features, CleanValue),
                DedupeRoms = GetDedupeType(features),
                DescriptionAsName = GetBoolean(features, DescriptionAsNameValue),
                KeepEmptyGames = GetBoolean(features, KeepEmptyGamesValue),
                OneGamePerRegion = GetBoolean(features, OneGamePerRegionValue),
                RegionList = GetList(features, RegionListValue),
                OneRomPerGame = GetBoolean(features, OneRomPerGameValue),
                RemoveUnicode = GetBoolean(features, RemoveUnicodeValue),
                Root = GetString(features, RootDirStringValue),
                SceneDateStrip = GetBoolean(features, SceneDateStripValue),
                Single = GetBoolean(features, SingleSetValue),
                Trim = GetBoolean(features, TrimValue),
            };

            foreach (string fieldName in GetList(features, ExcludeFieldListValue))
            {
                cleaner.ExcludeFields.Add(fieldName.AsField());
            }

            return cleaner;
        }

        /// <summary>
        /// Get DatHeader from feature list
        /// </summary>
        private DatHeader GetDatHeader(Dictionary<string, Library.Help.Feature> features)
        {
            // TODO: Sort this by region, like the actual header
            DatHeader datHeader = new DatHeader
            {
                AddExtension = GetString(features, AddExtensionStringValue),
                Author = GetString(features, AuthorStringValue),
                Category = GetString(features, CategoryStringValue),
                Comment = GetString(features, CommentStringValue),
                Date = GetString(features, DateStringValue),
                Description = GetString(features, DescriptionStringValue),
                Email = GetString(features, EmailStringValue),
                FileName = GetString(features, FilenameStringValue),
                ForceMerging = GetString(features, ForceMergingStringValue).AsMergingFlag(),
                ForceNodump = GetString(features, ForceNodumpStringValue).AsNodumpFlag(),
                ForcePacking = GetString(features, ForcePackingStringValue).AsPackingFlag(),
                GameName = GetBoolean(features, GamePrefixValue),
                HeaderSkipper = GetString(features, HeaderStringValue),
                Homepage = GetString(features, HomepageStringValue),
                Name = GetString(features, NameStringValue),
                Postfix = GetString(features, PostfixStringValue),
                Prefix = GetString(features, PrefixStringValue),
                Quotes = GetBoolean(features, QuotesValue),
                RemoveExtension = GetBoolean(features, RemoveExtensionsValue),
                ReplaceExtension = GetString(features, ReplaceExtensionStringValue),
                RootDir = GetString(features, RootStringValue),
                Type = GetBoolean(features, SuperdatValue) ? "SuperDAT" : null,
                Url = GetString(features, UrlStringValue),
                UseRomName = GetBoolean(features, RomsValue),
                Version = GetString(features, VersionStringValue),
            };

            // Get the depot information
            datHeader.InputDepot = new DepotInformation(
                GetBoolean(features, DepotValue),
                GetInt32(features, DepotDepthInt32Value));
            datHeader.OutputDepot = new DepotInformation(
                GetBoolean(features, RombaValue),
                GetInt32(features, RombaDepthInt32Value));

            bool deprecated = GetBoolean(features, DeprecatedValue);
            foreach (string ot in GetList(features, OutputTypeListValue))
            {
                DatFormat dftemp = ot.AsDatFormat();
                if (dftemp == DatFormat.Logiqx && deprecated)
                    datHeader.DatFormat |= DatFormat.LogiqxDeprecated;
                else
                    datHeader.DatFormat |= dftemp;
            }

            return datHeader;
        }

        /// <summary>
        /// Get DedupeType from feature list
        /// </summary>
        private DedupeType GetDedupeType(Dictionary<string, Library.Help.Feature> features)
        {
            if (GetBoolean(features, DedupValue))
                return DedupeType.Full;
            else if (GetBoolean(features, GameDedupValue))
                return DedupeType.Game;
            else
                return DedupeType.None;
        }

        /// <summary>
        /// Get ExtraIni from feature list
        /// </summary>
        private ExtraIni GetExtras(Dictionary<string, Library.Help.Feature> features)
        {
            ExtraIni extraIni = new ExtraIni();
            extraIni.PopulateFromList(GetList(features, ExtraIniListValue));
            return extraIni;
        }

        /// <summary>
        /// Get Filter from feature list
        /// </summary>
        private Filter GetFilter(Dictionary<string, Library.Help.Feature> features)
        {
            Filter filter = new Filter();

            // Use the Filter flag first
            List<string> filterPairs = GetList(features, FilterListValue);
            filter.PopulateFromList(filterPairs);

            #region Obsoleted Inputs

            // Category
            if (features.ContainsKey(NotCategoryListValue))
            {
                Globals.Logger.User($"This flag '{(NotCategoryListValue)}' is deprecated, please use {(string.Join(", ", FilterListInput.Flags))} instead. Please refer to README.1ST or the help feature for more details.");
                filter.SetFilter(Field.Machine_Category, GetList(features, NotCategoryListValue), true);
            }
            if (features.ContainsKey(CategoryListValue))
            {
                Globals.Logger.User($"This flag '{(CategoryListValue)}' is deprecated, please use {(string.Join(", ", FilterListInput.Flags))} instead. Please refer to README.1ST or the help feature for more details.");
                filter.SetFilter(Field.Machine_Category, GetList(features, CategoryListValue), false);
            }

            // CRC
            if (features.ContainsKey(NotCrcListValue))
            {
                Globals.Logger.User($"This flag '{(NotCrcListValue)}' is deprecated, please use {(string.Join(", ", FilterListInput.Flags))} instead. Please refer to README.1ST or the help feature for more details.");
                filter.SetFilter(Field.DatItem_CRC, GetList(features, NotCrcListValue), true);
            }
            if (features.ContainsKey(CrcListValue))
            {
                Globals.Logger.User($"This flag '{(CrcListValue)}' is deprecated, please use {(string.Join(", ", FilterListInput.Flags))} instead. Please refer to README.1ST or the help feature for more details.");
                filter.SetFilter(Field.DatItem_CRC, GetList(features, NotCrcListValue), false);
            }

            // Item name
            if (features.ContainsKey(NotItemNameListValue))
            {
                Globals.Logger.User($"This flag '{(NotItemNameListValue)}' is deprecated, please use {(string.Join(", ", FilterListInput.Flags))} instead. Please refer to README.1ST or the help feature for more details.");
                filter.SetFilter(Field.DatItem_Name, GetList(features, NotItemNameListValue), true);
            }
            if (features.ContainsKey(ItemNameListValue))
            {
                Globals.Logger.User($"This flag '{(ItemNameListValue)}' is deprecated, please use {(string.Join(", ", FilterListInput.Flags))} instead. Please refer to README.1ST or the help feature for more details.");
                filter.SetFilter(Field.DatItem_Name, GetList(features, ItemNameListValue), false);
            }

            // Item status
            if (features.ContainsKey(NotStatusListValue))
            {
                Globals.Logger.User($"This flag '{(NotStatusListValue)}' is deprecated, please use {(string.Join(", ", FilterListInput.Flags))} instead. Please refer to README.1ST or the help feature for more details.");
                filter.SetFilter(Field.DatItem_Status, GetList(features, NotStatusListValue), true);
            }
            if (features.ContainsKey(StatusListValue))
            {
                Globals.Logger.User($"This flag '{(StatusListValue)}' is deprecated, please use {(string.Join(", ", FilterListInput.Flags))} instead. Please refer to README.1ST or the help feature for more details.");
                filter.SetFilter(Field.DatItem_Status, GetList(features, StatusListValue), false);
            }

            // Item type
            if (features.ContainsKey(NotItemTypeListValue))
            {
                Globals.Logger.User($"This flag '{(NotItemTypeListValue)}' is deprecated, please use {(string.Join(", ", FilterListInput.Flags))} instead. Please refer to README.1ST or the help feature for more details.");
                filter.SetFilter(Field.DatItem_Type, GetList(features, NotItemTypeListValue), true);
            }
            if (features.ContainsKey(ItemTypeListValue))
            {
                Globals.Logger.User($"This flag '{(ItemTypeListValue)}' is deprecated, please use {(string.Join(", ", FilterListInput.Flags))} instead. Please refer to README.1ST or the help feature for more details.");
                filter.SetFilter(Field.DatItem_Type, GetList(features, ItemTypeListValue), false);
            }

            // Machine description
            if (features.ContainsKey(NotGameDescriptionListValue))
            {
                Globals.Logger.User($"This flag '{(NotGameDescriptionListValue)}' is deprecated, please use {(string.Join(", ", FilterListInput.Flags))} instead. Please refer to README.1ST or the help feature for more details.");
                filter.SetFilter(Field.Machine_Description, GetList(features, NotGameDescriptionListValue), true);
            }
            if (features.ContainsKey(GameDescriptionListValue))
            {
                Globals.Logger.User($"This flag '{(GameDescriptionListValue)}' is deprecated, please use {(string.Join(", ", FilterListInput.Flags))} instead. Please refer to README.1ST or the help feature for more details.");
                filter.SetFilter(Field.Machine_Description, GetList(features, GameDescriptionListValue), false);
            }

            // Machine name
            if (features.ContainsKey(NotGameNameListValue))
            {
                Globals.Logger.User($"This flag '{(NotGameNameListValue)}' is deprecated, please use {(string.Join(", ", FilterListInput.Flags))} instead. Please refer to README.1ST or the help feature for more details.");
                filter.SetFilter(Field.Machine_Name, GetList(features, NotGameNameListValue), true);
            }
            if (features.ContainsKey(GameNameListValue))
            {
                Globals.Logger.User($"This flag '{(GameNameListValue)}' is deprecated, please use {(string.Join(", ", FilterListInput.Flags))} instead. Please refer to README.1ST or the help feature for more details.");
                filter.SetFilter(Field.Machine_Name, GetList(features, GameNameListValue), false);
            }

            // Machine type
            if (features.ContainsKey(NotGameTypeListValue))
            {
                Globals.Logger.User($"This flag '{(NotGameTypeListValue)}' is deprecated, please use {(string.Join(", ", FilterListInput.Flags))} instead. Please refer to README.1ST or the help feature for more details.");
                filter.SetFilter(Field.Machine_Type, GetList(features, NotGameTypeListValue), true);
            }
            if (features.ContainsKey(GameTypeListValue))
            {
                Globals.Logger.User($"This flag '{(GameTypeListValue)}' is deprecated, please use {(string.Join(", ", FilterListInput.Flags))} instead. Please refer to README.1ST or the help feature for more details.");
                filter.SetFilter(Field.Machine_Type, GetList(features, GameTypeListValue), false);
            }

            // MD5
            if (features.ContainsKey(NotMd5ListValue))
            {
                Globals.Logger.User($"This flag '{(NotMd5ListValue)}' is deprecated, please use {(string.Join(", ", FilterListInput.Flags))} instead. Please refer to README.1ST or the help feature for more details.");
                filter.SetFilter(Field.DatItem_MD5, GetList(features, NotMd5ListValue), true);
            }
            if (features.ContainsKey(Md5ListValue))
            {
                Globals.Logger.User($"This flag '{Md5ListValue}' is deprecated, please use {string.Join(", ", FilterListInput.Flags)} instead. Please refer to README.1ST or the help feature for more details.");
                filter.SetFilter(Field.DatItem_MD5, GetList(features, Md5ListValue), false);
            }

#if NET_FRAMEWORK
            // RIPEMD160
            if (features.ContainsKey(NotRipeMd160ListValue))
            {
                Globals.Logger.User($"This flag '{NotRipeMd160ListValue}' is deprecated, please use {string.Join(", ", FilterListInput.Flags)} instead. Please refer to README.1ST or the help feature for more details.");
                filter.SetFilter(Field.DatItem_RIPEMD160, GetList(features, NotRipeMd160ListValue), true);
            }
            if (features.ContainsKey(RipeMd160ListValue))
            {
                Globals.Logger.User($"This flag '{RipeMd160ListValue}' is deprecated, please use {string.Join(", ", FilterListInput.Flags)} instead. Please refer to README.1ST or the help feature for more details.");
                filter.SetFilter(Field.DatItem_RIPEMD160, GetList(features, RipeMd160ListValue), false);
            }
#endif

            // Runnable
            if (features.ContainsKey(NotRunnableValue))
            {
                Globals.Logger.User($"This flag '{NotRunnableValue}' is deprecated, please use {string.Join(", ", FilterListInput.Flags)} instead. Please refer to README.1ST or the help feature for more details.");
                filter.SetFilter(Field.Machine_Runnable, string.Empty, true);
            }
            if (features.ContainsKey(RunnableValue))
            {
                Globals.Logger.User($"This flag '{RunnableValue}' is deprecated, please use {string.Join(", ", FilterListInput.Flags)} instead. Please refer to README.1ST or the help feature for more details.");
                filter.SetFilter(Field.Machine_Runnable, string.Empty, false);
            }

            // SHA1
            if (features.ContainsKey(NotSha1ListValue))
            {
                Globals.Logger.User($"This flag '{NotSha1ListValue}' is deprecated, please use {string.Join(", ", FilterListInput.Flags)} instead. Please refer to README.1ST or the help feature for more details.");
                filter.SetFilter(Field.DatItem_SHA1, GetList(features, NotSha1ListValue), true);
            }
            if (features.ContainsKey(Sha1ListValue))
            {
                Globals.Logger.User($"This flag '{Sha1ListValue}' is deprecated, please use {string.Join(", ", FilterListInput.Flags)} instead. Please refer to README.1ST or the help feature for more details.");
                filter.SetFilter(Field.DatItem_SHA1, GetList(features, Sha1ListValue), false);
            }

            // SHA256
            if (features.ContainsKey(NotSha256ListValue))
            {
                Globals.Logger.User($"This flag '{NotSha256ListValue}' is deprecated, please use {string.Join(",", FilterListInput.Flags)} instead. Please refer to README.1ST or the help feature for more details.");
                filter.SetFilter(Field.DatItem_SHA256, GetList(features, NotSha256ListValue), true);
            }
            if (features.ContainsKey(Sha256ListValue))
            {
                Globals.Logger.User($"This flag '{Sha256ListValue}' is deprecated, please use {string.Join(",", FilterListInput.Flags)} instead. Please refer to README.1ST or the help feature for more details.");
                filter.SetFilter(Field.DatItem_SHA256, GetList(features, Sha256ListValue), false);
            }

            // SHA384
            if (features.ContainsKey(NotSha384ListValue))
            {
                Globals.Logger.User($"This flag '{NotSha384ListValue}' is deprecated, please use {string.Join(",", FilterListInput.Flags)} instead. Please refer to README.1ST or the help feature for more details.");
                filter.SetFilter(Field.DatItem_SHA384, GetList(features, NotSha384ListValue), true);
            }
            if (features.ContainsKey(Sha384ListValue))
            {
                Globals.Logger.User($"This flag '{Sha384ListValue}' is deprecated, please use {string.Join(",", FilterListInput.Flags)} instead. Please refer to README.1ST or the help feature for more details.");
                filter.SetFilter(Field.DatItem_SHA384, GetList(features, Sha384ListValue), false);
            }

            // SHA512
            if (features.ContainsKey(NotSha512ListValue))
            {
                Globals.Logger.User($"This flag '{NotSha512ListValue}' is deprecated, please use {string.Join(",", FilterListInput.Flags)} instead. Please refer to README.1ST or the help feature for more details.");
                filter.SetFilter(Field.DatItem_SHA512, GetList(features, NotSha512ListValue), true);
            }
            if (features.ContainsKey(Sha512ListValue))
            {
                Globals.Logger.User($"This flag '{Sha512ListValue}' is deprecated, please use {string.Join(",", FilterListInput.Flags)} instead. Please refer to README.1ST or the help feature for more details.");
                filter.SetFilter(Field.DatItem_SHA512, GetList(features, Sha512ListValue), false);
            }

            // Size
            if (features.ContainsKey(LessStringValue))
            {
                Globals.Logger.User($"This flag '{LessStringValue}' is deprecated, please use {string.Join(",", FilterListInput.Flags)} instead. Please refer to README.1ST or the help feature for more details.");
                var value = Sanitizer.ToSize(GetString(features, LessStringValue));
                filter.SetFilter(Field.DatItem_Size, $"<{value}", false);
            }
            if (features.ContainsKey(EqualStringValue))
            {
                Globals.Logger.User($"This flag '{EqualStringValue}' is deprecated, please use {string.Join(",", FilterListInput.Flags)} instead. Please refer to README.1ST or the help feature for more details.");
                var value = Sanitizer.ToSize(GetString(features, EqualStringValue));
                filter.SetFilter(Field.DatItem_Size, $"={value}", false);
            }
            if (features.ContainsKey(GreaterStringValue))
            {
                Globals.Logger.User($"This flag '{GreaterStringValue}' is deprecated, please use {string.Join(",", FilterListInput.Flags)} instead. Please refer to README.1ST or the help feature for more details.");
                var value = Sanitizer.ToSize(GetString(features, GreaterStringValue));
                filter.SetFilter(Field.DatItem_Size, $">{value}", false);
            }

            #endregion

            #region Additional Filter flags

            // Include 'of" in game filters
            filter.IncludeOfInGame = GetBoolean(features, MatchOfTagsValue);

            #endregion

            return filter;
        }

        #endregion
    }
}
