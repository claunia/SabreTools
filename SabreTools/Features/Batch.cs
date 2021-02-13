using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.DatFiles;
using SabreTools.DatTools;
using SabreTools.Filtering;
using SabreTools.Help;
using SabreTools.IO;

namespace SabreTools.Features
{
    internal class Batch : BaseFeature
    {
        public const string Value = "Batch";

        public Batch()
        {
            Name = Value;
            Flags = new List<string>() { "-bt", "--batch" };
            Description = "Enable batch mode";
            _featureType = ParameterType.Flag;
            LongDescription = @"Run a special mode that takes input files as lists of batch commands to run sequentially. Each command has to be its own line and must be followed by a semicolon (`;`). Commented lines may start with either `REM` or `#`. Multiple batch files are allowed but they will be run independently from each other.

The following commands are currently implemented:

Set a header field (if default):    set(header.field, value);
Parse new file(s):                  input(datpath, ...);
Perform a dir2dat:                  d2d(path, ...);
Filter on a field and value:        filter(machine.field|item.field, value, [remove = false, [perMachine = false]]);
Apply a MAME Extra INI for a field: extra(field, inipath);
Perform a split/merge:              merge(split|merged|nonmerged|full|device);
Set game names from description:    descname();
Run 1G1R on the items:              1g1r(region, ...);
Split into one rom per game:        orpg();
Remove fields from games/items:     remove(machine.field|item.field, ...);
Remove scene dates from names:      sds();
Add new output format(s):           format(datformat, ...);
Set the output directory:           output(outdir);
Write the internal items:           write([overwrite = true]);
Reset the internal state:           reset();";
            Features = new Dictionary<string, Help.Feature>();

            // Common Features
            AddCommonFeatures();
        }

        public override void ProcessFeatures(Dictionary<string, Help.Feature> features)
        {
            base.ProcessFeatures(features);

            // Try to read each input as a batch run file
            foreach (string path in Inputs)
            {
                ProcessScript(path);
            }
        }

