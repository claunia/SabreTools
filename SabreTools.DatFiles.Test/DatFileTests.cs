using System.Collections.Generic;
using System.IO;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;
using Xunit;

namespace SabreTools.DatFiles.Test
{
    public class DatFileTests
    {
        #region Constructor

        [Fact]
        public void Constructor_Null()
        {
            DatFile? datFile = null;
            DatFile created = new Formats.Logiqx(datFile, deprecated: false);

            Assert.NotNull(created.Header);
            Assert.NotNull(created.Items);
            Assert.Empty(created.Items);
            Assert.NotNull(created.ItemsDB);
            Assert.Empty(created.ItemsDB.GetItems());
        }

        [Fact]
        public void Constructor_NonNull()
        {
            DatFile? datFile = new Formats.Logiqx(datFile: null, deprecated: false);
            datFile.Header.SetFieldValue(Models.Metadata.Header.NameKey, "name");
            datFile.Items.Add("key", new Rom());
            datFile.ItemsDB.AddItem(new Rom(), 0, 0, false);

            DatFile created = new Formats.Logiqx(datFile, deprecated: false);

            Assert.NotNull(created.Header);
            Assert.Equal("name", created.Header.GetStringFieldValue(Models.Metadata.Header.NameKey));

            Assert.NotNull(created.Items);
            KeyValuePair<string, List<DatItem>?> itemsKvp = Assert.Single(created.Items);
            Assert.Equal("key", itemsKvp.Key);
            Assert.NotNull(itemsKvp.Value);
            DatItem datItem = Assert.Single(itemsKvp.Value);
            Assert.True(datItem is Rom);

            Assert.NotNull(created.ItemsDB);
            KeyValuePair<long, DatItem> dbKvp = Assert.Single(created.ItemsDB.GetItems());
            Assert.Equal(0, dbKvp.Key);
            Assert.True(dbKvp.Value is Rom);
        }

        #endregion

        #region FillHeaderFromPath

        [Fact]
        public void FillHeaderFromPath_NoNameNoDesc_NotBare()
        {
            DatFile datFile = new Formats.Logiqx(datFile: null, deprecated: false);
            datFile.Header.SetFieldValue(Models.Metadata.Header.NameKey, string.Empty);
            datFile.Header.SetFieldValue(Models.Metadata.Header.DescriptionKey, string.Empty);
            datFile.Header.SetFieldValue(Models.Metadata.Header.DateKey, "1980-01-01");

            string path = Path.Combine("Fake", "Path", "Filename");
            datFile.FillHeaderFromPath(path, false);

            Assert.Equal("Filename (1980-01-01)", datFile.Header.GetStringFieldValue(Models.Metadata.Header.NameKey));
            Assert.Equal("Filename (1980-01-01)", datFile.Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey));
        }

