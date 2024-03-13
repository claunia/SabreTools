namespace SabreTools.Core.Filter
{
    /// <summary>
    /// Determines what operation is being done
    /// </summary>
    public enum Operation
    {
        /// <summary>
        /// Default value, does nothing
        /// </summary>
        NONE,
        
        Equals,
        NotEquals,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual,
    }
}