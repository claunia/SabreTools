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
        /// Remove header fields with given values
        /// </summary>
        public void RemoveHeaderFields(List<string> fields)
        {
            // If we have an invalid input, return
            if (fields.Count == 0)
                return;

            foreach (var fieldName in fields)
            {
                bool removed = Header.RemoveField(fieldName);
                _logger.Verbose($"Header field {fieldName} {(removed ? "removed" : "could not be removed")}");
            }
        }

        /// <summary>
        /// Apply removals to the item dictionary
        /// </summary>
        public void RemoveItemFields(List<string> machineFields, Dictionary<string, List<string>> itemFields)
        {
            // If we have an invalid input, return
            if (machineFields.Count == 0 && itemFields.Count == 0)
                return;

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
                    RemoveFields(items[j], machineFields, itemFields);
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
        public void RemoveItemFieldsDB(List<string> machineFields, Dictionary<string, List<string>> itemFields)
        {
            // If we have an invalid input, return
            if (machineFields.Count == 0 && itemFields.Count == 0)
                return;

            // Handle machine removals
#if NET452_OR_GREATER || NETCOREAPP
            Parallel.ForEach(ItemsDB.GetMachines(), Core.Globals.ParallelOptions, kvp =>
#elif NET40_OR_GREATER
            Parallel.ForEach(ItemsDB.GetMachines(), kvp =>
#else
            foreach (var kvp in ItemsDB.GetMachines())
#endif
            {
                RemoveFields(kvp.Value, machineFields);
#if NET40_OR_GREATER || NETCOREAPP
            });
#else
            }
#endif

            // Handle item removals
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
                    RemoveFields(item, [], itemFields);
                }
#if NET40_OR_GREATER || NETCOREAPP
            });
#else
            }
