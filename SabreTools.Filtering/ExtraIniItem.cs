using System;
using System.Collections.Generic;

using SabreTools.Core;
using SabreTools.IO.Readers;
using SabreTools.Logging;

namespace SabreTools.Filtering
{
    public class ExtraIniItem
    {
        #region Fields

        /// <summary>
        /// MachineField to update with INI information
        /// </summary>
        public MachineField MachineField { get; set; } = MachineField.NULL;

        /// <summary>
        /// DatItemField to update with INI information
        /// </summary>
        public DatItemField DatItemField { get; set; } = DatItemField.NULL;

        /// <summary>
        /// Mappings from machine names to value
        /// </summary>
        public Dictionary<string, string> Mappings { get; set; } = new Dictionary<string, string>();

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
            IniReader ir = new IniReader(ini) { ValidateRows = false };
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
                        // Get the value and machine name
                        string value = ir.Section;
                        string machineName = ir.CurrentLine.Trim();

                        // If the section is "ROOT_FOLDER", then we use the value "true" instead.
                        // This is done because some INI files use the name of the file as the
                        // category to be assigned to the items included.
                        if (value == "ROOT_FOLDER")
                            value = "true";

                        // Add the new mapping
                        Mappings[machineName] = value;
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerImpl.Warning(ex, $"Exception found while parsing '{ini}'");
                return false;
            }

            ir.Dispose();
            return true;
        }

        #endregion
    }
}
