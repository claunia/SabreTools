using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;
using SabreTools.Core.Filter;
using SabreTools.Core.Tools;

namespace SabreTools.DatFiles
{
    /// <summary>
    /// Represents all possible DAT header information
    /// </summary>
    [JsonObject("header"), XmlRoot("header")]
    public sealed class DatHeader : ModelBackedItem<Models.Metadata.Header>, ICloneable
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
        public bool CanOpenSpecified
        {
            get
            {
                var canOpen = GetStringArrayFieldValue(Models.Metadata.Header.CanOpenKey);
                return canOpen != null && canOpen.Length > 0;
            }
        }

        [JsonIgnore]
        public bool ImagesSpecified
        {
            get
            {
                return GetFieldValue<Models.OfflineList.Images?>(Models.Metadata.Header.ImagesKey) != null;
            }
        }

        [JsonIgnore]
        public bool InfosSpecified
        {
            get
            {
                return GetFieldValue<Models.OfflineList.Infos?>(Models.Metadata.Header.InfosKey) != null;
            }
        }

        [JsonIgnore]
        public bool NewDatSpecified
        {
            get
            {
                return GetFieldValue<Models.OfflineList.NewDat?>(Models.Metadata.Header.NewDatKey) != null;
            }
        }

        [JsonIgnore]
        public bool SearchSpecified
        {
            get
            {
                return GetFieldValue<Models.OfflineList.Search?>(Models.Metadata.Header.SearchKey) != null;
            }
        }

        #endregion

        #region Constructors

        public DatHeader() { }

        public DatHeader(Models.Metadata.Header header)
        {
            // Get all fields to automatically copy without processing
            var nonItemFields = TypeHelper.GetConstants(typeof(Models.Metadata.Header));
            if (nonItemFields == null)
                return;

            // Populate the internal machine from non-filter fields
            _internal = new Models.Metadata.Header();
            foreach (string fieldName in nonItemFields)
            {
                if (header.ContainsKey(fieldName))
                    _internal[fieldName] = header[fieldName];
            }

            // Get all fields specific to the DatFiles implementation
            var nonStandardFields = TypeHelper.GetConstants(typeof(DatHeader));
            if (nonStandardFields == null)
                return;

            // Populate the internal machine from filter fields
            foreach (string fieldName in nonStandardFields)
            {
                if (header.ContainsKey(fieldName))
                    _internal[fieldName] = header[fieldName];
            }
        }

        #endregion

        #region Cloning Methods

        /// <summary>
        /// Clone the current header
        /// </summary>
        public object Clone() => new DatHeader(GetInternalClone());

