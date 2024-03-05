using System;
using System.IO;
using SabreTools.DatFiles;
using Xunit;

namespace SabreTools.Test.DatTools
{
    public class ParserTests
    {
        [Theory]
        [InlineData(null, (DatFormat)0x00, 0)]
        [InlineData("test-logiqx.xml", DatFormat.Logiqx, 6)]
        //[InlineData(null, DatFormat.LogiqxDeprecated, 0)] // Not parsed separately
        [InlineData("test-softwarelist.xml", DatFormat.SoftwareList, 5)]
        [InlineData("test-listxml.xml", DatFormat.Listxml, 19)]
        [InlineData("test-offlinelist.xml", DatFormat.OfflineList, 1)]
        //[InlineData(null, DatFormat.SabreXML, 0)] // TODO: Create good-enough test file for this
        [InlineData("test-openmsx.xml", DatFormat.OpenMSX, 3)]
        [InlineData("test-archivedotorg.xml", DatFormat.ArchiveDotOrg, 1)]
        [InlineData("test-cmp.dat", DatFormat.ClrMamePro, 6)]
        [InlineData("test-romcenter.dat", DatFormat.RomCenter, 1)]
        [InlineData("test-doscenter.dat", DatFormat.DOSCenter, 1)]
        [InlineData("test-attractmode.txt", DatFormat.AttractMode, 1)]
        //[InlineData(null, DatFormat.MissFile, 0)] // Parsing is not supported
        //[InlineData(null, DatFormat.CSV, 0)] // TODO: Create good-enough test file for this
        //[InlineData(null, DatFormat.SSV, 0)] // TODO: Create good-enough test file for this
        //[InlineData(null, DatFormat.TSV, 0)] // TODO: Create good-enough test file for this
        [InlineData("test-listrom.txt", DatFormat.Listrom, 6)]
        [InlineData("test-smdb.txt", DatFormat.EverdriveSMDB, 1)]
        //[InlineData(null, DatFormat.SabreJSON, 0)] // TODO: Create good-enough test file for this
        [InlineData("test-sfv.sfv", DatFormat.RedumpSFV, 1)]
        [InlineData("test-md5.md5", DatFormat.RedumpMD5, 1)]
        [InlineData("test-sha1.sha1", DatFormat.RedumpSHA1, 1)]
        [InlineData("test-sha256.sha256", DatFormat.RedumpSHA256, 1)]
        [InlineData("test-sha384.sha384", DatFormat.RedumpSHA384, 1)]
        [InlineData("test-sha512.sha512", DatFormat.RedumpSHA512, 1)]
        [InlineData("test-spamsum.spamsum", DatFormat.RedumpSpamSum, 1)]
        public void CreateAndParseTest(string? filename, DatFormat datFormat, int totalCount)
        {
            // For all filenames, add the local path for test data
            if (filename != null)
                filename = Path.Combine(Environment.CurrentDirectory, "TestData", filename);
        
            var datFile = SabreTools.DatTools.Parser.CreateAndParse(filename, throwOnError: true);
            Assert.Equal(datFormat, datFile.Header.DatFormat);
            Assert.Equal(totalCount, datFile.Items.TotalCount);
        }
    }
}