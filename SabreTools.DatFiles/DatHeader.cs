using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.DatFiles.Formats;
using SabreTools.Filter;

namespace SabreTools.DatFiles
{
    /// <summary>
    /// Represents all possible DAT header information
    /// </summary>
    [JsonObject("header"), XmlRoot("header")]
    public class DatHeader : ICloneable
    {
        #region Constants

        /// <summary>
        /// Add a new extension to all items
        /// </summary>
        public const string AddExtensionKey = "ADDEXTENSION";

        /// <summary>
        /// Read or write format
        /// </summary>
        public const string DatFormatKey = "DATFORMAT";

        /// <summary>
        /// External name of the DAT
        /// </summary>
        public const string FileNameKey = "FILENAME";

        /// <summary>
        /// Output the machine name
        /// </summary>
        public const string GameNameKey = "GAMENAME";

        /// <summary>
        /// Input depot information
        /// </summary>
        public const string InputDepotKey = "INPUTDEPOT";

        /// <summary>
        /// Output depot information
        /// </summary>
        public const string OutputDepotKey = "OUTPUTDEPOT";

        /// <summary>
        /// Text to append to all outputted lines
        /// </summary>
        public const string PostfixKey = "POSTFIX";

        /// <summary>
        /// Text to prepend to all outputted lines
        /// </summary>
        public const string PrefixKey = "PREFIX";

        /// <summary>
        /// Wrap quotes around the entire line, sans prefix and postfix
        /// </summary>
        public const string QuotesKey = "QUOTES";

        /// <summary>
        /// Remove all item extensions
        /// </summary>
        public const string RemoveExtensionKey = "REMOVEEXTENSION";

        /// <summary>
        /// Replace all item extensions
        /// </summary>
        public const string ReplaceExtensionKey = "REPLACEEXTENSION";

        /// <summary>
        /// Output the item name
        /// </summary>
        public const string UseRomNameKey = "USEROMNAME";

        #endregion

        #region Fields

        [JsonIgnore]
        public bool InfosSpecified
        {
            get
            {
                var infos = GetFieldValue<OfflineListInfo[]?>(Models.Metadata.Header.InfosKey);
                return infos != null && infos.Length > 0;
            }
        }

        [JsonIgnore]
        public bool CanOpenSpecified
        {
            get
            {
                var canOpen = GetFieldValue<Models.OfflineList.CanOpen[]?>(Models.Metadata.Header.CanOpenKey);
                return canOpen != null && canOpen.Length > 0;
            }
        }

        [JsonIgnore]
        public bool NewDatSpecified
        {
            get
            {
                return GetFieldValue<string?>("DATVERSIONURL") != null
                    //&& GetFieldValue<Models.OfflineList.DatUrl?>("DATURL") != null // TODO: Add to internal model
                    && GetFieldValue<string?>("IMURL") != null;
            }
        }

        /// <summary>
        /// Internal Header model
        /// </summary>
        [JsonIgnore]
        private readonly Models.Metadata.Header _header = [];

        #endregion

        #region Instance Methods

        #region Constructors

        public DatHeader() { }

        public DatHeader(Models.Metadata.Header header)
        {
            // Get all fields to automatically copy without processing
            var nonItemFields = TypeHelper.GetConstants(typeof(Models.Metadata.Header));
            if (nonItemFields == null)
                return;

            // Populate the internal machine from non-filter fields
            _header = [];
            foreach (string fieldName in nonItemFields)
            {
                if (header.ContainsKey(fieldName))
                    _header[fieldName] = header[fieldName];
            }
        }

        #endregion

        #region Accessors

        /// <summary>
        /// Get the value from a field based on the type provided
        /// </summary>
        /// <typeparam name="T">Type of the value to get from the internal model</typeparam>
        /// <param name="fieldName">Field to retrieve</param>
        /// <returns>Value from the field, if possible</returns>
        public T? GetFieldValue<T>(string? fieldName)
        {
            // Invalid field cannot be processed
            if (string.IsNullOrEmpty(fieldName))
                return default;

            // Get the value based on the type
            return _header.Read<T>(fieldName!);
        }

        /// <summary>
        /// Set the value from a field based on the type provided
        /// </summary>
        /// <typeparam name="T">Type of the value to set in the internal model</typeparam>
        /// <param name="fieldName">Field to set</param>
        /// <param name="value">Value to set</param>
        /// <returns>True if the value was set, false otherwise</returns>
        public bool SetFieldValue<T>(string? fieldName, T? value)
        {
            // Invalid field cannot be processed
            if (string.IsNullOrEmpty(fieldName))
                return false;

            // Set the value based on the type
            _header[fieldName!] = value;
            return true;
        }

        /// <summary>
        /// Get a clone of the current internal model
        /// </summary>
        public Models.Metadata.Header GetInternalClone() => (_header.Clone() as Models.Metadata.Header)!;

        #endregion

        #region Cloning Methods

