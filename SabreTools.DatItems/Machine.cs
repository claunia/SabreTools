using System;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SabreTools.Core;
using SabreTools.Core.Tools;

namespace SabreTools.DatItems
{
    /// <summary>
    /// Represents the information specific to a set/game/machine
    /// </summary>
    [JsonObject("machine"), XmlRoot("machine")]
    public class Machine : ICloneable
    {
        #region Fields

        #region Common

        /// <summary>
        /// Name of the machine
        /// </summary>
        [JsonProperty("name", DefaultValueHandling = DefaultValueHandling.Include)]
        [XmlElement("name")]
        public string? Name
        {
            get => _machine.ReadString(Models.Internal.Machine.NameKey);
            set => _machine[Models.Internal.Machine.NameKey] = value;
        }

        /// <summary>
        /// Additional notes
        /// </summary>
        /// <remarks>Known as "Extra" in AttractMode</remarks>
        [JsonProperty("comment", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("comment")]
        public string? Comment
        {
            get => _machine.ReadString(Models.Internal.Machine.CommentKey);
            set => _machine[Models.Internal.Machine.CommentKey] = value;
        }

        /// <summary>
        /// Extended description
        /// </summary>
        [JsonProperty("description", DefaultValueHandling = DefaultValueHandling.Include)]
        [XmlElement("description")]
        public string? Description
        {
            get => _machine.ReadString(Models.Internal.Machine.DescriptionKey);
            set => _machine[Models.Internal.Machine.DescriptionKey] = value;
        }

        /// <summary>
        /// Year(s) of release/manufacture
        /// </summary>
        [JsonProperty("year", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("year")]
        public string? Year
        {
            get => _machine.ReadString(Models.Internal.Machine.YearKey);
            set => _machine[Models.Internal.Machine.YearKey] = value;
        }

        /// <summary>
        /// Manufacturer, if available
        /// </summary>
        [JsonProperty("manufacturer", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("manufacturer")]
        public string? Manufacturer
        {
            get => _machine.ReadString(Models.Internal.Machine.ManufacturerKey);
            set => _machine[Models.Internal.Machine.ManufacturerKey] = value;
        }

        /// <summary>
        /// Publisher, if available
        /// </summary>
        [JsonProperty("publisher", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("publisher")]
        public string? Publisher
        {
            get => _machine.ReadString(Models.Internal.Machine.PublisherKey);
            set => _machine[Models.Internal.Machine.PublisherKey] = value;
        }

        /// <summary>
        /// Category, if available
        /// </summary>
        [JsonProperty("category", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("category")]
        public string? Category
        {
            get => _machine.ReadString(Models.Internal.Machine.CategoryKey);
            set => _machine[Models.Internal.Machine.CategoryKey] = value;
        }

        /// <summary>
        /// fomof parent
        /// </summary>
        [JsonProperty("romof", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("romof")]
        public string? RomOf
        {
            get => _machine.ReadString(Models.Internal.Machine.RomOfKey);
            set => _machine[Models.Internal.Machine.RomOfKey] = value;
        }

        /// <summary>
        /// cloneof parent
        /// </summary>
        [JsonProperty("cloneof", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("cloneof")]
        public string? CloneOf
        {
            get => _machine.ReadString(Models.Internal.Machine.CloneOfKey);
            set => _machine[Models.Internal.Machine.CloneOfKey] = value;
        }

        /// <summary>
        /// sampleof parent
        /// </summary>
        [JsonProperty("sampleof", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("sampleof")]
        public string? SampleOf
        {
            get => _machine.ReadString(Models.Internal.Machine.SampleOfKey);
            set => _machine[Models.Internal.Machine.SampleOfKey] = value;
        }

        /// <summary>
        /// Type of the machine
        /// </summary>
        [JsonProperty("type", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonConverter(typeof(StringEnumConverter))]
        [XmlElement("type")]
        public MachineType MachineType
        {
            get
            {
                bool? isBios = _machine.ReadBool(Models.Internal.Machine.IsBiosKey);
                bool? isDevice = _machine.ReadBool(Models.Internal.Machine.IsDeviceKey);
                bool? isMechanical = _machine.ReadBool(Models.Internal.Machine.IsMechanicalKey);

                MachineType machineType = MachineType.None;
                if (isBios == true)
                    machineType |= MachineType.Bios;
                if (isDevice == true)
                    machineType |= MachineType.Device;
                if (isMechanical == true)
                    machineType |= MachineType.Mechanical;

                return machineType;
            }
            set
            {
                if (value.HasFlag(MachineType.Bios))
                    _machine[Models.Internal.Machine.IsBiosKey] = "yes";
                if (value.HasFlag(MachineType.Device))
                    _machine[Models.Internal.Machine.IsDeviceKey] = "yes";
                if (value.HasFlag(MachineType.Mechanical))
                    _machine[Models.Internal.Machine.IsMechanicalKey] = "yes";
            }
        }

        [JsonIgnore]
        public bool MachineTypeSpecified { get { return MachineType != 0x0 && MachineType != MachineType.None; } }

        #endregion

        #region AttractMode

        /// <summary>
        /// Player count
        /// </summary>
        /// <remarks>Also in Logiqx EmuArc</remarks>
        [JsonProperty("players", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("players")]
        public string? Players
        {
            get => _machine.ReadString(Models.Internal.Machine.PlayersKey);
            set => _machine[Models.Internal.Machine.PlayersKey] = value;
        }

        /// <summary>
        /// Screen rotation
        /// </summary>
        [JsonProperty("rotation", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("rotation")]
        public string? Rotation
        {
            get => _machine.ReadString(Models.Internal.Machine.RotationKey);
            set => _machine[Models.Internal.Machine.RotationKey] = value;
        }

        /// <summary>
        /// Control method
        /// </summary>
        [JsonProperty("control", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("control")]
        public string? Control
        {
            get => _machine.ReadString(Models.Internal.Machine.ControlKey);
            set => _machine[Models.Internal.Machine.ControlKey] = value;
        }

        /// <summary>
        /// Support status
        /// </summary>
        [JsonProperty("status", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("status")]
        public string? Status
        {
            get => _machine.ReadString(Models.Internal.Machine.StatusKey);
            set => _machine[Models.Internal.Machine.StatusKey] = value;
        }

        /// <summary>
        /// Display count
        /// </summary>
        [JsonProperty("displaycount", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("displaycount")]
        public string? DisplayCount
        {
            get => _machine.ReadString(Models.Internal.Machine.DisplayCountKey);
            set => _machine[Models.Internal.Machine.DisplayCountKey] = value;
        }

        /// <summary>
        /// Display type
        /// </summary>
        [JsonProperty("displaytype", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("displaytype")]
        public string? DisplayType
        {
            get => _machine.ReadString(Models.Internal.Machine.DisplayTypeKey);
            set => _machine[Models.Internal.Machine.DisplayTypeKey] = value;
        }

        /// <summary>
        /// Number of input buttons
        /// </summary>
        [JsonProperty("buttons", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("buttons")]
        public string? Buttons
        {
            get => _machine.ReadString(Models.Internal.Machine.ButtonsKey);
            set => _machine[Models.Internal.Machine.ButtonsKey] = value;
        }

        #endregion

        #region ListXML

        /// <summary>
        /// History.dat entry for the machine
        /// </summary>
        [JsonProperty("history", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("history")]
        public string? History
        {
            get => _machine.ReadString(Models.Internal.Machine.HistoryKey);
            set => _machine[Models.Internal.Machine.HistoryKey] = value;
        }

        /// <summary>
        /// Emulator source file related to the machine
        /// </summary>
        /// <remarks>Also in Logiqx</remarks>
        [JsonProperty("sourcefile", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("sourcefile")]
        public string? SourceFile
        {
            get => _machine.ReadString(Models.Internal.Machine.SourceFileKey);
            set => _machine[Models.Internal.Machine.SourceFileKey] = value;
        }

        /// <summary>
        /// Machine runnable status
        /// </summary>
        /// <remarks>Also in Logiqx</remarks>
        [JsonProperty("runnable", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("runnable")]
        public Runnable Runnable
        {
            get => _machine.ReadString(Models.Internal.Machine.RunnableKey).AsRunnable();
            set => _machine[Models.Internal.Machine.RunnableKey] = value.FromRunnable();
        }

        [JsonIgnore]
        public bool RunnableSpecified { get { return Runnable != Runnable.NULL; } }

        #endregion

        #region Logiqx

        /// <summary>
        /// Machine board name
        /// </summary>
        [JsonProperty("board", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("board")]
        public string? Board
        {
            get => _machine.ReadString(Models.Internal.Machine.BoardKey);
            set => _machine[Models.Internal.Machine.BoardKey] = value;
        }

        /// <summary>
        /// Rebuild location if different than machine name
        /// </summary>
        [JsonProperty("rebuildto", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("rebuildto")]
        public string? RebuildTo
        {
            get => _machine.ReadString(Models.Internal.Machine.RebuildToKey);
            set => _machine[Models.Internal.Machine.RebuildToKey] = value;
        }

        /// <summary>
        /// No-Intro ID for the game
        /// </summary>
        [JsonProperty("nointroid", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("nointroid")]
        public string? NoIntroId
        {
            get => _machine.ReadString(Models.Internal.Machine.IdKey);
            set => _machine[Models.Internal.Machine.IdKey] = value;
        }

        /// <summary>
        /// No-Intro ID for the game
        /// </summary>
        [JsonProperty("nointrocloneofid", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("nointrocloneofid")]
        public string? NoIntroCloneOfId
        {
            get => _machine.ReadString(Models.Internal.Machine.CloneOfIdKey);
            set => _machine[Models.Internal.Machine.CloneOfIdKey] = value;
        }

        #endregion

        // TODO: Should this be a separate object for TruRip?
        #region Logiqx EmuArc

        /// <summary>
        /// Title ID
        /// </summary>
        [JsonProperty("titleid", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("titleid")]
        public string? TitleID { get; set; } = null;

        /// <summary>
        /// Machine developer
        /// </summary>
        [JsonProperty("developer", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("developer")]
        public string? Developer { get; set; } = null;

        /// <summary>
        /// Game genre
        /// </summary>
        [JsonProperty("genre", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("genre")]
        public string? Genre { get; set; } = null;

        /// <summary>
        /// Game subgenre
        /// </summary>
        [JsonProperty("subgenre", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("subgenre")]
        public string? Subgenre { get; set; } = null;

        /// <summary>
        /// Game ratings
        /// </summary>
        [JsonProperty("ratings", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("ratings")]
        public string? Ratings { get; set; } = null;

        /// <summary>
        /// Game score
        /// </summary>
        [JsonProperty("score", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("score")]
        public string? Score { get; set; } = null;

        /// <summary>
        /// Is the machine enabled
        /// </summary>
        [JsonProperty("enabled", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("enabled")]
        public string? Enabled { get; set; } = null; // bool?

        /// <summary>
        /// Does the game have a CRC check
        /// </summary>
        [JsonProperty("hascrc", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("hascrc")]
        public bool? Crc { get; set; } = null;

        [JsonIgnore]
        public bool CrcSpecified { get { return Crc != null; } }

        /// <summary>
        /// Machine relations
        /// </summary>
        [JsonProperty("relatedto", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("relatedto")]
        public string? RelatedTo { get; set; } = null;

        #endregion

        #region OpenMSX

        /// <summary>
        /// Generation MSX ID
        /// </summary>
        [JsonProperty("genmsxid", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("genmsxid")]
        public string? GenMSXID
        {
            get => _machine.ReadString(Models.Internal.Machine.GenMSXIDKey);
            set => _machine[Models.Internal.Machine.GenMSXIDKey] = value;
        }

        /// <summary>
        /// MSX System
        /// </summary>
        [JsonProperty("system", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("system")]
        public string? System
        {
            get => _machine.ReadString(Models.Internal.Machine.SystemKey);
            set => _machine[Models.Internal.Machine.SystemKey] = value;
        }

        /// <summary>
        /// Machine country of origin
        /// </summary>
        [JsonProperty("country", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("country")]
        public string? Country
        {
            get => _machine.ReadString(Models.Internal.Machine.CountryKey);
            set => _machine[Models.Internal.Machine.CountryKey] = value;
        }

        #endregion

        #region SoftwareList

        /// <summary>
        /// Support status
        /// </summary>
        [JsonProperty("supported", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("supported")]
        public Supported Supported
        {
            get => _machine.ReadString(Models.Internal.Machine.SupportedKey).AsSupported();
            set => _machine[Models.Internal.Machine.SupportedKey] = value.FromSupported(verbose: true);
        }

        [JsonIgnore]
        public bool SupportedSpecified { get { return Supported != Supported.NULL; } }

        #endregion

        /// <summary>
        /// Internal Machine model
        /// </summary>
        [JsonIgnore]
        private Models.Internal.Machine _machine = new();

        #endregion // Fields

        #region Constructors

        /// <summary>
        /// Create a new Machine object
        /// </summary>
        public Machine()
        {
        }

        /// <summary>
        /// Create a new Machine object with the included information
        /// </summary>
        /// <param name="name">Name of the machine</param>
        /// <param name="description">Description of the machine</param>
        public Machine(string name, string description)
        {
            Name = name;
            Description = description;
        }

        #endregion

        #region Cloning methods

        /// <summary>
        /// Create a clone of the current machine
        /// </summary>
        /// <returns>New machine with the same values as the current one</returns>
        public object Clone()
        {
            return new Machine()
            {
                #region Common

                _machine = this._machine.Clone() as Models.Internal.Machine ?? new Models.Internal.Machine(),

                #endregion

                #region Logiqx EmuArc

                TitleID = this.TitleID,
                Developer = this.Developer,
                Genre = this.Genre,
                Subgenre = this.Subgenre,
                Ratings = this.Ratings,
                Score = this.Score,
                Enabled = this.Enabled,
                Crc = this.Crc,
                RelatedTo = this.RelatedTo,

                #endregion
            };
        }

        #endregion
    }
}
