using System.Collections.Generic;

namespace SabreTools.Skippers.SkipperFiles
{
    /// <summary>
    /// SkipperFile for Nintendo Entertainment System headers
    /// </summary>
    /// <remarks>Originally from nes.xml</remarks>
    internal class NintendoEntertainmentSystem : SkipperFile
    {
        public NintendoEntertainmentSystem()
        {
            // Create tests
            var rule1Test1 = new DataSkipperTest
            {
                Offset = 0x00,
                Value = new byte[] { 0x4E, 0x45, 0x53, 0x1A },
                Result = true,
            };

            // Create rules
            var rule1 = new SkipperRule
            {
                StartOffset = 0x10,
                EndOffset = null,
                Operation = HeaderSkipOperation.None,
                Tests = new List<SkipperTest>
                {
                    rule1Test1,
                }
            };

            // Create file
            Name = "Nintendo Famicon/NES";
            Author = "Roman Scherzer";
            Version = "1.1";
            SourceFile = "nes";
            Rules = new List<SkipperRule>
            {
                rule1,
            };
        }
    }
}
