using System;
using System.Collections.Generic;
using SabreTools.Core.Filter;
using SabreTools.DatFiles;
using SabreTools.DatItems;
using SabreTools.IO.Logging;

namespace SabreTools.Filtering
{
    public class ExtraIni
    {
        #region Fields

        /// <summary>
        /// List of extras to apply
        /// </summary>
        public readonly List<ExtraIniItem> Items = [];

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
            // If there are no inputs, just skip
            if (inputs == null || inputs.Count == 0)
                return;

            InternalStopwatch watch = new("Populating extras from list");

            foreach (string input in inputs)
            {
                // If we don't even have a possible field and file combination
                if (!input.Contains(":"))
                {
                    logger.Warning($"'{input}` is not a valid INI extras string. Valid INI extras strings are of the form 'key:value'. Please refer to README.1ST or the help feature for more details.");
                    return;
                }

                string inputTrimmed = input.Trim('"', ' ', '\t');
                string fieldString = inputTrimmed.Split(':')[0].ToLowerInvariant().Trim('"', ' ', '\t');
                string fileString = inputTrimmed.Substring(fieldString.Length + 1).Trim('"', ' ', '\t');

                FilterParser.ParseFilterId(fieldString, out string itemName, out string fieldName);
                var item = new ExtraIniItem(itemName, fieldName);
                if (item.PopulateFromFile(fileString))
                    Items.Add(item);
            }

            watch.Stop();
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
            // If we have no extras, don't attempt to apply and just return true
            if (Items == null || Items.Count == 0)
                return true;

            var watch = new InternalStopwatch("Applying extra mappings to DAT");

            try
            {
                // Bucket by game first
                datFile.Items.BucketBy(ItemKey.Machine, DedupeType.None);

                // Create mappings based on the extra items
                var combinedMaps = CombineExtras();
                var machines = combinedMaps.Keys;

                // Apply the mappings
                foreach (string machine in machines)
                {
                    // If the key doesn't exist, continue
                    if (!datFile.Items.ContainsKey(machine))
                        continue;

                    // Get the list of DatItems for the machine
                    var datItems = datFile.Items[machine];
                    if (datItems == null)
                        continue;

                    // Try to get the map values, if possible
                    combinedMaps.TryGetValue(machine, out var mappings);

                    // Create a setter with the new mappings
                    var setter = new Setter();
                    setter.PopulateSettersFromDictionary(mappings);

                    // Loop through and set the fields accordingly
                    foreach (var datItem in datItems)
                    {
                        setter.SetFields(datItem.GetFieldValue<Machine>(DatItem.MachineKey));
                        setter.SetFields(datItem);
                    }
                }
            }
            catch (Exception ex) when (!throwOnError)
            {
                logger.Error(ex);
                return false;
            }
            finally
            {
                watch.Stop();
            }

            return true;
        }

        /// <summary>
        /// Apply a set of Extra INIs on the DatFile
        /// </summary>
        /// <param name="datFile">Current DatFile object to run operations on</param>
        /// <param name="throwOnError">True if the error that is thrown should be thrown back to the caller, false otherwise</param>
        /// <returns>True if the extras were applied, false on error</returns>
        public bool ApplyExtrasDB(DatFile datFile, bool throwOnError = false)
        {
            // If we have no extras, don't attempt to apply and just return true
            if (Items == null || Items.Count == 0)
                return true;

            var watch = new InternalStopwatch("Applying extra mappings to DAT");

            try
            {
                // Bucket by game first
                datFile.ItemsDB.BucketBy(ItemKey.Machine, DedupeType.None);

                // Create mappings based on the extra items
                var combinedMaps = CombineExtras();
                var games = combinedMaps.Keys;

                // Apply the mappings
                foreach (string game in games)
                {
                    // Get the list of DatItems for the machine
                    var datItems = datFile.ItemsDB.GetItemsForBucket(game);
                    if (datItems == null)
                        continue;

                    // Try to get the map values, if possible
                    combinedMaps.TryGetValue(game, out var mappings);

                    // Create a setter with the new mappings
                    var setter = new Setter();
                    setter.PopulateSettersFromDictionary(mappings);

                    // Loop through and set the fields accordingly
                    foreach (var datItem in datItems)
                    {
                        var machine = datFile.ItemsDB.GetMachineForItem(datItem.Item1);
                        if (machine.Item2 != null)
                            setter.SetFields(machine.Item2);

                        setter.SetFields(datItem.Item2);
                    }
                }
            }
            catch (Exception ex) when (!throwOnError)
            {
                logger.Error(ex);
                return false;
            }
            finally
            {
                watch.Stop();
            }

            return true;
        }

        /// <summary>
        /// Combine ExtraIni fields
        /// </summary>
        /// <returns>Mapping dictionary from machine name to field mapping</returns>
        private Dictionary<string, Dictionary<FilterKey, string>> CombineExtras()
        {
            var machineMap = new Dictionary<string, Dictionary<FilterKey, string>>();

            // Loop through each of the extras
            foreach (ExtraIniItem item in Items)
            {
                foreach (var mapping in item.Mappings)
                {
                    string machineName = mapping.Key;
                    string value = mapping.Value;

                    if (!machineMap.ContainsKey(machineName))
                        machineMap[machineName] = [];

                    machineMap[machineName][item.Key] = value;
                }
            }

            return machineMap;
        }

        #endregion
    }
}
