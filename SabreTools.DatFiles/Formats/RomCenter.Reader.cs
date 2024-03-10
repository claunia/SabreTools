using System;
using SabreTools.Core;

namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents parsing a RomCenter INI file
    /// </summary>
    internal partial class RomCenter : DatFile
    {
        /// <inheritdoc/>
        public override void ParseFile(string filename, int indexId, bool keep, bool statsOnly = false, bool throwOnError = false)
        {
            try
            {
                // Deserialize the input file
                var metadataFile = new Serialization.Files.RomCenter().Deserialize(filename);
                var metadata = new Serialization.CrossModel.RomCenter().Serialize(metadataFile);

                // Convert the credits data to the internal format
                ConvertCredits(metadataFile?.Credits);

                // Convert the dat data to the internal format
                ConvertDat(metadataFile?.Dat);

                // Convert the emulator data to the internal format
                ConvertEmulator(metadataFile?.Emulator);

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
        /// Convert credits information
        /// </summary>
        /// <param name="credits">Deserialized model to convert</param>
        private void ConvertCredits(Models.RomCenter.Credits? credits)
        {
            // If the credits is missing, we can't do anything
            if (credits == null)
                return;

            Header.Author ??= credits.Author;
            Header.Version ??= credits.Version;
            Header.Email ??= credits.Email;
            Header.Homepage ??= credits.Homepage;
            Header.Url ??= credits.Url;
            Header.Date ??= credits.Date;
            Header.Comment ??= credits.Comment;
        }

        /// <summary>
        /// Convert dat information
        /// </summary>
        /// <param name="dat">Deserialized model to convert</param>
        private void ConvertDat(Models.RomCenter.Dat? dat)
        {
            // If the dat is missing, we can't do anything
            if (dat == null)
                return;

            Header.RomCenterVersion ??= dat.Version;
            Header.System ??= dat.Plugin;
            if (Header.ForceMerging == MergingFlag.None && dat.Split == "1")
                Header.ForceMerging = MergingFlag.Split;
            if (Header.ForceMerging == MergingFlag.None && dat.Merge == "1")
                Header.ForceMerging = MergingFlag.Merged;
        }

        /// <summary>
        /// Convert emulator information
        /// </summary>
        /// <param name="games">Deserialized model to convert</param>
        private void ConvertEmulator(Models.RomCenter.Emulator? emulator)
        {
            // If the emulator is missing, we can't do anything
            if (emulator == null)
                return;

            Header.Name ??= emulator.RefName;
            Header.Description ??= emulator.Version;
        }

        #endregion
    }
}
