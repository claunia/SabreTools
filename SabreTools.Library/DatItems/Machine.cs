using System;
using System.Collections.Generic;
using System.Linq;

using SabreTools.Library.Filtering;
using Newtonsoft.Json;
using SabreTools.Library.Tools;

namespace SabreTools.Library.DatItems
{
    /// <summary>
    /// Represents the information specific to a set/game/machine
    /// </summary>
    public class Machine : ICloneable
    {
        #region Fields

        #region Common Fields

        /// <summary>
        /// Name of the machine
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; } = null;

        /// <summary>
        /// Additional notes
        /// </summary>
        /// <remarks>Known as "Extra" in AttractMode</remarks>
        [JsonProperty("comment")]
        public string Comment { get; set; } = null;

        /// <summary>
        /// Extended description
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; } = null;

        /// <summary>
        /// Year(s) of release/manufacture
        /// </summary>
        [JsonProperty("year")]
        public string Year { get; set; } = null;

        /// <summary>
        /// Manufacturer, if available
        /// </summary>
        [JsonProperty("manufacturer")]
        public string Manufacturer { get; set; } = null;

        /// <summary>
        /// Publisher, if available
        /// </summary>
        [JsonProperty("publisher")]
        public string Publisher { get; set; } = null;

        /// <summary>
        /// Category, if available
        /// </summary>
        [JsonProperty("category")]
        public string Category { get; set; } = null;

        /// <summary>
        /// fomof parent
        /// </summary>
        [JsonProperty("romof")]
        public string RomOf { get; set; } = null;

        /// <summary>
        /// cloneof parent
        /// </summary>
        [JsonProperty("cloneof")]
        public string CloneOf { get; set; } = null;

        /// <summary>
        /// sampleof parent
        /// </summary>
        [JsonProperty("sampleof")]
        public string SampleOf { get; set; } = null;

        /// <summary>
        /// Type of the machine
        /// </summary>
        [JsonProperty("type")]
        public MachineType MachineType { get; set; } = MachineType.NULL;

        #endregion

        #region AttractMode Fields

        /// <summary>
        /// Player count
        /// </summary>
        /// <remarks>Also in Logiqx EmuArc</remarks>
        [JsonProperty("players")]
        public string Players { get; set; } = null;

        /// <summary>
        /// Screen rotation
        /// </summary>
        [JsonProperty("rotation")]
        public string Rotation { get; set; } = null;

        /// <summary>
        /// Control method
        /// </summary>
        [JsonProperty("control")]
        public string Control { get; set; } = null;

        /// <summary>
        /// Support status
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; } = null;

        /// <summary>
        /// Display count
        /// </summary>
        [JsonProperty("displaycount")]
        public string DisplayCount { get; set; } = null;

        /// <summary>
        /// Display type
        /// </summary>
        [JsonProperty("displaytype")]
        public string DisplayType { get; set; } = null;

        /// <summary>
        /// Number of input buttons
        /// </summary>
        [JsonProperty("buttons")]
        public string Buttons { get; set; } = null;

        #endregion

        #region ListXML Fields

        /// <summary>
        /// Emulator source file related to the machine
        /// </summary>
        /// <remarks>Also in Logiqx</remarks>
        [JsonProperty("sourcefile")]
        public string SourceFile { get; set; } = null;

        /// <summary>
        /// Machine runnable status
        /// </summary>
        /// <remarks>yes = true, partial = null, no = false</remarks>
        /// <remarks>Also in Logiqx</remarks>
        [JsonProperty("runnable")]
        public bool? Runnable { get; set; } = null;

        /// <summary>
        /// List of associated device names
        /// </summary>
        [JsonProperty("devices")]
        public List<string> Devices { get; set; } = null;

        /// <summary>
        /// List of slot options
        /// </summary>
        [JsonProperty("slotoptions")]
        public List<string> SlotOptions { get; set; } = null;

        /// <summary>
        /// List of info items
        /// </summary>
        [JsonProperty("infos")]
        public List<KeyValuePair<string, string>> Infos { get; set; } = null;

        #endregion

        #region Logiqx Fields

        /// <summary>
        /// Machine board name
        /// </summary>
        [JsonProperty("board")]
        public string Board { get; set; } = null;

        /// <summary>
        /// Rebuild location if different than machine name
        /// </summary>
        [JsonProperty("rebuildto")]
        public string RebuildTo { get; set; } = null;

        #endregion

        #region Logiqx EmuArc Fields

        /// <summary>
        /// Title ID
        /// </summary>
        [JsonProperty("titleid")]
        public string TitleID { get; set; } = null;

        /// <summary>
        /// Machine developer
        /// </summary>
        [JsonProperty("developer")]
        public string Developer { get; set; } = null;

        /// <summary>
        /// Game genre
        /// </summary>
        [JsonProperty("genre")]
        public string Genre { get; set; } = null;

        /// <summary>
        /// Game subgenre
        /// </summary>
        [JsonProperty("genre")]
        public string Subgenre { get; set; } = null;

        /// <summary>
        /// Game ratings
        /// </summary>
        [JsonProperty("ratings")]
        public string Ratings { get; set; } = null;

        /// <summary>
        /// Game score
        /// </summary>
        [JsonProperty("score")]
        public string Score { get; set; } = null;

        /// <summary>
        /// Is the machine enabled
        /// </summary>
        [JsonProperty("enabled")]
        public string Enabled { get; set; } = null; // bool?

        /// <summary>
        /// Does the game have a CRC check
        /// </summary>
        [JsonProperty("hascrc")]
        public bool? HasCrc { get; set; } = null;

        /// <summary>
        /// Machine relations
        /// </summary>
        [JsonProperty("relatedto")]
        public string RelatedTo { get; set; } = null;

        #endregion

        #region SoftwareList Fields

        /// <summary>
        /// Support status
        /// </summary>
        /// <remarks>yes = true, partial = null, no = false</remarks>
        [JsonProperty("supported")]
        public bool? Supported { get; set; } = true;

        #endregion

        #endregion

        #region Accessors

        /// <summary>
        /// Get the value of that field as a string, if possible
        /// </summary>
        public string GetField(Field field, List<Field> excludeFields)
        {
            // If the field is to be excluded, return empty string
            if (excludeFields.Contains(field))
                return string.Empty;

            string fieldValue = null;
            switch (field)
            {
                #region Common

                case Field.MachineName:
                    fieldValue = Name;
                    break;
                case Field.Comment:
                    fieldValue = Comment;
                    break;
                case Field.Description:
                    fieldValue = Description;
                    break;
                case Field.Year:
                    fieldValue = Year;
                    break;
                case Field.Manufacturer:
                    fieldValue = Manufacturer;
                    break;
                case Field.Publisher:
                    fieldValue = Publisher;
                    break;
                case Field.Category:
                    fieldValue = Category;
                    break;
                case Field.RomOf:
                    fieldValue = RomOf;
                    break;
                case Field.CloneOf:
                    fieldValue = CloneOf;
                    break;
                case Field.SampleOf:
                    fieldValue = SampleOf;
                    break;
                case Field.MachineType:
                    fieldValue = MachineType.ToString();
                    break;

                #endregion

                #region AttractMode

                case Field.Players:
                    fieldValue = Players;
                    break;
                case Field.Rotation:
                    fieldValue = Rotation;
                    break;
                case Field.Control:
                    fieldValue = Control;
                    break;
                case Field.SupportStatus:
                    fieldValue = Status;
                    break;
                case Field.DisplayCount:
                    fieldValue = DisplayCount;
                    break;
                case Field.DisplayType:
                    fieldValue = DisplayType;
                    break;
                case Field.Buttons:
                    fieldValue = Buttons;
                    break;

                #endregion

                #region ListXML

                case Field.SourceFile:
                    fieldValue = SourceFile;
                    break;
                case Field.Runnable:
                    fieldValue = Runnable?.ToString();
                    break;
                case Field.Devices:
                    fieldValue = string.Join(";", Devices ?? new List<string>());
                    break;
                case Field.SlotOptions:
                    fieldValue = string.Join(";", SlotOptions ?? new List<string>());
                    break;
                case Field.Infos:
                    fieldValue = string.Join(";", (Infos ?? new List<KeyValuePair<string, string>>()).Select(i => $"{i.Key}={i.Value}"));
                    break;

                #endregion

                #region Logiqx

                case Field.Board:
                    fieldValue = Board;
                    break;
                case Field.RebuildTo:
                    fieldValue = RebuildTo;
                    break;

                #endregion

                #region Logiqx EmuArc

                case Field.TitleID:
                    fieldValue = TitleID;
                    break;
                case Field.Developer:
                    fieldValue = Developer;
                    break;
                case Field.Genre:
                    fieldValue = Genre;
                    break;
                case Field.Subgenre:
                    fieldValue = Subgenre;
                    break;
                case Field.Ratings:
                    fieldValue = Ratings;
                    break;
                case Field.Score:
                    fieldValue = Score;
                    break;
                case Field.Enabled:
                    fieldValue = Enabled;
                    break;
                case Field.HasCrc:
                    fieldValue = HasCrc.ToString();
                    break;
                case Field.RelatedTo:
                    fieldValue = RelatedTo;
                    break;

                #endregion

                #region SoftwareList

                case Field.Supported:
                    fieldValue = Supported?.ToString();
                    break;

                #endregion

                default:
                    return null;
            }

            // Make sure we don't return null
            if (string.IsNullOrEmpty(fieldValue))
                fieldValue = string.Empty;

            return fieldValue;
        }

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="mappings">Mappings dictionary</param>
        public void SetFields(Dictionary<Field, string> mappings)
        {
            #region Common

            if (mappings.Keys.Contains(Field.MachineName))
                Name = mappings[Field.MachineName];

            if (mappings.Keys.Contains(Field.Comment))
                Comment = mappings[Field.Comment];

            if (mappings.Keys.Contains(Field.Description))
                Description = mappings[Field.Description];

            if (mappings.Keys.Contains(Field.Year))
                Year = mappings[Field.Year];

            if (mappings.Keys.Contains(Field.Manufacturer))
                Manufacturer = mappings[Field.Manufacturer];

            if (mappings.Keys.Contains(Field.Publisher))
                Publisher = mappings[Field.Publisher];

            if (mappings.Keys.Contains(Field.Category))
                Category = mappings[Field.Category];

            if (mappings.Keys.Contains(Field.RomOf))
                RomOf = mappings[Field.RomOf];

            if (mappings.Keys.Contains(Field.CloneOf))
                CloneOf = mappings[Field.CloneOf];

            if (mappings.Keys.Contains(Field.SampleOf))
                SampleOf = mappings[Field.SampleOf];

            if (mappings.Keys.Contains(Field.MachineType))
                MachineType = mappings[Field.MachineType].AsMachineType();

            #endregion

            #region AttractMode

            if (mappings.Keys.Contains(Field.Players))
                Players = mappings[Field.Players];

            if (mappings.Keys.Contains(Field.Rotation))
                Rotation = mappings[Field.Rotation];

            if (mappings.Keys.Contains(Field.Control))
                Control = mappings[Field.Control];

            if (mappings.Keys.Contains(Field.SupportStatus))
                Status = mappings[Field.SupportStatus];

            if (mappings.Keys.Contains(Field.DisplayCount))
                DisplayCount = mappings[Field.DisplayCount];

            if (mappings.Keys.Contains(Field.DisplayType))
                DisplayType = mappings[Field.DisplayType];

            if (mappings.Keys.Contains(Field.Buttons))
                Buttons = mappings[Field.Buttons];

            #endregion

            #region ListXML

            if (mappings.Keys.Contains(Field.SourceFile))
                SourceFile = mappings[Field.SourceFile];

            if (mappings.Keys.Contains(Field.Runnable))
                Runnable = mappings[Field.Runnable].AsYesNo();

            if (mappings.Keys.Contains(Field.Devices))
            {
                if (Devices == null)
                    Devices = new List<string>();

                string[] devices = mappings[Field.Devices].Split(';');
                Devices.AddRange(devices);
            }

            if (mappings.Keys.Contains(Field.SlotOptions))
            {
                if (SlotOptions == null)
                    SlotOptions = new List<string>();

                string[] slotOptions = mappings[Field.SlotOptions].Split(';');
                SlotOptions.AddRange(slotOptions);
            }

            if (mappings.Keys.Contains(Field.Infos))
            {
                if (Infos == null)
                    Infos = new List<KeyValuePair<string, string>>();

                string[] pairs = mappings[Field.Infos].Split(';');
                foreach (string pair in pairs)
                {
                    string[] split = pair.Split('=');
                    Infos.Add(new KeyValuePair<string, string>(split[0], split[1]));
                }
            }

            #endregion

            #region Logiqx

            if (mappings.Keys.Contains(Field.Board))
                Board = mappings[Field.Board];

            if (mappings.Keys.Contains(Field.RebuildTo))
                RebuildTo = mappings[Field.RebuildTo];

            #endregion

            #region Logiqx EmuArc

            if (mappings.Keys.Contains(Field.TitleID))
                TitleID = mappings[Field.TitleID];

            if (mappings.Keys.Contains(Field.Developer))
                Developer = mappings[Field.Developer];

            if (mappings.Keys.Contains(Field.Genre))
                Genre = mappings[Field.Genre];

            if (mappings.Keys.Contains(Field.Subgenre))
                Subgenre = mappings[Field.Subgenre];

            if (mappings.Keys.Contains(Field.Ratings))
                Ratings = mappings[Field.Ratings];

            if (mappings.Keys.Contains(Field.Score))
                Score = mappings[Field.Score];

            if (mappings.Keys.Contains(Field.Enabled))
                Enabled = mappings[Field.Enabled];

            if (mappings.Keys.Contains(Field.HasCrc))
                HasCrc = mappings[Field.HasCrc].AsYesNo();

            if (mappings.Keys.Contains(Field.RelatedTo))
                RelatedTo = mappings[Field.RelatedTo];

            #endregion

            #region SoftwareList

            if (mappings.Keys.Contains(Field.Supported))
                Supported = mappings[Field.Supported].AsYesNo();

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
                Devices = this.Devices,
                SlotOptions = this.SlotOptions,
                Infos = this.Infos,
                MachineType = this.MachineType,

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
                HasCrc = this.HasCrc,
                RelatedTo = this.RelatedTo,

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

            // Filter on machine name
            bool? machineNameFound = filter.MachineName.MatchesPositiveSet(Name);
            if (filter.IncludeOfInGame)
            {
                machineNameFound |= (filter.MachineName.MatchesPositiveSet(CloneOf) == true);
                machineNameFound |= (filter.MachineName.MatchesPositiveSet(RomOf) == true);
            }
            if (machineNameFound == false)
                return false;

            machineNameFound = filter.MachineName.MatchesNegativeSet(Name);
            if (filter.IncludeOfInGame)
            {
                machineNameFound |= (filter.MachineName.MatchesNegativeSet(CloneOf) == true);
                machineNameFound |= (filter.MachineName.MatchesNegativeSet(RomOf) == true);
            }
            if (machineNameFound == false)
                return false;

            // Filter on comment
            if (filter.Comment.MatchesPositiveSet(Comment) == false)
                return false;
            if (filter.Comment.MatchesNegativeSet(Comment) == true)
                return false;

            // Filter on machine description
            if (filter.MachineDescription.MatchesPositiveSet(Description) == false)
                return false;
            if (filter.MachineDescription.MatchesNegativeSet(Description) == true)
                return false;

            // Filter on year
            if (filter.Year.MatchesPositiveSet(Year) == false)
                return false;
            if (filter.Year.MatchesNegativeSet(Year) == true)
                return false;

            // Filter on manufacturer
            if (filter.Manufacturer.MatchesPositiveSet(Manufacturer) == false)
                return false;
            if (filter.Manufacturer.MatchesNegativeSet(Manufacturer) == true)
                return false;

            // Filter on publisher
            if (filter.Publisher.MatchesPositiveSet(Publisher) == false)
                return false;
            if (filter.Publisher.MatchesNegativeSet(Publisher) == true)
                return false;

            // Filter on category
            if (filter.Category.MatchesPositiveSet(Category) == false)
                return false;
            if (filter.Category.MatchesNegativeSet(Category) == true)
                return false;

            // Filter on romof
            if (filter.RomOf.MatchesPositiveSet(RomOf) == false)
                return false;
            if (filter.RomOf.MatchesNegativeSet(RomOf) == true)
                return false;

            // Filter on cloneof
            if (filter.CloneOf.MatchesPositiveSet(CloneOf) == false)
                return false;
            if (filter.CloneOf.MatchesNegativeSet(CloneOf) == true)
                return false;

            // Filter on sampleof
            if (filter.SampleOf.MatchesPositiveSet(SampleOf) == false)
                return false;
            if (filter.SampleOf.MatchesNegativeSet(SampleOf) == true)
                return false;

            #endregion

            #region AttractMode

            // Filter on players
            if (filter.Players.MatchesPositiveSet(Players) == false)
                return false;
            if (filter.Players.MatchesNegativeSet(Players) == true)
                return false;

            // Filter on rotation
            if (filter.Rotation.MatchesPositiveSet(Rotation) == false)
                return false;
            if (filter.Rotation.MatchesNegativeSet(Rotation) == true)
                return false;

            // Filter on control
            if (filter.Control.MatchesPositiveSet(Control) == false)
                return false;
            if (filter.Control.MatchesNegativeSet(Control) == true)
                return false;

            // Filter on support status
            if (filter.SupportStatus.MatchesPositiveSet(Status) == false)
                return false;
            if (filter.SupportStatus.MatchesNegativeSet(Status) == true)
                return false;

            // Filter on display count
            if (filter.DisplayCount.MatchesPositiveSet(DisplayCount) == false)
                return false;
            if (filter.DisplayCount.MatchesNegativeSet(DisplayCount) == true)
                return false;

            // Filter on display type
            if (filter.DisplayType.MatchesPositiveSet(DisplayType) == false)
                return false;
            if (filter.DisplayType.MatchesNegativeSet(DisplayType) == true)
                return false;

            // Filter on buttons
            if (filter.Buttons.MatchesPositiveSet(Buttons) == false)
                return false;
            if (filter.Buttons.MatchesNegativeSet(Buttons) == true)
                return false;

            #endregion

            #region ListXML

            // Filter on source file
            if (filter.SourceFile.MatchesPositiveSet(SourceFile) == false)
                return false;
            if (filter.SourceFile.MatchesNegativeSet(SourceFile) == true)
                return false;

            // Filter on runnable
            if (filter.Runnable.MatchesNeutral(null, Runnable) == false)
                return false;

            // Filter on devices
            if (Devices != null && Devices.Any())
            {
                bool anyPositiveDevice = false;
                bool anyNegativeDevice = false;
                foreach (string device in Devices)
                {
                    anyPositiveDevice |= filter.Devices.MatchesPositiveSet(device) != false;
                    anyNegativeDevice |= filter.Devices.MatchesNegativeSet(device) == false;
                }

                if (!anyPositiveDevice || anyNegativeDevice)
                    return false;
            }

            // Filter on slot options
            if (SlotOptions != null && SlotOptions.Any())
            {
                bool anyPositiveSlotOption = false;
                bool anyNegativeSlotOption = false;
                foreach (string slotOption in SlotOptions)
                {
                    anyPositiveSlotOption |= filter.SlotOptions.MatchesPositiveSet(slotOption) != false;
                    anyNegativeSlotOption |= filter.SlotOptions.MatchesNegativeSet(slotOption) == false;
                }

                if (!anyPositiveSlotOption || anyNegativeSlotOption)
                    return false;
            }

            // Filter on machine type
            if (filter.MachineTypes.MatchesPositive(MachineType.NULL, MachineType) == false)
                return false;
            if (filter.MachineTypes.MatchesNegative(MachineType.NULL, MachineType) == true)
                return false;

            #endregion

            #region Logiqx

            // Filter on board
            if (filter.Board.MatchesPositiveSet(Board) == false)
                return false;
            if (filter.Board.MatchesNegativeSet(Board) == true)
                return false;

            // Filter on rebuildto
            if (filter.RebuildTo.MatchesPositiveSet(RebuildTo) == false)
                return false;
            if (filter.RebuildTo.MatchesNegativeSet(RebuildTo) == true)
                return false;

            #endregion

            #region Logiqx EmuArc

            // Filter on title ID
            if (filter.TitleID.MatchesPositiveSet(TitleID) == false)
                return false;
            if (filter.TitleID.MatchesNegativeSet(TitleID) == true)
                return false;

            // Filter on developer
            if (filter.Developer.MatchesPositiveSet(Developer) == false)
                return false;
            if (filter.Developer.MatchesNegativeSet(Developer) == true)
                return false;

            // Filter on genre
            if (filter.Genre.MatchesPositiveSet(Genre) == false)
                return false;
            if (filter.Genre.MatchesNegativeSet(Genre) == true)
                return false;

            // Filter on rebuildto
            if (filter.Subgenre.MatchesPositiveSet(Subgenre) == false)
                return false;
            if (filter.Subgenre.MatchesNegativeSet(Subgenre) == true)
                return false;

            // Filter on subgenre
            if (filter.Ratings.MatchesPositiveSet(Ratings) == false)
                return false;
            if (filter.Ratings.MatchesNegativeSet(Ratings) == true)
                return false;

            // Filter on score
            if (filter.Score.MatchesPositiveSet(Score) == false)
                return false;
            if (filter.Score.MatchesNegativeSet(Score) == true)
                return false;

            // Filter on enabled
            if (filter.Enabled.MatchesPositiveSet(Enabled) == false)
                return false;
            if (filter.Enabled.MatchesNegativeSet(Enabled) == true)
                return false;

            // Filter on has CRC flag
            if (filter.HasCrc.MatchesNeutral(null, HasCrc) == false)
                return false;

            // Filter on related to
            if (filter.RelatedTo.MatchesPositiveSet(RelatedTo) == false)
                return false;
            if (filter.RelatedTo.MatchesNegativeSet(RelatedTo) == true)
                return false;

            #endregion

            #region SoftwareList

            // Filter on supported
            if (filter.Supported.MatchesNeutral(null, Supported) == false)
                return false;

            #endregion

            return true;
        }

        /// <summary>
        /// Remove fields from the Machine
        /// </summary>
        /// <param name="fields">List of Fields to remove</param>
        public void RemoveFields(List<Field> fields)
        {
            #region Common

            if (fields.Contains(Field.MachineName))
                Name = null;

            if (fields.Contains(Field.Comment))
                Comment = null;

            if (fields.Contains(Field.Description))
                Description = null;

            if (fields.Contains(Field.Year))
                Year = null;

            if (fields.Contains(Field.Manufacturer))
                Manufacturer = null;

            if (fields.Contains(Field.Publisher))
                Publisher = null;

            if (fields.Contains(Field.Category))
                Category = null;

            if (fields.Contains(Field.RomOf))
                RomOf = null;

            if (fields.Contains(Field.CloneOf))
                CloneOf = null;

            if (fields.Contains(Field.SampleOf))
                SampleOf = null;

            if (fields.Contains(Field.MachineType))
                MachineType = MachineType.NULL;

            #endregion

            #region AttractMode

            if (fields.Contains(Field.Players))
                Players = null;

            if (fields.Contains(Field.Rotation))
                Rotation = null;

            if (fields.Contains(Field.Control))
                Control = null;

            if (fields.Contains(Field.SupportStatus))
                Status = null;

            if (fields.Contains(Field.DisplayCount))
                DisplayCount = null;

            if (fields.Contains(Field.DisplayType))
                DisplayType = null;

            if (fields.Contains(Field.Buttons))
                Buttons = null;

            #endregion

            #region ListXML

            if (fields.Contains(Field.SourceFile))
                SourceFile = null;

            if (fields.Contains(Field.Runnable))
                Runnable = null;

            if (fields.Contains(Field.Devices))
                Devices = null;

            if (fields.Contains(Field.SlotOptions))
                SlotOptions = null;

            if (fields.Contains(Field.Infos))
                Infos = null;

            #endregion

            #region Logiqx

            if (fields.Contains(Field.Board))
                Board = null;

            if (fields.Contains(Field.RebuildTo))
                RebuildTo = null;

            #endregion

            #region Logiqx EmuArc

            if (fields.Contains(Field.TitleID))
                TitleID = null;

            if (fields.Contains(Field.Developer))
                Developer = null;

            if (fields.Contains(Field.Genre))
                Genre = null;

            if (fields.Contains(Field.Subgenre))
                Subgenre = null;

            if (fields.Contains(Field.Ratings))
                Ratings = null;

            if (fields.Contains(Field.Score))
                Score = null;

            if (fields.Contains(Field.Enabled))
                Enabled = null;

            if (fields.Contains(Field.HasCrc))
                HasCrc = null;

            if (fields.Contains(Field.RelatedTo))
                RelatedTo = null;

            #endregion

            #region SoftwareList

            if (fields.Contains(Field.Supported))
                Supported = null;

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

            if (fields.Contains(Field.MachineName))
                Name = machine.Name;

            if (fields.Contains(Field.Comment))
                Comment = machine.Comment;

            if (fields.Contains(Field.Description))
            {
                if (!onlySame || (onlySame && Name == Description))
                    Description = machine.Description;
            }

            if (fields.Contains(Field.Year))
                Year = machine.Year;

            if (fields.Contains(Field.Manufacturer))
                Manufacturer = machine.Manufacturer;

            if (fields.Contains(Field.Publisher))
                Publisher = machine.Publisher;

            if (fields.Contains(Field.Category))
                Category = machine.Category;

            if (fields.Contains(Field.RomOf))
                RomOf = machine.RomOf;

            if (fields.Contains(Field.CloneOf))
                CloneOf = machine.CloneOf;

            if (fields.Contains(Field.SampleOf))
                SampleOf = machine.SampleOf;

            if (fields.Contains(Field.MachineType))
                MachineType = machine.MachineType;

            #endregion

            #region AttractMode

            if (fields.Contains(Field.Players))
                Players = machine.Players;

            if (fields.Contains(Field.Rotation))
                Rotation = machine.Rotation;

            if (fields.Contains(Field.Control))
                Control = machine.Control;

            if (fields.Contains(Field.SupportStatus))
                Status = machine.Status;

            if (fields.Contains(Field.DisplayCount))
                DisplayCount = machine.DisplayCount;

            if (fields.Contains(Field.DisplayType))
                DisplayType = machine.DisplayType;

            if (fields.Contains(Field.Buttons))
                Buttons = machine.Buttons;

            #endregion

            #region ListXML

            if (fields.Contains(Field.SourceFile))
                SourceFile = machine.SourceFile;

            if (fields.Contains(Field.Runnable))
                Runnable = machine.Runnable;

            if (fields.Contains(Field.Devices))
                Devices = machine.Devices;

            if (fields.Contains(Field.SlotOptions))
                SlotOptions = machine.SlotOptions;

            if (fields.Contains(Field.Infos))
                Infos = machine.Infos;

            #endregion

            #region Logiqx

            if (fields.Contains(Field.Board))
                Board = machine.Board;

            if (fields.Contains(Field.RebuildTo))
                RebuildTo = machine.RebuildTo;

            #endregion

            #region Logiqx EmuArc

            if (fields.Contains(Field.TitleID))
                TitleID = machine.TitleID;

            if (fields.Contains(Field.Developer))
                Developer = machine.Developer;

            if (fields.Contains(Field.Genre))
                Genre = machine.Genre;

            if (fields.Contains(Field.Subgenre))
                Subgenre = machine.Subgenre;

            if (fields.Contains(Field.Ratings))
                Ratings = machine.Ratings;

            if (fields.Contains(Field.Score))
                Score = machine.Score;

            if (fields.Contains(Field.Enabled))
                Enabled = machine.Enabled;

            if (fields.Contains(Field.HasCrc))
                HasCrc = machine.HasCrc;

            if (fields.Contains(Field.RelatedTo))
                RelatedTo = machine.RelatedTo;

            #endregion

            #region SoftwareList

            if (fields.Contains(Field.Supported))
                Supported = machine.Supported;

            #endregion
        }

        #endregion
    }
}
