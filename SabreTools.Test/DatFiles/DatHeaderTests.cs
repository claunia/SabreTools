using SabreTools.DatFiles;
using Xunit;

namespace SabreTools.Test.DatFiles
{
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
            var datHeader = new DatHeader
            {
                DatFormat = datFormat,
                FileName = "test.dat",
            };

            // Invoke the method
            string outDir = "C:\\Test";
            var actual = datHeader.CreateOutFileNames(outDir, overwrite: true);

            // Check the result
            string expected = $"{outDir}\\test.{extension}";
            Assert.Single(actual);
            Assert.Equal(expected, actual[datFormat]);
        }

        [Fact]
        public void CreateOutFileNamesAllOutputsTest()
        {
            // Create the empty DatHeader
            var datHeader = new DatHeader
            {
                DatFormat = DatFormat.ALL,
                FileName = "test.dat",
            };

            // Invoke the method
            string outDir = "C:\\Test";
            var actual = datHeader.CreateOutFileNames(outDir, overwrite: true);

            // Check the result
            Assert.Equal(26, actual.Count);
        }
    }
}