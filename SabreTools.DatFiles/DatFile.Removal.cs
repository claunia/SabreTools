using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;

namespace SabreTools.DatFiles
{
    public partial class DatFile
    {
        #region Removal

        /// <summary>
        /// Remove fields indicated by the three input lists
        /// </summary>
        public void ApplyRemovals(List<string> headerFieldNames, List<string> machineFieldNames, Dictionary<string, List<string>> itemFieldNames)
        {
            // Remove DatHeader fields
            if (headerFieldNames.Count > 0)
                RemoveHeaderFields(headerFieldNames);

            // Remove DatItem and Machine fields
            if (machineFieldNames.Count > 0 || itemFieldNames.Count > 0)
            {
                ApplyRemovalsItemDictionary(machineFieldNames, itemFieldNames);
                ApplyRemovalsItemDictionaryDB(machineFieldNames, itemFieldNames);
            }
        }

        /// <summary>
        /// Apply removals to the item dictionary
        /// </summary>
        private void ApplyRemovalsItemDictionary(List<string> machineFieldNames, Dictionary<string, List<string>> itemFieldNames)
        {
#if NET452_OR_GREATER || NETCOREAPP
            Parallel.ForEach(Items.Keys, Globals.ParallelOptions, key =>
#elif NET40_OR_GREATER
            Parallel.ForEach(Items.Keys, key =>
#else
            foreach (var key in Items.Keys)
#endif
            {
                ConcurrentList<DatItem>? items = Items[key];
                if (items == null)
#if NET40_OR_GREATER || NETCOREAPP
                    return;
#else
                    continue;
#endif

                for (int j = 0; j < items.Count; j++)
                {
                    RemoveFields(items[j], machineFieldNames, itemFieldNames);
                }
#if NET40_OR_GREATER || NETCOREAPP
            });
#else
            }
#endif
        }

        /// <summary>
        /// Apply removals to the item dictionary
        /// </summary>
        private void ApplyRemovalsItemDictionaryDB(List<string> machineFieldNames, Dictionary<string, List<string>> itemFieldNames)
        {
#if NET452_OR_GREATER || NETCOREAPP
            Parallel.ForEach(ItemsDB.SortedKeys, Globals.ParallelOptions, key =>
#elif NET40_OR_GREATER
            Parallel.ForEach(ItemsDB.SortedKeys, key =>
#else
            foreach (var key in ItemsDB.SortedKeys)
#endif
            {
                var items = ItemsDB.GetDatItemsForBucket(key);
                if (items == null)
#if NET40_OR_GREATER || NETCOREAPP
                    return;
#else
                    continue;
#endif

                for (int j = 0; j < items.Length; j++)
                {
                    RemoveFields(items[j].Item2, machineFieldNames, itemFieldNames);
                }
#if NET40_OR_GREATER || NETCOREAPP
            });
#else
            }
#endif
        }

        /// <summary>
        /// Remove fields with given values
        /// </summary>
        private void RemoveHeaderFields(List<string> headerFieldNames)
        {
            // If we have an invalid input, return
            if (Header == null || !headerFieldNames.Any())
                return;

            foreach (var fieldName in headerFieldNames)
            {
                Header.RemoveField(fieldName);
            }
        }

        /// <summary>
        /// Remove fields with given values
        /// </summary>
        private static void RemoveFields(Machine? machine, List<string> machineFieldNames)
        {
            // If we have an invalid input, return
            if (machine == null || !machineFieldNames.Any())
                return;

            foreach (var fieldName in machineFieldNames)
            {
                machine.RemoveField(fieldName);
            }
        }

