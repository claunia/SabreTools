using System.Collections.Generic;
using System.Linq;

namespace SabreTools.Serialization
{
    /// <summary>
    /// Serializer for AttractMode models to internal structure
    /// </summary>
    public partial class Internal
    {
        #region Serialize

        /// <summary>
        /// Convert from <cref="Models.AttractMode.Row"/> to <cref="Models.Internal.Machine"/>
        /// </summary>
        public static Models.Internal.Machine ConvertMachineFromAttractMode(Models.AttractMode.Row item)
        {
            var machine = new Models.Internal.Machine
            {
                [Models.Internal.Machine.NameKey] = item.Name,
                [Models.Internal.Machine.EmulatorKey] = item.Emulator,
                [Models.Internal.Machine.CloneOfKey] = item.CloneOf,
                [Models.Internal.Machine.YearKey] = item.Year,
                [Models.Internal.Machine.ManufacturerKey] = item.Manufacturer,
                [Models.Internal.Machine.CategoryKey] = item.Category,
                [Models.Internal.Machine.PlayersKey] = item.Players,
                [Models.Internal.Machine.RotationKey] = item.Rotation,
                [Models.Internal.Machine.ControlKey] = item.Control,
                [Models.Internal.Machine.StatusKey] = item.Status,
                [Models.Internal.Machine.DisplayCountKey] = item.DisplayCount,
                [Models.Internal.Machine.DisplayTypeKey] = item.DisplayType,
                [Models.Internal.Machine.ExtraKey] = item.Extra,
                [Models.Internal.Machine.ButtonsKey] = item.Buttons,
                [Models.Internal.Machine.FavoriteKey] = item.Favorite,
                [Models.Internal.Machine.TagsKey] = item.Tags,
                [Models.Internal.Machine.PlayedCountKey] = item.PlayedCount,
                [Models.Internal.Machine.PlayedTimeKey] = item.PlayedTime,
                [Models.Internal.Machine.PlayedTimeKey] = item.PlayedTime,
                [Models.Internal.Machine.RomKey] = ConvertFromAttractMode(item),
            };
            return machine;
        }

        /// <summary>
        /// Convert from <cref="Models.AttractMode.Row"/> to <cref="Models.Internal.Rom"/>
        /// </summary>
        public static Models.Internal.Rom ConvertFromAttractMode(Models.AttractMode.Row item)
        {
            var rom = new Models.Internal.Rom
            {
                [Models.Internal.Rom.NameKey] = item.Title,
                [Models.Internal.Rom.AltRomnameKey] = item.AltRomname,
                [Models.Internal.Rom.AltTitleKey] = item.AltTitle,
                [Models.Internal.Rom.FileIsAvailableKey] = item.FileIsAvailable,
            };
            return rom;
        }

        #endregion

        #region Deserialize

        /// <summary>
        /// Convert from <cref="Models.Internal.Machine"/> to an array of <cref="Models.AttractMode.Row"/>
        /// </summary>
        public static Models.AttractMode.Row?[]? ConvertMachineToAttractMode(Models.Internal.Machine? item)
        {
            if (item == null)
                return null;

            var roms = item.Read<Models.Internal.Rom[]>(Models.Internal.Machine.RomKey);
            return roms?.Select(rom =>
            {
                if (rom == null)
                    return null;

                var rowItem = ConvertToAttractMode(rom);

                rowItem.Name = item.ReadString(Models.Internal.Machine.NameKey);
                rowItem.Emulator = item.ReadString(Models.Internal.Machine.EmulatorKey);
                rowItem.CloneOf = item.ReadString(Models.Internal.Machine.CloneOfKey);
                rowItem.Year = item.ReadString(Models.Internal.Machine.YearKey);
                rowItem.Manufacturer = item.ReadString(Models.Internal.Machine.ManufacturerKey);
                rowItem.Category = item.ReadString(Models.Internal.Machine.CategoryKey);
                rowItem.Players = item.ReadString(Models.Internal.Machine.PlayersKey);
                rowItem.Rotation = item.ReadString(Models.Internal.Machine.RotationKey);
                rowItem.Control = item.ReadString(Models.Internal.Machine.ControlKey);
                rowItem.Status = item.ReadString(Models.Internal.Machine.StatusKey);
                rowItem.DisplayCount = item.ReadString(Models.Internal.Machine.DisplayCountKey);
                rowItem.DisplayType = item.ReadString(Models.Internal.Machine.DisplayTypeKey);
                rowItem.Extra = item.ReadString(Models.Internal.Machine.ExtraKey);
                rowItem.Buttons = item.ReadString(Models.Internal.Machine.ButtonsKey);
                rowItem.Favorite = item.ReadString(Models.Internal.Machine.FavoriteKey);
                rowItem.Tags = item.ReadString(Models.Internal.Machine.TagsKey);
                rowItem.PlayedCount = item.ReadString(Models.Internal.Machine.PlayedCountKey);
                rowItem.PlayedTime = item.ReadString(Models.Internal.Machine.PlayedTimeKey);

                return rowItem;
            })?.ToArray();
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Rom"/> to <cref="Models.AttractMode.Row"/>
        /// </summary>
        public static Models.AttractMode.Row? ConvertToAttractMode(Models.Internal.Rom? item)
        {
            if (item == null)
                return null;

            var row = new Models.AttractMode.Row
            {
                Title = item.ReadString(Models.Internal.Rom.NameKey),
                AltRomname = item.ReadString(Models.Internal.Rom.AltRomnameKey),
                AltTitle = item.ReadString(Models.Internal.Rom.AltTitleKey),
                FileIsAvailable = item.ReadString(Models.Internal.Rom.FileIsAvailableKey),
            };
            return row;
        }

        #endregion
    }
}