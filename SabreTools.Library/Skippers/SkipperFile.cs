using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;

using SabreTools.Library.Data;
using SabreTools.Library.Tools;

namespace SabreTools.Library.Skippers
{
    public class SkipperFile
    {
        #region Fields

        /// <summary>
        /// Skipper name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Author names
        /// </summary>
        public string Author { get; set; } = string.Empty;

        /// <summary>
        /// File version
        /// </summary>
        public string Version { get; set; } = string.Empty;

        /// <summary>
        /// Set of all rules in the skipper
        /// </summary>
        public List<SkipperRule> Rules { get; set; } = new List<SkipperRule>();

        /// <summary>
        /// Filename the skipper lives in
        /// </summary>
        public string SourceFile { get; set; } = string.Empty;

        #endregion

        #region Constructors

        /// <summary>
        /// Create an empty SkipperFile object
        /// </summary>
        public SkipperFile() { }

        /// <summary>
        /// Create a SkipperFile object parsed from an input file
        /// </summary>
        /// <param name="filename">Name of the file to parse</param>
        public SkipperFile(string filename)
        {
            Rules = new List<SkipperRule>();
            SourceFile = Path.GetFileNameWithoutExtension(filename);

            XmlReader xtr = filename.GetXmlTextReader();
            bool valid = Parse(xtr);

            // If we somehow have an invalid file, zero out the fields
            if (!valid)
            {
                Name = null;
                Author = null;
                Version = null;
                Rules = null;
                SourceFile = null;
            }
        }

        #endregion

        #region Parsing Helpers

