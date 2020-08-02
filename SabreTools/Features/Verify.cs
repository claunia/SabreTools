using System.Collections.Generic;

using SabreTools.Library.DatFiles;
using SabreTools.Library.Filtering;
using SabreTools.Library.Help;
using SabreTools.Library.IO;
using SabreTools.Library.Tools;

namespace SabreTools.Features
{
    internal class Verify : BaseFeature
    {
        public const string Value = "Verify";

        public Verify()
        {
            Name = Value;
            Flags = new List<string>() { "-ve", "--verify" };
            Description = "Verify a folder against DATs";
            _featureType = FeatureType.Flag;
            LongDescription = "When used, this will use an input DAT or set of DATs to blindly check against an input folder. The base of the folder is considered the base for the combined DATs and games are either the directories or archives within. This will only do a direct verification of the items within and will create a fixdat afterwards for missing files.";
            Features = new Dictionary<string, Feature>();

            AddFeature(DatListInput);
            AddFeature(DepotFlag);
            AddFeature(TempStringInput);
            AddFeature(OutputDirStringInput);
            AddFeature(HashOnlyFlag);
            AddFeature(QuickFlag);
            AddFeature(HeaderStringInput);
            AddFeature(ChdsAsFilesFlag);
            AddFeature(IndividualFlag);
            AddInternalSplitFeatures();
            AddFilteringFeatures();
        }

        public override void ProcessFeatures(Dictionary<string, Feature> features)
        {
            base.ProcessFeatures(features);

            // Get a list of files from the input datfiles
            var datfiles = GetList(features, DatListValue);
            var datfilePaths = DirectoryExtensions.GetFilesOnly(datfiles);

            // Get feature flags
            bool chdsAsFiles = GetBoolean(features, ChdsAsFilesValue);
            bool depot = GetBoolean(features, DepotValue);
            bool hashOnly = GetBoolean(features, HashOnlyValue);
            bool quickScan = GetBoolean(features, QuickValue);

            // If we are in individual mode, process each DAT on their own
            if (GetBoolean(features, IndividualValue))
            {
                foreach (ParentablePath datfile in datfilePaths)
                {
                    DatFile datdata = DatFile.Create();
                    datdata.Parse(datfile, 99, keep: true);
                    Filter.FilterDatFile(datdata, true);
                    if (!string.IsNullOrEmpty(Header.HeaderSkipper))
                        datdata.Header.HeaderSkipper = Header.HeaderSkipper;

                    // If we have the depot flag, respect it
                    if (depot)
                        datdata.VerifyDepot(Inputs, OutputDir);
                    else
                        datdata.VerifyGeneric(Inputs, OutputDir, hashOnly, quickScan, chdsAsFiles, Filter);
                }
            }
            // Otherwise, process all DATs into the same output
            else
            {
                InternalStopwatch watch = new InternalStopwatch("Populating internal DAT");

                // Add all of the input DATs into one huge internal DAT
                DatFile datdata = DatFile.Create();
                foreach (ParentablePath datfile in datfilePaths)
                {
                    datdata.Parse(datfile, 99, keep: true);
                    Filter.FilterDatFile(datdata, true);
                }

                watch.Stop();

                // If we have the depot flag, respect it
                if (depot)
                    datdata.VerifyDepot(Inputs, OutputDir);
                else
                    datdata.VerifyGeneric(Inputs, OutputDir, hashOnly, quickScan, chdsAsFiles, Filter);
            }
        }
    }
}
