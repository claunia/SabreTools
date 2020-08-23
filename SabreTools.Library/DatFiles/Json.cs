using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using SabreTools.Library.Data;
using SabreTools.Library.DatItems;
using SabreTools.Library.IO;
using SabreTools.Library.Tools;
using Newtonsoft.Json;

namespace SabreTools.Library.DatFiles
{
    /// <summary>
    /// Represents parsing and writing of a JSON DAT
    /// </summary>
    internal class Json : DatFile
    {
        /// <summary>
        /// Constructor designed for casting a base DatFile
        /// </summary>
        /// <param name="datFile">Parent DatFile to copy from</param>
        public Json(DatFile datFile)
            : base(datFile)
        {
        }

        /// <summary>
        /// Parse a Logiqx XML DAT and return all found games and roms within
        /// </summary>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="keep">True if full pathnames are to be kept, false otherwise (default)</param>
        protected override void ParseFile(
            // Standard Dat parsing
            string filename,
            int indexId,

            // Miscellaneous
            bool keep)
        {
            // Prepare all internal variables
            StreamReader sr = new StreamReader(FileExtensions.TryOpenRead(filename), new UTF8Encoding(false));
            JsonTextReader jtr = new JsonTextReader(sr);

            // If we got a null reader, just return
            if (jtr == null)
                return;

            // Otherwise, read the file to the end
            try
            {
                jtr.Read();
                while (!sr.EndOfStream)
                {
                    // Skip everything not a property name
                    if (jtr.TokenType != JsonToken.PropertyName)
                    {
                        jtr.Read();
                        continue;
                    }

                    switch (jtr.Value)
                    {
                        // Header value
                        case "header":
                            ReadHeader(sr, jtr, keep);
                            jtr.Read();
                            break;

                        // Machine array
                        case "machines":
                            ReadMachines(sr, jtr, filename, indexId);
                            jtr.Read();
                            break;

                        default:
                            jtr.Read();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Globals.Logger.Warning($"Exception found while parsing '{filename}': {ex}");
            }

            jtr.Close();
        }

        /// <summary>
        /// Read header information
        /// </summary>
        /// <param name="sr">StreamReader to use to parse the header</param>
        /// <param name="jtr">JsonTextReader to use to parse the header</param>
        /// <param name="keep">True if full pathnames are to be kept, false otherwise (default)</param>
        private void ReadHeader(StreamReader sr, JsonTextReader jtr, bool keep)
        {
            bool superdat = false;

            // If the reader is invalid, skip
            if (jtr == null)
                return;

            jtr.Read();
            while (!sr.EndOfStream)
            {
                // If we hit the end of the header, return
                if (jtr.TokenType == JsonToken.EndObject)
                    return;

                // We don't care about anything except property names
                if (jtr.TokenType != JsonToken.PropertyName)
                {
                    jtr.Read();
                    continue;
                }

                // Get all header items (ONLY OVERWRITE IF THERE'S NO DATA)
                string content;
                switch (jtr.Value)
                {
                    #region Common

                    case "name":
                        content = jtr.ReadAsString();
                        Header.Name = (Header.Name == null ? content : Header.Name);
                        superdat = superdat || content.Contains(" - SuperDAT");
                        if (keep && superdat)
                        {
                            Header.Type = (Header.Type == null ? "SuperDAT" : Header.Type);
                        }
                        break;

                    case "description":
                        content = jtr.ReadAsString();
                        Header.Description = (Header.Description == null ? content : Header.Description);
                        break;

                    case "rootdir": // This is exclusive to TruRip XML
                        content = jtr.ReadAsString();
                        Header.RootDir = (Header.RootDir == null ? content : Header.RootDir);
                        break;

                    case "category":
                        content = jtr.ReadAsString();
                        Header.Category = (Header.Category == null ? content : Header.Category);
                        break;

                    case "version":
                        content = jtr.ReadAsString();
                        Header.Version = (Header.Version == null ? content : Header.Version);
                        break;

                    case "date":
                        content = jtr.ReadAsString();
                        Header.Date = (Header.Date == null ? content.Replace(".", "/") : Header.Date);
                        break;

                    case "author":
                        content = jtr.ReadAsString();
                        Header.Author = (Header.Author == null ? content : Header.Author);
                        break;

                    case "email":
                        content = jtr.ReadAsString();
                        Header.Email = (Header.Email == null ? content : Header.Email);
                        break;

                    case "homepage":
                        content = jtr.ReadAsString();
                        Header.Homepage = (Header.Homepage == null ? content : Header.Homepage);
                        break;

                    case "url":
                        content = jtr.ReadAsString();
                        Header.Url = (Header.Url == null ? content : Header.Url);
                        break;

                    case "comment":
                        content = jtr.ReadAsString();
                        Header.Comment = (Header.Comment == null ? content : Header.Comment);
                        break;

                    case "header":
                        content = jtr.ReadAsString();
                        Header.HeaderSkipper = (Header.HeaderSkipper == null ? content : Header.HeaderSkipper);
                        break;

                    case "type": // This is exclusive to TruRip XML
                        content = jtr.ReadAsString();
                        Header.Type = (Header.Type == null ? content : Header.Type);
                        superdat = superdat || content.Contains("SuperDAT");
                        break;

                    case "forcemerging":
                        if (Header.ForceMerging == MergingFlag.None)
                            Header.ForceMerging = jtr.ReadAsString().AsMergingFlag();

                        break;

                    case "forcenodump":
                        if (Header.ForceNodump == NodumpFlag.None)
                            Header.ForceNodump = jtr.ReadAsString().AsNodumpFlag();

                        break;

                    case "forcepacking":
                        if (Header.ForcePacking == PackingFlag.None)
                            Header.ForcePacking = jtr.ReadAsString().AsPackingFlag();

                        break;

                    #endregion

                    #region ListXML

                    case "debug":
                        content = jtr.ReadAsString();
                        Header.Debug = (Header.Debug == null ? content.AsYesNo() : Header.Debug);
                        break;

                    case "mameconfig":
                        content = jtr.ReadAsString();
                        Header.MameConfig = (Header.MameConfig == null ? content : Header.MameConfig);
                        break;

                    #endregion

                    #region Logiqx

                    case "build":
                        content = jtr.ReadAsString();
                        Header.Build = (Header.Build == null ? content : Header.Build);
                        break;

                    case "rommode":
                        content = jtr.ReadAsString();
                        Header.RomMode = (Header.RomMode == MergingFlag.None ? content.AsMergingFlag() : Header.RomMode);
                        break;

                    case "biosmode":
                        content = jtr.ReadAsString();
                        Header.BiosMode = (Header.BiosMode == MergingFlag.None ? content.AsMergingFlag() : Header.BiosMode);
                        break;

                    case "samplemode":
                        content = jtr.ReadAsString();
                        Header.SampleMode = (Header.SampleMode == MergingFlag.None ? content.AsMergingFlag() : Header.SampleMode);
                        break;

                    case "lockrommode":
                        content = jtr.ReadAsString();
                        Header.LockRomMode = (Header.LockRomMode == null ? content.AsYesNo() : Header.LockRomMode);
                        break;

                    case "lockbiosmode":
                        content = jtr.ReadAsString();
                        Header.LockBiosMode = (Header.LockBiosMode == null ? content.AsYesNo() : Header.LockBiosMode);
                        break;

                    case "locksamplemode":
                        content = jtr.ReadAsString();
                        Header.LockSampleMode = (Header.LockSampleMode == null ? content.AsYesNo() : Header.LockSampleMode);
                        break;

                    #endregion

                    #region OfflineList

                    case "system":
                        content = jtr.ReadAsString();
                        Header.System = (Header.System == null ? content : Header.System);
                        break;

                    case "screenshotswidth":
                        content = jtr.ReadAsString();
                        Header.ScreenshotsWidth = (Header.ScreenshotsWidth == null ? content : Header.ScreenshotsWidth);
                        break;

                    case "screenshotsheight":
                        content = jtr.ReadAsString();
                        Header.ScreenshotsHeight = (Header.ScreenshotsHeight == null ? content : Header.ScreenshotsHeight);
                        break;

                    case "infos":
                        Header.Infos = new List<OfflineListInfo>();
                        jtr.Read(); // Start Array
                        while (!sr.EndOfStream)
                        {
                            jtr.Read(); // Start object (or end array)
                            if (jtr.TokenType == JsonToken.EndArray)
                                break;

                            // Get default values
                            string nameValue = string.Empty;
                            bool? visibleValue = null;
                            bool? inNamingOptionValue = null;
                            bool? defaultValue = null;

                            jtr.Read();
                            while (!sr.EndOfStream)
                            {
                                // If we hit the end of the machine, return
                                if (jtr.TokenType == JsonToken.EndObject)
                                    return;

                                // We don't care about anything except properties
                                if (jtr.TokenType != JsonToken.PropertyName)
                                {
                                    jtr.Read();
                                    continue;
                                }

                                switch (jtr.Value)
                                {
                                    case "name":
                                        nameValue = jtr.ReadAsString();
                                        break;
                                    case "visible":
                                        visibleValue = jtr.ReadAsString().AsYesNo();
                                        break;
                                    case "inNamingOption":
                                        inNamingOptionValue = jtr.ReadAsString().AsYesNo();
                                        break;
                                    case "default":
                                        defaultValue = jtr.ReadAsString().AsYesNo();
                                        break;
                                }

                                jtr.Read(); // End object
                            }
                            
                            // Add the new info object
                            Header.Infos.Add(new OfflineListInfo(
                                nameValue,
                                visibleValue,
                                inNamingOptionValue,
                                defaultValue));
                        }

                        break;

                    case "canopen":
                        Header.CanOpen = new List<string>();
                        jtr.Read(); // Start Array
                        while (!sr.EndOfStream && jtr.TokenType != JsonToken.EndArray)
                        {
                            Header.CanOpen.Add(jtr.ReadAsString());
                        }

                        break;

                    case "romtitle":
                        content = jtr.ReadAsString();
                        Header.RomTitle = (Header.MameConfig == null ? content : Header.RomTitle);
                        break;

                    #endregion

                    #region RomCenter

                    case "rcversion":
                        content = jtr.ReadAsString();
                        Header.RomCenterVersion = (Header.RomCenterVersion == null ? content : Header.RomCenterVersion);
                        break;

                    #endregion

                    default:
                        break;
                }

                jtr.Read();
            }
        }

        /// <summary>
        /// Read machine array information
        /// </summary>
        /// <param name="sr">StreamReader to use to parse the header</param>
        /// <param name="itr">JsonTextReader to use to parse the machine</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        private void ReadMachines(
            StreamReader sr,
            JsonTextReader jtr,

            // Standard Dat parsing
            string filename,
            int indexId)
        {
            // If the reader is invalid, skip
            if (jtr == null)
                return;

            jtr.Read();
            while (!sr.EndOfStream)
            {
                // If we hit the end of an array, we want to return
                if (jtr.TokenType == JsonToken.EndArray)
                    return;

                // We don't care about anything except start object
                if (jtr.TokenType != JsonToken.StartObject)
                {
                    jtr.Read();
                    continue;
                }

                ReadMachine(sr, jtr, filename, indexId);
                jtr.Read();
            }
        }

        /// <summary>
        /// Read machine information
        /// </summary>
        /// <param name="sr">StreamReader to use to parse the header</param>
        /// <param name="itr">JsonTextReader to use to parse the machine</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        private void ReadMachine(
            StreamReader sr,
            JsonTextReader jtr,

            // Standard Dat parsing
            string filename,
            int indexId)
        {
            // If we have an empty machine, skip it
            if (jtr == null)
                return;

            // Prepare internal variables
            Machine machine = new Machine();

            jtr.Read();
            while (!sr.EndOfStream)
            {
                // If we hit the end of the machine, return
                if (jtr.TokenType == JsonToken.EndObject)
                    return;

                // We don't care about anything except properties
                if (jtr.TokenType != JsonToken.PropertyName)
                {
                    jtr.Read();
                    continue;
                }

                switch (jtr.Value)
                {
                    #region Common

                    case "name":
                        machine.Name = jtr.ReadAsString();
                        break;
                    case "comment":
                        machine.Comment = jtr.ReadAsString();
                        break;
                    case "description":
                        machine.Description = jtr.ReadAsString();
                        break;
                    case "year":
                        machine.Year = jtr.ReadAsString();
                        break;
                    case "manufacturer":
                        machine.Manufacturer = jtr.ReadAsString();
                        break;
                    case "publisher":
                        machine.Publisher = jtr.ReadAsString();
                        break;
                    case "category":
                        machine.Category = jtr.ReadAsString();
                        break;
                    case "romof":
                        machine.RomOf = jtr.ReadAsString();
                        break;
                    case "cloneof":
                        machine.CloneOf = jtr.ReadAsString();
                        break;
                    case "sampleof":
                        machine.SampleOf = jtr.ReadAsString();
                        break;
                    case "isbios":
                        string isbios = jtr.ReadAsString();
                        if (string.Equals(isbios, "yes", StringComparison.OrdinalIgnoreCase))
                            machine.MachineType &= MachineType.Bios;

                        break;
                    case "isdevice":
                        string isdevice = jtr.ReadAsString();
                        if (string.Equals(isdevice, "yes", StringComparison.OrdinalIgnoreCase))
                            machine.MachineType &= MachineType.Device;

                        break;
                    case "ismechanical":
                        string ismechanical = jtr.ReadAsString();
                        if (string.Equals(ismechanical, "yes", StringComparison.OrdinalIgnoreCase))
                            machine.MachineType &= MachineType.Mechanical;

                        break;
                    case "items":
                        ReadItems(sr, jtr, filename, indexId, machine);
                        break;

                    #endregion

                    #region AttractMode

                    case "players":
                        machine.Players = jtr.ReadAsString();
                        break;
                    case "rotation":
                        machine.Rotation = jtr.ReadAsString();
                        break;
                    case "control":
                        machine.Control = jtr.ReadAsString();
                        break;
                    case "status":
                        machine.Status = jtr.ReadAsString();
                        break;
                    case "displaycount":
                        machine.DisplayCount = jtr.ReadAsString();
                        break;
                    case "displaytype":
                        machine.DisplayType = jtr.ReadAsString();
                        break;
                    case "buttons":
                        machine.Buttons = jtr.ReadAsString();
                        break;

                    #endregion

                    #region ListXML

                    case "sourcefile":
                        machine.SourceFile = jtr.ReadAsString();
                        break;
                    case "runnable":
                        machine.Runnable = jtr.ReadAsString().AsRunnable();
                        break;
                    case "devices":
                        machine.DeviceReferences = new List<ListXmlDeviceReference>();
                        jtr.Read(); // Start Array
                        while (!sr.EndOfStream && jtr.TokenType != JsonToken.EndArray)
                        {
                            machine.DeviceReferences.Add(new ListXmlDeviceReference() { Name = jtr.ReadAsString() });
                        }

                        break;
                    case "slotoptions":
                        machine.SlotOptions = new List<string>();
                        jtr.Read(); // Start Array
                        while (!sr.EndOfStream && jtr.TokenType != JsonToken.EndArray)
                        {
                            machine.SlotOptions.Add(jtr.ReadAsString());
                        }

                        break;
                    case "infos":
                        machine.Infos = new List<ListXmlInfo>();
                        jtr.Read(); // Start Array
                        while (!sr.EndOfStream)
                        {
                            jtr.Read(); // Start object (or end array)
                            if (jtr.TokenType == JsonToken.EndArray)
                                break;

                            var info = new ListXmlInfo();

                            jtr.Read(); // Key
                            info.Name = jtr.Value as string;
                            info.Value = jtr.ReadAsString();
                            jtr.Read(); // End object

                            machine.Infos.Add(info);
                        }

                        break;

                    #endregion

                    #region Logiqx

                    case "board":
                        machine.Board = jtr.ReadAsString();
                        break;
                    case "rebuildto":
                        machine.RebuildTo = jtr.ReadAsString();
                        break;

                    #endregion

                    #region Logiqx EmuArc

                    case "titleid":
                        machine.TitleID = jtr.ReadAsString();
                        break;
                    case "developer":
                        machine.Developer = jtr.ReadAsString();
                        break;
                    case "genre":
                        machine.Genre = jtr.ReadAsString();
                        break;
                    case "subgenre":
                        machine.Subgenre = jtr.ReadAsString();
                        break;
                    case "ratings":
                        machine.Ratings = jtr.ReadAsString();
                        break;
                    case "score":
                        machine.Score = jtr.ReadAsString();
                        break;
                    case "enabled":
                        machine.Enabled = jtr.ReadAsString();
                        break;
                    case "hascrc":
                        machine.HasCrc = jtr.ReadAsString().AsYesNo();
                        break;
                    case "relatedto":
                        machine.RelatedTo = jtr.ReadAsString();
                        break;

                    #endregion

                    #region OpenMSX

                    case "genmsxid":
                        machine.GenMSXID = jtr.ReadAsString();
                        break;
                    case "system":
                        machine.System = jtr.ReadAsString();
                        break;
                    case "country":
                        machine.Country = jtr.ReadAsString();
                        break;

                    #endregion

                    #region SoftwareList

                    case "supported":
                        machine.Supported = jtr.ReadAsString().AsSupported();
                        break;

                    case "sharedfeat":
                        machine.SharedFeatures = new List<SoftwareListSharedFeature>();
                        jtr.Read(); // Start Array
                        while (!sr.EndOfStream)
                        {
                            jtr.Read(); // Start object (or end array)
                            if (jtr.TokenType == JsonToken.EndArray)
                                break;

                            jtr.Read(); // Key
                            string key = jtr.Value as string;
                            string value = jtr.ReadAsString();
                            jtr.Read(); // End object

                            machine.SharedFeatures.Add(new SoftwareListSharedFeature(key, value));
                        }

                        break;

                    case "dipswitches":
                        machine.DipSwitches = new List<ListXmlDipSwitch>();
                        jtr.Read(); // Start Array
                        while (!sr.EndOfStream)
                        {
                            jtr.Read(); // Start object (or end array)
                            if (jtr.TokenType == JsonToken.EndArray)
                                break;

                            jtr.Read(); // Name Key
                            string name = jtr.ReadAsString();
                            jtr.Read(); // Tag Key
                            string tag = jtr.ReadAsString();
                            jtr.Read(); // Mask Key
                            string mask = jtr.ReadAsString();

                            var dipSwitch = new ListXmlDipSwitch();
                            dipSwitch.Name = name;
                            dipSwitch.Tag = tag;
                            dipSwitch.Mask = mask;

                            jtr.Read(); // Start dipvalues object
                            while (!sr.EndOfStream)
                            {
                                jtr.Read(); // Start object (or end array)
                                if (jtr.TokenType == JsonToken.EndArray)
                                    break;

                                jtr.Read(); // Name Key
                                string valname = jtr.ReadAsString();
                                jtr.Read(); // Value Key
                                string value = jtr.ReadAsString();
                                jtr.Read(); // Default Key
                                bool? def = jtr.ReadAsString().AsYesNo();
                                jtr.Read(); // End object

                                var dipValue = new ListXmlDipValue();
                                dipValue.Name = valname;
                                dipValue.Value = value;
                                dipValue.Default = def;

                                dipSwitch.Values.Add(dipValue);
                            }

                            jtr.Read(); // End object

                            machine.DipSwitches.Add(dipSwitch);
                        }

                        break;

                        #endregion

                    default:
                        break;
                }

                jtr.Read();
            }
        }

