using System.Collections.Generic;
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

        // TODO: Write FillHeaderFromPath tests

        #endregion

        #region ResetDictionary

        // TODO: Write ResetDictionary tests

        #endregion

        #region SetHeader

        // TODO: Write SetHeader tests

        #endregion

        #region ExecuteFilters

        // TODO: Write ExecuteFilters tests

        #endregion

        #region CreatePrefixPostfix

        // TODO: Write CreatePrefixPostfix tests

        #endregion

        #region CreatePrefixPostfixDB

        // TODO: Write CreatePrefixPostfixDB tests

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