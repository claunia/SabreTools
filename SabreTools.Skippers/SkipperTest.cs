using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Skippers
{
    /// <summary>
    /// Individual test that applies to a SkipperRule
    /// </summary>
    public abstract class SkipperTest
    {
        #region Fields

        /// <summary>
        /// File offset to run the test
        /// </summary>
        /// <remarks>null is EOF</remarks>
        [XmlAttribute("offset")]
        public long? Offset { get; set; }

        /// <summary>
        /// Static value to be checked at the offset
        /// </summary>
        [XmlAttribute("value")]
        public byte[] Value { get; set; }

        /// <summary>
        /// Determines whether a pass or failure is expected
        /// </summary>
        [XmlAttribute("result")]
        public bool Result { get; set; }

        /// <summary>
        /// Byte mask to be applied to the tested bytes
        /// </summary>
        [XmlAttribute("mask")]
        public byte[] Mask { get; set; }

        /// <summary>
        /// Expected size of the input byte array, used with the Operator
        /// </summary>
        [XmlAttribute("size")]
        public long? Size { get; set; } // null is PO2, "power of 2" filesize

        /// <summary>
        /// Expected range value for the input byte array size, used with Size
        /// </summary>
        [XmlAttribute("operator")]
        public HeaderSkipTestFileOperator Operator { get; set; }

        #endregion

        /// <summary>
        /// Check if a stream passes the test
        /// </summary>
        /// <param name="input">Stream to check rule against</param>
        /// <remarks>The Stream is assumed to be in the proper position for a given test</remarks>
        public abstract bool Passes(Stream input);

        #region Checking Helpers

        /// <summary>
        /// Seek an input stream based on the test value
        /// </summary>
        /// <param name="input">Stream to seek</param>
        /// <returns>True if the stream could seek, false on error</returns>
        protected bool Seek(Stream input)
        {
            try
            {
                // Null offset means EOF
                if (Offset == null)
                    input.Seek(0, SeekOrigin.End);

                // Positive offset means from beginning
                else if (Offset >= 0 && Offset <= input.Length)
                    input.Seek(Offset.Value, SeekOrigin.Begin);

                // Negative offset means from end
                else if (Offset < 0 && Math.Abs(Offset.Value) <= input.Length)
                    input.Seek(Offset.Value, SeekOrigin.End);

                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }

    /// <summary>
    /// Skipper test using AND
    /// </summary>
    [XmlType("and")]
    public class AndSkipperTest : SkipperTest
    {
        /// <inheritdoc/>
        public override bool Passes(Stream input)
        {
            // First seek to the correct position
            Seek(input);

            bool result = true;
            try
            {
                // Then apply the mask if it exists
                byte[] read = new byte[Mask.Length];
                input.Read(read, 0, Mask.Length);

                byte[] masked = new byte[Mask.Length];
                for (int i = 0; i < read.Length; i++)
                {
                    masked[i] = (byte)(read[i] & Mask[i]);
                }

                // Finally, compare it against the value
                for (int i = 0; i < Value.Length; i++)
                {
                    if (masked[i] != Value[i])
                    {
                        result = false;
                        break;
                    }
                }
            }
            catch
            {
                result = false;
            }

            // Return if the expected and actual results match
            return result == Result;
        }
    }

    /// <summary>
    /// Skipper test using DATA
    /// </summary>
    [XmlType("data")]
    public class DataSkipperTest : SkipperTest
    {
        /// <inheritdoc/>
        public override bool Passes(Stream input)
        {
            // First seek to the correct position
            if (!Seek(input))
                return false;

            // Then read and compare bytewise
            bool result = true;
            for (int i = 0; i < Value.Length; i++)
            {
                try
                {
                    if (input.ReadByte() != Value[i])
                    {
                        result = false;
                        break;
                    }
                }
                catch
                {
                    result = false;
                    break;
                }
            }

            // Return if the expected and actual results match
            return result == Result;
        }
    }

    /// <summary>
    /// Skipper test using FILE
    /// </summary>
    [XmlType("file")]
    public class FileSkipperTest : SkipperTest
    {
        /// <inheritdoc/>
        public override bool Passes(Stream input)
        {
            // First get the file size from stream
            long size = input.Length;

            // If we have a null size, check that the size is a power of 2
            bool result = true;
            if (Size == null)
            {
                // http://stackoverflow.com/questions/600293/how-to-check-if-a-number-is-a-power-of-2
                result = (((ulong)size & ((ulong)size - 1)) == 0);
            }
            else if (Operator == HeaderSkipTestFileOperator.Less)
            {
                result = (size < Size);
            }
            else if (Operator == HeaderSkipTestFileOperator.Greater)
            {
                result = (size > Size);
            }
            else if (Operator == HeaderSkipTestFileOperator.Equal)
            {
                result = (size == Size);
            }

            // Return if the expected and actual results match
            return result == Result;
        }
    }

    /// <summary>
    /// Skipper test using OR
    /// </summary>
    [XmlType("or")]
    public class OrSkipperTest : SkipperTest
    {
        /// <inheritdoc/>
        public override bool Passes(Stream input)
        {
            // First seek to the correct position
            Seek(input);

            bool result = true;
            try
            {
                // Then apply the mask if it exists
                byte[] read = new byte[Mask.Length];
                input.Read(read, 0, Mask.Length);

                byte[] masked = new byte[Mask.Length];
                for (int i = 0; i < read.Length; i++)
                {
                    masked[i] = (byte)(read[i] | Mask[i]);
                }

                // Finally, compare it against the value
                for (int i = 0; i < Value.Length; i++)
                {
                    if (masked[i] != Value[i])
                    {
                        result = false;
                        break;
                    }
                }
            }
            catch
            {
                result = false;
            }

            // Return if the expected and actual results match
            return result == Result;
        }
    }

    /// <summary>
    /// Skipper test using XOR
    /// </summary>
    [XmlType("xor")]
    public class XorSkipperTest : SkipperTest
    {
        /// <inheritdoc/>
        public override bool Passes(Stream input)
        {
            // First seek to the correct position
            Seek(input);

            bool result = true;
            try
            {
                // Then apply the mask if it exists
                byte[] read = new byte[Mask.Length];
                input.Read(read, 0, Mask.Length);

                byte[] masked = new byte[Mask.Length];
                for (int i = 0; i < read.Length; i++)
                {
                    masked[i] = (byte)(read[i] ^ Mask[i]);
                }

                // Finally, compare it against the value
                for (int i = 0; i < Value.Length; i++)
                {
                    if (masked[i] != Value[i])
                    {
                        result = false;
                        break;
                    }
                }
            }
            catch
            {
                result = false;
            }

            // Return if the expected and actual results match
            return result == Result;
        }
    }
}