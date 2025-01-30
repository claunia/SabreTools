using Xunit;

namespace SabreTools.DatFiles.Test
{
    public class DatHeaderTests
    {
        #region CanOpenSpecified

        [Fact]
        public void CanOpenSpecified_Missing()
        {
            DatHeader header = new DatHeader();
            header.SetFieldValue<string[]>(Models.Metadata.Header.CanOpenKey, null);
            Assert.False(header.CanOpenSpecified);
        }

        [Fact]
        public void CanOpenSpecified_Empty()
        {
            DatHeader header = new DatHeader();
            header.SetFieldValue<string[]>(Models.Metadata.Header.CanOpenKey, []);
            Assert.False(header.CanOpenSpecified);
        }

        [Fact]
        public void CanOpenSpecified_Exists()
        {
            DatHeader header = new DatHeader();
            header.SetFieldValue<string[]>(Models.Metadata.Header.CanOpenKey, ["value"]);
            Assert.True(header.CanOpenSpecified);
        }

        #endregion

        #region ImagesSpecified

        [Fact]
        public void ImagesSpecified_Missing()
        {
            DatHeader header = new DatHeader();
            header.SetFieldValue<Models.OfflineList.Images>(Models.Metadata.Header.ImagesKey, null);
            Assert.False(header.ImagesSpecified);
        }

        [Fact]
        public void ImagesSpecified_Exists()
        {
            DatHeader header = new DatHeader();
            header.SetFieldValue<Models.OfflineList.Images>(Models.Metadata.Header.ImagesKey, new());
            Assert.True(header.ImagesSpecified);
        }

        #endregion

        #region InfosSpecified

        [Fact]
        public void InfosSpecified_Missing()
        {
            DatHeader header = new DatHeader();
            header.SetFieldValue<Models.OfflineList.Infos>(Models.Metadata.Header.InfosKey, null);
            Assert.False(header.InfosSpecified);
        }

        [Fact]
        public void InfosSpecified_Exists()
        {
            DatHeader header = new DatHeader();
            header.SetFieldValue<Models.OfflineList.Infos>(Models.Metadata.Header.InfosKey, new());
            Assert.True(header.InfosSpecified);
        }

        #endregion

        #region NewDatSpecified

        [Fact]
        public void NewDatSpecified_Missing()
        {
            DatHeader header = new DatHeader();
            header.SetFieldValue<Models.OfflineList.NewDat>(Models.Metadata.Header.NewDatKey, null);
            Assert.False(header.NewDatSpecified);
        }

        [Fact]
        public void NewDatSpecified_Exists()
        {
            DatHeader header = new DatHeader();
            header.SetFieldValue<Models.OfflineList.NewDat>(Models.Metadata.Header.NewDatKey, new());
            Assert.True(header.NewDatSpecified);
        }

        #endregion

        #region SearchSpecified

        [Fact]
        public void SearchSpecified_Missing()
        {
            DatHeader header = new DatHeader();
            header.SetFieldValue<Models.OfflineList.Search>(Models.Metadata.Header.SearchKey, null);
            Assert.False(header.SearchSpecified);
        }

        [Fact]
        public void SearchSpecified_Exists()
        {
            DatHeader header = new DatHeader();
            header.SetFieldValue<Models.OfflineList.Search>(Models.Metadata.Header.SearchKey, new());
            Assert.True(header.SearchSpecified);
        }

        #endregion

        #region Clone

        [Fact]
        public void CloneTest()
        {
            DatHeader header = new DatHeader();
            header.SetFieldValue(Models.Metadata.Header.NameKey, "name");

            object clone = header.Clone();
            DatHeader? actual = clone as DatHeader;
            Assert.NotNull(actual);
            Assert.Equal("name", actual.GetStringFieldValue(Models.Metadata.Header.NameKey));
        }

        #endregion

        #region CloneFormat

        [Fact]
        public void CloneFormatTest()
        {
            DatHeader header = new DatHeader();
            header.SetFieldValue(DatHeader.DatFormatKey, DatFormat.Logiqx);

            object clone = header.Clone();
            DatHeader? actual = clone as DatHeader;
            Assert.NotNull(actual);
            Assert.Equal(DatFormat.Logiqx, actual.GetFieldValue<DatFormat>(DatHeader.DatFormatKey));
        }

        #endregion

        #region GetInternalClone

        [Fact]
        public void GetInternalCloneTest()
        {
            DatHeader header = new DatHeader();
            header.SetFieldValue(Models.Metadata.Header.NameKey, "name");

            Models.Metadata.Header actual = header.GetInternalClone();
            Assert.Equal("name", actual[Models.Metadata.Header.NameKey]);
        }

        #endregion

        #region ConditionalCopy

        // TODO: Implement ConditionalCopy tests

        #endregion

        #region Equals

        [Fact]
        public void Equals_Null_False()
        {
            DatHeader self = new DatHeader();
            DatHeader? other = null;

            bool actual = self.Equals(other);
            Assert.False(actual);
        }

        [Fact]
        public void Equals_DefaultInternal_True()
        {
            DatHeader self = new DatHeader();
            DatHeader? other = new DatHeader();

            bool actual = self.Equals(other);
            Assert.True(actual);
        }

        [Fact]
        public void Equals_MismatchedInternal_False()
        {
            DatHeader self = new DatHeader();
            self.SetFieldValue(Models.Metadata.Header.NameKey, "self");

            DatHeader? other = new DatHeader();
            other.SetFieldValue(Models.Metadata.Header.NameKey, "other");

            bool actual = self.Equals(other);
            Assert.False(actual);
        }

        [Fact]
        public void Equals_EqualInternal_True()
        {
            DatHeader self = new DatHeader();
            self.SetFieldValue(Models.Metadata.Header.NameKey, "name");

            DatHeader? other = new DatHeader();
            other.SetFieldValue(Models.Metadata.Header.NameKey, "name");

            bool actual = self.Equals(other);
            Assert.True(actual);
        }

        #endregion
    }
}