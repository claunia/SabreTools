using System.IO;
using System.Linq;
using SabreTools.Models.OpenMSX;

namespace SabreTools.Serialization
{
    /// <summary>
    /// XML serializer for OpenMSX software database files
    /// </summary>
    public partial class OpenMSX : XmlSerializer<SoftwareDb>
    {
        /// <inheritdoc cref="SerializeToFile(SoftwareDb, string, string?, string?, string?, string?)" />
        public static bool SerializeToFileWithDocType(SoftwareDb obj, string path)
            => SerializeToFile(obj, path, DocTypeName, DocTypePubId, DocTypeSysId, DocTypeSysId);

        /// <inheritdoc cref="SerializeToStream(SoftwareDb, string?, string?, string?, string?)" />
        public static Stream? SerializeToStreamWithDocType(SoftwareDb obj, string path)
            => SerializeToStream(obj, DocTypeName, DocTypePubId, DocTypeSysId, DocTypeSysId);

        #region Internal

        /// <summary>
        /// Convert from <cref="Models.OpenMSX.SoftwareDb"/> to <cref="Models.Internal.MetadataFile"/>
        /// </summary>
        public static Models.Internal.MetadataFile ConvertToInternalModel(SoftwareDb item)
        {
            var metadataFile = new Models.Internal.MetadataFile
            {
                [Models.Internal.MetadataFile.HeaderKey] = ConvertHeaderToInternalModel(item),
            };

            if (item?.Software != null && item.Software.Any())
                metadataFile[Models.Internal.MetadataFile.MachineKey] = item.Software.Select(ConvertMachineToInternalModel).ToArray();

            return metadataFile;
        }

        /// <summary>
        /// Convert from <cref="Models.OpenMSX.SoftwareDb"/> to <cref="Models.Internal.Header"/>
        /// </summary>
        public static Models.Internal.Header ConvertHeaderToInternalModel(SoftwareDb item)
        {
            var header = new Models.Internal.Header
            {
                [Models.Internal.Header.TimestampKey] = item.Timestamp,
            };
            return header;
        }

        /// <summary>
        /// Convert from <cref="Models.OpenMSX.Software"/> to <cref="Models.Internal.Machine"/>
        /// </summary>
        public static Models.Internal.Machine ConvertMachineToInternalModel(Software item)
        {
            var machine = new Models.Internal.Machine
            {
                [Models.Internal.Machine.NameKey] = item.Title,
                [Models.Internal.Machine.GenMSXIDKey] = item.GenMSXID,
                [Models.Internal.Machine.SystemKey] = item.System,
                [Models.Internal.Machine.CompanyKey] = item.Company,
                [Models.Internal.Machine.YearKey] = item.Year,
                [Models.Internal.Machine.CountryKey] = item.Country,
            };

            if (item.Dump != null && item.Dump.Any())
                machine[Models.Internal.Machine.DumpKey] = item.Dump.Select(ConvertToInternalModel).ToArray();

            return machine;
        }

        /// <summary>
        /// Convert from <cref="Models.OpenMSX.Dump"/> to <cref="Models.Internal.Dump"/>
        /// </summary>
        public static Models.Internal.Dump ConvertToInternalModel(Dump item)
        {
            var dump = new Models.Internal.Dump();

            if (item.Original != null)
                dump[Models.Internal.Dump.OriginalKey] = ConvertToInternalModel(item.Original);

            switch (item.Rom)
            {
                case Rom rom:
                    dump[Models.Internal.Dump.RomKey] = ConvertToInternalModel(rom);
                    break;

                case MegaRom megaRom:
                    dump[Models.Internal.Dump.MegaRomKey] = ConvertToInternalModel(megaRom);
                    break;

                case SCCPlusCart sccPlusCart:
                    dump[Models.Internal.Dump.SCCPlusCartKey] = ConvertToInternalModel(sccPlusCart);
                    break;
            }

            return dump;
        }

        /// <summary>
        /// Convert from <cref="Models.OpenMSX.Original"/> to <cref="Models.Internal.Rom"/>
        /// </summary>
        public static Models.Internal.Original ConvertToInternalModel(Original item)
        {
            var original = new Models.Internal.Original
            {
                [Models.Internal.Original.ValueKey] = item.Value,
                [Models.Internal.Original.ContentKey] = item.Content,
            };
            return original;
        }

        /// <summary>
        /// Convert from <cref="Models.OpenMSX.RomBase"/> to <cref="Models.Internal.Rom"/>
        /// </summary>
        public static Models.Internal.Rom ConvertToInternalModel(RomBase item)
        {
            var rom = new Models.Internal.Rom
            {
                [Models.Internal.Rom.StartKey] = item.Start,
                [Models.Internal.Rom.TypeKey] = item.Type,
                [Models.Internal.Rom.SHA1Key] = item.Hash,
                [Models.Internal.Rom.RemarkKey] = item.Remark,
            };
            return rom;
        }

        #endregion
    }
}