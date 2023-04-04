using SabreTools.Skippers.Tests;

namespace SabreTools.Skippers.Detectors
{
    /// <summary>
    /// Detector for Atari 7800 headers
    /// </summary>
    /// <remarks>Originally from a7800.xml</remarks>
    internal class Atari7800 : Detector
    {
        public Atari7800()
        {
            // Create tests
            var rule1Test1 = new DataTest
            {
                Offset = "1",
                Value = "415441524937383030",
                Result = true,
            };

            var rule2Test1 = new DataTest
            {
                Offset = "64",
                Value = "41435455414C20434152542044415441205354415254532048455245",
                Result = true,
            };

            // Create rules
            var rule1 = new Rule
            {
                StartOffset = "80",
                EndOffset = "EOF",
                Operation = HeaderSkipOperation.None,
                Tests = new Test[]
                {
                    rule1Test1,
                }
            };

            var rule2 = new Rule
            {
                StartOffset = "80",
                EndOffset = "EOF",
                Operation = HeaderSkipOperation.None,
                Tests = new Test[]
                {
                    rule2Test1,
                }
            };

            // Create file
            Name = "Atari 7800";
            Author = "Roman Scherzer";
            Version = "1.0";
            SourceFile = "a7800";
            Rules = new Rule[]
            {
                rule1,
                rule2,
            };
        }
    }
}
