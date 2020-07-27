namespace SabreTools.Library.Tools
{
    /// <summary>
    /// A path that optionally contains a parent root
    /// </summary>
    public class ParentablePath
    {
        public string CurrentPath { get; set; }
        public string ParentPath { get; set; }

        public ParentablePath(string currentPath, string parentPath = null)
        {
            CurrentPath = currentPath;
            ParentPath = parentPath;
        }
    }
}
