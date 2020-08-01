using System.Collections.Generic;

using SabreTools.Library.Data;
using SabreTools.Library.DatFiles;
using SabreTools.Library.Help;

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

            DatFile datfile = DatFile.Create(Header.DatFormat);
            datfile.DetermineSplitType(
                Inputs,
                OutputDir,
                GetBoolean(features, InplaceValue),
                GetSplittingMode(features),
                GetList(features, ExtAListValue),
                GetList(features, ExtBListValue),
                GetBoolean(features, ShortValue),
                GetBoolean(features, BaseValue),
                GetInt64(features, RadixInt64Value));
        }
    }
}
