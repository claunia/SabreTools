using System.Collections.Generic;
#if NET40_OR_GREATER || NETCOREAPP
using System.Threading.Tasks;
#endif
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
            Parallel.ForEach(Items.Keys, Core.Globals.ParallelOptions, key =>
#elif NET40_OR_GREATER
            Parallel.ForEach(Items.Keys, key =>
#else
            foreach (var key in Items.Keys)
#endif
            {
                List<DatItem>? items = Items[key];
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
            Parallel.ForEach(ItemsDB.SortedKeys, Core.Globals.ParallelOptions, key =>
#elif NET40_OR_GREATER
            Parallel.ForEach(ItemsDB.SortedKeys, key =>
#else
            foreach (var key in ItemsDB.SortedKeys)
#endif
            {
                var items = ItemsDB.GetItemsForBucket(key);
                if (items == null)
#if NET40_OR_GREATER || NETCOREAPP
                    return;
#else
                    continue;
#endif

                foreach (var item in items.Values)
                {
                    RemoveFields(item, machineFieldNames, itemFieldNames);
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
            if (Header == null || headerFieldNames.Count == 0)
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
            if (machine == null || machineFieldNames.Count == 0)
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
            var machine = datItem.GetFieldValue<Machine>(DatItem.MachineKey);
            if (machineFieldNames.Count > 0 && machine != null)
                RemoveFields(machine, machineFieldNames);

            // If there are no field names, return
            if (itemFieldNames == null || itemFieldNames.Count == 0)
                return;

            // If there are no field names for this type or generic, return
            string? itemType = datItem.GetStringFieldValue(Models.Metadata.DatItem.TypeKey).AsEnumValue<ItemType>().AsStringValue();
            if (itemType == null || (!itemFieldNames.ContainsKey(itemType) && !itemFieldNames.ContainsKey("item")))
                return;

            // Get the combined list of fields to remove
            var fieldNames = new HashSet<string>();
            if (itemFieldNames.ContainsKey(itemType))
                fieldNames.UnionWith(itemFieldNames[itemType]);
            if (itemFieldNames.ContainsKey("item"))
                fieldNames.UnionWith(itemFieldNames["item"]);

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
            var conditions = adjuster.GetFieldValue<Condition[]?>(Models.Metadata.Adjuster.ConditionKey) ?? [];
            foreach (Condition subCondition in conditions)
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
            var conditions = configuration.GetFieldValue<Condition[]?>(Models.Metadata.Configuration.ConditionKey) ?? [];
            foreach (Condition subCondition in conditions)
            {
                RemoveFields(subCondition, [], itemFieldNames);
            }

            var locations = configuration.GetFieldValue<ConfLocation[]?>(Models.Metadata.Configuration.ConfLocationKey) ?? [];
            foreach (ConfLocation subLocation in locations)
            {
                RemoveFields(subLocation, [], itemFieldNames);
            }

            var settings = configuration.GetFieldValue<ConfSetting[]?>(Models.Metadata.Configuration.ConfSettingKey) ?? [];
            foreach (ConfSetting subSetting in settings)
            {
                RemoveFields(subSetting as DatItem, [], itemFieldNames);
            }
        }

        /// <summary>
        /// Remove fields with given values
        /// </summary>
        /// <param name="confsetting">ConfSetting to remove fields from</param>
        private static void RemoveFields(ConfSetting confsetting, Dictionary<string, List<string>> itemFieldNames)
        {
            var conditions = confsetting.GetFieldValue<Condition[]?>(Models.Metadata.ConfSetting.ConditionKey) ?? [];
            foreach (Condition subCondition in conditions)
            {
                RemoveFields(subCondition, [], itemFieldNames);
            }
        }

        /// <summary>
        /// Remove fields with given values
        /// </summary>
        /// <param name="device">Device to remove fields from</param>
        private static void RemoveFields(Device device, Dictionary<string, List<string>> itemFieldNames)
        {
            var extensions = device.GetFieldValue<Extension[]?>(Models.Metadata.Device.ExtensionKey) ?? [];
            foreach (Extension subExtension in extensions)
            {
                RemoveFields(subExtension, [], itemFieldNames);
            }

            var instances = device.GetFieldValue<Instance[]?>(Models.Metadata.Device.InstanceKey) ?? [];
            foreach (Instance subInstance in instances)
            {
                RemoveFields(subInstance, [], itemFieldNames);
            }
        }

        /// <summary>
        /// Remove fields with given values
        /// </summary>
        /// <param name="dipSwitch">DipSwitch to remove fields from</param>
        private static void RemoveFields(DipSwitch dipSwitch, Dictionary<string, List<string>> itemFieldNames)
        {
            var conditions = dipSwitch.GetFieldValue<Condition[]?>(Models.Metadata.DipSwitch.ConditionKey) ?? [];
            foreach (Condition subCondition in conditions)
            {
                RemoveFields(subCondition, [], itemFieldNames);
            }

            var locations = dipSwitch.GetFieldValue<DipLocation[]?>(Models.Metadata.DipSwitch.DipLocationKey) ?? [];
            foreach (DipLocation subLocation in locations)
            {
                RemoveFields(subLocation, [], itemFieldNames);
            }

            var dipValues = dipSwitch.GetFieldValue<DipValue[]?>(Models.Metadata.DipSwitch.DipValueKey) ?? [];
            foreach (DipValue subValue in dipValues)
            {
                RemoveFields(subValue as DatItem, [], itemFieldNames);
            }

            var part = dipSwitch.GetFieldValue<Part?>(DipSwitch.PartKey);
            if (part != null)
                RemoveFields(part as DatItem, [], itemFieldNames);
        }

        /// <summary>
        /// Remove fields with given values
        /// </summary>
        /// <param name="dipValue">DipValue to remove fields from</param>
        private static void RemoveFields(DipValue dipValue, Dictionary<string, List<string>> itemFieldNames)
        {
            var conditions = dipValue.GetFieldValue<Condition[]?>(Models.Metadata.DipValue.ConditionKey) ?? [];
            foreach (Condition subCondition in conditions)
            {
                RemoveFields(subCondition, [], itemFieldNames);
            }
        }

        /// <summary>
        /// Remove fields with given values
        /// </summary>
        /// <param name="disk">Disk to remove fields from</param>
        private static void RemoveFields(Disk disk, Dictionary<string, List<string>> itemFieldNames)
        {
            var diskArea = disk.GetFieldValue<DiskArea?>(Disk.DiskAreaKey);
            if (diskArea != null)
                RemoveFields(diskArea as DatItem, [], itemFieldNames);

            var part = disk.GetFieldValue<Part?>(Disk.PartKey);
            if (part != null)
                RemoveFields(part as DatItem, [], itemFieldNames);
        }

        /// <summary>
        /// Remove fields with given values
        /// </summary>
        /// <param name="input">Input to remove fields from</param>
        private static void RemoveFields(Input input, Dictionary<string, List<string>> itemFieldNames)
        {
            var controls = input.GetFieldValue<Control[]?>(Models.Metadata.Input.ControlKey) ?? [];
            foreach (Control subControl in controls)
            {
                RemoveFields(subControl, [], itemFieldNames);
            }
        }

        /// <summary>
        /// Remove fields with given values
        /// </summary>
        /// <param name="part">Part to remove fields from</param>
        private static void RemoveFields(Part part, Dictionary<string, List<string>> itemFieldNames)
        {
            var features = part.GetFieldValue<PartFeature[]?>(Models.Metadata.Part.FeatureKey) ?? [];
            foreach (PartFeature subPartFeature in features)
            {
                RemoveFields(subPartFeature, [], itemFieldNames);
            }
        }

        /// <summary>
        /// Remove fields with given values
        /// </summary>
        /// <param name="port">Port to remove fields from</param>
        private static void RemoveFields(Port port, Dictionary<string, List<string>> itemFieldNames)
        {
            var analogs = port.GetFieldValue<Analog[]?>(Models.Metadata.Port.AnalogKey) ?? [];
            foreach (Analog subAnalog in analogs)
            {
                RemoveFields(subAnalog, [], itemFieldNames);
            }
        }

        /// <summary>
        /// Remove fields with given values
        /// </summary>
        /// <param name="rom">Rom to remove fields from</param>
        private static void RemoveFields(Rom rom, Dictionary<string, List<string>> itemFieldNames)
        {
            var dataArea = rom.GetFieldValue<DataArea?>(Rom.DataAreaKey);
            if (dataArea != null)
                RemoveFields(dataArea as DatItem, [], itemFieldNames);

            var part = rom.GetFieldValue<Part?>(Rom.PartKey);
            if (part != null)
                RemoveFields(part as DatItem, [], itemFieldNames);
        }

        /// <summary>
        /// Remove fields with given values
        /// </summary>
        /// <param name="slot">Slot to remove fields from</param>
        private static void RemoveFields(Slot slot, Dictionary<string, List<string>> itemFieldNames)
        {
            var slotOptions = slot.GetFieldValue<SlotOption[]?>(Models.Metadata.Slot.SlotOptionKey) ?? [];
            foreach (SlotOption subSlotOption in slotOptions)
            {
                RemoveFields(subSlotOption, [], itemFieldNames);
            }
        }

        #endregion
    }
}
