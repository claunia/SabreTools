using System;

namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents parsing a Missfile
    /// </summary>
    internal partial class Missfile : DatFile
    {
        /// <inheritdoc/>
        /// <remarks>There is no consistent way to parse a missfile</remarks>
        public override void ParseFile(string filename, int indexId, bool keep, bool statsOnly = false, bool throwOnError = false)
        {
            throw new NotImplementedException();
        }
    }
}
