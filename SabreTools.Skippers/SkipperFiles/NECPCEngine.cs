using System.Collections.Generic;

namespace SabreTools.Skippers.SkipperFiles
{
    /// <summary>
    /// SkipperFile for NEC PC-Engine / TurboGrafx 16 headers
    /// </summary>
    /// <remarks>Originally from pce.xml</remarks>
    internal class NECPCEngine : SkipperFile
    {
        public NECPCEngine()
        {
            // Create tests
            var rule1Test1 = new SkipperTest
            {
                Type = HeaderSkipTest.Data,
                Offset = 0x00,
                Value = new byte[] { 0x40, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xAA, 0xBB, 0x02 },
                Result = true,
            };

            // Create rules
            var rule1 = new SkipperRule
            {
                StartOffset = 0x200,
                EndOffset = null,
                Operation = HeaderSkipOperation.None,
                Tests = new List<SkipperTest>
                {
                    rule1Test1,
                }
            };

            // Create file
            Name = "NEC TurboGrafx-16/PC-Engine";
            Author = "Matt Nadareski (darksabre76)";
            Version = "1.0";
            SourceFile = "pce";
            Rules = new List<SkipperRule>
            {
                rule1,
            };
        }
    }
}
