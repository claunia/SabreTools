using SabreTools.Core.Filter;
using SabreTools.Models.Metadata;
using Xunit;

namespace SabreTools.Core.Test.Filter
{
    public class FieldManipulatorTests
    {
        #region RemoveField

        [Fact]
        public void RemoveField_NullItem_False()
        {
            DictionaryBase? dictionaryBase = null;
            string? fieldName = Sample.NameKey;
            bool actual = FieldManipulator.RemoveField(dictionaryBase, fieldName);
            Assert.False(actual);
        }

        [Fact]
        public void RemoveField_NullFieldName_False()
        {
            DictionaryBase? dictionaryBase = new Sample();
            string? fieldName = null;
            bool actual = FieldManipulator.RemoveField(dictionaryBase, fieldName);
            Assert.False(actual);
        }

        [Fact]
        public void RemoveField_EmptyFieldName_False()
        {
            DictionaryBase? dictionaryBase = new Sample();
            string? fieldName = string.Empty;
            bool actual = FieldManipulator.RemoveField(dictionaryBase, fieldName);
            Assert.False(actual);
        }

        [Fact]
        public void RemoveField_MissingKey_True()
        {
            DictionaryBase? dictionaryBase = new Sample();
            string? fieldName = Sample.NameKey;
            bool actual = FieldManipulator.RemoveField(dictionaryBase, fieldName);
            Assert.True(actual);
            Assert.DoesNotContain(fieldName, dictionaryBase.Keys);
        }

        [Fact]
        public void RemoveField_ValidKey_True()
        {
            DictionaryBase? dictionaryBase = new Sample { [Sample.NameKey] = "value" };
            string? fieldName = Sample.NameKey;
            bool actual = FieldManipulator.RemoveField(dictionaryBase, fieldName);
            Assert.True(actual);
            Assert.DoesNotContain(fieldName, dictionaryBase.Keys);
        }

        #endregion

        #region ReplaceField

        [Fact]
        public void ReplaceField_NullFrom_False()
        {
            DictionaryBase? from = null;
            DictionaryBase? to = new Sample();
            string? fieldName = Sample.NameKey;
            bool actual = FieldManipulator.ReplaceField(from, to, fieldName);
            Assert.False(actual);
        }

        [Fact]
        public void ReplaceField_NullTo_False()
        {
            DictionaryBase? from = null;
            DictionaryBase? to = new Sample();
            string? fieldName = Sample.NameKey;
            bool actual = FieldManipulator.ReplaceField(from, to, fieldName);
            Assert.False(actual);
        }

        [Fact]
        public void ReplaceField_NullFieldName_False()
        {
            DictionaryBase? from = new Sample();
            DictionaryBase? to = new Sample();
            string? fieldName = null;
            bool actual = FieldManipulator.ReplaceField(from, to, fieldName);
            Assert.False(actual);
        }

        [Fact]
        public void ReplaceField_EmptyFieldName_False()
        {
            DictionaryBase? from = new Sample();
            DictionaryBase? to = new Sample();
            string? fieldName = string.Empty;
            bool actual = FieldManipulator.ReplaceField(from, to, fieldName);
            Assert.False(actual);
        }

        [Fact]
        public void ReplaceField_MismatchedTypes_False()
        {
            DictionaryBase? from = new Sample();
            DictionaryBase? to = new Rom();
            string? fieldName = Sample.NameKey;
            bool actual = FieldManipulator.ReplaceField(from, to, fieldName);
            Assert.False(actual);
        }

        [Fact]
        public void ReplaceField_MissingKey_False()
        {
            DictionaryBase? from = new Sample();
            DictionaryBase? to = new Sample();
            string? fieldName = Sample.NameKey;
            bool actual = FieldManipulator.ReplaceField(from, to, fieldName);
            Assert.False(actual);
        }

        [Fact]
        public void ReplaceField_ValidKey_True()
        {
            DictionaryBase? from = new Sample { [Sample.NameKey] = "value" };
            DictionaryBase? to = new Sample();
            string? fieldName = Sample.NameKey;
            bool actual = FieldManipulator.ReplaceField(from, to, fieldName);
            Assert.True(actual);
            Assert.Contains(fieldName, to.Keys);
            Assert.Equal("value", to[Sample.NameKey]);
        }

        #endregion

        #region SetField

        [Fact]
        public void SetField_NullItem_False()
        {
            DictionaryBase? dictionaryBase = null;
            string? fieldName = Sample.NameKey;
            object value = "value";
            bool actual = FieldManipulator.SetField(dictionaryBase, fieldName, value);
            Assert.False(actual);
        }

        [Fact]
        public void SetField_NullFieldName_False()
        {
            DictionaryBase? dictionaryBase = new Sample();
            string? fieldName = null;
            object value = "value";
            bool actual = FieldManipulator.SetField(dictionaryBase, fieldName, value);
            Assert.False(actual);
        }

        [Fact]
        public void SetField_EmptyFieldName_False()
        {
            DictionaryBase? dictionaryBase = new Sample();
            string? fieldName = string.Empty;
            object value = "value";
            bool actual = FieldManipulator.SetField(dictionaryBase, fieldName, value);
            Assert.False(actual);
        }

        [Fact]
        public void SetField_MissingKey_False()
        {
            DictionaryBase? dictionaryBase = new Sample();
            string? fieldName = Rom.SHA1Key;
            object value = "value";
            bool actual = FieldManipulator.SetField(dictionaryBase, fieldName, value);
            Assert.False(actual);
        }

        [Fact]
        public void SetField_ValidKey_True()
        {
            DictionaryBase? dictionaryBase = new Sample { [Sample.NameKey] = "old" };
            string? fieldName = Sample.NameKey;
            object value = "value";
            bool actual = FieldManipulator.SetField(dictionaryBase, fieldName, value);
            Assert.True(actual);
            Assert.Contains(fieldName, dictionaryBase.Keys);
            Assert.Equal(value, dictionaryBase[fieldName]);
        }

        #endregion
    }
}