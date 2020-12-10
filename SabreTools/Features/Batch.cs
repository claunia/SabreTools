using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.DatFiles;
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
        }

        public override void ProcessFeatures(Dictionary<string, Help.Feature> features)
        {
            base.ProcessFeatures(features);

            // Try to read each input as a batch run file
            foreach (string path in Inputs)
            {
                // If the file doesn't exist, warn but continue
                if (!File.Exists(path))
                {
                    logger.User($"{path} does not exist. Skipping...");
                    continue;
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
                                if (command.Arguments.Count != 2)
                                {
                                    logger.User($"Invoked {command.Name} but no arguments were provided");
                                    logger.User("Usage: set(header.field, value);");
                                    continue;
                                }

                                // Read in the individual arguments
                                Field field = command.Arguments[0].AsField();
                                string value = command.Arguments[1];

                                // If we had an invalid input, log and continue
                                if (field == Field.NULL)
                                {
                                    logger.User($"{command.Arguments[0]} was an invalid field name");
                                    continue;
                                }

                                // Set the header field
                                datFile.Header.SetFields(new Dictionary<Field, string> { [field] = value });

                                break;

                            // Parse in new input file(s)
                            case "input":
                                if (command.Arguments.Count == 0)
                                {
                                    logger.User($"Invoked {command.Name} but no arguments were provided");
                                    logger.User("Usage: input(datpath, ...);");
                                    continue;
                                }

                                // Get only files from inputs
                                List<ParentablePath> datFilePaths = DirectoryExtensions.GetFilesOnly(command.Arguments);

                                // Assume there could be multiple
                                foreach (ParentablePath datFilePath in datFilePaths)
                                {
                                    DatTool.ParseInto(datFile, datFilePath, index++);
                                }

                                break;

                            // Run DFD/D2D on path(s)
                            case "d2d":
                            case "dfd":
                                if (command.Arguments.Count == 0)
                                {
                                    logger.User($"Invoked {command.Name} but no arguments were provided");
                                    logger.User("Usage: d2d(path, ...);");
                                    continue;
                                }

                                // TODO: Should any of the other options be added for D2D?

                                // Assume there could be multiple
                                foreach (string input in command.Arguments)
                                {
                                    DirFromDat.PopulateFromDir(datFile, input);
                                }

                                // TODO: We might not want to remove higher order hashes in the future
                                // TODO: We might not want to remove dates in the future
                                Cleaner dfdCleaner = new Cleaner() { ExcludeFields = Hash.DeepHashes.AsFields() };
                                dfdCleaner.ExcludeFields.Add(Field.DatItem_Date);
                                DatTool.ApplyCleaning(datFile, dfdCleaner);

                                break;

                            // Apply a filter
                            case "filter":
                                if (command.Arguments.Count < 2 || command.Arguments.Count > 4)
                                {
                                    logger.User($"Invoked {command.Name} and expected between 2-4 arguments, but {command.Arguments.Count} arguments were provided");
                                    logger.User("Usage: filter(field, value, [remove = false, [perMachine = false]]);");
                                    continue;
                                }

                                // Read in the individual arguments
                                Field filterField = command.Arguments[0].AsField();
                                string filterValue = command.Arguments[1];
                                bool? filterRemove = false;
                                if (command.Arguments.Count >= 3)
                                    filterRemove = command.Arguments[2].AsYesNo();
                                bool? filterPerMachine = false;
                                if (command.Arguments.Count >= 4)
                                    filterPerMachine = command.Arguments[3].AsYesNo();

                                // If we had an invalid input, log and continue
                                if (filterField == Field.NULL)
                                {
                                    logger.User($"{command.Arguments[0]} was an invalid field name");
                                    continue;
                                }
                                if (filterRemove == null)
                                {
                                    logger.User($"{command.Arguments[2]} was an invalid true/false value");
                                    continue;
                                }
                                if (filterPerMachine == null)
                                {
                                    logger.User($"{command.Arguments[3]} was an invalid true/false value");
                                    continue;
                                }

                                // Create a filter with this new set of fields
                                Filter filter = new Filter();
                                filter.SetFilter(filterField, filterValue, filterRemove.Value);

                                // Apply the filter blindly
                                DatTool.ApplyFilter(datFile, filter, filterPerMachine.Value);

                                // Cleanup after the filter
                                // TODO: We might not want to remove immediately
                                datFile.Items.ClearMarked(); 
                                datFile.Items.ClearEmpty();

                                break;

                            // Apply an extra INI
                            case "extra":
                                if (command.Arguments.Count != 2)
                                {
                                    logger.User($"Invoked {command.Name} and expected 2 arguments, but {command.Arguments.Count} arguments were provided");
                                    logger.User("Usage: extra(field, inipath);");
                                    continue;
                                }

                                // Read in the individual arguments
                                Field extraField = command.Arguments[0].AsField();
                                string extraFile = command.Arguments[1];

                                // If we had an invalid input, log and continue
                                if (extraField == Field.NULL)
                                {
                                    logger.User($"{command.Arguments[0]} was an invalid field name");
                                    continue;
                                }
                                if (!File.Exists(command.Arguments[1]))
                                {
                                    logger.User($"{command.Arguments[1]} was an invalid file name");
                                    continue;
                                }

                                // Create the extra INI
                                ExtraIni extraIni = new ExtraIni();
                                ExtraIniItem extraIniItem = new ExtraIniItem();
                                extraIniItem.PopulateFromFile(extraFile);
                                extraIniItem.Field = extraField;
                                extraIni.Items.Add(extraIniItem);

                                // Apply the extra INI blindly
                                DatTool.ApplyExtras(datFile, extraIni);

                                break;

                            // Apply internal split/merge
                            case "merge":
                                if (command.Arguments.Count != 1)
                                {
                                    logger.User($"Invoked {command.Name} and expected 1 argument, but {command.Arguments.Count} arguments were provided");
                                    logger.User("Usage: merge(split|merged|nonmerged|full|device);");
                                    continue;
                                }

                                // Read in the individual arguments
                                MergingFlag mergingFlag = command.Arguments[0].AsMergingFlag();

                                // If we had an invalid input, log and continue
                                if (mergingFlag == MergingFlag.None)
                                {
                                    logger.User($"{command.Arguments[0]} was an invalid merging flag");
                                    continue;
                                }

                                // Apply the merging flag
                                DatTool.ApplySplitting(datFile, mergingFlag, false);

                                break;

                            // Apply description-as-name logic
                            case "descname":
                                if (command.Arguments.Count != 0)
                                {
                                    logger.User($"Invoked {command.Name} and expected no arguments, but {command.Arguments.Count} arguments were provided");
                                    logger.User("Usage: descname();");
                                    continue;
                                }

                                // Apply the logic
                                DatTool.MachineDescriptionToName(datFile);

                                break;

                            // Apply 1G1R
                            case "1g1r":
                                if (command.Arguments.Count == 0)
                                {
                                    logger.User($"Invoked {command.Name} but no arguments were provided");
                                    logger.User("Usage: 1g1r(region, ...);");
                                    continue;
                                }

                                // Run the 1G1R functionality
                                DatTool.OneGamePerRegion(datFile, command.Arguments);

                                break;

                            // Apply one rom per game (ORPG)
                            case "orpg":
                                if (command.Arguments.Count != 0)
                                {
                                    logger.User($"Invoked {command.Name} and expected no arguments, but {command.Arguments.Count} arguments were provided");
                                    logger.User("Usage: orpg();");
                                    continue;
                                }

                                // Apply the logic
                                DatTool.OneRomPerGame(datFile);

                                break;

                            // Remove a field
                            case "remove":
                                if (command.Arguments.Count == 0)
                                {
                                    logger.User($"Invoked {command.Name} but no arguments were provided");
                                    logger.User("Usage: remove(field, ...);");
                                    continue;
                                }

                                // Run the removal functionality
                                DatTool.RemoveFieldsFromItems(datFile, command.Arguments.Select(s => s.AsField()).ToList());

                                break;

                            // Apply scene date stripping
                            case "sds":
                                if (command.Arguments.Count != 0)
                                {
                                    logger.User($"Invoked {command.Name} and expected no arguments, but {command.Arguments.Count} arguments were provided");
                                    logger.User("Usage: sds();");
                                    continue;
                                }

                                // Apply the logic
                                DatTool.StripSceneDatesFromItems(datFile);

                                break;

                            // Set new output format(s)
                            case "format":
                                if (command.Arguments.Count == 0)
                                {
                                    logger.User($"Invoked {command.Name} but no arguments were provided");
                                    logger.User("Usage: format(datformat, ...);");
                                    continue;
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
                                    continue;
                                }

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
                                if (command.Arguments.Count > 1)
                                {
                                    logger.User($"Invoked {command.Name} and expected 0-1 arguments, but {command.Arguments.Count} arguments were provided");
                                    logger.User("Usage: write([overwrite = true]);");
                                    continue;
                                }

                                // Get overwrite value, if possible
                                bool? overwrite = true;
                                if (command.Arguments.Count == 1)
                                    overwrite = command.Arguments[0].AsYesNo();

                                // If we had an invalid input, log and continue
                                if (overwrite == null)
                                {
                                    logger.User($"{command.Arguments[0]} was an invalid true/false value");
                                    continue;
                                }

                                // Write out the dat with the current state
                                DatTool.Write(datFile, outputDirectory, overwrite: overwrite.Value);
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
                    continue;
                }
            }
        }

        /// <summary>
        /// Internal representation of a single batch command
        /// </summary>
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
