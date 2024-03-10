using System;
using SabreTools.Core;
using SabreTools.Core.Tools;

namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents parsing a Logiqx-derived DAT
    /// </summary>
    internal partial class Logiqx : DatFile
    {
        /// <inheritdoc/>
        public override void ParseFile(string filename, int indexId, bool keep, bool statsOnly = false, bool throwOnError = false)
        {
            try
            {
                // Deserialize the input file
                var metadataFile = new Serialization.Files.Logiqx().Deserialize(filename);
                var metadata = new Serialization.CrossModel.Logiqx().Serialize(metadataFile);

                // Convert the header to the internal format
                ConvertHeader(metadataFile, keep);

                // Convert to the internal format
                ConvertMetadata(metadata, filename, indexId, statsOnly);
            }
            catch (Exception ex) when (!throwOnError)
            {
                string message = $"'{filename}' - An error occurred during parsing";
                logger.Error(ex, message);
            }
        }

        #region Converters

        /// <summary>
        /// Convert header information
        /// </summary>
        /// <param name="datafile">Deserialized model to convert</param>
        /// <param name="keep">True if full pathnames are to be kept, false otherwise (default)</param>
        private void ConvertHeader(Models.Logiqx.Datafile? datafile, bool keep)
        {
            // If the datafile is missing, we can't do anything
            if (datafile == null)
                return;

            Header.Build ??= datafile.Build;
            Header.Debug ??= datafile.Debug.AsYesNo();
            // SchemaLocation is specifically skipped

            ConvertHeader(datafile.Header, keep);
        }

        /// <summary>
        /// Convert header information
        /// </summary>
        /// <param name="header">Deserialized model to convert</param>
        /// <param name="keep">True if full pathnames are to be kept, false otherwise (default)</param>
        private void ConvertHeader(Models.Logiqx.Header? header, bool keep)
        {
            // If the header is missing, we can't do anything
            if (header == null)
                return;

            Header.NoIntroID ??= header.Id;
            Header.Name ??= header.Name;
            Header.Description ??= header.Description;
            Header.RootDir ??= header.RootDir;
            Header.Category ??= header.Category;
            Header.Version ??= header.Version;
            Header.Date ??= header.Date;
            Header.Author ??= header.Author;
            Header.Email ??= header.Email;
            Header.Homepage ??= header.Homepage;
            Header.Url ??= header.Url;
            Header.Comment ??= header.Comment;
            Header.Type ??= header.Type;

            ConvertSubheader(header.ClrMamePro);
            ConvertSubheader(header.RomCenter);

            // Handle implied SuperDAT
            if (header.Name?.Contains(" - SuperDAT") == true && keep)
                Header.Type ??= "SuperDAT";
        }

        /// <summary>
        /// Convert subheader information
        /// </summary>
        /// <param name="clrMamePro">Deserialized model to convert</param>
        private void ConvertSubheader(Models.Logiqx.ClrMamePro? clrMamePro)
        {
            // If the subheader is missing, we can't do anything
            if (clrMamePro == null)
                return;

            Header.HeaderSkipper ??= clrMamePro.Header;

            if (Header.ForceMerging == MergingFlag.None)
                Header.ForceMerging = clrMamePro.ForceMerging.AsEnumValue<MergingFlag>();
            if (Header.ForceNodump == NodumpFlag.None)
                Header.ForceNodump = clrMamePro.ForceNodump.AsEnumValue<NodumpFlag>();
            if (Header.ForcePacking == PackingFlag.None)
                Header.ForcePacking = clrMamePro.ForcePacking.AsEnumValue<PackingFlag>();
        }

        /// <summary>
        /// Convert subheader information
        /// </summary>
        /// <param name="romCenter">Deserialized model to convert</param>
        private void ConvertSubheader(Models.Logiqx.RomCenter? romCenter)
        {
            // If the subheader is missing, we can't do anything
            if (romCenter == null)
                return;

            Header.System ??= romCenter.Plugin;

            if (Header.RomMode == MergingFlag.None)
                Header.RomMode = romCenter.RomMode.AsEnumValue<MergingFlag>();
            if (Header.BiosMode == MergingFlag.None)
                Header.BiosMode = romCenter.BiosMode.AsEnumValue<MergingFlag>();
            if (Header.SampleMode == MergingFlag.None)
                Header.SampleMode = romCenter.SampleMode.AsEnumValue<MergingFlag>();

            Header.LockRomMode ??= romCenter.LockRomMode.AsYesNo();
            Header.LockBiosMode ??= romCenter.LockBiosMode.AsYesNo();
            Header.LockSampleMode ??= romCenter.LockSampleMode.AsYesNo();
        }

        #endregion
    }
}
