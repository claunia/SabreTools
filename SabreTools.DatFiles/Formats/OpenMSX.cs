namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents an openMSX softawre list XML DAT
    /// </summary>
    internal partial class OpenMSX : DatFile
    {
        /// <summary>
        /// DTD for original openMSX DATs
        /// </summary>
        private const string OpenMSXDTD = @"<!ELEMENT softwaredb (person*)>
<!ELEMENT software (title, genmsxid?, system, company,year,country,dump)>
<!ELEMENT title (#PCDATA)>
<!ELEMENT genmsxid (#PCDATA)>
<!ELEMENT system (#PCDATA)>
<!ELEMENT company (#PCDATA)>
<!ELEMENT year (#PCDATA)>
<!ELEMENT country (#PCDATA)>
<!ELEMENT dump (#PCDATA)>
";

        /// <summary>
        /// Constructor designed for casting a base DatFile
        /// </summary>
        /// <param name="datFile">Parent DatFile to copy from</param>
        public OpenMSX(DatFile datFile)
            : base(datFile)
        {
        }
    }
}
