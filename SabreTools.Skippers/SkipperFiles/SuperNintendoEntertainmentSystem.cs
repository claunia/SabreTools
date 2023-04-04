using System.Collections.Generic;

namespace SabreTools.Skippers.SkipperFiles
{
    /// <summary>
    /// SkipperFile for Super Nintendo Entertainment System headers
    /// </summary>
    /// <remarks>Originally from snes.xml</remarks>
    internal class SuperNintendoEntertainmentSystem : SkipperFile
    {
        public SuperNintendoEntertainmentSystem()
        {
            // Create tests
            var rule1Test1 = new DataSkipperTest
            {
                Offset = 0x16,
                Value = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 },
                Result = true,
            };

            var rule2Test1 = new DataSkipperTest
            {
                Offset = 0x16,
                Value = new byte[] { 0xAA, 0xBB, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00 },
                Result = true,
            };

            var rule3Test1 = new DataSkipperTest
            {
                Offset = 0x16,
                Value = new byte[] { 0x53, 0x55, 0x50, 0x45, 0x52, 0x55, 0x46, 0x4F },
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

            var rule2 = new SkipperRule
            {
                StartOffset = 0x200,
                EndOffset = null,
                Operation = HeaderSkipOperation.None,
                Tests = new List<SkipperTest>
                {
                    rule2Test1,
                }
            };

            var rule3 = new SkipperRule
            {
                StartOffset = 0x200,
                EndOffset = null,
                Operation = HeaderSkipOperation.None,
                Tests = new List<SkipperTest>
                {
                    rule3Test1,
                }
            };

            // Create file
            Name = "Nintendo Super Famicom/SNES";
            Author = "Matt Nadareski (darksabre76)";
            Version = "1.0";
            SourceFile = "snes";
            Rules = new List<SkipperRule>
            {
                rule1, // FIG
                rule2, // SMC
                rule3, // UFO
            };
        }
    }
}
