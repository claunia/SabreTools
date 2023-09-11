using System;
using Xunit;

namespace SabreTools.Test.Parser
{
    public class SerializationTests
    {
        [Fact]
        public void OpenMSXSeserializeTest()
        {
            // Create the object for serialization
            var dat = GenerateOpenMSX();

            // Deserialize the file
            var stream = new Serialization.Streams.OpenMSX().Serialize(dat) as System.IO.MemoryStream;

            // Validate the values
            Assert.NotNull(stream);
            byte[] hash = System.Security.Cryptography.SHA1.Create().ComputeHash(stream.GetBuffer());
            string hashstr = BitConverter.ToString(hash).Replace("-", string.Empty);
            Assert.Equal("268940391C107ABE67E804BC5479E40B5FF68B34", hashstr);
        }

        #region Payload Generators

        /// <summary>
        /// Generate a consistent OpenMSX SoftwareDb for testing
        /// </summary>
        private static Models.OpenMSX.SoftwareDb GenerateOpenMSX()
        {
            var original = new Models.OpenMSX.Original
            {
                Value = "false",
                Content = "Original Name",
            };

            var rom = new Models.OpenMSX.Rom
            {
                Start = "0x0000",
                Type = "Game",
                Hash = "da39a3ee5e6b4b0d3255bfef95601890afd80709",
                Remark = "Comment",
            };

            var megaRom = new Models.OpenMSX.MegaRom
            {
                Start = "0x1000",
                Type = "Software",
                Hash = "da39a3ee5e6b4b0d3255bfef95601890afd80709",
                Remark = "Comment",
            };

            var sccPlusCart = new Models.OpenMSX.SCCPlusCart
            {
                Start = "0x2000",
                Type = "Utility",
                Hash = "da39a3ee5e6b4b0d3255bfef95601890afd80709",
                Remark = "Comment",
            };

            var dump = new Models.OpenMSX.Dump[]
            {
                new Models.OpenMSX.Dump { Original = original, Rom = rom },
                new Models.OpenMSX.Dump { Rom = megaRom },
                new Models.OpenMSX.Dump { Rom = sccPlusCart },
            };

            var software = new Models.OpenMSX.Software
            {
                Title = "Software Title",
                GenMSXID = "00000", // Not required
                System = "MSX 2",
                Company = "Imaginary Company, Inc.",
                Year = "19xx",
                Country = "Imaginaria",
                Dump = dump,
            };

            return new Models.OpenMSX.SoftwareDb
            {
                Timestamp = "1234567890",
                Software = new Models.OpenMSX.Software[] { software },
            };
        }

        #endregion
    }
}