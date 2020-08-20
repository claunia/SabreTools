using System;
using System.Collections.Generic;
using System.Linq;

using SabreTools.Library.Filtering;
using Newtonsoft.Json;

namespace SabreTools.Library.DatItems
{
    /// <summary>
    /// Represents the information specific to a set/game/machine
    /// </summary>
    public class Machine : ICloneable
    {
        #region Fields

        /// <summary>
        /// Name of the machine
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Additional notes
        /// </summary>
        [JsonProperty("comment")]
        public string Comment { get; set; }

        /// <summary>
        /// Extended description
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Year(s) of release/manufacture
        /// </summary>
        [JsonProperty("year")]
        public string Year { get; set; }

        /// <summary>
        /// Manufacturer, if available
        /// </summary>
        [JsonProperty("manufacturer")]
        public string Manufacturer { get; set; }

        /// <summary>
        /// Publisher, if available
        /// </summary>
        [JsonProperty("publisher")]
        public string Publisher { get; set; }

        /// <summary>
        /// Category, if available
        /// </summary>
        [JsonProperty("category")]
        public string Category { get; set; }

        /// <summary>
        /// fomof parent
        /// </summary>
        [JsonProperty("romof")]
        public string RomOf { get; set; }

        /// <summary>
        /// cloneof parent
        /// </summary>
        [JsonProperty("cloneof")]
        public string CloneOf { get; set; }

        /// <summary>
        /// sampleof parent
        /// </summary>
        [JsonProperty("sampleof")]
        public string SampleOf { get; set; }

        /// <summary>
        /// Support status
        /// </summary>
        /// <remarks>yes = true, partial = null, no = false</remarks>
        [JsonProperty("supported")]
        public bool? Supported { get; set; }

        /// <summary>
        /// Emulator source file related to the machine
        /// </summary>
        [JsonProperty("sourcefile")]
        public string SourceFile { get; set; }

        /// <summary>
        /// Machine runnable status
        /// </summary>
        /// <remarks>yes = true, partial = null, no = false</remarks>
        [JsonProperty("runnable")]
        public bool? Runnable { get; set; }

        /// <summary>
        /// Machine board name
        /// </summary>
        [JsonProperty("board")]
        public string Board { get; set; }

        /// <summary>
        /// Rebuild location if different than machine name
        /// </summary>
        [JsonProperty("rebuildto")]
        public string RebuildTo { get; set; }

        /// <summary>
        /// List of associated device names
        /// </summary>
        [JsonProperty("devices")]
        public List<string> Devices { get; set; }

        /// <summary>
        /// List of slot options
        /// </summary>
        [JsonProperty("slotoptions")]
        public List<string> SlotOptions { get; set; }

        /// <summary>
        /// List of info items
        /// </summary>
        [JsonProperty("infos")]
        public List<KeyValuePair<string, string>> Infos { get; set; }

        /// <summary>
        /// Type of the machine
        /// </summary>
        [JsonProperty("type")]
        public MachineType MachineType { get; set; }

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
                case Field.Supported:
                    fieldValue = Supported?.ToString();
                    break;
                case Field.SourceFile:
                    fieldValue = SourceFile;
                    break;
                case Field.Runnable:
                    fieldValue = Runnable?.ToString();
                    break;
                case Field.Board:
                    fieldValue = Board;
                    break;
                case Field.RebuildTo:
                    fieldValue = RebuildTo;
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
                case Field.MachineType:
                    fieldValue = MachineType.ToString();
                    break;

                default:
                    return null;
            }

            // Make sure we don't return null
            if (string.IsNullOrEmpty(fieldValue))
                fieldValue = string.Empty;

            return fieldValue;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new Machine object
        /// </summary>
        public Machine()
        {
            Name = null;
            Comment = null;
            Description = null;
            Year = null;
            Manufacturer = null;
            Publisher = null;
            Category = null;
            RomOf = null;
            CloneOf = null;
            SampleOf = null;
            Supported = true;
            SourceFile = null;
            Runnable = null;
            Board = null;
            RebuildTo = null;
            Devices = null;
            SlotOptions = null;
            Infos = null;
            MachineType = MachineType.NULL;
        }

        /// <summary>
        /// Create a new Machine object with the included information
        /// </summary>
        /// <param name="name">Name of the machine</param>
        /// <param name="description">Description of the machine</param>
        public Machine(string name, string description)
        {
            Name = name;
            Comment = null;
            Description = description;
            Year = null;
            Manufacturer = null;
            Publisher = null;
            Category = null;
            RomOf = null;
            CloneOf = null;
            SampleOf = null;
            Supported = true;
            SourceFile = null;
            Runnable = null;
            Board = null;
            RebuildTo = null;
            Devices = null;
            SlotOptions = null;
            Infos = null;
            MachineType = MachineType.NULL;
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
                Supported = this.Supported,
                SourceFile = this.SourceFile,
                Runnable = this.Runnable,
                Board = this.Board,
                RebuildTo = this.RebuildTo,
                Devices = this.Devices,
                SlotOptions = this.SlotOptions,
                Infos = this.Infos,
                MachineType = this.MachineType,
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

            // Filter on supported
            if (filter.Supported.MatchesNeutral(null, Supported) == false)
                return false;

            // Filter on source file
            if (filter.SourceFile.MatchesPositiveSet(SourceFile) == false)
                return false;
            if (filter.SourceFile.MatchesNegativeSet(SourceFile) == true)
                return false;

            // Filter on runnable
            if (filter.Runnable.MatchesNeutral(null, Runnable) == false)
                return false;

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

            return true;
        }

        /// <summary>
        /// Remove fields from the Machine
        /// </summary>
        /// <param name="fields">List of Fields to remove</param>
        public void RemoveFields(List<Field> fields)
        {
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

            if (fields.Contains(Field.Supported))
                Supported = null;

            if (fields.Contains(Field.SourceFile))
                SourceFile = null;

            if (fields.Contains(Field.Runnable))
                Runnable = null;

            if (fields.Contains(Field.Board))
                Board = null;

            if (fields.Contains(Field.RebuildTo))
                RebuildTo = null;

            if (fields.Contains(Field.Devices))
                Devices = null;

            if (fields.Contains(Field.SlotOptions))
                SlotOptions = null;

            if (fields.Contains(Field.Infos))
                Infos = null;

            if (fields.Contains(Field.MachineType))
                MachineType = MachineType.NULL;
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

            if (fields.Contains(Field.Supported))
                Supported = machine.Supported;

            if (fields.Contains(Field.SourceFile))
                SourceFile = machine.SourceFile;

            if (fields.Contains(Field.Runnable))
                Runnable = machine.Runnable;

            if (fields.Contains(Field.Board))
                Board = machine.Board;

            if (fields.Contains(Field.RebuildTo))
                RebuildTo = machine.RebuildTo;

            if (fields.Contains(Field.Devices))
                Devices = machine.Devices;

            if (fields.Contains(Field.SlotOptions))
                SlotOptions = machine.SlotOptions;

            if (fields.Contains(Field.Infos))
                Infos = machine.Infos;

            if (fields.Contains(Field.MachineType))
                MachineType = machine.MachineType;
        }

        #endregion
    }
}
