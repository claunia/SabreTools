using System.Collections.Generic;

using SabreTools.Library.DatFiles;
using SabreTools.Library.Help;
using SabreTools.Library.IO;

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
                    internalDat.SplitByExtension(
                        OutputDir,
                        GetList(features, ExtAListValue),
                        GetList(features, ExtBListValue));
                }

                // Hash splitting
                if (splittingMode.HasFlag(SplittingMode.Hash))
                    internalDat.SplitByHash(OutputDir);

                // Level splitting
                if (splittingMode.HasFlag(SplittingMode.Level))
                {
                    internalDat.SplitByLevel(
                        OutputDir,
                        GetBoolean(features, ShortValue),
                        GetBoolean(features, BaseValue));
                }

                // Size splitting
                if (splittingMode.HasFlag(SplittingMode.Size))
                    internalDat.SplitBySize(OutputDir, GetInt64(features, RadixInt64Value));

                // Type splitting
                if (splittingMode.HasFlag(SplittingMode.Type))
                    internalDat.SplitByType(OutputDir);
            }
        }
    }
}
