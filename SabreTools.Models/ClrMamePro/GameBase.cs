namespace SabreTools.Models.ClrMamePro
{
    /// <summary>
    /// Base class to unify the various game-like types
    /// </summary>
    public abstract class GameBase
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

        /// <remarks>release, biosset, rom, disk, media, sample, archive</remarks>
        public ItemBase[]? Item { get; set; }

        #region DO NOT USE IN PRODUCTION

        /// <remarks>Should be empty</remarks>
        public string[]? ADDITIONAL_ELEMENTS { get; set; }

        #endregion
    }
}