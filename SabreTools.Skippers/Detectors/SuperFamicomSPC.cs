using SabreTools.Skippers.Tests;

namespace SabreTools.Skippers.Detectors
{
    /// <summary>
    /// Detector for Super Famicom SPC headers
    /// </summary>
    /// <remarks>Originally from spc.xml</remarks>
    internal class SuperFamicomSPC : Detector
    {
        public SuperFamicomSPC()
        {
            // Create tests
            var rule1Test1 = new DataTest
            {
                Offset = "0",
                Value = "534E45532D535043",
                Result = true,
            };

            // Create rules
            var rule1 = new Rule
            {
                StartOffset = "00100",
                EndOffset = "EOF",
                Operation = HeaderSkipOperation.None,
                Tests = new Test[]
                {
                    rule1Test1,
                }
            };

            // Create file
            Name = "Nintendo Super Famicon SPC";
            Author = "Yori Yoshizuki";
            Version = "1.0";
            SourceFile = "spc";
            Rules = new Rule[]
            {
                rule1,
            };
        }
    }
}
