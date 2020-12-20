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
        [InlineData(null, (DatFormat)0x00)]
        //[InlineData(null, DatFormat.Logiqx)]
        //[InlineData(null, DatFormat.LogiqxDeprecated)] // Not parsed separately
        //[InlineData(null, DatFormat.SoftwareList)]
        //[InlineData(null, DatFormat.Listxml)]
        //[InlineData(null, DatFormat.OfflineList)]
        //[InlineData(null, DatFormat.SabreXML)]
        //[InlineData(null, DatFormat.OpenMSX)]
        //[InlineData(null, DatFormat.ClrMamePro)]
        //[InlineData(null, DatFormat.RomCenter)]
        //[InlineData(null, DatFormat.DOSCenter)]
        //[InlineData(null, DatFormat.AttractMode)]
        //[InlineData(null, DatFormat.MissFile)] // Parsing is not supported
        //[InlineData(null, DatFormat.CSV)]
        //[InlineData(null, DatFormat.SSV)]
        //[InlineData(null, DatFormat.TSV)]
        //[InlineData(null, DatFormat.Listrom)]
        [InlineData("test-smdb.txt", DatFormat.EverdriveSMDB)]
        //[InlineData(null, DatFormat.SabreJSON)]
        [InlineData("test-sfv.sfv", DatFormat.RedumpSFV)]
        [InlineData("test-md5.md5", DatFormat.RedumpMD5)]
        [InlineData("test-sha1.sha1", DatFormat.RedumpSHA1)]
        [InlineData("test-sha256.sha256", DatFormat.RedumpSHA256)]
        [InlineData("test-sha384.sha384", DatFormat.RedumpSHA384)]
        [InlineData("test-sha512.sha512", DatFormat.RedumpSHA512)]
        [InlineData("test-spamsum.spamsum", DatFormat.RedumpSpamSum)]
        public void CreateAndParseTest(string filename, DatFormat datFormat)
        {
            // For all filenames, add the local path for test data
            if (filename != null)
                filename = Path.Combine(Environment.CurrentDirectory, "TestData", filename);
        
            var datFile = Parser.CreateAndParse(filename);
            Assert.Equal(datFormat, datFile.Header.DatFormat);
        }
    }
}