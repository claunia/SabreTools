using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

// TODO: Figure out a reasonable way of adding logging back to this
namespace SabreTools.IO
{
    /// <summary>
    /// Extensions to Directory functionality
    /// </summary>
    public static class DirectoryExtensions
    {
        /// <summary>
        /// Cleans out the temporary directory
        /// </summary>
        /// <param name="dir">Name of the directory to clean out</param>
        public static void Clean(string dir)
        {
            foreach (string file in Directory.EnumerateFiles(dir, "*", SearchOption.TopDirectoryOnly))
            {
                try
                {
                    if (File.Exists(file))
                        File.Delete(file);
                }
                catch { }
            }

            foreach (string subdir in Directory.EnumerateDirectories(dir, "*", SearchOption.TopDirectoryOnly))
            {
                try
                {
                    if (Directory.Exists(subdir))
                        Directory.Delete(subdir);
                }
                catch { }
            }
        }

        /// <summary>
        /// Ensure the output directory is a proper format and can be created
        /// </summary>
        /// <param name="dir">Directory to check</param>
        /// <param name="create">True if the directory should be created, false otherwise (default)</param>
        /// <param name="temp">True if this is a temp directory, false otherwise</param>
        /// <returns>Full path to the directory</returns>
        public static string Ensure(string dir, bool create = false, bool temp = false)
        {
            // If the output directory is invalid
            if (string.IsNullOrWhiteSpace(dir))
            {
                if (temp)
                    dir = Path.GetTempPath();
                else
                    dir = Environment.CurrentDirectory;
            }

            // Get the full path for the output directory
            dir = Path.GetFullPath(dir);

            // If we're creating the output folder, do so
            if (create)
                Directory.CreateDirectory(dir);

            return dir;
        }

        /// <summary>
        /// Retrieve a list of just directories from inputs
        /// </summary>
        /// <param name="inputs">List of strings representing directories and files</param>
        /// <param name="appendparent">True if the parent name should be included in the ParentablePath, false otherwise (default)</param>
        /// <returns>List of strings representing just directories from the inputs</returns>
        public static List<ParentablePath> GetDirectoriesOnly(List<string> inputs, bool appendparent = false)
        {
            List<ParentablePath> outputs = new List<ParentablePath>();
            for (int i = 0; i < inputs.Count; i++)
            {
                string input = inputs[i];

                // If we have a null or empty path
                if (string.IsNullOrEmpty(input))
                    continue;

                // If we have a wildcard
                string pattern = "*";
                if (input.Contains("*") || input.Contains("?"))
                {
                    pattern = Path.GetFileName(input);
                    input = input.Substring(0, input.Length - pattern.Length);
                }

                // Get the parent path in case of appending
                string parentPath;
                try
                {
                    parentPath = Path.GetFullPath(input);
                }
                catch (Exception ex)
                {
                    //LoggerImpl.Error(ex, $"An exception occurred getting the full path for '{input}'");
                    continue;
                }

                if (Directory.Exists(input))
                {
                    List<string> directories = GetDirectoriesOrdered(input, pattern);
                    foreach (string dir in directories)
                    {
                        try
                        {
                            outputs.Add(new ParentablePath(Path.GetFullPath(dir), appendparent ? parentPath : string.Empty));
                        }
                        catch (PathTooLongException ex)
                        {
                            //LoggerImpl.Warning(ex, $"The path for '{dir}' was too long");
                        }
                        catch (Exception ex)
                        {
                            //LoggerImpl.Error(ex, $"An exception occurred processing '{dir}'");
                        }
                    }
                }
            }

            return outputs;
        }

        /// <summary>
        /// Retrieve a list of directories from a directory recursively in proper order
        /// </summary>
        /// <param name="dir">Directory to parse</param>
        /// <param name="pattern">Optional pattern to search for directory names</param>
        /// <returns>List with all new files</returns>
        private static List<string> GetDirectoriesOrdered(string dir, string pattern = "*")
        {
            return GetDirectoriesOrderedHelper(dir, new List<string>(), pattern);
        }

        /// <summary>
        /// Retrieve a list of directories from a directory recursively in proper order
        /// </summary>
        /// <param name="dir">Directory to parse</param>
        /// <param name="infiles">List representing existing files</param>
        /// <param name="pattern">Optional pattern to search for directory names</param>
        /// <returns>List with all new files</returns>
        private static List<string> GetDirectoriesOrderedHelper(string dir, List<string> infiles, string pattern)
        {
            // Take care of the files in the top directory
            List<string> toadd = Directory.EnumerateDirectories(dir, pattern, SearchOption.TopDirectoryOnly).ToList();
            toadd.Sort(new NaturalComparer());
            infiles.AddRange(toadd);

            // Then recurse through and add from the directories
            foreach (string subDir in toadd)
            {
                infiles = GetDirectoriesOrderedHelper(subDir, infiles, pattern);
            }

            // Return the new list
            return infiles;
        }

