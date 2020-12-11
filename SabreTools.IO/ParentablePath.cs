using System;
using System.IO;

namespace SabreTools.IO
{
    /// <summary>
    /// A path that optionally contains a parent root
    /// </summary>
    public class ParentablePath
    {
        /// <summary>
        /// Current full path represented
        /// </summary>
        public string CurrentPath { get; private set; }

        /// <summary>
        /// Possible parent path represented (may be null or empty)
        /// </summary>
        public string ParentPath { get; private set; }

        public ParentablePath(string currentPath, string parentPath = null)
        {
            CurrentPath = currentPath;
            ParentPath = parentPath;
        }

        /// <summary>
        /// Get the proper filename (with subpath) from the file and parent combination
        /// </summary>
        /// <param name="sanitize">True if path separators should be converted to '-', false otherwise</param>
        /// <returns>Subpath for the file</returns>
        public string GetNormalizedFileName(bool sanitize)
        {
            // Check that we have a combined path first
            if (string.IsNullOrWhiteSpace(ParentPath))
            {
                string filename = Path.GetFileName(CurrentPath);
                if (sanitize)
                    filename = filename.Replace(Path.DirectorySeparatorChar, '-').Replace(Path.AltDirectorySeparatorChar, '-');

                return filename;
            }

            // If the parts are the same, return the filename from the first part
            if (string.Equals(CurrentPath, ParentPath, StringComparison.Ordinal))
            {
                string filename = Path.GetFileName(CurrentPath);
                if (sanitize)
                    filename = filename.Replace(Path.DirectorySeparatorChar, '-').Replace(Path.AltDirectorySeparatorChar, '-');

                return filename;
            }

            // Otherwise, remove the path.ParentPath from the path.CurrentPath and return the remainder
            else
            {
                string filename = CurrentPath.Remove(0, ParentPath.Length + 1);
                if (sanitize)
                    filename = filename.Replace(Path.DirectorySeparatorChar, '-').Replace(Path.AltDirectorySeparatorChar, '-');

                return filename;
            }
        }

        /// <summary>
        /// Get the proper output path for a given input file and output directory
        /// </summary>
        /// <param name="outDir">Output directory to use</param>
        /// <param name="inplace">True if the output file should go to the same input folder, false otherwise</param>
        /// <returns>Complete output path</returns>
        public string GetOutputPath(string outDir, bool inplace)
        {
            // First, we need to ensure the output directory
            outDir = outDir.Ensure();

            // Check if we have a split path or not
            bool splitpath = !string.IsNullOrWhiteSpace(ParentPath);

            // If we have a split path, we need to treat the input separately
            if (splitpath)
            {
                // If we have an inplace output, use the directory name from the input path
                if (inplace)
                {
                    outDir = Path.GetDirectoryName(CurrentPath);
                }

                // TODO: Should this be the default? Always create a subfolder if a folder is found?
                // If we are processing a path that is coming from a directory and we are outputting to the current directory, we want to get the subfolder to write to
                else if (CurrentPath.Length != ParentPath.Length && outDir == Environment.CurrentDirectory)
                {
                    outDir = Path.GetDirectoryName(Path.Combine(outDir, CurrentPath.Remove(0, Path.GetDirectoryName(ParentPath).Length + 1)));
                }

                // If we are processing a path that is coming from a directory, we want to get the subfolder to write to
                else if (CurrentPath.Length != ParentPath.Length)
                {
                    outDir = Path.GetDirectoryName(Path.Combine(outDir, CurrentPath.Remove(0, ParentPath.Length + 1)));
                }

                // If we are processing a single file from the root of a directory, we just use the output directory
                else
                {
                    // No-op
                }
            }
            // Otherwise, assume the input path is just a filename
            else
            {
                // If we have an inplace output, use the directory name from the input path
                if (inplace)
                {
                    outDir = Path.GetDirectoryName(CurrentPath);
                }

                // Otherwise, just use the supplied output directory
                else
                {
                    // No-op
                }
            }

            // Finally, return the output directory
            return outDir;
        }
    }
}
