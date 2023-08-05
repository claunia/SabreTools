using System.Collections.Generic;

namespace SabreTools.Serialization
{
    /// <summary>
    /// Serializer for EverdriveSMDB models to internal structure
    /// </summary>
    public partial class Internal
    {
        #region Serialize

        /// <summary>
        /// Convert from <cref="Models.EverdriveSMDB.Row"/> to <cref="Models.Internal.Machine"/>
        /// </summary>
        public static Models.Internal.Machine ConvertMachineFromEverdriveSMDB(Models.EverdriveSMDB.Row item)
        {
            var machine = new Models.Internal.Machine
            {
                [Models.Internal.Machine.RomKey] = ConvertFromEverdriveSMDB(item),
            };
            return machine;
        }

        /// <summary>
        /// Convert from <cref="Models.EverdriveSMDB.Row"/> to <cref="Models.Internal.Rom"/>
        /// </summary>
        public static Models.Internal.Rom ConvertFromEverdriveSMDB(Models.EverdriveSMDB.Row item)
        {
            var rom = new Models.Internal.Rom
            {
                [Models.Internal.Rom.SHA256Key] = item.SHA256,
                [Models.Internal.Rom.NameKey] = item.Name,
                [Models.Internal.Rom.SHA1Key] = item.SHA1,
                [Models.Internal.Rom.MD5Key] = item.MD5,
                [Models.Internal.Rom.CRCKey] = item.CRC32,
                [Models.Internal.Rom.SizeKey] = item.Size,
            };
            return rom;
        }

        #endregion

        #region Deserialize

        /// <summary>
        /// Convert from <cref="Models.Internal.Machine"/> to an array of <cref="Models.EverdriveSMDB.Row"/>
        /// </summary>
        public static Models.EverdriveSMDB.Row[]? ConvertMachineToEverdriveSMDB(Models.Internal.Machine item)
        {
            if (!item.ContainsKey(Models.Internal.Machine.RomKey) || item[Models.Internal.Machine.RomKey] is not Models.Internal.Rom[] roms)
                return null;

            var fileItems = new List<Models.EverdriveSMDB.Row>();
            foreach (var rom in roms)
            {
                fileItems.Add(ConvertToEverdriveSMDB(rom));
            }
            return fileItems.ToArray();
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Rom"/> to <cref="Models.EverdriveSMDB.Row"/>
        /// </summary>
        public static Models.EverdriveSMDB.Row ConvertToEverdriveSMDB(Models.Internal.Rom item)
        {
            var row = new Models.EverdriveSMDB.Row
            {
                SHA256 = item.ReadString(Models.Internal.Rom.SHA256Key),
                Name = item.ReadString(Models.Internal.Rom.NameKey),
                SHA1 = item.ReadString(Models.Internal.Rom.SHA1Key),
                MD5 = item.ReadString(Models.Internal.Rom.MD5Key),
                CRC32 = item.ReadString(Models.Internal.Rom.CRCKey),
                Size = item.ReadString(Models.Internal.Rom.SizeKey),
            };
            return row;
        }

        #endregion
    }
}