        /// <summary>
        /// Process a single input file as a batch run file
        /// </summary>
        /// <param name="path">Path to the input file</param>
        private void ProcessScript(string path)
        {
            // If the file doesn't exist, warn but continue
            if (!File.Exists(path))
            {
                logger.User($"{path} does not exist. Skipping...");
                return;
            }

            // Try to process the file now
            try
            {
                // Every line is its own command
                string[] lines = File.ReadAllLines(path);

                // Each batch file has its own state
                int index = 0;
                DatFile datFile = DatFile.Create();
                string outputDirectory = null;

                // Process each command line
                foreach (string line in lines)
                {
                    // Skip empty lines
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    // Skip lines that start with REM or #
                    if (line.StartsWith("REM") || line.StartsWith("#"))
                        continue;

                    // Read the command in, if possible
                    var command = BatchCommand.Create(line);
                    if (command == null)
                    {
                        logger.User($"Could not process {path} due to the following line: {line}");
                        break;
                    }

                    // Now switch on the command
                    logger.User($"Attempting to invoke {command.Name} with {(command.Arguments.Count == 0 ? "no arguments" : "the following argument(s): " + string.Join(", ", command.Arguments))}");
                    switch (command.Name.ToLowerInvariant())
                    {
                        // Set a header field
                        case "set":
                            SetHeaderField(command, datFile);
                            break;

                        // Parse in new input file(s)
                        case "input":
                            ParseInputs(command, datFile, ref index);
                            break;

                        // Run DFD/D2D on path(s)
                        case "d2d":
                        case "dfd":
                            PopulateFromDir(command, datFile);
                            break;

                        // Apply a filter
                        case "filter":
                            ApplyFilter(command, datFile);
                            break;

                        // Apply an extra INI
                        case "extra":
                            ApplyExtra(command, datFile);
                            break;

                        // Apply internal split/merge
                        case "merge":
                            RunMerge(command, datFile);
                            break;

                        // Apply description-as-name logic
                        case "descname":
                            DescriptionAsName(command, datFile);
                            break;

                        // Apply 1G1R
                        case "1g1r":
                            OneGamePerRegion(command, datFile);
                            break;

                        // Apply one rom per game (ORPG)
                        case "orpg":
                            OneRomPerGame(command, datFile);
                            break;

                        // Remove a field
                        case "remove":
                            RemoveField(command, datFile);
                            break;

                        // Apply scene date stripping
                        case "sds":
                            SceneDateStrip(command, datFile);
                            break;

                        // Set new output format(s)
                        case "format":
                            SetOutputFormat(command, datFile);
                            break;

                        // Set output directory
                        case "output":
                            if (command.Arguments.Count != 1)
                            {
                                logger.User($"Invoked {command.Name} and expected exactly 1 argument, but {command.Arguments.Count} arguments were provided");
                                logger.User("Usage: output(outdir);");
                                continue;
                            }

                            // Only set the first as the output directory
                            outputDirectory = command.Arguments[0];
                            break;

                        // Write out the current DatFile
                        case "write":
                            Write(command, datFile, outputDirectory);
                            break;

                        // Reset the internal state
                        case "reset":
                            if (command.Arguments.Count != 0)
                            {
                                logger.User($"Invoked {command.Name} and expected no arguments, but {command.Arguments.Count} arguments were provided");
                                logger.User("Usage: reset();");
                                continue;
                            }

                            // Reset all state variables
                            index = 0;
                            datFile = DatFile.Create();
                            outputDirectory = null;
                            break;

                        default:
                            logger.User($"Could not find a match for '{command.Name}'. Please see the help text for more details.");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"There was an exception processing {path}");
            }
        }

        #region Batch Commands

        /// <summary>
        /// Apply a single extras file to the internal DAT
        /// </summary>
        /// <param name="command">BatchCommand representing the line</param>
        /// <param name="datFile">DatFile representing the internal DAT</param>
        private void ApplyExtra(BatchCommand command, DatFile datFile)
        {
            if (command.Arguments.Count != 2)
            {
                logger.User($"Invoked {command.Name} and expected 2 arguments, but {command.Arguments.Count} arguments were provided");
                logger.User("Usage: extra(field, inipath);");
                return;
            }

            // Read in the individual arguments
            MachineField extraMachineField = command.Arguments[0].AsMachineField();
            DatItemField extraDatItemField = command.Arguments[0].AsDatItemField();
            string extraFile = command.Arguments[1];

            // If we had an invalid input, log and continue
            if (extraMachineField == MachineField.NULL
                && extraDatItemField == DatItemField.NULL)
            {
                logger.User($"{command.Arguments[0]} was an invalid field name");
                return;
            }
            if (!File.Exists(command.Arguments[1]))
            {
                logger.User($"{command.Arguments[1]} was an invalid file name");
                return;
            }

            // Create the extra INI
            ExtraIni extraIni = new ExtraIni();
            ExtraIniItem extraIniItem = new ExtraIniItem
            {
                MachineField = extraMachineField,
                DatItemField = extraDatItemField,
            };
            extraIniItem.PopulateFromFile(extraFile);
            extraIni.Items.Add(extraIniItem);

            // Apply the extra INI blindly
            extraIni.ApplyExtras(datFile);
        }

        /// <summary>
        /// Apply a single filter to the internal DAT
        /// </summary>
        /// <param name="command">BatchCommand representing the line</param>
        /// <param name="datFile">DatFile representing the internal DAT</param>
        private void ApplyFilter(BatchCommand command, DatFile datFile)
        {
            if (command.Arguments.Count < 2 || command.Arguments.Count > 4)
            {
                logger.User($"Invoked {command.Name} and expected between 2-4 arguments, but {command.Arguments.Count} arguments were provided");
                logger.User("Usage: filter(field, value, [remove = false, [perMachine = false]]);");
                return;
            }

            // Read in the individual arguments
            DatHeaderField filterDatHeaderField = command.Arguments[0].AsDatHeaderField();
            MachineField filterMachineField = command.Arguments[0].AsMachineField();
            DatItemField filterDatItemField = command.Arguments[0].AsDatItemField();
            string filterValue = command.Arguments[1];
            bool? filterRemove = false;
            if (command.Arguments.Count >= 3)
                filterRemove = command.Arguments[2].AsYesNo();
            bool? filterPerMachine = false;
            if (command.Arguments.Count >= 4)
                filterPerMachine = command.Arguments[3].AsYesNo();

            // If we had an invalid input, log and continue
            if (filterDatHeaderField == DatHeaderField.NULL
                && filterMachineField == MachineField.NULL
                && filterDatItemField == DatItemField.NULL)
            {
                logger.User($"{command.Arguments[0]} was an invalid field name");
                return;
            }
            if (filterRemove == null)
            {
                logger.User($"{command.Arguments[2]} was an invalid true/false value");
                return;
            }
            if (filterPerMachine == null)
            {
                logger.User($"{command.Arguments[3]} was an invalid true/false value");
                return;
            }

            // Create filter to run filters from
            Filter filter = new Filter
            {
                MachineFilter = new MachineFilter { HasFilters = true },
                DatItemFilter = new DatItemFilter { HasFilters = true },
            };

            // Set the possible filters
            filter.MachineFilter.SetFilter(filterMachineField, filterValue, filterRemove.Value);
            filter.DatItemFilter.SetFilter(filterDatItemField, filterValue, filterRemove.Value);

            // Apply the filters blindly
            filter.ApplyFilters(datFile, filterPerMachine.Value);

            // Cleanup after the filter
            // TODO: We might not want to remove immediately
            datFile.Items.ClearMarked(); 
            datFile.Items.ClearEmpty();
        }

        /// <summary>
        /// Apply a description-as-name to the internal DAT
        /// </summary>
        /// <param name="command">BatchCommand representing the line</param>
        /// <param name="datFile">DatFile representing the internal DAT</param>
        private void DescriptionAsName(BatchCommand command, DatFile datFile)
        {
            if (command.Arguments.Count != 0)
            {
                logger.User($"Invoked {command.Name} and expected no arguments, but {command.Arguments.Count} arguments were provided");
                logger.User("Usage: descname();");
                return;
            }

            // Apply the logic
            Cleaner descNameCleaner = new Cleaner { DescriptionAsName = true };
            descNameCleaner.ApplyCleaning(datFile);
        }

        /// <summary>
        /// Apply 1G1R to the internal DAT
        /// </summary>
        /// <param name="command">BatchCommand representing the line</param>
        /// <param name="datFile">DatFile representing the internal DAT</param>
        private void OneGamePerRegion(BatchCommand command, DatFile datFile)
        {
            if (command.Arguments.Count == 0)
            {
                logger.User($"Invoked {command.Name} but no arguments were provided");
                logger.User("Usage: 1g1r(region, ...);");
                return;
            }

            // Run the 1G1R functionality
            Cleaner ogorCleaner = new Cleaner { OneGamePerRegion = true, RegionList = command.Arguments }; 
            ogorCleaner.ApplyCleaning(datFile);
        }

        /// <summary>
        /// Apply ORPG to the internal DAT
        /// </summary>
        /// <param name="command">BatchCommand representing the line</param>
        /// <param name="datFile">DatFile representing the internal DAT</param>
        private void OneRomPerGame(BatchCommand command, DatFile datFile)
        {
            if (command.Arguments.Count != 0)
            {
                logger.User($"Invoked {command.Name} and expected no arguments, but {command.Arguments.Count} arguments were provided");
                logger.User("Usage: orpg();");
                return;
            }

            // Apply the logic
            Cleaner orpgCleaner = new Cleaner { OneRomPerGame = true }; 
            orpgCleaner.ApplyCleaning(datFile);
        }

        /// <summary>
        /// Populate the internal DAT from one or more files
        /// </summary>
        /// <param name="command">BatchCommand representing the line</param>
        /// <param name="datFile">DatFile representing the internal DAT</param>
        /// <param name="index">Current input file index</param>
        private void ParseInputs(BatchCommand command, DatFile datFile, ref int index)
        {
            if (command.Arguments.Count == 0)
            {
                logger.User($"Invoked {command.Name} but no arguments were provided");
                logger.User("Usage: input(datpath, ...);");
                return;
            }

            // Get only files from inputs
            List<ParentablePath> datFilePaths = PathTool.GetFilesOnly(command.Arguments);

            // Assume there could be multiple
            foreach (ParentablePath datFilePath in datFilePaths)
            {
                Parser.ParseInto(datFile, datFilePath, index++);
            }
        }

        /// <summary>
        /// Populate the internal DAT from one or more paths
        /// </summary>
        /// <param name="command">BatchCommand representing the line</param>
        /// <param name="datFile">DatFile representing the internal DAT</param>
        private void PopulateFromDir(BatchCommand command, DatFile datFile)
        {
            if (command.Arguments.Count == 0)
            {
                logger.User($"Invoked {command.Name} but no arguments were provided");
                logger.User("Usage: d2d(path, ...);");
                return;
            }

            // TODO: Should any of the other options be added for D2D?

            // Assume there could be multiple
            foreach (string input in command.Arguments)
            {
                DatTools.DatFromDir.PopulateFromDir(datFile, input, hashes: Hash.Standard);
            }

            // TODO: We might not want to remove dates in the future
            Remover dfdRemover = new Remover();
            dfdRemover.PopulateExclusionsFromList(new List<string> { "DatItem.Date" });
            dfdRemover.ApplyRemovals(datFile);
        }

        /// <summary>
        /// Remove a field from the internal DAT
        /// </summary>
        /// <param name="command">BatchCommand representing the line</param>
        /// <param name="datFile">DatFile representing the internal DAT</param>
        private void RemoveField(BatchCommand command, DatFile datFile)
        {
            if (command.Arguments.Count == 0)
            {
                logger.User($"Invoked {command.Name} but no arguments were provided");
                logger.User("Usage: remove(field, ...);");
                return;
            }

            // Run the removal functionality
            Remover remover = new Remover();
            remover.PopulateExclusionsFromList(command.Arguments);
            remover.ApplyRemovals(datFile);
        }

        /// <summary>
        /// Run internal split/merge on the internal DAT
        /// </summary>
        /// <param name="command">BatchCommand representing the line</param>
        /// <param name="datFile">DatFile representing the internal DAT</param>
        private void RunMerge(BatchCommand command, DatFile datFile)
        {
            if (command.Arguments.Count != 1)
            {
                logger.User($"Invoked {command.Name} and expected 1 argument, but {command.Arguments.Count} arguments were provided");
                logger.User("Usage: merge(split|merged|nonmerged|full|device);");
                return;
            }

            // Read in the individual arguments
            MergingFlag mergingFlag = command.Arguments[0].AsMergingFlag();

            // If we had an invalid input, log and continue
            if (mergingFlag == MergingFlag.None)
            {
                logger.User($"{command.Arguments[0]} was an invalid merging flag");
                return;
            }

            // Apply the merging flag
            Filtering.Splitter splitter = new Filtering.Splitter { SplitType = mergingFlag };
            splitter.ApplySplitting(datFile, false);
        }

        /// <summary>
        /// Apply scene date stripping to the internal DAT
        /// </summary>
        /// <param name="command">BatchCommand representing the line</param>
        /// <param name="datFile">DatFile representing the internal DAT</param>
        private void SceneDateStrip(BatchCommand command, DatFile datFile)
        {
            if (command.Arguments.Count != 0)
            {
                logger.User($"Invoked {command.Name} and expected no arguments, but {command.Arguments.Count} arguments were provided");
                logger.User("Usage: sds();");
                return;
            }

            // Apply the logic
            Cleaner stripCleaner = new Cleaner { SceneDateStrip = true }; 
            stripCleaner.ApplyCleaning(datFile);
        }

        /// <summary>
        /// Set a single header field on the internal DAT
        /// </summary>
        /// <param name="command">BatchCommand representing the line</param>
        /// <param name="datFile">DatFile representing the internal DAT</param>
        private void SetHeaderField(BatchCommand command, DatFile datFile)
        {
            if (command.Arguments.Count != 2)
            {
                logger.User($"Invoked {command.Name} but no arguments were provided");
                logger.User("Usage: set(header.field, value);");
                return;
            }

            // Read in the individual arguments
            DatHeaderField field = command.Arguments[0].AsDatHeaderField();
            string value = command.Arguments[1];

            // If we had an invalid input, log and continue
            if (field == DatHeaderField.NULL)
            {
                logger.User($"{command.Arguments[0]} was an invalid field name");
                return;
            }

            // Set the header field
            datFile.Header.SetFields(new Dictionary<DatHeaderField, string> { [field] = value });
        }

        /// <summary>
        /// Set output DatFile format
        /// </summary>
        /// <param name="command">BatchCommand representing the line</param>
        /// <param name="datFile">DatFile representing the internal DAT</param>
        private void SetOutputFormat(BatchCommand command, DatFile datFile)
        {
            if (command.Arguments.Count == 0)
            {
                logger.User($"Invoked {command.Name} but no arguments were provided");
                logger.User("Usage: format(datformat, ...);");
                return;
            }

            // Assume there could be multiple
            datFile.Header.DatFormat = 0x00;
            foreach (string format in command.Arguments)
            {
                datFile.Header.DatFormat |= GetDatFormat(format);
            }

            // If we had an invalid input, log and continue
            if (datFile.Header.DatFormat == 0x00)
            {
                logger.User($"No valid output format found");
                return;
            }
        }

        /// <summary>
        /// Write out the internal DAT
        /// </summary>
        /// <param name="command">BatchCommand representing the line</param>
        /// <param name="datFile">DatFile representing the internal DAT</param>
        /// <param name="outputDirectory">Directory to write outputs to</param>
        private void Write(BatchCommand command, DatFile datFile, string outputDirectory)
        {
            if (command.Arguments.Count > 1)
            {
                logger.User($"Invoked {command.Name} and expected 0-1 arguments, but {command.Arguments.Count} arguments were provided");
                logger.User("Usage: write([overwrite = true]);");
                return;
            }

            // Get overwrite value, if possible
            bool? overwrite = true;
            if (command.Arguments.Count == 1)
                overwrite = command.Arguments[0].AsYesNo();

            // If we had an invalid input, log and continue
            if (overwrite == null)
            {
                logger.User($"{command.Arguments[0]} was an invalid true/false value");
                return;
            }

            // Write out the dat with the current state
            Writer.Write(datFile, outputDirectory, overwrite: overwrite.Value);
        }

        #endregion

        /// <summary>
        /// Internal representation of a single batch command
        /// </summary>
        /// TODO: Should there be individual commands like there's individual features?
        /// TODO: Should BatchCommand take care of the branching command values?
        /// TODO: Should BatchCommand be a part of SabreTools.DatTools?
        private class BatchCommand
        {
            public string Name { get; private set; }
            public List<string> Arguments { get; private set; } = new List<string>();

            /// <summary>
            /// Create a command based on parsing a line
            /// </summary>
            public static BatchCommand Create(string line)
            {
                // Empty lines don't count
                if (string.IsNullOrEmpty(line))
                    return null;

                // Split into name and arguments
                string splitRegex = @"^(\S+)\((.*?)\);";
                var match = Regex.Match(line, splitRegex);

                // If we didn't get a success, just return null
                if (!match.Success)
                    return null;

                // Otherwise, get the name and arguments
                string commandName = match.Groups[1].Value;
                List<string> arguments = match
                    .Groups[2]
                    .Value
                    .Split(',')
                    .Select(s => s.Trim().Trim('"').Trim())
                    .Where(s => !string.IsNullOrWhiteSpace(s)) // TODO: This may interfere with header value replacement
                    .ToList();

                return new BatchCommand { Name = commandName, Arguments = arguments };
            }
        }
    }
}
