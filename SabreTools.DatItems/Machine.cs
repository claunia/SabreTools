using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

using SabreTools.Core;
using SabreTools.Core.Tools;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

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
        public string Name { get; set; } = null;

        /// <summary>
        /// Additional notes
        /// </summary>
        /// <remarks>Known as "Extra" in AttractMode</remarks>
        [JsonProperty("comment", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("comment")]
        public string Comment { get; set; } = null;

        /// <summary>
        /// Extended description
        /// </summary>
        [JsonProperty("description", DefaultValueHandling = DefaultValueHandling.Include)]
        [XmlElement("description")]
        public string Description { get; set; } = null;

        /// <summary>
        /// Year(s) of release/manufacture
        /// </summary>
        [JsonProperty("year", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("year")]
        public string Year { get; set; } = null;

        /// <summary>
        /// Manufacturer, if available
        /// </summary>
        [JsonProperty("manufacturer", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("manufacturer")]
        public string Manufacturer { get; set; } = null;

        /// <summary>
        /// Publisher, if available
        /// </summary>
        [JsonProperty("publisher", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("publisher")]
        public string Publisher { get; set; } = null;

        /// <summary>
        /// Category, if available
        /// </summary>
        [JsonProperty("category", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("category")]
        public string Category { get; set; } = null;

        /// <summary>
        /// fomof parent
        /// </summary>
        [JsonProperty("romof", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("romof")]
        public string RomOf { get; set; } = null;

        /// <summary>
        /// cloneof parent
        /// </summary>
        [JsonProperty("cloneof", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("cloneof")]
        public string CloneOf { get; set; } = null;

        /// <summary>
        /// sampleof parent
        /// </summary>
        [JsonProperty("sampleof", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("sampleof")]
        public string SampleOf { get; set; } = null;

        /// <summary>
        /// Type of the machine
        /// </summary>
        [JsonProperty("type", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonConverter(typeof(StringEnumConverter))]
        [XmlElement("type")]
        public MachineType MachineType { get; set; } = 0x0;

        [JsonIgnore]
        public bool MachineTypeSpecified { get { return MachineType != 0x0 && MachineType != MachineType.NULL; } }

        #endregion

        #region AttractMode

        /// <summary>
        /// Player count
        /// </summary>
        /// <remarks>Also in Logiqx EmuArc</remarks>
        [JsonProperty("players", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("players")]
        public string Players { get; set; } = null;

        /// <summary>
        /// Screen rotation
        /// </summary>
        [JsonProperty("rotation", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("rotation")]
        public string Rotation { get; set; } = null;

        /// <summary>
        /// Control method
        /// </summary>
        [JsonProperty("control", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("control")]
        public string Control { get; set; } = null;

        /// <summary>
        /// Support status
        /// </summary>
        [JsonProperty("status", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("status")]
        public string Status { get; set; } = null;

        /// <summary>
        /// Display count
        /// </summary>
        [JsonProperty("displaycount", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("displaycount")]
        public string DisplayCount { get; set; } = null;

        /// <summary>
        /// Display type
        /// </summary>
        [JsonProperty("displaytype", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("displaytype")]
        public string DisplayType { get; set; } = null;

        /// <summary>
        /// Number of input buttons
        /// </summary>
        [JsonProperty("buttons", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("buttons")]
        public string Buttons { get; set; } = null;

        #endregion

        #region ListXML

        /// <summary>
        /// Emulator source file related to the machine
        /// </summary>
        /// <remarks>Also in Logiqx</remarks>
        [JsonProperty("sourcefile", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("sourcefile")]
        public string SourceFile { get; set; } = null;

        /// <summary>
        /// Machine runnable status
        /// </summary>
        /// <remarks>Also in Logiqx</remarks>
        [JsonProperty("runnable", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("runnable")]
        public Runnable Runnable { get; set; } = Runnable.NULL;

        [JsonIgnore]
        public bool RunnableSpecified { get { return Runnable != Runnable.NULL; } }

        #endregion

        #region Logiqx

        /// <summary>
        /// Machine board name
        /// </summary>
        [JsonProperty("board", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("board")]
        public string Board { get; set; } = null;

        /// <summary>
        /// Rebuild location if different than machine name
        /// </summary>
        [JsonProperty("rebuildto", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("rebuildto")]
        public string RebuildTo { get; set; } = null;

        #endregion

        // TODO: Should this be a separate object for TruRip?
        #region Logiqx EmuArc

        /// <summary>
        /// Title ID
        /// </summary>
        [JsonProperty("titleid", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("titleid")]
        public string TitleID { get; set; } = null;

        /// <summary>
        /// Machine developer
        /// </summary>
        [JsonProperty("developer", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("developer")]
        public string Developer { get; set; } = null;

        /// <summary>
        /// Game genre
        /// </summary>
        [JsonProperty("genre", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("genre")]
        public string Genre { get; set; } = null;

        /// <summary>
        /// Game subgenre
        /// </summary>
        [JsonProperty("subgenre", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("subgenre")]
        public string Subgenre { get; set; } = null;

        /// <summary>
        /// Game ratings
        /// </summary>
        [JsonProperty("ratings", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("ratings")]
        public string Ratings { get; set; } = null;

        /// <summary>
        /// Game score
        /// </summary>
        [JsonProperty("score", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("score")]
        public string Score { get; set; } = null;

        /// <summary>
        /// Is the machine enabled
        /// </summary>
        [JsonProperty("enabled", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("enabled")]
        public string Enabled { get; set; } = null; // bool?

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
        public string RelatedTo { get; set; } = null;

        #endregion

        #region OpenMSX

        /// <summary>
        /// Generation MSX ID
        /// </summary>
        [JsonProperty("genmsxid", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("genmsxid")]
        public string GenMSXID { get; set; } = null;

        /// <summary>
        /// MSX System
        /// </summary>
        [JsonProperty("system", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("system")]
        public string System { get; set; } = null;

        /// <summary>
        /// Machine country of origin
        /// </summary>
        [JsonProperty("country", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("country")]
        public string Country { get; set; } = null;

        #endregion

        #region SoftwareList

        /// <summary>
        /// Support status
        /// </summary>
        [JsonProperty("supported", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("supported")]
        public Supported Supported { get; set; } = Supported.NULL;

        [JsonIgnore]
        public bool SupportedSpecified { get { return Supported != Supported.NULL; } }

        #endregion

        #endregion // Fields

        #region Accessors

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="mappings">Mappings dictionary</param>
        public void SetFields(Dictionary<MachineField, string> mappings)
        {
            if (mappings == null)
                return;

            #region Common

            if (mappings.Keys.Contains(MachineField.Name))
                Name = mappings[MachineField.Name];

            if (mappings.Keys.Contains(MachineField.Comment))
                Comment = mappings[MachineField.Comment];

            if (mappings.Keys.Contains(MachineField.Description))
                Description = mappings[MachineField.Description];

            if (mappings.Keys.Contains(MachineField.Year))
                Year = mappings[MachineField.Year];

            if (mappings.Keys.Contains(MachineField.Manufacturer))
                Manufacturer = mappings[MachineField.Manufacturer];

            if (mappings.Keys.Contains(MachineField.Publisher))
                Publisher = mappings[MachineField.Publisher];

            if (mappings.Keys.Contains(MachineField.Category))
                Category = mappings[MachineField.Category];

            if (mappings.Keys.Contains(MachineField.RomOf))
                RomOf = mappings[MachineField.RomOf];

            if (mappings.Keys.Contains(MachineField.CloneOf))
                CloneOf = mappings[MachineField.CloneOf];

            if (mappings.Keys.Contains(MachineField.SampleOf))
                SampleOf = mappings[MachineField.SampleOf];

            if (mappings.Keys.Contains(MachineField.Type))
                MachineType = mappings[MachineField.Type].AsMachineType();

            #endregion

            #region AttractMode

            if (mappings.Keys.Contains(MachineField.Players))
                Players = mappings[MachineField.Players];

            if (mappings.Keys.Contains(MachineField.Rotation))
                Rotation = mappings[MachineField.Rotation];

            if (mappings.Keys.Contains(MachineField.Control))
                Control = mappings[MachineField.Control];

            if (mappings.Keys.Contains(MachineField.Status))
                Status = mappings[MachineField.Status];

            if (mappings.Keys.Contains(MachineField.DisplayCount))
                DisplayCount = mappings[MachineField.DisplayCount];

            if (mappings.Keys.Contains(MachineField.DisplayType))
                DisplayType = mappings[MachineField.DisplayType];

            if (mappings.Keys.Contains(MachineField.Buttons))
                Buttons = mappings[MachineField.Buttons];

            #endregion

            #region ListXML

            if (mappings.Keys.Contains(MachineField.SourceFile))
                SourceFile = mappings[MachineField.SourceFile];

            if (mappings.Keys.Contains(MachineField.Runnable))
                Runnable = mappings[MachineField.Runnable].AsRunnable();

            #endregion

            #region Logiqx

            if (mappings.Keys.Contains(MachineField.Board))
                Board = mappings[MachineField.Board];

            if (mappings.Keys.Contains(MachineField.RebuildTo))
                RebuildTo = mappings[MachineField.RebuildTo];

            #endregion

            #region Logiqx EmuArc

            if (mappings.Keys.Contains(MachineField.TitleID))
                TitleID = mappings[MachineField.TitleID];

            if (mappings.Keys.Contains(MachineField.Developer))
                Developer = mappings[MachineField.Developer];

            if (mappings.Keys.Contains(MachineField.Genre))
                Genre = mappings[MachineField.Genre];

            if (mappings.Keys.Contains(MachineField.Subgenre))
                Subgenre = mappings[MachineField.Subgenre];

            if (mappings.Keys.Contains(MachineField.Ratings))
                Ratings = mappings[MachineField.Ratings];

            if (mappings.Keys.Contains(MachineField.Score))
                Score = mappings[MachineField.Score];

            if (mappings.Keys.Contains(MachineField.Enabled))
                Enabled = mappings[MachineField.Enabled];

            if (mappings.Keys.Contains(MachineField.CRC))
                Crc = mappings[MachineField.CRC].AsYesNo();

            if (mappings.Keys.Contains(MachineField.RelatedTo))
                RelatedTo = mappings[MachineField.RelatedTo];

            #endregion

            #region OpenMSX

            if (mappings.Keys.Contains(MachineField.GenMSXID))
                GenMSXID = mappings[MachineField.GenMSXID];

            if (mappings.Keys.Contains(MachineField.System))
                System = mappings[MachineField.System];

            if (mappings.Keys.Contains(MachineField.Country))
                Country = mappings[MachineField.Country];

            #endregion

            #region SoftwareList

            if (mappings.Keys.Contains(MachineField.Supported))
                Supported = mappings[MachineField.Supported].AsSupported();

            #endregion
        }

        #endregion

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

                Name = this.Name,
                Comment = this.Comment,
                Description = this.Description,
                Year = this.Year,
                Manufacturer = this.Manufacturer,
                Publisher = this.Publisher,
                Category = this.Category,
                RomOf = this.RomOf,
                CloneOf = this.CloneOf,
                SampleOf = this.SampleOf,
                MachineType = this.MachineType,

                #endregion

                #region AttractMode

                Players = this.Players,
                Rotation = this.Rotation,
                Control = this.Control,
                Status = this.Status,
                DisplayCount = this.DisplayCount,
                DisplayType = this.DisplayType,
                Buttons = this.Buttons,

                #endregion

                #region ListXML

                SourceFile = this.SourceFile,
                Runnable = this.Runnable,

                #endregion

                #region Logiqx

                Board = this.Board,
                RebuildTo = this.RebuildTo,

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

                #region OpenMSX

                GenMSXID = this.GenMSXID,
                System = this.System,
                Country = this.Country,

                #endregion

                #region SoftwareList

                Supported = this.Supported,

                #endregion
            };
        }

        #endregion

        #region Filtering

        /// <summary>
        /// Remove fields from the Machine
        /// </summary>
        /// <param name="fields">List of Fields to remove</param>
        public void RemoveFields(List<MachineField> fields)
        {
            #region Common

            if (fields.Contains(MachineField.Name))
                Name = null;

            if (fields.Contains(MachineField.Comment))
                Comment = null;

            if (fields.Contains(MachineField.Description))
                Description = null;

            if (fields.Contains(MachineField.Year))
                Year = null;

            if (fields.Contains(MachineField.Manufacturer))
                Manufacturer = null;

            if (fields.Contains(MachineField.Publisher))
                Publisher = null;

            if (fields.Contains(MachineField.Category))
                Category = null;

            if (fields.Contains(MachineField.RomOf))
                RomOf = null;

            if (fields.Contains(MachineField.CloneOf))
                CloneOf = null;

            if (fields.Contains(MachineField.SampleOf))
                SampleOf = null;

            if (fields.Contains(MachineField.Type))
                MachineType = 0x0;

            #endregion

            #region AttractMode

            if (fields.Contains(MachineField.Players))
                Players = null;

            if (fields.Contains(MachineField.Rotation))
                Rotation = null;

            if (fields.Contains(MachineField.Control))
                Control = null;

            if (fields.Contains(MachineField.Status))
                Status = null;

            if (fields.Contains(MachineField.DisplayCount))
                DisplayCount = null;

            if (fields.Contains(MachineField.DisplayType))
                DisplayType = null;

            if (fields.Contains(MachineField.Buttons))
                Buttons = null;

            #endregion

            #region ListXML

            if (fields.Contains(MachineField.SourceFile))
                SourceFile = null;

            if (fields.Contains(MachineField.Runnable))
                Runnable = Runnable.NULL;

            #endregion

            #region Logiqx

            if (fields.Contains(MachineField.Board))
                Board = null;

            if (fields.Contains(MachineField.RebuildTo))
                RebuildTo = null;

            #endregion

            #region Logiqx EmuArc

            if (fields.Contains(MachineField.TitleID))
                TitleID = null;

            if (fields.Contains(MachineField.Developer))
                Developer = null;

            if (fields.Contains(MachineField.Genre))
                Genre = null;

            if (fields.Contains(MachineField.Subgenre))
                Subgenre = null;

            if (fields.Contains(MachineField.Ratings))
                Ratings = null;

            if (fields.Contains(MachineField.Score))
                Score = null;

            if (fields.Contains(MachineField.Enabled))
                Enabled = null;

            if (fields.Contains(MachineField.CRC))
                Crc = null;

            if (fields.Contains(MachineField.RelatedTo))
                RelatedTo = null;

            #endregion

            #region OpenMSX

            if (fields.Contains(MachineField.GenMSXID))
                GenMSXID = null;

            if (fields.Contains(MachineField.System))
                System = null;

            if (fields.Contains(MachineField.Country))
                Country = null;

            #endregion

            #region SoftwareList

            if (fields.Contains(MachineField.Supported))
                Supported = Supported.NULL;

            #endregion
        }

        #endregion

        #region Sorting and Merging

        /// <summary>
        /// Replace machine fields from another item
        /// </summary>
        /// <param name="item">DatItem to pull new information from</param>
        /// <param name="fields">List of Fields representing what should be updated</param>
        /// <param name="onlySame">True if descriptions should only be replaced if the game name is the same, false otherwise</param>
        public void ReplaceFields(Machine machine, List<MachineField> fields, bool onlySame)
        {
            #region Common

            if (fields.Contains(MachineField.Name))
                Name = machine.Name;

            if (fields.Contains(MachineField.Comment))
                Comment = machine.Comment;

            if (fields.Contains(MachineField.Description))
            {
                if (!onlySame || (onlySame && Name == Description))
                    Description = machine.Description;
            }

            if (fields.Contains(MachineField.Year))
                Year = machine.Year;

            if (fields.Contains(MachineField.Manufacturer))
                Manufacturer = machine.Manufacturer;

            if (fields.Contains(MachineField.Publisher))
                Publisher = machine.Publisher;

            if (fields.Contains(MachineField.Category))
                Category = machine.Category;

            if (fields.Contains(MachineField.RomOf))
                RomOf = machine.RomOf;

            if (fields.Contains(MachineField.CloneOf))
                CloneOf = machine.CloneOf;

            if (fields.Contains(MachineField.SampleOf))
                SampleOf = machine.SampleOf;

            if (fields.Contains(MachineField.Type))
                MachineType = machine.MachineType;

            #endregion

            #region AttractMode

            if (fields.Contains(MachineField.Players))
                Players = machine.Players;

            if (fields.Contains(MachineField.Rotation))
                Rotation = machine.Rotation;

            if (fields.Contains(MachineField.Control))
                Control = machine.Control;

            if (fields.Contains(MachineField.Status))
                Status = machine.Status;

            if (fields.Contains(MachineField.DisplayCount))
                DisplayCount = machine.DisplayCount;

            if (fields.Contains(MachineField.DisplayType))
                DisplayType = machine.DisplayType;

            if (fields.Contains(MachineField.Buttons))
                Buttons = machine.Buttons;

            #endregion

            #region ListXML

            if (fields.Contains(MachineField.SourceFile))
                SourceFile = machine.SourceFile;

            if (fields.Contains(MachineField.Runnable))
                Runnable = machine.Runnable;

            #endregion

            #region Logiqx

            if (fields.Contains(MachineField.Board))
                Board = machine.Board;

            if (fields.Contains(MachineField.RebuildTo))
                RebuildTo = machine.RebuildTo;

            #endregion

            #region Logiqx EmuArc

            if (fields.Contains(MachineField.TitleID))
                TitleID = machine.TitleID;

            if (fields.Contains(MachineField.Developer))
                Developer = machine.Developer;

            if (fields.Contains(MachineField.Genre))
                Genre = machine.Genre;

            if (fields.Contains(MachineField.Subgenre))
                Subgenre = machine.Subgenre;

            if (fields.Contains(MachineField.Ratings))
                Ratings = machine.Ratings;

            if (fields.Contains(MachineField.Score))
                Score = machine.Score;

            if (fields.Contains(MachineField.Enabled))
                Enabled = machine.Enabled;

            if (fields.Contains(MachineField.CRC))
                Crc = machine.Crc;

            if (fields.Contains(MachineField.RelatedTo))
                RelatedTo = machine.RelatedTo;

            #endregion

            #region OpenMSX

            if (fields.Contains(MachineField.GenMSXID))
                GenMSXID = machine.GenMSXID;

            if (fields.Contains(MachineField.System))
                System = machine.System;

            if (fields.Contains(MachineField.Country))
                Country = machine.Country;

            #endregion

            #region SoftwareList

            if (fields.Contains(MachineField.Supported))
                Supported = machine.Supported;

            #endregion
        }

        #endregion
    }
}
