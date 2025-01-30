using Xunit;

namespace SabreTools.DatFiles.Test
{
    public class DatHeaderTests
    {
        // TODO: Specified tests?

        #region Clone

        [Fact]
        public void CloneTest()
        {
            DatHeader item = new DatHeader();
            item.SetFieldValue(Models.Metadata.Header.NameKey, "name");

            object clone = item.Clone();
            DatHeader? actual = clone as DatHeader;
            Assert.NotNull(actual);
            Assert.Equal("name", actual.GetStringFieldValue(Models.Metadata.Header.NameKey));
        }

        #endregion

        #region CloneFormat

        [Fact]
        public void CloneFormatTest()
        {
            DatHeader item = new DatHeader();
            item.SetFieldValue(DatHeader.DatFormatKey, DatFormat.Logiqx);

            object clone = item.Clone();
            DatHeader? actual = clone as DatHeader;
            Assert.NotNull(actual);
            Assert.Equal(DatFormat.Logiqx, actual.GetFieldValue<DatFormat>(DatHeader.DatFormatKey));
        }

        #endregion

        #region GetInternalClone

        [Fact]
        public void GetInternalCloneTest()
        {
            DatHeader item = new DatHeader();
            item.SetFieldValue(Models.Metadata.Header.NameKey, "name");

            Models.Metadata.Header actual = item.GetInternalClone();
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