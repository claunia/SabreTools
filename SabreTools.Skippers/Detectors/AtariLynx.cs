using SabreTools.Skippers.Tests;

namespace SabreTools.Skippers.Detectors
{
    /// <summary>
    /// Detector for Atari Lynx headers
    /// </summary>
    /// <remarks>Originally from lynx.xml</remarks>
    internal class AtariLynx : Detector
    {
        public AtariLynx()
        {
            // Create tests
            var rule1Test1 = new DataTest
            {
                Offset = "0",
                Value = "4C594E58",
                Result = true,
            };

            var rule2Test1 = new DataTest
            {
                Offset = "6",
                Value = "425339",
                Result = true,
            };

            // Create rules
            var rule1 = new Rule
            {
                StartOffset = "40",
                EndOffset = "EOF",
                Operation = HeaderSkipOperation.None,
                Tests =
                [
                    rule1Test1,
                ]
            };

            var rule2 = new Rule
            {
                StartOffset = "40",
                EndOffset = "EOF",
                Operation = HeaderSkipOperation.None,
                Tests =
                [
                    rule2Test1,
                ]
            };

            // Create file
            Name = "Atari Lynx";
            Author = "Roman Scherzer";
            Version = "1.0";
            SourceFile = "lynx";
            Rules =
            [
                rule1,
                rule2,
            ];
        }
    }
}
