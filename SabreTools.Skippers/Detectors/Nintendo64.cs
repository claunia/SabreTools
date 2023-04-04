using SabreTools.Skippers.Tests;

namespace SabreTools.Skippers.Detectors
{
    /// <summary>
    /// Detector for Nintendo 64 headers
    /// </summary>
    /// <remarks>Originally from n64.xml</remarks>
    internal class Nintendo64 : Detector
    {
        public Nintendo64()
        {
            // Create tests
            var rule1Test1 = new DataTest
            {
                Offset = "0",
                Value = "80371240",
                Result = true,
            };

            var rule2Test1 = new DataTest
            {
                Offset = "0",
                Value = "37804012",
                Result = true,
            };

            var rule3Test1 = new DataTest
            {
                Offset = "0",
                Value = "40123780",
                Result = true,
            };

            // Create rules
            var rule1 = new Rule
            {
                StartOffset = "0",
                EndOffset = "EOF",
                Operation = HeaderSkipOperation.None,
                Tests = new Test[]
                {
                    rule1Test1,
                }
            };

            var rule2 = new Rule
            {
                StartOffset = "0",
                EndOffset = "EOF",
                Operation = HeaderSkipOperation.Byteswap,
                Tests = new Test[]
                {
                    rule2Test1,
                }
            };

            var rule3 = new Rule
            {
                StartOffset = "0",
                EndOffset = "EOF",
                Operation = HeaderSkipOperation.Wordswap,
                Tests = new Test[]
                {
                    rule3Test1,
                }
            };

            // Create file
            Name = "Nintendo 64 - ABCD";
            Author = "CUE";
            Version = "1.1";
            SourceFile = "n64";
            Rules = new Rule[]
            {
                rule1, // V64
                rule2, // Z64
                rule3, // N64
            };
        }
    }
}
