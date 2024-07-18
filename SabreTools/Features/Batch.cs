using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using SabreTools.Core.Filter;
using SabreTools.Core.Tools;
using SabreTools.DatFiles;
using SabreTools.DatTools;
using SabreTools.Filtering;
using SabreTools.Hashing;
using SabreTools.Help;
using SabreTools.IO;
using SabreTools.Logging;

namespace SabreTools.Features
{
    // TODO: Should the private classes here be split into a new namespace?
    internal class Batch : BaseFeature
    {
        public const string Value = "Batch";

        public Batch()
        {
            Name = Value;
            Flags.AddRange(["bt", "batch"]);
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

            // Common Features
            AddCommonFeatures();
        }

        public override bool ProcessFeatures(Dictionary<string, Feature?> features)
        {
            // If the base fails, just fail out
            if (!base.ProcessFeatures(features))
                return false;

            // Try to read each input as a batch run file
            foreach (string path in Inputs)
            {
                var watch = new InternalStopwatch($"Processing '{path}'...");
                ProcessScript(path);
                watch.Stop();
            }

            return true;
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
                var batchState = new BatchState();

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
                        logger.User($"Please see the help text for more details about possible commands");
                        break;
                    }

                    // Validate that the command has the proper number and type of arguments
                    (bool valid, string? error) = command.ValidateArguments();
                    if (!valid)
                    {
                        logger.User(error ?? string.Empty);
                        logger.User($"Usage: {command.Usage()}");
                        break;
                    }