        /// <summary>
        /// Parse an XML document in as a SkipperFile
        /// </summary>
        /// <param name="xtr">XmlReader representing the document</param>
        private bool Parse(XmlReader xtr)
        {
            if (xtr == null)
                return false;

            try
            {
                bool valid = false;
                xtr.MoveToContent();
                while (!xtr.EOF)
                {
                    if (xtr.NodeType != XmlNodeType.Element)
                        xtr.Read();

                    switch (xtr.Name.ToLowerInvariant())
                    {
                        case "detector":
                            valid = true;
                            xtr.Read();
                            break;

                        case "name":
                            Name = xtr.ReadElementContentAsString();
                            break;

                        case "author":
                            Author = xtr.ReadElementContentAsString();
                            break;

                        case "version":
                            Version = xtr.ReadElementContentAsString();
                            break;

                        case "rule":
                            SkipperRule rule = ParseRule(xtr);
                            if (rule != null)
                                Rules.Add(rule);

                            xtr.Skip();
                            break;
                        default:
                            xtr.Read();
                            break;
                    }
                }

                return valid;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Parse an XML document in as a SkipperRule
        /// </summary>
        /// <param name="xtr">XmlReader representing the document</param>
        private SkipperRule ParseRule(XmlReader xtr)
        {
            if (xtr == null)
                return null;

            try
            {
                // Get the information from the rule first
                SkipperRule rule = new SkipperRule
                {
                    StartOffset = null,
                    EndOffset = null,
                    Operation = HeaderSkipOperation.None,
                    Tests = new List<SkipperTest>(),
                    SourceFile = this.SourceFile,
                };

                if (xtr.GetAttribute("start_offset") != null)
                {
                    string offset = xtr.GetAttribute("start_offset");
                    if (offset.ToLowerInvariant() == "eof")
                    {
                        rule.StartOffset = null;
                    }
                    else
                    {
                        rule.StartOffset = Convert.ToInt64(offset, 16);
                    }
                }

                if (xtr.GetAttribute("end_offset") != null)
                {
                    string offset = xtr.GetAttribute("end_offset");
                    if (offset.ToLowerInvariant() == "eof")
                    {
                        rule.EndOffset = null;
                    }
                    else
                    {
                        rule.EndOffset = Convert.ToInt64(offset, 16);
                    }
                }

                if (xtr.GetAttribute("operation") != null)
                {
                    string operation = xtr.GetAttribute("operation");
                    switch (operation.ToLowerInvariant())
                    {
                        case "bitswap":
                            rule.Operation = HeaderSkipOperation.Bitswap;
                            break;
                        case "byteswap":
                            rule.Operation = HeaderSkipOperation.Byteswap;
                            break;
                        case "wordswap":
                            rule.Operation = HeaderSkipOperation.Wordswap;
                            break;
                    }
                }

                // Now read the individual tests into the Rule
                XmlReader subreader = xtr.ReadSubtree();

                if (subreader != null)
                {
                    while (!subreader.EOF)
                    {
                        if (subreader.NodeType != XmlNodeType.Element)
                            subreader.Read();

                        // Get the test type
                        SkipperTest test = new SkipperTest
                        {
                            Offset = 0,
                            Value = new byte[0],
                            Result = true,
                            Mask = new byte[0],
                            Size = 0,
                            Operator = HeaderSkipTestFileOperator.Equal,
                        };
                        switch (subreader.Name.ToLowerInvariant())
                        {
                            case "data":
                                test.Type = HeaderSkipTest.Data;
                                break;

                            case "or":
                                test.Type = HeaderSkipTest.Or;
                                break;

                            case "xor":
                                test.Type = HeaderSkipTest.Xor;
                                break;

                            case "and":
                                test.Type = HeaderSkipTest.And;
                                break;

                            case "file":
                                test.Type = HeaderSkipTest.File;
                                break;

                            default:
                                subreader.Read();
                                break;
                        }

                        // Now populate all the parts that we can
                        if (subreader.GetAttribute("offset") != null)
                        {
                            string offset = subreader.GetAttribute("offset");
                            if (offset.ToLowerInvariant() == "eof")
                                test.Offset = null;
                            else
                                test.Offset = Convert.ToInt64(offset, 16);
                        }

                        if (subreader.GetAttribute("value") != null)
                        {
                            string value = subreader.GetAttribute("value");

                            // http://stackoverflow.com/questions/321370/how-can-i-convert-a-hex-string-to-a-byte-array
                            test.Value = new byte[value.Length / 2];
                            for (int index = 0; index < test.Value.Length; index++)
                            {
                                string byteValue = value.Substring(index * 2, 2);
                                test.Value[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                            }
                        }

                        if (subreader.GetAttribute("result") != null)
                        {
                            string result = subreader.GetAttribute("result");
                            if (!bool.TryParse(result, out bool resultBool))
                                resultBool = true;

                            test.Result = resultBool;
                        }

                        if (subreader.GetAttribute("mask") != null)
                        {
                            string mask = subreader.GetAttribute("mask");

                            // http://stackoverflow.com/questions/321370/how-can-i-convert-a-hex-string-to-a-byte-array
                            test.Mask = new byte[mask.Length / 2];
                            for (int index = 0; index < test.Mask.Length; index++)
                            {
                                string byteValue = mask.Substring(index * 2, 2);
                                test.Mask[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                            }
                        }

                        if (subreader.GetAttribute("size") != null)
                        {
                            string size = subreader.GetAttribute("size");
                            if (size.ToLowerInvariant() == "po2")
                                test.Size = null;
                            else
                                test.Size = Convert.ToInt64(size, 16);
                        }

                        if (subreader.GetAttribute("operator") != null)
                        {
                            string oper = subreader.GetAttribute("operator");
#if NET_FRAMEWORK
                            switch (oper.ToLowerInvariant())
                            {
                                case "less":
                                    test.Operator = HeaderSkipTestFileOperator.Less;
                                    break;
                                case "greater":
                                    test.Operator = HeaderSkipTestFileOperator.Greater;
                                    break;
                                case "equal":
                                default:
                                    test.Operator = HeaderSkipTestFileOperator.Equal;
                                    break;
                            }
#else
                            test.Operator = oper.ToLowerInvariant() switch
                            {
                                "less" => HeaderSkipTestFileOperator.Less,
                                "greater" => HeaderSkipTestFileOperator.Greater,
                                "equal" => HeaderSkipTestFileOperator.Equal,
                                _ => HeaderSkipTestFileOperator.Equal,
                            };
#endif
                        }

                        // Add the created test to the rule
                        rule.Tests.Add(test);
                        subreader.Read();
                    }
                }

                return rule;
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
