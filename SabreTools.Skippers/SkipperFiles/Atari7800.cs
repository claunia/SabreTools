using System.Collections.Generic;

namespace SabreTools.Skippers.SkipperFiles
{
    /// <summary>
    /// SkipperFile for Atari 7800 headers
    /// </summary>
    /// <remarks>Originally from a7800.xml</remarks>
    internal class Atari7800 : SkipperFile
    {
        public Atari7800()
        {
            // Create tests
            var rule1Test1 = new SkipperTest
            {
                Type = HeaderSkipTest.Data,
                Offset = 0x01,
                Value = new byte[] { 0x41, 0x54, 0x41, 0x52, 0x49, 0x37, 0x38, 0x30, 0x30 },
                Result = true,
            };

            var rule2Test1 = new SkipperTest
            {
                Type = HeaderSkipTest.Data,
                Offset = 0x64,
                Value = new byte[] { 0x41, 0x43, 0x54, 0x55, 0x41, 0x4C, 0x20, 0x43, 0x41, 0x52, 0x54, 0x20, 0x44, 0x41, 0x54, 0x41, 0x20, 0x53, 0x54, 0x41, 0x52, 0x54, 0x53, 0x20, 0x48, 0x45, 0x52, 0x45 },
                Result = true,
            };

            // Create rules
            var rule1 = new SkipperRule
            {
                StartOffset = 0x80,
                EndOffset = null,
                Operation = HeaderSkipOperation.None,
                Tests = new List<SkipperTest>
                {
                    rule1Test1,
                }
            };

            var rule2 = new SkipperRule
            {
                StartOffset = 0x80,
                EndOffset = null,
                Operation = HeaderSkipOperation.None,
                Tests = new List<SkipperTest>
                {
                    rule2Test1,
                }
            };

            // Create file
            Name = "Atari 7800";
            Author = "Roman Scherzer";
            Version = "1.0";
            SourceFile = "a7800";
            Rules = new List<SkipperRule>
            {
                rule1,
                rule2,
            };
        }
    }
}