        /// <summary>
        /// Read item array information
        /// </summary>
        /// <param name="sr">StreamReader to use to parse the header</param>
        /// <param name="jtr">JsonTextReader to use to parse the machine</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="machine">Machine information to add to the parsed items</param>
        private void ReadItems(
            StreamReader sr,
            JsonTextReader jtr,

            // Standard Dat parsing
            string filename,
            int indexId,

            // Miscellaneous
            Machine machine)
        {
            // If the reader is invalid, skip
            if (jtr == null)
                return;

            jtr.Read();
            while (!sr.EndOfStream)
            {
                // If we hit the end of an array, we want to return
                if (jtr.TokenType == JsonToken.EndArray)
                    return;

                // We don't care about anything except start object
                if (jtr.TokenType != JsonToken.StartObject)
                {
                    jtr.Read();
                    continue;
                }

                ReadItem(sr, jtr, filename, indexId, machine);
                jtr.Read();
            }
        }

        /// <summary>
        /// Read item information
        /// </summary>
        /// <param name="sr">StreamReader to use to parse the header</param>
        /// <param name="jtr">JsonTextReader to use to parse the machine</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="machine">Machine information to add to the parsed items</param>
        private void ReadItem(
            StreamReader sr,
            JsonTextReader jtr,

            // Standard Dat parsing
            string filename,
            int indexId,

            // Miscellaneous
            Machine machine)
        {
            // If we have an empty machine, skip it
            if (jtr == null)
                return;

            // Prepare internal variables
            bool? def = null,
                writable = null,
                optional = null,
                inverted = null;
            long size = -1;
            long? areaSize = null;
            string name = null,
                altName = null,
                altTitle = null,
                original = null,
                msxType = null,
                remark = null,
                boot = null,
                partName = null,
                partInterface = null,
                areaName = null,
                areaWidth = null,
                areaEndianness = null,
                value = null,
                loadFlag = null,
                biosDescription = null,
                region = null,
                language = null,
                date = null,
                crc = null,
                md5 = null,
                ripemd160 = null,
                sha1 = null,
                sha256 = null,
                sha384 = null,
                sha512 = null,
                merge = null,
                index = null,
                offset = null,
                bios = null;
            ItemStatus? itemStatus = null;
            ItemType? itemType = null;
            OpenMSXSubType subType = OpenMSXSubType.NULL;
            List<SoftwareListFeature> features = null;

            jtr.Read();
            while (!sr.EndOfStream)
            {
                // If we hit the end of the machine - create, add, and return
                if (jtr.TokenType == JsonToken.EndObject)
                {
                    // If we didn't read something valid, just return
                    if (itemType == null)
                        return;

                    DatItem datItem = DatItem.Create(itemType.Value);
                    datItem.CopyMachineInformation(machine);
                    datItem.Source = new Source { Index = indexId, Name = filename };

                    datItem.Name = name;

                    datItem.AltName = altName;
                    datItem.AltTitle = altTitle;

                    datItem.Original = new OpenMSXOriginal() { Name = original };
                    datItem.OpenMSXSubType = subType;
                    datItem.OpenMSXType = msxType;
                    datItem.Remark = remark;
                    datItem.Boot = boot;

                    datItem.PartName = partName;
                    datItem.PartInterface = partInterface;
                    datItem.Features = features;
                    datItem.AreaName = areaName;
                    datItem.AreaSize = areaSize;
                    datItem.AreaWidth = areaWidth;
                    datItem.AreaEndianness = areaEndianness;
                    datItem.Value = value;
                    datItem.LoadFlag = loadFlag;

                    if (itemType == ItemType.BiosSet)
                    {
                        (datItem as BiosSet).Description = biosDescription;
                        (datItem as BiosSet).Default = def;
                    }
                    else if (itemType == ItemType.Disk)
                    {
                        (datItem as Disk).MD5 = md5;
#if NET_FRAMEWORK
                        (datItem as Disk).RIPEMD160 = ripemd160;
#endif
                        (datItem as Disk).SHA1 = sha1;
                        (datItem as Disk).SHA256 = sha256;
                        (datItem as Disk).SHA384 = sha384;
                        (datItem as Disk).SHA512 = sha512;
                        (datItem as Disk).MergeTag = merge;
                        (datItem as Disk).Region = region;
                        (datItem as Disk).Index = index;
                        (datItem as Disk).Writable = writable;
                        (datItem as Disk).ItemStatus = itemStatus ?? ItemStatus.None;
                        (datItem as Disk).Optional = optional;
                    }
                    else if (itemType == ItemType.Release)
                    {
                        (datItem as Release).Region = region;
                        (datItem as Release).Language = language;
                        (datItem as Release).Date = date;
                        (datItem as Release).Default = def;
                    }
                    else if (itemType == ItemType.Rom)
                    {
                        (datItem as Rom).Bios = bios;
                        (datItem as Rom).Size = size;
                        (datItem as Rom).CRC = crc;
                        (datItem as Rom).MD5 = md5;
#if NET_FRAMEWORK
                        (datItem as Rom).RIPEMD160 = ripemd160;
#endif
                        (datItem as Rom).SHA1 = sha1;
                        (datItem as Rom).SHA256 = sha256;
                        (datItem as Rom).SHA384 = sha384;
                        (datItem as Rom).SHA512 = sha512;
                        (datItem as Rom).MergeTag = merge;
                        (datItem as Rom).Region = region;
                        (datItem as Rom).Offset = offset;
                        (datItem as Rom).Date = date;
                        (datItem as Rom).ItemStatus = itemStatus ?? ItemStatus.None;
                        (datItem as Rom).Optional = optional;
                        (datItem as Rom).Inverted = inverted;
                    }

                    ParseAddHelper(datItem);

                    return;
                }

                // We don't care about anything except properties
                if (jtr.TokenType != JsonToken.PropertyName)
                {
                    jtr.Read();
                    continue;
                }

                switch (jtr.Value)
                {
                    #region Common

                    case "type":
                        itemType = jtr.ReadAsString().AsItemType();
                        break;

                    case "name":
                        name = jtr.ReadAsString();
                        break;

                    #endregion

                    #region AttractMode

                    case "alt_romname":
                        altName = jtr.ReadAsString();
                        break;

                    case "alt_title":
                        altTitle = jtr.ReadAsString();
                        break;

                    #endregion

                    #region OpenMSX

                    case "original":
                        original = jtr.ReadAsString();
                        break;

                    case "openmsx_subtype":
                        subType = jtr.ReadAsString().AsOpenMSXSubType();
                        break;

                    case "openmsx_type":
                        msxType = jtr.ReadAsString();
                        break;

                    case "remark":
                        remark = jtr.ReadAsString();
                        break;

                    case "boot":
                        boot = jtr.ReadAsString();
                        break;

                    #endregion

                    #region SoftwareList

                    case "partname":
                        partName = jtr.ReadAsString();
                        break;

                    case "partinterface":
                        partInterface = jtr.ReadAsString();
                        break;

                    case "features":
                        features = new List<SoftwareListFeature>();
                        jtr.Read(); // Start Array
                        while (!sr.EndOfStream)
                        {
                            jtr.Read(); // Start object (or end array)
                            if (jtr.TokenType == JsonToken.EndArray)
                                break;

                            jtr.Read(); // Key
                            string key = jtr.Value as string;
                            string featureValue = jtr.ReadAsString();
                            jtr.Read(); // End object

                            features.Add(new SoftwareListFeature(key, featureValue));
                        }

                        break;

                    case "areaname":
                        areaName = jtr.ReadAsString();
                        break;

                    case "areasize":
                        if (Int64.TryParse(jtr.ReadAsString(), out long tempAreaSize))
                            areaSize = tempAreaSize;
                        else
                            areaSize = null;

                        break;

                    case "areawidth":
                        areaWidth = jtr.ReadAsString();
                        break;

                    case "areaendianness":
                        areaEndianness = jtr.ReadAsString();
                        break;

                    case "value":
                        value = jtr.ReadAsString();
                        break;

                    case "loadflag":
                        loadFlag = jtr.ReadAsString();
                        break;

                    #endregion

                    case "description":
                        biosDescription = jtr.ReadAsString();
                        break;

                    case "default":
                        def = jtr.ReadAsBoolean();
                        break;

                    case "region":
                        region = jtr.ReadAsString();
                        break;

                    case "language":
                        language = jtr.ReadAsString();
                        break;

                    case "date":
                        date = jtr.ReadAsString();
                        break;

                    case "size":
                        if (!Int64.TryParse(jtr.ReadAsString(), out size))
                            size = -1;

                        break;

                    case "crc":
                        crc = jtr.ReadAsString();
                        break;

                    case "md5":
                        md5 = jtr.ReadAsString();
                        break;

                    case "ripemd160":
                        ripemd160 = jtr.ReadAsString();
                        break;

                    case "sha1":
                        sha1 = jtr.ReadAsString();
                        break;

                    case "sha256":
                        sha256 = jtr.ReadAsString();
                        break;

                    case "sha384":
                        sha384 = jtr.ReadAsString();
                        break;

                    case "sha512":
                        sha512 = jtr.ReadAsString();
                        break;

                    case "merge":
                        merge = jtr.ReadAsString();
                        break;

                    case "index":
                        index = jtr.ReadAsString();
                        break;

                    case "writable":
                        writable = jtr.ReadAsBoolean();
                        break;

                    case "status":
                        itemStatus = jtr.ReadAsString().AsItemStatus();
                        break;

                    case "optional":
                        optional = jtr.ReadAsBoolean();
                        break;

                    case "offset":
                        offset = jtr.ReadAsString();
                        break;

                    case "bios":
                        bios = jtr.ReadAsString();
                        break;

                    case "inverted":
                        inverted = jtr.ReadAsBoolean();
                        break;

                    default:
                        break;
                }

                jtr.Read();
            }
        }

