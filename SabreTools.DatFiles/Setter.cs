using System.Collections.Generic;
using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;
using SabreTools.Logging;

namespace SabreTools.DatFiles
{
    /// <summary>
    /// Set fields on DatItems
    /// </summary>
    /// TODO: Figure out how to get this into the Filtering namespace
    public class Setter
    {
        #region Fields

        /// <summary>
        /// Mappings to set DatHeader fields
        /// </summary>
        public Dictionary<DatHeaderField, string>? DatHeaderMappings { get; set; }

        /// <summary>
        /// Mappings to set Machine fields
        /// </summary>
        public Dictionary<MachineField, string>? MachineMappings { get; set; }

        /// <summary>
        /// Mappings to set DatItem fields
        /// </summary>
        public Dictionary<DatItemField, string>? DatItemMappings { get; set; }

        #endregion

        #region Logging

        /// <summary>
        /// Logging object
        /// </summary>
        private readonly Logger logger = new();

        #endregion

        #region Population

        /// <summary>
        /// Populate the setters using a set of field names
        /// </summary>
        /// <param name="headers">List of header names</param>
        /// <param name="fields">List of field names</param>
        public void PopulateSettersFromList(List<string> headers, List<string> fields)
        {
            // Instantiate the setters, if necessary
            DatHeaderMappings ??= new Dictionary<DatHeaderField, string>();
            MachineMappings ??= new Dictionary<MachineField, string>();
            DatItemMappings ??= new Dictionary<DatItemField, string>();

            // If the list is null or empty, just return
            if (fields == null || fields.Count == 0)
                return;

            // Now we loop through and get values for everything
            for (int i = 0; i < headers.Count; i++)
            {
                string field = fields[i];
                DatHeaderField dhf = headers[i].AsDatHeaderField();
                if (dhf != DatHeaderField.NULL)
                {
                    DatHeaderMappings[dhf] = field;
                    continue;
                }

                MachineField mf = headers[i].AsMachineField();
                if (mf != MachineField.NULL)
                {
                    MachineMappings[mf] = field;
                    continue;
                }

                DatItemField dif = headers[i].AsDatItemField();
                if (dif != DatItemField.NULL)
                {
                    DatItemMappings[dif] = field;
                    continue;
                }

                // If we didn't match anything, log an error
                logger.Warning($"The value {field} did not match any known field names. Please check the wiki for more details on supported field names.");
            }
        }

