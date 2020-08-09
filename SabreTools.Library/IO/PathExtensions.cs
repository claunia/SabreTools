using System.IO;

using SabreTools.Library.Data;

namespace SabreTools.Library.IO
{
    /// <summary>
    /// Extensions to Path functionality
    /// </summary>
    public static class PathExtensions
    {
        /// <summary>
        /// Get the extension from the path, if possible
        /// </summary>
        /// <param name="path">Path to get extension from</param>
        /// <returns>Extension, if possible</returns>
        public static string GetNormalizedExtension(string path)
        {
            // Check null or empty first
            if (string.IsNullOrWhiteSpace(path))
                return null;

            // Get the extension from the path, if possible
            string ext = Path.GetExtension(path)?.ToLowerInvariant();

            // Check if the extension is null or empty
            if (string.IsNullOrWhiteSpace(ext))
                return null;

            // Make sure that extensions are valid
            ext = ext.TrimStart('.');

            return ext;
        }

        /// <summary>
        /// Get a proper romba sub path
        /// </summary>
        /// <param name="hash">SHA-1 hash to get the path for</param>
        /// <returns>Subfolder path for the given hash</returns>
        public static string GetRombaPath(string hash)
        {
            // If the hash isn't the right size, then we return null
            if (hash.Length != Constants.SHA1Length) // TODO: When updating to SHA-256, this needs to update to Constants.SHA256Length
                return null;

            return Path.Combine(hash.Substring(0, 2), hash.Substring(2, 2), hash.Substring(4, 2), hash.Substring(6, 2), hash + ".gz");
        }

        /// <summary>
        /// Get if the given path has a valid DAT extension
        /// </summary>
        /// <param name="path">Path to check</param>
        /// <returns>True if the extension is valid, false otherwise</returns>
        public static bool HasValidArchiveExtension(string path)
        {
            // Get the extension from the path, if possible
            string ext = GetNormalizedExtension(path);

            // Check against the list of known archive extensions
            switch (ext)
            {
                case "7z":
                case "gz":
                case "lzma":
                case "rar":
                case "rev":
                case "r00":
                case "r01":
                case "tar":
                case "tgz":
                case "tlz":
                case "zip":
                case "zipx":
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Get if the given path has a valid DAT extension
        /// </summary>
        /// <param name="path">Path to check</param>
        /// <returns>True if the extension is valid, false otherwise</returns>
        public static bool HasValidDatExtension(string path)
        {
            // Get the extension from the path, if possible
            string ext = GetNormalizedExtension(path);

            // Check against the list of known DAT extensions
            switch (ext)
            {
                case "csv":
                case "dat":
                case "json":
                case "md5":
                case "ripemd160":
                case "sfv":
                case "sha1":
                case "sha256":
                case "sha384":
                case "sha512":
                case "ssv":
                case "tsv":
                case "txt":
                case "xml":
                    return true;
                default:
                    return false;
            }
        }
    }
}
