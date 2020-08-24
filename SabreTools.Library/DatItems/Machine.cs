using System;
using System.Collections.Generic;
using System.Linq;

using SabreTools.Library.Filtering;
using SabreTools.Library.Tools;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SabreTools.Library.DatItems
{
    /// <summary>
    /// Represents the information specific to a set/game/machine
    /// </summary>
    [JsonObject("machine")]
    public class Machine : ICloneable
    {
        #region Fields

        #region Common Fields

        /// <summary>
        /// Name of the machine
        /// </summary>
        [JsonProperty("name", DefaultValueHandling = DefaultValueHandling.Include)]
        public string Name { get; set; } = null;

        /// <summary>
        /// Additional notes
        /// </summary>
        /// <remarks>Known as "Extra" in AttractMode</remarks>
        [JsonProperty("comment", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Comment { get; set; } = null;

        /// <summary>
        /// Extended description
        /// </summary>
        [JsonProperty("description", DefaultValueHandling = DefaultValueHandling.Include)]
        public string Description { get; set; } = null;

        /// <summary>
        /// Year(s) of release/manufacture
        /// </summary>
        [JsonProperty("year", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Year { get; set; } = null;

        /// <summary>
        /// Manufacturer, if available
        /// </summary>
        [JsonProperty("manufacturer", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Manufacturer { get; set; } = null;

        /// <summary>
        /// Publisher, if available
        /// </summary>
        [JsonProperty("publisher", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Publisher { get; set; } = null;

        /// <summary>
        /// Category, if available
        /// </summary>
        [JsonProperty("category", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Category { get; set; } = null;

        /// <summary>
        /// fomof parent
        /// </summary>
        [JsonProperty("romof", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string RomOf { get; set; } = null;

        /// <summary>
        /// cloneof parent
        /// </summary>
        [JsonProperty("cloneof", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string CloneOf { get; set; } = null;

        /// <summary>
        /// sampleof parent
        /// </summary>
        [JsonProperty("sampleof", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string SampleOf { get; set; } = null;

        /// <summary>
        /// Type of the machine
        /// </summary>
        [JsonProperty("type", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonConverter(typeof(StringEnumConverter))]
        public MachineType MachineType { get; set; } = MachineType.NULL;

        #endregion

        #region AttractMode Fields

        /// <summary>
        /// Player count
        /// </summary>
        /// <remarks>Also in Logiqx EmuArc</remarks>
        [JsonProperty("players", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Players { get; set; } = null;

        /// <summary>
        /// Screen rotation
        /// </summary>
        [JsonProperty("rotation", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Rotation { get; set; } = null;

        /// <summary>
        /// Control method
        /// </summary>
        [JsonProperty("control", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Control { get; set; } = null;

        /// <summary>
        /// Support status
        /// </summary>
        [JsonProperty("status", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Status { get; set; } = null;

        /// <summary>
        /// Display count
        /// </summary>
        [JsonProperty("displaycount", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string DisplayCount { get; set; } = null;

        /// <summary>
        /// Display type
        /// </summary>
        [JsonProperty("displaytype", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string DisplayType { get; set; } = null;

        /// <summary>
        /// Number of input buttons
        /// </summary>
        [JsonProperty("buttons", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Buttons { get; set; } = null;

        #endregion

        #region ListXML Fields

        /// <summary>
        /// Emulator source file related to the machine
        /// </summary>
        /// <remarks>Also in Logiqx</remarks>
        [JsonProperty("sourcefile", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string SourceFile { get; set; } = null;

        /// <summary>
        /// Machine runnable status
        /// </summary>
        /// <remarks>Also in Logiqx</remarks>
        [JsonProperty("runnable", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Runnable Runnable { get; set; } = Runnable.NULL;

        /// <summary>
        /// List of associated device names
        /// </summary>
        [JsonProperty("devices", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<ListXmlDeviceReference> DeviceReferences { get; set; } = null;

        /// <summary>
        /// List of associated chips
        /// </summary>
        [JsonProperty("chips", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<ListXmlChip> Chips { get; set; } = null;

        /// <summary>
        /// List of associated displays
        /// </summary>
        [JsonProperty("displays", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<ListXmlDisplay> Displays { get; set; } = null;

        /// <summary>
        /// List of associated sounds
        /// </summary>
        [JsonProperty("sounds", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<ListXmlSound> Sounds { get; set; } = null;

        /// <summary>
        /// List of associated conditions
        /// </summary>
        [JsonProperty("conditions", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<ListXmlCondition> Conditions { get; set; } = null;

        /// <summary>
        /// List of associated inputs
        /// </summary>
        [JsonProperty("inputs", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<ListXmlInput> Inputs { get; set; } = null;

        /// <summary>
        /// List of associated dipswitches
        /// </summary>
        /// <remarks>Also in SoftwareList</remarks>
        /// TODO: Order ListXML and SoftwareList outputs by area names
        [JsonProperty("dipswitches", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<ListXmlDipSwitch> DipSwitches { get; set; } = null;

        /// <summary>
        /// List of associated configurations
        /// </summary>
        [JsonProperty("configurations", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<ListXmlConfiguration> Configurations { get; set; } = null;

        /// <summary>
        /// List of slot options
        /// </summary>
        [JsonProperty("slots", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<ListXmlSlot> Slots { get; set; } = null;

        /// <summary>
        /// List of info items
        /// </summary>
        /// <remarks>Also in SoftwareList</remarks>
        [JsonProperty("infos", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<ListXmlInfo> Infos { get; set; } = null;

        #endregion

        #region Logiqx Fields

        /// <summary>
        /// Machine board name
        /// </summary>
        [JsonProperty("board", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Board { get; set; } = null;

        /// <summary>
        /// Rebuild location if different than machine name
        /// </summary>
        [JsonProperty("rebuildto", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string RebuildTo { get; set; } = null;

        #endregion

        #region Logiqx EmuArc Fields

        /// <summary>
        /// Title ID
        /// </summary>
        [JsonProperty("titleid", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string TitleID { get; set; } = null;

        /// <summary>
        /// Machine developer
        /// </summary>
        [JsonProperty("developer", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Developer { get; set; } = null;

        /// <summary>
        /// Game genre
        /// </summary>
        [JsonProperty("genre", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Genre { get; set; } = null;

        /// <summary>
        /// Game subgenre
        /// </summary>
        [JsonProperty("subgenre", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Subgenre { get; set; } = null;

        /// <summary>
        /// Game ratings
        /// </summary>
        [JsonProperty("ratings", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Ratings { get; set; } = null;

        /// <summary>
        /// Game score
        /// </summary>
        [JsonProperty("score", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Score { get; set; } = null;

        /// <summary>
        /// Is the machine enabled
        /// </summary>
        [JsonProperty("enabled", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Enabled { get; set; } = null; // bool?

        /// <summary>
        /// Does the game have a CRC check
        /// </summary>
        [JsonProperty("hascrc", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool? HasCrc { get; set; } = null;

        /// <summary>
        /// Machine relations
        /// </summary>
        [JsonProperty("relatedto", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string RelatedTo { get; set; } = null;

        #endregion

        #region OpenMSX Fields

        /// <summary>
        /// Generation MSX ID
        /// </summary>
        [JsonProperty("genmsxid", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string GenMSXID { get; set; } = null;

        /// <summary>
        /// MSX System
        /// </summary>
        [JsonProperty("system", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string System { get; set; } = null;

        /// <summary>
        /// Machine country of origin
        /// </summary>
        [JsonProperty("country", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Country { get; set; } = null;

        #endregion

        #region SoftwareList Fields

        /// <summary>
        /// Support status
        /// </summary>
        [JsonProperty("supported", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Supported Supported { get; set; } = Supported.NULL;

        /// <summary>
        /// List of shared feature items
        /// </summary>
        [JsonProperty("sharedfeat", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<SoftwareListSharedFeature> SharedFeatures { get; set; } = null;

        #endregion

        #endregion

        #region Accessors

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
                Runnable = mappings[Field.Runnable].AsRunnable();

            if (mappings.Keys.Contains(Field.DeviceReferences))
            {
                if (DeviceReferences == null)
                    DeviceReferences = new List<ListXmlDeviceReference>();

                var devices = mappings[Field.DeviceReferences].Split(';').Select(d => new ListXmlDeviceReference() { Name = d, });
                DeviceReferences.AddRange(devices);
            }

            // TODO: Add Field.Slot

            if (mappings.Keys.Contains(Field.Infos))
            {
                if (Infos == null)
                    Infos = new List<ListXmlInfo>();

                string[] pairs = mappings[Field.Infos].Split(';');
                foreach (string pair in pairs)
                {
                    string[] split = pair.Split('=');

                    var infoObj = new ListXmlInfo();
                    infoObj.Name = split[0];
                    infoObj.Value = split[1];

                    Infos.Add(infoObj);
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

            #region OpenMSX

            if (mappings.Keys.Contains(Field.GenMSXID))
                GenMSXID = mappings[Field.GenMSXID];

            if (mappings.Keys.Contains(Field.System))
                System = mappings[Field.System];

            if (mappings.Keys.Contains(Field.Country))
                Country = mappings[Field.Country];

            #endregion

            #region SoftwareList

            if (mappings.Keys.Contains(Field.Supported))
                Supported = mappings[Field.Supported].AsSupported();

            if (mappings.Keys.Contains(Field.SharedFeatures))
            {
                if (SharedFeatures == null)
                    SharedFeatures = new List<SoftwareListSharedFeature>();

                string[] pairs = mappings[Field.SharedFeatures].Split(';');
                foreach (string pair in pairs)
                {
                    string[] split = pair.Split('=');

                    var sharedFeature = new SoftwareListSharedFeature();
                    sharedFeature.Name = split[0];
                    sharedFeature.Value = split[1];

                    SharedFeatures.Add(sharedFeature);
                }
            }

            if (mappings.Keys.Contains(Field.DipSwitches))
            {
                if (DipSwitches == null)
                    DipSwitches = new List<ListXmlDipSwitch>();

                // TODO: There's no way this will work... just create the new list for now
            }

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
                DeviceReferences = this.DeviceReferences,
                Slots = this.Slots,
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

                #region OpenMSX

                GenMSXID = this.GenMSXID,
                System = this.System,
                Country = this.Country,

                #endregion

                #region SoftwareList

                Supported = this.Supported,
                SharedFeatures = this.SharedFeatures,
                DipSwitches = this.DipSwitches,

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
            if (filter.Runnables.MatchesPositive(Runnable.NULL, Runnable) == false)
                return false;
            if (filter.Runnables.MatchesNegative(Runnable.NULL, Runnable) == true)
                return false;

            // Filter on devices
            if (DeviceReferences != null && DeviceReferences.Any())
            {
                bool anyPositiveDevice = false;
                bool anyNegativeDevice = false;
                foreach (ListXmlDeviceReference device in DeviceReferences)
                {
                    anyPositiveDevice |= filter.Devices.MatchesPositiveSet(device.Name) != false;
                    anyNegativeDevice |= filter.Devices.MatchesNegativeSet(device.Name) == false;
                }

                if (!anyPositiveDevice || anyNegativeDevice)
                    return false;
            }

            // TODO: Add Slot filter

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

            #region OpenMSX

            // Filter on Generation MSX ID
            if (filter.GenMSXID.MatchesPositiveSet(GenMSXID) == false)
                return false;
            if (filter.GenMSXID.MatchesNegativeSet(GenMSXID) == true)
                return false;

            // Filter on system
            if (filter.System.MatchesPositiveSet(System) == false)
                return false;
            if (filter.System.MatchesNegativeSet(System) == true)
                return false;

            // Filter on country
            if (filter.Country.MatchesPositiveSet(Country) == false)
                return false;
            if (filter.Country.MatchesNegativeSet(Country) == true)
                return false;

            #endregion

            #region SoftwareList

            // Filter on supported
            if (filter.SupportedStatus.MatchesPositive(Supported.NULL, Supported) == false)
                return false;
            if (filter.SupportedStatus.MatchesNegative(Supported.NULL, Supported) == true)
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
                Runnable = Runnable.NULL;

            if (fields.Contains(Field.DeviceReferences))
                DeviceReferences = null;

            if (fields.Contains(Field.Slots))
                Slots = null;

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

            #region OpenMSX

            if (fields.Contains(Field.GenMSXID))
                GenMSXID = null;

            if (fields.Contains(Field.System))
                System = null;

            if (fields.Contains(Field.Country))
                Country = null;

            #endregion

            #region SoftwareList

            if (fields.Contains(Field.Supported))
                Supported = Supported.NULL;

            if (fields.Contains(Field.SharedFeatures))
                SharedFeatures = null;

            if (fields.Contains(Field.DipSwitches))
                DipSwitches = null;

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

            if (fields.Contains(Field.DeviceReferences))
                DeviceReferences = machine.DeviceReferences;

            if (fields.Contains(Field.Slots))
                Slots = machine.Slots;

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

            #region OpenMSX

            if (fields.Contains(Field.GenMSXID))
                GenMSXID = machine.GenMSXID;

            if (fields.Contains(Field.System))
                System = machine.System;

            if (fields.Contains(Field.Country))
                Country = machine.Country;

            #endregion

            #region SoftwareList

            if (fields.Contains(Field.Supported))
                Supported = machine.Supported;

            if (fields.Contains(Field.SharedFeatures))
                SharedFeatures = machine.SharedFeatures;

            if (fields.Contains(Field.DipSwitches))
                DipSwitches = machine.DipSwitches;

            #endregion
        }

        #endregion
    }
}
