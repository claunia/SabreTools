using SabreTools.Skippers.Tests;

namespace SabreTools.Skippers.Detectors
{
    /// <summary>
    /// Detector for Nintendo Famicom Disk System headers
    /// </summary>
    /// <remarks>Originally from fds.xml</remarks>
    internal class NintendoFamicomDiskSystem : Detector
    {
        public NintendoFamicomDiskSystem()
        {
            // Create tests
            var rule1Test1 = new DataTest
            {
                Offset = "0",
                Value = "4644531A010000000000000000000000",
            };

            var rule2Test1 = new DataTest
            {
                Offset = "0",
                Value = "4644531A020000000000000000000000",
            };

            var rule3Test1 = new DataTest
            {
                Offset = "0",
                Value = "4644531A030000000000000000000000",
            };

            var rule4Test1 = new DataTest
            {
                Offset = "0",
                Value = "4644531A040000000000000000000000",
            };

            // Create rules
            var rule1 = new Rule
            {
                StartOffset = "10",
                Operation = HeaderSkipOperation.None,
                Tests = new Test[]
                {
                    rule1Test1,
                }
            };

            var rule2 = new Rule
            {
                StartOffset = "10",
                Operation = HeaderSkipOperation.None,
                Tests = new Test[]
                {
                    rule2Test1,
                }
            };

            var rule3 = new Rule
            {
                StartOffset = "10",
                Operation = HeaderSkipOperation.None,
                Tests = new Test[]
                {
                    rule3Test1,
                }
            };

            var rule4 = new Rule
            {
                StartOffset = "10",
                Operation = HeaderSkipOperation.None,
                Tests = new Test[]
                {
                    rule4Test1,
                }
            };

            // Create file
            Name = "fds";
            Author = "Yori Yoshizuki";
            Version = "1.0";
            SourceFile = "fds";
            Rules = new Rule[]
            {
                rule1,
                rule2,
                rule3,
                rule4,
            };
        }
    }
}
