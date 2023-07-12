namespace SabreTools.Models.DosCenter
{
    /// <remarks>file</remarks>
    public class File
    {
        /// <remarks>name, attribute</remarks>
        public string? Name { get; set; }

        /// <remarks>size, attribute</remarks>
        public long? Size { get; set; }

        /// <remarks>crc, attribute</remarks>
        public string? CRC { get; set; }

        /// <remarks>date, attribute</remarks>
        public string? Date { get; set; }
    }
}