using SabreTools.Skippers.Tests;

namespace SabreTools.Skippers.Detectors
{
    /// <summary>
    /// Detector for Nintendo Entertainment System headers
    /// </summary>
    /// <remarks>Originally from nes.xml</remarks>
    internal class NintendoEntertainmentSystem : Detector
    {
        public NintendoEntertainmentSystem()
        {
            // Create tests
            var rule1Test1 = new DataTest
            {
                Offset = "0",
                Value = "4E45531A",
                Result = true,
            };

            // Create rules
            var rule1 = new Rule
            {
                StartOffset = "10",
                EndOffset = "EOF",
                Operation = HeaderSkipOperation.None,
                Tests =
                [
                    rule1Test1,
                ]
            };

            // Create file
            Name = "Nintendo Famicon/NES";
            Author = "Roman Scherzer";
            Version = "1.1";
            SourceFile = "nes";
            Rules =
            [
                rule1,
            ];
        }
    }
}
