namespace SabreTools.Models.EverdriveSMDB
{
    public class Row
    {
        public string SHA256 { get; set; }

        public string Name { get; set; }

        public string SHA1 { get; set; }

        public string MD5 { get; set; }

        public string CRC32 { get; set; }

        public long? Size { get; set; }
    }
}