        /// <summary>
        /// Clone the current header
        /// </summary>
        public object Clone()
        {
            var header = new DatHeader();
            header.SetFieldValue<string?>(DatHeader.AddExtensionKey, GetFieldValue<string?>(DatHeader.AddExtensionKey));
            header.SetFieldValue<string?>(Models.Metadata.Header.AuthorKey, GetFieldValue<string?>(Models.Metadata.Header.AuthorKey));
            header.SetFieldValue<MergingFlag>(Models.Metadata.Header.BiosModeKey, GetFieldValue<MergingFlag>(Models.Metadata.Header.BiosModeKey));
            header.SetFieldValue<string?>(Models.Metadata.Header.BuildKey, GetFieldValue<string?>(Models.Metadata.Header.BuildKey));
            header.SetFieldValue<string[]?>(Models.Metadata.Header.CanOpenKey, GetFieldValue<string[]?>(Models.Metadata.Header.CanOpenKey)); // TODO: Perform a deep clone
            header.SetFieldValue<string?>(Models.Metadata.Header.CategoryKey, GetFieldValue<string?>(Models.Metadata.Header.CategoryKey));
            header.SetFieldValue<string?>(Models.Metadata.Header.CommentKey, GetFieldValue<string?>(Models.Metadata.Header.CommentKey));
            header.SetFieldValue<string?>(Models.Metadata.Header.DateKey, GetFieldValue<string?>(Models.Metadata.Header.DateKey));
            header.SetFieldValue<DatFormat>(DatHeader.DatFormatKey, GetFieldValue<DatFormat>(DatHeader.DatFormatKey));
            header.SetFieldValue<string?>(Models.Metadata.Header.DatVersionKey, GetFieldValue<string?>(Models.Metadata.Header.DatVersionKey));
            header.SetFieldValue<bool?>(Models.Metadata.Header.DebugKey, GetFieldValue<bool?>(Models.Metadata.Header.DebugKey));
            header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, GetFieldValue<string?>(Models.Metadata.Header.DescriptionKey));
            header.SetFieldValue<string?>(Models.Metadata.Header.EmailKey, GetFieldValue<string?>(Models.Metadata.Header.EmailKey));
            header.SetFieldValue<string?>(DatHeader.FileNameKey, GetFieldValue<string?>(DatHeader.FileNameKey));
            header.SetFieldValue<MergingFlag>(Models.Metadata.Header.ForceMergingKey, GetFieldValue<MergingFlag>(Models.Metadata.Header.ForceMergingKey));
            header.SetFieldValue<NodumpFlag>(Models.Metadata.Header.ForceNodumpKey, GetFieldValue<NodumpFlag>(Models.Metadata.Header.ForceNodumpKey));
            header.SetFieldValue<PackingFlag>(Models.Metadata.Header.ForcePackingKey, GetFieldValue<PackingFlag>(Models.Metadata.Header.ForcePackingKey));
            header.SetFieldValue<bool>(DatHeader.GameNameKey, GetFieldValue<bool>(DatHeader.GameNameKey));
            header.SetFieldValue<string?>(Models.Metadata.Header.HeaderKey, GetFieldValue<string?>(Models.Metadata.Header.HeaderKey));
            header.SetFieldValue<string?>(Models.Metadata.Header.HomepageKey, GetFieldValue<string?>(Models.Metadata.Header.HomepageKey));
            header.SetFieldValue<string?>(Models.Metadata.Header.IdKey, GetFieldValue<string?>(Models.Metadata.Header.IdKey));
            header.SetFieldValue<OfflineListInfo[]?>(Models.Metadata.Header.InfosKey, GetFieldValue<OfflineListInfo[]?>(Models.Metadata.Header.InfosKey)); // TODO: Perform a deep clone
            header.SetFieldValue<DepotInformation?>(DatHeader.InputDepotKey, GetFieldValue<DepotInformation?>(DatHeader.InputDepotKey)?.Clone() as DepotInformation);
            header.SetFieldValue<DepotInformation?>(DatHeader.OutputDepotKey, GetFieldValue<DepotInformation?>(DatHeader.OutputDepotKey)?.Clone() as DepotInformation);
            header.SetFieldValue<bool?>(Models.Metadata.Header.LockBiosModeKey, GetFieldValue<bool?>(Models.Metadata.Header.LockBiosModeKey));
            header.SetFieldValue<bool?>(Models.Metadata.Header.LockRomModeKey, GetFieldValue<bool?>(Models.Metadata.Header.LockRomModeKey));
            header.SetFieldValue<bool?>(Models.Metadata.Header.LockSampleModeKey, GetFieldValue<bool?>(Models.Metadata.Header.LockSampleModeKey));
            header.SetFieldValue<string?>(Models.Metadata.Header.MameConfigKey, GetFieldValue<string?>(Models.Metadata.Header.MameConfigKey));
            header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, GetFieldValue<string?>(Models.Metadata.Header.NameKey));
            header.SetFieldValue<string?>(DatHeader.PostfixKey, GetFieldValue<string?>(DatHeader.PostfixKey));
            header.SetFieldValue<string?>(DatHeader.PrefixKey, GetFieldValue<string?>(DatHeader.PrefixKey));
            header.SetFieldValue<bool>(DatHeader.QuotesKey, GetFieldValue<bool>(DatHeader.QuotesKey));
            header.SetFieldValue<bool>(DatHeader.RemoveExtensionKey, GetFieldValue<bool>(DatHeader.RemoveExtensionKey));
            header.SetFieldValue<string?>(DatHeader.ReplaceExtensionKey, GetFieldValue<string?>(DatHeader.ReplaceExtensionKey));
            header.SetFieldValue<string?>(Models.Metadata.Header.RomTitleKey, GetFieldValue<string?>(Models.Metadata.Header.RomTitleKey));
            header.SetFieldValue<MergingFlag>(Models.Metadata.Header.RomModeKey, GetFieldValue<MergingFlag>(Models.Metadata.Header.RomModeKey));
            header.SetFieldValue<string?>(Models.Metadata.Header.RootDirKey, GetFieldValue<string?>(Models.Metadata.Header.RootDirKey));
            header.SetFieldValue<MergingFlag>(Models.Metadata.Header.SampleModeKey, GetFieldValue<MergingFlag>(Models.Metadata.Header.SampleModeKey));
            header.SetFieldValue<string?>(Models.Metadata.Header.ScreenshotsHeightKey, GetFieldValue<string?>(Models.Metadata.Header.ScreenshotsHeightKey));
            header.SetFieldValue<string?>(Models.Metadata.Header.ScreenshotsWidthKey, GetFieldValue<string?>(Models.Metadata.Header.ScreenshotsWidthKey));
            header.SetFieldValue<string?>(Models.Metadata.Header.SystemKey, GetFieldValue<string?>(Models.Metadata.Header.SystemKey));
            header.SetFieldValue<string?>(Models.Metadata.Header.TypeKey, GetFieldValue<string?>(Models.Metadata.Header.TypeKey));
            header.SetFieldValue<string?>(Models.Metadata.Header.UrlKey, GetFieldValue<string?>(Models.Metadata.Header.UrlKey));
            header.SetFieldValue<bool>(DatHeader.UseRomNameKey, GetFieldValue<bool>(DatHeader.UseRomNameKey));
            header.SetFieldValue<string?>(Models.Metadata.Header.VersionKey, GetFieldValue<string?>(Models.Metadata.Header.VersionKey));

