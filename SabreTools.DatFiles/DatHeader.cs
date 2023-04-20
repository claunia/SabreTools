using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.DatFiles.Formats;
using Newtonsoft.Json;

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
        public string FileName { get; set; }

        /// <summary>
        /// Internal name of the DAT
        /// </summary>
        [JsonProperty("name", DefaultValueHandling = DefaultValueHandling.Include)]
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// DAT description
        /// </summary>
        [JsonProperty("description", DefaultValueHandling = DefaultValueHandling.Include)]
        [XmlElement("description")]
        public string Description { get; set; }

        /// <summary>
        /// Root directory for the files; currently TruRip/EmuARC-exclusive
        /// </summary>
        [JsonProperty("rootdir", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("rootdir")]
        public string RootDir { get; set; }

        /// <summary>
        /// General category of items found in the DAT
        /// </summary>
        [JsonProperty("category", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("category")]
        public string Category { get; set; }

        /// <summary>
        /// Version of the DAT
        /// </summary>
        [JsonProperty("version", DefaultValueHandling = DefaultValueHandling.Include)]
        [XmlElement("version")]
        public string Version { get; set; }

        /// <summary>
        /// Creation or modification date
        /// </summary>
        [JsonProperty("date", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("date")]
        public string Date { get; set; }

        /// <summary>
        /// List of authors who contributed to the DAT
        /// </summary>
        [JsonProperty("author", DefaultValueHandling = DefaultValueHandling.Include)]
        [XmlElement("author")]
        public string Author { get; set; }

        /// <summary>
        /// Email address for DAT author(s)
        /// </summary>
        [JsonProperty("email", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("email")]
        public string Email { get; set; }

        /// <summary>
        /// Author or distribution homepage name
        /// </summary>
        [JsonProperty("homepage", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("homepage")]
        public string Homepage { get; set; }

        /// <summary>
        /// Author or distribution URL
        /// </summary>
        [JsonProperty("url", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("url")]
        public string Url { get; set; }

        /// <summary>
        /// Any comment that does not already fit an existing field
        /// </summary>
        [JsonProperty("comment", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("comment")]
        public string Comment { get; set; }

        /// <summary>
        /// Header skipper to be used when loading the DAT
        /// </summary>
        [JsonProperty("header", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("header")]
        public string HeaderSkipper { get; set; }

        /// <summary>
        /// Classification of the DAT. Generally only used for SuperDAT
        /// </summary>
        [JsonProperty("type", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("type")]
        public string Type { get; set; }

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
        public string MameConfig { get; set; }

        #endregion

        #region Logiqx

        /// <summary>
        /// No-Intro system ID
        /// </summary>
        [JsonProperty("nointroid", DefaultValueHandling = DefaultValueHandling.Include)]
        [XmlElement("nointroid")]
        public string NoIntroID { get; set; }

        /// <summary>
        /// Build version
        /// </summary>
        [JsonProperty("build", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("build")]
        public string Build { get; set; }

        /// <summary>
        /// Logiqx/RomCenter plugin, OfflineList System
        /// </summary>
        [JsonProperty("system", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("system")]
        public string System { get; set; }

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
        public string ScreenshotsWidth { get; set; }

        /// <summary>
        /// Screenshots height
        /// </summary>
        [JsonProperty("screenshotsheight", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("screenshotsheight")]
        public string ScreenshotsHeight { get; set; }

        /// <summary>
        /// OfflineList info list
        /// </summary>
        [JsonProperty("infos", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("infos")]
        public List<OfflineListInfo> Infos { get; set; }

        [JsonIgnore]
        public bool InfosSpecified { get { return Infos != null && Infos.Count > 0; } }

        /// <summary>
        /// OfflineList can-open extensions
        /// </summary>
        [JsonProperty("canopen", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("canopen")]
        public List<string> CanOpen { get; set; }

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
        public string RomTitle { get; set; }

        #endregion

        #region RomCenter

        /// <summary>
        /// RomCenter DAT format version
        /// </summary>
        [JsonProperty("rcversion", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("rcversion")]
        public string RomCenterVersion { get; set; }

        #endregion

        #region Write pre-processing

        /// <summary>
        /// Text to prepend to all outputted lines
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public string Prefix { get; set; }

        /// <summary>
        /// Text to append to all outputted lines
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public string Postfix { get; set; }

        /// <summary>
        /// Add a new extension to all items
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public string AddExtension { get; set; }

        /// <summary>
        /// Replace all item extensions
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public string ReplaceExtension { get; set; }

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
        public DepotInformation InputDepot { get; set; }

        /// <summary>
        /// Output depot information
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public DepotInformation OutputDepot { get; set; }

        #endregion

        #endregion

        #region Instance Methods

        #region Accessors

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="mappings">Mappings dictionary</param>
        public void SetFields(Dictionary<DatHeaderField, string> mappings)
        {
            #region Common

            if (mappings.ContainsKey(DatHeaderField.FileName))
                FileName = mappings[DatHeaderField.FileName];

            if (mappings.ContainsKey(DatHeaderField.Name))
                Name = mappings[DatHeaderField.Name];

            if (mappings.ContainsKey(DatHeaderField.Description))
                Description = mappings[DatHeaderField.Description];

            if (mappings.ContainsKey(DatHeaderField.RootDir))
                RootDir = mappings[DatHeaderField.RootDir];

            if (mappings.ContainsKey(DatHeaderField.Category))
                Category = mappings[DatHeaderField.Category];

            if (mappings.ContainsKey(DatHeaderField.Version))
                Version = mappings[DatHeaderField.Version];

            if (mappings.ContainsKey(DatHeaderField.Date))
                Date = mappings[DatHeaderField.Date];

            if (mappings.ContainsKey(DatHeaderField.Author))
                Author = mappings[DatHeaderField.Author];

            if (mappings.ContainsKey(DatHeaderField.Email))
                Email = mappings[DatHeaderField.Email];

            if (mappings.ContainsKey(DatHeaderField.Homepage))
                Homepage = mappings[DatHeaderField.Homepage];

            if (mappings.ContainsKey(DatHeaderField.Url))
                Url = mappings[DatHeaderField.Url];

            if (mappings.ContainsKey(DatHeaderField.Comment))
                Comment = mappings[DatHeaderField.Comment];

            if (mappings.ContainsKey(DatHeaderField.HeaderSkipper))
                HeaderSkipper = mappings[DatHeaderField.HeaderSkipper];

            if (mappings.ContainsKey(DatHeaderField.Type))
                Type = mappings[DatHeaderField.Type];

            if (mappings.ContainsKey(DatHeaderField.ForceMerging))
                ForceMerging = mappings[DatHeaderField.ForceMerging].AsMergingFlag();

            if (mappings.ContainsKey(DatHeaderField.ForceNodump))
                ForceNodump = mappings[DatHeaderField.ForceNodump].AsNodumpFlag();

            if (mappings.ContainsKey(DatHeaderField.ForcePacking))
                ForcePacking = mappings[DatHeaderField.ForcePacking].AsPackingFlag();

            #endregion

            #region ListXML

            if (mappings.ContainsKey(DatHeaderField.Debug))
                Debug = mappings[DatHeaderField.Debug].AsYesNo();

            if (mappings.ContainsKey(DatHeaderField.MameConfig))
                MameConfig = mappings[DatHeaderField.MameConfig];

            #endregion

            #region Logiqx

            if (mappings.ContainsKey(DatHeaderField.ID))
                NoIntroID = mappings[DatHeaderField.ID];

            if (mappings.ContainsKey(DatHeaderField.Build))
                Build = mappings[DatHeaderField.Build];

            if (mappings.ContainsKey(DatHeaderField.RomMode))
                RomMode = mappings[DatHeaderField.RomMode].AsMergingFlag();

            if (mappings.ContainsKey(DatHeaderField.BiosMode))
                BiosMode = mappings[DatHeaderField.BiosMode].AsMergingFlag();

            if (mappings.ContainsKey(DatHeaderField.SampleMode))
                SampleMode = mappings[DatHeaderField.SampleMode].AsMergingFlag();

            if (mappings.ContainsKey(DatHeaderField.LockRomMode))
                LockRomMode = mappings[DatHeaderField.LockRomMode].AsYesNo();

            if (mappings.ContainsKey(DatHeaderField.LockBiosMode))
                LockBiosMode = mappings[DatHeaderField.LockBiosMode].AsYesNo();

            if (mappings.ContainsKey(DatHeaderField.LockSampleMode))
                LockSampleMode = mappings[DatHeaderField.LockSampleMode].AsYesNo();

            #endregion

            #region OfflineList

            if (mappings.ContainsKey(DatHeaderField.System))
                System = mappings[DatHeaderField.System];

            if (mappings.ContainsKey(DatHeaderField.ScreenshotsWidth))
                ScreenshotsWidth = mappings[DatHeaderField.ScreenshotsWidth];

            if (mappings.ContainsKey(DatHeaderField.ScreenshotsHeight))
                ScreenshotsHeight = mappings[DatHeaderField.ScreenshotsHeight];

            // TODO: Add DatHeader_Info*
            // TDOO: Add DatHeader_CanOpen*

            if (mappings.ContainsKey(DatHeaderField.RomTitle))
                RomTitle = mappings[DatHeaderField.RomTitle];

            #endregion

            #region RomCenter

            if (mappings.ContainsKey(DatHeaderField.RomCenterVersion))
                RomCenterVersion = mappings[DatHeaderField.RomCenterVersion];

            #endregion
        }

        #endregion

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
        public void ConditionalCopy(DatHeader datHeader)
        {
            if (!string.IsNullOrWhiteSpace(datHeader.FileName))
                FileName = datHeader.FileName;

            if (!string.IsNullOrWhiteSpace(datHeader.Name))
                Name = datHeader.Name;

            if (!string.IsNullOrWhiteSpace(datHeader.Description))
                Description = datHeader.Description;

            if (!string.IsNullOrWhiteSpace(datHeader.RootDir))
                RootDir = datHeader.RootDir;

            if (!string.IsNullOrWhiteSpace(datHeader.Category))
                Category = datHeader.Category;

            if (!string.IsNullOrWhiteSpace(datHeader.Version))
                Version = datHeader.Version;

            if (!string.IsNullOrWhiteSpace(datHeader.Date))
                Date = datHeader.Date;

            if (!string.IsNullOrWhiteSpace(datHeader.Author))
                Author = datHeader.Author;

            if (!string.IsNullOrWhiteSpace(datHeader.Email))
                Email = datHeader.Email;

            if (!string.IsNullOrWhiteSpace(datHeader.Homepage))
                Homepage = datHeader.Homepage;

            if (!string.IsNullOrWhiteSpace(datHeader.Url))
                Url = datHeader.Url;

            if (!string.IsNullOrWhiteSpace(datHeader.Comment))
                Comment = datHeader.Comment;

            if (!string.IsNullOrWhiteSpace(datHeader.HeaderSkipper))
                HeaderSkipper = datHeader.HeaderSkipper;

            if (!string.IsNullOrWhiteSpace(datHeader.Type))
                Type = datHeader.Type;

            if (datHeader.ForceMerging != MergingFlag.None)
                ForceMerging = datHeader.ForceMerging;

            if (datHeader.ForceNodump != NodumpFlag.None)
                ForceNodump = datHeader.ForceNodump;

            if (datHeader.ForcePacking != PackingFlag.None)
                ForcePacking = datHeader.ForcePacking;

            if (datHeader.DatFormat != 0x00)
                DatFormat = datHeader.DatFormat;

            if (!string.IsNullOrWhiteSpace(datHeader.Prefix))
                Prefix = datHeader.Prefix;

            if (!string.IsNullOrWhiteSpace(datHeader.Postfix))
                Postfix = datHeader.Postfix;

            if (!string.IsNullOrWhiteSpace(datHeader.AddExtension))
                AddExtension = datHeader.AddExtension;

            if (!string.IsNullOrWhiteSpace(datHeader.ReplaceExtension))
                ReplaceExtension = datHeader.ReplaceExtension;

            RemoveExtension = datHeader.RemoveExtension;
            InputDepot = datHeader.InputDepot?.Clone() as DepotInformation;
            OutputDepot = datHeader.OutputDepot?.Clone() as DepotInformation;
            GameName = datHeader.GameName;
            Quotes = datHeader.Quotes;
            UseRomName = datHeader.UseRomName;
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
            Dictionary<DatFormat, string> outfileNames = new();

            // Double check the outDir for the end delim
            if (!outDir.EndsWith(Path.DirectorySeparatorChar.ToString()))
                outDir += Path.DirectorySeparatorChar;

            // Get all used extensions
            List<string> usedExtensions = new();

            // Get the extensions from the output type

            #region .csv

            // CSV
            if (DatFormat.HasFlag(DatFormat.CSV))
            {
                outfileNames.Add(DatFormat.CSV, CreateOutFileNamesHelper(outDir, ".csv", overwrite));
                usedExtensions.Add(".csv");
            };

            #endregion

            #region .dat

            // ClrMamePro
            if (DatFormat.HasFlag(DatFormat.ClrMamePro))
            {
                outfileNames.Add(DatFormat.ClrMamePro, CreateOutFileNamesHelper(outDir, ".dat", overwrite));
                usedExtensions.Add(".dat");
            };

            // RomCenter
            if (DatFormat.HasFlag(DatFormat.RomCenter))
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
            if (DatFormat.HasFlag(DatFormat.DOSCenter))
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
            if (DatFormat.HasFlag(DatFormat.SabreJSON))
            {
                outfileNames.Add(DatFormat.SabreJSON, CreateOutFileNamesHelper(outDir, ".json", overwrite));
                usedExtensions.Add(".json");
            }

            #endregion

            #region .md5

            // Redump MD5
            if (DatFormat.HasFlag(DatFormat.RedumpMD5))
            {
                outfileNames.Add(DatFormat.RedumpMD5, CreateOutFileNamesHelper(outDir, ".md5", overwrite));
                usedExtensions.Add(".md5");
            };

            #endregion

            #region .sfv

            // Redump SFV
            if (DatFormat.HasFlag(DatFormat.RedumpSFV))
            {
                outfileNames.Add(DatFormat.RedumpSFV, CreateOutFileNamesHelper(outDir, ".sfv", overwrite));
                usedExtensions.Add(".sfv");
            };

            #endregion

            #region .sha1

            // Redump SHA-1
            if (DatFormat.HasFlag(DatFormat.RedumpSHA1))
            {
                outfileNames.Add(DatFormat.RedumpSHA1, CreateOutFileNamesHelper(outDir, ".sha1", overwrite));
                usedExtensions.Add(".sha1");
            };

            #endregion

            #region .sha256

            // Redump SHA-256
            if (DatFormat.HasFlag(DatFormat.RedumpSHA256))
            {
                outfileNames.Add(DatFormat.RedumpSHA256, CreateOutFileNamesHelper(outDir, ".sha256", overwrite));
                usedExtensions.Add(".sha256");
            };

            #endregion

            #region .sha384

            // Redump SHA-384
            if (DatFormat.HasFlag(DatFormat.RedumpSHA384))
            {
                outfileNames.Add(DatFormat.RedumpSHA384, CreateOutFileNamesHelper(outDir, ".sha384", overwrite));
                usedExtensions.Add(".sha384");
            };

            #endregion

            #region .sha512

            // Redump SHA-512
            if (DatFormat.HasFlag(DatFormat.RedumpSHA512))
            {
                outfileNames.Add(DatFormat.RedumpSHA512, CreateOutFileNamesHelper(outDir, ".sha512", overwrite));
                usedExtensions.Add(".sha512");
            };

            #endregion

            #region .spamsum

            // Redump SpamSum
            if (DatFormat.HasFlag(DatFormat.RedumpSpamSum))
            {
                outfileNames.Add(DatFormat.RedumpSpamSum, CreateOutFileNamesHelper(outDir, ".spamsum", overwrite));
                usedExtensions.Add(".spamsum");
            };

            #endregion

            #region .ssv

            // SSV
            if (DatFormat.HasFlag(DatFormat.SSV))
            {
                outfileNames.Add(DatFormat.SSV, CreateOutFileNamesHelper(outDir, ".ssv", overwrite));
                usedExtensions.Add(".ssv");
            };

            #endregion

            #region .tsv

            // TSV
            if (DatFormat.HasFlag(DatFormat.TSV))
            {
                outfileNames.Add(DatFormat.TSV, CreateOutFileNamesHelper(outDir, ".tsv", overwrite));
                usedExtensions.Add(".tsv");
            };

            #endregion

            #region .txt

            // AttractMode
            if (DatFormat.HasFlag(DatFormat.AttractMode))
            {
                outfileNames.Add(DatFormat.AttractMode, CreateOutFileNamesHelper(outDir, ".txt", overwrite));
                usedExtensions.Add(".txt");
            }

            // MAME Listroms
            if (DatFormat.HasFlag(DatFormat.Listrom))
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
            if (DatFormat.HasFlag(DatFormat.MissFile))
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
            if (DatFormat.HasFlag(DatFormat.EverdriveSMDB))
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
            if (DatFormat.HasFlag(DatFormat.Logiqx))
            {
                outfileNames.Add(DatFormat.Logiqx, CreateOutFileNamesHelper(outDir, ".xml", overwrite));
                usedExtensions.Add(".xml");
            }
            if (DatFormat.HasFlag(DatFormat.LogiqxDeprecated))
            {
                outfileNames.Add(DatFormat.LogiqxDeprecated, CreateOutFileNamesHelper(outDir, ".xml", overwrite));
                usedExtensions.Add(".xml");
            }

            // SabreDAT
            if (DatFormat.HasFlag(DatFormat.SabreXML))
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
            if (DatFormat.HasFlag(DatFormat.SoftwareList))
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
            if (DatFormat.HasFlag(DatFormat.Listxml))
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
            if (DatFormat.HasFlag(DatFormat.OfflineList))
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
            if (DatFormat.HasFlag(DatFormat.OpenMSX))
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
            if (DatFormat.HasFlag(DatFormat.ArchiveDotOrg))
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
            string filename = string.IsNullOrWhiteSpace(FileName) ? Description : FileName;

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
