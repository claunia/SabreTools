using System.Collections.Generic;

namespace SabreTools.Skippers.SkipperFiles
{
    /// <summary>
    /// SkipperFile for Super Famicom SPC headers
    /// </summary>
    /// <remarks>Originally from spc.xml</remarks>
    internal class SuperFamicomSPC : SkipperFile
    {
        public SuperFamicomSPC()
        {
            // Create tests
            var rule1Test1 = new SkipperTest
            {
                Type = HeaderSkipTest.Data,
                Offset = 0x00,
                Value = new byte[] { 0x53, 0x4E, 0x45, 0x53, 0x2D, 0x53, 0x50, 0x43 },
                Result = true,
            };

            // Create rules
            var rule1 = new SkipperRule
            {
                StartOffset = 0x100,
                EndOffset = null,
                Operation = HeaderSkipOperation.None,
                Tests = new List<SkipperTest>
                {
                    rule1Test1,
                }
            };

            // Create file
            Name = "Nintendo Super Famicon SPC";
            Author = "Yori Yoshizuki";
            Version = "1.0";
            SourceFile = "spc";
            Rules = new List<SkipperRule>
            {
                rule1,
            };
        }
    }
}
