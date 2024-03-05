using System.Collections.Generic;
using System.Linq;
using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;

namespace SabreTools.Filtering
{
    /// <summary>
    /// Represents the removal operations that need to be performed on a set of items, usually a DAT
    /// </summary>
    public class DatItemRemover
    {
        #region Fields

        /// <summary>
        /// List of Machine fields to exclude from writing
        /// </summary>
        public List<MachineField> MachineFields { get; private set; } = new List<MachineField>();

        /// <summary>
        /// List of DatItem fields to exclude from writing
        /// </summary>
        public List<DatItemField> DatItemFields { get; private set; } = new List<DatItemField>();

        #endregion

        #region Population

        /// <summary>
        /// Set remover from a value
        /// </summary>
        /// <param name="field">Key for the remover to be set</param>
        public bool SetRemover(string field)
        {
            // If the key is null or empty, return false
            if (string.IsNullOrEmpty(field))
                return false;

            // If we have a Machine field
            MachineField machineField = field.AsMachineField();
            if (machineField != MachineField.NULL)
            {
                MachineFields.Add(machineField);
                return true;
            }

            // If we have a DatItem field
            DatItemField datItemField = field.AsDatItemField();
            if (datItemField != DatItemField.NULL)
            {
                DatItemFields.Add(datItemField);
                return true;
            }

            return false;
        }

        #endregion

        #region Running

        /// <summary>
        /// Remove fields with given values
        /// </summary>
        /// <param name="datItem">DatItem to remove fields from</param>
        public void RemoveFields(DatItem? datItem)
        {
            if (datItem == null)
                return;

            #region Common

            if (MachineFields != null && MachineFields.Any() && datItem.Machine != null)
                RemoveFields(datItem.Machine);

            if (DatItemFields == null || !DatItemFields.Any())
                return;

            if (DatItemFields.Contains(DatItemField.Name))
                datItem.SetName(null);

            #endregion

            #region Item-Specific

            // Handle unnested removals first
            foreach (var datItemField in DatItemFields)
            {
                datItem.RemoveField(datItemField);
            }

            // Handle nested removals
            switch (datItem)
            {
                case Adjuster adjuster: RemoveFields(adjuster); break;
                case Configuration configuration: RemoveFields(configuration); break;
                case ConfSetting confSetting: RemoveFields(confSetting); break;
                case Device device: RemoveFields(device); break;
                case DipSwitch dipSwitch: RemoveFields(dipSwitch); break;
                case DipValue dipValue: RemoveFields(dipValue); break;
                case Disk disk: RemoveFields(disk); break;
                case Input input: RemoveFields(input); break;
                case Part part: RemoveFields(part); break;
                case Port port: RemoveFields(port); break;
                case Rom rom: RemoveFields(rom); break;
                case Slot slot: RemoveFields(slot); break;
            }

            #endregion
        }

        /// <summary>
        /// Remove fields with given values
        /// </summary>
        /// <param name="machine">Machine to remove fields from</param>
        private void RemoveFields(Machine? machine)
        {
            if (machine == null)
                return;

            foreach (var machineField in MachineFields)
            {
                machine.RemoveField(machineField);
            }
        }

        /// <summary>
        /// Remove fields with given values
        /// </summary>
        /// <param name="adjuster">Adjuster to remove fields from</param>
        private void RemoveFields(Adjuster adjuster)
        {
            if (!adjuster.ConditionsSpecified)
                return;

            foreach (Condition subCondition in adjuster.Conditions!)
            {
                RemoveFields(subCondition);
            }
        }

        /// <summary>
        /// Remove fields with given values
        /// </summary>
        /// <param name="configuration">Configuration to remove fields from</param>
        private void RemoveFields(Configuration configuration)
        {
            if (configuration.ConditionsSpecified)
            {
                foreach (Condition subCondition in configuration.Conditions!)
                {
                    RemoveFields(subCondition);
                }
            }

            if (configuration.LocationsSpecified)
            {
                foreach (ConfLocation subLocation in configuration.Locations!)
                {
                    RemoveFields(subLocation);
                }
            }

            if (configuration.SettingsSpecified)
            {
                foreach (ConfSetting subSetting in configuration.Settings!)
                {
                    RemoveFields(subSetting as DatItem);
                }
            }
        }