#endif
        }

        /// <summary>
        /// Remove machine fields with given values
        /// </summary>
        private static void RemoveFields(Machine? machine, List<string> fields)
        {
            // If we have an invalid input, return
            if (machine == null || fields.Count == 0)
                return;

            foreach (var fieldName in fields)
            {
                machine.RemoveField(fieldName);
            }
        }

        /// <summary>
        /// Remove fields with given values
        /// </summary>
        /// <param name="datItem">DatItem to remove fields from</param>
        private static void RemoveFields(DatItem? datItem, List<string> machineFields, Dictionary<string, List<string>> itemFields)
        {
            if (datItem == null)
                return;

            #region Common

            // Handle Machine fields
            var machine = datItem.GetFieldValue<Machine>(DatItem.MachineKey);
            if (machineFields.Count > 0 && machine != null)
                RemoveFields(machine, machineFields);

            // If there are no field names, return
            if (itemFields == null || itemFields.Count == 0)
                return;

            // If there are no field names for this type or generic, return
            string? itemType = datItem.GetStringFieldValue(Models.Metadata.DatItem.TypeKey).AsEnumValue<ItemType>().AsStringValue();
            if (itemType == null || (!itemFields.ContainsKey(itemType) && !itemFields.ContainsKey("item")))
                return;

            // Get the combined list of fields to remove
            var fields = new HashSet<string>();
            if (itemFields.ContainsKey(itemType))
                fields.UnionWith(itemFields[itemType]);
            if (itemFields.ContainsKey("item"))
                fields.UnionWith(itemFields["item"]);

            // If the field specifically contains Name, set it separately
            if (fields.Contains(Models.Metadata.Rom.NameKey))
                datItem.SetName(null);

            #endregion

            #region Item-Specific

            // Handle unnested removals first
            foreach (var datItemField in fields)
            {
                datItem.RemoveField(datItemField);
            }

            // Handle nested removals
            switch (datItem)
            {
                case Adjuster adjuster: RemoveFields(adjuster, itemFields); break;
                case Configuration configuration: RemoveFields(configuration, itemFields); break;
                case ConfSetting confSetting: RemoveFields(confSetting, itemFields); break;
                case Device device: RemoveFields(device, itemFields); break;
                case DipSwitch dipSwitch: RemoveFields(dipSwitch, itemFields); break;
                case DipValue dipValue: RemoveFields(dipValue, itemFields); break;
                case Disk disk: RemoveFields(disk, itemFields); break;
                case Input input: RemoveFields(input, itemFields); break;
                case Part part: RemoveFields(part, itemFields); break;
                case Port port: RemoveFields(port, itemFields); break;
                case Rom rom: RemoveFields(rom, itemFields); break;
                case Slot slot: RemoveFields(slot, itemFields); break;
            }

            #endregion
        }

        /// <summary>
        /// Remove fields with given values
        /// </summary>
        /// <param name="adjuster">Adjuster to remove fields from</param>
        private static void RemoveFields(Adjuster adjuster, Dictionary<string, List<string>> fields)
        {
            var conditions = adjuster.GetFieldValue<Condition[]?>(Models.Metadata.Adjuster.ConditionKey) ?? [];
            foreach (Condition subCondition in conditions)
            {
                RemoveFields(subCondition, [], fields);
            }
        }

        /// <summary>
        /// Remove fields with given values
        /// </summary>
        /// <param name="configuration">Configuration to remove fields from</param>
        private static void RemoveFields(Configuration configuration, Dictionary<string, List<string>> fields)
        {
            var conditions = configuration.GetFieldValue<Condition[]?>(Models.Metadata.Configuration.ConditionKey) ?? [];
            foreach (Condition subCondition in conditions)
            {
                RemoveFields(subCondition, [], fields);
            }

            var locations = configuration.GetFieldValue<ConfLocation[]?>(Models.Metadata.Configuration.ConfLocationKey) ?? [];
            foreach (ConfLocation subLocation in locations)
            {
                RemoveFields(subLocation, [], fields);
            }

            var settings = configuration.GetFieldValue<ConfSetting[]?>(Models.Metadata.Configuration.ConfSettingKey) ?? [];
            foreach (ConfSetting subSetting in settings)
            {
                RemoveFields(subSetting as DatItem, [], fields);
            }
        }

        /// <summary>
        /// Remove fields with given values
        /// </summary>
        /// <param name="confsetting">ConfSetting to remove fields from</param>
        private static void RemoveFields(ConfSetting confsetting, Dictionary<string, List<string>> fields)
        {
            var conditions = confsetting.GetFieldValue<Condition[]?>(Models.Metadata.ConfSetting.ConditionKey) ?? [];
            foreach (Condition subCondition in conditions)
            {
                RemoveFields(subCondition, [], fields);
            }
        }

        /// <summary>
        /// Remove fields with given values
        /// </summary>
        /// <param name="device">Device to remove fields from</param>
        private static void RemoveFields(Device device, Dictionary<string, List<string>> fields)
        {
            var extensions = device.GetFieldValue<Extension[]?>(Models.Metadata.Device.ExtensionKey) ?? [];
            foreach (Extension subExtension in extensions)
            {
                RemoveFields(subExtension, [], fields);
            }

            var instances = device.GetFieldValue<Instance[]?>(Models.Metadata.Device.InstanceKey) ?? [];
            foreach (Instance subInstance in instances)
            {
                RemoveFields(subInstance, [], fields);
            }
        }

        /// <summary>
        /// Remove fields with given values
        /// </summary>
        /// <param name="dipSwitch">DipSwitch to remove fields from</param>
        private static void RemoveFields(DipSwitch dipSwitch, Dictionary<string, List<string>> fields)
        {
            var conditions = dipSwitch.GetFieldValue<Condition[]?>(Models.Metadata.DipSwitch.ConditionKey) ?? [];
            foreach (Condition subCondition in conditions)
            {
                RemoveFields(subCondition, [], fields);
            }

            var locations = dipSwitch.GetFieldValue<DipLocation[]?>(Models.Metadata.DipSwitch.DipLocationKey) ?? [];
            foreach (DipLocation subLocation in locations)
            {
                RemoveFields(subLocation, [], fields);
            }

            var dipValues = dipSwitch.GetFieldValue<DipValue[]?>(Models.Metadata.DipSwitch.DipValueKey) ?? [];
            foreach (DipValue subValue in dipValues)
            {
                RemoveFields(subValue as DatItem, [], fields);
            }

            var part = dipSwitch.GetFieldValue<Part?>(DipSwitch.PartKey);
            if (part != null)
                RemoveFields(part as DatItem, [], fields);
        }

        /// <summary>
        /// Remove fields with given values
        /// </summary>
        /// <param name="dipValue">DipValue to remove fields from</param>
        private static void RemoveFields(DipValue dipValue, Dictionary<string, List<string>> fields)
        {
            var conditions = dipValue.GetFieldValue<Condition[]?>(Models.Metadata.DipValue.ConditionKey) ?? [];
            foreach (Condition subCondition in conditions)
            {
                RemoveFields(subCondition, [], fields);
            }
        }

        /// <summary>
        /// Remove fields with given values
        /// </summary>
        /// <param name="disk">Disk to remove fields from</param>
        private static void RemoveFields(Disk disk, Dictionary<string, List<string>> fields)
        {
            var diskArea = disk.GetFieldValue<DiskArea?>(Disk.DiskAreaKey);
            if (diskArea != null)
                RemoveFields(diskArea as DatItem, [], fields);

            var part = disk.GetFieldValue<Part?>(Disk.PartKey);
            if (part != null)
                RemoveFields(part as DatItem, [], fields);
        }

        /// <summary>
        /// Remove fields with given values
        /// </summary>
        /// <param name="input">Input to remove fields from</param>
        private static void RemoveFields(Input input, Dictionary<string, List<string>> fields)
        {
            var controls = input.GetFieldValue<Control[]?>(Models.Metadata.Input.ControlKey) ?? [];
            foreach (Control subControl in controls)
            {
                RemoveFields(subControl, [], fields);
            }
        }

        /// <summary>
        /// Remove fields with given values
        /// </summary>
        /// <param name="part">Part to remove fields from</param>
        private static void RemoveFields(Part part, Dictionary<string, List<string>> fields)
        {
            var features = part.GetFieldValue<PartFeature[]?>(Models.Metadata.Part.FeatureKey) ?? [];
            foreach (PartFeature subPartFeature in features)
            {
                RemoveFields(subPartFeature, [], fields);
            }
        }

        /// <summary>
        /// Remove fields with given values
        /// </summary>
        /// <param name="port">Port to remove fields from</param>
        private static void RemoveFields(Port port, Dictionary<string, List<string>> fields)
        {
            var analogs = port.GetFieldValue<Analog[]?>(Models.Metadata.Port.AnalogKey) ?? [];
            foreach (Analog subAnalog in analogs)
            {
                RemoveFields(subAnalog, [], fields);
            }
        }

        /// <summary>
        /// Remove fields with given values
        /// </summary>
        /// <param name="rom">Rom to remove fields from</param>
        private static void RemoveFields(Rom rom, Dictionary<string, List<string>> fields)
        {
            var dataArea = rom.GetFieldValue<DataArea?>(Rom.DataAreaKey);
            if (dataArea != null)
                RemoveFields(dataArea as DatItem, [], fields);

            var part = rom.GetFieldValue<Part?>(Rom.PartKey);
            if (part != null)
                RemoveFields(part as DatItem, [], fields);
        }

        /// <summary>
        /// Remove fields with given values
        /// </summary>
        /// <param name="slot">Slot to remove fields from</param>
        private static void RemoveFields(Slot slot, Dictionary<string, List<string>> fields)
        {
            var slotOptions = slot.GetFieldValue<SlotOption[]?>(Models.Metadata.Slot.SlotOptionKey) ?? [];
            foreach (SlotOption subSlotOption in slotOptions)
            {
                RemoveFields(subSlotOption, [], fields);
            }
        }

        #endregion
    }
}
