using System;
using System.IO;
using System.Linq;

using SabreTools.Logging;

namespace SabreTools.IO
{
    /// <summary>
    /// Extensions to Stream functionality
    /// </summary>
    public static class StreamExtensions
    {
        /// <summary>
        /// Add an aribtrary number of bytes to the inputted stream
        /// </summary>
        /// <param name="input">Stream to be appended to</param>
        /// <param name="output">Outputted stream</param>
        /// <param name="bytesToAddToHead">Bytes to be added to head of stream</param>
        /// <param name="bytesToAddToTail">Bytes to be added to tail of stream</param>
        public static void AppendBytes(Stream input, Stream output, byte[] bytesToAddToHead, byte[] bytesToAddToTail)
        {
            // Write out prepended bytes
            if (bytesToAddToHead != null && bytesToAddToHead.Count() > 0)
                output.Write(bytesToAddToHead, 0, bytesToAddToHead.Length);

            // Now copy the existing file over
            input.CopyTo(output);

            // Write out appended bytes
            if (bytesToAddToTail != null && bytesToAddToTail.Count() > 0)
                output.Write(bytesToAddToTail, 0, bytesToAddToTail.Length);
        }

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
            catch (NotSupportedException ex)
            {
                LoggerImpl.Verbose(ex, "Stream does not support seeking to starting offset. Stream position not changed");
            }
            catch (NotImplementedException ex)
            {
                LoggerImpl.Warning(ex, "Stream does not support seeking to starting offset. Stream position not changed");
            }

            return -1;
        }
    }
}
