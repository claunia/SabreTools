using System.Collections.Generic;

namespace SabreTools.Skippers.SkipperFiles
{
    /// <summary>
    /// SkipperFile for Nintendo 64 headers
    /// </summary>
    /// <remarks>Originally from n64.xml</remarks>
    internal class Nintendo64 : SkipperFile
    {
        public Nintendo64()
        {
            // Create tests
            var rule1Test1 = new DataSkipperTest
            {
                Offset = 0x00,
                Value = new byte[] { 0x80, 0x37, 0x12, 0x40 },
                Result = true,
            };

            var rule2Test1 = new DataSkipperTest
            {
                Offset = 0x00,
                Value = new byte[] { 0x37, 0x80, 0x40, 0x12 },
                Result = true,
            };

            var rule3Test1 = new DataSkipperTest
            {
                Offset = 0x00,
                Value = new byte[] { 0x40, 0x12, 0x37, 0x80 },
                Result = true,
            };

            // Create rules
            var rule1 = new SkipperRule
            {
                StartOffset = 0x00,
                EndOffset = null,
                Operation = HeaderSkipOperation.None,
                Tests = new List<SkipperTest>
                {
                    rule1Test1,
                }
            };

            var rule2 = new SkipperRule
            {
                StartOffset = 0x00,
                EndOffset = null,
                Operation = HeaderSkipOperation.Byteswap,
                Tests = new List<SkipperTest>
                {
                    rule2Test1,
                }
            };

            var rule3 = new SkipperRule
            {
                StartOffset = 0x00,
                EndOffset = null,
                Operation = HeaderSkipOperation.Wordswap,
                Tests = new List<SkipperTest>
                {
                    rule3Test1,
                }
            };

            // Create file
            Name = "Nintendo 64 - ABCD";
            Author = "CUE";
            Version = "1.1";
            SourceFile = "n64";
            Rules = new List<SkipperRule>
            {
                rule1, // V64
                rule2, // Z64
                rule3, // N64
            };
        }
    }
}
