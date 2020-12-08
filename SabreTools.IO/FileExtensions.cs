using System.IO;
using System.Text;

namespace SabreTools.IO
{
    /// <summary>
    /// Extensions to File functionality
    /// </summary>
    public static class FileExtensions
    {
        /// <summary>
        /// Determines a text file's encoding by analyzing its byte order mark (BOM).
        /// Defaults to ASCII when detection of the text file's endianness fails.
        /// </summary>
        /// <param name="filename">The text file to analyze.</param>
        /// <returns>The detected encoding.</returns>
        /// <link>http://stackoverflow.com/questions/3825390/effective-way-to-find-any-files-encoding</link>
        public static Encoding GetEncoding(string filename)
        {
            if (!File.Exists(filename))
                return Encoding.Default;

            // Try to open the file
            try
            {
                FileStream file = File.OpenRead(filename);
                if (file == null)
                    return Encoding.Default;

                // Read the BOM
                var bom = new byte[4];
                file.Read(bom, 0, 4);
                file.Dispose();

                // Analyze the BOM
                if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) return Encoding.UTF7;
                if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;
                if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode; //UTF-16LE
                if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode; //UTF-16BE
                if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return Encoding.UTF32;
                return Encoding.Default;
            }
            catch
            {
                return Encoding.Default;
            }
        }

        /// <summary>
        /// Returns if the first byte array starts with the second array
        /// </summary>
        /// <param name="arr1">First byte array to compare</param>
        /// <param name="arr2">Second byte array to compare</param>
        /// <param name="exact">True if the input arrays should match exactly, false otherwise (default)</param>
        /// <returns>True if the first byte array starts with the second, false otherwise</returns>
        public static bool StartsWith(this byte[] arr1, byte[] arr2, bool exact = false)
        {
            // If we have any invalid inputs, we return false
            if (arr1 == null || arr2 == null
                || arr1.Length == 0 || arr2.Length == 0
                || arr2.Length > arr1.Length
                || (exact && arr1.Length != arr2.Length))
            {
                return false;
            }

            // Otherwise, loop through and see
            for (int i = 0; i < arr2.Length; i++)
            {
                if (arr1[i] != arr2[i])
                    return false;
            }

            return true;
        }
    }
}