        /// <summary>
        /// Clone the standard parts of the current header
        /// </summary>
        public DatHeader CloneStandard()
        {
            var header = new DatHeader();
            header.SetFieldValue<string?>(Models.Metadata.Header.AuthorKey,
                GetStringFieldValue(Models.Metadata.Header.AuthorKey));
            header.SetFieldValue<string?>(Models.Metadata.Header.CategoryKey,
                GetStringFieldValue(Models.Metadata.Header.CategoryKey));
            header.SetFieldValue<string?>(Models.Metadata.Header.CommentKey,
                GetStringFieldValue(Models.Metadata.Header.CommentKey));
            header.SetFieldValue<string?>(Models.Metadata.Header.DateKey,
                GetStringFieldValue(Models.Metadata.Header.DateKey));
            header.SetFieldValue<DatFormat>(DatHeader.DatFormatKey,
                GetFieldValue<DatFormat>(DatHeader.DatFormatKey));
            header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey,
                GetStringFieldValue(Models.Metadata.Header.DescriptionKey));
            header.SetFieldValue<string?>(Models.Metadata.Header.EmailKey,
                GetStringFieldValue(Models.Metadata.Header.EmailKey));
            header.SetFieldValue<string?>(DatHeader.FileNameKey,
                GetStringFieldValue(DatHeader.FileNameKey));
            header.SetFieldValue<string?>(Models.Metadata.Header.ForceMergingKey,
                GetStringFieldValue(Models.Metadata.Header.ForceMergingKey).AsEnumValue<MergingFlag>().AsStringValue());
            header.SetFieldValue<string?>(Models.Metadata.Header.ForceNodumpKey,
                GetStringFieldValue(Models.Metadata.Header.ForceNodumpKey).AsEnumValue<NodumpFlag>().AsStringValue());
            header.SetFieldValue<string?>(Models.Metadata.Header.ForcePackingKey,
                GetStringFieldValue(Models.Metadata.Header.ForcePackingKey).AsEnumValue<PackingFlag>().AsStringValue());
            header.SetFieldValue<string?>(Models.Metadata.Header.HeaderKey,
                GetStringFieldValue(Models.Metadata.Header.HeaderKey));
            header.SetFieldValue<string?>(Models.Metadata.Header.HomepageKey,
                GetStringFieldValue(Models.Metadata.Header.HomepageKey));
            header.SetFieldValue<string?>(Models.Metadata.Header.NameKey,
                GetStringFieldValue(Models.Metadata.Header.NameKey));
            header.SetFieldValue<string?>(Models.Metadata.Header.RootDirKey,
                GetStringFieldValue(Models.Metadata.Header.RootDirKey));
            header.SetFieldValue<string?>(Models.Metadata.Header.TypeKey,
                GetStringFieldValue(Models.Metadata.Header.TypeKey));
            header.SetFieldValue<string?>(Models.Metadata.Header.UrlKey,
                GetStringFieldValue(Models.Metadata.Header.UrlKey));
            header.SetFieldValue<string?>(Models.Metadata.Header.VersionKey,
                GetStringFieldValue(Models.Metadata.Header.VersionKey));

