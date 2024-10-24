using System.Collections.Generic;
using System.IO;
using SabreTools.DatFiles;
using SabreTools.DatTools;
using SabreTools.FileTypes;
using SabreTools.Help;
using SabreTools.IO;
using SabreTools.IO.Logging;

namespace SabreTools.Features
{
    internal class Sort : BaseFeature
    {
        public const string Value = "Sort";

        public Sort()
        {
            Name = Value;
            Flags.AddRange(["ss", "sort"]);
            Description = "Sort inputs by a set of DATs";
            _featureType = ParameterType.Flag;
            LongDescription = "This feature allows the user to quickly rebuild based on a supplied DAT file(s). By default all files will be rebuilt to uncompressed folders in the output directory.";

            // Common Features
            AddCommonFeatures();

            AddFeature(DatListInput);
            AddFeature(OutputDirStringInput);
            AddFeature(DepotFlag);
            this[DepotFlag]!.AddFeature(DepotDepthInt32Input);
            AddFeature(DeleteFlag);
            AddFeature(InverseFlag);
            AddFeature(QuickFlag);
            AddFeature(AaruFormatsAsFilesFlag);
            AddFeature(ChdsAsFilesFlag);
            AddFeature(AddDateFlag);
            AddFeature(IndividualFlag);

            // Output Formats
            AddFeature(Torrent7zipFlag);
            AddFeature(TarFlag);
            AddFeature(TorrentGzipFlag);
            this[TorrentGzipFlag]!.AddFeature(RombaFlag);
            this[TorrentGzipFlag]![RombaFlag]!.AddFeature(RombaDepthInt32Input);
            //AddFeature(SharedInputs.TorrentRarFlag);
            //AddFeature(SharedInputs.TorrentXzFlag);
            //this[SharedInputs.TorrentXzFlag]!.AddFeature(SharedInputs.RombaFlag);
            AddFeature(TorrentZipFlag);

            AddFeature(HeaderStringInput);
            AddInternalSplitFeatures();
            AddFeature(UpdateDatFlag);
        }

        public override bool ProcessFeatures(Dictionary<string, Feature?> features)
        {
            // If the base fails, just fail out
            if (!base.ProcessFeatures(features))
                return false;

            // Get feature flags
            TreatAsFile asFiles = GetTreatAsFiles(features);
            bool date = GetBoolean(features, AddDateValue);
            bool delete = GetBoolean(features, DeleteValue);
            bool inverse = GetBoolean(features, InverseValue);
            bool quickScan = GetBoolean(features, QuickValue);
            bool updateDat = GetBoolean(features, UpdateDatValue);
            var outputFormat = GetOutputFormat(features);

            // Get the depots
            var inputDepot = Header!.GetFieldValue<DepotInformation?>(DatHeader.InputDepotKey);
            var outputDepot = Header!.GetFieldValue<DepotInformation?>(DatHeader.OutputDepotKey);

            // If we have the romba flag
            if (outputDepot?.IsActive == true)
            {
                // Update TorrentGzip output
                if (outputFormat == OutputFormat.TorrentGzip)
                    outputFormat = OutputFormat.TorrentGzipRomba;

                // Update TorrentXz output
                else if (outputFormat == OutputFormat.TorrentXZ)
                    outputFormat = OutputFormat.TorrentXZRomba;
            }

            // Get a list of files from the input datfiles
            var datfiles = GetList(features, DatListValue);
            var datfilePaths = PathTool.GetFilesOnly(datfiles);

            // If we are in individual mode, process each DAT on their own, appending the DAT name to the output dir
            if (GetBoolean(features, IndividualValue))
            {
                foreach (ParentablePath datfile in datfilePaths)
                {
                    DatFile datdata = DatFile.Create();
                    Parser.ParseInto(datdata, datfile, int.MaxValue, keep: true);

                    // Skip if nothing was parsed
                    if (datdata.Items.Count == 0) // datdata.ItemsDB.SortedKeys.Length == 0
                        continue;

                    // Set depot information
                    datdata.Header.SetFieldValue<DepotInformation?>(DatHeader.InputDepotKey, inputDepot?.Clone() as DepotInformation);
                    datdata.Header.SetFieldValue<DepotInformation?>(DatHeader.OutputDepotKey, outputDepot?.Clone() as DepotInformation);

                    // If we have overridden the header skipper, set it now
                    if (!string.IsNullOrEmpty(Header.GetStringFieldValue(Models.Metadata.Header.HeaderKey)))
                        datdata.Header.SetFieldValue<string?>(Models.Metadata.Header.HeaderKey, Header.GetStringFieldValue(Models.Metadata.Header.HeaderKey));

                    // If we have the depot flag, respect it
                    bool success;
                    if (inputDepot?.IsActive ?? false)
                        success = Rebuilder.RebuildDepot(datdata, Inputs, Path.Combine(OutputDir!, datdata.Header.GetStringFieldValue(DatHeader.FileNameKey)!), date, delete, inverse, outputFormat);
                    else
                        success = Rebuilder.RebuildGeneric(datdata, Inputs, Path.Combine(OutputDir!, datdata.Header.GetStringFieldValue(DatHeader.FileNameKey)!), quickScan, date, delete, inverse, outputFormat, asFiles);

                    // If we have a success and we're updating the DAT, write it out
                    if (success && updateDat)
                    {
                        datdata.Header.SetFieldValue<string?>(DatHeader.FileNameKey, $"fixDAT_{Header.GetStringFieldValue(DatHeader.FileNameKey)}");
                        datdata.Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, $"fixDAT_{Header.GetStringFieldValue(Models.Metadata.Header.NameKey)}");
                        datdata.Header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, $"fixDAT_{Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey)}");
                        datdata.Items.ClearMarked();
                        datdata.ItemsDB.ClearMarked();
                        Writer.Write(datdata, OutputDir);
                    }
                }
            }

