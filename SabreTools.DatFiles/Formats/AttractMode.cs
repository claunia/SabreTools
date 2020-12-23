using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using SabreTools.Core;
using SabreTools.DatItems;
using SabreTools.IO;
using SabreTools.IO.Readers;
using SabreTools.IO.Writers;

namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents parsing and writing of an AttractMode DAT
    /// </summary>
    internal class AttractMode : DatFile
    {
        /// <summary>
        /// Constructor designed for casting a base DatFile
        /// </summary>
        /// <param name="datFile">Parent DatFile to copy from</param>
        public AttractMode(DatFile datFile)
            : base(datFile)
        {
        }

        /// <inheritdoc/>
        public override void ParseFile(string filename, int indexId, bool keep, bool statsOnly = false, bool throwOnError = false)
        {
            // Open a file reader
            Encoding enc = filename.GetEncoding();
            SeparatedValueReader svr = new SeparatedValueReader(File.OpenRead(filename), enc)
            {
                Header = true,
                Quotes = false,
                Separator = ';',
                VerifyFieldCount = true
            };

            // If we're somehow at the end of the stream already, we can't do anything
            if (svr.EndOfStream)
                return;

            // Read in the header
            svr.ReadHeader();

            // Header values should match
            // #Name;Title;Emulator;CloneOf;Year;Manufacturer;Category;Players;Rotation;Control;Status;DisplayCount;DisplayType;AltRomname;AltTitle;Extra;Buttons

            // Loop through all of the data lines
            while (!svr.EndOfStream)
            {
                try
                {
                    // Get the current line, split and parse
                    svr.ReadNextLine();

                    Rom rom = new Rom
                    {
                        Name = "-",
                        Size = Constants.SizeZero,
                        CRC = Constants.CRCZero,
                        MD5 = Constants.MD5Zero,
                        SHA1 = Constants.SHA1Zero,
                        ItemStatus = ItemStatus.None,

                        Machine = new Machine
                        {
                            Name = svr.Line[0], // #Name
                            Description = svr.Line[1], // Title
                            CloneOf = svr.Line[3], // CloneOf
                            Year = svr.Line[4], // Year
                            Manufacturer = svr.Line[5], // Manufacturer
                            Category = svr.Line[6], // Category
                            Players = svr.Line[7], // Players
                            Rotation = svr.Line[8], // Rotation
                            Control = svr.Line[9], // Control
                            Status = svr.Line[10], // Status
                            DisplayCount = svr.Line[11], // DisplayCount
                            DisplayType = svr.Line[12], // DisplayType
                            Comment = svr.Line[15], // Extra
                            Buttons = svr.Line[16], // Buttons
                        },

                        AltName = svr.Line[13], // AltRomname
                        AltTitle = svr.Line[14], // AltTitle

                        Source = new Source
                        {
                            Index = indexId,
                            Name = filename,
                        },
                    };

                    // Now process and add the rom
                    ParseAddHelper(rom, statsOnly);
                }
                catch (Exception ex)
                {
                    string message = $"'{filename}' - There was an error parsing line {svr.LineNumber} '{svr.CurrentLine}'";
                    logger.Error(ex, message);
                    if (throwOnError)
                    {
                        svr.Dispose();
                        throw new Exception(message, ex);
                    }
                }
            }

            svr.Dispose();
        }

        /// <inheritdoc/>
        protected override ItemType[] GetSupportedTypes()
        {
            return new ItemType[] { ItemType.Rom };
        }

        /// <inheritdoc/>
        public override bool WriteToFile(string outfile, bool ignoreblanks = false, bool throwOnError = false)
        {
            try
            {
                logger.User($"Opening file for writing: {outfile}");
                FileStream fs = File.Create(outfile);

                // If we get back null for some reason, just log and return
                if (fs == null)
                {
                    logger.Warning($"File '{outfile}' could not be created for writing! Please check to see if the file is writable");
                    return false;
                }

                SeparatedValueWriter svw = new SeparatedValueWriter(fs, new UTF8Encoding(false))
                {
                    Quotes = false,
                    Separator = ';',
                    VerifyFieldCount = true
                };

                // Write out the header
                WriteHeader(svw);

                // Use a sorted list of games to output
                foreach (string key in Items.SortedKeys)
                {
                    List<DatItem> datItems = Items.FilteredItems(key);

                    // If this machine doesn't contain any writable items, skip
                    if (!ContainsWritable(datItems))
                        continue;

                    // Resolve the names in the block
                    datItems = DatItem.ResolveNames(datItems);

                    for (int index = 0; index < datItems.Count; index++)
                    {
                        DatItem datItem = datItems[index];

                        // Check for a "null" item
                        datItem = ProcessNullifiedItem(datItem);

                        // Write out the item if we're not ignoring
                        if (!ShouldIgnore(datItem, ignoreblanks))
                            WriteDatItem(svw, datItem);
                    }
                }

                logger.Verbose($"File written!{Environment.NewLine}");
                svw.Dispose();
                fs.Dispose();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                if (throwOnError) throw ex;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Write out DAT header using the supplied StreamWriter
        /// </summary>
        /// <param name="svw">SeparatedValueWriter to output to</param>
        private void WriteHeader(SeparatedValueWriter svw)
        {
            string[] headers = new string[]
            {
                "#Name",
                "Title",
                "Emulator",
                "CloneOf",
                "Year",
                "Manufacturer",
                "Category",
                "Players",
                "Rotation",
                "Control",
                "Status",
                "DisplayCount",
                "DisplayType",
                "AltRomname",
                "AltTitle",
                "Extra",
                "Buttons",
            };

            svw.WriteHeader(headers);

            svw.Flush();
        }

        /// <summary>
        /// Write out Game start using the supplied StreamWriter
        /// </summary>
        /// <param name="svw">SeparatedValueWriter to output to</param>
        /// <param name="datItem">DatItem object to be output</param>
        /// <param name="throwOnError">True if the error that is thrown should be thrown back to the caller, false otherwise</param>
        private void WriteDatItem(SeparatedValueWriter svw, DatItem datItem)
        {
            // No game should start with a path separator
            datItem.Machine.Name = datItem.Machine.Name.TrimStart(Path.DirectorySeparatorChar);

            // Pre-process the item name
            ProcessItemName(datItem, true);

            // Build the state
            switch (datItem.ItemType)
            {
                case ItemType.Rom:
                    var rom = datItem as Rom;
                    string[] fields = new string[]
                    {
                            rom.Machine.Name,
                            rom.Machine.Description,
                            Header.FileName,
                            rom.Machine.CloneOf,
                            rom.Machine.Year,
                            rom.Machine.Manufacturer,
                            rom.Machine.Category,
                            rom.Machine.Players,
                            rom.Machine.Rotation,
                            rom.Machine.Control,
                            rom.ItemStatus.ToString(),
                            rom.Machine.DisplayCount,
                            rom.Machine.DisplayType,
                            rom.AltName,
                            rom.AltTitle,
                            rom.Machine.Comment,
                            rom.Machine.Buttons,
                    };

                    svw.WriteValues(fields);
                    break;
            }

            svw.Flush();
        }
    }
}