        /// <summary>
        /// Create and open an output file for writing direct from a dictionary
        /// </summary>
        /// <param name="outfile">Name of the file to write to</param>
        /// <param name="ignoreblanks">True if blank roms should be skipped on output, false otherwise (default)</param>
        /// <returns>True if the DAT was written correctly, false otherwise</returns>
        public override bool WriteToFile(string outfile, bool ignoreblanks = false)
        {
            try
            {
                Globals.Logger.User($"Opening file for writing: {outfile}");
                FileStream fs = FileExtensions.TryCreate(outfile);

                // If we get back null for some reason, just log and return
                if (fs == null)
                {
                    Globals.Logger.Warning($"File '{outfile}' could not be created for writing! Please check to see if the file is writable");
                    return false;
                }

                StreamWriter sw = new StreamWriter(fs, new UTF8Encoding(false));
                JsonTextWriter jtw = new JsonTextWriter(sw)
                {
                    Formatting = Formatting.Indented,
                    IndentChar = '\t',
                    Indentation = 1
                };

                // Write out the header
                WriteHeader(jtw);

                // Write out each of the machines and roms
                string lastgame = null;

                // Use a sorted list of games to output
                foreach (string key in Items.SortedKeys)
                {
                    List<DatItem> roms = Items[key];

                    // Resolve the names in the block
                    roms = DatItem.ResolveNames(roms);

                    for (int index = 0; index < roms.Count; index++)
                    {
                        DatItem rom = roms[index];

                        // There are apparently times when a null rom can skip by, skip them
                        if (rom.Name == null || rom.Machine.Name == null)
                        {
                            Globals.Logger.Warning("Null rom found!");
                            continue;
                        }

                        // If we have a different game and we're not at the start of the list, output the end of last item
                        if (lastgame != null && lastgame.ToLowerInvariant() != rom.Machine.Name.ToLowerInvariant())
                            WriteEndGame(jtw);

                        // If we have a new game, output the beginning of the new item
                        if (lastgame == null || lastgame.ToLowerInvariant() != rom.Machine.Name.ToLowerInvariant())
                            WriteStartGame(jtw, rom);

                        // If we have a "null" game (created by DATFromDir or something similar), log it to file
                        if (rom.ItemType == ItemType.Rom
                            && ((Rom)rom).Size == -1
                            && ((Rom)rom).CRC == "null")
                        {
                            Globals.Logger.Verbose($"Empty folder found: {rom.Machine.Name}");

                            rom.Name = (rom.Name == "null" ? "-" : rom.Name);
                            ((Rom)rom).Size = Constants.SizeZero;
                            ((Rom)rom).CRC = ((Rom)rom).CRC == "null" ? Constants.CRCZero : null;
                            ((Rom)rom).MD5 = ((Rom)rom).MD5 == "null" ? Constants.MD5Zero : null;
#if NET_FRAMEWORK
                            ((Rom)rom).RIPEMD160 = ((Rom)rom).RIPEMD160 == "null" ? Constants.RIPEMD160Zero : null;
#endif
                            ((Rom)rom).SHA1 = ((Rom)rom).SHA1 == "null" ? Constants.SHA1Zero : null;
                            ((Rom)rom).SHA256 = ((Rom)rom).SHA256 == "null" ? Constants.SHA256Zero : null;
                            ((Rom)rom).SHA384 = ((Rom)rom).SHA384 == "null" ? Constants.SHA384Zero : null;
                            ((Rom)rom).SHA512 = ((Rom)rom).SHA512 == "null" ? Constants.SHA512Zero : null;
                        }

                        // Now, output the rom data
                        WriteDatItem(jtw, rom, ignoreblanks);

                        // Set the new data to compare against
                        lastgame = rom.Machine.Name;
                    }
                }

                // Write the file footer out
                WriteFooter(jtw);

                Globals.Logger.Verbose("File written!" + Environment.NewLine);
                jtw.Close();
                fs.Dispose();
            }
            catch (Exception ex)
            {
                Globals.Logger.Error(ex.ToString());
                return false;
            }

            return true;
        }

