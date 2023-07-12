namespace SabreTools.Models.ClrMamePro
{
    /// <remarks>machine</remarks>
    public class Machine
    {
        /// <remarks>name</remarks>
        public string? Name { get; set; }

        /// <remarks>description</remarks>
        public string? Description { get; set; }

        /// <remarks>year</remarks>
        public string? Year { get; set; }

        /// <remarks>manufacturer</remarks>
        public string? Manufacturer { get; set; }

        /// <remarks>category</remarks>
        public string? Category { get; set; }

        /// <remarks>cloneof</remarks>
        public string? CloneOf { get; set; }

        /// <remarks>romof</remarks>
        public string? RomOf { get; set; }

        /// <remarks>sampleof</remarks>
        public string? SampleOf { get; set; }

        /// <remarks>release</remarks>
        public Release[]? Release { get; set; }

        /// <remarks>biosset</remarks>
        public BiosSet[]? BiosSet { get; set; }

        /// <remarks>rom</remarks>
        public Rom[]? Rom { get; set; }

        /// <remarks>disk</remarks>
        public Disk[]? Disk { get; set; }

        /// <remarks>sample</remarks>
        public Sample[]? Sample { get; set; }

        /// <remarks>archive</remarks>
        public Archive[]? Archive { get; set; }

        #region Aaru Extensions

        /// <remarks>media; Appears after Disk</remarks>
        public Media[]? Media { get; set; }

        #endregion
    }
}