                    // Now run the command
                    logger.User($"Attempting to invoke {command.Name} with {(command.Arguments.Count == 0 ? "no arguments" : "the following argument(s): " + string.Join(", ", command.Arguments))}");
                    command.Process(batchState);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"There was an exception processing {path}");
            }
        }

        #region Commands

        /// <summary>
        /// Internal representation of a single batch command
        /// </summary>
        private abstract class BatchCommand
        {
            public string? Name { get; set; }
            public List<string> Arguments { get; private set; } = [];

            /// <summary>
            /// Default constructor for setting arguments
            /// </summary>
            public BatchCommand(List<string> arguments)
            {
                Arguments = arguments;
            }

            /// <summary>
            /// Create a command based on parsing a line
            /// </summary>
            /// <param name="line">Current line to parse into a command</param>
            public static BatchCommand? Create(string line)
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

                return commandName.ToLowerInvariant() switch
                {
                    "1g1r" => new OneGamePerRegionCommand(arguments),
                    "d2d" => new DFDCommand(arguments),
                    "dfd" => new DFDCommand(arguments),
                    "descname" => new DescriptionAsNameCommand(arguments),
                    "extra" => new ExtraCommand(arguments),
                    "filter" => new FilterCommand(arguments),
                    "format" => new FormatCommand(arguments),
                    "input" => new InputCommand(arguments),
                    "merge" => new MergeCommand(arguments),
                    "orpg" => new OneRomPerGameCommand(arguments),
                    "output" => new OutputCommand(arguments),
                    "remove" => new RemoveCommand(arguments),
                    "reset" => new ResetCommand(arguments),
                    "sds" => new SceneDateStripCommand(arguments),
                    "set" => new SetCommand(arguments),
                    "write" => new WriteCommand(arguments),
                    _ => null,
                };
            }

            /// <summary>
            /// Return usage string for a given command
            /// </summary>
            public abstract string Usage();

            /// <summary>
            /// Validate that a set of arguments are sufficient for a given command
            /// </summary>
            public abstract (bool, string?) ValidateArguments();

            /// <summary>
            /// Process a batch file state with the current command
            /// </summary>
            /// <param name="batchState">Current batch file state to work on</param>
            public abstract void Process(BatchState batchState);
        }

        /// <summary>
        /// Apply description-as-name logic
        /// </summary>
        private class DescriptionAsNameCommand : BatchCommand
        {
            /// <inheritdoc/>
            public DescriptionAsNameCommand(List<string> arguments) : base(arguments) { }

            /// <inheritdoc/>
            public override string Usage()
            {
                return "descname();";
            }

            /// <inheritdoc/>
            public override (bool, string?) ValidateArguments()
            {
                if (Arguments.Count != 0)
                {
                    string message = $"Invoked {Name} and expected no arguments, but {Arguments.Count} arguments were provided";
                    return (false, message);
                }

                return (true, null);
            }

            /// <inheritdoc/>
            public override void Process(BatchState batchState)
            {
                Cleaner descNameCleaner = new() { DescriptionAsName = true };
                descNameCleaner.ApplyCleaning(batchState.DatFile);
            }
        }

        /// <summary>
        /// Run DFD/D2D on path(s)
        /// </summary>
        private class DFDCommand : BatchCommand
        {
            /// <inheritdoc/>
            public DFDCommand(List<string> arguments) : base(arguments) { }

            /// <inheritdoc/>
            public override string Usage()
            {
                return "d2d(path, ...);";
            }

            /// <inheritdoc/>
            public override (bool, string?) ValidateArguments()
            {
                if (Arguments.Count == 0)
                {
                    string message = $"Invoked {Name} but no arguments were provided";
                    return (false, message);
                }

                return (true, null);
            }

            /// <inheritdoc/>
            /// <remarks>TODO: Should any of the other options be added for D2D?</remarks>
            public override void Process(BatchState batchState)
            {
                // Assume there could be multiple
                foreach (string input in Arguments)
                {
                    DatTools.DatFromDir.PopulateFromDir(batchState.DatFile, input, hashes: [HashType.CRC32, HashType.MD5, HashType.SHA1]);
                }

                // TODO: We might not want to remove dates in the future
                Remover dfdRemover = new();
                dfdRemover.PopulateExclusionsFromList(new List<string> { "DatItem.Date" });
                dfdRemover.ApplyRemovals(batchState.DatFile);
            }
        }

        /// <summary>
        /// Apply an extra INI
        /// </summary>
        private class ExtraCommand : BatchCommand
        {
            /// <inheritdoc/>
            public ExtraCommand(List<string> arguments) : base(arguments) { }

            /// <inheritdoc/>
            public override string Usage()
            {
                return "extra(field, inipath);";
            }

            /// <inheritdoc/>
            public override (bool, string?) ValidateArguments()
            {
                if (Arguments.Count != 2)
                {
                    string message = $"Invoked {Name} and expected 2 arguments, but {Arguments.Count} arguments were provided";
                    return (false, message);
                }

                // Read in the individual arguments
                (string? type, string? key) = FilterParser.ParseFilterId(Arguments[0]);
                string extraFile = Arguments[1];

                // If we had an invalid input, log and continue
                if (type == null && key == null)
                {
                    string message = $"{Arguments[0]} was an invalid field name";
                    return (false, message);
                }
                if (!File.Exists(extraFile))
                {
                    string message = $"{Arguments[1]} was an invalid file name";
                    return (false, message);
                }

                return (true, null);
            }

            /// <inheritdoc/>
            public override void Process(BatchState batchState)
            {
                // Read in the individual arguments
                (string?, string?) fieldName = FilterParser.ParseFilterId(Arguments[0]);
                string extraFile = Arguments[1];

                // Create the extra INI
                var extraIni = new ExtraIni();
                var extraIniItem = new ExtraIniItem() { FieldName = fieldName };
                extraIniItem.PopulateFromFile(extraFile);
                extraIni.Items.Add(extraIniItem);

                // Apply the extra INI blindly
                extraIni.ApplyExtras(batchState.DatFile);
                extraIni.ApplyExtrasDB(batchState.DatFile);
            }
        }

        /// <summary>
        /// Apply a filter
        /// </summary>
        private class FilterCommand : BatchCommand
        {
            /// <inheritdoc/>
            public FilterCommand(List<string> arguments) : base(arguments) { }

            /// <inheritdoc/>
            public override string Usage()
            {
                return "filter(field, value, [remove = false, [perMachine = false]]);";
            }

            /// <inheritdoc/>
            public override (bool, string?) ValidateArguments()
            {
                if (Arguments.Count < 2 || Arguments.Count > 4)
                {
                    string message = $"Invoked {Name} and expected between 2-4 arguments, but {Arguments.Count} arguments were provided";
                    return (false, message);
                }

                // Read in the individual arguments
                (string? type, string? key) = FilterParser.ParseFilterId(Arguments[0]);
                bool? filterRemove = false;
                if (Arguments.Count >= 3)
                    filterRemove = Arguments[2].AsYesNo();
                bool? filterPerMachine = false;
                if (Arguments.Count >= 4)
                    filterPerMachine = Arguments[3].AsYesNo();

                // If we had an invalid input, log and continue
                if (type == null && key == null)
                {
                    string message = $"{Arguments[0]} was an invalid field name";
                    return (false, message);
                }
                if (filterRemove == null)
                {
                    string message = $"{Arguments[2]} was an invalid true/false value";
                    return (false, message);
                }
                if (filterPerMachine == null)
                {
                    string message = $"{Arguments[3]} was an invalid true/false value";
                    return (false, message);
                }

                return (true, null);
            }

            /// <inheritdoc/>
            public override void Process(BatchState batchState)
            {
                // Read in the individual arguments
                string filterField = Arguments[0];
                string filterValue = Arguments[1];
                bool? filterRemove = false;
                if (Arguments.Count >= 3)
                    filterRemove = Arguments[2].AsYesNo();
                // TODO: Add back this functionality
                bool? filterPerMachine = false;
                if (Arguments.Count >= 4)
                    filterPerMachine = Arguments[3].AsYesNo();

                // Build the filter statement
                string filterString = $"{filterField}{(filterRemove == true ? "!" : string.Empty)}:{filterValue}";

                // Create filter to run filters from
                var filter = new FilterRunner([filterString]);

                // Apply the filters blindly
                batchState.DatFile.ExecuteFilters(filter);

                // Cleanup after the filter
                // TODO: We might not want to remove immediately
                batchState.DatFile.Items.ClearMarked();
                batchState.DatFile.ItemsDB.ClearMarked();
                batchState.DatFile.Items.ClearEmpty();
                batchState.DatFile.ItemsDB.ClearEmpty();
            }
        }

        /// <summary>
        /// Set new output format(s)
        /// </summary>
        private class FormatCommand : BatchCommand
        {
            /// <inheritdoc/>
            public FormatCommand(List<string> arguments) : base(arguments) { }

            /// <inheritdoc/>
            public override string Usage()
            {
                return "format(datformat, ...);";
            }

            /// <inheritdoc/>
            public override (bool, string?) ValidateArguments()
            {
                if (Arguments.Count == 0)
                {
                    string message = $"Invoked {Name} but no arguments were provided";
                    return (false, message);
                }

                // Check all inputs to be valid formats
                List<string> unmappedFormats = new();
                foreach (string format in Arguments)
                {
                    if (GetDatFormat(format) == 0x0)
                        unmappedFormats.Add(format);
                }

                // If we had any unmapped formats, return an issue
                if (unmappedFormats.Any())
                {
                    string message = $"The following inputs were invalid formats: {string.Join(", ", unmappedFormats)}";
                    return (false, message);
                }

                return (true, null);
            }

            /// <inheritdoc/>
            public override void Process(BatchState batchState)
            {
                // Assume there could be multiple
                batchState.DatFile.Header.SetFieldValue<DatFormat>(DatHeader.DatFormatKey, 0x00);
                foreach (string format in Arguments)
                {
                    DatFormat currentFormat = batchState.DatFile.Header.GetFieldValue<DatFormat>(DatHeader.DatFormatKey);
                    batchState.DatFile.Header.SetFieldValue(DatHeader.DatFormatKey, currentFormat | GetDatFormat(format));
                }
            }
        }

        /// <summary>
        /// Parse in new input file(s)
        /// </summary>
        private class InputCommand : BatchCommand
        {
            /// <inheritdoc/>
            public InputCommand(List<string> arguments) : base(arguments) { }

            /// <inheritdoc/>
            public override string Usage()
            {
                return "input(datpath, ...);";
            }

            /// <inheritdoc/>
            public override (bool, string?) ValidateArguments()
            {
                if (Arguments.Count == 0)
                {
                    string message = $"Invoked {Name} but no arguments were provided";
                    return (false, message);
                }

                return (true, null);
            }

            /// <inheritdoc/>
            public override void Process(BatchState batchState)
            {
                // Get only files from inputs
                List<ParentablePath> datFilePaths = PathTool.GetFilesOnly(Arguments);

                // Assume there could be multiple
                foreach (ParentablePath datFilePath in datFilePaths)
                {
                    Parser.ParseInto(batchState.DatFile, datFilePath, batchState.Index++);
                }
            }
        }

        /// <summary>
        /// Apply internal split/merge
        /// </summary>
        private class MergeCommand : BatchCommand
        {
            /// <inheritdoc/>
            public MergeCommand(List<string> arguments) : base(arguments) { }

            /// <inheritdoc/>
            public override string Usage()
            {
                return "merge(split|merged|nonmerged|full|device);";
            }

            /// <inheritdoc/>
            public override (bool, string?) ValidateArguments()
            {
                if (Arguments.Count != 1)
                {
                    string message = $"Invoked {Name} and expected 1 argument, but {Arguments.Count} arguments were provided";
                    return (false, message);
                }

                // Read in the individual arguments
                MergingFlag mergingFlag = Arguments[0].AsEnumValue<MergingFlag>();

                // If we had an invalid input, log and continue
                if (mergingFlag == MergingFlag.None)
                {
                    string message = $"{Arguments[0]} was an invalid merging flag";
                    return (false, message);
                }

                return (true, null);
            }

            /// <inheritdoc/>
            public override void Process(BatchState batchState)
            {
                // Read in the individual arguments
                MergingFlag mergingFlag = Arguments[0].AsEnumValue<MergingFlag>();

                // Apply the merging flag
                Filtering.Splitter splitter = new() { SplitType = mergingFlag };
                splitter.ApplySplitting(batchState.DatFile, false, false);
            }
        }

        /// <summary>
        /// Apply 1G1R
        /// </summary>
        private class OneGamePerRegionCommand : BatchCommand
        {
            /// <inheritdoc/>
            public OneGamePerRegionCommand(List<string> arguments) : base(arguments) { }

            /// <inheritdoc/>
            public override string Usage()
            {
                return "1g1r(region, ...);";
            }

            /// <inheritdoc/>
            public override (bool, string?) ValidateArguments()
            {
                if (Arguments.Count == 0)
                {
                    string message = $"Invoked {Name} but no arguments were provided";
                    return (false, message);
                }

                return (true, null);
            }

            /// <inheritdoc/>
            public override void Process(BatchState batchState)
            {
                Cleaner ogorCleaner = new() { OneGamePerRegion = true, RegionList = Arguments };
                ogorCleaner.ApplyCleaning(batchState.DatFile);
            }
        }

        /// <summary>
        /// Apply one rom per game (ORPG)
        /// </summary>
        private class OneRomPerGameCommand : BatchCommand
        {
            /// <inheritdoc/>
            public OneRomPerGameCommand(List<string> arguments) : base(arguments) { }

            /// <inheritdoc/>
            public override string Usage()
            {
                return "orpg();";
            }

            /// <inheritdoc/>
            public override (bool, string?) ValidateArguments()
            {
                if (Arguments.Count == 0)
                {
                    string message = $"Invoked {Name} and expected no arguments, but {Arguments.Count} arguments were provided";
                    return (false, message);
                }

                return (true, null);
            }

            /// <inheritdoc/>
            public override void Process(BatchState batchState)
            {
                Cleaner orpgCleaner = new() { OneRomPerGame = true };
                orpgCleaner.ApplyCleaning(batchState.DatFile);
            }
        }

        /// <summary>
        /// Set output directory
        /// </summary>
        private class OutputCommand : BatchCommand
        {
            /// <inheritdoc/>
            public OutputCommand(List<string> arguments) : base(arguments) { }

            /// <inheritdoc/>
            public override string Usage()
            {
                return "output(outdir);";
            }

            /// <inheritdoc/>
            public override (bool, string?) ValidateArguments()
            {
                if (Arguments.Count != 1)
                {
                    string message = $"Invoked {Name} and expected exactly 1 argument, but {Arguments.Count} arguments were provided";
                    return (false, message);
                }

                return (true, null);
            }

            /// <inheritdoc/>
            public override void Process(BatchState batchState)
            {
                batchState.OutputDirectory = Arguments[0];
            }
        }

        /// <summary>
        /// Remove field(s)
        /// </summary>
        private class RemoveCommand : BatchCommand
        {
            /// <inheritdoc/>
            public RemoveCommand(List<string> arguments) : base(arguments) { }

            /// <inheritdoc/>
            public override string Usage()
            {
                return "remove(field, ...);";
            }

            /// <inheritdoc/>
            public override (bool, string?) ValidateArguments()
            {
                if (Arguments.Count == 0)
                {
                    string message = $"Invoked {Name} but no arguments were provided";
                    return (false, message);
                }

                return (true, null);
            }

            /// <inheritdoc/>
            public override void Process(BatchState batchState)
            {
                var remover = new Remover();
                remover.PopulateExclusionsFromList(Arguments);
                remover.ApplyRemovals(batchState.DatFile);
            }
        }

        /// <summary>
        /// Reset the internal state
        /// </summary>
        private class ResetCommand : BatchCommand
        {
            /// <inheritdoc/>
            public ResetCommand(List<string> arguments) : base(arguments) { }

            /// <inheritdoc/>
            public override string Usage()
            {
                return "reset();";
            }

            /// <inheritdoc/>
            public override (bool, string?) ValidateArguments()
            {
                if (Arguments.Count != 0)
                {
                    string message = $"Invoked {Name} and expected no arguments, but {Arguments.Count} arguments were provided";
                    return (false, message);
                }

                return (true, null);
            }

            /// <inheritdoc/>
            public override void Process(BatchState batchState)
            {
                batchState.Reset();
            }
        }

        /// <summary>
        /// Apply scene date stripping
        /// </summary>
        private class SceneDateStripCommand : BatchCommand
        {
            /// <inheritdoc/>
            public SceneDateStripCommand(List<string> arguments) : base(arguments) { }

            /// <inheritdoc/>
            public override string Usage()
            {
                return "sds();";
            }

            /// <inheritdoc/>
            public override (bool, string?) ValidateArguments()
            {
                if (Arguments.Count != 0)
                {
                    string message = $"Invoked {Name} and expected no arguments, but {Arguments.Count} arguments were provided";
                    return (false, message);
                }

                return (true, null);
            }

            /// <inheritdoc/>
            public override void Process(BatchState batchState)
            {
                Cleaner stripCleaner = new() { SceneDateStrip = true };
                stripCleaner.ApplyCleaning(batchState.DatFile);
            }
        }

        /// <summary>
        /// Set a header field
        /// </summary>
        private class SetCommand : BatchCommand
        {
            /// <inheritdoc/>
            public SetCommand(List<string> arguments) : base(arguments) { }

            /// <inheritdoc/>
            public override string Usage()
            {
                return "set(header.field, value);";
            }

            /// <inheritdoc/>
            public override (bool, string?) ValidateArguments()
            {
                if (Arguments.Count != 2)
                {
                    string message = $"Invoked {Name} but no arguments were provided";
                    return (false, message);
                }

                // Read in the individual arguments
                (string? type, string? key) = FilterParser.ParseFilterId(Arguments[0]);

                // If we had an invalid input, log and continue
                if ((type == null || !string.Equals(type, Models.Metadata.MetadataFile.HeaderKey, StringComparison.OrdinalIgnoreCase)) && key == null)
                {
                    string message = $"{Arguments[0]} was an invalid field name";
                    return (false, message);
                }

                return (true, null);
            }

            /// <inheritdoc/>
            public override void Process(BatchState batchState)
            {
                // Read in the individual arguments
                string field = Arguments[0];
                string value = Arguments[1];

                var setter = new Setter();
                setter.PopulateSetters(field, value);

                // Set the header field
                setter.SetFields(batchState.DatFile.Header);
            }
        }

        /// <summary>
        /// Write out the current DatFile
        /// </summary>
        private class WriteCommand : BatchCommand
        {
            /// <inheritdoc/>
            public WriteCommand(List<string> arguments) : base(arguments) { }

            /// <inheritdoc/>
            public override string Usage()
            {
                return "write([overwrite = true]);";
            }

            /// <inheritdoc/>
            public override (bool, string?) ValidateArguments()
            {
                if (Arguments.Count > 1)
                {
                    string message = $"Invoked {Name} and expected 0-1 arguments, but {Arguments.Count} arguments were provided";
                    return (false, message);
                }

                // Get overwrite value, if possible
                bool? overwrite = true;
                if (Arguments.Count == 1)
                    overwrite = Arguments[0].AsYesNo();

                // If we had an invalid input, log and continue
                if (overwrite == null)
                {
                    string message = $"{Arguments[0]} was an invalid true/false value";
                    return (false, message);
                }

                return (true, null);
            }

            /// <inheritdoc/>
            public override void Process(BatchState batchState)
            {
                // Get overwrite value, if possible
                bool overwrite = true;
                if (Arguments.Count == 1)
                    overwrite = Arguments[0].AsYesNo() ?? true;

                // Write out the dat with the current state
                Writer.Write(batchState.DatFile, batchState.OutputDirectory, overwrite: overwrite);
            }
        }

        #endregion

        #region Private Helper Classes

        /// <summary>
        /// Internal representation of a single batch file state
        /// </summary>
        private class BatchState
        {
            public DatFile DatFile { get; set; } = DatFile.Create();
            public int Index { get; set; } = 0;
            public string? OutputDirectory { get; set; } = null;

            /// <summary>
            /// Reset the current state
            /// </summary>
            public void Reset()
            {
                this.Index = 0;
                this.DatFile = DatFile.Create();
                this.OutputDirectory = null;
            }
        }

        #endregion
    }
}
