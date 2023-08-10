using System.Linq;
using SabreTools.Models.Internal;

namespace SabreTools.Serialization
{
    /// <summary>
    /// Serializer for AttractMode models to internal structure
    /// </summary>
    public partial class Internal
    {
        #region Serialize

        /// <summary>
        /// Convert from <cref="Models.AttractMode.MetadataFile"/> to <cref="MetadataFile"/>
        /// </summary>
        public static MetadataFile ConvertToInternalModel(Models.AttractMode.MetadataFile item)
        {
            var metadataFile = new MetadataFile
            {
                [MetadataFile.HeaderKey] = ConvertHeaderToInternalModel(item),
            };

            if (item?.Row != null && item.Row.Any())
                metadataFile[MetadataFile.MachineKey] = item.Row.Select(ConvertMachineToInternalModel).ToArray();

            return metadataFile;
        }

        /// <summary>
        /// Convert from <cref="Models.AttractMode.MetadataFile"/> to <cref="Header"/>
        /// </summary>
        private static Header ConvertHeaderToInternalModel(Models.AttractMode.MetadataFile item)
        {
            var header = new Header
            {
                [Header.HeaderKey] = item.Header,
            };
            return header;
        }

        /// <summary>
        /// Convert from <cref="Models.AttractMode.Row"/> to <cref="Machine"/>
        /// </summary>
        private static Machine ConvertMachineToInternalModel(Models.AttractMode.Row item)
        {
            var machine = new Machine
            {
                [Machine.NameKey] = item.Name,
                [Machine.EmulatorKey] = item.Emulator,
                [Machine.CloneOfKey] = item.CloneOf,
                [Machine.YearKey] = item.Year,
                [Machine.ManufacturerKey] = item.Manufacturer,
                [Machine.CategoryKey] = item.Category,
                [Machine.PlayersKey] = item.Players,
                [Machine.RotationKey] = item.Rotation,
                [Machine.ControlKey] = item.Control,
                [Machine.StatusKey] = item.Status,
                [Machine.DisplayCountKey] = item.DisplayCount,
                [Machine.DisplayTypeKey] = item.DisplayType,
                [Machine.ExtraKey] = item.Extra,
                [Machine.ButtonsKey] = item.Buttons,
                [Machine.FavoriteKey] = item.Favorite,
                [Machine.TagsKey] = item.Tags,
                [Machine.PlayedCountKey] = item.PlayedCount,
                [Machine.PlayedTimeKey] = item.PlayedTime,
                [Machine.PlayedTimeKey] = item.PlayedTime,
                [Machine.RomKey] = ConvertToInternalModel(item),
            };
            return machine;
        }

        /// <summary>
        /// Convert from <cref="Models.AttractMode.Row"/> to <cref="Rom"/>
        /// </summary>
        private static Rom ConvertToInternalModel(Models.AttractMode.Row item)
        {
            var rom = new Rom
            {
                [Rom.NameKey] = item.Title,
                [Rom.AltRomnameKey] = item.AltRomname,
                [Rom.AltTitleKey] = item.AltTitle,
                [Rom.FileIsAvailableKey] = item.FileIsAvailable,
            };
            return rom;
        }

        #endregion

        #region Deserialize

        /// <summary>
        /// Convert from <cref="Header"/> to <cref="Models.AttractMode.MetadataFile"/>
        /// </summary>
        public static Models.AttractMode.MetadataFile? ConvertHeaderToAttractMode(Header? item)
        {
            if (item == null)
                return null;

            var metadataFile = new Models.AttractMode.MetadataFile
            {
                Header = item.ReadStringArray(Header.HeaderKey),
            };
            return metadataFile;
        }

        /// <summary>
        /// Convert from <cref="Machine"/> to an array of <cref="Models.AttractMode.Row"/>
        /// </summary>
        public static Models.AttractMode.Row?[]? ConvertMachineToAttractMode(Machine? item)
        {
            if (item == null)
                return null;

            var roms = item.Read<Rom[]>(Machine.RomKey);
            return roms?.Select(rom =>
            {
                if (rom == null)
                    return null;

                var rowItem = ConvertToAttractMode(rom);

                rowItem.Name = item.ReadString(Machine.NameKey);
                rowItem.Emulator = item.ReadString(Machine.EmulatorKey);
                rowItem.CloneOf = item.ReadString(Machine.CloneOfKey);
                rowItem.Year = item.ReadString(Machine.YearKey);
                rowItem.Manufacturer = item.ReadString(Machine.ManufacturerKey);
                rowItem.Category = item.ReadString(Machine.CategoryKey);
                rowItem.Players = item.ReadString(Machine.PlayersKey);
                rowItem.Rotation = item.ReadString(Machine.RotationKey);
                rowItem.Control = item.ReadString(Machine.ControlKey);
                rowItem.Status = item.ReadString(Machine.StatusKey);
                rowItem.DisplayCount = item.ReadString(Machine.DisplayCountKey);
                rowItem.DisplayType = item.ReadString(Machine.DisplayTypeKey);
                rowItem.Extra = item.ReadString(Machine.ExtraKey);
                rowItem.Buttons = item.ReadString(Machine.ButtonsKey);
                rowItem.Favorite = item.ReadString(Machine.FavoriteKey);
                rowItem.Tags = item.ReadString(Machine.TagsKey);
                rowItem.PlayedCount = item.ReadString(Machine.PlayedCountKey);
                rowItem.PlayedTime = item.ReadString(Machine.PlayedTimeKey);

                return rowItem;
            })?.ToArray();
        }

        /// <summary>
        /// Convert from <cref="Rom"/> to <cref="Models.AttractMode.Row"/>
        /// </summary>
        private static Models.AttractMode.Row? ConvertToAttractMode(Rom? item)
        {
            if (item == null)
                return null;

            var row = new Models.AttractMode.Row
            {
                Title = item.ReadString(Rom.NameKey),
                AltRomname = item.ReadString(Rom.AltRomnameKey),
                AltTitle = item.ReadString(Rom.AltTitleKey),
                FileIsAvailable = item.ReadString(Rom.FileIsAvailableKey),
            };
            return row;
        }

        #endregion
    }
}