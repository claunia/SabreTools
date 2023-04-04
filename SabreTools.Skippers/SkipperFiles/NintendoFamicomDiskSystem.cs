using System.Collections.Generic;

namespace SabreTools.Skippers.SkipperFiles
{
    /// <summary>
    /// SkipperFile for Nintendo Famicom Disk System headers
    /// </summary>
    /// <remarks>Originally from fds.xml</remarks>
    internal class NintendoFamicomDiskSystem : SkipperFile
    {
        public NintendoFamicomDiskSystem()
        {
            // Create tests
            var rule1Test1 = new DataSkipperTest
            {
                Offset = 0x00,
                Value = new byte[] { 0x46, 0x44, 0x53, 0x1A, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 },
                Result = true,
            };

            var rule2Test1 = new DataSkipperTest
            {
                Offset = 0x00,
                Value = new byte[] { 0x46, 0x44, 0x53, 0x1A, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 },
                Result = true,
            };

            var rule3Test1 = new DataSkipperTest
            {
                Offset = 0x00,
                Value = new byte[] { 0x46, 0x44, 0x53, 0x1A, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 },
                Result = true,
            };

            var rule4Test1 = new DataSkipperTest
            {
                Offset = 0x00,
                Value = new byte[] { 0x46, 0x44, 0x53, 0x1A, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 },
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

            var rule2 = new SkipperRule
            {
                StartOffset = 0x10,
                EndOffset = null,
                Operation = HeaderSkipOperation.None,
                Tests = new List<SkipperTest>
                {
                    rule2Test1,
                }
            };

            var rule3 = new SkipperRule
            {
                StartOffset = 0x10,
                EndOffset = null,
                Operation = HeaderSkipOperation.None,
                Tests = new List<SkipperTest>
                {
                    rule3Test1,
                }
            };

            var rule4 = new SkipperRule
            {
                StartOffset = 0x10,
                EndOffset = null,
                Operation = HeaderSkipOperation.None,
                Tests = new List<SkipperTest>
                {
                    rule4Test1,
                }
            };

            // Create file
            Name = "fds";
            Author = "Yori Yoshizuki";
            Version = "1.0";
            SourceFile = "fds";
            Rules = new List<SkipperRule>
            {
                rule1,
                rule2,
                rule3,
                rule4,
            };
        }
    }
}