        /// <summary>
        /// Write out DAT header using the supplied StreamWriter
        /// </summary>
        /// <param name="jtw">JsonTextWriter to output to</param>
        /// <returns>True if the data was written, false on error</returns>
        private bool WriteHeader(JsonTextWriter jtw)
        {
            try
            {
                jtw.WriteStartObject();

                jtw.WritePropertyName("header");
                jtw.WriteStartObject();

                #region Common

                jtw.WritePropertyName("name");
                jtw.WriteValue(Header.Name);
                jtw.WritePropertyName("description");
                jtw.WriteValue(Header.Description);
                if (!string.IsNullOrWhiteSpace(Header.RootDir))
                {
                    jtw.WritePropertyName("rootdir");
                    jtw.WriteValue(Header.RootDir);
                }
                if (!string.IsNullOrWhiteSpace(Header.Category))
                {
                    jtw.WritePropertyName("category");
                    jtw.WriteValue(Header.Category);
                }
                jtw.WritePropertyName("version");
                jtw.WriteValue(Header.Version);
                if (!string.IsNullOrWhiteSpace(Header.Date))
                {
                    jtw.WritePropertyName("date");
                    jtw.WriteValue(Header.Date);
                }
                jtw.WritePropertyName("author");
                jtw.WriteValue(Header.Author);
                if (!string.IsNullOrWhiteSpace(Header.Email))
                {
                    jtw.WritePropertyName("email");
                    jtw.WriteValue(Header.Email);
                }
                if (!string.IsNullOrWhiteSpace(Header.Homepage))
                {
                    jtw.WritePropertyName("homepage");
                    jtw.WriteValue(Header.Homepage);
                }
                if (!string.IsNullOrWhiteSpace(Header.Url))
                {
                    jtw.WritePropertyName("date");
                    jtw.WriteValue(Header.Url);
                }
                if (!string.IsNullOrWhiteSpace(Header.Comment))
                {
                    jtw.WritePropertyName("comment");
                    jtw.WriteValue(Header.Comment);
                }
                if (!string.IsNullOrWhiteSpace(Header.Type))
                {
                    jtw.WritePropertyName("type");
                    jtw.WriteValue(Header.Type);
                }
                if (Header.ForceMerging != MergingFlag.None)
                {
                    jtw.WritePropertyName("forcemerging");
                    switch (Header.ForceMerging)
                    {
                        case MergingFlag.Full:
                            jtw.WriteValue("full");
                            break;
                        case MergingFlag.Split:
                            jtw.WriteValue("split");
                            break;
                        case MergingFlag.Merged:
                            jtw.WriteValue("merged");
                            break;
                        case MergingFlag.NonMerged:
                            jtw.WriteValue("nonmerged");
                            break;
                    }
                }
                if (Header.ForcePacking != PackingFlag.None)
                {
                    jtw.WritePropertyName("forcepacking");
                    switch (Header.ForcePacking)
                    {
                        case PackingFlag.Unzip:
                            jtw.WriteValue("unzip");
                            break;
                        case PackingFlag.Zip:
                            jtw.WriteValue("zip");
                            break;
                    }
                }
                if (Header.ForceNodump != NodumpFlag.None)
                {
                    jtw.WritePropertyName("forcenodump");
                    switch (Header.ForceNodump)
                    {
                        case NodumpFlag.Ignore:
                            jtw.WriteValue("ignore");
                            break;
                        case NodumpFlag.Obsolete:
                            jtw.WriteValue("obsolete");
                            break;
                        case NodumpFlag.Required:
                            jtw.WriteValue("required");
                            break;
                    }
                }
                if (!string.IsNullOrWhiteSpace(Header.HeaderSkipper))
                {
                    jtw.WritePropertyName("header");
                    jtw.WriteValue(Header.HeaderSkipper);
                }

                #endregion

                #region ListXML

                if (Header.Debug != null)
                {
                    jtw.WritePropertyName("debug");
                    switch (Header.Debug)
                    {
                        case true:
                            jtw.WriteValue("yes");
                            break;
                        case false:
                            jtw.WriteValue("no");
                            break;
                    }
                }

                if (!string.IsNullOrWhiteSpace(Header.MameConfig))
                {
                    jtw.WritePropertyName("mameconfig");
                    jtw.WriteValue(Header.MameConfig);
                }

                #endregion

                #region Logiqx

                if (!string.IsNullOrWhiteSpace(Header.Build))
                {
                    jtw.WritePropertyName("build");
                    jtw.WriteValue(Header.Build);
                }

                if (Header.RomMode != MergingFlag.None)
                {
                    jtw.WritePropertyName("rommode");
                    switch (Header.RomMode)
                    {
                        case MergingFlag.Split:
                            jtw.WriteValue("split");
                            break;
                        case MergingFlag.Merged:
                            jtw.WriteValue("merged");
                            break;
                        case MergingFlag.NonMerged:
                            jtw.WriteValue("unmerged");
                            break;
                    }
                }

                if (Header.BiosMode != MergingFlag.None)
                {
                    jtw.WritePropertyName("biosmode");
                    switch (Header.BiosMode)
                    {
                        case MergingFlag.Split:
                            jtw.WriteValue("split");
                            break;
                        case MergingFlag.Merged:
                            jtw.WriteValue("merged");
                            break;
                        case MergingFlag.NonMerged:
                            jtw.WriteValue("unmerged");
                            break;
                    }
                }

                if (Header.SampleMode != MergingFlag.None)
                {
                    jtw.WritePropertyName("samplemode");
                    switch (Header.SampleMode)
                    {
                        case MergingFlag.Merged:
                            jtw.WriteValue("merged");
                            break;
                        case MergingFlag.NonMerged:
                            jtw.WriteValue("unmerged");
                            break;
                    }
                }

                if (Header.LockRomMode != null)
                {
                    switch (Header.LockRomMode)
                    {
                        case true:
                            jtw.WritePropertyName("lockrommode");
                            jtw.WriteValue("yes");
                            break;
                        case false:
                            jtw.WritePropertyName("lockrommode");
                            jtw.WriteValue("no");
                            break;
                    }
                }

                if (Header.LockBiosMode != null)
                {
                    switch (Header.LockBiosMode)
                    {
                        case true:
                            jtw.WritePropertyName("lockbiosmode");
                            jtw.WriteValue("yes");
                            break;
                        case false:
                            jtw.WritePropertyName("lockbiosmode");
                            jtw.WriteValue("no");
                            break;
                    }
                }

                if (Header.LockSampleMode != null)
                {
                    switch (Header.LockSampleMode)
                    {
                        case true:
                            jtw.WritePropertyName("locksamplemode");
                            jtw.WriteValue("yes");
                            break;
                        case false:
                            jtw.WritePropertyName("locksamplemode");
                            jtw.WriteValue("no");
                            break;
                    }
                }

                #endregion

                #region OfflineList

                if (!string.IsNullOrWhiteSpace(Header.System))
                {
                    jtw.WritePropertyName("system");
                    jtw.WriteValue(Header.System);
                }

                if (!string.IsNullOrWhiteSpace(Header.ScreenshotsWidth))
                {
                    jtw.WritePropertyName("screenshotswidth");
                    jtw.WriteValue(Header.ScreenshotsWidth);
                }

                if (!string.IsNullOrWhiteSpace(Header.ScreenshotsHeight))
                {
                    jtw.WritePropertyName("screenshotsheight");
                    jtw.WriteValue(Header.ScreenshotsHeight);
                }

                if (Header.Infos != null)
                {
                    jtw.WritePropertyName("infos");
                    jtw.WriteStartArray();
                    foreach (var info in Header.Infos)
                    {
                        jtw.WriteStartObject();
                        jtw.WritePropertyName("name");
                        jtw.WriteValue(info.Name);
                        jtw.WritePropertyName("visible");
                        jtw.WriteValue(info.Visible.ToString());
                        jtw.WritePropertyName("inNamingOption");
                        jtw.WriteValue(info.IsNamingOption.ToString());
                        jtw.WritePropertyName("default");
                        jtw.WriteValue(info.Default.ToString());
                        jtw.WriteEndObject();
                    }

                    jtw.WriteEndArray();
                }

                if (Header.CanOpen != null)
                {
                    jtw.WritePropertyName("canopen");
                    jtw.WriteStartArray();
                    foreach (string extension in Header.CanOpen)
                    {
                        jtw.WriteValue(extension);
                    }

                    jtw.WriteEndArray();
                }

                if (!string.IsNullOrWhiteSpace(Header.RomTitle))
                {
                    jtw.WritePropertyName("romtitle");
                    jtw.WriteValue(Header.RomTitle);
                }

                #endregion

                #region RomCenter

                if (!string.IsNullOrWhiteSpace(Header.RomCenterVersion))
                {
                    jtw.WritePropertyName("rcversion");
                    jtw.WriteValue(Header.RomCenterVersion);
                }

                #endregion

                // End header
                jtw.WriteEndObject();

                jtw.WritePropertyName("machines");
                jtw.WriteStartArray();

                jtw.Flush();
            }
            catch (Exception ex)
            {
                Globals.Logger.Error(ex.ToString());
                return false;
            }

            return true;
        }

