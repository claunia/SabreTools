using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.Filtering;
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
        public void SetFields(Dictionary<Field, string> mappings)
        {
            #region Common

            if (mappings.Keys.Contains(Field.Machine_Name))
                Name = mappings[Field.Machine_Name];

            if (mappings.Keys.Contains(Field.Machine_Comment))
                Comment = mappings[Field.Machine_Comment];

            if (mappings.Keys.Contains(Field.Machine_Description))
                Description = mappings[Field.Machine_Description];

            if (mappings.Keys.Contains(Field.Machine_Year))
                Year = mappings[Field.Machine_Year];

            if (mappings.Keys.Contains(Field.Machine_Manufacturer))
                Manufacturer = mappings[Field.Machine_Manufacturer];

            if (mappings.Keys.Contains(Field.Machine_Publisher))
                Publisher = mappings[Field.Machine_Publisher];

            if (mappings.Keys.Contains(Field.Machine_Category))
                Category = mappings[Field.Machine_Category];

            if (mappings.Keys.Contains(Field.Machine_RomOf))
                RomOf = mappings[Field.Machine_RomOf];

            if (mappings.Keys.Contains(Field.Machine_CloneOf))
                CloneOf = mappings[Field.Machine_CloneOf];

            if (mappings.Keys.Contains(Field.Machine_SampleOf))
                SampleOf = mappings[Field.Machine_SampleOf];

            if (mappings.Keys.Contains(Field.Machine_Type))
                MachineType = mappings[Field.Machine_Type].AsMachineType();

            #endregion

            #region AttractMode

            if (mappings.Keys.Contains(Field.Machine_Players))
                Players = mappings[Field.Machine_Players];

            if (mappings.Keys.Contains(Field.Machine_Rotation))
                Rotation = mappings[Field.Machine_Rotation];

            if (mappings.Keys.Contains(Field.Machine_Control))
                Control = mappings[Field.Machine_Control];

            if (mappings.Keys.Contains(Field.Machine_Status))
                Status = mappings[Field.Machine_Status];

            if (mappings.Keys.Contains(Field.Machine_DisplayCount))
                DisplayCount = mappings[Field.Machine_DisplayCount];

            if (mappings.Keys.Contains(Field.Machine_DisplayType))
                DisplayType = mappings[Field.Machine_DisplayType];

            if (mappings.Keys.Contains(Field.Machine_Buttons))
                Buttons = mappings[Field.Machine_Buttons];

            #endregion

            #region ListXML

            if (mappings.Keys.Contains(Field.Machine_SourceFile))
                SourceFile = mappings[Field.Machine_SourceFile];

            if (mappings.Keys.Contains(Field.Machine_Runnable))
                Runnable = mappings[Field.Machine_Runnable].AsRunnable();

            #endregion

            #region Logiqx

            if (mappings.Keys.Contains(Field.Machine_Board))
                Board = mappings[Field.Machine_Board];

            if (mappings.Keys.Contains(Field.Machine_RebuildTo))
                RebuildTo = mappings[Field.Machine_RebuildTo];

            #endregion

            #region Logiqx EmuArc

            if (mappings.Keys.Contains(Field.Machine_TitleID))
                TitleID = mappings[Field.Machine_TitleID];

            if (mappings.Keys.Contains(Field.Machine_Developer))
                Developer = mappings[Field.Machine_Developer];

            if (mappings.Keys.Contains(Field.Machine_Genre))
                Genre = mappings[Field.Machine_Genre];

            if (mappings.Keys.Contains(Field.Machine_Subgenre))
                Subgenre = mappings[Field.Machine_Subgenre];

            if (mappings.Keys.Contains(Field.Machine_Ratings))
                Ratings = mappings[Field.Machine_Ratings];

            if (mappings.Keys.Contains(Field.Machine_Score))
                Score = mappings[Field.Machine_Score];

            if (mappings.Keys.Contains(Field.Machine_Enabled))
                Enabled = mappings[Field.Machine_Enabled];

            if (mappings.Keys.Contains(Field.Machine_CRC))
                Crc = mappings[Field.Machine_CRC].AsYesNo();

            if (mappings.Keys.Contains(Field.Machine_RelatedTo))
                RelatedTo = mappings[Field.Machine_RelatedTo];

            #endregion

            #region OpenMSX

            if (mappings.Keys.Contains(Field.Machine_GenMSXID))
                GenMSXID = mappings[Field.Machine_GenMSXID];

            if (mappings.Keys.Contains(Field.Machine_System))
                System = mappings[Field.Machine_System];

            if (mappings.Keys.Contains(Field.Machine_Country))
                Country = mappings[Field.Machine_Country];

            #endregion

            #region SoftwareList

            if (mappings.Keys.Contains(Field.Machine_Supported))
                Supported = mappings[Field.Machine_Supported].AsSupported();

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
        /// Check to see if a Machine passes the filter
        /// </summary>
        /// <param name="filter">Filter to check against</param>
        /// <returns>True if the item passed the filter, false otherwise</returns>
        public bool PassesFilter(Filter filter)
        {
            #region Common

            // Machine_Name
            bool passes = filter.PassStringFilter(filter.Machine_Name, Name);
            if (filter.IncludeOfInGame)
            {
                passes |= filter.PassStringFilter(filter.Machine_Name, CloneOf);
                passes |= filter.PassStringFilter(filter.Machine_Name, RomOf);
            }
            if (!passes)
                return false;

            // Machine_Comment
            if (!filter.PassStringFilter(filter.Machine_Comment, Comment))
                return false;

            // Machine_Description
            if (!filter.PassStringFilter(filter.Machine_Description, Description))
                return false;

            // Machine_Year
            if (!filter.PassStringFilter(filter.Machine_Year, Year))
                return false;

            // Machine_Manufacturer
            if (!filter.PassStringFilter(filter.Machine_Manufacturer, Manufacturer))
                return false;

            // Machine_Publisher
            if (!filter.PassStringFilter(filter.Machine_Publisher, Publisher))
                return false;

            // Machine_Category
            if (!filter.PassStringFilter(filter.Machine_Category, Category))
                return false;

            // Machine_RomOf
            if (!filter.PassStringFilter(filter.Machine_RomOf, RomOf))
                return false;

            // Machine_CloneOf
            if (!filter.PassStringFilter(filter.Machine_CloneOf, CloneOf))
                return false;

            // Machine_SampleOf
            if (!filter.PassStringFilter(filter.Machine_SampleOf, SampleOf))
                return false;

            // Machine_Type
            if (filter.Machine_Type.MatchesPositive(0x0, MachineType) == false)
                return false;
            if (filter.Machine_Type.MatchesNegative(0x0, MachineType) == true)
                return false;

            #endregion

            #region AttractMode

            // Machine_Players
            if (!filter.PassStringFilter(filter.Machine_Players, Players))
                return false;

            // Machine_Rotation
            if (!filter.PassStringFilter(filter.Machine_Rotation, Rotation))
                return false;

            // Machine_Control
            if (!filter.PassStringFilter(filter.Machine_Control, Control))
                return false;

            // Machine_Status
            if (!filter.PassStringFilter(filter.Machine_Status, Status))
                return false;

            // Machine_DisplayCount
            if (!filter.PassStringFilter(filter.Machine_DisplayCount, DisplayCount))
                return false;

            // Machine_DisplayType
            if (!filter.PassStringFilter(filter.Machine_DisplayType, DisplayType))
                return false;

            // Machine_Buttons
            if (!filter.PassStringFilter(filter.Machine_Buttons, Buttons))
                return false;

            #endregion

            #region ListXML

            // Machine_SourceFile
            if (!filter.PassStringFilter(filter.Machine_SourceFile, SourceFile))
                return false;

            // Machine_Runnable
            if (filter.Machine_Runnable.MatchesPositive(Runnable.NULL, Runnable) == false)
                return false;
            if (filter.Machine_Runnable.MatchesNegative(Runnable.NULL, Runnable) == true)
                return false;

            #endregion

            #region Logiqx

            // Machine_Board
            if (!filter.PassStringFilter(filter.Machine_Board, Board))
                return false;

            // Machine_RebuildTo
            if (!filter.PassStringFilter(filter.Machine_RebuildTo, RebuildTo))
                return false;

            #endregion

            #region Logiqx EmuArc

            // Machine_TitleID
            if (!filter.PassStringFilter(filter.Machine_TitleID, TitleID))
                return false;

            // Machine_Developer
            if (!filter.PassStringFilter(filter.Machine_Developer, Developer))
                return false;

            // Machine_Genre
            if (!filter.PassStringFilter(filter.Machine_Genre, Genre))
                return false;

            // Machine_Subgenre
            if (!filter.PassStringFilter(filter.Machine_Subgenre, Subgenre))
                return false;

            // Machine_Ratings
            if (!filter.PassStringFilter(filter.Machine_Ratings, Ratings))
                return false;

            // Machine_Score
            if (!filter.PassStringFilter(filter.Machine_Score, Score))
                return false;

            // Machine_Enabled
            if (!filter.PassStringFilter(filter.Machine_Enabled, Enabled))
                return false;

            // Machine_CRC
            if (!filter.PassBoolFilter(filter.Machine_CRC, Crc))
                return false;

            // Machine_RelatedTo
            if (!filter.PassStringFilter(filter.Machine_RelatedTo, RelatedTo))
                return false;

            #endregion

            #region OpenMSX

            // Machine_GenMSXID
            if (!filter.PassStringFilter(filter.Machine_GenMSXID, GenMSXID))
                return false;

            // Machine_System
            if (!filter.PassStringFilter(filter.Machine_System, System))
                return false;

            // Machine_Country
            if (!filter.PassStringFilter(filter.Machine_Country, Country))
                return false;

            #endregion

            #region SoftwareList

            // Machine_Supported
            if (filter.Machine_Supported.MatchesPositive(Supported.NULL, Supported) == false)
                return false;
            if (filter.Machine_Supported.MatchesNegative(Supported.NULL, Supported) == true)
                return false;

            #endregion // SoftwareList

            return true;
        }

        /// <summary>
        /// Remove fields from the Machine
        /// </summary>
        /// <param name="fields">List of Fields to remove</param>
        public void RemoveFields(List<Field> fields)
        {
            #region Common

            if (fields.Contains(Field.Machine_Name))
                Name = null;

            if (fields.Contains(Field.Machine_Comment))
                Comment = null;

            if (fields.Contains(Field.Machine_Description))
                Description = null;

            if (fields.Contains(Field.Machine_Year))
                Year = null;

            if (fields.Contains(Field.Machine_Manufacturer))
                Manufacturer = null;

            if (fields.Contains(Field.Machine_Publisher))
                Publisher = null;

            if (fields.Contains(Field.Machine_Category))
                Category = null;

            if (fields.Contains(Field.Machine_RomOf))
                RomOf = null;

            if (fields.Contains(Field.Machine_CloneOf))
                CloneOf = null;

            if (fields.Contains(Field.Machine_SampleOf))
                SampleOf = null;

            if (fields.Contains(Field.Machine_Type))
                MachineType = 0x0;

            #endregion

            #region AttractMode

            if (fields.Contains(Field.Machine_Players))
                Players = null;

            if (fields.Contains(Field.Machine_Rotation))
                Rotation = null;

            if (fields.Contains(Field.Machine_Control))
                Control = null;

            if (fields.Contains(Field.Machine_Status))
                Status = null;

            if (fields.Contains(Field.Machine_DisplayCount))
                DisplayCount = null;

            if (fields.Contains(Field.Machine_DisplayType))
                DisplayType = null;

            if (fields.Contains(Field.Machine_Buttons))
                Buttons = null;

            #endregion

            #region ListXML

            if (fields.Contains(Field.Machine_SourceFile))
                SourceFile = null;

            if (fields.Contains(Field.Machine_Runnable))
                Runnable = Runnable.NULL;

            #endregion

            #region Logiqx

            if (fields.Contains(Field.Machine_Board))
                Board = null;

            if (fields.Contains(Field.Machine_RebuildTo))
                RebuildTo = null;

            #endregion

            #region Logiqx EmuArc

            if (fields.Contains(Field.Machine_TitleID))
                TitleID = null;

            if (fields.Contains(Field.Machine_Developer))
                Developer = null;

            if (fields.Contains(Field.Machine_Genre))
                Genre = null;

            if (fields.Contains(Field.Machine_Subgenre))
                Subgenre = null;

            if (fields.Contains(Field.Machine_Ratings))
                Ratings = null;

            if (fields.Contains(Field.Machine_Score))
                Score = null;

            if (fields.Contains(Field.Machine_Enabled))
                Enabled = null;

            if (fields.Contains(Field.Machine_CRC))
                Crc = null;

            if (fields.Contains(Field.Machine_RelatedTo))
                RelatedTo = null;

            #endregion

            #region OpenMSX

            if (fields.Contains(Field.Machine_GenMSXID))
                GenMSXID = null;

            if (fields.Contains(Field.Machine_System))
                System = null;

            if (fields.Contains(Field.Machine_Country))
                Country = null;

            #endregion

            #region SoftwareList

            if (fields.Contains(Field.Machine_Supported))
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
        public void ReplaceFields(Machine machine, List<Field> fields, bool onlySame)
        {
            #region Common

            if (fields.Contains(Field.Machine_Name))
                Name = machine.Name;

            if (fields.Contains(Field.Machine_Comment))
                Comment = machine.Comment;

            if (fields.Contains(Field.Machine_Description))
            {
                if (!onlySame || (onlySame && Name == Description))
                    Description = machine.Description;
            }

            if (fields.Contains(Field.Machine_Year))
                Year = machine.Year;

            if (fields.Contains(Field.Machine_Manufacturer))
                Manufacturer = machine.Manufacturer;

            if (fields.Contains(Field.Machine_Publisher))
                Publisher = machine.Publisher;

            if (fields.Contains(Field.Machine_Category))
                Category = machine.Category;

            if (fields.Contains(Field.Machine_RomOf))
                RomOf = machine.RomOf;

            if (fields.Contains(Field.Machine_CloneOf))
                CloneOf = machine.CloneOf;

            if (fields.Contains(Field.Machine_SampleOf))
                SampleOf = machine.SampleOf;

            if (fields.Contains(Field.Machine_Type))
                MachineType = machine.MachineType;

            #endregion

            #region AttractMode

            if (fields.Contains(Field.Machine_Players))
                Players = machine.Players;

            if (fields.Contains(Field.Machine_Rotation))
                Rotation = machine.Rotation;

            if (fields.Contains(Field.Machine_Control))
                Control = machine.Control;

            if (fields.Contains(Field.Machine_Status))
                Status = machine.Status;

            if (fields.Contains(Field.Machine_DisplayCount))
                DisplayCount = machine.DisplayCount;

            if (fields.Contains(Field.Machine_DisplayType))
                DisplayType = machine.DisplayType;

            if (fields.Contains(Field.Machine_Buttons))
                Buttons = machine.Buttons;

            #endregion

            #region ListXML

            if (fields.Contains(Field.Machine_SourceFile))
                SourceFile = machine.SourceFile;

            if (fields.Contains(Field.Machine_Runnable))
                Runnable = machine.Runnable;

            #endregion

            #region Logiqx

            if (fields.Contains(Field.Machine_Board))
                Board = machine.Board;

            if (fields.Contains(Field.Machine_RebuildTo))
                RebuildTo = machine.RebuildTo;

            #endregion

            #region Logiqx EmuArc

            if (fields.Contains(Field.Machine_TitleID))
                TitleID = machine.TitleID;

            if (fields.Contains(Field.Machine_Developer))
                Developer = machine.Developer;

            if (fields.Contains(Field.Machine_Genre))
                Genre = machine.Genre;

            if (fields.Contains(Field.Machine_Subgenre))
                Subgenre = machine.Subgenre;

            if (fields.Contains(Field.Machine_Ratings))
                Ratings = machine.Ratings;

            if (fields.Contains(Field.Machine_Score))
                Score = machine.Score;

            if (fields.Contains(Field.Machine_Enabled))
                Enabled = machine.Enabled;

            if (fields.Contains(Field.Machine_CRC))
                Crc = machine.Crc;

            if (fields.Contains(Field.Machine_RelatedTo))
                RelatedTo = machine.RelatedTo;

            #endregion

            #region OpenMSX

            if (fields.Contains(Field.Machine_GenMSXID))
                GenMSXID = machine.GenMSXID;

            if (fields.Contains(Field.Machine_System))
                System = machine.System;

            if (fields.Contains(Field.Machine_Country))
                Country = machine.Country;

            #endregion

            #region SoftwareList

            if (fields.Contains(Field.Machine_Supported))
                Supported = machine.Supported;

            #endregion
        }

        #endregion
    }
}