        /// <summary>
        /// Remove fields with given values
        /// </summary>
        /// <param name="datItem">DatItem to remove fields from</param>
        private static void RemoveFields(DatItem? datItem, List<string> machineFieldNames, Dictionary<string, List<string>> itemFieldNames)
        {
            if (datItem == null)
                return;

            #region Common

            // Handle Machine fields
            if (machineFieldNames.Any() && datItem.GetFieldValue<Machine>(DatItem.MachineKey) != null)
                RemoveFields(datItem.GetFieldValue<Machine>(DatItem.MachineKey), machineFieldNames);

            // If there are no field names, return
            if (itemFieldNames == null || !itemFieldNames.Any())
                return;

            // If there are no field names for this type or generic, return
            string? itemType = datItem.GetStringFieldValue(Models.Metadata.DatItem.TypeKey).AsEnumValue<ItemType>().AsStringValue();
            if (itemType == null || (!itemFieldNames.ContainsKey(itemType) && !itemFieldNames.ContainsKey("item")))
                return;

            // Get the combined list of fields to remove
            var fieldNames = new List<string>();
            if (itemFieldNames.ContainsKey(itemType))
                fieldNames.AddRange(itemFieldNames[itemType]);
            if (itemFieldNames.ContainsKey("item"))
                fieldNames.AddRange(itemFieldNames["item"]);
            fieldNames = fieldNames.Distinct().ToList();

            // If the field specifically contains Name, set it separately
            if (fieldNames.Contains(Models.Metadata.Rom.NameKey))
                datItem.SetName(null);

            #endregion

            #region Item-Specific

            // Handle unnested removals first
            foreach (var datItemField in fieldNames)
            {
                datItem.RemoveField(datItemField);
            }

            // Handle nested removals
            switch (datItem)
            {
                case Adjuster adjuster: RemoveFields(adjuster, itemFieldNames); break;
                case Configuration configuration: RemoveFields(configuration, itemFieldNames); break;
                case ConfSetting confSetting: RemoveFields(confSetting, itemFieldNames); break;
                case Device device: RemoveFields(device, itemFieldNames); break;
                case DipSwitch dipSwitch: RemoveFields(dipSwitch, itemFieldNames); break;
                case DipValue dipValue: RemoveFields(dipValue, itemFieldNames); break;
                case Disk disk: RemoveFields(disk, itemFieldNames); break;
                case Input input: RemoveFields(input, itemFieldNames); break;
                case Part part: RemoveFields(part, itemFieldNames); break;
                case Port port: RemoveFields(port, itemFieldNames); break;
                case Rom rom: RemoveFields(rom, itemFieldNames); break;
                case Slot slot: RemoveFields(slot, itemFieldNames); break;
            }

            #endregion
        }

        /// <summary>
        /// Remove fields with given values
        /// </summary>
        /// <param name="adjuster">Adjuster to remove fields from</param>
        private static void RemoveFields(Adjuster adjuster, Dictionary<string, List<string>> itemFieldNames)
        {
            if (!adjuster.ConditionsSpecified)
                return;

            foreach (Condition subCondition in adjuster.GetFieldValue<Condition[]?>(Models.Metadata.Adjuster.ConditionKey)!)
            {
                RemoveFields(subCondition, [], itemFieldNames);
            }
        }

        /// <summary>
        /// Remove fields with given values
        /// </summary>
        /// <param name="configuration">Configuration to remove fields from</param>
        private static void RemoveFields(Configuration configuration, Dictionary<string, List<string>> itemFieldNames)
        {
            if (configuration.ConditionsSpecified)
            {
                foreach (Condition subCondition in configuration.GetFieldValue<Condition[]?>(Models.Metadata.Configuration.ConditionKey)!)
                {
                    RemoveFields(subCondition, [], itemFieldNames);
                }
            }

            if (configuration.LocationsSpecified)
            {
                foreach (ConfLocation subLocation in configuration.GetFieldValue<ConfLocation[]?>(Models.Metadata.Configuration.ConfLocationKey)!)
                {
                    RemoveFields(subLocation, [], itemFieldNames);
                }
            }

            if (configuration.SettingsSpecified)
            {
                foreach (ConfSetting subSetting in configuration.GetFieldValue<ConfSetting[]?>(Models.Metadata.Configuration.ConfSettingKey)!)
                {
                    RemoveFields(subSetting as DatItem, [], itemFieldNames);
                }
            }
        }

        /// <summary>
        /// Remove fields with given values
        /// </summary>
        /// <param name="confsetting">ConfSetting to remove fields from</param>
        private static void RemoveFields(ConfSetting confsetting, Dictionary<string, List<string>> itemFieldNames)
        {
            if (confsetting.ConditionsSpecified)
            {
                foreach (Condition subCondition in confsetting.GetFieldValue<Condition[]?>(Models.Metadata.ConfSetting.ConditionKey)!)
                {
                    RemoveFields(subCondition, [], itemFieldNames);
                }
            }
        }

        /// <summary>
        /// Remove fields with given values
        /// </summary>
        /// <param name="device">Device to remove fields from</param>
        private static void RemoveFields(Device device, Dictionary<string, List<string>> itemFieldNames)
        {
            if (device.ExtensionsSpecified)
            {
                foreach (Extension subExtension in device.GetFieldValue<Extension[]?>(Models.Metadata.Device.ExtensionKey)!)
                {
                    RemoveFields(subExtension, [], itemFieldNames);
                }
            }

            if (device.InstancesSpecified)
            {
                foreach (Instance subInstance in device.GetFieldValue<Instance[]?>(Models.Metadata.Device.InstanceKey)!)
                {
                    RemoveFields(subInstance, [], itemFieldNames);
                }
            }
        }

