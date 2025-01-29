using SabreTools.DatFiles;
using SabreTools.DatTools;
using Xunit;

namespace SabreTools.Test.DatFiles
{
    // TODO: Migrate tests to WriterTests when available
    public class DatHeaderTests
    {
        [Theory]
        [InlineData(DatFormat.CSV, "csv")]
        [InlineData(DatFormat.ClrMamePro, "dat")]
        [InlineData(DatFormat.SabreJSON, "json")]
        [InlineData(DatFormat.AttractMode, "txt")]
        [InlineData(DatFormat.Logiqx, "xml")]
        public void CreateOutFileNamesTest(DatFormat datFormat, string extension)
        {
            // Create the empty DatHeader
            var datHeader = new DatHeader();
            datHeader.SetFieldValue<string?>(DatHeader.FileNameKey, "test.dat");
            datHeader.SetFieldValue<DatFormat>(DatHeader.DatFormatKey, datFormat);

            // Invoke the method
            string outDir = "C:\\Test";
            var actual = Writer.CreateOutFileNames(datHeader, outDir, overwrite: true);

            // Check the result
            string expected = $"{outDir}{System.IO.Path.DirectorySeparatorChar}test.{extension}";
            Assert.Single(actual);
            Assert.Equal(expected, actual[datFormat]);
        }

        [Fact]
        public void CreateOutFileNamesAllOutputsTest()
        {
            // Create the empty DatHeader
            var datHeader = new DatHeader();
            datHeader.SetFieldValue<string?>(DatHeader.FileNameKey, "test.dat");
            datHeader.SetFieldValue<DatFormat>(DatHeader.DatFormatKey, DatFormat.ALL);

            // Invoke the method
            string outDir = "C:\\Test";
            var actual = Writer.CreateOutFileNames(datHeader, outDir, overwrite: true);

            // Check the result
            Assert.Equal(28, actual.Count);
        }
    }
}