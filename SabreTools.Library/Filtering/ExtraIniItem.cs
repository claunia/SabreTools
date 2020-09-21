using System;
using System.Collections.Generic;

using SabreTools.Library.Data;
using SabreTools.Library.DatItems;
using SabreTools.Library.IO;

namespace SabreTools.Library.Filtering
{
    public class ExtraIniItem
    {
        #region Fields

        /// <summary>
        /// Field to update with INI information
        /// </summary>
        public Field Field { get; set; }

        /// <summary>
        /// Mappings from value to machine name
        /// </summary>
        public Dictionary<string, List<string>> Mappings { get; set; } = new Dictionary<string, List<string>>();

        #endregion

        #region Extras Population

        /// <summary>
        /// Populate the dictionary from an INI file
        /// </summary>
        /// <param name="ini">Path to INI file to populate from</param>
        /// <remarks>
        /// The INI file format that is supported here is not exactly the same
        /// as a traditional one. This expects a MAME extras format, which usually
        /// doesn't contain key value pairs and always at least contains one section
        /// called `ROOT_FOLDER`. If that's the name of a section, then we assume
        /// the value is boolean. If there's another section name, then that is set
        /// as the value instead.
        /// </remarks>
        public bool PopulateFromFile(string ini)
        {
            // Prepare all intenral variables
            IniReader ir = ini.GetIniReader(false);
            bool foundRootFolder = false;

            // If we got a null reader, just return
            if (ir == null)
                return false;

            // Otherwise, read the file to the end
            try
            {
                while (!ir.EndOfStream)
                {
                    // Read in the next line and process
                    ir.ReadNextLine();

                    // We don't care about whitespace or comments
                    if (ir.RowType == IniRowType.None || ir.RowType == IniRowType.Comment)
                        continue;

                    // If we have a section, just read it in
                    if (ir.RowType == IniRowType.SectionHeader)
                    {
                        // If we've found the start of the extras, set the flag
                        if (string.Equals(ir.Section, "ROOT_FOLDER", StringComparison.OrdinalIgnoreCase))
                            foundRootFolder = true;

                        continue;
                    }

                    // If we have a value, then we start populating the dictionary
                    else if (foundRootFolder)
                    {
                        // Get the key and value
                        string key = ir.Section;
                        string value = ir.CurrentLine.Trim();

                        // If the section is "ROOT_FOLDER", then we use the value "true" instead.
                        // This is done because some INI files use the name of the file as the
                        // category to be assigned to the items included.
                        if (key == "ROOT_FOLDER")
                            key = "true";

                        // Ensure the key exists
                        if (!Mappings.ContainsKey(key))
                            Mappings[key] = new List<string>();

                        // Add the new mapping
                        Mappings[key].Add(value);
                    }
                }
            }
            catch (Exception ex)
            {
                Globals.Logger.Warning(ex, $"Exception found while parsing '{ini}'");
                return false;
            }

            ir.Dispose();
            return true;
        }

        #endregion
    }
}