        /// <summary>
        /// Write out Game start using the supplied StreamWriter
        /// </summary>
        /// <param name="jtw">JsonTextWriter to output to</param>
        /// <param name="datItem">DatItem object to be output</param>
        /// <returns>True if the data was written, false on error</returns>
        private bool WriteStartGame(JsonTextWriter jtw, DatItem datItem)
        {
            try
            {
                // No game should start with a path separator
                datItem.Machine.Name = datItem.Machine.Name.TrimStart(Path.DirectorySeparatorChar);

                // Build the state based on excluded fields
                jtw.WriteStartObject();

                #region Common

                jtw.WritePropertyName("name");
                jtw.WriteValue(datItem.GetField(Field.MachineName, Header.ExcludeFields));

                if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.Comment, Header.ExcludeFields)))
                {
                    jtw.WritePropertyName("comment");
                    jtw.WriteValue(datItem.Machine.Comment);
                }
                if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.Description, Header.ExcludeFields)))
                {
                    jtw.WritePropertyName("description");
                    jtw.WriteValue(datItem.Machine.Description);
                }
                if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.Year, Header.ExcludeFields)))
                {
                    jtw.WritePropertyName("year");
                    jtw.WriteValue(datItem.Machine.Year);
                }
                if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.Manufacturer, Header.ExcludeFields)))
                {
                    jtw.WritePropertyName("manufacturer");
                    jtw.WriteValue(datItem.Machine.Manufacturer);
                }
                if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.Publisher, Header.ExcludeFields)))
                {
                    jtw.WritePropertyName("publisher");
                    jtw.WriteValue(datItem.Machine.Publisher);
                }
                if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.Category, Header.ExcludeFields)))
                {
                    jtw.WritePropertyName("category");
                    jtw.WriteValue(datItem.Machine.Category);
                }
                if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.RomOf, Header.ExcludeFields)) && !string.Equals(datItem.Machine.Name, datItem.Machine.RomOf, StringComparison.OrdinalIgnoreCase))
                {
                    jtw.WritePropertyName("romof");
                    jtw.WriteValue(datItem.Machine.RomOf);
                }
                if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.CloneOf, Header.ExcludeFields)) && !string.Equals(datItem.Machine.Name, datItem.Machine.CloneOf, StringComparison.OrdinalIgnoreCase))
                {
                    jtw.WritePropertyName("cloneof");
                    jtw.WriteValue(datItem.Machine.CloneOf);
                }
                if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.SampleOf, Header.ExcludeFields)) && !string.Equals(datItem.Machine.Name, datItem.Machine.SampleOf, StringComparison.OrdinalIgnoreCase))
                {
                    jtw.WritePropertyName("sampleof");
                    jtw.WriteValue(datItem.Machine.SampleOf);
                }
                if (!Header.ExcludeFields.Contains(Field.MachineType))
                {
                    if (datItem.Machine.MachineType.HasFlag(MachineType.Bios))
                    {
                        jtw.WritePropertyName("isbios");
                        jtw.WriteValue("yes");
                    }
                    if (datItem.Machine.MachineType.HasFlag(MachineType.Device))
                    {
                        jtw.WritePropertyName("isdevice");
                        jtw.WriteValue("yes");
                    }
                    if (datItem.Machine.MachineType.HasFlag(MachineType.Mechanical))
                    {
                        jtw.WritePropertyName("ismechanical");
                        jtw.WriteValue("yes");
                    }
                }

                #endregion

                #region AttractMode

                if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.Players, Header.ExcludeFields)))
                {
                    jtw.WritePropertyName("players");
                    jtw.WriteValue(datItem.Machine.Players);
                }
                if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.Rotation, Header.ExcludeFields)))
                {
                    jtw.WritePropertyName("rotation");
                    jtw.WriteValue(datItem.Machine.Rotation);
                }
                if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.Control, Header.ExcludeFields)))
                {
                    jtw.WritePropertyName("control");
                    jtw.WriteValue(datItem.Machine.Control);
                }
                if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.SupportStatus, Header.ExcludeFields)))
                {
                    jtw.WritePropertyName("status");
                    jtw.WriteValue(datItem.Machine.Status);
                }
                if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.DisplayCount, Header.ExcludeFields)))
                {
                    jtw.WritePropertyName("displaycount");
                    jtw.WriteValue(datItem.Machine.DisplayCount);
                }
                if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.DisplayType, Header.ExcludeFields)))
                {
                    jtw.WritePropertyName("displaytype");
                    jtw.WriteValue(datItem.Machine.DisplayType);
                }
                if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.Buttons, Header.ExcludeFields)))
                {
                    jtw.WritePropertyName("buttons");
                    jtw.WriteValue(datItem.Machine.Buttons);
                }

                #endregion

                #region ListXML

                if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.SourceFile, Header.ExcludeFields)))
                {
                    jtw.WritePropertyName("sourcefile");
                    jtw.WriteValue(datItem.Machine.SourceFile);
                }
                if (!Header.ExcludeFields.Contains(Field.Runnable) && datItem.Machine.Runnable != null)
                {
                    switch (datItem.Machine.Runnable)
                    {
                        case Runnable.No:
                            jtw.WritePropertyName("runnable");
                            jtw.WriteValue("no");
                            break;
                        case Runnable.Partial:
                            jtw.WritePropertyName("runnable");
                            jtw.WriteValue("partial");
                            break;
                        case Runnable.Yes:
                            jtw.WritePropertyName("runnable");
                            jtw.WriteValue("yes");
                            break;
                    }
                }
                if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.Devices, Header.ExcludeFields)))
                {
                    jtw.WritePropertyName("devices");
                    jtw.WriteStartArray();
                    foreach (ListXmlDeviceReference device in datItem.Machine.DeviceReferences)
                    {
                        jtw.WriteValue(device.Name);
                    }

                    jtw.WriteEndArray();
                }
                if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.SlotOptions, Header.ExcludeFields)))
                {
                    jtw.WritePropertyName("slotoptions");
                    jtw.WriteStartArray();
                    foreach (string slotoption in datItem.Machine.SlotOptions)
                    {
                        jtw.WriteValue(slotoption);
                    }

                    jtw.WriteEndArray();
                }
                if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.Infos, Header.ExcludeFields)))
                {
                    jtw.WritePropertyName("infos");
                    jtw.WriteStartArray();
                    foreach (var info in datItem.Machine.Infos)
                    {
                        jtw.WriteStartObject();
                        jtw.WritePropertyName(info.Name);
                        jtw.WriteValue(info.Value);
                        jtw.WriteEndObject();
                    }

                    jtw.WriteEndArray();
                }

                #endregion

                #region Logiqx

                if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.Board, Header.ExcludeFields)))
                {
                    jtw.WritePropertyName("board");
                    jtw.WriteValue(datItem.Machine.Board);
                }
                if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.RebuildTo, Header.ExcludeFields)))
                {
                    jtw.WritePropertyName("rebuildto");
                    jtw.WriteValue(datItem.Machine.RebuildTo);
                }

                #endregion

                #region Logiqx EmuArc

                if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.TitleID, Header.ExcludeFields)))
                {
                    jtw.WritePropertyName("titleid");
                    jtw.WriteValue(datItem.Machine.TitleID);
                }
                if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.Developer, Header.ExcludeFields)))
                {
                    jtw.WritePropertyName("developer");
                    jtw.WriteValue(datItem.Machine.Developer);
                }
                if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.Genre, Header.ExcludeFields)))
                {
                    jtw.WritePropertyName("genre");
                    jtw.WriteValue(datItem.Machine.Genre);
                }
                if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.Subgenre, Header.ExcludeFields)))
                {
                    jtw.WritePropertyName("subgenre");
                    jtw.WriteValue(datItem.Machine.Subgenre);
                }
                if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.Ratings, Header.ExcludeFields)))
                {
                    jtw.WritePropertyName("ratings");
                    jtw.WriteValue(datItem.Machine.Ratings);
                }
                if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.Score, Header.ExcludeFields)))
                {
                    jtw.WritePropertyName("score");
                    jtw.WriteValue(datItem.Machine.Score);
                }
                if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.Enabled, Header.ExcludeFields)))
                {
                    jtw.WritePropertyName("enabled");
                    jtw.WriteValue(datItem.Machine.Enabled);
                }
                if (!Header.ExcludeFields.Contains(Field.HasCrc) && datItem.Machine.HasCrc != null)
                {
                    if (datItem.Machine.HasCrc == true)
                    {
                        jtw.WritePropertyName("hascrc");
                        jtw.WriteValue("yes");
                    }
                    else if (datItem.Machine.HasCrc == false)
                    {
                        jtw.WritePropertyName("hascrc");
                        jtw.WriteValue("no");
                    }
                }
                if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.RelatedTo, Header.ExcludeFields)))
                {
                    jtw.WritePropertyName("relatedto");
                    jtw.WriteValue(datItem.Machine.RelatedTo);
                }

                #endregion

                #region OpenMSX

                if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.GenMSXID, Header.ExcludeFields)))
                {
                    jtw.WritePropertyName("genmsxid");
                    jtw.WriteValue(datItem.Machine.GenMSXID);
                }
                if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.System, Header.ExcludeFields)))
                {
                    jtw.WritePropertyName("system");
                    jtw.WriteValue(datItem.Machine.System);
                }
                if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.Country, Header.ExcludeFields)))
                {
                    jtw.WritePropertyName("country");
                    jtw.WriteValue(datItem.Machine.Country);
                }

                #endregion

                #region SoftwareList

                if (!Header.ExcludeFields.Contains(Field.Supported) && datItem.Machine.Supported != Supported.NULL)
                {
                    switch (datItem.Machine.Supported)
                    {
                        case Supported.No:
                            jtw.WritePropertyName("supported");
                            jtw.WriteValue("yes");
                            break;
                        case Supported.Partial:
                            jtw.WritePropertyName("supported");
                            jtw.WriteValue("partial");
                            break;
                        case Supported.Yes:
                            jtw.WritePropertyName("supported");
                            jtw.WriteValue("no");
                            break;
                    }
                }
                if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.SharedFeatures, Header.ExcludeFields)))
                {
                    jtw.WritePropertyName("sharedfeat");
                    jtw.WriteStartArray();
                    foreach (var feature in datItem.Machine.SharedFeatures)
                    {
                        jtw.WriteStartObject();
                        jtw.WritePropertyName(feature.Name);
                        jtw.WriteValue(feature.Value);
                        jtw.WriteEndObject();
                    }

                    jtw.WriteEndArray();
                }
                if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.DipSwitches, Header.ExcludeFields)))
                {
                    jtw.WritePropertyName("dipswitches");
                    jtw.WriteStartArray();
                    foreach (var dip in datItem.Machine.DipSwitches)
                    {
                        jtw.WriteStartObject();
                        jtw.WritePropertyName("name");
                        jtw.WriteValue(dip.Name);
                        jtw.WritePropertyName("tag");
                        jtw.WriteValue(dip.Tag);
                        jtw.WritePropertyName("mask");
                        jtw.WriteValue(dip.Mask);
                        jtw.WriteStartArray();

                        foreach (ListXmlDipValue dipval in dip.Values)
                        {
                            jtw.WriteStartObject();
                            jtw.WritePropertyName("name");
                            jtw.WriteValue(dipval.Name);
                            jtw.WritePropertyName("value");
                            jtw.WriteValue(dipval.Value);
                            jtw.WritePropertyName("default");
                            jtw.WriteValue(dipval.Default == true ? "yes" : "no");
                            jtw.WriteEndObject();
                        }

                        jtw.WriteEndArray();

                        // End dipswitch
                        jtw.WriteEndObject();
                    }

                    jtw.WriteEndArray();
                }

                #endregion

                jtw.WritePropertyName("items");
                jtw.WriteStartArray();

                jtw.Flush();
            }
            catch (Exception ex)
            {
                Globals.Logger.Error(ex.ToString());
                return false;
            }

            return true;
        }

        /// <summary>
        /// Write out Game end using the supplied StreamWriter
        /// </summary>
        /// <param name="jtw">JsonTextWriter to output to</param>
        /// <returns>True if the data was written, false on error</returns>
        private bool WriteEndGame(JsonTextWriter jtw)
        {
            try
            {
                // End items
                jtw.WriteEndArray();

                // End machine
                jtw.WriteEndObject();

                jtw.Flush();
            }
            catch (Exception ex)
            {
                Globals.Logger.Error(ex.ToString());
                return false;
            }

            return true;
        }

        /// <summary>
        /// Write out DatItem using the supplied StreamWriter
        /// </summary>
        /// <param name="jtw">JsonTextWriter to output to</param>
        /// <param name="datItem">DatItem object to be output</param>
        /// <param name="ignoreblanks">True if blank roms should be skipped on output, false otherwise (default)</param>
        /// <returns>True if the data was written, false on error</returns>
        private bool WriteDatItem(JsonTextWriter jtw, DatItem datItem, bool ignoreblanks = false)
        {
            // If we are in ignore blanks mode AND we have a blank (0-size) rom, skip
            if (ignoreblanks && (datItem.ItemType == ItemType.Rom && ((datItem as Rom).Size == 0 || (datItem as Rom).Size == -1)))
                return true;

            // If we have the blank item type somehow, skip
            if (datItem.ItemType == ItemType.Blank)
                return true;

            try
            {
                // Pre-process the item name
                ProcessItemName(datItem, true);

                // Build the state based on excluded fields
                jtw.WriteStartObject();
                jtw.WritePropertyName("type");

                switch (datItem.ItemType)
                {
                    case ItemType.Archive:
                        jtw.WriteValue("archive");
                        jtw.WritePropertyName("name");
                        jtw.WriteValue(datItem.GetField(Field.Name, Header.ExcludeFields));
                        break;

                    case ItemType.BiosSet:
                        var biosSet = datItem as BiosSet;
                        jtw.WriteValue("biosset");
                        jtw.WritePropertyName("name");
                        jtw.WriteValue(biosSet.GetField(Field.Name, Header.ExcludeFields));
                        if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.BiosDescription, Header.ExcludeFields)))
                        {
                            jtw.WritePropertyName("description");
                            jtw.WriteValue(biosSet.Description);
                        }
                        if (!Header.ExcludeFields.Contains(Field.Default) && biosSet.Default != null)
                        {
                            jtw.WritePropertyName("default");
                            jtw.WriteValue(biosSet.Default);
                        }
                        break;

                    case ItemType.Disk:
                        var disk = datItem as Disk;
                        jtw.WriteValue("disk");
                        jtw.WritePropertyName("name");
                        jtw.WriteValue(disk.GetField(Field.Name, Header.ExcludeFields));
                        if (!string.IsNullOrWhiteSpace(disk.GetField(Field.MD5, Header.ExcludeFields)))
                        {
                            jtw.WritePropertyName("md5");
                            jtw.WriteValue(disk.MD5.ToLowerInvariant());
                        }