            // Otherwise, process all DATs into the same output
            else
            {
                var watch = new InternalStopwatch("Populating internal DAT");

                // Add all of the input DATs into one huge internal DAT
                DatFile datdata = DatFile.Create();
                foreach (ParentablePath datfile in datfilePaths)
                {
                    Parser.ParseInto(datdata, datfile, int.MaxValue, keep: true);
                }

                // Set depot information
                datdata.Header.SetFieldValue<DepotInformation?>(DatHeader.InputDepotKey, inputDepot?.Clone() as DepotInformation);
                datdata.Header.SetFieldValue<DepotInformation?>(DatHeader.OutputDepotKey, outputDepot?.Clone() as DepotInformation);

                // If we have overridden the header skipper, set it now
                string? headerSkpper = Header.GetStringFieldValue(Models.Metadata.Header.HeaderKey);
                if (!string.IsNullOrEmpty(headerSkpper))
                    datdata.Header.SetFieldValue<string?>(Models.Metadata.Header.HeaderKey, headerSkpper);

                watch.Stop();

                // If we have the depot flag, respect it
                bool success;
                if (inputDepot?.IsActive ?? false)
                    success = Rebuilder.RebuildDepot(datdata, Inputs, OutputDir!, date, delete, inverse, outputFormat);
                else
                    success = Rebuilder.RebuildGeneric(datdata, Inputs, OutputDir!, quickScan, date, delete, inverse, outputFormat, asFiles);

                // If we have a success and we're updating the DAT, write it out
                if (success && updateDat)
                {
                    datdata.Header.SetFieldValue<string?>(DatHeader.FileNameKey,
                        $"fixDAT_{Header.GetStringFieldValue(DatHeader.FileNameKey)}");
                    datdata.Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey,
                        $"fixDAT_{Header.GetStringFieldValue(Models.Metadata.Header.NameKey)}");
                    datdata.Header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey,
                        $"fixDAT_{Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey)}");

                    datdata.Items.ClearMarked();
                    datdata.ItemsDB.ClearMarked();
                    Writer.Write(datdata, OutputDir);
                }
            }

            return true;
        }
    }
}
