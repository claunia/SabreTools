using SabreTools.Skippers.Tests;

namespace SabreTools.Skippers.Detectors
{
    /// <summary>
    /// Detector for NEC PC-Engine / TurboGrafx 16 headers
    /// </summary>
    /// <remarks>Originally from pce.xml</remarks>
    internal class NECPCEngine : Detector
    {
        public NECPCEngine()
        {
            // Create tests
            var rule1Test1 = new DataTest
            {
                Offset = "0",
                Value = "4000000000000000AABB02",
                Result = true,
            };

            // Create rules
            var rule1 = new Rule
            {
                StartOffset = "200",
                Operation = HeaderSkipOperation.None,
                Tests =
                [
                    rule1Test1,
                ]
            };

            // Create file
            Name = "NEC TurboGrafx-16/PC-Engine";
            Author = "Matt Nadareski (darksabre76)";
            Version = "1.0";
            SourceFile = "pce";
            Rules =
            [
                rule1,
            ];
        }
    }
}
