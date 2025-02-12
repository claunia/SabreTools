using System.Collections.Generic;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;
using Xunit;

namespace SabreTools.DatFiles.Test
{
    public class ReplacerTests
    {
        #region ReplaceFields

        [Fact]
        public void ReplaceFields_Machine()
        {
            var machine = new Machine();
            machine.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "bar");
            machine.SetFieldValue<string?>(Models.Metadata.Machine.DescriptionKey, "bar");

            var repMachine = new Machine();
            machine.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "foo");
            machine.SetFieldValue<string?>(Models.Metadata.Machine.DescriptionKey, "bar");

            List<string> fields = [Models.Metadata.Machine.NameKey];

            Replacer.ReplaceFields(machine, repMachine, fields, false);

            Assert.Equal("foo", machine.GetStringFieldValue(Models.Metadata.Machine.NameKey));
        }

        // TODO: Check more fields for replacement
        [Fact]
        public void ReplaceFields_Disk()
        {
            var datItem = new Disk();
            datItem.SetName("foo");

            var repDatItem = new Disk();
            repDatItem.SetName("bar");

            var fields = new Dictionary<string, List<string>>
            {
                ["item"] = [Models.Metadata.Disk.NameKey]
            };

            Replacer.ReplaceFields(datItem, repDatItem, fields);

            Assert.Equal("bar", datItem.GetName());
        }

        // TODO: Check more fields for replacement
        [Fact]
        public void ReplaceFields_File()
        {
            var datItem = new DatItems.Formats.File();
            datItem.SetName("foo");

            var repDatItem = new DatItems.Formats.File();
            repDatItem.SetName("bar");

            var fields = new Dictionary<string, List<string>>
            {
                ["item"] = [Models.Metadata.Rom.NameKey]
            };

            Replacer.ReplaceFields(datItem, repDatItem, fields);

            // TODO: There is no name field for File type
            //Assert.Equal("bar", datItem.GetName());
        }

        // TODO: Check more fields for replacement
        [Fact]
        public void ReplaceFields_Media()
        {
            var datItem = new Media();
            datItem.SetName("foo");

            var repDatItem = new Media();
            repDatItem.SetName("bar");

            var fields = new Dictionary<string, List<string>>
            {
                ["item"] = [Models.Metadata.Media.NameKey]
            };

            Replacer.ReplaceFields(datItem, repDatItem, fields);

            Assert.Equal("bar", datItem.GetName());
        }

        // TODO: Check more fields for replacement
        [Fact]
        public void ReplaceFields_Rom()
        {
            var datItem = new Rom();
            datItem.SetName("foo");

            var repDatItem = new Rom();
            repDatItem.SetName("bar");

            var fields = new Dictionary<string, List<string>>
            {
                ["item"] = [Models.Metadata.Rom.NameKey]
            };

            Replacer.ReplaceFields(datItem, repDatItem, fields);

            Assert.Equal("bar", datItem.GetName());
        }

        [Fact]
        public void ReplaceFields_Sample()
        {
            var datItem = new Sample();
            datItem.SetName("foo");

            var repDatItem = new Sample();
            repDatItem.SetName("bar");

            var fields = new Dictionary<string, List<string>>
            {
                ["item"] = [Models.Metadata.Rom.NameKey]
            };

            Replacer.ReplaceFields(datItem, repDatItem, fields);

            Assert.Equal("bar", datItem.GetName());
        }

        #endregion
    }
}