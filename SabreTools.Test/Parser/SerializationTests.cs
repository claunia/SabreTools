using System;
using System.Xml.Serialization;
using Xunit;

namespace SabreTools.Test.Parser
{
    public class SerializationTests
    {
        [Fact]
        public void ArchiveDotOrgDeserializeTest()
        {
            // Open the file for reading
            string filename = System.IO.Path.Combine(Environment.CurrentDirectory, "TestData", "test-archivedotorg-files.xml");
            using var fs = System.IO.File.OpenRead(filename);

            // Setup the serializer
            var serializer = new XmlSerializer(typeof(Models.ArchiveDotOrg.Files));

            // Deserialize the file
            var dat = serializer.Deserialize(fs) as Models.ArchiveDotOrg.Files;

            // Validate the values
            Assert.NotNull(dat);
            Assert.NotNull(dat.File);
            Assert.Equal(22, dat.File.Length);

            // Validate we're not missing any attributes or elements
            Assert.Null(dat.ADDITIONAL_ATTRIBUTES);
            Assert.Null(dat.ADDITIONAL_ELEMENTS);
            foreach (var file in dat.File)
            {
                Assert.Null(file.ADDITIONAL_ATTRIBUTES);
                Assert.Null(file.ADDITIONAL_ELEMENTS);
            }
        }
    }
}