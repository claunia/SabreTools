using System;
using System.IO;

using SabreTools.DatFiles;
using SabreTools.DatTools;
using Xunit;

namespace SabreTools.Test.DatTools
{
    public class ParserTests
    {
        // TODO: Create files for each of these
        // TODO: Ensure that all stress all bits of reading
        // TODO: Add total count? Might be a good metric if everything read
        [Theory]
        [InlineData(null, (DatFormat)0x00, 0)]
        //[InlineData(null, DatFormat.Logiqx, 0)]
        //[InlineData(null, DatFormat.LogiqxDeprecated, 0)] // Not parsed separately
        //[InlineData(null, DatFormat.SoftwareList, 0)]
        //[InlineData(null, DatFormat.Listxml, 0)]
        //[InlineData(null, DatFormat.OfflineList, 0)]
        //[InlineData(null, DatFormat.SabreXML, 0)]
        [InlineData("test-openmsx.xml", DatFormat.OpenMSX, 3)]
        [InlineData("test-cmp.dat", DatFormat.ClrMamePro, 6)]
        //[InlineData(null, DatFormat.RomCenter, 0)]
        //[InlineData(null, DatFormat.DOSCenter, 0)]
        [InlineData("test-attractmode.txt", DatFormat.AttractMode, 1)]
        //[InlineData(null, DatFormat.MissFile, 0)] // Parsing is not supported
        //[InlineData(null, DatFormat.CSV, 0)]
        //[InlineData(null, DatFormat.SSV, 0)]
        //[InlineData(null, DatFormat.TSV, 0)]
        [InlineData("test-listrom.txt", DatFormat.Listrom, 6)]
        [InlineData("test-smdb.txt", DatFormat.EverdriveSMDB, 1)]
        //[InlineData(null, DatFormat.SabreJSON, 0)]
        [InlineData("test-sfv.sfv", DatFormat.RedumpSFV, 1)]
        [InlineData("test-md5.md5", DatFormat.RedumpMD5, 1)]
        [InlineData("test-sha1.sha1", DatFormat.RedumpSHA1, 1)]
        [InlineData("test-sha256.sha256", DatFormat.RedumpSHA256, 1)]
        [InlineData("test-sha384.sha384", DatFormat.RedumpSHA384, 1)]
        [InlineData("test-sha512.sha512", DatFormat.RedumpSHA512, 1)]
        [InlineData("test-spamsum.spamsum", DatFormat.RedumpSpamSum, 1)]
        public void CreateAndParseTest(string filename, DatFormat datFormat, int totalCount)
        {
            // For all filenames, add the local path for test data
            if (filename != null)
                filename = Path.Combine(Environment.CurrentDirectory, "TestData", filename);
        
            var datFile = Parser.CreateAndParse(filename);
            Assert.Equal(datFormat, datFile.Header.DatFormat);
            Assert.Equal(totalCount, datFile.Items.TotalCount);
        }
    }
}