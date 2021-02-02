using System;
using System.IO;
using System.Text;

using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.DatItems.Formats;
using SabreTools.FileTypes;
using Xunit;

namespace SabreTools.Test.FileTypes
{
    public class BaseFileTests
    {
        [Fact]
        public void GetInfoNormalFileTest()
        {
            // Get the path to the test data
            string filename = Path.Combine(Environment.CurrentDirectory, "TestData", "file-to-hash.bin");

            // Set all of the expected hash values
            string expectedCrc = "ba02a660";
            string expectedMd5 = "b722871eaa950016296184d026c5dec9";
            string expectedSha1 = "eea1ee2d801d830c4bdad4df3c8da6f9f52d1a9f";
            string expectedSha256 = "fdb02dee8c319c52087382c45f099c90d0b6cc824850aff28c1bfb2884b7b855";
            string expectedSha384 = "e276c49618fff25bc1fe2e0659cd0ef0e7c1186563b063e07c52323b9899f3ce9b091be04d6208444b3ef1265e879074";
            string expectedSha512 = "15d69514eb628c2403e945a7cafd1d27e557f6e336c69b63ea17e7ed9d256cc374ee662f09305836d6de37fdae59d83883b982aa8446e4ff26346b6b6b50b240";
            string expectedSpamSum = "3:hMCPQCE6AFQxWyENFACBE+rW6Tj7SMQmKozr9MVERk:hZRdxZENFs+rPSromek";

            // Get the BaseFile generated from the path
            var baseFile = BaseFile.GetInfo(filename, hashes: Hash.All);

            // Extract all the hashes to string
            string actualCrc = Utilities.ByteArrayToString(baseFile.CRC);
            string actualMd5 = Utilities.ByteArrayToString(baseFile.MD5);
            string actualSha1 = Utilities.ByteArrayToString(baseFile.SHA1);
            string actualSha256 = Utilities.ByteArrayToString(baseFile.SHA256);
            string actualSha384 = Utilities.ByteArrayToString(baseFile.SHA384);
            string actualSha512 = Utilities.ByteArrayToString(baseFile.SHA512);
            string actualSpamSum = Encoding.UTF8.GetString(baseFile.SpamSum);

            // Verify all of the hashes match
            Assert.Equal(expectedCrc, actualCrc);
            Assert.Equal(expectedMd5, actualMd5);
            Assert.Equal(expectedSha1, actualSha1);
            Assert.Equal(expectedSha256, actualSha256);
            Assert.Equal(expectedSha384, actualSha384);
            Assert.Equal(expectedSha512, actualSha512);
            Assert.Equal(expectedSpamSum, actualSpamSum);

            // Convert to Rom for sanity check
            var rom = new Rom(baseFile);

            // Verify all of the hashes match
            Assert.Equal(expectedCrc, rom.CRC);
            Assert.Equal(expectedMd5, rom.MD5);
            Assert.Equal(expectedSha1, rom.SHA1);
            Assert.Equal(expectedSha256, rom.SHA256);
            Assert.Equal(expectedSha384, rom.SHA384);
            Assert.Equal(expectedSha512, rom.SHA512);
            Assert.Equal(expectedSpamSum, rom.SpamSum);
        }
    }
}