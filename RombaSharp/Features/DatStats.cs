using System.Collections.Generic;
using System.IO;
using SabreTools.DatTools;
using SabreTools.Help;
using SabreTools.Reports;

namespace RombaSharp.Features
{
    internal class DatStats : BaseFeature
    {
        public const string Value = "DatStats";

        public DatStats()
        {
            Name = Value;
            Flags.AddRange(["datstats"]);
            Description = "Prints dat stats.";
            _featureType = ParameterType.Flag;
            LongDescription = "Print dat stats.";

            // Common Features
            AddCommonFeatures();
        }

        public override bool ProcessFeatures(Dictionary<string, Feature?> features)
        {
            // If the base fails, just fail out
            if (!base.ProcessFeatures(features))
                return false;

            // If we have no inputs listed, we want to use datroot
            if (Inputs.Count == 0)
                Inputs.Add(Path.GetFullPath(_dats!));

            // Now output the stats for all inputs
            var statistics = Statistics.CalculateStatistics(Inputs, single: true);
            Statistics.Write(
                statistics,
                "rombasharp-datstats",
                outDir: null,
                baddumpCol: true,
                nodumpCol: true,
                StatReportFormat.Textfile);

            return true;
        }
    }
}
