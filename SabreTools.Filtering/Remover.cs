using System.Collections.Generic;
using System.Linq;
#if NET40_OR_GREATER || NETCOREAPP
using System.Threading.Tasks;
#endif
using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.DatFiles;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;
using SabreTools.Filter;
using SabreTools.Logging;

namespace SabreTools.Filtering
{
    /// <summary>
    /// Represents the removal operations that need to be performed on a set of items, usually a DAT
    /// </summary>
    public class Remover
    {
        #region Fields

        /// <summary>
        /// List of header fields to exclude from writing
        /// </summary>
        public List<string> HeaderFieldNames { get; } = [];

        /// <summary>
        /// List of machine fields to exclude from writing
        /// </summary>
        public List<string> MachineFieldNames { get; } = [];

        /// <summary>
        /// List of fields to exclude from writing
        /// </summary>
        public Dictionary<string, List<string>> ItemFieldNames { get; } = [];

        #endregion

        #region Logging

        /// <summary>
        /// Logging object
        /// </summary>
        protected Logger logger;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public Remover()
        {
            logger = new Logger(this);
        }

        #endregion

        #region Population

        /// <summary>
        /// Populate the exclusion objects using a field name
        /// </summary>
        /// <param name="field">Field name</param>
        public void PopulateExclusions(string field)
            => PopulateExclusionsFromList([field]);

        /// <summary>
        /// Populate the exclusion objects using a set of field names
        /// </summary>
        /// <param name="fields">List of field names</param>
        public void PopulateExclusionsFromList(List<string>? fields)
        {
            // If the list is null or empty, just return
            if (fields == null || fields.Count == 0)
                return;

            var watch = new InternalStopwatch("Populating removals from list");

            foreach (string field in fields)
            {
                bool removerSet = SetRemover(field);
                if (!removerSet)
                    logger.Warning($"The value {field} did not match any known field names. Please check the wiki for more details on supported field names.");
            }

            watch.Stop();
        }

        /// <summary>
        /// Set remover from a value
        /// </summary>
        /// <param name="field">Key for the remover to be set</param>
        private bool SetRemover(string field)
        {
            // If the key is null or empty, return false
            if (string.IsNullOrEmpty(field))
                return false;

            // Get the parser pair out of it, if possible
            (string? type, string? key) = FilterParser.ParseFilterId(field);
            if (type == null || key == null)
                return false;

            switch (type)
            {
                case Models.Metadata.MetadataFile.HeaderKey:
                    HeaderFieldNames.Add(key);
                    return true;

                case Models.Metadata.MetadataFile.MachineKey:
                    MachineFieldNames.Add(key);
                    return true;

                default:
                    if (!ItemFieldNames.ContainsKey(type))
                        ItemFieldNames[type] = [];

                    ItemFieldNames[type].Add(key);
                    return true;
            }
        }

        #endregion

        #region Running

        /// <summary>
        /// Remove fields from a DatFile
        /// </summary>
        /// <param name="datFile">Current DatFile object to run operations on</param>
        public void ApplyRemovals(DatFile datFile)
        {
            InternalStopwatch watch = new("Applying removals to DAT");

            // Remove DatHeader fields
            if (HeaderFieldNames.Any())
                RemoveFields(datFile.Header);

            // Remove DatItem and Machine fields
            if (MachineFieldNames.Any() || ItemFieldNames.Any())
            {
#if NET452_OR_GREATER || NETCOREAPP
                Parallel.ForEach(datFile.Items.Keys, Globals.ParallelOptions, key =>
#elif NET40_OR_GREATER
                Parallel.ForEach(datFile.Items.Keys, key =>
#else
                foreach (var key in datFile.Items.Keys)
#endif
                {
                    ConcurrentList<DatItem>? items = datFile.Items[key];
                    if (items == null)
#if NET40_OR_GREATER || NETCOREAPP
                        return;
#else
                        continue;
#endif

                    for (int j = 0; j < items.Count; j++)
                    {
                        RemoveFields(items[j]);
                    }

                    datFile.Items.Remove(key);
                    datFile.Items.AddRange(key, items);
#if NET40_OR_GREATER || NETCOREAPP
                });
#else
                }
#endif
            }

            watch.Stop();
        }

        /// <summary>
        /// Remove fields with given values
        /// </summary>
        /// <param name="datItem">DatHeader to remove fields from</param>
        public void RemoveFields(DatHeader datHeader)
        {
            // If we have an invalid input, return
            if (datHeader == null || !HeaderFieldNames.Any())
                return;

            foreach (var fieldName in HeaderFieldNames)
            {
                datHeader.RemoveField(fieldName);
            }
        }

        /// <summary>
        /// Remove fields with given values
        /// </summary>
        /// <param name="machine">Machine to remove fields from</param>
        public void RemoveFields(Machine? machine)
        {
            // If we have an invalid input, return
            if (machine == null || !MachineFieldNames.Any())
                return;

            foreach (var fieldName in MachineFieldNames)
            {
                machine.RemoveField(fieldName);
            }
        }

        /// <summary>
        /// Remove fields with given values
        /// </summary>
        /// <param name="datItem">DatItem to remove fields from</param>
        public void RemoveFields(DatItem? datItem)
        {
            if (datItem == null)
                return;

            #region Common

            // Handle Machine fields
            if (MachineFieldNames.Any() && datItem.Machine != null)
                RemoveFields(datItem.Machine);

            // If there are no field names, return
            if (ItemFieldNames == null || !ItemFieldNames.Any())
                return;

            // If there are no field names for this type or generic, return
            string? itemType = datItem.ItemType.AsStringValue<ItemType>();
            if (itemType == null || (!ItemFieldNames.ContainsKey(itemType) && !ItemFieldNames.ContainsKey("item")))
                return;

            // Get the combined list of fields to remove
            var fieldNames = new List<string>();
            if (ItemFieldNames.ContainsKey(itemType))
                fieldNames.AddRange(ItemFieldNames[itemType]);
            if (ItemFieldNames.ContainsKey("item"))
                fieldNames.AddRange(ItemFieldNames["item"]);
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