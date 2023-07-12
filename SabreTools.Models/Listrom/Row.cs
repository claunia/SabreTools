namespace SabreTools.Models.Listrom
{
    /// <summary>
    /// ROMs required for driver "testdriver".
    /// Name                                   Size Checksum
    /// abcd.bin                               1024 CRC(00000000) SHA1(da39a3ee5e6b4b0d3255bfef95601890afd80709)
    /// efgh.bin                               1024 BAD CRC(00000000) SHA1(da39a3ee5e6b4b0d3255bfef95601890afd80709) BAD_DUMP
    /// ijkl.bin                               1024 NO GOOD DUMP KNOWN
    /// abcd.chd                                    SHA1(da39a3ee5e6b4b0d3255bfef95601890afd80709)
    /// efgh.chd                                    BAD (da39a3ee5e6b4b0d3255bfef95601890afd80709) BAD_DUMP
    /// ijkl.chd                                    NO GOOD DUMP KNOWN
    /// </summary>
    public class Row
    {
        public string Name { get; set; }

        public long? Size { get; set; }

        public bool Bad { get; set; }

        public string? CRC { get; set; }

        public string? SHA1 { get; set; }

        public bool NoGoodDumpKnown { get; set; }
    }
}