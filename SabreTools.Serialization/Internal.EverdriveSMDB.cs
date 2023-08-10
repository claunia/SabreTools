using System.Linq;
using SabreTools.Models.Internal;

namespace SabreTools.Serialization
{
    /// <summary>
    /// Serializer for EverdriveSMDB models to internal structure
    /// </summary>
    public partial class Internal
    {
        #region Serialize

        /// <summary>
        /// Convert from <cref="Models.EverdriveSMDB.MetadataFile"/> to <cref="Header"/>
        /// </summary>
        public static Header ConvertHeaderFromEverdriveSMDB(Models.EverdriveSMDB.MetadataFile item)
        {
            var header = new Header
            {
                [Header.NameKey] = "Everdrive SMDB",
            };
            return header;
        }

        /// <summary>
        /// Convert from <cref="Models.EverdriveSMDB.Row"/> to <cref="Machine"/>
        /// </summary>
        public static Machine ConvertMachineFromEverdriveSMDB(Models.EverdriveSMDB.Row item)
        {
            var machine = new Machine
            {
                [Machine.RomKey] = ConvertFromEverdriveSMDB(item),
            };
            return machine;
        }

        /// <summary>
        /// Convert from <cref="Models.EverdriveSMDB.Row"/> to <cref="Rom"/>
        /// </summary>
        public static Rom ConvertFromEverdriveSMDB(Models.EverdriveSMDB.Row item)
        {
            var rom = new Rom
            {
                [Rom.SHA256Key] = item.SHA256,
                [Rom.NameKey] = item.Name,
                [Rom.SHA1Key] = item.SHA1,
                [Rom.MD5Key] = item.MD5,
                [Rom.CRCKey] = item.CRC32,
                [Rom.SizeKey] = item.Size,
            };
            return rom;
        }

        #endregion

        #region Deserialize

        /// <summary>
        /// Convert from <cref="Machine"/> to an array of <cref="Models.EverdriveSMDB.Row"/>
        /// </summary>
        public static Models.EverdriveSMDB.Row[]? ConvertMachineToEverdriveSMDB(Machine item)
        {
            if (item == null)
                return null;

            var roms = item.Read<Rom[]>(Machine.RomKey);
            return roms?.Select(ConvertToEverdriveSMDB)?.ToArray();
        }

        /// <summary>
        /// Convert from <cref="Rom"/> to <cref="Models.EverdriveSMDB.Row"/>
        /// </summary>
        private static Models.EverdriveSMDB.Row? ConvertToEverdriveSMDB(Rom? item)
        {
            if (item == null)
                return null;

            var row = new Models.EverdriveSMDB.Row
            {
                SHA256 = item.ReadString(Rom.SHA256Key),
                Name = item.ReadString(Rom.NameKey),
                SHA1 = item.ReadString(Rom.SHA1Key),
                MD5 = item.ReadString(Rom.MD5Key),
                CRC32 = item.ReadString(Rom.CRCKey),
                Size = item.ReadString(Rom.SizeKey),
            };
            return row;
        }

        #endregion
    }
}