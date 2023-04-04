using SabreTools.Skippers.Tests;

namespace SabreTools.Skippers.Detectors
{
    /// <summary>
    /// Detector for Super Nintendo Entertainment System headers
    /// </summary>
    /// <remarks>Originally from snes.xml</remarks>
    internal class SuperNintendoEntertainmentSystem : Detector
    {
        public SuperNintendoEntertainmentSystem()
        {
            // Create tests
            var rule1Test1 = new DataTest
            {
                Offset = "16",
                Value = "0000000000000000",
            };

            var rule2Test1 = new DataTest
            {
                Offset = "16",
                Value = "AABB040000000000",
            };

            var rule3Test1 = new DataTest
            {
                Offset = "16",
                Value = "535550455255464F",
            };

            // Create rules
            var rule1 = new Rule
            {
                StartOffset = "200",
                Operation = HeaderSkipOperation.None,
                Tests = new Test[]
                {
                    rule1Test1,
                }
            };

            var rule2 = new Rule
            {
                StartOffset = "200",
                Operation = HeaderSkipOperation.None,
                Tests = new Test[]
                {
                    rule2Test1,
                }
            };

            var rule3 = new Rule
            {
                StartOffset = "200",
                Operation = HeaderSkipOperation.None,
                Tests = new Test[]
                {
                    rule3Test1,
                }
            };

            // Create file
            Name = "Nintendo Super Famicom/SNES";
            Author = "Matt Nadareski (darksabre76)";
            Version = "1.0";
            SourceFile = "snes";
            Rules = new Rule[]
            {
                rule1, // FIG
                rule2, // SMC
                rule3, // UFO
            };
        }
    }
}
