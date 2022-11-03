using System.Collections.Generic;

namespace SabreTools.Skippers.SkipperFiles
{
    /// <summary>
    /// SkipperFile for Atari Lynx headers
    /// </summary>
    /// <remarks>Originally from lynx.xml</remarks>
    internal class AtariLynx : SkipperFile
    {
        public AtariLynx()
        {
            // Create tests
            var rule1Test1 = new SkipperTest
            {
                Type = HeaderSkipTest.Data,
                Offset = 0x00,
                Value = new byte[] { 0x4C, 0x59, 0x4E, 0x58 },
                Result = true,
            };

            var rule2Test1 = new SkipperTest
            {
                Type = HeaderSkipTest.Data,
                Offset = 0x06,
                Value = new byte[] { 0x42, 0x53, 0x39 },
                Result = true,
            };

            // Create rules
            var rule1 = new SkipperRule
            {
                StartOffset = 0x40,
                EndOffset = null,
                Operation = HeaderSkipOperation.None,
                Tests = new List<SkipperTest>
                {
                    rule1Test1,
                }
            };

            var rule2 = new SkipperRule
            {
                StartOffset = 0x40,
                EndOffset = null,
                Operation = HeaderSkipOperation.None,
                Tests = new List<SkipperTest>
                {
                    rule2Test1,
                }
            };

            // Create file
            Name = "Atari Lynx";
            Author = "Roman Scherzer";
            Version = "1.0";
            SourceFile = "lynx";
            Rules = new List<SkipperRule>
            {
                rule1,
                rule2,
            };
        }
    }
}
