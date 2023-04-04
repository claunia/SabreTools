using System.Collections.Generic;

namespace SabreTools.Skippers.SkipperFiles
{
    /// <summary>
    /// SkipperFile for Commodore PSID headers
    /// </summary>
    /// <remarks>Originally from psid.xml</remarks>
    internal class CommodorePSID : SkipperFile
    {
        public CommodorePSID()
        {
            // Create tests
            var rule1Test1 = new DataSkipperTest
            {
                Offset = 0x00,
                Value = new byte[] { 0x50, 0x53, 0x49, 0x44, 0x00, 0x01, 0x00, 0x76 },
                Result = true,
            };

            var rule2Test1 = new DataSkipperTest
            {
                Offset = 0x00,
                Value = new byte[] { 0x50, 0x53, 0x49, 0x44, 0x00, 0x03, 0x00, 0x7c },
                Result = true,
            };

            var rule3Test1 = new DataSkipperTest
            {
                Offset = 0x00,
                Value = new byte[] { 0x50, 0x53, 0x49, 0x44, 0x00, 0x02, 0x00, 0x7c },
                Result = true,
            };

            var rule4Test1 = new DataSkipperTest
            {
                Offset = 0x00,
                Value = new byte[] { 0x50, 0x53, 0x49, 0x44, 0x00, 0x01, 0x00, 0x7c },
                Result = true,
            };

            var rule5Test1 = new DataSkipperTest
            {
                Offset = 0x00,
                Value = new byte[] { 0x52, 0x53, 0x49, 0x44, 0x00, 0x02, 0x00, 0x7c },
                Result = true,
            };

            // Create rules
            var rule1 = new SkipperRule
            {
                StartOffset = 0x76,
                EndOffset = null,
                Operation = HeaderSkipOperation.None,
                Tests = new List<SkipperTest>
                {
                    rule1Test1,
                }
            };

            var rule2 = new SkipperRule
            {
                StartOffset = 0x76,
                EndOffset = null,
                Operation = HeaderSkipOperation.None,
                Tests = new List<SkipperTest>
                {
                    rule2Test1,
                }
            };

            var rule3 = new SkipperRule
            {
                StartOffset = 0x7c,
                EndOffset = null,
                Operation = HeaderSkipOperation.None,
                Tests = new List<SkipperTest>
                {
                    rule3Test1,
                }
            };

            var rule4 = new SkipperRule
            {
                StartOffset = 0x7c,
                EndOffset = null,
                Operation = HeaderSkipOperation.None,
                Tests = new List<SkipperTest>
                {
                    rule4Test1,
                }
            };

            var rule5 = new SkipperRule
            {
                StartOffset = 0x7c,
                EndOffset = null,
                Operation = HeaderSkipOperation.None,
                Tests = new List<SkipperTest>
                {
                    rule5Test1,
                }
            };

            // Create file
            Name = "psid";
            Author = "Yori Yoshizuki";
            Version = "1.2";
            SourceFile = "psid";
            Rules = new List<SkipperRule>
            {
                rule1,
                rule2,
                rule3,
                rule4,
                rule5,
            };
        }
    }
}