            return header;
        }

        /// <summary>
        /// Clone the filtering parts of the current header
        /// </summary>
        public DatHeader CloneFiltering()
        {
            var header = new DatHeader();
            header.SetFieldValue<string?>(DatHeader.AddExtensionKey,
                GetStringFieldValue(DatHeader.AddExtensionKey));
            header.SetFieldValue<DatFormat>(DatHeader.DatFormatKey,
                GetFieldValue<DatFormat>(DatHeader.DatFormatKey));
            header.SetFieldValue<bool?>(DatHeader.GameNameKey,
                GetBoolFieldValue(DatHeader.GameNameKey));
            header.SetFieldValue<DepotInformation?>(DatHeader.InputDepotKey,
                GetFieldValue<DepotInformation?>(DatHeader.InputDepotKey)?.Clone() as DepotInformation);
            header.SetFieldValue<DepotInformation?>(DatHeader.OutputDepotKey,
                GetFieldValue<DepotInformation?>(DatHeader.OutputDepotKey)?.Clone() as DepotInformation);
            header.SetFieldValue<string?>(DatHeader.PostfixKey,
                GetStringFieldValue(DatHeader.PostfixKey));
            header.SetFieldValue<string?>(DatHeader.PrefixKey,
                GetStringFieldValue(DatHeader.PrefixKey));
            header.SetFieldValue<bool?>(DatHeader.RemoveExtensionKey,
                GetBoolFieldValue(DatHeader.RemoveExtensionKey));
            header.SetFieldValue<string?>(DatHeader.ReplaceExtensionKey,
                GetStringFieldValue(DatHeader.ReplaceExtensionKey));
            header.SetFieldValue<bool?>(DatHeader.QuotesKey,
                GetBoolFieldValue(DatHeader.QuotesKey));
            header.SetFieldValue<bool?>(DatHeader.UseRomNameKey,
                GetBoolFieldValue(DatHeader.UseRomNameKey));

            return header;
        }

        /// <summary>
        /// Get a clone of the current internal model
        /// </summary>
        public Models.Metadata.Header GetInternalClone()
        {
            var header = (_internal.Clone() as Models.Metadata.Header)!;

            // Remove fields with default values
            if (header.ReadString(Models.Metadata.Header.ForceMergingKey).AsEnumValue<MergingFlag>() == MergingFlag.None)
                header.Remove(Models.Metadata.Header.ForceMergingKey);
            if (header.ReadString(Models.Metadata.Header.ForceNodumpKey).AsEnumValue<NodumpFlag>() == NodumpFlag.None)
                header.Remove(Models.Metadata.Header.ForceNodumpKey);
            if (header.ReadString(Models.Metadata.Header.ForcePackingKey).AsEnumValue<PackingFlag>() == PackingFlag.None)
                header.Remove(Models.Metadata.Header.ForcePackingKey);

            // Convert subheader values
            if (CanOpenSpecified)
                header[Models.Metadata.Header.CanOpenKey] = new Models.OfflineList.CanOpen { Extension = GetStringArrayFieldValue(Models.Metadata.Header.CanOpenKey) };
            if (ImagesSpecified)
                header[Models.Metadata.Header.ImagesKey] = GetFieldValue<Models.OfflineList.Images>(Models.Metadata.Header.ImagesKey);
            if (InfosSpecified)
                header[Models.Metadata.Header.InfosKey] = GetFieldValue<Models.OfflineList.Infos>(Models.Metadata.Header.InfosKey);
            if (NewDatSpecified)
                header[Models.Metadata.Header.NewDatKey] = GetFieldValue<Models.OfflineList.NewDat>(Models.Metadata.Header.NewDatKey);
            if (SearchSpecified)
                header[Models.Metadata.Header.SearchKey] = GetFieldValue<Models.OfflineList.Search>(Models.Metadata.Header.SearchKey);

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

            if (!string.IsNullOrEmpty(datHeader.GetStringFieldValue(DatHeader.FileNameKey)))
                SetFieldValue<string?>(DatHeader.FileNameKey,
                    datHeader.GetStringFieldValue(DatHeader.FileNameKey));

            if (!string.IsNullOrEmpty(datHeader.GetStringFieldValue(Models.Metadata.Header.NameKey)))
                SetFieldValue<string?>(Models.Metadata.Header.NameKey,
                    datHeader.GetStringFieldValue(Models.Metadata.Header.NameKey));

            if (!string.IsNullOrEmpty(datHeader.GetStringFieldValue(Models.Metadata.Header.DescriptionKey)))
                SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey,
                    datHeader.GetStringFieldValue(Models.Metadata.Header.DescriptionKey));

            if (!string.IsNullOrEmpty(datHeader.GetStringFieldValue(Models.Metadata.Header.RootDirKey)))
                SetFieldValue<string?>(Models.Metadata.Header.RootDirKey,
                    datHeader.GetStringFieldValue(Models.Metadata.Header.RootDirKey));

            if (!string.IsNullOrEmpty(datHeader.GetStringFieldValue(Models.Metadata.Header.CategoryKey)))
                SetFieldValue<string?>(Models.Metadata.Header.CategoryKey,
                    datHeader.GetStringFieldValue(Models.Metadata.Header.CategoryKey));

            if (!string.IsNullOrEmpty(datHeader.GetStringFieldValue(Models.Metadata.Header.VersionKey)))
                SetFieldValue<string?>(Models.Metadata.Header.VersionKey,
                    datHeader.GetStringFieldValue(Models.Metadata.Header.VersionKey));

            if (!string.IsNullOrEmpty(datHeader.GetStringFieldValue(Models.Metadata.Header.DateKey)))
                SetFieldValue<string?>(Models.Metadata.Header.DateKey,
                    datHeader.GetStringFieldValue(Models.Metadata.Header.DateKey));

            if (!string.IsNullOrEmpty(datHeader.GetStringFieldValue(Models.Metadata.Header.AuthorKey)))
                SetFieldValue<string?>(Models.Metadata.Header.AuthorKey,
                    datHeader.GetStringFieldValue(Models.Metadata.Header.AuthorKey));

            if (!string.IsNullOrEmpty(datHeader.GetStringFieldValue(Models.Metadata.Header.EmailKey)))
                SetFieldValue<string?>(Models.Metadata.Header.EmailKey,
                    datHeader.GetStringFieldValue(Models.Metadata.Header.EmailKey));

            if (!string.IsNullOrEmpty(datHeader.GetStringFieldValue(Models.Metadata.Header.HomepageKey)))
                SetFieldValue<string?>(Models.Metadata.Header.HomepageKey,
                datHeader.GetStringFieldValue(Models.Metadata.Header.HomepageKey));

            if (!string.IsNullOrEmpty(datHeader.GetStringFieldValue(Models.Metadata.Header.UrlKey)))
                SetFieldValue<string?>(Models.Metadata.Header.UrlKey,
                    datHeader.GetStringFieldValue(Models.Metadata.Header.UrlKey));

            if (!string.IsNullOrEmpty(datHeader.GetStringFieldValue(Models.Metadata.Header.CommentKey)))
                SetFieldValue<string?>(Models.Metadata.Header.CommentKey,
                    datHeader.GetStringFieldValue(Models.Metadata.Header.CommentKey));

            if (!string.IsNullOrEmpty(datHeader.GetStringFieldValue(Models.Metadata.Header.HeaderKey)))
                SetFieldValue<string?>(Models.Metadata.Header.HeaderKey,
                    datHeader.GetStringFieldValue(Models.Metadata.Header.HeaderKey));

            if (!string.IsNullOrEmpty(datHeader.GetStringFieldValue(Models.Metadata.Header.TypeKey)))
                SetFieldValue<string?>(Models.Metadata.Header.TypeKey,
                    datHeader.GetStringFieldValue(Models.Metadata.Header.TypeKey));

            if (datHeader.GetStringFieldValue(Models.Metadata.Header.ForceMergingKey).AsEnumValue<MergingFlag>() != MergingFlag.None)
                SetFieldValue<string?>(Models.Metadata.Header.ForceMergingKey,
                    datHeader.GetStringFieldValue(Models.Metadata.Header.ForceMergingKey).AsEnumValue<MergingFlag>().AsStringValue());

            if (datHeader.GetStringFieldValue(Models.Metadata.Header.ForceNodumpKey).AsEnumValue<NodumpFlag>() != NodumpFlag.None)
                SetFieldValue<string?>(Models.Metadata.Header.ForceNodumpKey,
                    datHeader.GetStringFieldValue(Models.Metadata.Header.ForceNodumpKey).AsEnumValue<NodumpFlag>().AsStringValue());

            if (datHeader.GetStringFieldValue(Models.Metadata.Header.ForcePackingKey).AsEnumValue<PackingFlag>() != PackingFlag.None)
                SetFieldValue<string?>(Models.Metadata.Header.ForcePackingKey,
                    datHeader.GetStringFieldValue(Models.Metadata.Header.ForcePackingKey).AsEnumValue<PackingFlag>().AsStringValue());

            if (datHeader.GetFieldValue<DatFormat>(DatHeader.DatFormatKey) != 0x00)
                SetFieldValue<DatFormat>(DatHeader.DatFormatKey,
                    datHeader.GetFieldValue<DatFormat>(DatHeader.DatFormatKey));

            if (!string.IsNullOrEmpty(datHeader.GetStringFieldValue(DatHeader.PrefixKey)))
                SetFieldValue<string?>(DatHeader.PrefixKey,
                    datHeader.GetStringFieldValue(DatHeader.PrefixKey));

            if (!string.IsNullOrEmpty(datHeader.GetStringFieldValue(DatHeader.PostfixKey)))
                SetFieldValue<string?>(DatHeader.PostfixKey,
                    datHeader.GetStringFieldValue(DatHeader.PostfixKey));

            if (!string.IsNullOrEmpty(datHeader.GetStringFieldValue(DatHeader.AddExtensionKey)))
                SetFieldValue<string?>(DatHeader.AddExtensionKey,
                    datHeader.GetStringFieldValue(DatHeader.AddExtensionKey));

            if (!string.IsNullOrEmpty(datHeader.GetStringFieldValue(DatHeader.ReplaceExtensionKey)))
                SetFieldValue<string?>(DatHeader.ReplaceExtensionKey,
                    datHeader.GetStringFieldValue(DatHeader.ReplaceExtensionKey));

            SetFieldValue<DepotInformation?>(DatHeader.InputDepotKey,
                datHeader.GetFieldValue<DepotInformation?>(DatHeader.InputDepotKey)?.Clone() as DepotInformation);
            SetFieldValue<DepotInformation?>(DatHeader.OutputDepotKey,
                datHeader.GetFieldValue<DepotInformation?>(DatHeader.OutputDepotKey)?.Clone() as DepotInformation);
            SetFieldValue<bool?>(DatHeader.GameNameKey,
                datHeader.GetBoolFieldValue(DatHeader.GameNameKey));
            SetFieldValue<bool?>(DatHeader.QuotesKey,
                datHeader.GetBoolFieldValue(DatHeader.QuotesKey));
            SetFieldValue<bool?>(DatHeader.RemoveExtensionKey,
                datHeader.GetBoolFieldValue(DatHeader.RemoveExtensionKey));
            SetFieldValue<bool?>(DatHeader.UseRomNameKey,
                datHeader.GetBoolFieldValue(DatHeader.UseRomNameKey));
        }

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(ModelBackedItem? other)
        {
            // If other is null
            if (other == null)
                return false;

            // If the type is mismatched
            if (other is not DatHeader otherItem)
                return false;

            // Compare internal models
            return _internal.EqualTo(otherItem._internal);
        }

        /// <inheritdoc/>
        public override bool Equals(ModelBackedItem<Models.Metadata.Header>? other)
        {
            // If other is null
            if (other == null)
                return false;

            // If the type is mismatched
            if (other is not DatHeader otherItem)
                return false;

            // Compare internal models
            return _internal.EqualTo(otherItem._internal);
        }

        #endregion

        #region Manipulation

        /// <summary>
        /// Runs a filter and determines if it passes or not
        /// </summary>
        /// <param name="filterRunner">Filter runner to use for checking</param>
        /// <returns>True if the Machine passes the filter, false otherwise</returns>
        public bool PassesFilter(FilterRunner filterRunner) => filterRunner.Run(_internal);

        #endregion

        #region Writing

        /// <summary>
        /// Map of all formats to extensions, including "backup" extensions
        /// </summary>
        private static readonly Dictionary<DatFormat, string[]> ExtensionMappings = new()
        {
            // .csv
            { DatFormat.CSV, new string[] { ".csv" } },

            // .dat
            { DatFormat.ClrMamePro, new string[] { ".dat" } },
            { DatFormat.RomCenter, new string[] { ".dat", ".rc.dat" } },
            { DatFormat.DOSCenter, new string[] { ".dat", ".dc.dat" } },

            // .json
            { DatFormat.SabreJSON, new string[] { ".json" } },

            // .md2
            { DatFormat.RedumpMD2, new string[] { ".md2" } },

            // .md4
            { DatFormat.RedumpMD4, new string[] { ".md4" } },

            // .md5
            { DatFormat.RedumpMD5, new string[] { ".md5" } },

            // .sfv
            { DatFormat.RedumpSFV, new string[] { ".sfv" } },

            // .sha1
            { DatFormat.RedumpSHA1, new string[] { ".sha1" } },

            // .sha256
            { DatFormat.RedumpSHA256, new string[] { ".sha256" } },

            // .sha384
            { DatFormat.RedumpSHA384, new string[] { ".sha384" } },

            // .sha512
            { DatFormat.RedumpSHA512, new string[] { ".sha512" } },

            // .spamsum
            { DatFormat.RedumpSpamSum, new string[] { ".spamsum" } },

            // .ssv
            { DatFormat.SSV, new string[] { ".ssv" } },

            // .tsv
            { DatFormat.TSV, new string[] { ".tsv" } },

            // .txt
            { DatFormat.AttractMode, new string[] { ".txt" } },
            { DatFormat.Listrom, new string[] { ".txt", ".lr.txt" } },
            { DatFormat.MissFile, new string[] { ".txt", ".miss.txt" } },
            { DatFormat.EverdriveSMDB, new string[] { ".txt", ".smdb.txt" } },

            // .xml
            { DatFormat.Logiqx, new string[] { ".xml" } },
            { DatFormat.LogiqxDeprecated, new string[] { ".xml", ".xml" } }, // Intentional duplicate
            { DatFormat.SabreXML, new string[] { ".xml", ".sd.xml" } },
            { DatFormat.SoftwareList, new string[] { ".xml", ".sl.xml" } },
            { DatFormat.Listxml, new string[] { ".xml", ".mame.xml" } },
            { DatFormat.OfflineList, new string[] { ".xml", ".ol.xml" } },
            { DatFormat.OpenMSX, new string[] { ".xml", ".msx.xml" } },
            { DatFormat.ArchiveDotOrg, new string[] { ".xml", ".ado.xml" } },
        };

        /// <summary>
        /// Generate a proper outfile name based on a DAT and output directory
        /// </summary>
        /// <param name="datHeader">DatHeader value to pull information from</param>
        /// <param name="outDir">Output directory</param>
        /// <param name="overwrite">True if we ignore existing files (default), false otherwise</param>
        /// <returns>Dictionary of output formats mapped to file names</returns>
        public static Dictionary<DatFormat, string> CreateOutFileNames(DatHeader datHeader, string outDir, bool overwrite = true)
        {
            // Create the output dictionary
            Dictionary<DatFormat, string> outfileNames = [];

            // Get the filename to use
            string? filename = string.IsNullOrEmpty(datHeader.GetStringFieldValue(DatHeader.FileNameKey))
                ? datHeader.GetStringFieldValue(Models.Metadata.Header.DescriptionKey)
                : datHeader.GetStringFieldValue(DatHeader.FileNameKey);

            // Strip off the extension if it's a holdover from the DAT
            if (Utilities.HasValidDatExtension(filename))
                filename = Path.GetFileNameWithoutExtension(filename);

            // Double check the outDir for the end delim
            if (!outDir.EndsWith(Path.DirectorySeparatorChar.ToString()))
                outDir += Path.DirectorySeparatorChar;

            // Get the current format types
            DatFormat datFormat = datHeader.GetFieldValue<DatFormat>(DatHeader.DatFormatKey);
            List<DatFormat> usedFormats = SplitFormats(datFormat);

            // Get the extensions from the output type
            List<string> usedExtensions = [];
            foreach (var map in ExtensionMappings)
            {
                // Split the pair
                DatFormat format = map.Key;
                string[] extensions = map.Value;

                // Ignore unused formats
                if (!usedFormats.Contains(format))
                    continue;

                // Get the correct extension, assuming a backup exists
                string extension = extensions[0];
                if (usedExtensions.Contains(extension))
                    extension = extensions[1];

                // Create the filename and set the extension as used
                outfileNames.Add(format, CreateOutFileNamesHelper(filename, outDir, extension, overwrite));
                usedExtensions.Add(extension);
            }

            return outfileNames;
        }

        /// <summary>
        /// Help generating the outfile name
        /// </summary>
        /// <param name="filename">Base filename to use</param>
        /// <param name="outDir">Output directory</param>
        /// <param name="extension">Extension to use for the file</param>
        /// <param name="overwrite">True if we ignore existing files, false otherwise</param>
        /// <returns>String containing the new filename</returns>
        private static string CreateOutFileNamesHelper(string? filename, string outDir, string extension, bool overwrite)
        {
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

        /// <summary>
        /// Split a format flag into multiple distinct values
        /// </summary>
        /// <param name="datFormat">Combined DatFormat value to split</param>
        /// <returns>List representing the individual flag values set</returns>
        /// TODO: Consider making DatFormat a non-flag enum so this doesn't need to happen
        private static List<DatFormat> SplitFormats(DatFormat datFormat)
        {
            List<DatFormat> usedFormats = [];

#if NET20 || NET35
            if ((datFormat & DatFormat.ArchiveDotOrg) != 0)
                usedFormats.Add(DatFormat.ArchiveDotOrg);
            if ((datFormat & DatFormat.AttractMode) != 0)
                usedFormats.Add(DatFormat.AttractMode);
            if ((datFormat & DatFormat.ClrMamePro) != 0)
                usedFormats.Add(DatFormat.ClrMamePro);
            if ((datFormat & DatFormat.CSV) != 0)
                usedFormats.Add(DatFormat.CSV);
            if ((datFormat & DatFormat.DOSCenter) != 0)
                usedFormats.Add(DatFormat.DOSCenter);
            if ((datFormat & DatFormat.EverdriveSMDB) != 0)
                usedFormats.Add(DatFormat.EverdriveSMDB);
            if ((datFormat & DatFormat.Listrom) != 0)
                usedFormats.Add(DatFormat.Listrom);
            if ((datFormat & DatFormat.Listxml) != 0)
                usedFormats.Add(DatFormat.Listxml);
            if ((datFormat & DatFormat.Logiqx) != 0)
                usedFormats.Add(DatFormat.Logiqx);
            if ((datFormat & DatFormat.LogiqxDeprecated) != 0)
                usedFormats.Add(DatFormat.LogiqxDeprecated);
            if ((datFormat & DatFormat.MissFile) != 0)
                usedFormats.Add(DatFormat.MissFile);
            if ((datFormat & DatFormat.OfflineList) != 0)
                usedFormats.Add(DatFormat.OfflineList);
            if ((datFormat & DatFormat.OpenMSX) != 0)
                usedFormats.Add(DatFormat.OpenMSX);
            if ((datFormat & DatFormat.RedumpMD2) != 0)
                usedFormats.Add(DatFormat.RedumpMD2);
            if ((datFormat & DatFormat.RedumpMD4) != 0)
                usedFormats.Add(DatFormat.RedumpMD4);
            if ((datFormat & DatFormat.RedumpMD5) != 0)
                usedFormats.Add(DatFormat.RedumpMD5);
            if ((datFormat & DatFormat.RedumpSFV) != 0)
                usedFormats.Add(DatFormat.RedumpSFV);
            if ((datFormat & DatFormat.RedumpSHA1) != 0)
                usedFormats.Add(DatFormat.RedumpSHA1);
            if ((datFormat & DatFormat.RedumpSHA256) != 0)
                usedFormats.Add(DatFormat.RedumpSHA256);
            if ((datFormat & DatFormat.RedumpSHA384) != 0)
                usedFormats.Add(DatFormat.RedumpSHA384);
            if ((datFormat & DatFormat.RedumpSHA512) != 0)
                usedFormats.Add(DatFormat.RedumpSHA512);
            if ((datFormat & DatFormat.RedumpSpamSum) != 0)
                usedFormats.Add(DatFormat.RedumpSpamSum);
            if ((datFormat & DatFormat.RomCenter) != 0)
                usedFormats.Add(DatFormat.RomCenter);
            if ((datFormat & DatFormat.SabreJSON) != 0)
                usedFormats.Add(DatFormat.SabreJSON);
            if ((datFormat & DatFormat.SabreXML) != 0)
                usedFormats.Add(DatFormat.SabreXML);
            if ((datFormat & DatFormat.SoftwareList) != 0)
                usedFormats.Add(DatFormat.SoftwareList);
            if ((datFormat & DatFormat.SSV) != 0)
                usedFormats.Add(DatFormat.SSV);
            if ((datFormat & DatFormat.TSV) != 0)
                usedFormats.Add(DatFormat.TSV);
#else
            if (datFormat.HasFlag(DatFormat.ArchiveDotOrg))
                usedFormats.Add(DatFormat.ArchiveDotOrg);
            if (datFormat.HasFlag(DatFormat.AttractMode))
                usedFormats.Add(DatFormat.AttractMode);
            if (datFormat.HasFlag(DatFormat.ClrMamePro))
                usedFormats.Add(DatFormat.ClrMamePro);
            if (datFormat.HasFlag(DatFormat.CSV))
                usedFormats.Add(DatFormat.CSV);
            if (datFormat.HasFlag(DatFormat.DOSCenter))
                usedFormats.Add(DatFormat.DOSCenter);
            if (datFormat.HasFlag(DatFormat.EverdriveSMDB))
                usedFormats.Add(DatFormat.EverdriveSMDB);
            if (datFormat.HasFlag(DatFormat.Listrom))
                usedFormats.Add(DatFormat.Listrom);
            if (datFormat.HasFlag(DatFormat.Listxml))
                usedFormats.Add(DatFormat.Listxml);
            if (datFormat.HasFlag(DatFormat.Logiqx))
                usedFormats.Add(DatFormat.Logiqx);
            if (datFormat.HasFlag(DatFormat.LogiqxDeprecated))
                usedFormats.Add(DatFormat.LogiqxDeprecated);
            if (datFormat.HasFlag(DatFormat.MissFile))
                usedFormats.Add(DatFormat.MissFile);
            if (datFormat.HasFlag(DatFormat.OfflineList))
                usedFormats.Add(DatFormat.OfflineList);
            if (datFormat.HasFlag(DatFormat.OpenMSX))
                usedFormats.Add(DatFormat.OpenMSX);
            if (datFormat.HasFlag(DatFormat.RedumpMD2))
                usedFormats.Add(DatFormat.RedumpMD2);
            if (datFormat.HasFlag(DatFormat.RedumpMD4))
                usedFormats.Add(DatFormat.RedumpMD4);
            if (datFormat.HasFlag(DatFormat.RedumpMD5))
                usedFormats.Add(DatFormat.RedumpMD5);
            if (datFormat.HasFlag(DatFormat.RedumpSFV))
                usedFormats.Add(DatFormat.RedumpSFV);
            if (datFormat.HasFlag(DatFormat.RedumpSHA1))
                usedFormats.Add(DatFormat.RedumpSHA1);
            if (datFormat.HasFlag(DatFormat.RedumpSHA256))
                usedFormats.Add(DatFormat.RedumpSHA256);
            if (datFormat.HasFlag(DatFormat.RedumpSHA384))
                usedFormats.Add(DatFormat.RedumpSHA384);
            if (datFormat.HasFlag(DatFormat.RedumpSHA512))
                usedFormats.Add(DatFormat.RedumpSHA512);
            if (datFormat.HasFlag(DatFormat.RedumpSpamSum))
                usedFormats.Add(DatFormat.RedumpSpamSum);
            if (datFormat.HasFlag(DatFormat.RomCenter))
                usedFormats.Add(DatFormat.RomCenter);
            if (datFormat.HasFlag(DatFormat.SabreJSON))
                usedFormats.Add(DatFormat.SabreJSON);
            if (datFormat.HasFlag(DatFormat.SabreXML))
                usedFormats.Add(DatFormat.SabreXML);
            if (datFormat.HasFlag(DatFormat.SoftwareList))
                usedFormats.Add(DatFormat.SoftwareList);
            if (datFormat.HasFlag(DatFormat.SSV))
                usedFormats.Add(DatFormat.SSV);
            if (datFormat.HasFlag(DatFormat.TSV))
                usedFormats.Add(DatFormat.TSV);
#endif

            return usedFormats;
        }

        #endregion
    }
}
