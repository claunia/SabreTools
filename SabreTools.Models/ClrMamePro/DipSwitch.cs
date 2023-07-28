namespace SabreTools.Models.ClrMamePro
{
    /// <remarks>dipswitch</remarks>
    public class DipSwitch
    {
        /// <remarks>name</remarks>
        public string Name { get; set; }

        /// <remarks>entry</remarks>
        public string[]? Entry { get; set; }

        /// <remarks>default</remarks>
        public string? Default { get; set; }

        #region DO NOT USE IN PRODUCTION

        /// <remarks>Should be empty</remarks>
        public object[]? ADDITIONAL_ELEMENTS { get; set; }

        #endregion
    }
}