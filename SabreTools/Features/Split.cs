using System.Collections.Generic;
using System.Threading.Tasks;

using SabreTools.Core;
using SabreTools.DatFiles;
using SabreTools.Help;
using SabreTools.IO;
using SabreTools.Logging;

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
            _featureType = ParameterType.Flag;
            LongDescription = "This feature allows the user to split input DATs by a number of different possible criteria. See the individual input information for details. More than one split type is allowed at a time.";
            Features = new Dictionary<string, Help.Feature>();

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

        public override void ProcessFeatures(Dictionary<string, Help.Feature> features)
        {
            base.ProcessFeatures(features);
            SplittingMode splittingMode = GetSplittingMode(features);

            // If we somehow have the "none" split type, return
            if (splittingMode == SplittingMode.None)
                return;

            // Get only files from the inputs
            List<ParentablePath> files = PathTool.GetFilesOnly(Inputs, appendparent: true);

            // Loop over the input files
            foreach (ParentablePath file in files)
            {
                // Create and fill the new DAT
                DatFile internalDat = DatFile.Create(Header);
                Parser.ParseInto(internalDat, file);

                // Get the output directory
                OutputDir = file.GetOutputPath(OutputDir, GetBoolean(features, InplaceValue));

                // Extension splitting
                if (splittingMode.HasFlag(SplittingMode.Extension))
                {
                    (DatFile extADat, DatFile extBDat) = Splitter.SplitByExtension(internalDat, GetList(features, ExtAListValue), GetList(features, ExtBListValue));

                    InternalStopwatch watch = new InternalStopwatch("Outputting extension-split DATs");

                    // Output both possible DatFiles
                    Writer.Write(extADat, OutputDir);
                    Writer.Write(extBDat, OutputDir);

                    watch.Stop();
                }

                // Hash splitting
                if (splittingMode.HasFlag(SplittingMode.Hash))
                {
                    Dictionary<Field, DatFile> typeDats = Splitter.SplitByHash(internalDat);

                    InternalStopwatch watch = new InternalStopwatch("Outputting hash-split DATs");

                    // Loop through each type DatFile
                    Parallel.ForEach(typeDats.Keys, Globals.ParallelOptions, itemType =>
                    {
                        Writer.Write(typeDats[itemType], OutputDir);
                    });

                    watch.Stop();
                }

                // Level splitting
                if (splittingMode.HasFlag(SplittingMode.Level))
                {
                    logger.Warning("This feature is not implemented: level-split");
                    Splitter.SplitByLevel(
                        internalDat,
                        OutputDir,
                        GetBoolean(features, ShortValue),
                        GetBoolean(features, BaseValue));
                }

                // Size splitting
                if (splittingMode.HasFlag(SplittingMode.Size))
                {
                    (DatFile lessThan, DatFile greaterThan) = Splitter.SplitBySize(internalDat, GetInt64(features, RadixInt64Value));

                    InternalStopwatch watch = new InternalStopwatch("Outputting size-split DATs");

                    // Output both possible DatFiles
                    Writer.Write(lessThan, OutputDir);
                    Writer.Write(greaterThan, OutputDir);

                    watch.Stop();
                }

                // Type splitting
                if (splittingMode.HasFlag(SplittingMode.Type))
                {
                    Dictionary<ItemType, DatFile> typeDats = Splitter.SplitByType(internalDat);

                    InternalStopwatch watch = new InternalStopwatch("Outputting ItemType DATs");

                    // Loop through each type DatFile
                    Parallel.ForEach(typeDats.Keys, Globals.ParallelOptions, itemType =>
                    {
                        Writer.Write(typeDats[itemType], OutputDir);
                    });

                    watch.Stop();
                }
            }
        }
    }
}
