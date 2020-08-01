using System.Collections.Generic;
using System.IO;

using SabreTools.Library.Data;
using SabreTools.Library.DatFiles;
using SabreTools.Library.Help;

namespace RombaSharp.Features
{
    internal class DatStats : BaseFeature
    {
        public const string Value = "DatStats";

        public DatStats()
        {
            Name = Value;
            Flags = new List<string>() { "datstats" };
            Description = "Prints dat stats.";
            _featureType = FeatureType.Flag;
            LongDescription = "Print dat stats.";
            Features = new Dictionary<string, Feature>();
        }

        public override void ProcessFeatures(Dictionary<string, Feature> features)
        {
            base.ProcessFeatures(features);

            // If we have no inputs listed, we want to use datroot
            if (Inputs == null || Inputs.Count == 0)
                Inputs = new List<string> { Path.GetFullPath(_dats) };

            // Now output the stats for all inputs
            ItemDictionary.OutputStats(Inputs, "rombasharp-datstats", null /* outDir */, true /* single */, true /* baddumpCol */, true /* nodumpCol */, StatReportFormat.Textfile);
        }
    }
}
