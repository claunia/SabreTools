using System;
using System.Collections.Generic;

using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.DatFiles;
using SabreTools.DatItems;
using SabreTools.Logging;

namespace SabreTools.Filtering
{
    public class ExtraIni
    {
        #region Fields

        /// <summary>
        /// List of extras to apply
        /// </summary>
        public List<ExtraIniItem> Items { get; set; } = new List<ExtraIniItem>();

        #endregion

        #region Logging

        /// <summary>
        /// Logging object
        /// </summary>
        private readonly Logger logger;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public ExtraIni()
        {
            logger = new Logger(this);
        }

        #endregion

        #region Population

        /// <summary>
        /// Populate item using field:file inputs
        /// </summary>
        /// <param name="inputs">Field and file combinations</param>
        public void PopulateFromList(List<string> inputs)
        {
            foreach (string input in inputs)
            {
                ExtraIniItem item = new ExtraIniItem();

                // If we don't even have a possible field and file combination
                if (!input.Contains(":"))
                {
                    logger.Warning($"'{input}` is not a valid INI extras string. Valid INI extras strings are of the form 'key:value'. Please refer to README.1ST or the help feature for more details.");
                    return;
                }

                string inputTrimmed = input.Trim('"', ' ', '\t');
                string fieldString = inputTrimmed.Split(':')[0].ToLowerInvariant().Trim('"', ' ', '\t');
                string fileString = inputTrimmed[(fieldString.Length + 1)..].Trim('"', ' ', '\t');

                item.DatItemField = fieldString.AsDatItemField();
                item.MachineField = fieldString.AsMachineField();
                if (item.PopulateFromFile(fileString))
                    Items.Add(item);
            }
        }

        #endregion
    
        #region Running

        /// <summary>
        /// Apply a set of Extra INIs on the DatFile
        /// </summary>
        /// <param name="datFile">Current DatFile object to run operations on</param>
        /// <param name="throwOnError">True if the error that is thrown should be thrown back to the caller, false otherwise</param>
        /// <returns>True if the extras were applied, false on error</returns>
        public bool ApplyExtras(DatFile datFile, bool throwOnError = false)
        {
            try
            {
                // Bucket by game first
                datFile.Items.BucketBy(ItemKey.Machine, DedupeType.None);

                // Create a new set of mappings based on the items
                var machineMap = new Dictionary<string, Dictionary<MachineField, string>>();
                var datItemMap = new Dictionary<string, Dictionary<DatItemField, string>>();

                // Loop through each of the extras
                foreach (ExtraIniItem item in Items)
                {
                    foreach (var mapping in item.Mappings)
                    {
                        string key = mapping.Key;
                        List<string> machineNames = mapping.Value;

                        // Loop through the machines and add the new mappings
                        foreach (string machine in machineNames)
                        {
                            if (item.MachineField != MachineField.NULL)
                            {
                                if (!machineMap.ContainsKey(machine))
                                    machineMap[machine] = new Dictionary<MachineField, string>();

                                machineMap[machine][item.MachineField] = key;
                            }
                            else if (item.DatItemField != DatItemField.NULL)
                            {
                                if (!datItemMap.ContainsKey(machine))
                                    datItemMap[machine] = new Dictionary<DatItemField, string>();

                                datItemMap[machine][item.DatItemField] = key;
                            }
                        }
                    }
                }

                // Now apply the new set of Machine mappings
                foreach (string key in machineMap.Keys)
                {
                    // If the key doesn't exist, continue
                    if (!datFile.Items.ContainsKey(key))
                        continue;

                    List<DatItem> datItems = datFile.Items[key];
                    var mappings = machineMap[key];

                    foreach (var datItem in datItems)
                    {
                        Setter.SetFields(datItem.Machine, mappings);
                    }
                }

                // Now apply the new set of DatItem mappings
                foreach (string key in datItemMap.Keys)
                {
                    // If the key doesn't exist, continue
                    if (!datFile.Items.ContainsKey(key))
                        continue;

                    List<DatItem> datItems = datFile.Items[key];
                    var mappings = datItemMap[key];

                    foreach (var datItem in datItems)
                    {
                        Setter.SetFields(datItem, mappings, null);
                    }
                }
            }
            catch (Exception ex) when (!throwOnError)
            {
                logger.Error(ex);
                return false;
            }

            return true;
        }

        #endregion
    }
}
