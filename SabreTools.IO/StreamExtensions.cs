using System.IO;

namespace SabreTools.IO
{
    /// <summary>
    /// Extensions to Stream functionality
    /// </summary>
    public static class StreamExtensions
    {
        /// <summary>
        /// Seek to a specific point in the stream, if possible
        /// </summary>
        /// <param name="input">Input stream to try seeking on</param>
        /// <param name="offset">Optional offset to seek to</param>
        public static long SeekIfPossible(this Stream input, long offset = 0)
        {
            try
            {
                if (input.CanSeek)
                {
                    if (offset < 0)
                        return input.Seek(offset, SeekOrigin.End);
                    else if (offset >= 0)
                        return input.Seek(offset, SeekOrigin.Begin);
                }

                return input.Position;
            }
            catch
            {
                return -1;
            }
        }
    }
}
