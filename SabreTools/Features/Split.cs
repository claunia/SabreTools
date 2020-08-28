using System.Collections.Generic;
using System.Threading.Tasks;

using SabreTools.Library.Data;
using SabreTools.Library.DatFiles;
using SabreTools.Library.DatItems;
using SabreTools.Library.Help;
using SabreTools.Library.IO;
using SabreTools.Library.Tools;

namespace SabreTools.Features
{
    internal class Split : BaseFeature
    {
        public const string Value = "Split";

        public Split()
        {
            Name = Value;
            Flags = new List<string>() { "-sp", "--split" };
            Description = "Split input DATs by a given criteria";
            _featureType = FeatureType.Flag;
            LongDescription = "This feature allows the user to split input DATs by a number of different possible criteria. See the individual input information for details. More than one split type is allowed at a time.";
            Features = new Dictionary<string, Feature>();

            AddFeature(OutputTypeListInput);
            this[OutputTypeListInput].AddFeature(DeprecatedFlag);
            AddFeature(OutputDirStringInput);
            AddFeature(InplaceFlag);
            AddFeature(ExtensionFlag);
            this[ExtensionFlag].AddFeature(ExtaListInput);
            this[ExtensionFlag].AddFeature(ExtbListInput);
            AddFeature(HashFlag);
            AddFeature(LevelFlag);
            this[LevelFlag].AddFeature(ShortFlag);
            this[LevelFlag].AddFeature(BaseFlag);
            AddFeature(SizeFlag);
            this[SizeFlag].AddFeature(RadixInt64Input);
            AddFeature(TypeFlag);
        }

        public override void ProcessFeatures(Dictionary<string, Feature> features)
        {
            base.ProcessFeatures(features);
            SplittingMode splittingMode = GetSplittingMode(features);

            // If we somehow have the "none" split type, return
            if (splittingMode == SplittingMode.None)
                return;

            // Get only files from the inputs
            List<ParentablePath> files = DirectoryExtensions.GetFilesOnly(Inputs, appendparent: true);

            // Loop over the input files
            foreach (ParentablePath file in files)
            {
                // Create and fill the new DAT
                DatFile internalDat = DatFile.Create(Header);
                internalDat.Parse(file);

                // Get the output directory
                OutputDir = file.GetOutputPath(OutputDir, GetBoolean(features, InplaceValue));

                // Extension splitting
                if (splittingMode.HasFlag(SplittingMode.Extension))
                {
                    (DatFile extADat, DatFile extBDat) = internalDat.SplitByExtension(GetList(features, ExtAListValue), GetList(features, ExtBListValue));

                    InternalStopwatch watch = new InternalStopwatch("Outputting extension-split DATs");

                    // Output both possible DatFiles
                    extADat.Write(OutputDir);
                    extBDat.Write(OutputDir);

                    watch.Stop();
                }

                // Hash splitting
                if (splittingMode.HasFlag(SplittingMode.Hash))
                    internalDat.SplitByHash(OutputDir);

                // Level splitting
                if (splittingMode.HasFlag(SplittingMode.Level))
                {
                    Globals.Logger.Warning("This feature is not implemented: level-split");
                    internalDat.SplitByLevel(
                        OutputDir,
                        GetBoolean(features, ShortValue),
                        GetBoolean(features, BaseValue));
                }

                // Size splitting
                if (splittingMode.HasFlag(SplittingMode.Size))
                {
                    (DatFile lessThan, DatFile greaterThan) = internalDat.SplitBySize(GetInt64(features, RadixInt64Value));

                    InternalStopwatch watch = new InternalStopwatch("Outputting size-split DATs");

                    // Output both possible DatFiles
                    lessThan.Write(OutputDir);
                    greaterThan.Write(OutputDir);

                    watch.Stop();
                }

                // Type splitting
                if (splittingMode.HasFlag(SplittingMode.Type))
                {
                    Dictionary<ItemType, DatFile> typeDats = internalDat.SplitByType();

                    InternalStopwatch watch = new InternalStopwatch("Outputting ItemType DATs");

                    // Loop through each type DatFile
                    Parallel.ForEach(typeDats.Keys, Globals.ParallelOptions, itemType =>
                    {
                        typeDats[itemType].Write(OutputDir);
                    });

                    watch.Stop();
                }
            }
        }
    }
}