#if NET_FRAMEWORK
                        if (!string.IsNullOrWhiteSpace(disk.GetField(Field.RIPEMD160, Header.ExcludeFields)))
                        {
                            jtw.WritePropertyName("ripemd160");
                            jtw.WriteValue(disk.RIPEMD160.ToLowerInvariant());
                        }
#endif
                        if (!string.IsNullOrWhiteSpace(disk.GetField(Field.SHA1, Header.ExcludeFields)))
                        {
                            jtw.WritePropertyName("sha1");
                            jtw.WriteValue(disk.SHA1.ToLowerInvariant());
                        }
                        if (!string.IsNullOrWhiteSpace(disk.GetField(Field.SHA256, Header.ExcludeFields)))
                        {
                            jtw.WritePropertyName("sha256");
                            jtw.WriteValue(disk.SHA256.ToLowerInvariant());
                        }
                        if (!string.IsNullOrWhiteSpace(disk.GetField(Field.SHA384, Header.ExcludeFields)))
                        {
                            jtw.WritePropertyName("sha384");
                            jtw.WriteValue(disk.SHA384.ToLowerInvariant());
                        }
                        if (!string.IsNullOrWhiteSpace(disk.GetField(Field.SHA512, Header.ExcludeFields)))
                        {
                            jtw.WritePropertyName("sha512");
                            jtw.WriteValue(disk.SHA512.ToLowerInvariant());
                        }
                        if (!string.IsNullOrWhiteSpace(disk.GetField(Field.Merge, Header.ExcludeFields)))
                        {
                            jtw.WritePropertyName("merge");
                            jtw.WriteValue(disk.MergeTag);
                        }
                        if (!string.IsNullOrWhiteSpace(disk.GetField(Field.Region, Header.ExcludeFields)))
                        {
                            jtw.WritePropertyName("region");
                            jtw.WriteValue(disk.Region);
                        }
                        if (!string.IsNullOrWhiteSpace(disk.GetField(Field.Index, Header.ExcludeFields)))
                        {
                            jtw.WritePropertyName("index");
                            jtw.WriteValue(disk.Index);
                        }
                        if (!string.IsNullOrWhiteSpace(disk.GetField(Field.Writable, Header.ExcludeFields)))
                        {
                            jtw.WritePropertyName("writable");
                            jtw.WriteValue(disk.Writable);
                        }
                        if (!Header.ExcludeFields.Contains(Field.Status) && disk.ItemStatus != ItemStatus.None)
                        {
                            jtw.WritePropertyName("status");
                            jtw.WriteValue(disk.ItemStatus.ToString().ToLowerInvariant());
                        }
                        if (!string.IsNullOrWhiteSpace(disk.GetField(Field.Optional, Header.ExcludeFields)))
                        {
                            jtw.WritePropertyName("optional");
                            jtw.WriteValue(disk.Optional);
                        }
                        break;

                    case ItemType.Release:
                        var release = datItem as Release;
                        jtw.WriteValue("release");
                        jtw.WritePropertyName("name");
                        jtw.WriteValue(release.GetField(Field.Name, Header.ExcludeFields));
                        if (!string.IsNullOrWhiteSpace(release.GetField(Field.Region, Header.ExcludeFields)))
                        {
                            jtw.WritePropertyName("region");
                            jtw.WriteValue(release.Region);
                        }
                        if (!string.IsNullOrWhiteSpace(release.GetField(Field.Language, Header.ExcludeFields)))
                        {
                            jtw.WritePropertyName("language");
                            jtw.WriteValue(release.Language);
                        }
                        if (!string.IsNullOrWhiteSpace(release.GetField(Field.Date, Header.ExcludeFields)))
                        {
                            jtw.WritePropertyName("date");
                            jtw.WriteValue(release.Date);
                        }
                        if (!Header.ExcludeFields.Contains(Field.Default) && release.Default != null)
                        {
                            jtw.WritePropertyName("default");
                            jtw.WriteValue(release.Default);
                        }
                        break;

                    case ItemType.Rom:
                        var rom = datItem as Rom;
                        jtw.WriteValue("rom");
                        jtw.WritePropertyName("name");
                        jtw.WriteValue(rom.GetField(Field.Name, Header.ExcludeFields));
                        if (!Header.ExcludeFields.Contains(Field.Size) && rom.Size != -1)
                        {
                            jtw.WritePropertyName("size");
                            jtw.WriteValue(rom.Size);
                        }
                        if (!string.IsNullOrWhiteSpace(rom.GetField(Field.Offset, Header.ExcludeFields)))
                        {
                            jtw.WritePropertyName("offset");
                            jtw.WriteValue(rom.Offset);
                        }
                        if (!string.IsNullOrWhiteSpace(rom.GetField(Field.CRC, Header.ExcludeFields)))
                        {
                            jtw.WritePropertyName("crc");
                            jtw.WriteValue(rom.CRC.ToLowerInvariant());
                        }
                        if (!string.IsNullOrWhiteSpace(rom.GetField(Field.MD5, Header.ExcludeFields)))
                        {
                            jtw.WritePropertyName("md5");
                            jtw.WriteValue(rom.MD5.ToLowerInvariant());
                        }
