using System;
using System.Linq;
using SabreTools.DatFiles.Formats;
using SabreTools.DatItems;
using Xunit;

namespace SabreTools.DatFiles.Test
{
    /// <summary>
    /// Contains tests for all specific DatFile formats
    /// </summary>
    public class FormatsTests
    {
        #region Testing Constants

        /// <summary>
        /// All defined item types
        /// </summary>
        private static readonly ItemType[] AllTypes = Enum.GetValues(typeof(ItemType)) as ItemType[] ?? [];

        #endregion

        #region ArchiveDotOrg

        [Fact]
        public void ArchiveDotOrg_SupportedTypes()
        {
            var datFile = new ArchiveDotOrg(null);
            var actual = datFile.SupportedTypes;
            Assert.True(actual.SequenceEqual([ItemType.Rom]));
        }

        #endregion

        #region AttractMode

        // TODO: Write AttractMode format tests

        [Fact]
        public void AttractMode_SupportedTypes()
        {
            var datFile = new AttractMode(null);
            var actual = datFile.SupportedTypes;
            Assert.True(actual.SequenceEqual([ItemType.Rom]));
        }

        #endregion

        #region ClrMamePro

        // TODO: Write ClrMamePro format tests

        [Fact]
        public void ClrMamePro_SupportedTypes()
        {
            var datFile = new ClrMamePro(null);
            var actual = datFile.SupportedTypes;
            Assert.True(actual.SequenceEqual([
                ItemType.Archive,
                ItemType.BiosSet,
                ItemType.Chip,
                ItemType.DipSwitch,
                ItemType.Disk,
                ItemType.Display,
                ItemType.Driver,
                ItemType.Input,
                ItemType.Media,
                ItemType.Release,
                ItemType.Rom,
                ItemType.Sample,
                ItemType.Sound,
            ]));
        }

        #endregion

        #region DosCenter

        // TODO: Write DosCenter format tests

        [Fact]
        public void DosCenter_SupportedTypes()
        {
            var datFile = new DosCenter(null);
            var actual = datFile.SupportedTypes;
            Assert.True(actual.SequenceEqual([ItemType.Rom]));
        }

        #endregion

        #region EverdriveSMDB

        // TODO: Write EverdriveSMDB format tests

        [Fact]
        public void EverdriveSMDB_SupportedTypes()
        {
            var datFile = new EverdriveSMDB(null);
            var actual = datFile.SupportedTypes;
            Assert.True(actual.SequenceEqual([ItemType.Rom]));
        }

        #endregion

        #region Hashfile

        // TODO: Write Hashfile format tests

        [Fact]
        public void SfvFile_SupportedTypes()
        {
            var datFile = new SfvFile(null);
            var actual = datFile.SupportedTypes;
            Assert.True(actual.SequenceEqual([
                ItemType.Disk,
                ItemType.Media,
                ItemType.Rom,
            ]));
        }

        [Fact]
        public void Md2File_SupportedTypes()
        {
            var datFile = new Md2File(null);
            var actual = datFile.SupportedTypes;
            Assert.True(actual.SequenceEqual([
                ItemType.Disk,
                ItemType.Media,
                ItemType.Rom,
            ]));
        }

        [Fact]
        public void Md4File_SupportedTypes()
        {
            var datFile = new Md4File(null);
            var actual = datFile.SupportedTypes;
            Assert.True(actual.SequenceEqual([
                ItemType.Disk,
                ItemType.Media,
                ItemType.Rom,
            ]));
        }

        [Fact]
        public void Md5File_SupportedTypes()
        {
            var datFile = new Md5File(null);
            var actual = datFile.SupportedTypes;
            Assert.True(actual.SequenceEqual([
                ItemType.Disk,
                ItemType.Media,
                ItemType.Rom,
            ]));
        }

        [Fact]
        public void Sha1File_SupportedTypes()
        {
            var datFile = new Sha1File(null);
            var actual = datFile.SupportedTypes;
            Assert.True(actual.SequenceEqual([
                ItemType.Disk,
                ItemType.Media,
                ItemType.Rom,
            ]));
        }

        [Fact]
        public void Sha256File_SupportedTypes()
        {
            var datFile = new Sha256File(null);
            var actual = datFile.SupportedTypes;
            Assert.True(actual.SequenceEqual([
                ItemType.Disk,
                ItemType.Media,
                ItemType.Rom,
            ]));
        }

        [Fact]
        public void Sha384File_SupportedTypes()
        {
            var datFile = new Sha384File(null);
            var actual = datFile.SupportedTypes;
            Assert.True(actual.SequenceEqual([
                ItemType.Disk,
                ItemType.Media,
                ItemType.Rom,
            ]));
        }

        [Fact]
        public void Sha512File_SupportedTypes()
        {
            var datFile = new Sha512File(null);
            var actual = datFile.SupportedTypes;
            Assert.True(actual.SequenceEqual([
                ItemType.Disk,
                ItemType.Media,
                ItemType.Rom,
            ]));
        }

        [Fact]
        public void SpamSumFile_SupportedTypes()
        {
            var datFile = new SpamSumFile(null);
            var actual = datFile.SupportedTypes;
            Assert.True(actual.SequenceEqual([
                ItemType.Disk,
                ItemType.Media,
                ItemType.Rom,
            ]));
        }

