namespace SabreTools.Core.Filter
{
    /// <summary>
    /// Represents a single filter key
    /// </summary>
    public class FilterKey
    {
        /// <summary>
        /// Item name associated with the filter
        /// </summary>
        public readonly string ItemName;

        /// <summary>
        /// Field name associated with the filter
        /// </summary>
        public readonly string FieldName;

        /// <summary>
        /// Discrete value constructor
        /// </summary>
        public FilterKey(string itemName, string fieldName)
        {
            ItemName = itemName;
            FieldName = fieldName;
        }

        /// <inheritdoc/>
        public override string ToString() => $"{ItemName}.{FieldName}";
    }
}