using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.DatFiles.Formats;

namespace SabreTools.DatFiles
{
    /// <summary>
    /// Represents all possible DAT header information
    /// </summary>
    [JsonObject("header"), XmlRoot("header")]
    public class DatHeader : ICloneable
    {
        #region Fields

        #region Common

        /// <summary>
        /// External name of the DAT
        /// </summary>
        [JsonProperty("filename", DefaultValueHandling = DefaultValueHandling.Include)]
        [XmlElement("filename")]
        public string? FileName { get; set; }

        /// <summary>
        /// Internal name of the DAT
        /// </summary>
        [JsonProperty("name", DefaultValueHandling = DefaultValueHandling.Include)]
        [XmlElement("name")]
        public string? Name { get; set; }

        /// <summary>
        /// DAT description
        /// </summary>
        [JsonProperty("description", DefaultValueHandling = DefaultValueHandling.Include)]
        [XmlElement("description")]
        public string? Description { get; set; }

        /// <summary>
        /// Root directory for the files; currently TruRip/EmuARC-exclusive
        /// </summary>
        [JsonProperty("rootdir", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("rootdir")]
        public string? RootDir { get; set; }

        /// <summary>
        /// General category of items found in the DAT
        /// </summary>
        [JsonProperty("category", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("category")]
        public string? Category { get; set; }

        /// <summary>
        /// Version of the DAT
        /// </summary>
        [JsonProperty("version", DefaultValueHandling = DefaultValueHandling.Include)]
        [XmlElement("version")]
        public string? Version { get; set; }

        /// <summary>
        /// Creation or modification date
        /// </summary>
        [JsonProperty("date", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("date")]
        public string? Date { get; set; }

        /// <summary>
        /// List of authors who contributed to the DAT
        /// </summary>
        [JsonProperty("author", DefaultValueHandling = DefaultValueHandling.Include)]
        [XmlElement("author")]
        public string? Author { get; set; }

        /// <summary>
        /// Email address for DAT author(s)
        /// </summary>
        [JsonProperty("email", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("email")]
        public string? Email { get; set; }

        /// <summary>
        /// Author or distribution homepage name
        /// </summary>
        [JsonProperty("homepage", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("homepage")]
        public string? Homepage { get; set; }

        /// <summary>
        /// Author or distribution URL
        /// </summary>
        [JsonProperty("url", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("url")]
        public string? Url { get; set; }

        /// <summary>
        /// Any comment that does not already fit an existing field
        /// </summary>
        [JsonProperty("comment", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("comment")]
        public string? Comment { get; set; }

        /// <summary>
        /// Header skipper to be used when loading the DAT
        /// </summary>
        [JsonProperty("header", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("header")]
        public string? HeaderSkipper { get; set; }

        /// <summary>
        /// Classification of the DAT. Generally only used for SuperDAT
        /// </summary>
        [JsonProperty("type", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("type")]
        public string? Type { get; set; }

        /// <summary>
        /// Force a merging style when loaded
        /// </summary>
        [JsonProperty("forcemerging", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("forcemerging")]
        public MergingFlag ForceMerging { get; set; }

        [JsonIgnore]
        public bool ForceMergingSpecified { get { return ForceMerging != MergingFlag.None; } }

        /// <summary>
        /// Force nodump handling when loaded
        /// </summary>
        [JsonProperty("forcenodump", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("forcenodump")]
        public NodumpFlag ForceNodump { get; set; }

        [JsonIgnore]
        public bool ForceNodumpSpecified { get { return ForceNodump != NodumpFlag.None; } }

        /// <summary>
        /// Force output packing when loaded
        /// </summary>
        [JsonProperty("forcepacking", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("forcepacking")]
        public PackingFlag ForcePacking { get; set; }

        [JsonIgnore]
        public bool ForcePackingSpecified { get { return ForcePacking != PackingFlag.None; } }

        /// <summary>
        /// Read or write format
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public DatFormat DatFormat { get; set; }

        #endregion

        #region ListXML

        /// <summary>
        /// Debug build flag
        /// </summary>
        /// <remarks>Also in Logiqx</remarks>
        [JsonProperty("debug", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("debug")]
        public bool? Debug { get; set; } = null;

        [JsonIgnore]
        public bool DebugSpecified { get { return Debug != null; } }

        /// <summary>
        /// MAME configuration name
        /// </summary>
        [JsonProperty("mameconfig", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("mameconfig")]
        public string? MameConfig { get; set; }

        #endregion

        #region Logiqx

        /// <summary>
        /// No-Intro system ID
        /// </summary>
        [JsonProperty("nointroid", DefaultValueHandling = DefaultValueHandling.Include)]
        [XmlElement("nointroid")]
        public string? NoIntroID { get; set; }

        /// <summary>
        /// Build version
        /// </summary>
        [JsonProperty("build", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("build")]
        public string? Build { get; set; }

        /// <summary>
        /// Logiqx/RomCenter plugin, OfflineList System
        /// </summary>
        [JsonProperty("system", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("system")]
        public string? System { get; set; }

        /// <summary>
        /// RomCenter rom mode
        /// </summary>
        /// <remarks>(merged|split|unmerged) "split"</remarks>
        [JsonProperty("rommode", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("rommode")]
        public MergingFlag RomMode { get; set; }

        [JsonIgnore]
        public bool RomModeSpecified { get { return RomMode != MergingFlag.None; } }

        /// <summary>
        /// RomCenter bios mode
        /// </summary>
        /// <remarks>(merged|split|unmerged) "split"</remarks>
        [JsonProperty("biosmode", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("biosmode")]
        public MergingFlag BiosMode { get; set; }

        [JsonIgnore]
        public bool BiosModeSpecified { get { return BiosMode != MergingFlag.None; } }

        /// <summary>
        /// RomCenter sample mode
        /// </summary>
        /// <remarks>(merged|unmerged) "merged"</remarks>
        [JsonProperty("samplemode", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("samplemode")]
        public MergingFlag SampleMode { get; set; }

        [JsonIgnore]
        public bool SampleModeSpecified { get { return SampleMode != MergingFlag.None; } }

        /// <summary>
        /// RomCenter lock rom mode
        /// </summary>
        [JsonProperty("lockrommode", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("lockrommode")]
        public bool? LockRomMode { get; set; }

        [JsonIgnore]
        public bool LockRomModeSpecified { get { return LockRomMode != null; } }

        /// <summary>
        /// RomCenter lock bios mode
        /// </summary>
        [JsonProperty("lockbiosmode", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("lockbiosmode")]
        public bool? LockBiosMode { get; set; }

        [JsonIgnore]
        public bool LockBiosModeSpecified { get { return LockBiosMode != null; } }

        /// <summary>
        /// RomCenter lock sample mode
        /// </summary>
        [JsonProperty("locksamplemode", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("locksamplemode")]
        public bool? LockSampleMode { get; set; }

        [JsonIgnore]
        public bool LockSampleModeSpecified { get { return LockSampleMode != null; } }

        #endregion

        #region Missfile

        /// <summary>
        /// Output the item name
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public bool UseRomName { get; set; }

        #endregion

        #region OfflineList

        /// <summary>
        /// Screenshots width
        /// </summary>
        [JsonProperty("screenshotswidth", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("screenshotswidth")]
        public string? ScreenshotsWidth { get; set; }

        /// <summary>
        /// Screenshots height
        /// </summary>
        [JsonProperty("screenshotsheight", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("screenshotsheight")]
        public string? ScreenshotsHeight { get; set; }

        /// <summary>
        /// OfflineList info list
        /// </summary>
        [JsonProperty("infos", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("infos")]
        public List<OfflineListInfo>? Infos { get; set; }

        [JsonIgnore]
        public bool InfosSpecified { get { return Infos != null && Infos.Count > 0; } }

        /// <summary>
        /// OfflineList can-open extensions
        /// </summary>
        [JsonProperty("canopen", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("canopen")]
        public List<string>? CanOpen { get; set; }

        [JsonIgnore]
        public bool CanOpenSpecified { get { return CanOpen != null && CanOpen.Count > 0; } }

        // TODO: Implement the following header values:
        // - newdat.datversionurl (currently reads and writes to Header.Url, not strictly correct)
        // - newdat.daturl (currently writes to Header.Url, not strictly correct)
        // - newdat.daturl[fileName] (currently writes to Header.FileName + ".zip", not strictly correct)
        // - newdat.imurl (currently writes to Header.Url, not strictly correct)
        // - search[...].to.find[operation, value (Int32?)]
        // - search[...].to[value, default (true|false), auto (true, false)]

        /// <summary>
        /// Rom title
        /// </summary>
        [JsonProperty("romtitle", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("romtitle")]
        public string? RomTitle { get; set; }

        #endregion

        #region RomCenter

        /// <summary>
        /// RomCenter DAT format version
        /// </summary>
        [JsonProperty("rcversion", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("rcversion")]
        public string? RomCenterVersion { get; set; }

        #endregion

        #region Write pre-processing

        /// <summary>
        /// Text to prepend to all outputted lines
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public string? Prefix { get; set; }

        /// <summary>
        /// Text to append to all outputted lines
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public string? Postfix { get; set; }

        /// <summary>
        /// Add a new extension to all items
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public string? AddExtension { get; set; }

        /// <summary>
        /// Replace all item extensions
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public string? ReplaceExtension { get; set; }

        /// <summary>
        /// Remove all item extensions
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public bool RemoveExtension { get; set; }

        /// <summary>
        /// Output the machine name
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public bool GameName { get; set; }

        /// <summary>
        /// Wrap quotes around the entire line, sans prefix and postfix
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public bool Quotes { get; set; }

        #endregion

        #region Depot Information

        /// <summary>
        /// Input depot information
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public DepotInformation? InputDepot { get; set; }

        /// <summary>
        /// Output depot information
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public DepotInformation? OutputDepot { get; set; }

        #endregion

        #endregion

        #region Instance Methods

        #region Cloning Methods

        /// <summary>
        /// Clone the current header
        /// </summary>
        public object Clone()
        {
            return new DatHeader()
            {
                FileName = this.FileName,
                Name = this.Name,
                Description = this.Description,
                RootDir = this.RootDir,
                Category = this.Category,
                Version = this.Version,
                Date = this.Date,
                Author = this.Author,
                Email = this.Email,
                Homepage = this.Homepage,
                Url = this.Url,
                Comment = this.Comment,
                HeaderSkipper = this.HeaderSkipper,
                Type = this.Type,
                ForceMerging = this.ForceMerging,
                ForceNodump = this.ForceNodump,
                ForcePacking = this.ForcePacking,
                DatFormat = this.DatFormat,

                Debug = this.Debug,
                MameConfig = this.MameConfig,

                NoIntroID = this.NoIntroID,
                Build = this.Build,
                System = this.System,
                RomMode = this.RomMode,
                BiosMode = this.BiosMode,
                SampleMode = this.SampleMode,
                LockRomMode = this.LockRomMode,
                LockBiosMode = this.LockBiosMode,
                LockSampleMode = this.LockSampleMode,

                ScreenshotsWidth = this.ScreenshotsWidth,
                ScreenshotsHeight = this.ScreenshotsHeight,
                Infos = this.Infos, // TODO: Perform a deep clone
                CanOpen = this.CanOpen, // TODO: Perform a deep clone
                RomTitle = this.RomTitle,

                RomCenterVersion = this.RomCenterVersion,

                UseRomName = this.UseRomName,
                Prefix = this.Prefix,
                Postfix = this.Postfix,
                Quotes = this.Quotes,
                ReplaceExtension = this.ReplaceExtension,
                AddExtension = this.AddExtension,
                RemoveExtension = this.RemoveExtension,
                GameName = this.GameName,
                InputDepot = this.InputDepot?.Clone() as DepotInformation,
                OutputDepot = this.OutputDepot?.Clone() as DepotInformation,
            };
        }

        /// <summary>
        /// Clone the standard parts of the current header
        /// </summary>
        public DatHeader CloneStandard()
        {
            return new DatHeader()
            {
                FileName = this.FileName,
                Name = this.Name,
                Description = this.Description,
                RootDir = this.RootDir,
                Category = this.Category,
                Version = this.Version,
                Date = this.Date,
                Author = this.Author,
                Email = this.Email,
                Homepage = this.Homepage,
                Url = this.Url,
                Comment = this.Comment,
                HeaderSkipper = this.HeaderSkipper,
                Type = this.Type,
                ForceMerging = this.ForceMerging,
                ForceNodump = this.ForceNodump,
                ForcePacking = this.ForcePacking,
                DatFormat = this.DatFormat,
            };
        }

        /// <summary>
        /// Clone the filtering parts of the current header
        /// </summary>
        public DatHeader CloneFiltering()
        {
            return new DatHeader()
            {
                DatFormat = this.DatFormat,

                UseRomName = this.UseRomName,
                Prefix = this.Prefix,
                Postfix = this.Postfix,
                Quotes = this.Quotes,
                ReplaceExtension = this.ReplaceExtension,
                AddExtension = this.AddExtension,
                RemoveExtension = this.RemoveExtension,
                GameName = this.GameName,
                InputDepot = this.InputDepot?.Clone() as DepotInformation,
                OutputDepot = this.OutputDepot?.Clone() as DepotInformation,
            };
        }

        /// <summary>
        /// Overwrite local values from another DatHeader if not default
        /// </summary>
        /// <param name="datHeader">DatHeader to get the values from</param>
        public void ConditionalCopy(DatHeader? datHeader)
        {
            if (datHeader == null)
                return;

            if (!string.IsNullOrEmpty(datHeader.FileName))
                FileName = datHeader.FileName;

            if (!string.IsNullOrEmpty(datHeader.Name))
                Name = datHeader.Name;

            if (!string.IsNullOrEmpty(datHeader.Description))
                Description = datHeader.Description;

            if (!string.IsNullOrEmpty(datHeader.RootDir))
                RootDir = datHeader.RootDir;

            if (!string.IsNullOrEmpty(datHeader.Category))
                Category = datHeader.Category;

            if (!string.IsNullOrEmpty(datHeader.Version))
                Version = datHeader.Version;

            if (!string.IsNullOrEmpty(datHeader.Date))
                Date = datHeader.Date;

            if (!string.IsNullOrEmpty(datHeader.Author))
                Author = datHeader.Author;

            if (!string.IsNullOrEmpty(datHeader.Email))
                Email = datHeader.Email;

            if (!string.IsNullOrEmpty(datHeader.Homepage))
                Homepage = datHeader.Homepage;

            if (!string.IsNullOrEmpty(datHeader.Url))
                Url = datHeader.Url;

            if (!string.IsNullOrEmpty(datHeader.Comment))
                Comment = datHeader.Comment;

            if (!string.IsNullOrEmpty(datHeader.HeaderSkipper))
                HeaderSkipper = datHeader.HeaderSkipper;

            if (!string.IsNullOrEmpty(datHeader.Type))
                Type = datHeader.Type;

            if (datHeader.ForceMerging != MergingFlag.None)
                ForceMerging = datHeader.ForceMerging;

            if (datHeader.ForceNodump != NodumpFlag.None)
                ForceNodump = datHeader.ForceNodump;

            if (datHeader.ForcePacking != PackingFlag.None)
                ForcePacking = datHeader.ForcePacking;

            if (datHeader.DatFormat != 0x00)
                DatFormat = datHeader.DatFormat;

            if (!string.IsNullOrEmpty(datHeader.Prefix))
                Prefix = datHeader.Prefix;

            if (!string.IsNullOrEmpty(datHeader.Postfix))
                Postfix = datHeader.Postfix;

            if (!string.IsNullOrEmpty(datHeader.AddExtension))
                AddExtension = datHeader.AddExtension;

            if (!string.IsNullOrEmpty(datHeader.ReplaceExtension))
                ReplaceExtension = datHeader.ReplaceExtension;

            RemoveExtension = datHeader.RemoveExtension;
            InputDepot = datHeader.InputDepot?.Clone() as DepotInformation;
            OutputDepot = datHeader.OutputDepot?.Clone() as DepotInformation;
            GameName = datHeader.GameName;
            Quotes = datHeader.Quotes;
            UseRomName = datHeader.UseRomName;
        }

        #endregion

        #region Manipulation

        //// <summary>
        /// Remove a field from the header
        /// </summary>
        /// <param name="fieldName">Field to remove</param>
        /// <returns>True if the removal was successful, false otherwise</returns>
        public bool RemoveField(string fieldName)
        {
            DatHeaderField datHeaderField = fieldName.AsDatHeaderField();
            switch (datHeaderField)
            {
                case DatHeaderField.Author: Author = null; break;
                case DatHeaderField.BiosMode: BiosMode = MergingFlag.None; break;
                case DatHeaderField.Build: Build = null; break;
                case DatHeaderField.CanOpen: CanOpen = null; break;
                case DatHeaderField.Category: Category = null; break;
                case DatHeaderField.Comment: Comment = null; break;
                case DatHeaderField.Date: Date = null; break;
                case DatHeaderField.Debug: Debug = null; break;
                case DatHeaderField.Description: Description = null; break;
                case DatHeaderField.Email: Email = null; break;
                case DatHeaderField.FileName: FileName = null; break;
                case DatHeaderField.ForceMerging: ForceMerging = MergingFlag.None; break;
                case DatHeaderField.ForceNodump: ForceNodump = NodumpFlag.None; break;
                case DatHeaderField.ForcePacking: ForcePacking = PackingFlag.None; break;
                case DatHeaderField.HeaderSkipper: HeaderSkipper = null; break;
                case DatHeaderField.Homepage: Homepage = null; break;
                case DatHeaderField.ID: NoIntroID = null; break;
                // case DatHeaderField.Info_Default: Info_Default = null; break;
                // case DatHeaderField.Info_IsNamingOption: Info_IsNamingOption = null; break;
                // case DatHeaderField.Info_Name: Info_Name = null; break;
                // case DatHeaderField.Info_Visible: Info_Visible = null; break;
                case DatHeaderField.LockBiosMode: LockBiosMode = null; break;
                case DatHeaderField.LockRomMode: LockRomMode = null; break;
                case DatHeaderField.LockSampleMode: LockSampleMode = null; break;
                case DatHeaderField.MameConfig: MameConfig = null; break;
                case DatHeaderField.Name: Name = null; break;
                case DatHeaderField.RomCenterVersion: RomCenterVersion = null; break;
                case DatHeaderField.RomMode: RomMode = MergingFlag.None; break;
                case DatHeaderField.RomTitle: RomTitle = null; break;
                case DatHeaderField.RootDir: RootDir = null; break;
                case DatHeaderField.SampleMode: SampleMode = MergingFlag.None; break;
                case DatHeaderField.ScreenshotsHeight: ScreenshotsHeight = null; break;
                case DatHeaderField.ScreenshotsWidth: ScreenshotsWidth = null; break;
                case DatHeaderField.System: System = null; break;
                case DatHeaderField.Type: Type = null; break;
                case DatHeaderField.Url: Url = null; break;
                case DatHeaderField.Version: Version = null; break;
                default: return false;
            }

            return true;
        }

        /// <summary>
        /// Set a field in the header from a mapping string
        /// </summary>
        /// <param name="fieldName">Field to set</param>
        /// <param name="value">String representing the value to set</param>
        /// <returns>True if the setting was successful, false otherwise</returns>
        /// <remarks>This only performs minimal validation before setting</remarks>
        public bool SetField(string? fieldName, string value)
        {
            DatHeaderField datHeaderField = fieldName.AsDatHeaderField();
            switch (datHeaderField)
            {
                case DatHeaderField.Author: Author = value; break;
                case DatHeaderField.BiosMode: BiosMode = value.AsEnumValue<MergingFlag>(); break;
                case DatHeaderField.Build: Build = value; break;
                case DatHeaderField.CanOpen: CanOpen = [.. value.Split(',')]; break;
                case DatHeaderField.Category: Category = value; break;
                case DatHeaderField.Comment: Comment = value; break;
                case DatHeaderField.Date: Date = value; break;
                case DatHeaderField.Debug: Debug = value.AsYesNo(); break;
                case DatHeaderField.Description: Description = value; break;
                case DatHeaderField.Email: Email = value; break;
                case DatHeaderField.FileName: FileName = value; break;
                case DatHeaderField.ForceMerging: ForceMerging = value.AsEnumValue<MergingFlag>(); break;
                case DatHeaderField.ForceNodump: ForceNodump = value.AsEnumValue<NodumpFlag>(); break;
                case DatHeaderField.ForcePacking: ForcePacking = value.AsEnumValue<PackingFlag>(); break;
                case DatHeaderField.HeaderSkipper: HeaderSkipper = value; break;
                case DatHeaderField.Homepage: Homepage = value; break;
                case DatHeaderField.ID: NoIntroID = value; break;
                // case DatHeaderField.Info_Default: Info_Default = value; break;
                // case DatHeaderField.Info_IsNamingOption: Info_IsNamingOption = value; break;
                // case DatHeaderField.Info_Name: Info_Name = value; break;
                // case DatHeaderField.Info_Visible: Info_Visible = value; break;
                case DatHeaderField.LockBiosMode: LockBiosMode = value.AsYesNo(); break;
                case DatHeaderField.LockRomMode: LockRomMode = value.AsYesNo(); break;
                case DatHeaderField.LockSampleMode: LockSampleMode = value.AsYesNo(); break;
                case DatHeaderField.MameConfig: MameConfig = value; break;
                case DatHeaderField.Name: Name = value; break;
                case DatHeaderField.RomCenterVersion: RomCenterVersion = value; break;
                case DatHeaderField.RomMode: RomMode = value.AsEnumValue<MergingFlag>(); break;
                case DatHeaderField.RomTitle: RomTitle = value; break;
                case DatHeaderField.RootDir: RootDir = value; break;
                case DatHeaderField.SampleMode: SampleMode = value.AsEnumValue<MergingFlag>(); break;
                case DatHeaderField.ScreenshotsHeight: ScreenshotsHeight = value; break;
                case DatHeaderField.ScreenshotsWidth: ScreenshotsWidth = value; break;
                case DatHeaderField.System: System = value; break;
                case DatHeaderField.Type: Type = value; break;
                case DatHeaderField.Url: Url = value; break;
                case DatHeaderField.Version: Version = value; break;
                default: return false;
            }

            return true;
        }

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
            if ((DatFormat & DatFormat.CSV) != 0)
#else
            if (DatFormat.HasFlag(DatFormat.CSV))
#endif
            {
                outfileNames.Add(DatFormat.CSV, CreateOutFileNamesHelper(outDir, ".csv", overwrite));
                usedExtensions.Add(".csv");
            };

            #endregion

            #region .dat

            // ClrMamePro
#if NETFRAMEWORK
            if ((DatFormat & DatFormat.ClrMamePro) != 0)
#else
            if (DatFormat.HasFlag(DatFormat.ClrMamePro))
#endif
            {
                outfileNames.Add(DatFormat.ClrMamePro, CreateOutFileNamesHelper(outDir, ".dat", overwrite));
                usedExtensions.Add(".dat");
            };

            // RomCenter
#if NETFRAMEWORK
            if ((DatFormat & DatFormat.RomCenter) != 0)
#else
            if (DatFormat.HasFlag(DatFormat.RomCenter))
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
            if ((DatFormat & DatFormat.DOSCenter) != 0)
#else
            if (DatFormat.HasFlag(DatFormat.DOSCenter))
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
            if ((DatFormat & DatFormat.SabreJSON) != 0)
#else
            if (DatFormat.HasFlag(DatFormat.SabreJSON))
#endif
            {
                outfileNames.Add(DatFormat.SabreJSON, CreateOutFileNamesHelper(outDir, ".json", overwrite));
                usedExtensions.Add(".json");
            }

            #endregion

            #region .md5

            // Redump MD5
#if NETFRAMEWORK
            if ((DatFormat & DatFormat.RedumpMD5) != 0)
#else
            if (DatFormat.HasFlag(DatFormat.RedumpMD5))
#endif
            {
                outfileNames.Add(DatFormat.RedumpMD5, CreateOutFileNamesHelper(outDir, ".md5", overwrite));
                usedExtensions.Add(".md5");
            };

            #endregion

            #region .sfv

            // Redump SFV
#if NETFRAMEWORK
            if ((DatFormat & DatFormat.RedumpSFV) != 0)
#else
            if (DatFormat.HasFlag(DatFormat.RedumpSFV))
#endif
            {
                outfileNames.Add(DatFormat.RedumpSFV, CreateOutFileNamesHelper(outDir, ".sfv", overwrite));
                usedExtensions.Add(".sfv");
            };

            #endregion

            #region .sha1

            // Redump SHA-1
#if NETFRAMEWORK
            if ((DatFormat & DatFormat.RedumpSHA1) != 0)
#else
            if (DatFormat.HasFlag(DatFormat.RedumpSHA1))
#endif
            {
                outfileNames.Add(DatFormat.RedumpSHA1, CreateOutFileNamesHelper(outDir, ".sha1", overwrite));
                usedExtensions.Add(".sha1");
            };

            #endregion

            #region .sha256

            // Redump SHA-256
#if NETFRAMEWORK
            if ((DatFormat & DatFormat.RedumpSHA256) != 0)
#else
            if (DatFormat.HasFlag(DatFormat.RedumpSHA256))
#endif
            {
                outfileNames.Add(DatFormat.RedumpSHA256, CreateOutFileNamesHelper(outDir, ".sha256", overwrite));
                usedExtensions.Add(".sha256");
            };

            #endregion

            #region .sha384

            // Redump SHA-384
#if NETFRAMEWORK
            if ((DatFormat & DatFormat.RedumpSHA384) != 0)
#else
            if (DatFormat.HasFlag(DatFormat.RedumpSHA384))
#endif
            {
                outfileNames.Add(DatFormat.RedumpSHA384, CreateOutFileNamesHelper(outDir, ".sha384", overwrite));
                usedExtensions.Add(".sha384");
            };

            #endregion

            #region .sha512

            // Redump SHA-512
#if NETFRAMEWORK
            if ((DatFormat & DatFormat.RedumpSHA512) != 0)
#else
            if (DatFormat.HasFlag(DatFormat.RedumpSHA512))
#endif
            {
                outfileNames.Add(DatFormat.RedumpSHA512, CreateOutFileNamesHelper(outDir, ".sha512", overwrite));
                usedExtensions.Add(".sha512");
            };

            #endregion

            #region .spamsum

            // Redump SpamSum
#if NETFRAMEWORK
            if ((DatFormat & DatFormat.RedumpSpamSum) != 0)
#else
            if (DatFormat.HasFlag(DatFormat.RedumpSpamSum))
#endif
            {
                outfileNames.Add(DatFormat.RedumpSpamSum, CreateOutFileNamesHelper(outDir, ".spamsum", overwrite));
                usedExtensions.Add(".spamsum");
            };

            #endregion

            #region .ssv

            // SSV
#if NETFRAMEWORK
            if ((DatFormat & DatFormat.SSV) != 0)
#else
            if (DatFormat.HasFlag(DatFormat.SSV))
#endif
            {
                outfileNames.Add(DatFormat.SSV, CreateOutFileNamesHelper(outDir, ".ssv", overwrite));
                usedExtensions.Add(".ssv");
            };

            #endregion

            #region .tsv

            // TSV
#if NETFRAMEWORK
            if ((DatFormat & DatFormat.TSV) != 0)
#else
            if (DatFormat.HasFlag(DatFormat.TSV))
#endif
            {
                outfileNames.Add(DatFormat.TSV, CreateOutFileNamesHelper(outDir, ".tsv", overwrite));
                usedExtensions.Add(".tsv");
            };

            #endregion

            #region .txt

            // AttractMode
#if NETFRAMEWORK
            if ((DatFormat & DatFormat.AttractMode) != 0)
#else
            if (DatFormat.HasFlag(DatFormat.AttractMode))
#endif
            {
                outfileNames.Add(DatFormat.AttractMode, CreateOutFileNamesHelper(outDir, ".txt", overwrite));
                usedExtensions.Add(".txt");
            }

            // MAME Listroms
#if NETFRAMEWORK
            if ((DatFormat & DatFormat.Listrom) != 0)
#else
            if (DatFormat.HasFlag(DatFormat.Listrom))
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
            if ((DatFormat & DatFormat.MissFile) != 0)
#else
            if (DatFormat.HasFlag(DatFormat.MissFile))
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
            if ((DatFormat & DatFormat.EverdriveSMDB) != 0)
#else
            if (DatFormat.HasFlag(DatFormat.EverdriveSMDB))
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
            if ((DatFormat & DatFormat.Logiqx) != 0)
#else
            if (DatFormat.HasFlag(DatFormat.Logiqx))
#endif
            {
                outfileNames.Add(DatFormat.Logiqx, CreateOutFileNamesHelper(outDir, ".xml", overwrite));
                usedExtensions.Add(".xml");
            }
#if NETFRAMEWORK
            if ((DatFormat & DatFormat.LogiqxDeprecated) != 0)
#else
            if (DatFormat.HasFlag(DatFormat.LogiqxDeprecated))
#endif
            {
                outfileNames.Add(DatFormat.LogiqxDeprecated, CreateOutFileNamesHelper(outDir, ".xml", overwrite));
                usedExtensions.Add(".xml");
            }

            // SabreDAT
#if NETFRAMEWORK
            if ((DatFormat & DatFormat.SabreXML) != 0)
#else
            if (DatFormat.HasFlag(DatFormat.SabreXML))
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
            if ((DatFormat & DatFormat.SoftwareList) != 0)
#else
            if (DatFormat.HasFlag(DatFormat.SoftwareList))
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
            if ((DatFormat & DatFormat.Listxml) != 0)
#else
            if (DatFormat.HasFlag(DatFormat.Listxml))
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
            if ((DatFormat & DatFormat.OfflineList) != 0)
#else
            if (DatFormat.HasFlag(DatFormat.OfflineList))
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
            if ((DatFormat & DatFormat.OpenMSX) != 0)
#else
            if (DatFormat.HasFlag(DatFormat.OpenMSX))
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
            if ((DatFormat & DatFormat.ArchiveDotOrg) != 0)
#else
            if (DatFormat.HasFlag(DatFormat.ArchiveDotOrg))
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
            string? filename = string.IsNullOrEmpty(FileName) ? Description : FileName;

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