        #endregion

        #region Listrom

        // TODO: Write Listrom format tests

        [Fact]
        public void Listrom_SupportedTypes()
        {
            var datFile = new Listrom(null);
            var actual = datFile.SupportedTypes;
            Assert.True(actual.SequenceEqual([
                ItemType.Disk,
                ItemType.Rom,
            ]));
        }

        #endregion

        #region Listxml

        // TODO: Write Listxml format tests

        [Fact]
        public void Listxml_SupportedTypes()
        {
            var datFile = new Listxml(null);
            var actual = datFile.SupportedTypes;
            Assert.True(actual.SequenceEqual([
                ItemType.Adjuster,
                ItemType.BiosSet,
                ItemType.Chip,
                ItemType.Condition,
                ItemType.Configuration,
                ItemType.Device,
                ItemType.DeviceRef,
                ItemType.DipSwitch,
                ItemType.Disk,
                ItemType.Display,
                ItemType.Driver,
                ItemType.Feature,
                ItemType.Input,
                ItemType.Port,
                ItemType.RamOption,
                ItemType.Rom,
                ItemType.Sample,
                ItemType.Slot,
                ItemType.SoftwareList,
                ItemType.Sound,
            ]));
        }

        #endregion

        #region Logiqx

        // TODO: Write Logiqx format tests

        [Fact]
        public void Logiqx_SupportedTypes()
        {
            var datFile = new Logiqx(null, false);
            var actual = datFile.SupportedTypes;
            Assert.True(actual.SequenceEqual([
                ItemType.Archive,
                ItemType.BiosSet,
                ItemType.Disk,
                ItemType.Media,
                ItemType.Release,
                ItemType.Rom,
                ItemType.Sample,
            ]));
        }

        #endregion

        #region Missfile

        // TODO: Write Missfile format tests

        [Fact]
        public void Missfile_SupportedTypes()
        {
            var datFile = new Missfile(null);
            var actual = datFile.SupportedTypes;
            Assert.True(actual.SequenceEqual(AllTypes));
        }

        #endregion

        #region OfflineList

        // TODO: Write OfflineList format tests

        [Fact]
        public void OfflineList_SupportedTypes()
        {
            var datFile = new OfflineList(null);
            var actual = datFile.SupportedTypes;
            Assert.True(actual.SequenceEqual([ItemType.Rom]));
        }

        #endregion

        #region OpenMSX

        // TODO: Write OpenMSX format tests

        [Fact]
        public void OpenMSX_SupportedTypes()
        {
            var datFile = new OpenMSX(null);
            var actual = datFile.SupportedTypes;
            Assert.True(actual.SequenceEqual([ItemType.Rom]));
        }

        #endregion

        #region RomCenter

        // TODO: Write RomCenter format tests

        [Fact]
        public void RomCenter_SupportedTypes()
        {
            var datFile = new RomCenter(null);
            var actual = datFile.SupportedTypes;
            Assert.True(actual.SequenceEqual([ItemType.Rom]));
        }

        #endregion

        #region SabreJSON

        // TODO: Write SabreJSON format tests

        [Fact]
        public void SabreJSON_SupportedTypes()
        {
            var datFile = new SabreJSON(null);
            var actual = datFile.SupportedTypes;
            Assert.True(actual.SequenceEqual(AllTypes));
        }

        #endregion

        #region SabreXML

        // TODO: Write SabreXML format tests

        [Fact]
        public void SabreXML_SupportedTypes()
        {
            var datFile = new SabreXML(null);
            var actual = datFile.SupportedTypes;
            Assert.True(actual.SequenceEqual(AllTypes));
        }

        #endregion

        #region SeparatedValue

        // TODO: Write SeparatedValue format tests

        [Fact]
        public void CommaSeparatedValue_SupportedTypes()
        {
            var datFile = new CommaSeparatedValue(null);
            var actual = datFile.SupportedTypes;
            Assert.True(actual.SequenceEqual([
                ItemType.Disk,
                ItemType.Media,
                ItemType.Rom,
            ]));
        }

        [Fact]
        public void SemicolonSeparatedValue_SupportedTypes()
        {
            var datFile = new SemicolonSeparatedValue(null);
            var actual = datFile.SupportedTypes;
            Assert.True(actual.SequenceEqual([
                ItemType.Disk,
                ItemType.Media,
                ItemType.Rom,
            ]));
        }

        [Fact]
        public void TabSeparatedValue_SupportedTypes()
        {
            var datFile = new TabSeparatedValue(null);
            var actual = datFile.SupportedTypes;
            Assert.True(actual.SequenceEqual([
                ItemType.Disk,
                ItemType.Media,
                ItemType.Rom,
            ]));
        }

        #endregion

        #region SoftwareList

        // TODO: Write SoftwareList format tests

        [Fact]
        public void SoftwareList_SupportedTypes()
        {
            var datFile = new SoftwareList(null);
            var actual = datFile.SupportedTypes;
            Assert.True(actual.SequenceEqual([
                ItemType.DipSwitch,
                ItemType.Disk,
                ItemType.Info,
                ItemType.PartFeature,
                ItemType.Rom,
                ItemType.SharedFeat,
            ]));
        }

        #endregion
    }
}