#if NET_FRAMEWORK
                        if (!string.IsNullOrWhiteSpace(rom.GetField(Field.RIPEMD160, Header.ExcludeFields)))
                        {
                            jtw.WritePropertyName("ripemd160");
                            jtw.WriteValue(rom.RIPEMD160.ToLowerInvariant());
                        }
#endif
                        if (!string.IsNullOrWhiteSpace(rom.GetField(Field.SHA1, Header.ExcludeFields)))
                        {
                            jtw.WritePropertyName("sha1");
                            jtw.WriteValue(rom.SHA1.ToLowerInvariant());
                        }
                        if (!string.IsNullOrWhiteSpace(rom.GetField(Field.SHA256, Header.ExcludeFields)))
                        {
                            jtw.WritePropertyName("sha256");
                            jtw.WriteValue(rom.SHA256.ToLowerInvariant());
                        }
                        if (!string.IsNullOrWhiteSpace(rom.GetField(Field.SHA384, Header.ExcludeFields)))
                        {
                            jtw.WritePropertyName("sha384");
                            jtw.WriteValue(rom.SHA384.ToLowerInvariant());
                        }
                        if (!string.IsNullOrWhiteSpace(rom.GetField(Field.SHA512, Header.ExcludeFields)))
                        {
                            jtw.WritePropertyName("sha512");
                            jtw.WriteValue(rom.SHA512.ToLowerInvariant());
                        }
                        if (!string.IsNullOrWhiteSpace(rom.GetField(Field.Bios, Header.ExcludeFields)))
                        {
                            jtw.WritePropertyName("bios");
                            jtw.WriteValue(rom.Bios);
                        }
                        if (!string.IsNullOrWhiteSpace(rom.GetField(Field.Merge, Header.ExcludeFields)))
                        {
                            jtw.WritePropertyName("merge");
                            jtw.WriteValue(rom.MergeTag);
                        }
                        if (!string.IsNullOrWhiteSpace(rom.GetField(Field.Region, Header.ExcludeFields)))
                        {
                            jtw.WritePropertyName("region");
                            jtw.WriteValue(rom.Region);
                        }
                        if (!string.IsNullOrWhiteSpace(rom.GetField(Field.Date, Header.ExcludeFields)))
                        {
                            jtw.WritePropertyName("date");
                            jtw.WriteValue(rom.Date);
                        }
                        if (!Header.ExcludeFields.Contains(Field.Status) && rom.ItemStatus != ItemStatus.None)
                        {
                            jtw.WritePropertyName("status");
                            jtw.WriteValue(rom.ItemStatus.ToString().ToLowerInvariant());
                        }
                        if (!string.IsNullOrWhiteSpace(rom.GetField(Field.Optional, Header.ExcludeFields)))
                        {
                            jtw.WritePropertyName("optional");
                            jtw.WriteValue(rom.Optional);
                        }
                        if (!string.IsNullOrWhiteSpace(rom.GetField(Field.Inverted, Header.ExcludeFields)))
                        {
                            jtw.WritePropertyName("inverted");
                            jtw.WriteValue(rom.Inverted);
                        }
                        break;

                    case ItemType.Sample:
                        jtw.WriteValue("sample");
                        jtw.WritePropertyName("name");
                        jtw.WriteValue(datItem.GetField(Field.Name, Header.ExcludeFields));
                        break;
                }

                #region AttractMode

                if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.AltName, Header.ExcludeFields)))
                {
                    jtw.WritePropertyName("alt_romname");
                    jtw.WriteValue(datItem.AltName);
                }
                if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.AltTitle, Header.ExcludeFields)))
                {
                    jtw.WritePropertyName("alt_title");
                    jtw.WriteValue(datItem.AltTitle);
                }

                #endregion

                #region OpenMSX

                if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.Original, Header.ExcludeFields)))
                {
                    jtw.WritePropertyName("original");
                    jtw.WriteValue(datItem.Original.Name);
                }
                if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.OpenMSXSubType, Header.ExcludeFields)))
                {
                    jtw.WritePropertyName("openmsx_subtype");
                    jtw.WriteValue(datItem.OpenMSXSubType);
                }
                if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.OpenMSXType, Header.ExcludeFields)))
                {
                    jtw.WritePropertyName("openmsx_type");
                    jtw.WriteValue(datItem.OpenMSXType);
                }
                if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.Remark, Header.ExcludeFields)))
                {
                    jtw.WritePropertyName("remark");
                    jtw.WriteValue(datItem.Remark);
                }
                if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.Boot, Header.ExcludeFields)))
                {
                    jtw.WritePropertyName("boot");
                    jtw.WriteValue(datItem.Boot);
                }

                #endregion

                #region SoftwareList

                if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.PartName, Header.ExcludeFields)))
                {
                    jtw.WritePropertyName("partname");
                    jtw.WriteValue(datItem.PartName);
                }
                if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.PartInterface, Header.ExcludeFields)))
                {
                    jtw.WritePropertyName("partinterface");
                    jtw.WriteValue(datItem.PartInterface);
                }
                if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.Features, Header.ExcludeFields)))
                {
                    jtw.WritePropertyName("features");
                    jtw.WriteStartArray();
                    foreach (var feature in datItem.Features)
                    {
                        jtw.WriteStartObject();
                        jtw.WritePropertyName(feature.Name);
                        jtw.WriteValue(feature.Value);
                        jtw.WriteEndObject();
                    }

                    jtw.WriteEndArray();
                }
                if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.AreaName, Header.ExcludeFields)))
                {
                    jtw.WritePropertyName("areaname");
                    jtw.WriteValue(datItem.AreaName);
                }
                if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.AreaSize, Header.ExcludeFields)))
                {
                    jtw.WritePropertyName("areasize");
                    jtw.WriteValue(datItem.AreaSize);
                }
                if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.AreaWidth, Header.ExcludeFields)))
                {
                    jtw.WritePropertyName("areawidth");
                    jtw.WriteValue(datItem.AreaWidth);
                }
                if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.AreaEndianness, Header.ExcludeFields)))
                {
                    jtw.WritePropertyName("areaendianness");
                    jtw.WriteValue(datItem.AreaEndianness);
                }
                if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.Value, Header.ExcludeFields)))
                {
                    jtw.WritePropertyName("value");
                    jtw.WriteValue(datItem.Value);
                }
                if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.LoadFlag, Header.ExcludeFields)))
                {
                    jtw.WritePropertyName("loadflag");
                    jtw.WriteValue(datItem.LoadFlag);
                }

                #endregion

                // End item
                jtw.WriteEndObject();

                jtw.Flush();
            }
            catch (Exception ex)
            {
                Globals.Logger.Error(ex.ToString());
                return false;
            }

            return true;
        }

        /// <summary>
        /// Write out DAT footer using the supplied StreamWriter
        /// </summary>
        /// <param name="jtw">JsonTextWriter to output to</param>
        /// <returns>True if the data was written, false on error</returns>
        private bool WriteFooter(JsonTextWriter jtw)
        {
            try
            {
                // End items
                jtw.WriteEndArray();

                // End machine
                jtw.WriteEndObject();

                // End machines
                jtw.WriteEndArray();

                // End file
                jtw.WriteEndObject();

                jtw.Flush();
            }
            catch (Exception ex)
            {
                Globals.Logger.Error(ex.ToString());
                return false;
            }

            return true;
        }
    }
}