        /// <summary>
        /// Retrieve a list of just files from inputs
        /// </summary>
        /// <param name="inputs">List of strings representing directories and files</param>
        /// <param name="appendparent">True if the parent name should be be included in the ParentablePath, false otherwise (default)</param>
        /// <returns>List of strings representing just files from the inputs</returns>
        public static List<ParentablePath> GetFilesOnly(List<string> inputs, bool appendparent = false)
        {
            List<ParentablePath> outputs = new List<ParentablePath>();
            for (int i = 0; i < inputs.Count; i++)
            {
                string input = inputs[i].Trim('"');

                // If we have a null or empty path
                if (string.IsNullOrEmpty(input))
                    continue;

                // If we have a wildcard
                string pattern = "*";
                if (input.Contains("*") || input.Contains("?"))
                {
                    pattern = Path.GetFileName(input);
                    input = input.Substring(0, input.Length - pattern.Length);
                }

                // Get the parent path in case of appending
                string parentPath;
                try
                {
                    parentPath = Path.GetFullPath(input);
                }
                catch (Exception ex)
                {
                    //LoggerImpl.Error(ex, $"An exception occurred getting the full path for '{input}'");
                    continue;
                }

                if (Directory.Exists(input))
                {
                    List<string> files = GetFilesOrdered(input, pattern);
                    foreach (string file in files)
                    {
                        try
                        {
                            outputs.Add(new ParentablePath(Path.GetFullPath(file), appendparent ? parentPath : string.Empty));
                        }
                        catch (PathTooLongException ex)
                        {
                            //LoggerImpl.Warning(ex, $"The path for '{file}' was too long");
                        }
                        catch (Exception ex)
                        {
                            //LoggerImpl.Error(ex, $"An exception occurred processing '{file}'");
                        }
                    }
                }
                else if (File.Exists(input))
                {
                    try
                    {
                        outputs.Add(new ParentablePath(Path.GetFullPath(input), appendparent ? parentPath : string.Empty));
                    }
                    catch (PathTooLongException ex)
                    {
                        //LoggerImpl.Warning(ex, $"The path for '{input}' was too long");
                    }
                    catch (Exception ex)
                    {
                        //LoggerImpl.Error(ex, $"An exception occurred processing '{input}'");
                    }
                }
            }

            return outputs;
        }

        /// <summary>
        /// Retrieve a list of files from a directory recursively in proper order
        /// </summary>
        /// <param name="dir">Directory to parse</param>
        /// <param name="pattern">Optional pattern to search for directory names</param>
        /// <returns>List with all new files</returns>
        public static List<string> GetFilesOrdered(string dir, string pattern = "*")
        {
            return GetFilesOrderedHelper(dir, new List<string>(), pattern);
        }

        /// <summary>
        /// Retrieve a list of files from a directory recursively in proper order
        /// </summary>
        /// <param name="dir">Directory to parse</param>
        /// <param name="infiles">List representing existing files</param>
        /// <param name="pattern">Optional pattern to search for directory names</param>
        /// <returns>List with all new files</returns>
        private static List<string> GetFilesOrderedHelper(string dir, List<string> infiles, string pattern)
        {
            // Take care of the files in the top directory
            List<string> toadd = Directory.EnumerateFiles(dir, pattern, SearchOption.TopDirectoryOnly).ToList();
            toadd.Sort(new NaturalComparer());
            infiles.AddRange(toadd);

            // Then recurse through and add from the directories
            List<string> subDirs = Directory.EnumerateDirectories(dir, pattern, SearchOption.TopDirectoryOnly).ToList();
            subDirs.Sort(new NaturalComparer());
            foreach (string subdir in subDirs)
            {
                infiles = GetFilesOrderedHelper(subdir, infiles, pattern);
            }

            // Return the new list
            return infiles;
        }

        /// <summary>
        /// Get all empty folders within a root folder
        /// </summary>
        /// <param name="root">Root directory to parse</param>
        /// <returns>IEumerable containing all directories that are empty, an empty enumerable if the root is empty, null otherwise</returns>
        public static List<string> ListEmpty(string root)
        {
            // Check if the root exists first
            if (!Directory.Exists(root))
                return null;

            // If it does and it is empty, return a blank enumerable
            if (Directory.EnumerateFileSystemEntries(root, "*", SearchOption.AllDirectories).Count() == 0)
                return new List<string>();

            // Otherwise, get the complete list
            return Directory.EnumerateDirectories(root, "*", SearchOption.AllDirectories)
                .Where(dir => Directory.EnumerateFileSystemEntries(dir, "*", SearchOption.AllDirectories).Count() == 0)
                .ToList();
        }

