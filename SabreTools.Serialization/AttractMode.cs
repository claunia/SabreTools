namespace SabreTools.Serialization
{
    /// <summary>
    /// Separated value serializer/deserializer for AttractMode romlists
    /// </summary>
    public partial class AttractMode
    {
        private const string HeaderWithoutRomname = "#Name;Title;Emulator;CloneOf;Year;Manufacturer;Category;Players;Rotation;Control;Status;DisplayCount;DisplayType;AltRomname;AltTitle;Extra;Buttons";
        private const int HeaderWithoutRomnameCount = 17;

        private const string HeaderWithRomname = "#Romname;Title;Emulator;Cloneof;Year;Manufacturer;Category;Players;Rotation;Control;Status;DisplayCount;DisplayType;AltRomname;AltTitle;Extra;Buttons;Favourite;Tags;PlayedCount;PlayedTime;FileIsAvailable";
        private const int HeaderWithRomnameCount = 22;
    }
}