            return header;
        }

        /// <summary>
        /// Clone the standard parts of the current header
        /// </summary>
        public DatHeader CloneStandard()
        {
            var header = new DatHeader();
            header.SetFieldValue<string?>(Models.Metadata.Header.AuthorKey, GetFieldValue<string?>(Models.Metadata.Header.AuthorKey));
            header.SetFieldValue<string?>(Models.Metadata.Header.CategoryKey, GetFieldValue<string?>(Models.Metadata.Header.CategoryKey));
            header.SetFieldValue<string?>(Models.Metadata.Header.CommentKey, GetFieldValue<string?>(Models.Metadata.Header.CommentKey));
            header.SetFieldValue<string?>(Models.Metadata.Header.DateKey, GetFieldValue<string?>(Models.Metadata.Header.DateKey));
            header.SetFieldValue<DatFormat>(DatHeader.DatFormatKey, GetFieldValue<DatFormat>(DatHeader.DatFormatKey));
            header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, GetFieldValue<string?>(Models.Metadata.Header.DescriptionKey));
            header.SetFieldValue<string?>(Models.Metadata.Header.EmailKey, GetFieldValue<string?>(Models.Metadata.Header.EmailKey));
            header.SetFieldValue<string?>(DatHeader.FileNameKey, GetFieldValue<string?>(DatHeader.FileNameKey));
            header.SetFieldValue<MergingFlag>(Models.Metadata.Header.ForceMergingKey, GetFieldValue<MergingFlag>(Models.Metadata.Header.ForceMergingKey));
            header.SetFieldValue<NodumpFlag>(Models.Metadata.Header.ForceNodumpKey, GetFieldValue<NodumpFlag>(Models.Metadata.Header.ForceNodumpKey));
            header.SetFieldValue<PackingFlag>(Models.Metadata.Header.ForcePackingKey, GetFieldValue<PackingFlag>(Models.Metadata.Header.ForcePackingKey));
            header.SetFieldValue<string?>(Models.Metadata.Header.HeaderKey, GetFieldValue<string?>(Models.Metadata.Header.HeaderKey));
            header.SetFieldValue<string?>(Models.Metadata.Header.HomepageKey, GetFieldValue<string?>(Models.Metadata.Header.HomepageKey));
            header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, GetFieldValue<string?>(Models.Metadata.Header.NameKey));
            header.SetFieldValue<string?>(Models.Metadata.Header.RootDirKey, GetFieldValue<string?>(Models.Metadata.Header.RootDirKey));
            header.SetFieldValue<string?>(Models.Metadata.Header.TypeKey, GetFieldValue<string?>(Models.Metadata.Header.TypeKey));
            header.SetFieldValue<string?>(Models.Metadata.Header.UrlKey, GetFieldValue<string?>(Models.Metadata.Header.UrlKey));
            header.SetFieldValue<string?>(Models.Metadata.Header.VersionKey, GetFieldValue<string?>(Models.Metadata.Header.VersionKey));

            return header;
        }

        /// <summary>
        /// Clone the filtering parts of the current header
        /// </summary>
        public DatHeader CloneFiltering()
        {
            var header = new DatHeader();
            header.SetFieldValue<string?>(DatHeader.AddExtensionKey, GetFieldValue<string?>(DatHeader.AddExtensionKey));
            header.SetFieldValue<DatFormat>(DatHeader.DatFormatKey, GetFieldValue<DatFormat>(DatHeader.DatFormatKey));
            header.SetFieldValue<bool>(DatHeader.GameNameKey, GetFieldValue<bool>(DatHeader.GameNameKey));
            header.SetFieldValue<DepotInformation?>(DatHeader.InputDepotKey, GetFieldValue<DepotInformation?>(DatHeader.InputDepotKey)?.Clone() as DepotInformation);
            header.SetFieldValue<DepotInformation?>(DatHeader.OutputDepotKey, GetFieldValue<DepotInformation?>(DatHeader.OutputDepotKey)?.Clone() as DepotInformation);
            header.SetFieldValue<string?>(DatHeader.PostfixKey, GetFieldValue<string?>(DatHeader.PostfixKey));
            header.SetFieldValue<string?>(DatHeader.PrefixKey, GetFieldValue<string?>(DatHeader.PrefixKey));
            header.SetFieldValue<bool>(DatHeader.RemoveExtensionKey, GetFieldValue<bool>(DatHeader.RemoveExtensionKey));
            header.SetFieldValue<string?>(DatHeader.ReplaceExtensionKey, GetFieldValue<string?>(DatHeader.ReplaceExtensionKey));
            header.SetFieldValue<bool>(DatHeader.QuotesKey, GetFieldValue<bool>(DatHeader.QuotesKey));
            header.SetFieldValue<bool>(DatHeader.UseRomNameKey, GetFieldValue<bool>(DatHeader.UseRomNameKey));

            return header;
        }

        /// <summary>
        /// Overwrite local values from another DatHeader if not default
        /// </summary>
        /// <param name="datHeader">DatHeader to get the values from</param>
        public void ConditionalCopy(DatHeader? datHeader)
        {
            if (datHeader == null)
                return;

            if (!string.IsNullOrEmpty(datHeader.GetFieldValue<string?>(DatHeader.FileNameKey)))
                SetFieldValue<string?>(DatHeader.FileNameKey, datHeader.GetFieldValue<string?>(DatHeader.FileNameKey));

            if (!string.IsNullOrEmpty(datHeader.GetFieldValue<string?>(Models.Metadata.Header.NameKey)))
                SetFieldValue<string?>(Models.Metadata.Header.NameKey, datHeader.GetFieldValue<string?>(Models.Metadata.Header.NameKey));

            if (!string.IsNullOrEmpty(datHeader.GetFieldValue<string?>(Models.Metadata.Header.DescriptionKey)))
                SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, datHeader.GetFieldValue<string?>(Models.Metadata.Header.DescriptionKey));

            if (!string.IsNullOrEmpty(datHeader.GetFieldValue<string?>(Models.Metadata.Header.RootDirKey)))
                SetFieldValue<string?>(Models.Metadata.Header.RootDirKey, datHeader.GetFieldValue<string?>(Models.Metadata.Header.RootDirKey));

            if (!string.IsNullOrEmpty(datHeader.GetFieldValue<string?>(Models.Metadata.Header.CategoryKey)))
                SetFieldValue<string?>(Models.Metadata.Header.CategoryKey, datHeader.GetFieldValue<string?>(Models.Metadata.Header.CategoryKey));

            if (!string.IsNullOrEmpty(datHeader.GetFieldValue<string?>(Models.Metadata.Header.VersionKey)))
                SetFieldValue<string?>(Models.Metadata.Header.VersionKey, datHeader.GetFieldValue<string?>(Models.Metadata.Header.VersionKey));

            if (!string.IsNullOrEmpty(datHeader.GetFieldValue<string?>(Models.Metadata.Header.DateKey)))
                SetFieldValue<string?>(Models.Metadata.Header.DateKey, datHeader.GetFieldValue<string?>(Models.Metadata.Header.DateKey));

            if (!string.IsNullOrEmpty(datHeader.GetFieldValue<string?>(Models.Metadata.Header.AuthorKey)))
                SetFieldValue<string?>(Models.Metadata.Header.AuthorKey, datHeader.GetFieldValue<string?>(Models.Metadata.Header.AuthorKey));

            if (!string.IsNullOrEmpty(datHeader.GetFieldValue<string?>(Models.Metadata.Header.EmailKey)))
                SetFieldValue<string?>(Models.Metadata.Header.EmailKey, datHeader.GetFieldValue<string?>(Models.Metadata.Header.EmailKey));

            if (!string.IsNullOrEmpty(datHeader.GetFieldValue<string?>(Models.Metadata.Header.HomepageKey)))
                SetFieldValue<string?>(Models.Metadata.Header.HomepageKey, datHeader.GetFieldValue<string?>(Models.Metadata.Header.HomepageKey));

            if (!string.IsNullOrEmpty(datHeader.GetFieldValue<string?>(Models.Metadata.Header.UrlKey)))
                SetFieldValue<string?>(Models.Metadata.Header.UrlKey, datHeader.GetFieldValue<string?>(Models.Metadata.Header.UrlKey));

            if (!string.IsNullOrEmpty(datHeader.GetFieldValue<string?>(Models.Metadata.Header.CommentKey)))
                SetFieldValue<string?>(Models.Metadata.Header.CommentKey, datHeader.GetFieldValue<string?>(Models.Metadata.Header.CommentKey));

            if (!string.IsNullOrEmpty(datHeader.GetFieldValue<string?>(Models.Metadata.Header.HeaderKey)))
                SetFieldValue<string?>(Models.Metadata.Header.HeaderKey, datHeader.GetFieldValue<string?>(Models.Metadata.Header.HeaderKey));

            if (!string.IsNullOrEmpty(datHeader.GetFieldValue<string?>(Models.Metadata.Header.TypeKey)))
                SetFieldValue<string?>(Models.Metadata.Header.TypeKey, datHeader.GetFieldValue<string?>(Models.Metadata.Header.TypeKey));

            if (datHeader.GetFieldValue<MergingFlag>(Models.Metadata.Header.ForceMergingKey) != MergingFlag.None)
                SetFieldValue<MergingFlag>(Models.Metadata.Header.ForceMergingKey, datHeader.GetFieldValue<MergingFlag>(Models.Metadata.Header.ForceMergingKey));

            if (datHeader.GetFieldValue<NodumpFlag>(Models.Metadata.Header.ForceNodumpKey) != NodumpFlag.None)
                SetFieldValue<NodumpFlag>(Models.Metadata.Header.ForceNodumpKey, datHeader.GetFieldValue<NodumpFlag>(Models.Metadata.Header.ForceNodumpKey));

            if (datHeader.GetFieldValue<PackingFlag>(Models.Metadata.Header.ForcePackingKey) != PackingFlag.None)
                SetFieldValue<PackingFlag>(Models.Metadata.Header.ForcePackingKey, datHeader.GetFieldValue<PackingFlag>(Models.Metadata.Header.ForcePackingKey));

            if (datHeader.GetFieldValue<DatFormat>(DatHeader.DatFormatKey) != 0x00)
                SetFieldValue<DatFormat>(DatHeader.DatFormatKey, datHeader.GetFieldValue<DatFormat>(DatHeader.DatFormatKey));

            if (!string.IsNullOrEmpty(datHeader.GetFieldValue<string?>(DatHeader.PrefixKey)))
                SetFieldValue<string?>(DatHeader.PrefixKey, datHeader.GetFieldValue<string?>(DatHeader.PrefixKey));

            if (!string.IsNullOrEmpty(datHeader.GetFieldValue<string?>(DatHeader.PostfixKey)))
                SetFieldValue<string?>(DatHeader.PostfixKey, datHeader.GetFieldValue<string?>(DatHeader.PostfixKey));

            if (!string.IsNullOrEmpty(datHeader.GetFieldValue<string?>(DatHeader.AddExtensionKey)))
                SetFieldValue<string?>(DatHeader.AddExtensionKey, datHeader.GetFieldValue<string?>(DatHeader.AddExtensionKey));

            if (!string.IsNullOrEmpty(datHeader.GetFieldValue<string?>(DatHeader.ReplaceExtensionKey)))
                SetFieldValue<string?>(DatHeader.ReplaceExtensionKey, datHeader.GetFieldValue<string?>(DatHeader.ReplaceExtensionKey));

            SetFieldValue<DepotInformation?>(DatHeader.InputDepotKey, datHeader.GetFieldValue<DepotInformation?>(DatHeader.InputDepotKey)?.Clone() as DepotInformation);
            SetFieldValue<DepotInformation?>(DatHeader.OutputDepotKey, datHeader.GetFieldValue<DepotInformation?>(DatHeader.OutputDepotKey)?.Clone() as DepotInformation);
            SetFieldValue<bool>(DatHeader.GameNameKey, datHeader.GetFieldValue<bool>(DatHeader.GameNameKey));
            SetFieldValue<bool>(DatHeader.QuotesKey, datHeader.GetFieldValue<bool>(DatHeader.QuotesKey));
            SetFieldValue<bool>(DatHeader.RemoveExtensionKey, datHeader.GetFieldValue<bool>(DatHeader.RemoveExtensionKey));
            SetFieldValue<bool>(DatHeader.UseRomNameKey, datHeader.GetFieldValue<bool>(DatHeader.UseRomNameKey));
        }

        #endregion

        #region Manipulation

        /// <summary>
        /// Runs a filter and determines if it passes or not
        /// </summary>
        /// <param name="filterRunner">Filter runner to use for checking</param>
        /// <returns>True if the Machine passes the filter, false otherwise</returns>
        public bool PassesFilter(FilterRunner filterRunner) => filterRunner.Run(_header);

        /// <summary>
        /// Remove a field from the header
        /// </summary>
        /// <param name="fieldName">Field to remove</param>
        /// <returns>True if the removal was successful, false otherwise</returns>
        public bool RemoveField(string fieldName)
            => FieldManipulator.RemoveField(_header, fieldName);

        /// <summary>
        /// Replace a field from another DatHeader
        /// </summary>
        /// <param name="other">DatHeader to replace field from</param>
        /// <param name="fieldName">Field to replace</param>
        /// <returns>True if the replacement was successful, false otherwise</returns>
        public bool ReplaceField(DatHeader? other, string? fieldName)
            => FieldManipulator.ReplaceField(other?._header, _header, fieldName);

        /// <summary>
        /// Set a field in the header from a mapping string
        /// </summary>
        /// <param name="fieldName">Field to set</param>
        /// <param name="value">String representing the value to set</param>
        /// <returns>True if the setting was successful, false otherwise</returns>
        /// <remarks>This only performs minimal validation before setting</remarks>
        public bool SetField(string? fieldName, string value)
            => FieldManipulator.SetField(_header, fieldName, value);

        #endregion

        #region Writing

        /// <summary>
        /// Generate a proper outfile name based on a DAT and output directory
        /// </summary>
        /// <param name="outDir">Output directory</param>
        /// <param name="overwrite">True if we ignore existing files (default), false otherwise</param>
        /// <returns>Dictionary of output formats mapped to file names</returns>
        public Dictionary<DatFormat, string> CreateOutFileNames(string outDir, bool overwrite = true)
        {
            // Create the output dictionary
            Dictionary<DatFormat, string> outfileNames = [];

            // Double check the outDir for the end delim
            if (!outDir.EndsWith(Path.DirectorySeparatorChar.ToString()))
                outDir += Path.DirectorySeparatorChar;

            // Get all used extensions
            List<string> usedExtensions = [];

            // Get the extensions from the output type

            #region .csv

            // CSV
#if NETFRAMEWORK
            if ((GetFieldValue<DatFormat>(DatHeader.DatFormatKey) & DatFormat.CSV) != 0)
#else
            if (GetFieldValue<DatFormat>(DatHeader.DatFormatKey).HasFlag(DatFormat.CSV))
#endif
            {
                outfileNames.Add(DatFormat.CSV, CreateOutFileNamesHelper(outDir, ".csv", overwrite));
                usedExtensions.Add(".csv");
            };

            #endregion

            #region .dat

            // ClrMamePro
#if NETFRAMEWORK
            if ((GetFieldValue<DatFormat>(DatHeader.DatFormatKey) & DatFormat.ClrMamePro) != 0)
#else
            if (GetFieldValue<DatFormat>(DatHeader.DatFormatKey).HasFlag(DatFormat.ClrMamePro))
#endif
            {
                outfileNames.Add(DatFormat.ClrMamePro, CreateOutFileNamesHelper(outDir, ".dat", overwrite));
                usedExtensions.Add(".dat");
            };

            // RomCenter
#if NETFRAMEWORK
            if ((GetFieldValue<DatFormat>(DatHeader.DatFormatKey) & DatFormat.RomCenter) != 0)
#else
            if (GetFieldValue<DatFormat>(DatHeader.DatFormatKey).HasFlag(DatFormat.RomCenter))
#endif
            {
                if (usedExtensions.Contains(".dat"))
                {
                    outfileNames.Add(DatFormat.RomCenter, CreateOutFileNamesHelper(outDir, ".rc.dat", overwrite));
                    usedExtensions.Add(".rc.dat");
                }
                else
                {
                    outfileNames.Add(DatFormat.RomCenter, CreateOutFileNamesHelper(outDir, ".dat", overwrite));
                    usedExtensions.Add(".dat");
                }
            }

            // DOSCenter
#if NETFRAMEWORK
            if ((GetFieldValue<DatFormat>(DatHeader.DatFormatKey) & DatFormat.DOSCenter) != 0)
#else
            if (GetFieldValue<DatFormat>(DatHeader.DatFormatKey).HasFlag(DatFormat.DOSCenter))
#endif
            {
                if (usedExtensions.Contains(".dat"))
                {
                    outfileNames.Add(DatFormat.DOSCenter, CreateOutFileNamesHelper(outDir, ".dc.dat", overwrite));
                    usedExtensions.Add(".dc.dat");
                }
                else
                {
                    outfileNames.Add(DatFormat.DOSCenter, CreateOutFileNamesHelper(outDir, ".dat", overwrite));
                    usedExtensions.Add(".dat");
                }
            }

            #endregion

            #region .json

            // JSON
#if NETFRAMEWORK
            if ((GetFieldValue<DatFormat>(DatHeader.DatFormatKey) & DatFormat.SabreJSON) != 0)
#else
            if (GetFieldValue<DatFormat>(DatHeader.DatFormatKey).HasFlag(DatFormat.SabreJSON))
#endif
            {
                outfileNames.Add(DatFormat.SabreJSON, CreateOutFileNamesHelper(outDir, ".json", overwrite));
                usedExtensions.Add(".json");
            }

            #endregion

            #region .md5

            // Redump MD5
#if NETFRAMEWORK
            if ((GetFieldValue<DatFormat>(DatHeader.DatFormatKey) & DatFormat.RedumpMD5) != 0)
#else
            if (GetFieldValue<DatFormat>(DatHeader.DatFormatKey).HasFlag(DatFormat.RedumpMD5))
#endif
            {
                outfileNames.Add(DatFormat.RedumpMD5, CreateOutFileNamesHelper(outDir, ".md5", overwrite));
                usedExtensions.Add(".md5");
            };

            #endregion

            #region .sfv

            // Redump SFV
#if NETFRAMEWORK
            if ((GetFieldValue<DatFormat>(DatHeader.DatFormatKey) & DatFormat.RedumpSFV) != 0)
#else
            if (GetFieldValue<DatFormat>(DatHeader.DatFormatKey).HasFlag(DatFormat.RedumpSFV))
#endif
            {
                outfileNames.Add(DatFormat.RedumpSFV, CreateOutFileNamesHelper(outDir, ".sfv", overwrite));
                usedExtensions.Add(".sfv");
            };

            #endregion

            #region .sha1

            // Redump SHA-1
#if NETFRAMEWORK
            if ((GetFieldValue<DatFormat>(DatHeader.DatFormatKey) & DatFormat.RedumpSHA1) != 0)
#else
            if (GetFieldValue<DatFormat>(DatHeader.DatFormatKey).HasFlag(DatFormat.RedumpSHA1))
#endif
            {
                outfileNames.Add(DatFormat.RedumpSHA1, CreateOutFileNamesHelper(outDir, ".sha1", overwrite));
                usedExtensions.Add(".sha1");
            };

            #endregion

            #region .sha256

            // Redump SHA-256
#if NETFRAMEWORK
            if ((GetFieldValue<DatFormat>(DatHeader.DatFormatKey) & DatFormat.RedumpSHA256) != 0)
#else
            if (GetFieldValue<DatFormat>(DatHeader.DatFormatKey).HasFlag(DatFormat.RedumpSHA256))
#endif
            {
                outfileNames.Add(DatFormat.RedumpSHA256, CreateOutFileNamesHelper(outDir, ".sha256", overwrite));
                usedExtensions.Add(".sha256");
            };

            #endregion

            #region .sha384

            // Redump SHA-384
#if NETFRAMEWORK
            if ((GetFieldValue<DatFormat>(DatHeader.DatFormatKey) & DatFormat.RedumpSHA384) != 0)
#else
            if (GetFieldValue<DatFormat>(DatHeader.DatFormatKey).HasFlag(DatFormat.RedumpSHA384))
#endif
            {
                outfileNames.Add(DatFormat.RedumpSHA384, CreateOutFileNamesHelper(outDir, ".sha384", overwrite));
                usedExtensions.Add(".sha384");
            };

            #endregion

            #region .sha512

            // Redump SHA-512
#if NETFRAMEWORK
            if ((GetFieldValue<DatFormat>(DatHeader.DatFormatKey) & DatFormat.RedumpSHA512) != 0)
#else
            if (GetFieldValue<DatFormat>(DatHeader.DatFormatKey).HasFlag(DatFormat.RedumpSHA512))
#endif
            {
                outfileNames.Add(DatFormat.RedumpSHA512, CreateOutFileNamesHelper(outDir, ".sha512", overwrite));
                usedExtensions.Add(".sha512");
            };

            #endregion

            #region .spamsum

            // Redump SpamSum
#if NETFRAMEWORK
            if ((GetFieldValue<DatFormat>(DatHeader.DatFormatKey) & DatFormat.RedumpSpamSum) != 0)
#else
            if (GetFieldValue<DatFormat>(DatHeader.DatFormatKey).HasFlag(DatFormat.RedumpSpamSum))
#endif
            {
                outfileNames.Add(DatFormat.RedumpSpamSum, CreateOutFileNamesHelper(outDir, ".spamsum", overwrite));
                usedExtensions.Add(".spamsum");
            };

            #endregion

            #region .ssv

            // SSV
#if NETFRAMEWORK
            if ((GetFieldValue<DatFormat>(DatHeader.DatFormatKey) & DatFormat.SSV) != 0)
#else
            if (GetFieldValue<DatFormat>(DatHeader.DatFormatKey).HasFlag(DatFormat.SSV))
#endif
            {
                outfileNames.Add(DatFormat.SSV, CreateOutFileNamesHelper(outDir, ".ssv", overwrite));
                usedExtensions.Add(".ssv");
            };

            #endregion

            #region .tsv

            // TSV
#if NETFRAMEWORK
            if ((GetFieldValue<DatFormat>(DatHeader.DatFormatKey) & DatFormat.TSV) != 0)
#else
            if (GetFieldValue<DatFormat>(DatHeader.DatFormatKey).HasFlag(DatFormat.TSV))
#endif
            {
                outfileNames.Add(DatFormat.TSV, CreateOutFileNamesHelper(outDir, ".tsv", overwrite));
                usedExtensions.Add(".tsv");
            };

            #endregion

            #region .txt

            // AttractMode
#if NETFRAMEWORK
            if ((GetFieldValue<DatFormat>(DatHeader.DatFormatKey) & DatFormat.AttractMode) != 0)
#else
            if (GetFieldValue<DatFormat>(DatHeader.DatFormatKey).HasFlag(DatFormat.AttractMode))
#endif
            {
                outfileNames.Add(DatFormat.AttractMode, CreateOutFileNamesHelper(outDir, ".txt", overwrite));
                usedExtensions.Add(".txt");
            }

            // MAME Listroms
#if NETFRAMEWORK
            if ((GetFieldValue<DatFormat>(DatHeader.DatFormatKey) & DatFormat.Listrom) != 0)
#else
            if (GetFieldValue<DatFormat>(DatHeader.DatFormatKey).HasFlag(DatFormat.Listrom))
#endif
            {
                if (usedExtensions.Contains(".txt"))
                {
                    outfileNames.Add(DatFormat.Listrom, CreateOutFileNamesHelper(outDir, ".lr.txt", overwrite));
                    usedExtensions.Add(".lr.txt");
                }
                else
                {
                    outfileNames.Add(DatFormat.Listrom, CreateOutFileNamesHelper(outDir, ".txt", overwrite));
                    usedExtensions.Add(".txt");
                }
            }

            // Missfile
#if NETFRAMEWORK
            if ((GetFieldValue<DatFormat>(DatHeader.DatFormatKey) & DatFormat.MissFile) != 0)
#else
            if (GetFieldValue<DatFormat>(DatHeader.DatFormatKey).HasFlag(DatFormat.MissFile))
#endif
            {
                if (usedExtensions.Contains(".txt"))
                {
                    outfileNames.Add(DatFormat.MissFile, CreateOutFileNamesHelper(outDir, ".miss.txt", overwrite));
                    usedExtensions.Add(".miss.txt");
                }
                else
                {
                    outfileNames.Add(DatFormat.MissFile, CreateOutFileNamesHelper(outDir, ".txt", overwrite));
                    usedExtensions.Add(".txt");
                }
            }

            // Everdrive SMDB
#if NETFRAMEWORK
            if ((GetFieldValue<DatFormat>(DatHeader.DatFormatKey) & DatFormat.EverdriveSMDB) != 0)
#else
            if (GetFieldValue<DatFormat>(DatHeader.DatFormatKey).HasFlag(DatFormat.EverdriveSMDB))
#endif
            {
                if (usedExtensions.Contains(".txt"))
                {
                    outfileNames.Add(DatFormat.EverdriveSMDB, CreateOutFileNamesHelper(outDir, ".smdb.txt", overwrite));
                    usedExtensions.Add(".smdb.txt");
                }
                else
                {
                    outfileNames.Add(DatFormat.EverdriveSMDB, CreateOutFileNamesHelper(outDir, ".txt", overwrite));
                    usedExtensions.Add(".txt");
                }
            }

            #endregion

            #region .xml

            // Logiqx XML
#if NETFRAMEWORK
            if ((GetFieldValue<DatFormat>(DatHeader.DatFormatKey) & DatFormat.Logiqx) != 0)
#else
            if (GetFieldValue<DatFormat>(DatHeader.DatFormatKey).HasFlag(DatFormat.Logiqx))
#endif
            {
                outfileNames.Add(DatFormat.Logiqx, CreateOutFileNamesHelper(outDir, ".xml", overwrite));
                usedExtensions.Add(".xml");
            }
#if NETFRAMEWORK
            if ((GetFieldValue<DatFormat>(DatHeader.DatFormatKey) & DatFormat.LogiqxDeprecated) != 0)
#else
            if (GetFieldValue<DatFormat>(DatHeader.DatFormatKey).HasFlag(DatFormat.LogiqxDeprecated))
#endif
            {
                outfileNames.Add(DatFormat.LogiqxDeprecated, CreateOutFileNamesHelper(outDir, ".xml", overwrite));
                usedExtensions.Add(".xml");
            }

            // SabreDAT
#if NETFRAMEWORK
            if ((GetFieldValue<DatFormat>(DatHeader.DatFormatKey) & DatFormat.SabreXML) != 0)
#else
            if (GetFieldValue<DatFormat>(DatHeader.DatFormatKey).HasFlag(DatFormat.SabreXML))
#endif
            {
                if (usedExtensions.Contains(".xml"))
                {
                    outfileNames.Add(DatFormat.SabreXML, CreateOutFileNamesHelper(outDir, ".sd.xml", overwrite));
                    usedExtensions.Add(".sd.xml");
                }
                else
                {
                    outfileNames.Add(DatFormat.SabreXML, CreateOutFileNamesHelper(outDir, ".xml", overwrite));
                    usedExtensions.Add(".xml");
                }
            }

            // Software List
#if NETFRAMEWORK
            if ((GetFieldValue<DatFormat>(DatHeader.DatFormatKey) & DatFormat.SoftwareList) != 0)
#else
            if (GetFieldValue<DatFormat>(DatHeader.DatFormatKey).HasFlag(DatFormat.SoftwareList))
#endif
            {
                if (usedExtensions.Contains(".xml"))
                {
                    outfileNames.Add(DatFormat.SoftwareList, CreateOutFileNamesHelper(outDir, ".sl.xml", overwrite));
                    usedExtensions.Add(".sl.xml");
                }
                else
                {
                    outfileNames.Add(DatFormat.SoftwareList, CreateOutFileNamesHelper(outDir, ".xml", overwrite));
                    usedExtensions.Add(".xml");
                }
            }

            // MAME Listxml
#if NETFRAMEWORK
            if ((GetFieldValue<DatFormat>(DatHeader.DatFormatKey) & DatFormat.Listxml) != 0)
#else
            if (GetFieldValue<DatFormat>(DatHeader.DatFormatKey).HasFlag(DatFormat.Listxml))
#endif
            {
                if (usedExtensions.Contains(".xml"))
                {
                    outfileNames.Add(DatFormat.Listxml, CreateOutFileNamesHelper(outDir, ".mame.xml", overwrite));
                    usedExtensions.Add(".mame.xml");
                }
                else
                {
                    outfileNames.Add(DatFormat.Listxml, CreateOutFileNamesHelper(outDir, ".xml", overwrite));
                    usedExtensions.Add(".xml");
                }
            }

            // OfflineList
#if NETFRAMEWORK
            if ((GetFieldValue<DatFormat>(DatHeader.DatFormatKey) & DatFormat.OfflineList) != 0)
#else
            if (GetFieldValue<DatFormat>(DatHeader.DatFormatKey).HasFlag(DatFormat.OfflineList))
#endif
            {
                if (usedExtensions.Contains(".xml"))
                {
                    outfileNames.Add(DatFormat.OfflineList, CreateOutFileNamesHelper(outDir, ".ol.xml", overwrite));
                    usedExtensions.Add(".ol.xml");
                }
                else
                {
                    outfileNames.Add(DatFormat.OfflineList, CreateOutFileNamesHelper(outDir, ".xml", overwrite));
                    usedExtensions.Add(".xml");
                }
            }

            // openMSX
#if NETFRAMEWORK
            if ((GetFieldValue<DatFormat>(DatHeader.DatFormatKey) & DatFormat.OpenMSX) != 0)
#else
            if (GetFieldValue<DatFormat>(DatHeader.DatFormatKey).HasFlag(DatFormat.OpenMSX))
#endif
            {
                if (usedExtensions.Contains(".xml"))
                {
                    outfileNames.Add(DatFormat.OpenMSX, CreateOutFileNamesHelper(outDir, ".msx.xml", overwrite));
                    usedExtensions.Add(".msx.xml");
                }
                else
                {
                    outfileNames.Add(DatFormat.OpenMSX, CreateOutFileNamesHelper(outDir, ".xml", overwrite));
                    usedExtensions.Add(".xml");
                }
            }

            // Archive.org
#if NETFRAMEWORK
            if ((GetFieldValue<DatFormat>(DatHeader.DatFormatKey) & DatFormat.ArchiveDotOrg) != 0)
#else
            if (GetFieldValue<DatFormat>(DatHeader.DatFormatKey).HasFlag(DatFormat.ArchiveDotOrg))
#endif
            {
                if (usedExtensions.Contains(".xml"))
                {
                    outfileNames.Add(DatFormat.ArchiveDotOrg, CreateOutFileNamesHelper(outDir, ".ado.xml", overwrite));
                    usedExtensions.Add(".ado.xml");
                }
                else
                {
                    outfileNames.Add(DatFormat.ArchiveDotOrg, CreateOutFileNamesHelper(outDir, ".xml", overwrite));
                    usedExtensions.Add(".xml");
                }
            }

            #endregion

            return outfileNames;
        }

        /// <summary>
        /// Help generating the outfile name
        /// </summary>
        /// <param name="outDir">Output directory</param>
        /// <param name="extension">Extension to use for the file</param>
        /// <param name="overwrite">True if we ignore existing files, false otherwise</param>
        /// <returns>String containing the new filename</returns>
        private string CreateOutFileNamesHelper(string outDir, string extension, bool overwrite)
        {
            string? filename = string.IsNullOrEmpty(GetFieldValue<string?>(DatHeader.FileNameKey))
                ? GetFieldValue<string?>(Models.Metadata.Header.DescriptionKey)
                : GetFieldValue<string?>(DatHeader.FileNameKey);

            // Strip off the extension if it's a holdover from the DAT
            if (Utilities.HasValidDatExtension(filename))
                filename = Path.GetFileNameWithoutExtension(filename);

            string outfile = $"{outDir}{filename}{extension}";
            outfile = outfile.Replace($"{Path.DirectorySeparatorChar}{Path.DirectorySeparatorChar}", Path.DirectorySeparatorChar.ToString());

            if (!overwrite)
            {
                int i = 1;
                while (File.Exists(outfile))
                {
                    outfile = $"{outDir}{filename}_{i}{extension}";
                    outfile = outfile.Replace($"{Path.DirectorySeparatorChar}{Path.DirectorySeparatorChar}", Path.DirectorySeparatorChar.ToString());
                    i++;
                }
            }

            return outfile;
        }

        #endregion

        #endregion // Instance Methods
    }
}