        /// <summary>
        /// Remove fields with given values
        /// </summary>
        /// <param name="confsetting">ConfSetting to remove fields from</param>
        private void RemoveFields(ConfSetting confsetting)
        {
            if (confsetting.ConditionsSpecified)
            {
                foreach (Condition subCondition in confsetting.Conditions!)
                {
                    RemoveFields(subCondition);
                }
            }
        }

        /// <summary>
        /// Remove fields with given values
        /// </summary>
        /// <param name="device">Device to remove fields from</param>
        private void RemoveFields(Device device)
        {
            if (device.ExtensionsSpecified)
            {
                foreach (Extension subExtension in device.Extensions!)
                {
                    RemoveFields(subExtension);
                }
            }

            if (device.InstancesSpecified)
            {
                foreach (Instance subInstance in device.Instances!)
                {
                    RemoveFields(subInstance);
                }
            }
        }

        /// <summary>
        /// Remove fields with given values
        /// </summary>
        /// <param name="dipSwitch">DipSwitch to remove fields from</param>
        private void RemoveFields(DipSwitch dipSwitch)
        {
            if (dipSwitch.ConditionsSpecified)
            {
                foreach (Condition subCondition in dipSwitch.Conditions!)
                {
                    RemoveFields(subCondition);
                }
            }

            if (dipSwitch.LocationsSpecified)
            {
                foreach (DipLocation subLocation in dipSwitch.Locations!)
                {
                    RemoveFields(subLocation);
                }
            }

            if (dipSwitch.ValuesSpecified)
            {
                foreach (DipValue subValue in dipSwitch.Values!)
                {
                    RemoveFields(subValue as DatItem);
                }
            }

            if (dipSwitch.PartSpecified)
                RemoveFields(dipSwitch.Part! as DatItem);
        }

        /// <summary>
        /// Remove fields with given values
        /// </summary>
        /// <param name="dipValue">DipValue to remove fields from</param>
        private void RemoveFields(DipValue dipValue)
        {
            if (dipValue.ConditionsSpecified)
            {
                foreach (Condition subCondition in dipValue.Conditions!)
                {
                    RemoveFields(subCondition);
                }
            }
        }

        /// <summary>
        /// Remove fields with given values
        /// </summary>
        /// <param name="disk">Disk to remove fields from</param>
        private void RemoveFields(Disk disk)
        {
            if (disk.DiskAreaSpecified)
                RemoveFields(disk.DiskArea);

            if (disk.PartSpecified)
                RemoveFields(disk.Part! as DatItem);
        }

        /// <summary>
        /// Remove fields with given values
        /// </summary>
        /// <param name="input">Input to remove fields from</param>
        private void RemoveFields(Input input)
        {
            if (input.ControlsSpecified)
            {
                foreach (Control subControl in input.Controls!)
                {
                    RemoveFields(subControl);
                }
            }
        }

        /// <summary>
        /// Remove fields with given values
        /// </summary>
        /// <param name="part">Part to remove fields from</param>
        private void RemoveFields(Part part)
        {
            if (part.FeaturesSpecified)
            {
                foreach (PartFeature subPartFeature in part.Features!)
                {
                    RemoveFields(subPartFeature);
                }
            }
        }

        /// <summary>
        /// Remove fields with given values
        /// </summary>
        /// <param name="port">Port to remove fields from</param>
        private void RemoveFields(Port port)
        {
            if (port.AnalogsSpecified)
            {
                foreach (Analog subAnalog in port.Analogs!)
                {
                    RemoveFields(subAnalog);
                }
            }
        }

        /// <summary>
        /// Remove fields with given values
        /// </summary>
        /// <param name="rom">Rom to remove fields from</param>
        private void RemoveFields(Rom rom)
        {
            if (rom.DataAreaSpecified)
                RemoveFields(rom.DataArea!);

            if (rom.PartSpecified)
                RemoveFields(rom.Part! as DatItem);
        }

        /// <summary>
        /// Remove fields with given values
        /// </summary>
        /// <param name="slot">Slot to remove fields from</param>
        private void RemoveFields(Slot slot)
        {
            if (slot.SlotOptionsSpecified)
            {
                foreach (SlotOption subSlotOption in slot.SlotOptions!)
                {
                    RemoveFields(subSlotOption);
                }
            }
        }

        #endregion
    }
}