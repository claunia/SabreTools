using System.Collections.Generic;
using System.Linq;

namespace SabreTools.Serialization
{
    /// <summary>
    /// Serializer for RomCenter models to internal structure
    /// </summary>
    public partial class Internal
    {
        #region Serialize

        /// <summary>
        /// Convert from <cref="Models.RomCenter.Game"/> to <cref="Models.Internal.Machine"/>
        /// </summary>
        public static Models.Internal.Machine ConvertMachineFromRomCenter(Models.RomCenter.Rom item)
        {
            var machine = new Models.Internal.Machine
            {
                [Models.Internal.Machine.RomOfKey] = item.ParentName,
                //[Models.Internal.Machine.RomOfKey] = item.ParentDescription, // This is unmappable
                [Models.Internal.Machine.NameKey] = item.GameName,
                [Models.Internal.Machine.DescriptionKey] = item.GameDescription,
                [Models.Internal.Machine.RomKey] = new Models.Internal.Rom[] { ConvertFromRomCenter(item) },
            };

            return machine;
        }

        /// <summary>
        /// Convert from <cref="Models.RomCenter.Rom"/> to <cref="Models.Internal.Rom"/>
        /// </summary>
        public static Models.Internal.Rom ConvertFromRomCenter(Models.RomCenter.Rom item)
        {
            var rom = new Models.Internal.Rom
            {
                [Models.Internal.Rom.NameKey] = item.RomName,
                [Models.Internal.Rom.CRCKey] = item.RomCRC,
                [Models.Internal.Rom.SizeKey] = item.RomSize,
                [Models.Internal.Rom.MergeKey] = item.MergeName,
            };
            return rom;
        }

        #endregion

        #region Deserialize

        /// <summary>
        /// Convert from <cref="Models.Internal.Machine"/> to an array of <cref="Models.RomCenter.Rom"/>
        /// </summary>
        public static Models.RomCenter.Rom[] ConvertMachineToRomCenter(Models.Internal.Machine item)
        {
            var romItems = new List<Models.RomCenter.Rom>();

            if (item.ContainsKey(Models.Internal.Machine.RomKey) && item[Models.Internal.Machine.RomKey] is Models.Internal.Rom[] roms)
            {
                foreach (var rom in roms)
                {
                    var romItem = ConvertToRomCenter(rom);

                    romItem.ParentName = rom.ReadString(Models.Internal.Machine.RomOfKey);
                    //romItem.ParentDescription = rom.ReadString(Models.Internal.Machine.RomOfKey); // This is unmappable
                    romItem.GameName = rom.ReadString(Models.Internal.Machine.NameKey);
                    romItem.GameDescription = rom.ReadString(Models.Internal.Machine.DescriptionKey);

                    romItems.Add(romItem);
                }
            }

            return romItems.ToArray();
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Rom"/> to <cref="Models.RomCenter.Rom"/>
        /// </summary>
        public static Models.RomCenter.Rom ConvertToRomCenter(Models.Internal.Rom item)
        {
            var row = new Models.RomCenter.Rom
            {
                RomName = item.ReadString(Models.Internal.Rom.NameKey),
                RomCRC = item.ReadString(Models.Internal.Rom.CRCKey),
                RomSize = item.ReadString(Models.Internal.Rom.SizeKey),
                MergeName = item.ReadString(Models.Internal.Rom.MergeKey),
            };
            return row;
        }

        #endregion
    }
}