        // TODO: Remove this entire section once External and the rest of IO is in its own library (or pulled in otherwise)
        #region TEMPORARY - REMOVEME

        private class NaturalComparer : Comparer<string>, IDisposable
        {
            private Dictionary<string, string[]> table;

            public NaturalComparer()
            {
                table = new Dictionary<string, string[]>();
            }

            public void Dispose()
            {
                table.Clear();
                table = null;
            }

            public override int Compare(string x, string y)
            {
                if (x.ToLowerInvariant() == y.ToLowerInvariant())
                {
                    return x.CompareTo(y);
                }
                if (!table.TryGetValue(x, out string[] x1))
                {
                    //x1 = Regex.Split(x.Replace(" ", string.Empty), "([0-9]+)");
                    x1 = System.Text.RegularExpressions.Regex.Split(x.ToLowerInvariant(), "([0-9]+)").Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
                    table.Add(x, x1);
                }
                if (!table.TryGetValue(y, out string[] y1))
                {
                    //y1 = Regex.Split(y.Replace(" ", string.Empty), "([0-9]+)");
                    y1 = System.Text.RegularExpressions.Regex.Split(y.ToLowerInvariant(), "([0-9]+)").Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
                    table.Add(y, y1);
                }

                for (int i = 0; i < x1.Length && i < y1.Length; i++)
                {
                    if (x1[i] != y1[i])
                    {
                        return PartCompare(x1[i], y1[i]);
                    }
                }
                if (y1.Length > x1.Length)
                {
                    return 1;
                }
                else if (x1.Length > y1.Length)
                {
                    return -1;
                }
                else
                {
                    return x.CompareTo(y);
                }
            }

            private static int PartCompare(string left, string right)
            {
                if (!long.TryParse(left, out long x))
                {
                    return CompareNumeric(left, right);
                }

                if (!long.TryParse(right, out long y))
                {
                    return CompareNumeric(left, right);
                }

                // If we have an equal part, then make sure that "longer" ones are taken into account
                if (x.CompareTo(y) == 0)
                {
                    return left.Length - right.Length;
                }

                return x.CompareTo(y);
            }

            private static int CompareNumeric(string s1, string s2)
            {
                // Save the orginal strings, for later comparison
                string s1orig = s1;
                string s2orig = s2;

                // We want to normalize the strings, so we set both to lower case
                s1 = s1.ToLowerInvariant();
                s2 = s2.ToLowerInvariant();

                // If the strings are the same exactly, return
                if (s1 == s2)
                    return s1orig.CompareTo(s2orig);

                // If one is null, then say that's less than
                if (s1 == null)
                    return -1;
                if (s2 == null)
                    return 1;

                // Now split into path parts after converting AltDirSeparator to DirSeparator
                s1 = s1.Replace(System.IO.Path.AltDirectorySeparatorChar, System.IO.Path.DirectorySeparatorChar);
                s2 = s2.Replace(System.IO.Path.AltDirectorySeparatorChar, System.IO.Path.DirectorySeparatorChar);
                string[] s1parts = s1.Split(System.IO.Path.DirectorySeparatorChar);
                string[] s2parts = s2.Split(System.IO.Path.DirectorySeparatorChar);

                // Then compare each part in turn
                for (int j = 0; j < s1parts.Length && j < s2parts.Length; j++)
                {
                    int compared = CompareNumericPart(s1parts[j], s2parts[j]);
                    if (compared != 0)
                        return compared;
                }

                // If we got out here, then it looped through at least one of the strings
                if (s1parts.Length > s2parts.Length)
                    return 1;
                if (s1parts.Length < s2parts.Length)
                    return -1;

                return s1orig.CompareTo(s2orig);
            }

            private static int CompareNumericPart(string s1, string s2)
            {
                // Otherwise, loop through until we have an answer
                for (int i = 0; i < s1.Length && i < s2.Length; i++)
                {
                    int s1c = s1[i];
                    int s2c = s2[i];

                    // If the characters are the same, continue
                    if (s1c == s2c)
                        continue;

                    // If they're different, check which one was larger
                    if (s1c > s2c)
                        return 1;
                    if (s1c < s2c)
                        return -1;
                }

                // If we got out here, then it looped through at least one of the strings
                if (s1.Length > s2.Length)
                    return 1;
                if (s1.Length < s2.Length)
                    return -1;

                return 0;
            }
        }

        #endregion
    }
}