        #endregion

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="datHeader">DatHeader to set fields on</param>
        public void SetFields(DatHeader datHeader)
        {
            if (datHeader == null || DatHeaderMappings == null)
                return;

            if (DatHeaderMappings.ContainsKey(DatHeaderField.Author))
                datHeader.Author = DatHeaderMappings[DatHeaderField.Author];

            if (DatHeaderMappings.ContainsKey(DatHeaderField.BiosMode))
                datHeader.BiosMode = DatHeaderMappings[DatHeaderField.BiosMode].AsMergingFlag();

            if (DatHeaderMappings.ContainsKey(DatHeaderField.Build))
                datHeader.Build = DatHeaderMappings[DatHeaderField.Build];

            // TODO: Support CanOpen

            if (DatHeaderMappings.ContainsKey(DatHeaderField.Category))
                datHeader.Category = DatHeaderMappings[DatHeaderField.Category];

            if (DatHeaderMappings.ContainsKey(DatHeaderField.Comment))
                datHeader.Comment = DatHeaderMappings[DatHeaderField.Comment];

            if (DatHeaderMappings.ContainsKey(DatHeaderField.Date))
                datHeader.Date = DatHeaderMappings[DatHeaderField.Date];

            if (DatHeaderMappings.ContainsKey(DatHeaderField.Debug))
                datHeader.Debug = DatHeaderMappings[DatHeaderField.Debug].AsYesNo();

            if (DatHeaderMappings.ContainsKey(DatHeaderField.Description))
                datHeader.Description = DatHeaderMappings[DatHeaderField.Description];

            if (DatHeaderMappings.ContainsKey(DatHeaderField.Email))
                datHeader.Email = DatHeaderMappings[DatHeaderField.Email];

            if (DatHeaderMappings.ContainsKey(DatHeaderField.FileName))
                datHeader.FileName = DatHeaderMappings[DatHeaderField.FileName];

            if (DatHeaderMappings.ContainsKey(DatHeaderField.ForceMerging))
                datHeader.ForceMerging = DatHeaderMappings[DatHeaderField.ForceMerging].AsMergingFlag();

            if (DatHeaderMappings.ContainsKey(DatHeaderField.ForceNodump))
                datHeader.ForceNodump = DatHeaderMappings[DatHeaderField.ForceNodump].AsNodumpFlag();

            if (DatHeaderMappings.ContainsKey(DatHeaderField.ForcePacking))
                datHeader.ForcePacking = DatHeaderMappings[DatHeaderField.ForcePacking].AsPackingFlag();

            if (DatHeaderMappings.ContainsKey(DatHeaderField.HeaderSkipper))
                datHeader.HeaderSkipper = DatHeaderMappings[DatHeaderField.HeaderSkipper];

            if (DatHeaderMappings.ContainsKey(DatHeaderField.Homepage))
                datHeader.Homepage = DatHeaderMappings[DatHeaderField.Homepage];

            if (DatHeaderMappings.ContainsKey(DatHeaderField.ID))
                datHeader.NoIntroID = DatHeaderMappings[DatHeaderField.ID];

            // TODO: Support Info_Default
            // TODO: Support Info_IsNamingOption
            // TODO: Support Info_Name
            // TODO: Support Info_Visible

            if (DatHeaderMappings.ContainsKey(DatHeaderField.LockBiosMode))
                datHeader.LockBiosMode = DatHeaderMappings[DatHeaderField.LockBiosMode].AsYesNo();

            if (DatHeaderMappings.ContainsKey(DatHeaderField.LockRomMode))
                datHeader.LockRomMode = DatHeaderMappings[DatHeaderField.LockRomMode].AsYesNo();

            if (DatHeaderMappings.ContainsKey(DatHeaderField.LockSampleMode))
                datHeader.LockSampleMode = DatHeaderMappings[DatHeaderField.LockSampleMode].AsYesNo();

            if (DatHeaderMappings.ContainsKey(DatHeaderField.MameConfig))
                datHeader.MameConfig = DatHeaderMappings[DatHeaderField.MameConfig];

            if (DatHeaderMappings.ContainsKey(DatHeaderField.Name))
                datHeader.Name = DatHeaderMappings[DatHeaderField.Name];

            if (DatHeaderMappings.ContainsKey(DatHeaderField.RomCenterVersion))
                datHeader.RomCenterVersion = DatHeaderMappings[DatHeaderField.RomCenterVersion];

            if (DatHeaderMappings.ContainsKey(DatHeaderField.RomMode))
                datHeader.RomMode = DatHeaderMappings[DatHeaderField.RomMode].AsMergingFlag();

            if (DatHeaderMappings.ContainsKey(DatHeaderField.RomTitle))
                datHeader.RomTitle = DatHeaderMappings[DatHeaderField.RomTitle];

            if (DatHeaderMappings.ContainsKey(DatHeaderField.RootDir))
                datHeader.RootDir = DatHeaderMappings[DatHeaderField.RootDir];

            if (DatHeaderMappings.ContainsKey(DatHeaderField.SampleMode))
                datHeader.SampleMode = DatHeaderMappings[DatHeaderField.SampleMode].AsMergingFlag();

            if (DatHeaderMappings.ContainsKey(DatHeaderField.ScreenshotsHeight))
                datHeader.ScreenshotsHeight = DatHeaderMappings[DatHeaderField.ScreenshotsHeight];

            if (DatHeaderMappings.ContainsKey(DatHeaderField.ScreenshotsWidth))
                datHeader.ScreenshotsWidth = DatHeaderMappings[DatHeaderField.ScreenshotsWidth];

            if (DatHeaderMappings.ContainsKey(DatHeaderField.System))
                datHeader.System = DatHeaderMappings[DatHeaderField.System];

            if (DatHeaderMappings.ContainsKey(DatHeaderField.Type))
                datHeader.Type = DatHeaderMappings[DatHeaderField.Type];

            if (DatHeaderMappings.ContainsKey(DatHeaderField.Url))
                datHeader.Url = DatHeaderMappings[DatHeaderField.Url];

            if (DatHeaderMappings.ContainsKey(DatHeaderField.Version))
                datHeader.Version = DatHeaderMappings[DatHeaderField.Version];
        }

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="datItem">DatItem to set fields on</param>
        public void SetFields(DatItem datItem)
        {
            if (datItem == null || DatItemMappings == null)
                return;

            #region Common

            if (DatItemMappings!.ContainsKey(DatItemField.Name))
                datItem.SetName(DatItemMappings[DatItemField.Name]);

            #endregion

            #region Item-Specific
            
            switch (datItem)
            {
                case Adjuster adjuster: SetFields(adjuster); break;
                case Analog analog: SetFields(analog); break;
                case Archive archive: SetFields(archive); break;
                case BiosSet biosSet: SetFields(biosSet); break;
                case Chip chip: SetFields(chip); break;
                case Condition condition: SetFields(condition); break;
                case Configuration condition: SetFields(condition); break;
                case Control control: SetFields(control); break;
                case DataArea dataArea: SetFields(dataArea); break;
                case Device device: SetFields(device); break;
                case DipSwitch dipSwitch: SetFields(dipSwitch); break;
                case Disk disk: SetFields(disk); break;
                case DiskArea diskArea: SetFields(diskArea); break;
                case Display display: SetFields(display); break;
                case Driver driver: SetFields(driver); break;
                case Extension extension: SetFields(extension); break;
                case Feature feature: SetFields(feature); break;
                case Input input: SetFields(input); break;
                case Instance instance: SetFields(instance); break;
                case Location location: SetFields(location); break;
                case Media media: SetFields(media); break;
                case Part part: SetFields(part); break;
                case PartFeature partFeature: SetFields(partFeature); break;
                case Port port: SetFields(port); break;
                case RamOption ramOption: SetFields(ramOption); break;
                case Release release: SetFields(release); break;
                case Rom rom: SetFields(rom); break;
                case Setting setting: SetFields(setting); break;
                case SharedFeature sharedFeat: SetFields(sharedFeat); break;
                case Slot slot: SetFields(slot); break;
                case SlotOption slotOption: SetFields(slotOption); break;
                case SoftwareList softwareList: SetFields(softwareList); break;
                case Sound sound: SetFields(sound); break;
            }

            #endregion
        }

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="machine">Machine to set fields on</param>
        public void SetFields(Machine machine)
        {
            if (machine == null || MachineMappings == null)
                return;

            if (MachineMappings.ContainsKey(MachineField.Board))
                machine.Board = MachineMappings[MachineField.Board];

            if (MachineMappings.ContainsKey(MachineField.Buttons))
                machine.Buttons = MachineMappings[MachineField.Buttons];

            if (MachineMappings.ContainsKey(MachineField.Category))
                machine.Category = MachineMappings[MachineField.Category];

            if (MachineMappings.ContainsKey(MachineField.CloneOf))
                machine.CloneOf = MachineMappings[MachineField.CloneOf];

            if (MachineMappings.ContainsKey(MachineField.CloneOfID))
                machine.NoIntroCloneOfId = MachineMappings[MachineField.CloneOfID];

            if (MachineMappings.ContainsKey(MachineField.Comment))
                machine.Comment = MachineMappings[MachineField.Comment];

            if (MachineMappings.ContainsKey(MachineField.Control))
                machine.Control = MachineMappings[MachineField.Control];

            if (MachineMappings.ContainsKey(MachineField.Country))
                machine.Country = MachineMappings[MachineField.Country];

            if (MachineMappings.ContainsKey(MachineField.CRC))
                machine.Crc = MachineMappings[MachineField.CRC].AsYesNo();

            if (MachineMappings.ContainsKey(MachineField.Description))
                machine.Description = MachineMappings[MachineField.Description];

            if (MachineMappings.ContainsKey(MachineField.Developer))
                machine.Developer = MachineMappings[MachineField.Developer];

            if (MachineMappings.ContainsKey(MachineField.DisplayCount))
                machine.DisplayCount = MachineMappings[MachineField.DisplayCount];

            if (MachineMappings.ContainsKey(MachineField.DisplayType))
                machine.DisplayType = MachineMappings[MachineField.DisplayType];

            if (MachineMappings.ContainsKey(MachineField.Enabled))
                machine.Enabled = MachineMappings[MachineField.Enabled];

            if (MachineMappings.ContainsKey(MachineField.GenMSXID))
                machine.GenMSXID = MachineMappings[MachineField.GenMSXID];

            if (MachineMappings.ContainsKey(MachineField.Genre))
                machine.Genre = MachineMappings[MachineField.Genre];

            if (MachineMappings.ContainsKey(MachineField.History))
                machine.History = MachineMappings[MachineField.History];

            if (MachineMappings.ContainsKey(MachineField.ID))
                machine.NoIntroId = MachineMappings[MachineField.ID];

            if (MachineMappings.ContainsKey(MachineField.Manufacturer))
                machine.Manufacturer = MachineMappings[MachineField.Manufacturer];

            if (MachineMappings.ContainsKey(MachineField.Name))
                machine.Name = MachineMappings[MachineField.Name];

            if (MachineMappings.ContainsKey(MachineField.Players))
                machine.Players = MachineMappings[MachineField.Players];

            if (MachineMappings.ContainsKey(MachineField.Publisher))
                machine.Publisher = MachineMappings[MachineField.Publisher];

            if (MachineMappings.ContainsKey(MachineField.Ratings))
                machine.Ratings = MachineMappings[MachineField.Ratings];

            if (MachineMappings.ContainsKey(MachineField.RebuildTo))
                machine.RebuildTo = MachineMappings[MachineField.RebuildTo];

            if (MachineMappings.ContainsKey(MachineField.RelatedTo))
                machine.RelatedTo = MachineMappings[MachineField.RelatedTo];

            if (MachineMappings.ContainsKey(MachineField.RomOf))
                machine.RomOf = MachineMappings[MachineField.RomOf];

            if (MachineMappings.ContainsKey(MachineField.Rotation))
                machine.Rotation = MachineMappings[MachineField.Rotation];

            if (MachineMappings.ContainsKey(MachineField.Runnable))
                machine.Runnable = MachineMappings[MachineField.Runnable].AsRunnable();

            if (MachineMappings.ContainsKey(MachineField.SampleOf))
                machine.SampleOf = MachineMappings[MachineField.SampleOf];

            if (MachineMappings.ContainsKey(MachineField.Score))
                machine.Score = MachineMappings[MachineField.Score];

            if (MachineMappings.ContainsKey(MachineField.SourceFile))
                machine.SourceFile = MachineMappings[MachineField.SourceFile];

            if (MachineMappings.ContainsKey(MachineField.Status))
                machine.Status = MachineMappings[MachineField.Status];

            if (MachineMappings.ContainsKey(MachineField.Subgenre))
                machine.Subgenre = MachineMappings[MachineField.Subgenre];

            if (MachineMappings.ContainsKey(MachineField.Supported))
                machine.Supported = MachineMappings[MachineField.Supported].AsSupported();

            if (MachineMappings.ContainsKey(MachineField.System))
                machine.System = MachineMappings[MachineField.System];

            if (MachineMappings.ContainsKey(MachineField.TitleID))
                machine.TitleID = MachineMappings[MachineField.TitleID];

            if (MachineMappings.ContainsKey(MachineField.Type))
                machine.MachineType = MachineMappings[MachineField.Type].AsMachineType();

            if (MachineMappings.ContainsKey(MachineField.Year))
                machine.Year = MachineMappings[MachineField.Year];
        }

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="adjuster">Adjuster to remove replace fields in</param>
        private void SetFields(Adjuster adjuster)
        {
            if (DatItemMappings!.ContainsKey(DatItemField.Default))
                adjuster.Default = DatItemMappings[DatItemField.Default].AsYesNo();

            // Field.DatItem_Conditions does not apply here
            if (adjuster.ConditionsSpecified)
            {
                foreach (Condition subCondition in adjuster.Conditions)
                {
                    SetFields(subCondition, true);
                }
            }
        }

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="analog">Analog to remove replace fields in</param>
        private void SetFields(Analog analog)
        {
            if (DatItemMappings!.ContainsKey(DatItemField.Analog_Mask))
                analog.Mask = DatItemMappings[DatItemField.Analog_Mask];
        }

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="archive">Archive to remove replace fields in</param>
        private void SetFields(Archive archive)
        {
            if (DatItemMappings!.ContainsKey(DatItemField.Categories))
                archive.Categories = DatItemMappings[DatItemField.Categories];

            if (DatItemMappings!.ContainsKey(DatItemField.Clone))
                archive.CloneValue = DatItemMappings[DatItemField.Clone];

            if (DatItemMappings!.ContainsKey(DatItemField.Complete))
                archive.Complete = DatItemMappings[DatItemField.Complete];

            if (DatItemMappings!.ContainsKey(DatItemField.DevStatus))
                archive.DevStatus = DatItemMappings[DatItemField.DevStatus];

            if (DatItemMappings!.ContainsKey(DatItemField.Languages))
                archive.Languages = DatItemMappings[DatItemField.Languages];

            if (DatItemMappings!.ContainsKey(DatItemField.Number))
                archive.Number = DatItemMappings[DatItemField.Number];

            if (DatItemMappings!.ContainsKey(DatItemField.Physical))
                archive.Physical = DatItemMappings[DatItemField.Physical];

            if (DatItemMappings!.ContainsKey(DatItemField.Region))
                archive.Region = DatItemMappings[DatItemField.Region];

            if (DatItemMappings!.ContainsKey(DatItemField.RegParent))
                archive.RegParent = DatItemMappings[DatItemField.RegParent];
        }

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="biosSet">BiosSet to remove replace fields in</param>
        private void SetFields(BiosSet biosSet)
        {
            if (DatItemMappings!.ContainsKey(DatItemField.Default))
                biosSet.Default = DatItemMappings[DatItemField.Default].AsYesNo();

            if (DatItemMappings!.ContainsKey(DatItemField.Description))
                biosSet.Description = DatItemMappings[DatItemField.Description];
        }

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="chip">Chip to remove replace fields in</param>
        private void SetFields(Chip chip)
        {
            if (DatItemMappings!.ContainsKey(DatItemField.ChipType))
                chip.ChipType = DatItemMappings[DatItemField.ChipType].AsChipType();

            if (DatItemMappings!.ContainsKey(DatItemField.Clock))
                chip.Clock = Utilities.CleanLong(DatItemMappings[DatItemField.Clock]);

            if (DatItemMappings!.ContainsKey(DatItemField.Tag))
                chip.Tag = DatItemMappings[DatItemField.Tag];
        }

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="condition">Condition to remove replace fields in</param>
        /// <param name="sub">True if this is a subitem, false otherwise</param>
        private void SetFields(Condition condition, bool sub = false)
        {
            if (sub)
            {
                if (DatItemMappings!.ContainsKey(DatItemField.Condition_Mask))
                    condition.Mask = DatItemMappings[DatItemField.Condition_Mask];

                if (DatItemMappings!.ContainsKey(DatItemField.Condition_Relation))
                    condition.Relation = DatItemMappings[DatItemField.Condition_Relation].AsRelation();

                if (DatItemMappings!.ContainsKey(DatItemField.Condition_Tag))
                    condition.Tag = DatItemMappings[DatItemField.Condition_Tag];

                if (DatItemMappings!.ContainsKey(DatItemField.Condition_Value))
                    condition.Value = DatItemMappings[DatItemField.Condition_Value];
            }
            else
            {
                if (DatItemMappings!.ContainsKey(DatItemField.Mask))
                    condition.Mask = DatItemMappings[DatItemField.Mask];

                if (DatItemMappings!.ContainsKey(DatItemField.Relation))
                    condition.Relation = DatItemMappings[DatItemField.Relation].AsRelation();

                if (DatItemMappings!.ContainsKey(DatItemField.Tag))
                    condition.Tag = DatItemMappings[DatItemField.Tag];

                if (DatItemMappings!.ContainsKey(DatItemField.Value))
                    condition.Value = DatItemMappings[DatItemField.Value];
            }
        }

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="configuration">Configuration to remove replace fields in</param>
        private void SetFields(Configuration configuration)
        {
            if (DatItemMappings!.ContainsKey(DatItemField.Mask))
                configuration.Mask = DatItemMappings[DatItemField.Mask];

            if (DatItemMappings!.ContainsKey(DatItemField.Tag))
                configuration.Tag = DatItemMappings[DatItemField.Tag];

            if (configuration.ConditionsSpecified)
            {
                foreach (Condition subCondition in configuration.Conditions)
                {
                    SetFields(subCondition, true);
                }
            }

            if (configuration.LocationsSpecified)
            {
                foreach (Location subLocation in configuration.Locations)
                {
                    SetFields(subLocation);
                }
            }

            if (configuration.SettingsSpecified)
            {
                foreach (Setting subSetting in configuration.Settings)
                {
                    SetFields(subSetting);
                }
            }
        }

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="control">Control to remove replace fields in</param>
        private void SetFields(Control control)
        {
            if (DatItemMappings!.ContainsKey(DatItemField.Control_Buttons))
                control.Buttons = Utilities.CleanLong(DatItemMappings[DatItemField.Control_Buttons]);

            if (DatItemMappings!.ContainsKey(DatItemField.Control_Type))
                control.ControlType = DatItemMappings[DatItemField.Control_Type].AsControlType();

            if (DatItemMappings!.ContainsKey(DatItemField.Control_KeyDelta))
                control.KeyDelta = Utilities.CleanLong(DatItemMappings[DatItemField.Control_KeyDelta]);

            if (DatItemMappings!.ContainsKey(DatItemField.Control_Maximum))
                control.Maximum = Utilities.CleanLong(DatItemMappings[DatItemField.Control_Maximum]);

            if (DatItemMappings!.ContainsKey(DatItemField.Control_Minimum))
                control.Minimum = Utilities.CleanLong(DatItemMappings[DatItemField.Control_Minimum]);

            if (DatItemMappings!.ContainsKey(DatItemField.Control_Player))
                control.Player = Utilities.CleanLong(DatItemMappings[DatItemField.Control_Player]);

            if (DatItemMappings!.ContainsKey(DatItemField.Control_RequiredButtons))
                control.RequiredButtons = Utilities.CleanLong(DatItemMappings[DatItemField.Control_RequiredButtons]);

            if (DatItemMappings!.ContainsKey(DatItemField.Control_Reverse))
                control.Reverse = DatItemMappings[DatItemField.Control_Reverse].AsYesNo();

            if (DatItemMappings!.ContainsKey(DatItemField.Control_Sensitivity))
                control.Sensitivity = Utilities.CleanLong(DatItemMappings[DatItemField.Control_Sensitivity]);

            if (DatItemMappings!.ContainsKey(DatItemField.Control_Ways))
                control.Ways = DatItemMappings[DatItemField.Control_Ways];

            if (DatItemMappings!.ContainsKey(DatItemField.Control_Ways2))
                control.Ways2 = DatItemMappings[DatItemField.Control_Ways2];

            if (DatItemMappings!.ContainsKey(DatItemField.Control_Ways3))
                control.Ways3 = DatItemMappings[DatItemField.Control_Ways3];
        }

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="dataArea">DataArea to remove replace fields in</param>
        private void SetFields(DataArea dataArea)
        {
            if (DatItemMappings!.ContainsKey(DatItemField.AreaEndianness))
                dataArea.Endianness = DatItemMappings[DatItemField.AreaEndianness].AsEndianness();

            if (DatItemMappings!.ContainsKey(DatItemField.AreaName))
                dataArea.Name = DatItemMappings[DatItemField.AreaName];

            if (DatItemMappings!.ContainsKey(DatItemField.AreaSize))
                dataArea.Size = Utilities.CleanLong(DatItemMappings[DatItemField.AreaSize]);

            if (DatItemMappings!.ContainsKey(DatItemField.AreaWidth))
                dataArea.Width = Utilities.CleanLong(DatItemMappings[DatItemField.AreaWidth]);
        }

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="device">Device to remove replace fields in</param>
        private void SetFields(Device device)
        {
            if (DatItemMappings!.ContainsKey(DatItemField.DeviceType))
                device.DeviceType = DatItemMappings[DatItemField.DeviceType].AsDeviceType();

            if (DatItemMappings!.ContainsKey(DatItemField.FixedImage))
                device.FixedImage = DatItemMappings[DatItemField.FixedImage];

            if (DatItemMappings!.ContainsKey(DatItemField.Interface))
                device.Interface = DatItemMappings[DatItemField.Interface];

            if (DatItemMappings!.ContainsKey(DatItemField.Mandatory))
                device.Mandatory = Utilities.CleanLong(DatItemMappings[DatItemField.Mandatory]);

            if (DatItemMappings!.ContainsKey(DatItemField.Tag))
                device.Tag = DatItemMappings[DatItemField.Tag];

            if (device.ExtensionsSpecified)
            {
                foreach (Extension subExtension in device.Extensions)
                {
                    SetFields(subExtension);
                }
            }

            if (device.InstancesSpecified)
            {
                foreach (Instance subInstance in device.Instances)
                {
                    SetFields(subInstance);
                }
            }
        }

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="dipSwitch">DipSwitch to remove replace fields in</param>
        private void SetFields(DipSwitch dipSwitch)
        {
            if (DatItemMappings!.ContainsKey(DatItemField.Mask))
                dipSwitch.Mask = DatItemMappings[DatItemField.Mask];

            if (DatItemMappings!.ContainsKey(DatItemField.Tag))
                dipSwitch.Tag = DatItemMappings[DatItemField.Tag];

            if (dipSwitch.ConditionsSpecified)
            {
                foreach (Condition subCondition in dipSwitch.Conditions)
                {
                    SetFields(subCondition, true);
                }
            }

            if (dipSwitch.LocationsSpecified)
            {
                foreach (Location subLocation in dipSwitch.Locations)
                {
                    SetFields(subLocation);
                }
            }

            if (dipSwitch.ValuesSpecified)
            {
                foreach (Setting subValue in dipSwitch.Values)
                {
                    SetFields(subValue);
                }
            }

            dipSwitch.Part ??= new Part();
            SetFields(dipSwitch.Part);
        }

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="disk">Disk to remove replace fields in</param>
        private void SetFields(Disk disk)
        {
            if (DatItemMappings!.ContainsKey(DatItemField.Index))
                disk.Index = DatItemMappings[DatItemField.Index];

            if (DatItemMappings!.ContainsKey(DatItemField.MD5))
                disk.MD5 = DatItemMappings[DatItemField.MD5];

            if (DatItemMappings!.ContainsKey(DatItemField.Merge))
                disk.MergeTag = DatItemMappings[DatItemField.Merge];

            if (DatItemMappings!.ContainsKey(DatItemField.Optional))
                disk.Optional = DatItemMappings[DatItemField.Optional].AsYesNo();

            if (DatItemMappings!.ContainsKey(DatItemField.Region))
                disk.Region = DatItemMappings[DatItemField.Region];

            if (DatItemMappings!.ContainsKey(DatItemField.SHA1))
                disk.SHA1 = DatItemMappings[DatItemField.SHA1];

            if (DatItemMappings!.ContainsKey(DatItemField.Status))
                disk.ItemStatus = DatItemMappings[DatItemField.Status].AsItemStatus();

            if (DatItemMappings!.ContainsKey(DatItemField.Writable))
                disk.Writable = DatItemMappings[DatItemField.Writable].AsYesNo();

            disk.DiskArea ??= new DiskArea();
            SetFields(disk.DiskArea);

            disk.Part ??= new Part();
            SetFields(disk.Part);
        }

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="diskArea">DiskArea to remove replace fields in</param>
        private void SetFields(DiskArea diskArea)
        {
            if (DatItemMappings!.ContainsKey(DatItemField.AreaName))
                diskArea.Name = DatItemMappings[DatItemField.AreaName];
        }

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="display">Display to remove replace fields in</param>
        private void SetFields(Display display)
        {
            if (DatItemMappings!.ContainsKey(DatItemField.DisplayType))
                display.DisplayType = DatItemMappings[DatItemField.DisplayType].AsDisplayType();

            if (DatItemMappings!.ContainsKey(DatItemField.FlipX))
                display.FlipX = DatItemMappings[DatItemField.FlipX].AsYesNo();

            if (DatItemMappings!.ContainsKey(DatItemField.Height))
                display.Height = Utilities.CleanLong(DatItemMappings[DatItemField.Height]);

            if (DatItemMappings!.ContainsKey(DatItemField.HBEnd))
                display.HBEnd = Utilities.CleanLong(DatItemMappings[DatItemField.HBEnd]);

            if (DatItemMappings!.ContainsKey(DatItemField.HBStart))
                display.HBStart = Utilities.CleanLong(DatItemMappings[DatItemField.HBStart]);

            if (DatItemMappings!.ContainsKey(DatItemField.HTotal))
                display.HTotal = Utilities.CleanLong(DatItemMappings[DatItemField.HTotal]);

            if (DatItemMappings!.ContainsKey(DatItemField.PixClock))
                display.PixClock = Utilities.CleanLong(DatItemMappings[DatItemField.PixClock]);

            if (DatItemMappings!.ContainsKey(DatItemField.Refresh))
                display.Refresh = Utilities.CleanDouble(DatItemMappings[DatItemField.Refresh]);

            if (DatItemMappings!.ContainsKey(DatItemField.Rotate))
                display.Rotate = Utilities.CleanLong(DatItemMappings[DatItemField.Rotate]);

            if (DatItemMappings!.ContainsKey(DatItemField.Tag))
                display.Tag = DatItemMappings[DatItemField.Tag];

            if (DatItemMappings!.ContainsKey(DatItemField.VBEnd))
                display.VBEnd = Utilities.CleanLong(DatItemMappings[DatItemField.VBEnd]);

            if (DatItemMappings!.ContainsKey(DatItemField.VBStart))
                display.VBStart = Utilities.CleanLong(DatItemMappings[DatItemField.VBStart]);

            if (DatItemMappings!.ContainsKey(DatItemField.VTotal))
                display.VTotal = Utilities.CleanLong(DatItemMappings[DatItemField.VTotal]);

            if (DatItemMappings!.ContainsKey(DatItemField.Width))
                display.Width = Utilities.CleanLong(DatItemMappings[DatItemField.Width]);
        }

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="driver">Driver to remove replace fields in</param>
        private void SetFields(Driver driver)
        {
            if (DatItemMappings!.ContainsKey(DatItemField.CocktailStatus))
                driver.Cocktail = DatItemMappings[DatItemField.CocktailStatus].AsSupportStatus();

            if (DatItemMappings!.ContainsKey(DatItemField.Incomplete))
                driver.Incomplete = DatItemMappings[DatItemField.Incomplete].AsYesNo();

            if (DatItemMappings!.ContainsKey(DatItemField.EmulationStatus))
                driver.Emulation = DatItemMappings[DatItemField.EmulationStatus].AsSupportStatus();

            if (DatItemMappings!.ContainsKey(DatItemField.NoSoundHardware))
                driver.NoSoundHardware = DatItemMappings[DatItemField.NoSoundHardware].AsYesNo();

            if (DatItemMappings!.ContainsKey(DatItemField.RequiresArtwork))
                driver.RequiresArtwork = DatItemMappings[DatItemField.RequiresArtwork].AsYesNo();

            if (DatItemMappings!.ContainsKey(DatItemField.SaveStateStatus))
                driver.SaveState = DatItemMappings[DatItemField.SaveStateStatus].AsSupported();

            if (DatItemMappings!.ContainsKey(DatItemField.SupportStatus))
                driver.Status = DatItemMappings[DatItemField.SupportStatus].AsSupportStatus();

            if (DatItemMappings!.ContainsKey(DatItemField.Unofficial))
                driver.Unofficial = DatItemMappings[DatItemField.Unofficial].AsYesNo();
        }

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="extension">Extension to remove replace fields in</param>
        private void SetFields(Extension extension)
        {
            if (DatItemMappings!.ContainsKey(DatItemField.Extension_Name))
                extension.Name = DatItemMappings[DatItemField.Extension_Name];
        }

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="feature">Feature to remove replace fields in</param>
        private void SetFields(Feature feature)
        {
            if (DatItemMappings!.ContainsKey(DatItemField.FeatureOverall))
                feature.Overall = DatItemMappings[DatItemField.FeatureOverall].AsFeatureStatus();

            if (DatItemMappings!.ContainsKey(DatItemField.FeatureStatus))
                feature.Status = DatItemMappings[DatItemField.FeatureStatus].AsFeatureStatus();

            if (DatItemMappings!.ContainsKey(DatItemField.FeatureType))
                feature.Type = DatItemMappings[DatItemField.FeatureType].AsFeatureType();
        }

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="info">Info to remove replace fields in</param>
        private void SetFields(Info info)
        {
            if (DatItemMappings!.ContainsKey(DatItemField.Value))
                info.Value = DatItemMappings[DatItemField.Value];
        }

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="input">Input to remove replace fields in</param>
        private void SetFields(Input input)
        {
            if (DatItemMappings!.ContainsKey(DatItemField.Coins))
                input.Coins = Utilities.CleanLong(DatItemMappings[DatItemField.Coins]);

            if (DatItemMappings!.ContainsKey(DatItemField.Players))
                input.Players = Utilities.CleanLong(DatItemMappings[DatItemField.Players]);

            if (DatItemMappings!.ContainsKey(DatItemField.Service))
                input.Service = DatItemMappings[DatItemField.Service].AsYesNo();

            if (DatItemMappings!.ContainsKey(DatItemField.Tilt))
                input.Tilt = DatItemMappings[DatItemField.Tilt].AsYesNo();

            if (input.ControlsSpecified)
            {
                foreach (Control subControl in input.Controls)
                {
                    SetFields(subControl);
                }
            }
        }

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="instance">Instance to remove replace fields in</param>
        private void SetFields(Instance instance)
        {
            if (DatItemMappings!.ContainsKey(DatItemField.Instance_BriefName))
                instance.BriefName = DatItemMappings[DatItemField.Instance_BriefName];

            if (DatItemMappings!.ContainsKey(DatItemField.Instance_Name))
                instance.BriefName = DatItemMappings[DatItemField.Instance_Name];
        }

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="location">Location to remove replace fields in</param>
        private void SetFields(Location location)
        {
            if (DatItemMappings!.ContainsKey(DatItemField.Location_Inverted))
                location.Inverted = DatItemMappings[DatItemField.Location_Inverted].AsYesNo();

            if (DatItemMappings!.ContainsKey(DatItemField.Location_Name))
                location.Name = DatItemMappings[DatItemField.Location_Name];

            if (DatItemMappings!.ContainsKey(DatItemField.Location_Number))
                location.Number = Utilities.CleanLong(DatItemMappings[DatItemField.Location_Number]);
        }

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="media">Media to remove replace fields in</param>
        private void SetFields(Media media)
        {
            if (DatItemMappings!.ContainsKey(DatItemField.MD5))
                media.MD5 = DatItemMappings[DatItemField.MD5];

            if (DatItemMappings!.ContainsKey(DatItemField.SHA1))
                media.SHA1 = DatItemMappings[DatItemField.SHA1];

            if (DatItemMappings!.ContainsKey(DatItemField.SHA256))
                media.SHA256 = DatItemMappings[DatItemField.SHA256];

            if (DatItemMappings!.ContainsKey(DatItemField.SpamSum))
                media.SpamSum = DatItemMappings[DatItemField.SpamSum];
        }

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="part">Part to remove replace fields in</param>
        private void SetFields(Part part)
        {
            if (DatItemMappings!.ContainsKey(DatItemField.Part_Interface))
                part.Interface = DatItemMappings[DatItemField.Part_Interface];

            if (DatItemMappings!.ContainsKey(DatItemField.Part_Name))
                part.Name = DatItemMappings[DatItemField.Part_Name];

            if (part.FeaturesSpecified)
            {
                foreach (PartFeature subPartFeature in part.Features)
                {
                    SetFields(subPartFeature);
                }
            }
        }

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="partFeature">PartFeature to remove replace fields in</param>
        private void SetFields(PartFeature partFeature)
        {
            if (DatItemMappings!.ContainsKey(DatItemField.Part_Feature_Name))
                partFeature.Name = DatItemMappings[DatItemField.Part_Feature_Name];

            if (DatItemMappings!.ContainsKey(DatItemField.Part_Feature_Value))
                partFeature.Value = DatItemMappings[DatItemField.Part_Feature_Value];
        }

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="port">Port to remove replace fields in</param>
        private void SetFields(Port port)
        {
            if (DatItemMappings!.ContainsKey(DatItemField.Tag))
                port.Tag = DatItemMappings[DatItemField.Tag];

            if (port.AnalogsSpecified)
            {
                foreach (Analog subAnalog in port.Analogs)
                {
                    SetFields(subAnalog);
                }
            }
        }

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="ramOption">RamOption to remove replace fields in</param>
        private void SetFields(RamOption ramOption)
        {
            if (DatItemMappings!.ContainsKey(DatItemField.Content))
                ramOption.Content = DatItemMappings[DatItemField.Content];

            if (DatItemMappings!.ContainsKey(DatItemField.Default))
                ramOption.Default = DatItemMappings[DatItemField.Default].AsYesNo();
        }

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="release">Release to remove replace fields in</param>
        private void SetFields(Release release)
        {
            if (DatItemMappings!.ContainsKey(DatItemField.Date))
                release.Date = DatItemMappings[DatItemField.Date];

            if (DatItemMappings!.ContainsKey(DatItemField.Default))
                release.Default = DatItemMappings[DatItemField.Default].AsYesNo();

            if (DatItemMappings!.ContainsKey(DatItemField.Language))
                release.Language = DatItemMappings[DatItemField.Language];

            if (DatItemMappings!.ContainsKey(DatItemField.Region))
                release.Region = DatItemMappings[DatItemField.Region];
        }

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="rom">Rom to remove replace fields in</param>
        private void SetFields(Rom rom)
        {
            if (DatItemMappings!.ContainsKey(DatItemField.AltName))
                rom.AltName = DatItemMappings[DatItemField.AltName];

            if (DatItemMappings!.ContainsKey(DatItemField.AltTitle))
                rom.AltTitle = DatItemMappings[DatItemField.AltTitle];

            if (DatItemMappings!.ContainsKey(DatItemField.ArchiveDotOrgFormat))
                rom.ArchiveDotOrgFormat = DatItemMappings[DatItemField.ArchiveDotOrgFormat];

            if (DatItemMappings!.ContainsKey(DatItemField.ArchiveDotOrgSource))
                rom.ArchiveDotOrgSource = DatItemMappings[DatItemField.ArchiveDotOrgSource];

            if (DatItemMappings!.ContainsKey(DatItemField.Bios))
                rom.Bios = DatItemMappings[DatItemField.Bios];

            if (DatItemMappings!.ContainsKey(DatItemField.Boot))
                rom.Boot = DatItemMappings[DatItemField.Boot];

            if (DatItemMappings!.ContainsKey(DatItemField.CRC))
                rom.CRC = DatItemMappings[DatItemField.CRC];

            if (DatItemMappings!.ContainsKey(DatItemField.Date))
                rom.Date = DatItemMappings[DatItemField.Date];

            if (DatItemMappings!.ContainsKey(DatItemField.Inverted))
                rom.Inverted = DatItemMappings[DatItemField.Inverted].AsYesNo();

            if (DatItemMappings!.ContainsKey(DatItemField.LoadFlag))
                rom.LoadFlag = DatItemMappings[DatItemField.LoadFlag].AsLoadFlag();

            if (DatItemMappings!.ContainsKey(DatItemField.MD5))
                rom.MD5 = DatItemMappings[DatItemField.MD5];

            if (DatItemMappings!.ContainsKey(DatItemField.Merge))
                rom.MergeTag = DatItemMappings[DatItemField.Merge];

            if (DatItemMappings!.ContainsKey(DatItemField.MIA))
                rom.MIA = DatItemMappings[DatItemField.MIA].AsYesNo();

            if (DatItemMappings!.ContainsKey(DatItemField.Offset))
                rom.Offset = DatItemMappings[DatItemField.Offset];

            if (DatItemMappings!.ContainsKey(DatItemField.OpenMSXSubType))
                rom.OpenMSXSubType = DatItemMappings[DatItemField.OpenMSXSubType].AsOpenMSXSubType();

            if (DatItemMappings!.ContainsKey(DatItemField.OpenMSXType))
                rom.OpenMSXType = DatItemMappings[DatItemField.OpenMSXType];

            if (DatItemMappings!.ContainsKey(DatItemField.Optional))
                rom.Optional = DatItemMappings[DatItemField.Optional].AsYesNo();

            if (DatItemMappings!.ContainsKey(DatItemField.Original))
                rom.Original = new Original() { Content = DatItemMappings[DatItemField.Original] };

            if (DatItemMappings!.ContainsKey(DatItemField.OriginalFilename))
                rom.OriginalFilename = DatItemMappings[DatItemField.OriginalFilename];

            if (DatItemMappings!.ContainsKey(DatItemField.Region))
                rom.Region = DatItemMappings[DatItemField.Region];

            if (DatItemMappings!.ContainsKey(DatItemField.Remark))
                rom.Remark = DatItemMappings[DatItemField.Remark];

            if (DatItemMappings!.ContainsKey(DatItemField.Rotation))
                rom.Rotation = DatItemMappings[DatItemField.Rotation];

            if (DatItemMappings!.ContainsKey(DatItemField.SHA1))
                rom.SHA1 = DatItemMappings[DatItemField.SHA1];

            if (DatItemMappings!.ContainsKey(DatItemField.SHA256))
                rom.SHA256 = DatItemMappings[DatItemField.SHA256];

            if (DatItemMappings!.ContainsKey(DatItemField.SHA384))
                rom.SHA384 = DatItemMappings[DatItemField.SHA384];

            if (DatItemMappings!.ContainsKey(DatItemField.SHA512))
                rom.SHA512 = DatItemMappings[DatItemField.SHA512];

            if (DatItemMappings!.ContainsKey(DatItemField.Size))
                rom.Size = Utilities.CleanLong(DatItemMappings[DatItemField.Size]);

            if (DatItemMappings!.ContainsKey(DatItemField.SpamSum))
                rom.SpamSum = DatItemMappings[DatItemField.SpamSum];

            if (DatItemMappings!.ContainsKey(DatItemField.Status))
                rom.ItemStatus = DatItemMappings[DatItemField.Status].AsItemStatus();

            if (DatItemMappings!.ContainsKey(DatItemField.Summation))
                rom.Summation = DatItemMappings[DatItemField.Summation];

            if (DatItemMappings!.ContainsKey(DatItemField.Value))
                rom.Value = DatItemMappings[DatItemField.Value];

            rom.DataArea ??= new DataArea();
            SetFields(rom.DataArea);

            rom.Part ??= new Part();
            SetFields(rom.Part);
        }

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="setting">Setting to remove replace fields in</param>
        private void SetFields(Setting setting)
        {
            if (DatItemMappings!.ContainsKey(DatItemField.Setting_Default))
                setting.Default = DatItemMappings[DatItemField.Setting_Default].AsYesNo();

            if (DatItemMappings!.ContainsKey(DatItemField.Setting_Name))
                setting.Name = DatItemMappings[DatItemField.Setting_Name];

            if (DatItemMappings!.ContainsKey(DatItemField.Setting_Value))
                setting.Value = DatItemMappings[DatItemField.Setting_Value];

            if (setting.ConditionsSpecified)
            {
                foreach (Condition subCondition in setting.Conditions)
                {
                    SetFields(subCondition, true);
                }
            }
        }

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="sharedFeature">SharedFeature to remove replace fields in</param>
        private void SetFields(SharedFeature sharedFeature)
        {
            if (DatItemMappings!.ContainsKey(DatItemField.Value))
                sharedFeature.Value = DatItemMappings[DatItemField.Value];
        }

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="slot">Slot to remove replace fields in</param>
        private void SetFields(Slot slot)
        {
            if (slot.SlotOptionsSpecified)
            {
                foreach (SlotOption subSlotOption in slot.SlotOptions)
                {
                    SetFields(subSlotOption);
                }
            }
        }

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="slotOption">SlotOption to remove replace fields in</param>
        private void SetFields(SlotOption slotOption)
        {
            if (DatItemMappings!.ContainsKey(DatItemField.SlotOption_Default))
                slotOption.Default = DatItemMappings[DatItemField.SlotOption_Default].AsYesNo();

            if (DatItemMappings!.ContainsKey(DatItemField.SlotOption_DeviceName))
                slotOption.DeviceName = DatItemMappings[DatItemField.SlotOption_DeviceName];

            if (DatItemMappings!.ContainsKey(DatItemField.SlotOption_Name))
                slotOption.Name = DatItemMappings[DatItemField.SlotOption_Name];
        }

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="softwareList">SoftwareList to remove replace fields in</param>
        private void SetFields(SoftwareList softwareList)
        {
            if (DatItemMappings!.ContainsKey(DatItemField.Filter))
                softwareList.Filter = DatItemMappings[DatItemField.Filter];

            if (DatItemMappings!.ContainsKey(DatItemField.SoftwareListStatus))
                softwareList.Status = DatItemMappings[DatItemField.SoftwareListStatus].AsSoftwareListStatus();

            if (DatItemMappings!.ContainsKey(DatItemField.Tag))
                softwareList.Tag = DatItemMappings[DatItemField.Tag];
        }

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="sound">Sound to remove replace fields in</param>
        private void SetFields(Sound sound)
        {
            if (DatItemMappings!.ContainsKey(DatItemField.Channels))
                sound.Channels = Utilities.CleanLong(DatItemMappings[DatItemField.Channels]);
        }
    }
}