        /// <summary>
        /// Remove fields with given values
        /// </summary>
        /// <param name="dipSwitch">DipSwitch to remove fields from</param>
        private static void RemoveFields(DipSwitch dipSwitch, Dictionary<string, List<string>> itemFieldNames)
        {
            if (dipSwitch.ConditionsSpecified)
            {
                foreach (Condition subCondition in dipSwitch.GetFieldValue<Condition[]?>(Models.Metadata.DipSwitch.ConditionKey)!)
                {
                    RemoveFields(subCondition, [], itemFieldNames);
                }
            }

            if (dipSwitch.LocationsSpecified)
            {
                foreach (DipLocation subLocation in dipSwitch.GetFieldValue<DipLocation[]?>(Models.Metadata.DipSwitch.DipLocationKey)!)
                {
                    RemoveFields(subLocation, [], itemFieldNames);
                }
            }

            if (dipSwitch.ValuesSpecified)
            {
                foreach (DipValue subValue in dipSwitch.GetFieldValue<DipValue[]?>(Models.Metadata.DipSwitch.DipValueKey)!)
                {
                    RemoveFields(subValue as DatItem, [], itemFieldNames);
                }
            }

            if (dipSwitch.PartSpecified)
                RemoveFields(dipSwitch.GetFieldValue<Part?>(DipSwitch.PartKey)! as DatItem, [], itemFieldNames);
        }

        /// <summary>
        /// Remove fields with given values
        /// </summary>
        /// <param name="dipValue">DipValue to remove fields from</param>
        private static void RemoveFields(DipValue dipValue, Dictionary<string, List<string>> itemFieldNames)
        {
            if (dipValue.ConditionsSpecified)
            {
                foreach (Condition subCondition in dipValue.GetFieldValue<Condition[]?>(Models.Metadata.DipValue.ConditionKey)!)
                {
                    RemoveFields(subCondition, [], itemFieldNames);
                }
            }
        }

        /// <summary>
        /// Remove fields with given values
        /// </summary>
        /// <param name="disk">Disk to remove fields from</param>
        private static void RemoveFields(Disk disk, Dictionary<string, List<string>> itemFieldNames)
        {
            if (disk.DiskAreaSpecified)
                RemoveFields(disk.GetFieldValue<DiskArea?>(Disk.DiskAreaKey)! as DatItem, [], itemFieldNames);

            if (disk.PartSpecified)
                RemoveFields(disk.GetFieldValue<Part?>(Disk.PartKey)! as DatItem, [], itemFieldNames);
        }

        /// <summary>
        /// Remove fields with given values
        /// </summary>
        /// <param name="input">Input to remove fields from</param>
        private static void RemoveFields(Input input, Dictionary<string, List<string>> itemFieldNames)
        {
            if (input.ControlsSpecified)
            {
                foreach (Control subControl in input.GetFieldValue<Control[]?>(Models.Metadata.Input.ControlKey)!)
                {
                    RemoveFields(subControl, [], itemFieldNames);
                }
            }
        }

        /// <summary>
        /// Remove fields with given values
        /// </summary>
        /// <param name="part">Part to remove fields from</param>
        private static void RemoveFields(Part part, Dictionary<string, List<string>> itemFieldNames)
        {
            if (part.FeaturesSpecified)
            {
                foreach (PartFeature subPartFeature in part.GetFieldValue<PartFeature[]?>(Models.Metadata.Part.FeatureKey)!)
                {
                    RemoveFields(subPartFeature, [], itemFieldNames);
                }
            }
        }

        /// <summary>
        /// Remove fields with given values
        /// </summary>
        /// <param name="port">Port to remove fields from</param>
        private static void RemoveFields(Port port, Dictionary<string, List<string>> itemFieldNames)
        {
            if (port.AnalogsSpecified)
            {
                foreach (Analog subAnalog in port.GetFieldValue<Analog[]?>(Models.Metadata.Port.AnalogKey)!)
                {
                    RemoveFields(subAnalog, [], itemFieldNames);
                }
            }
        }

        /// <summary>
        /// Remove fields with given values
        /// </summary>
        /// <param name="rom">Rom to remove fields from</param>
        private static void RemoveFields(Rom rom, Dictionary<string, List<string>> itemFieldNames)
        {
            if (rom.DataAreaSpecified)
                RemoveFields(rom.GetFieldValue<DataArea?>(Rom.DataAreaKey)!, [], itemFieldNames);

            if (rom.PartSpecified)
                RemoveFields(rom.GetFieldValue<Part?>(Rom.PartKey)! as DatItem, [], itemFieldNames);
        }

        /// <summary>
        /// Remove fields with given values
        /// </summary>
        /// <param name="slot">Slot to remove fields from</param>
        private static void RemoveFields(Slot slot, Dictionary<string, List<string>> itemFieldNames)
        {
            if (slot.SlotOptionsSpecified)
            {
                foreach (SlotOption subSlotOption in slot.GetFieldValue<SlotOption[]?>(Models.Metadata.Slot.SlotOptionKey)!)
                {
                    RemoveFields(subSlotOption, [], itemFieldNames);
                }
            }
        }

        #endregion
    }
}
