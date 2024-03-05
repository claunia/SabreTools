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
            DatHeaderMappings ??= [];
            MachineMappings ??= [];
            DatItemMappings ??= [];

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

            // Handle unnested sets first
            foreach (var kvp in DatItemMappings)
            {
                datItem.SetField(kvp.Key, kvp.Value);
            }
            
            // Handle nested sets
            switch (datItem)
            {
                case Adjuster adjuster: SetFields(adjuster); break;
                case Configuration condition: SetFields(condition); break;
                case ConfSetting confSetting: SetFields(confSetting); break;
                case Device device: SetFields(device); break;
                case DipSwitch dipSwitch: SetFields(dipSwitch); break;
                case DipValue dipValue: SetFields(dipValue); break;
                case Disk disk: SetFields(disk); break;
                case Input input: SetFields(input); break;
                case Part part: SetFields(part); break;
                case Port port: SetFields(port); break;
                case Rom rom: SetFields(rom); break;
                case Slot slot: SetFields(slot); break;
            }

            #endregion
        }

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="machine">Machine to set fields on</param>
        public void SetFields(Machine? machine)
        {
            if (machine == null || MachineMappings == null)
                return;

            foreach (var kvp in MachineMappings)
            {
                machine.SetField(kvp.Key, kvp.Value);
            }
        }

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="adjuster">Adjuster to remove replace fields in</param>
        private void SetFields(Adjuster adjuster)
        {
            // Field.DatItem_Conditions does not apply here
            if (adjuster.ConditionsSpecified)
            {
                foreach (Condition subCondition in adjuster.Conditions!)
                {
                    SetFields(subCondition);
                }
            }
        }

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="configuration">Configuration to remove replace fields in</param>
        private void SetFields(Configuration configuration)
        {
            if (configuration.ConditionsSpecified)
            {
                foreach (Condition subCondition in configuration.Conditions!)
                {
                    SetFields(subCondition);
                }
            }

            if (configuration.LocationsSpecified)
            {
                foreach (ConfLocation subLocation in configuration.Locations!)
                {
                    SetFields(subLocation);
                }
            }

            if (configuration.SettingsSpecified)
            {
                foreach (ConfSetting subSetting in configuration.Settings!)
                {
                    SetFields(subSetting as DatItem);
                }
            }
        }

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="confSetting">ConfSetting to remove replace fields in</param>
        private void SetFields(ConfSetting confSetting)
        {
            if (confSetting.ConditionsSpecified)
            {
                foreach (Condition subCondition in confSetting.Conditions!)
                {
                    SetFields(subCondition);
                }
            }
        }

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="device">Device to remove replace fields in</param>
        private void SetFields(Device device)
        {
            if (device.ExtensionsSpecified)
            {
                foreach (Extension subExtension in device.Extensions!)
                {
                    SetFields(subExtension);
                }
            }

            if (device.InstancesSpecified)
            {
                foreach (Instance subInstance in device.Instances!)
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
            if (dipSwitch.ConditionsSpecified)
            {
                foreach (Condition subCondition in dipSwitch.Conditions!)
                {
                    SetFields(subCondition);
                }
            }

            if (dipSwitch.LocationsSpecified)
            {
                foreach (DipLocation subLocation in dipSwitch.Locations!)
                {
                    SetFields(subLocation);
                }
            }

            if (dipSwitch.ValuesSpecified)
            {
                foreach (DipValue subValue in dipSwitch.Values!)
                {
                    SetFields(subValue as DatItem);
                }
            }

            dipSwitch.Part ??= new Part();
            SetFields(dipSwitch.Part as DatItem);
        }

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="dipValue">DipValue to remove replace fields in</param>
        private void SetFields(DipValue dipValue)
        {
            if (dipValue.ConditionsSpecified)
            {
                foreach (Condition subCondition in dipValue.Conditions!)
                {
                    SetFields(subCondition);
                }
            }
        }

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="disk">Disk to remove replace fields in</param>
        private void SetFields(Disk disk)
        {
            disk.DiskArea ??= new DiskArea();
            SetFields(disk.DiskArea);

            disk.Part ??= new Part();
            SetFields(disk.Part as DatItem);
        }

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="input">Input to remove replace fields in</param>
        private void SetFields(Input input)
        {
            if (input.ControlsSpecified)
            {
                foreach (Control subControl in input.Controls!)
                {
                    SetFields(subControl);
                }
            }
        }

        /// <summary>s
        /// Set fields with given values
        /// </summary>
        /// <param name="part">Part to remove replace fields in</param>
        private void SetFields(Part part)
        {
            if (part.FeaturesSpecified)
            {
                foreach (PartFeature subPartFeature in part.Features!)
                {
                    SetFields(subPartFeature);
                }
            }
        }

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="port">Port to remove replace fields in</param>
        private void SetFields(Port port)
        {
            if (port.AnalogsSpecified)
            {
                foreach (Analog subAnalog in port.Analogs!)
                {
                    SetFields(subAnalog);
                }
            }
        }

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="rom">Rom to remove replace fields in</param>
        private void SetFields(Rom rom)
        {
            rom.DataArea ??= new DataArea();
            SetFields(rom.DataArea);

            rom.Part ??= new Part();
            SetFields(rom.Part as DatItem);
        }

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="slot">Slot to remove replace fields in</param>
        private void SetFields(Slot slot)
        {
            if (slot.SlotOptionsSpecified)
            {
                foreach (SlotOption subSlotOption in slot.SlotOptions!)
                {
                    SetFields(subSlotOption);
                }
            }
        }
    }
}