using System;

namespace SabreTools.DatItems
{
    /// <summary>
    /// Source information wrapper
    /// </summary>
    public class Source : ICloneable
    {
        /// <summary>
        /// Source index
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// Source name
        /// </summary>
        public string? Name { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">Source ID</param>
        /// <param name="source">Source name, optional</param>
        public Source(int id, string? source = null)
        {
            Index = id;
            Name = source;
        }

        #region Cloning

        /// <summary>
        /// Clone the current object
        /// </summary>
        public object Clone()
        {
            return new Source(Index, Name);
        }

        #endregion
    }
}
