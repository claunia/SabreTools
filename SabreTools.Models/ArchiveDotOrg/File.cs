using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Models.ArchiveDotOrg
{
    [XmlRoot("file")]
    public class File
    {
        [XmlAttribute("name")]
        public string? Name { get; set; }

        /// <remarks>Is this a set of defined values?</remarks>
        [XmlAttribute("source")]
        public string? Source { get; set; }

        [XmlElement("btih")]
        public string? BitTorrentMagnetHash { get; set; }

        [XmlElement("mtime")]
        public long? LastModifiedTime { get; set; }

        [XmlElement("size")]
        public long? Size { get; set; }

        [XmlElement("md5")]
        public string? MD5 { get; set; }

        [XmlElement("crc32")]
        public string? CRC32 { get; set; }

        [XmlElement("sha1")]
        public string? SHA1 { get; set; }

        [XmlElement("filecount")]
        public long? FileCount { get; set; }

        /// <remarks>Is this a set of defined values?</remarks>
        [XmlElement("format")]
        public string? Format { get; set; }

        [XmlElement("original")]
        public string? Original { get; set; }

        /// <remarks>Is this a set of defined values?</remarks>
        [XmlElement("summation")]
        public string? Summation { get; set; }

        /// <remarks>Is this a set of defined values?</remarks>
        [XmlElement("rotation")]
        public long? Rotation { get; set; }

        #region OCR-Related

        [XmlElement("cloth_cover_detection_module_version")]
        public string? ClothCoverDetectionModuleVersion { get; set; }

        [XmlElement("hocr_char_to_word_hocr_version")]
        public string? hOCRCharToWordhOCRVersion { get; set; }

        [XmlElement("hocr_char_to_word_module_version")]
        public string? hOCRCharToWordModuleVersion { get; set; }

        [XmlElement("hocr_fts_text_hocr_version")]
        public string? hOCRFtsTexthOCRVersion { get; set; }

        [XmlElement("hocr_fts_text_module_version")]
        public string? hOCRFtsTextModuleVersion { get; set; }

        [XmlElement("hocr_pageindex_hocr_version")]
        public string? hOCRPageIndexhOCRVersion { get; set; }

        [XmlElement("hocr_pageindex_module_version")]
        public string? hOCRPageIndexModuleVersion { get; set; }

        [XmlElement("ocr")]
        public string? TesseractOCR { get; set; }

        [XmlElement("ocr_converted")]
        public string? TesseractOCRConverted { get; set; }

        [XmlElement("ocr_detected_lang")]
        public string? TesseractOCRDetectedLang { get; set; }

        [XmlElement("ocr_detected_lang_conf")]
        public string? TesseractOCRDetectedLangConf { get; set; }

        [XmlElement("ocr_detected_script")]
        public string? TesseractOCRDetectedScript { get; set; }

        [XmlElement("ocr_detected_script_conf")]
        public string? TesseractOCRDetectedScriptConf { get; set; }

        [XmlElement("ocr_parameters")]
        public string? TesseractOCRParameters { get; set; }

        [XmlElement("ocr_module_version")]
        public string? TesseractOCRModuleVersion { get; set; }

        [XmlElement("pdf_module_version")]
        public string? PDFModuleVersion { get; set; }

        [XmlElement("word_conf_0_10")]
        public long? WordConfidenceInterval0To10 { get; set; }

        [XmlElement("word_conf_11_20")]
        public long? WordConfidenceInterval11To20 { get; set; }

        [XmlElement("word_conf_21_30")]
        public long? WordConfidenceInterval21To30 { get; set; }

        [XmlElement("word_conf_31_40")]
        public long? WordConfidenceInterval31To40 { get; set; }

        [XmlElement("word_conf_41_50")]
        public long? WordConfidenceInterval41To50 { get; set; }

        [XmlElement("word_conf_51_60")]
        public long? WordConfidenceInterval51To60 { get; set; }

        [XmlElement("word_conf_61_70")]
        public long? WordConfidenceInterval61To70 { get; set; }

        [XmlElement("word_conf_71_80")]
        public long? WordConfidenceInterval71To80 { get; set; }

        [XmlElement("word_conf_81_90")]
        public long? WordConfidenceInterval81To90 { get; set; }

        [XmlElement("word_conf_91_100")]
        public long? WordConfidenceInterval91To100 { get; set; }

        #endregion

        #region DO NOT USE IN PRODUCTION

        /// <remarks>Should be empty</remarks>
        [XmlAnyAttribute]
        public XmlAttribute[]? ADDITIONAL_ATTRIBUTES { get; set; }

        /// <remarks>Should be empty</remarks>
        [XmlAnyElement]
        public object[]? ADDITIONAL_ELEMENTS { get; set; }

        #endregion
    }
}