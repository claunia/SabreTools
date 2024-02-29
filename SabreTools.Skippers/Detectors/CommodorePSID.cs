using SabreTools.Skippers.Tests;

namespace SabreTools.Skippers.Detectors
{
    /// <summary>
    /// Detector for Commodore PSID headers
    /// </summary>
    /// <remarks>Originally from psid.xml</remarks>
    internal class CommodorePSID : Detector
    {
        public CommodorePSID()
        {
            // Create tests
            var rule1Test1 = new DataTest
            {
                Offset = "0",
                Value = "5053494400010076",
                Result = true,
            };

            var rule2Test1 = new DataTest
            {
                Offset = "0",
                Value = "505349440003007c",
                Result = true,
            };

            var rule3Test1 = new DataTest
            {
                Offset = "0",
                Value = "505349440002007c",
                Result = true,
            };

            var rule4Test1 = new DataTest
            {
                Offset = "0",
                Value = "505349440001007c",
                Result = true,
            };

            var rule5Test1 = new DataTest
            {
                Offset = "0",
                Value = "525349440002007c",
                Result = true,
            };

            // Create rules
            var rule1 = new Rule
            {
                StartOffset = "76",
                EndOffset = "EOF",
                Operation = HeaderSkipOperation.None,
                Tests =
                [
                    rule1Test1,
                ]
            };

            var rule2 = new Rule
            {
                StartOffset = "76",
                EndOffset = "EOF",
                Operation = HeaderSkipOperation.None,
                Tests =
                [
                    rule2Test1,
                ]
            };

            var rule3 = new Rule
            {
                StartOffset = "7c",
                EndOffset = "EOF",
                Operation = HeaderSkipOperation.None,
                Tests =
                [
                    rule3Test1,
                ]
            };

            var rule4 = new Rule
            {
                StartOffset = "7c",
                EndOffset = "EOF",
                Operation = HeaderSkipOperation.None,
                Tests =
                [
                    rule4Test1,
                ]
            };

            var rule5 = new Rule
            {
                StartOffset = "7c",
                EndOffset = "EOF",
                Operation = HeaderSkipOperation.None,
                Tests =
                [
                    rule5Test1,
                ]
            };

            // Create file
            Name = "psid";
            Author = "Yori Yoshizuki";
            Version = "1.2";
            SourceFile = "psid";
            Rules =
            [
                rule1,
                rule2,
                rule3,
                rule4,
                rule5,
            ];
        }
    }
}
