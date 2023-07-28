namespace SabreTools.Models.ClrMamePro
{
    /// <remarks>chip</remarks>
    public class Chip
    {
        /// <remarks>type, (cpu|audio)</remarks>
        public string Type { get; set; }

        /// <remarks>name</remarks>
        public string Name { get; set; }

        /// <remarks>flags</remarks>
        public string? Flags { get; set; }

        /// <remarks>clock</remarks>
        public string? Clock { get; set; }

        #region DO NOT USE IN PRODUCTION

        /// <remarks>Should be empty</remarks>
        public object[]? ADDITIONAL_ELEMENTS { get; set; }

        #endregion
    }
}