        [Fact]
        public void FillHeaderFromPath_NoNameNoDesc_Bare()
        {
            DatFile datFile = new Formats.Logiqx(datFile: null, deprecated: false);
            datFile.Header.SetFieldValue(Models.Metadata.Header.NameKey, string.Empty);
            datFile.Header.SetFieldValue(Models.Metadata.Header.DescriptionKey, string.Empty);
            datFile.Header.SetFieldValue(Models.Metadata.Header.DateKey, "1980-01-01");

            string path = Path.Combine("Fake", "Path", "Filename");
            datFile.FillHeaderFromPath(path, true);

            Assert.Equal("Filename", datFile.Header.GetStringFieldValue(Models.Metadata.Header.NameKey));
            Assert.Equal("Filename", datFile.Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey));
        }

        [Fact]
        public void FillHeaderFromPath_NoNameDesc_NotBare()
        {
            DatFile datFile = new Formats.Logiqx(datFile: null, deprecated: false);
            datFile.Header.SetFieldValue(Models.Metadata.Header.NameKey, string.Empty);
            datFile.Header.SetFieldValue(Models.Metadata.Header.DescriptionKey, "Description");
            datFile.Header.SetFieldValue(Models.Metadata.Header.DateKey, "1980-01-01");

            string path = Path.Combine("Fake", "Path", "Filename");
            datFile.FillHeaderFromPath(path, false);

            Assert.Equal("Description (1980-01-01)", datFile.Header.GetStringFieldValue(Models.Metadata.Header.NameKey));
            Assert.Equal("Description", datFile.Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey));
        }

        [Fact]
        public void FillHeaderFromPath_NoNameDesc_Bare()
        {
            DatFile datFile = new Formats.Logiqx(datFile: null, deprecated: false);
            datFile.Header.SetFieldValue(Models.Metadata.Header.NameKey, string.Empty);
            datFile.Header.SetFieldValue(Models.Metadata.Header.DescriptionKey, "Description");
            datFile.Header.SetFieldValue(Models.Metadata.Header.DateKey, "1980-01-01");

            string path = Path.Combine("Fake", "Path", "Filename");
            datFile.FillHeaderFromPath(path, true);

            Assert.Equal("Description", datFile.Header.GetStringFieldValue(Models.Metadata.Header.NameKey));
            Assert.Equal("Description", datFile.Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey));
        }

        [Fact]
        public void FillHeaderFromPath_NameNoDesc_NotBare()
        {
            DatFile datFile = new Formats.Logiqx(datFile: null, deprecated: false);
            datFile.Header.SetFieldValue(Models.Metadata.Header.NameKey, "Name");
            datFile.Header.SetFieldValue(Models.Metadata.Header.DescriptionKey, string.Empty);
            datFile.Header.SetFieldValue(Models.Metadata.Header.DateKey, "1980-01-01");

            string path = Path.Combine("Fake", "Path", "Filename");
            datFile.FillHeaderFromPath(path, false);

            Assert.Equal("Name", datFile.Header.GetStringFieldValue(Models.Metadata.Header.NameKey));
            Assert.Equal("Name (1980-01-01)", datFile.Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey));
        }

        [Fact]
        public void FillHeaderFromPath_NameNoDesc_Bare()
        {
            DatFile datFile = new Formats.Logiqx(datFile: null, deprecated: false);
            datFile.Header.SetFieldValue(Models.Metadata.Header.NameKey, "Name");
            datFile.Header.SetFieldValue(Models.Metadata.Header.DescriptionKey, string.Empty);
            datFile.Header.SetFieldValue(Models.Metadata.Header.DateKey, "1980-01-01");

            string path = Path.Combine("Fake", "Path", "Filename");
            datFile.FillHeaderFromPath(path, true);

            Assert.Equal("Name", datFile.Header.GetStringFieldValue(Models.Metadata.Header.NameKey));
            Assert.Equal("Name", datFile.Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey));
        }

        [Fact]
        public void FillHeaderFromPath_NameDesc_NotBare()
        {
            DatFile datFile = new Formats.Logiqx(datFile: null, deprecated: false);
            datFile.Header.SetFieldValue(Models.Metadata.Header.NameKey, "Name");
            datFile.Header.SetFieldValue(Models.Metadata.Header.DescriptionKey, "Description");
            datFile.Header.SetFieldValue(Models.Metadata.Header.DateKey, "1980-01-01");

            string path = Path.Combine("Fake", "Path", "Filename");
            datFile.FillHeaderFromPath(path, false);

            Assert.Equal("Name", datFile.Header.GetStringFieldValue(Models.Metadata.Header.NameKey));
            Assert.Equal("Description", datFile.Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey));
        }

        [Fact]
        public void FillHeaderFromPath_NameDesc_Bare()
        {
            DatFile datFile = new Formats.Logiqx(datFile: null, deprecated: false);
            datFile.Header.SetFieldValue(Models.Metadata.Header.NameKey, "Name ");
            datFile.Header.SetFieldValue(Models.Metadata.Header.DescriptionKey, "Description ");
            datFile.Header.SetFieldValue(Models.Metadata.Header.DateKey, "1980-01-01");

            string path = Path.Combine("Fake", "Path", "Filename");
            datFile.FillHeaderFromPath(path, true);

            Assert.Equal("Name", datFile.Header.GetStringFieldValue(Models.Metadata.Header.NameKey));
            Assert.Equal("Description", datFile.Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey));
        }

        #endregion

        #region ResetDictionary

        [Fact]
        public void ResetDictionaryTest()
        {
            DatFile datFile = new Formats.Logiqx(datFile: null, deprecated: false);
            datFile.Header.SetFieldValue(Models.Metadata.Header.NameKey, "name");
            datFile.Items.Add("key", new Rom());
            datFile.ItemsDB.AddItem(new Rom(), 0, 0, false);

            datFile.ResetDictionary();

            Assert.NotNull(datFile.Header);
            Assert.NotNull(datFile.Items);
            Assert.Empty(datFile.Items);
            Assert.NotNull(datFile.ItemsDB);
            Assert.Empty(datFile.ItemsDB.GetItems());
        }

        #endregion

        #region SetHeader

        [Fact]
        public void SetHeaderTest()
        {
            DatHeader datHeader = new DatHeader();
            datHeader.SetFieldValue(Models.Metadata.Header.NameKey, "name");

            DatFile? datFile = new Formats.Logiqx(datFile: null, deprecated: false);
            datFile.Header.SetFieldValue(Models.Metadata.Header.NameKey, "notname");

            datFile.SetHeader(datHeader);
            Assert.NotNull(datFile.Header);
            Assert.Equal("name", datFile.Header.GetStringFieldValue(Models.Metadata.Header.NameKey));
        }

        #endregion

        #region FormatPrefixPostfix

        // TODO: Write FormatPrefixPostfix tests

        #endregion

        #region ProcessItemName

        // TODO: Write ProcessItemName tests

        #endregion

        #region ProcessItemNameDB

        // TODO: Write ProcessItemNameDB tests

        #endregion

        #region ProcessNullifiedItem

        // TODO: Write ProcessNullifiedItem tests

        #endregion

        #region ProcessNullifiedItemDB

        // TODO: Write ProcessNullifiedItemDB tests

        #endregion

        #region GetMissingRequiredFields

        // TODO: Write GetMissingRequiredFields tests

        #endregion

        #region ContainsWritable

        // TODO: Write ContainsWritable tests

        #endregion

        #region ContainsWritableDB

        // TODO: Write ContainsWritableDB tests

        #endregion

        #region ResolveNames

        // TODO: Write ResolveNames tests

        #endregion

        #region ResolveNamesDB

        // TODO: Write ResolveNamesDB tests

        #endregion

        #region ShouldIgnore

        // TODO: Write ShouldIgnore tests

        #endregion
    }
}